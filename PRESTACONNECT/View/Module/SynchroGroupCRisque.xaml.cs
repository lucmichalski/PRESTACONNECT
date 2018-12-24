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
    /// Logique d'interaction pour SynchroGroupCRisque.xaml
    /// </summary>
    public partial class SynchroGroupCRisque : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public List<string> logs = new List<string>();

        public SynchroGroupCRisque(List<Model.Local.Customer> ListCustomer = null)
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
                Model.Prestashop.PsGroupLangRepository PsGroupLangRepository = new Model.Prestashop.PsGroupLangRepository();
                Model.Prestashop.PsGroupRepository PsGroupRepository = new Model.Prestashop.PsGroupRepository();

                if (F_COMPTETRepository.ExistId(Customer.Sag_Id)
                    && PsCustomerRepository.ExistCustomer((uint)Customer.Pre_Id))
                {
                    Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);
                    Model.Prestashop.PsCustomer PsCustomer = PsCustomerRepository.ReadCustomer((uint)Customer.Pre_Id);

                    Model.Local.Group_CRisqueRepository Group_CRisqueRepository = new Model.Local.Group_CRisqueRepository();
                    
                    if (F_COMPTET.N_Risque == null)
                    {
                        lock (this.logs)
                            logs.Add("SC30- Le client [ " + F_COMPTET.NumIntitule + " ] ne possède pas de code risque dans Sage !");
                    }
                    else if (!Group_CRisqueRepository.ExistCRisque((int)F_COMPTET.N_Risque))
                    {
                        lock (this.logs)
                            logs.Add("SC40- Aucun groupe affecté au code risque du client [ " + F_COMPTET.NumIntitule + " ]");
                    }
                    else
                    {
                        Model.Local.Group_CRisque Group_CRisque = Group_CRisqueRepository.ReadCRisque((int)F_COMPTET.N_Risque);

                        if (F_COMPTET.CT_ControlEnc != null 
                            && F_COMPTET.CT_ControlEnc == (short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_ControlEnc.Compte_Bloque
                            && Group_CRisque.Grp_LockCondition)
                        {
                            // si compte bloqué dans Sage et indication à PrestaConnect de détacher le client
                            uint IdPsGroup = (uint)Group_CRisque.Grp_Pre_Id;
                            if (PsCustomerGroupRepository.Exist(PsCustomer.IDCustomer, IdPsGroup))
                            {
                                PsCustomerGroupRepository.Delete(PsCustomer.IDCustomer, IdPsGroup);
                                string name = PsGroupLangRepository.Read(Core.Global.Lang, IdPsGroup).Name;
                                lock (this.logs)
                                    logs.Add("SC11- Détachement du groupe [" + (!string.IsNullOrWhiteSpace(name) ? name : IdPsGroup.ToString()) + "] pour le client Sage [ " + F_COMPTET.NumIntitule + " ] / PrestaShop ID : " + PsCustomer.IDCustomer);
                            }

                            uint IdDefaultGroup = (uint)Group_CRisque.Grp_PreId_Default;
                            if (PsGroupRepository.ExistGroup((int)IdDefaultGroup) &&
                                PsCustomerGroupRepository.Exist(PsCustomer.IDCustomer, IdDefaultGroup) == false)
                            {
                                PsCustomerGroupRepository.Add(new Model.Prestashop.PsCustomerGroup()
                                {
                                    IDCustomer = PsCustomer.IDCustomer,
                                    IDGroup = IdDefaultGroup,
                                });
                                string name = PsGroupLangRepository.Read(Core.Global.Lang, IdDefaultGroup).Name;
                                lock (this.logs)
                                    logs.Add("SC12- Rattachement au groupe par défaut [" + (!string.IsNullOrWhiteSpace(name) ? name : IdDefaultGroup.ToString()) + "] pour le client Sage [ " + F_COMPTET.NumIntitule + " ] / PrestaShop ID : " + PsCustomer.IDCustomer);
                            }

                            // si groupe du code risque défini par défaut sur le client - rattachement au groupe défaut du code risque
                            if (IdPsGroup == PsCustomer.IDDefaultGroup)
                            {
                                PsCustomer.IDDefaultGroup = IdDefaultGroup;
                                PsCustomerRepository.Save();
                            }
                        }
                        else
                        {
                            uint IdPsGroup = (uint)Group_CRisque.Grp_Pre_Id;
                            if (PsCustomerGroupRepository.Exist(PsCustomer.IDCustomer, IdPsGroup) == false)
                            {
                                if (PsGroupRepository.ExistGroup((int)IdPsGroup))
                                {
                                    //uint old_group = PsCustomer.IDDefaultGroup;
                                    //PsCustomer.IDDefaultGroup = IdPsGroup;
                                    //PsCustomerRepository.Save();

                                    if (PsCustomerGroupRepository.Exist(PsCustomer.IDCustomer, IdPsGroup) == false)
									{
                                        PsCustomerGroupRepository.Add(new Model.Prestashop.PsCustomerGroup()
                                        {
                                            IDCustomer = PsCustomer.IDCustomer,
                                            IDGroup = IdPsGroup,
                                        });
                                        string name = PsGroupLangRepository.Read(Core.Global.Lang, IdPsGroup).Name;
                                        lock (this.logs)
                                            logs.Add("SC10- Rattachement au groupe [" + (!string.IsNullOrWhiteSpace(name) ? name : IdPsGroup.ToString()) + "] pour le client Sage [ " + F_COMPTET.NumIntitule + " ] / PrestaShop ID : " + PsCustomer.IDCustomer);
									}

                                    // détachement ancien groupe
                                    //if (PsCustomerGroupRepository.Exist(PsCustomer.IDCustomer, old_group))
                                    //    PsCustomerGroupRepository.Delete(PsCustomer.IDCustomer, old_group);
                                }
                                else
                                {
                                    lock (this.logs)
                                        logs.Add("SC50- Le groupe affecté au code risque du client [ " + F_COMPTET.NumIntitule + " ] est invalide");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (this.logs)
                    logs.Add("SC20- Erreur synchronisation groupe/code risque : " + ex.ToString());
                Core.Error.SendMailError("[SC20] " + ex.ToString());
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
                        Core.Log.SendLog(logs, Core.Log.LogIdentifier.SynchroGroupCodeRisque);
                    }
                    this.Close();
                }
            }, null);
        }
    }
}