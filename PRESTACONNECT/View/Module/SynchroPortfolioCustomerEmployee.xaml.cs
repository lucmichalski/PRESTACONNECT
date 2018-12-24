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
    /// Logique d'interaction pour SynchroPortfolioCustomerEmployee.xaml
    /// </summary>
    public partial class SynchroPortfolioCustomerEmployee : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public List<string> logs = new List<string>();

        public SynchroPortfolioCustomerEmployee(List<Model.Local.Customer> ListCustomer = null)
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
                Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();

                if (F_COMPTETRepository.ExistId(Customer.Sag_Id)
                    && PsCustomerRepository.ExistCustomer((uint)Customer.Pre_Id))
                {
                    Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);
                    Model.Prestashop.PsCustomer PsCustomer = PsCustomerRepository.ReadCustomer((uint)Customer.Pre_Id);

                    Model.Local.Employee_CollaborateurRepository Employee_CollaborateurRepository = new Model.Local.Employee_CollaborateurRepository();
                    Model.Prestashop.PsEmployeeRepository PsEmployeeRepository = new Model.Prestashop.PsEmployeeRepository();
                    Model.Prestashop.PsPortfolioCustomerEmployeeRepository PsPortfolioCustomerEmployeeRepository = new Model.Prestashop.PsPortfolioCustomerEmployeeRepository();

                    if (F_COMPTET.CO_No == null)
                    {
                        lock (this.logs)
                            logs.Add("SP30- Le client [ " + F_COMPTET.NumIntitule + " ] ne possède pas de collaborateur dans Sage !");
                    }
                    else if (!Employee_CollaborateurRepository.ExistCollaborateur((int)F_COMPTET.CO_No))
                    {
                        lock (this.logs)
                            logs.Add("SP40- Aucun compte employé PrestaShop affecté au collaborateur du client [ " + F_COMPTET.NumIntitule + " ]");

                        if (PsPortfolioCustomerEmployeeRepository.ExistCustomer(PsCustomer.IDCustomer))
                        {
                            List<Model.Prestashop.PsPortfolioCustomerEmployee> ListPsPortfolioCustomerEmployee = PsPortfolioCustomerEmployeeRepository.ListCustomer(PsCustomer.IDCustomer);
                            foreach (Model.Prestashop.PsPortfolioCustomerEmployee PsPortfolioCustomerEmployee in ListPsPortfolioCustomerEmployee)
                            {
                                if (PsPortfolioCustomerEmployee.IDEmployee != null)
                                {
                                    string name = (PsEmployeeRepository.Exist(PsPortfolioCustomerEmployee.IDEmployee.Value))
                                        ? PsEmployeeRepository.Read(PsPortfolioCustomerEmployee.IDEmployee.Value).EmployeeName
                                        : string.Empty;

                                    PsPortfolioCustomerEmployeeRepository.Delete(PsPortfolioCustomerEmployee);
                                    lock (this.logs)
                                        logs.Add("SP12- Détachement du compte employé [" + (!string.IsNullOrWhiteSpace(name) ? name : PsPortfolioCustomerEmployee.IDEmployee.Value.ToString()) + "] pour le client Sage [ " + F_COMPTET.NumIntitule + " ] / PrestaShop ID : " + PsCustomer.IDCustomer);
                                }
                            }
                        }
                    }
                    else
                    {
                        uint IdEmployee = (uint)Employee_CollaborateurRepository.ReadCollaborateur((int)F_COMPTET.CO_No).IdEmployee;

                        List<Model.Prestashop.PsPortfolioCustomerEmployee> ListPsPortfolioCustomerEmployee = PsPortfolioCustomerEmployeeRepository.ListCustomer(PsCustomer.IDCustomer);
                        if (ListPsPortfolioCustomerEmployee.Count(pce => pce.IDEmployee != IdEmployee) > 0)
                        {
                            foreach (Model.Prestashop.PsPortfolioCustomerEmployee PsPortfolioCustomerEmployee in ListPsPortfolioCustomerEmployee.Where(pce => pce.IDEmployee != IdEmployee))
                            {
                                if (PsPortfolioCustomerEmployee.IDEmployee != null)
                                {
                                    string name = (PsEmployeeRepository.Exist(PsPortfolioCustomerEmployee.IDEmployee.Value))
                                        ? PsEmployeeRepository.Read(PsPortfolioCustomerEmployee.IDEmployee.Value).EmployeeName
                                        : string.Empty;

                                    PsPortfolioCustomerEmployeeRepository.Delete(PsPortfolioCustomerEmployee);
                                    lock (this.logs)
                                        logs.Add("SP11- Détachement du compte employé [" + (!string.IsNullOrWhiteSpace(name) ? name : PsPortfolioCustomerEmployee.IDEmployee.Value.ToString()) + "] pour le client Sage [ " + F_COMPTET.NumIntitule + " ] / PrestaShop ID : " + PsCustomer.IDCustomer);
                                }
                            }
                        }
                        if (IdEmployee == 0)
                        {
                            lock (this.logs)
                                logs.Add("SP50- Aucun compte employé PrestaShop affecté au collaborateur du client [ " + F_COMPTET.NumIntitule + " ]");
                        }
                        else if (PsPortfolioCustomerEmployeeRepository.Exist(PsCustomer.IDCustomer, IdEmployee) == false)
                        {
                            if (PsEmployeeRepository.Exist(IdEmployee))
                            {
                                PsPortfolioCustomerEmployeeRepository.Add(new Model.Prestashop.PsPortfolioCustomerEmployee()
                                {
                                    IDCustomer = PsCustomer.IDCustomer,
                                    IDEmployee = IdEmployee,
                                });
                                string name = (PsEmployeeRepository.Exist(IdEmployee))
                                        ? PsEmployeeRepository.Read(IdEmployee).EmployeeName
                                        : string.Empty;

                                lock (this.logs)
                                    logs.Add("SP10- Rattachement au compte employé [" + (!string.IsNullOrWhiteSpace(name) ? name : IdEmployee.ToString()) + "] pour le client Sage [ " + F_COMPTET.NumIntitule + " ] / PrestaShop ID : " + PsCustomer.IDCustomer);
                            }
                            else
                            {
                                lock (this.logs)
                                    logs.Add("SP60- Le compte employé affecté au collaborateur du client [ " + F_COMPTET.NumIntitule + " ] est invalide");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (this.logs)
                    logs.Add("SC20- Erreur affecation client/employé : " + ex.ToString());
                Core.Error.SendMailError("[SP20] " + ex.ToString());
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
                        Core.Log.SendLog(logs, Core.Log.LogIdentifier.SynchroPortfolioCustomerEmployee);
                    }
                    this.Close();
                }
            }, null);
        }
    }
}