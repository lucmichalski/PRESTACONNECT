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
    /// Logique d'interaction pour AECRepresentativeCustomer.xaml
    /// </summary>
    public partial class AECRepresentativeCustomer : Window
    {

        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public AECRepresentativeCustomer(List<Model.Local.Customer> ListCustomer = null)
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
                    if (F_COMPTET.F_COLLABORATEUR != null && F_COMPTET.F_COLLABORATEUR.CO_No.HasValue)
                    {
                        Model.Prestashop.PsAECRepresentativeRepository PsAECRepresentativeRepository = new Model.Prestashop.PsAECRepresentativeRepository();
                        if (PsAECRepresentativeRepository.ExistSage((uint)F_COMPTET.F_COLLABORATEUR.CO_No))
                        {
                            Model.Prestashop.PsAEcRepresentative PsAECRepresentative = PsAECRepresentativeRepository.ReadSage((uint)F_COMPTET.F_COLLABORATEUR.CO_No);

                            Model.Prestashop.PsAECRepresentativeCustomerRepository PsAECRepresentativeCustomerRepository = new Model.Prestashop.PsAECRepresentativeCustomerRepository();
                            Model.Prestashop.PsAEcRepresentativeCustomer PsAEcRepresentativeCustomer = new Model.Prestashop.PsAEcRepresentativeCustomer();

                            if (PsAECRepresentativeCustomerRepository.Exist((uint)Customer.Pre_Id))
                            {
                                PsAEcRepresentativeCustomer = PsAECRepresentativeCustomerRepository.Read((uint)Customer.Pre_Id);
                                PsAEcRepresentativeCustomer.IDRepresentative = PsAECRepresentative.IDRepresentative;
                                PsAECRepresentativeCustomerRepository.Save();
                            }
                            else
                            {
                                PsAEcRepresentativeCustomer = new Model.Prestashop.PsAEcRepresentativeCustomer();
                                PsAEcRepresentativeCustomer.IDCustomer = (uint)Customer.Pre_Id;
                                PsAEcRepresentativeCustomer.IDRepresentative = PsAECRepresentative.IDRepresentative;
                                PsAECRepresentativeCustomerRepository.Add(PsAEcRepresentativeCustomer);
                            }
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