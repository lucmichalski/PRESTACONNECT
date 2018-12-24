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
    /// Logique d'interaction pour SynchroGroupCatTarif.xaml
    /// </summary>
    public partial class SynchroGroupCatTarif : Window
    {

        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public List<string> logs = new List<string>();

        public SynchroGroupCatTarif(List<Model.Local.Customer> ListCustomer = null)
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
                Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                Model.Prestashop.PsCustomerGroupRepository PsCustomerGroupRepository = new Model.Prestashop.PsCustomerGroupRepository();
                Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();

                if (F_COMPTETRepository.ExistId(Customer.Sag_Id)
                    && PsCustomerRepository.ExistCustomer((uint)Customer.Pre_Id))
                {
                    Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);
                    Model.Prestashop.PsCustomer PsCustomer = PsCustomerRepository.ReadCustomer((uint)Customer.Pre_Id);

                    Model.Local.GroupRepository GroupRepository = new Model.Local.GroupRepository();

                    if (GroupRepository.CatTarifSageMonoGroupe((int)F_COMPTET.N_CatTarif))
                    {
                        uint IdPsGroup = (uint)GroupRepository.SearchIdGroupCatTarifSage((int)F_COMPTET.N_CatTarif);
                        if (IdPsGroup != PsCustomer.IDDefaultGroup)
                        {
                            uint old_group = PsCustomer.IDDefaultGroup;
                            PsCustomer.IDDefaultGroup = IdPsGroup;
                            PsCustomerRepository.Save();
                            lock (this.logs)
                                logs.Add("SG10- Modification du groupe pour le client [ " + PsCustomer.IDCustomer + " - " + PsCustomer.LastName + " " + PsCustomer.FirstName + " ]");

                            if (PsCustomerGroupRepository.Exist(PsCustomer.IDCustomer, IdPsGroup) == false)
                                PsCustomerGroupRepository.Add(new Model.Prestashop.PsCustomerGroup()
                                {
                                    IDCustomer = PsCustomer.IDCustomer,
                                    IDGroup = IdPsGroup,
                                });

                            // détachement ancien groupe
                            if (PsCustomerGroupRepository.Exist(PsCustomer.IDCustomer, old_group))
                                PsCustomerGroupRepository.Delete(PsCustomer.IDCustomer, old_group);
                        }
                    }
                    //else if (GroupRepository.ExistCatTarif((int)F_COMPTET.N_CatTarif) == false)
                    //{
                    //    PsCustomer.Active = 0;
                    //    PsCustomerRepository.Save();
                    //    logs.Add("SG10- La catégorie tarifaire n'a pas de lien avec un groupe. Désactivation du compte client [ " + PsCustomer.IDCustomer + " - " + PsCustomer.LastName + " " + PsCustomer.FirstName + " ]");
                    //}
                }
            }
            catch (Exception ex)
            {
                lock (this.logs)
                    logs.Add("SG20- Erreur synchronisation groupe/catégorie tarifaire : " + ex.ToString());
                Core.Error.SendMailError("[SG20] " + ex.ToString());
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
                    if (logs != null && logs.Count > 0)
                    {
                        Core.Log.SendLog(logs, Core.Log.LogIdentifier.SynchroGroupCatTarif);
                    }
                    this.Close();
                }
            }, null);
        }
    }
}