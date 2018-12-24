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
	/// Logique d'interaction pour AECCustomerCollaborateur.xaml
	/// </summary>
	public partial class AECCustomerCollaborateur : Window
	{
        
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public AECCustomerCollaborateur(List<Model.Local.Customer> ListCustomer = null)
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

                    F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);
                    if (F_COMPTET.F_COLLABORATEUR != null)
                    {
                        Model.Prestashop.PsAECCustomerCollaborateurRepository PsAECCollaborateurRepository = new Model.Prestashop.PsAECCustomerCollaborateurRepository();
                        Model.Prestashop.PsAEcCustomerCollaborateur PsAEcCollaborateur = new Model.Prestashop.PsAEcCustomerCollaborateur();

                        if (PsAECCollaborateurRepository.ExistCollaborateur((uint)Customer.Pre_Id))
                        {
                            PsAEcCollaborateur = PsAECCollaborateurRepository.ReadCollaborateur((uint)Customer.Pre_Id);
                            PsAEcCollaborateur.NomCollaborateur = F_COMPTET.F_COLLABORATEUR.CO_Nom;
                            PsAEcCollaborateur.PrenomCollaborateur = F_COMPTET.F_COLLABORATEUR.CO_Prenom;
                            PsAEcCollaborateur.TelephoneCollaborateur = F_COMPTET.F_COLLABORATEUR.CO_Telephone;
                            PsAECCollaborateurRepository.Save();
                        }
                        else
                        {
                            PsAEcCollaborateur = new Model.Prestashop.PsAEcCustomerCollaborateur();
                            PsAEcCollaborateur.IDCustomer = (uint)Customer.Pre_Id;
                            PsAEcCollaborateur.NomCollaborateur = F_COMPTET.F_COLLABORATEUR.CO_Nom;
                            PsAEcCollaborateur.PrenomCollaborateur = F_COMPTET.F_COLLABORATEUR.CO_Prenom;
                            PsAEcCollaborateur.TelephoneCollaborateur = F_COMPTET.F_COLLABORATEUR.CO_Telephone;
                            PsAECCollaborateurRepository.Add(PsAEcCollaborateur);
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
	}
}