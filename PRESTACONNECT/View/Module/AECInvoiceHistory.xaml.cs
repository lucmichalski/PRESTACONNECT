using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Collections.ObjectModel;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour AECInvoiceHistory.xaml
    /// </summary>
    public partial class AECInvoiceHistory : Window
    {
        ObservableCollection<Model.Local.Customer_Progress> list_progress = new ObservableCollection<Model.Local.Customer_Progress>();

        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        private DateTime? _PeriodeDebut, _PeriodeFin;
        private bool _ClearMode;

        public AECInvoiceHistory(DateTime? PeriodeDebut, DateTime? PeriodeFin, bool ClearMode = false)
        {
            this.InitializeComponent();

            this._PeriodeDebut = PeriodeDebut;
            this._PeriodeFin = PeriodeFin;
            this._ClearMode = ClearMode;

            if (this._ClearMode)
            {
                this.LabelOperation.Content = "Nettoyage des données !";
            }
            this.ProgressBar.ToolTip = "Nombre de coeurs processeurs utilisés : " + MaximumThreadCount;

            // récupération de la liste des comptes client mappés
            Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
            List<Model.Local.Customer> ListCustomer = CustomerRepository.List();
            // récupération de la liste des comptes Sage existants
            Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
            //List<int> ListSage = F_COMPTETRepository.ListIdTypeSommeil((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client, (short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Non);
            List<Model.Sage.F_COMPTET_Light> ListSage = F_COMPTETRepository.ListLight((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client, (short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Non);

            // filtrage des comptes qui sont centralisés
            ListCustomer = ListCustomer.Where(c => ListSage.Count(s => s.cbMarq == c.Sag_Id) == 1).ToList();

            // filtres clients de l'interface
            if (Core.Global.UILaunch)
            {
                if (!string.IsNullOrWhiteSpace(Core.Temp.ModuleAECInvoiceHistory_Numero))
                {
                    Core.Temp.LoadF_COMPTET_BtoBIfEmpty();
                    ListCustomer = ListCustomer.Where(c => c.Sag_Numero.StartsWith(Core.Temp.ModuleAECInvoiceHistory_Numero)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(Core.Temp.ModuleAECInvoiceHistory_Intitule))
                {
                    Core.Temp.LoadF_COMPTET_BtoBIfEmpty();
                    ListCustomer = ListCustomer.Where(c => c.Sag_Name.ToLower().Contains(Core.Temp.ModuleAECInvoiceHistory_Intitule.ToLower())).ToList();
                }
            }

            this.ListCount = ListCustomer.Count;

            Context = SynchronizationContext.Current;

            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListCustomer, this.ParallelOptions, Exec);
            });
        }

        public void Exec(Model.Local.Customer CustomerSend)
        {
            this.Semaphore.WaitOne();

            int invoice_count_transfert = 0;
            bool send_mail_notif = false;
            Model.Prestashop.PsOrders MailDatas = new Model.Prestashop.PsOrders();
            try
            {
                Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                uint IDCustomer = (uint)CustomerSend.Pre_Id;
                if (PsCustomerRepository.ExistCustomer(IDCustomer))
                {
                    MailDatas.IDCustomer = IDCustomer;

                    Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                    Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(CustomerSend.Sag_Id);
                    if (F_COMPTET != null)
                    {
                        // <JG> 03/08/2017 gestion rafraichissement progression
                        Model.Local.Customer_Progress datas_progress = new Model.Local.Customer_Progress()
                        {
                            CT_Num = F_COMPTET.CT_Num,
                            CT_Intitule = F_COMPTET.CT_Intitule,
                            Comment = string.Empty,
                        };
                        ReportInfosSynchro(datas_progress, Core.Temp._action_information_synchro.debut);

                        Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Model.Sage.F_DOCENTETERepository();

                        // <JG> 22/01/2016
                        send_mail_notif = (!string.IsNullOrWhiteSpace(Core.Global.GetConfig().ModuleAECInvoiceHistoryInfoLibreClientSendMail)
                            && F_COMPTETRepository.ExistArticleInformationLibreText(Core.Global.GetConfig().ModuleAECInvoiceHistoryInfoLibreClientSendMail, F_COMPTET.CT_Num)
                            && F_COMPTETRepository.ReadArticleInformationLibreText(Core.Global.GetConfig().ModuleAECInvoiceHistoryInfoLibreClientSendMail, F_COMPTET.CT_Num) == Core.Global.GetConfig().ModuleAECInvoiceHistoryInfoLibreClientSendMailValue);

                        Model.Prestashop.PsAECInvoiceHistoryRepository PsAECInvoiceHistoryRepository = new Model.Prestashop.PsAECInvoiceHistoryRepository();

                        // <JG> 04/03/2015 ajout gestion envoi des bons de commandes Sage dans PrestaShop
                        string order_path = System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, "AEC_Order.rpt");
                        if (System.IO.File.Exists(order_path))
                        {
                            uint TypeDocOrder = (uint)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Bon_Commande_Vente;
                            datas_progress.Comment = GetComment(TypeDocOrder);
                            ReportInfosSynchro(datas_progress, Core.Temp._action_information_synchro.refresh);
                            ClearData(PsAECInvoiceHistoryRepository, IDCustomer, TypeDocOrder);

                            if (!_ClearMode)
                            {
                                datas_progress.Comment = GetComment(TypeDocOrder);
                                ReportInfosSynchro(datas_progress, Core.Temp._action_information_synchro.refresh);

                                List<Model.Sage.Piece> ListBC = F_DOCENTETERepository.ListPieceTiers(F_COMPTET.CT_Num, ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine.Vente, ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Bon_Commande_Vente, this._PeriodeDebut, this._PeriodeFin);
                                foreach (Model.Sage.Piece Piece in ListBC)
                                {
                                    TransfertPiece(order_path, PsAECInvoiceHistoryRepository, Piece, IDCustomer, TypeDocOrder, F_COMPTET.CT_Num);
                                    datas_progress.Comment = Piece.DO_Piece + " " + ListBC.IndexOf(Piece) + "/" + ListBC.Count;
                                    ReportInfosSynchro(datas_progress, Core.Temp._action_information_synchro.refresh);
                                }
                            }
                        }

                        string invoice_path = System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, "AEC_Invoice.rpt");
                        if (System.IO.File.Exists(invoice_path))
                        {
                            uint TypeDocInvoice = (uint)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Facture_Comptabilisee_Vente;
                            if (!_ClearMode)
                            {
                                datas_progress.Comment = GetComment(TypeDocInvoice);
                                ReportInfosSynchro(datas_progress, Core.Temp._action_information_synchro.refresh);

                                List<Model.Sage.Piece> ListFacturesCompta = F_DOCENTETERepository.ListPieceTiers(F_COMPTET.CT_Num, ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine.Vente, ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Facture_Comptabilisee_Vente, this._PeriodeDebut, this._PeriodeFin);

                                List<Model.Prestashop.PsAEcInvoiceHistory_Light> ListTransfert = PsAECInvoiceHistoryRepository.ListLight(IDCustomer, TypeDocInvoice);
                                ListFacturesCompta = ListFacturesCompta.Where(d => ListTransfert.Count(i => i.invoice_number == d.DO_Piece) == 0).ToList();

                                foreach (Model.Sage.Piece Piece in ListFacturesCompta)
                                {
                                    if (TransfertPiece(invoice_path, PsAECInvoiceHistoryRepository, Piece, IDCustomer, TypeDocInvoice, F_COMPTET.CT_Num))
                                    {
                                        invoice_count_transfert++;

                                        if (string.IsNullOrEmpty(MailDatas.Mail_Invoice_numbers))
                                            MailDatas.Mail_Invoice_numbers = Piece.DO_Piece;
                                        else
                                            MailDatas.Mail_Invoice_numbers += ", " + Piece.DO_Piece;
                                    }
                                    datas_progress.Comment = Piece.DO_Piece + " " + ListFacturesCompta.IndexOf(Piece) + "/" + ListFacturesCompta.Count;
                                    ReportInfosSynchro(datas_progress, Core.Temp._action_information_synchro.refresh);
                                }
                            }
                            else
                            {
                                datas_progress.Comment = GetComment(TypeDocInvoice);
                                ReportInfosSynchro(datas_progress, Core.Temp._action_information_synchro.refresh);

                                ClearData(PsAECInvoiceHistoryRepository, IDCustomer, TypeDocInvoice);
                            }
                        }
                        ReportInfosSynchro(datas_progress, Core.Temp._action_information_synchro.fin);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }

            // <JG> 22/01/2016 ajout mail notification
            try
            {
                if (send_mail_notif && invoice_count_transfert > 0)
                {
                    Core.Sync.SynchronisationCommande.SendMail(40, MailDatas);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }

            lock (this)
            {
                this.CurrentCount += 1;
            }
            this.ReportProgress(this.CurrentCount * 100 / this.ListCount);
            this.Semaphore.Release();
        }

        private bool TransfertPiece(string report_path, Model.Prestashop.PsAECInvoiceHistoryRepository PsAECInvoiceHistoryRepository, Model.Sage.Piece Piece, uint IDCustomer, uint TypeDoc, String CT_Num)
        {
            bool transfert_complete = false;
            string temp_file = System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, Piece.cbMarq + ".pdf");

            Core.PrintCrystal.ExportPDF(report_path, Piece, TypeDoc, CT_Num, temp_file);

            try
            {
                #region Upload

                // identifiants FTP
                String FTP = Core.Global.GetConfig().ConfigFTPIP;
                String FTPUser = Core.Global.GetConfig().ConfigFTPUser;
                String FTPPassword = Core.Global.GetConfig().ConfigFTPPassword;

                // nommmage par défaut du fichier
                string ftp_filename = Piece.DO_Piece + ".pdf";

                // répertoire cible racine
                string ftp_path = FTP + "/AECInvoice";
                Core.Ftp.CreateFolder(ftp_path, FTPUser, FTPPassword);

                // ajout .htaccess pour bloquer navigation
                Core.Ftp.Uploadhtaccess("Options -Indexes", ftp_path, FTPUser, FTPPassword);


                // répertoire cible client
                string md5folder = Core.RandomString.HashMD5(IDCustomer.ToString());
                ftp_path += "/" + md5folder;
                Core.Ftp.CreateFolder(ftp_path, FTPUser, FTPPassword);

                // nommage hexa du fichier
                string ftp_fullfile = ftp_path + "/" + ftp_filename;
                do
                {
                    ftp_filename = Core.Global.GetRandomHexNumber(47);
                    ftp_fullfile = ftp_path + "/" + ftp_filename;
                }
                while (PsAECInvoiceHistoryRepository.ExistFile(ftp_filename)
                    || Core.Ftp.ExistFile(ftp_fullfile, FTPUser, FTPPassword));

                Core.Ftp.UploadFile(temp_file, ftp_fullfile, FTPUser, FTPPassword);

                System.IO.File.Delete(temp_file);
                #endregion

                #region insertion bdd module
                PsAECInvoiceHistoryRepository.Add(new Model.Prestashop.PsAEcInvoiceHistory()
                {
                    IDCustomer = IDCustomer,
                    InvoiceNumber = Piece.DO_Piece,
                    File = ftp_filename,
                    FileName = Piece.DO_Piece + ".pdf",
                    InvoiceDate = (Piece.DO_Date != null) ? Piece.DO_Date.Value : DateTime.Now,
                    TotalAmountTaxInCl = (Piece.TotalAmountTaxExcl.HasValue && Piece.TotalTaxAmount.HasValue) ? Piece.TotalAmountTaxExcl.Value + Piece.TotalTaxAmount.Value : 0,
                    TotalAmountTaxExCl = (Piece.TotalAmountTaxExcl.HasValue) ? Piece.TotalAmountTaxExcl.Value : 0,
                    TypeDocument = TypeDoc,
                });
                #endregion

                transfert_complete = true;
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(temp_file))
                    System.IO.File.Delete(temp_file);
                Core.Error.SendMailError(ex.ToString());
            }
            return transfert_complete;
        }

        private void ClearData(Model.Prestashop.PsAECInvoiceHistoryRepository PsAECInvoiceHistoryRepository, uint IDCustomer, uint TypeDoc, DateTime? start = null, DateTime? end = null)
        {
            IQueryable<Model.Prestashop.PsAEcInvoiceHistory> List = PsAECInvoiceHistoryRepository.ListCustomerType(IDCustomer, TypeDoc);

            if (start.HasValue)
                List = List.Where(d => d.InvoiceDate >= start.Value.Date);
            if (end.HasValue)
                List = List.Where(d => d.InvoiceDate <= end.Value.Date);

            foreach (Model.Prestashop.PsAEcInvoiceHistory Order in List)
            {
                String FTP = Core.Global.GetConfig().ConfigFTPIP;
                String FTPUser = Core.Global.GetConfig().ConfigFTPUser;
                String FTPPassword = Core.Global.GetConfig().ConfigFTPPassword;

                string ftp_path = FTP + "/AECInvoice";

                string md5folder = Core.RandomString.HashMD5(IDCustomer.ToString());
                ftp_path += "/" + md5folder;

                string ftp_fullfile = ftp_path + "/" + Order.File;
                if (Core.Ftp.ExistFile(ftp_fullfile, FTPUser, FTPPassword))
                {
                    Core.Ftp.DeleteFile(ftp_fullfile, FTPUser, FTPPassword);
                }
            }

            PsAECInvoiceHistoryRepository.DeleteSelection(List);
            //PsAECInvoiceHistoryRepository.DeleteCustomerType(IDCustomer, TypeDocOrder);
        }

        public void ReportProgress(Int32 Percentage)
        {
            Context.Post(state =>
            {
                this.ProgressBar.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }

        public void ReportInfosSynchro(Model.Local.Customer_Progress datas, Core.Temp._action_information_synchro action, string comment = null)
        {
            Context.Post(state =>
            {
                switch (action)
                {
                    case PRESTACONNECT.Core.Temp._action_information_synchro.debut:
                        this.list_progress.Add(new Model.Local.Customer_Progress(datas));
                        break;
                    case PRESTACONNECT.Core.Temp._action_information_synchro.fin:
                        this.list_progress.Remove(list_progress.FirstOrDefault(p => p.CT_Num == datas.CT_Num));
                        break;
                    case PRESTACONNECT.Core.Temp._action_information_synchro.refresh:
                        this.list_progress.FirstOrDefault(p => p.CT_Num == datas.CT_Num).Comment = (comment != null) ? comment : datas.Comment;
                        break;
                    default:
                        break;
                }
                listBoxProgress.ItemsSource = list_progress;
            }, null);
        }

        private string GetComment(uint TypeDoc)
        {
            string mode = (this._ClearMode) ? "Nettoyage" : "Récupération";
            string doc = ((TypeDoc == 1) ? "BC"
                    : (TypeDoc == 7) ? "factures"
                    : "documents");
            return mode + " des " + doc + " Vente"
                + ((this._PeriodeDebut.HasValue && this._PeriodeFin.HasValue) ? " pour la période du " + this._PeriodeDebut.Value.ToShortDateString() + " au " + this._PeriodeFin.Value.ToShortDateString()
                    : (this._PeriodeDebut.HasValue) ? " à partir du " + this._PeriodeDebut.Value.ToShortDateString()
                    : (this._PeriodeDebut.HasValue) ? " jusqu'au " + this._PeriodeFin.Value.ToShortDateString()
                    : string.Empty);
        }
    }
}