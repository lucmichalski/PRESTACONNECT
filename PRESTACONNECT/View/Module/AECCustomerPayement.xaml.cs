using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
	/// <summary>
	/// Logique d'interaction pour AECCustomerPayement.xaml
	/// </summary>
	public partial class AECCustomerPayement : Window
	{
        
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public AECCustomerPayement(List<Model.Local.Customer> ListCustomer = null)
        {
            this.InitializeComponent();
            if (ListCustomer == null)
            {
                Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                ListCustomer = CustomerRepository.List();
            }
            this.ListCount = ListCustomer.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListCustomer, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Model.Local.Customer Customer)
        {
            this.Semaphore.WaitOne();

            try
            {
                Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();

                if (F_COMPTETRepository.ExistId(Customer.Sag_Id))
                {
                    Model.Sage.F_COMPTET F_COMPTET = new Model.Sage.F_COMPTET();
                    Model.Prestashop.PsAECCustomerPayementRepository PsAECPayementRepository = new Model.Prestashop.PsAECCustomerPayementRepository();
                    Model.Prestashop.PsAEcCustomerPayement PsAEcPayement = new Model.Prestashop.PsAEcCustomerPayement();
                    F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);
                    List<Model.Sage.F_REGLEMENTT> ListF_REGLEMENTT = new Model.Sage.F_REGLEMENTTRepository().ListCLient(F_COMPTET.CT_Num);

                    // <JG> 05/03/2015 ajout suppression de prestashop ni modèle n'existe plus dans Sage
                    IQueryable<Model.Prestashop.PsAEcCustomerPayement> ListCustomerPayement = PsAECPayementRepository.ListCustomer((uint)Customer.Pre_Id);

                    foreach (Model.Prestashop.PsAEcCustomerPayement PsAEcCustomerPayement in ListCustomerPayement)
                    {     
                        if (ListF_REGLEMENTT.Count(rg => rg.cbMarq == (int)PsAEcCustomerPayement.IDSage) == 0)
                            PsAECPayementRepository.Delete(PsAEcCustomerPayement);
                    }

                    foreach (Model.Sage.F_REGLEMENTT F_REGLEMENTT in ListF_REGLEMENTT)
                    {
                        PsAEcPayement = new Model.Prestashop.PsAEcCustomerPayement();
                        PsAECPayementRepository = new Model.Prestashop.PsAECCustomerPayementRepository();

                        if (PsAECPayementRepository.ExistCustomerPayement((uint)Customer.Pre_Id, (uint)F_REGLEMENTT.cbMarq))
                        {
                            PsAEcPayement = PsAECPayementRepository.ReadCustomerPayement((uint)Customer.Pre_Id, (uint)F_REGLEMENTT.cbMarq);
                            PsAEcPayement.Payement = ReadPayement(F_REGLEMENTT.cbMarq);
                            PsAECPayementRepository.Save();
                        }
                        else
                        {
                            PsAEcPayement.IDCustomer = (uint)Customer.Pre_Id;
                            PsAEcPayement.IDSage = (uint)F_REGLEMENTT.cbMarq;
                            PsAEcPayement.Payement = ReadPayement(F_REGLEMENTT.cbMarq);
                            PsAECPayementRepository.Add(PsAEcPayement);
                        }
                    }
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

        private static String ReadPayement(int IdReglementT)
        {
            String Paiement = string.Empty;
            Model.Sage.F_REGLEMENTT F_REGLEMENTT = new Model.Sage.F_REGLEMENTT();
            Model.Sage.F_REGLEMENTTRepository F_REGLEMENTTRepository = new Model.Sage.F_REGLEMENTTRepository();
            if (F_REGLEMENTTRepository.Exist(IdReglementT))
            {
                F_REGLEMENTT = F_REGLEMENTTRepository.Read(IdReglementT);

                // Pourcentage
                if (F_REGLEMENTT.RT_TRepart == 0)
                {
                    Paiement = Math.Round(F_REGLEMENTT.RT_VRepart.Value, 2).ToString() + "% à ";
                }
                else
                {
                    // Equilibre
                    if (F_REGLEMENTT.RT_TRepart == 1)
                    {
                        Paiement = "Paiement à ";
                    }
                    // Montant
                    else
                    {
                        Paiement = Math.Round(F_REGLEMENTT.RT_VRepart.Value, 2).ToString() + "€ à ";
                    }
                }

                Paiement += F_REGLEMENTT.RT_NbJour.ToString() + " jour(s) ";

                if (F_REGLEMENTT.RT_Condition == 0)
                {
                    Paiement += "net(s) ";
                }
                else
                {
                    if (F_REGLEMENTT.RT_Condition == 1)
                    {
                        Paiement += "fin de mois civil ";
                    }
                    else
                    {
                        Paiement += "fin de mois ";
                    }
                }

                if (F_REGLEMENTT.RT_JourTb01 > 0)
                {
                    Paiement += "le(s) " + F_REGLEMENTT.RT_JourTb01.ToString();

                    if (F_REGLEMENTT.RT_JourTb02 > 0)
                    {
                        Paiement += ", " + F_REGLEMENTT.RT_JourTb02.ToString();

                        if (F_REGLEMENTT.RT_JourTb03 > 0)
                        {
                            Paiement += ", " + F_REGLEMENTT.RT_JourTb03.ToString();

                            if (F_REGLEMENTT.RT_JourTb04 > 0)
                            {
                                Paiement += ", " + F_REGLEMENTT.RT_JourTb04.ToString();

                                if (F_REGLEMENTT.RT_JourTb05 > 0)
                                {
                                    Paiement += ", " + F_REGLEMENTT.RT_JourTb05.ToString();

                                    if (F_REGLEMENTT.RT_JourTb06 > 0)
                                    {
                                        Paiement += ", " + F_REGLEMENTT.RT_JourTb06.ToString();
                                    }
                                }
                            }
                        }
                    }

                    Paiement += " ";
                }

                Paiement += "par " + F_REGLEMENTT.P_REGLEMENT.R_Intitule;
            }
            return Paiement;
        }
	}
}