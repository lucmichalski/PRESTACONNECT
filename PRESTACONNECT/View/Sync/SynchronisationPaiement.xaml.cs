using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour SynchronisationPaiement.xaml
    /// </summary>
    public partial class SynchronisationPaiement : Window
    {
        private BackgroundWorker worker = new BackgroundWorker();
        ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion;
        public Int32 CurrentCount = 0;

        public SynchronisationPaiement()
        {
            this.InitializeComponent();
            this.LabelInformation.Content = "Recherche des paiements à copier ...";

            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                if (Core.Global.GetConfig().SyncReglementActif)
                {

                    this.worker.ReportProgress(-42);
                    Connexion = Core.Global.GetODBC();

                    if (Connexion != null)
                    {
                        Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
                        Model.Prestashop.PsOrderPaymentRepository PsOrderPaymentRepository = new Model.Prestashop.PsOrderPaymentRepository();

                        Model.Local.OrderRepository OrderRepository = new Model.Local.OrderRepository();
                        Model.Local.OrderPaymentRepository OrderPaymentRepository = new Model.Local.OrderPaymentRepository();

                        Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Model.Sage.F_DOCENTETERepository();

                        List<Model.Prestashop.order_payment> list_order = PsOrdersRepository.ListIDPayment(Core.Global.CurrentShop.IDShop);

                        foreach (Model.Prestashop.order_payment order in list_order)
                        {
                            try
                            {
                                // comparaison nombre de paiements PrestaShop avec ceux déjà transférés
                                if (OrderRepository.ExistPrestashop((int)order.id_order)
                                    && F_DOCENTETERepository.ExistWeb(order.id_order.ToString())
                                    && PsOrderPaymentRepository.CountOrderReference(order.reference) > OrderPaymentRepository.CountOrder((int)order.id_order))
									// pour ne pas prendre en compte les articles non suivi en stock
								{
									// récupération du dernier document Sage
									Model.Sage.F_DOCENTETE F_DOCENTETE = F_DOCENTETERepository.ListWeb(order.id_order.ToString()).FirstOrDefault();

                                    ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE = new ABSTRACTION_SAGE.F_DOCENTETE.Obj()
                                    {
                                        DO_Domaine = (ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine)F_DOCENTETE.DO_Domaine,
                                        DO_Type = (ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type)F_DOCENTETE.DO_Type,
                                        DO_Piece = F_DOCENTETE.DO_Piece
                                    };

                                    ObjF_DOCENTETE.ReadDO_Domaine_DO_Type_DO_Piece(Connexion, false);
                                    if (ObjF_DOCENTETE != null)
                                    {
                                        Model.Prestashop.PsOrders PsOrders = PsOrdersRepository.ReadOrder((int)order.id_order);
                                        Core.Sync.SynchronisationCommande form = new Core.Sync.SynchronisationCommande();
                                        bool exist_payment;
                                        form.ExecReglement(PsOrders, Connexion, ObjF_DOCENTETE, out exist_payment);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Core.Error.SendMailError("[SYNCHRONISATION REGLEMENTS]" + ex.ToString());
                            }
                            lock (this)
                            {
                                this.CurrentCount += 1;
                            }
                            this.worker.ReportProgress((this.CurrentCount * 100 / list_order.Count));
                        }
                    }
                }
            };

            this.worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
            {
                if (args.ProgressPercentage >= 0)
                {
                    this.ProgressBar.Value = args.ProgressPercentage;
                    this.LabelInformation.Content = "Informations : " + args.ProgressPercentage + " %";
                }
                else if (args.ProgressPercentage == -42)
                {
                    this.LabelInformation.Content = "Ouverture connexion ODBC Sage...";
                }
            };

            this.worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                if (Connexion != null)
                    Connexion.Close_Connexion();
                this.Close();
            };

            // Insérez le code requis pour la création d’objet sous ce point.
            this.worker.RunWorkerAsync();
        }
    }
}