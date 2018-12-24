using System;
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
using System.Linq;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        private IQueryable<Model.Local.Customer> ListBtoBCustomer = null;

        internal new Contexts.ClientContext DataContext
        {
            get { return (Contexts.ClientContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        public Client()
        {
            this.InitializeComponent();
            DataContext = new Contexts.ClientContext();
            this.LoadComponent();

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
        }

        private void LoadComponent()
        {
            // <JG> 26/12/2012
            this.CbGroupeClient.ItemsSource = new Model.Local.GroupRepository().ListGroupesLies();

            this.ButtonOustanding.IsEnabled = Core.Global.GetConfig().ModuleAECCustomerOutstandingActif;

            if (Core.Global.GetConfig().ConfigBToB)
            {
                this.TabItemClientBToB.IsEnabled = true;
                this.TabItemClientBToC.IsEnabled = false;
                this.TabItemClientBToB.IsSelected = true;

                this.GroupBoxSageCustomer.Header = (Core.Global.GetConfig().ConfigClientMultiMappageBtoB) ? "Client(s) Sage" : "Client(s) Sage non mappé(s)";

                this.LoadBTOBCustomer();
            }
            else if (Core.Global.GetConfig().ConfigBToC)
            {
                this.TabItemClientBToC.IsEnabled = true;
                this.TabItemClientBToB.IsEnabled = false;
                this.TabItemClientBToC.IsSelected = true;

                Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();

                this.DataGridClient.ItemsSource = (Core.Global.GetConfig().ConfigClientFiltreCommande)
                            ? PsCustomerRepository.ListTopActiveOrderByDateAdd(60, 1, Core.Global.CurrentShop.IDShop)
                            : PsCustomerRepository.ListTopActiveOrderByDateAddWithOrder(60, 1, Core.Global.CurrentShop.IDShop);
                this.TabItemClientBToB.IsEnabled = false;

                #region centralisation clients
                Model.Local.Config Config = new Model.Local.Config();

                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                if (ConfigRepository.ExistName(Core.Global.ConfigClientTypeLien)
                    && ConfigRepository.ReadName(Core.Global.ConfigClientTypeLien).Con_Value == Core.Global.ConfigClientTypeLienEnum.CompteCentralisateur.ToString())
                {
                    if (ConfigRepository.ExistName(Core.Global.ConfigClientCompteCentralisateur))
                    {
                        Config = ConfigRepository.ReadName(Core.Global.ConfigClientCompteCentralisateur);
                        if (Core.Global.IsInteger(Config.Con_Value))
                        {
                            Int32 cbMarqCentralisateur = Int32.Parse(Config.Con_Value);
                            Model.Sage.F_COMPTETRepository F_COMPTETRepositoryCentralisateur = new Model.Sage.F_COMPTETRepository();
                            Model.Sage.F_COMPTET F_COMPTETCentralisateur;
                            if (F_COMPTETRepositoryCentralisateur.ExistId(cbMarqCentralisateur))
                            {
                                F_COMPTETCentralisateur = F_COMPTETRepositoryCentralisateur.Read(cbMarqCentralisateur);

                                this.LabelClientCentralisateur.Content = "Commandes et adresses des clients Prestashop centralisées vers le compte Sage " + F_COMPTETCentralisateur.CT_Num + " " + F_COMPTETCentralisateur.CT_Intitule;
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                this.BtTransfert.IsEnabled = false;
            }

            LoadModules();
        }

        private void LoadModules()
        {
            buttonAECRepresentative.IsEnabled = Core.Global.ExistAECRepresentativeModule();
            buttonAECBalance.IsEnabled = Core.Global.ExistAECBalanceModule();
            ButtonPortfolioCustomerEmployee.IsEnabled = Core.Global.ExistPortfolioCustomerEmployeeModule() && Core.Global.GetConfig().ModulePortfolioCustomerEmployeeActif;
        }

        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();


                DateTime? filtre = null, old_config = Core.Global.GetConfig().ConfigCommandeFiltreDate;
                if (DataContext.SyncDay)
                    filtre = DateTime.Now.Date;
                else if (DataContext.SyncWeek)
                    filtre = DateTime.Now.Date.AddDays(-7);
                else if (DataContext.SyncMonth)
                    filtre = DateTime.Now.Date.AddMonths(-1);

                if (filtre != null)
                    Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(filtre, true);
                

                if (Core.Global.GetConfig().ConfigBToC)
                {
                    PRESTACONNECT.SynchronisationClient SynchronisationClient = new SynchronisationClient();
                    SynchronisationClient.ShowDialog();
                }

                PRESTACONNECT.SynchronisationLivraison SynchronisationLivraison = new SynchronisationLivraison();
                SynchronisationLivraison.ShowDialog();


                if (filtre != null)
                    Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(old_config, true);

                Loading.Close();

                if (this.TabItemClientBToC.IsEnabled)
                {
                    Core.Temp.ListLocalCustomer = new Model.Local.CustomerRepository().List();
                    Core.Temp.ListF_COMPTET_BtoB = new Model.Sage.F_COMPTETRepository().ListBtoB((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client);

                    Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();

                    this.DataGridClient.ItemsSource = (Core.Global.GetConfig().ConfigClientFiltreCommande)
                                ? PsCustomerRepository.ListTopActiveOrderByDateAdd(60, 1, Core.Global.CurrentShop.IDShop)
                                : PsCustomerRepository.ListTopActiveOrderByDateAddWithOrder(60, 1, Core.Global.CurrentShop.IDShop);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        #region BTOB
        private void LoadBTOBCustomer()
        {
            this.listBoxBTOBCustomerSage.IsEnabled = false;
            this.ButtonLinkAccount.IsEnabled = false;

            Core.Temp.ListLocalCustomer = new Model.Local.CustomerRepository().List();

            //if (Core.Temp.ListPrestashopCustomerBtoB == null || Core.Temp.ListPrestashopCustomerBtoB.Count == 0)
            {
                Core.Temp.ListPrestashopCustomerBtoB = new Model.Prestashop.PsCustomerRepository().ListFull(Core.Global.CurrentShop.IDShop);
            }

            this.listBoxBTOBCustomerPrestashop.ItemsSource = Core.Temp.ListPrestashopCustomerBtoB.Where(p => Core.Temp.ListLocalCustomer.Count(l => l.Pre_Id == p.id_customer) == 0);

            Core.Temp.ListF_COMPTET_BtoB = new Model.Sage.F_COMPTETRepository().ListBtoB((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client);

            this.LoadDataGridCustomer();
        }

        private void SearchCustomerBToB()
        {
            List<Model.Sage.F_COMPTET_BtoB> ListF_COMPTET = new Model.Sage.F_COMPTETRepository().ListBtoB((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client);

            if (!Core.Global.GetConfig().ConfigClientMultiMappageBtoB)
                ListF_COMPTET = ListF_COMPTET.Where(s => Core.Temp.ListLocalCustomer.Count(l => l.Sag_Id == s.cbMarq) == 0).ToList();

            if (!string.IsNullOrWhiteSpace(this.TextBoxBTOBCustomerBegin.Text)
                || !string.IsNullOrWhiteSpace(this.TextBoxBTOBCustomerIntitule.Text)
                || !string.IsNullOrWhiteSpace(this.TextBoxBTOBCustomerEmail.Text))
            {
                string intitule = this.TextBoxBTOBCustomerIntitule.Text.ToLower().Trim();
                string mail = this.TextBoxBTOBCustomerEmail.Text.ToLower().Trim();
                ListF_COMPTET = ListF_COMPTET.Where(s => s.CT_Num.StartsWith(this.TextBoxBTOBCustomerBegin.Text.ToUpper())
                                    && s.CT_Intitule.ToLower().Contains(intitule)
                                    && (s.CT_EMail.ToLower().Contains(mail)
                                        || s.F_LIVRAISON().Count(l => l.LI_EMail.ToLower().Contains(mail)) > 0)).ToList();
            }

            int count = ListF_COMPTET.Count;
            if (count > 500)
            {
                MessageBox.Show("Nombre de clients trouvés : " + count + ".\n\rVeuillez effectuer une recherche plus précise !", "Client", MessageBoxButton.OK);
            }
            else
            {
                this.listBoxBTOBCustomerSage.ItemsSource = ListF_COMPTET;
            }
        }

        private void LoadDataGridCustomer()
        {
            this.Cursor = Cursors.Wait;

            if (this.ListBtoBCustomer == null)
            {
                this.ListBtoBCustomer = new Model.Local.CustomerRepository().List().AsQueryable();
            }
            IQueryable<Model.Local.Customer> temp = this.ListBtoBCustomer;
            if (!string.IsNullOrWhiteSpace(this.textBoxFiltreBtoBList.Text))
            {
                string filtre = this.textBoxFiltreBtoBList.Text.ToLower();
                temp = temp.AsParallel().Where(c => c.Pre_Numero.Contains(filtre)
                                    || c.Pre_Name.ToLower().Contains(filtre)
                                    || c.Sag_Numero.ToLower().Contains(filtre)
                                    || c.Sag_Name.ToLower().Contains(filtre)).AsQueryable().OrderBy(c => c.Pre_Id);
            }

            this.DataGridCustomerBTOB.ItemsSource = temp;

            this.Cursor = Cursors.Arrow;
        }

        private void ButtonSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            this.SearchCustomerBToB();
        }

        private void ComboBoxBTOBCustomerPrestashop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.listBoxBTOBCustomerSage.IsEnabled = (this.listBoxBTOBCustomerPrestashop.SelectedItem != null);
        }

        private void ComboBoxBTOBCustomerSage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.LinkAccounts();
        }

        private void LinkAccounts()
        {
            Model.Sage.F_COMPTET_BtoB F_COMPTET = (Model.Sage.F_COMPTET_BtoB)listBoxBTOBCustomerSage.SelectedItem;
            Model.Prestashop.btob_customer PsCustomer = (Model.Prestashop.btob_customer)listBoxBTOBCustomerPrestashop.SelectedItem;

            if (F_COMPTET != null && PsCustomer != null)
            {
                Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();

                if (!CustomerRepository.ExistPrestashop((int)PsCustomer.id_customer))
                {
                    if (!CustomerRepository.ExistSage(F_COMPTET.cbMarq))
                    {
                        CustomerRepository.Add(new Model.Local.Customer()
                        {
                            Pre_Id = (int)PsCustomer.id_customer,
                            Sag_Id = F_COMPTET.cbMarq,
                        });
                        this.RefreshBtoB();
                    }
                    else if (Core.Global.GetConfig().ConfigClientMultiMappageBtoB)
                    {
                        if (MessageBox.Show("Le client Sage " + F_COMPTET.CT_Intitule + " est déjà mappé avec un ou plusieurs comptes PrestaShop."
                            + "\nÊtes-vous sûr de vouloir y associer le compte :\n" + PsCustomer.BtoBString,
                            "Multi-Mappage", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            CustomerRepository.Add(new Model.Local.Customer()
                            {
                                Pre_Id = (int)PsCustomer.id_customer,
                                Sag_Id = F_COMPTET.cbMarq,
                            });
                            this.RefreshBtoB();
                        }
                    }
                }
            }
        }

        private void DataGridBTOBCustomerButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Model.Local.Customer Customer = this.DataGridCustomerBTOB.SelectedItem as Model.Local.Customer;
            Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
            bool delete = false;
            if (CustomerRepository.ExistPrestashopSage(Customer.Pre_Id, Customer.Sag_Id))
            {
                Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                if (F_COMPTETRepository.ExistId(Customer.Sag_Id) == false)
                {
                    delete = true;
                }
                else if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer le mappage entre le compte PrestaShop : " + Customer.Pre_Name +
                    "\n" + "et le compte Sage : " + Customer.Sag_Name + " ?", "Suppression mappage", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    delete = true;
                }
                //else
                //{
                //    MessageBox.Show("Votre client Prestashop a déjà été synchronisé. \n Pour pouvoir le supprimer, il faut préalablement supprimer le compte Sage associé : " + Customer.Sag_Name, "Client", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}
                if (delete)
                {
                    Model.Local.Customer CustomerDelete = CustomerRepository.ReadPrestashopSage(Customer.Pre_Id, Customer.Sag_Id);
                    CustomerRepository.Delete(CustomerDelete);

                    // suppression mappage adresse
                    Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();
                    Model.Local.AddressRepository AddressRepository = new Model.Local.AddressRepository();
                    foreach (Model.Sage.F_LIVRAISON F_LIVRAISON in F_LIVRAISONRepository.ListComptet(Customer.Sag_Numero))
                    {
                        if (AddressRepository.ExistSage(F_LIVRAISON.cbMarq))
                            AddressRepository.Delete(AddressRepository.ReadSage(F_LIVRAISON.cbMarq));
                    }

                    this.RefreshBtoB();
                }
            }
        }

        private void ButtonButtonLinkAccount_Click(object sender, RoutedEventArgs e)
        {
            this.LinkAccounts();
        }

        private void ButtonSearchPrestashopCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxFiltrePrestashopCustomer.Text))
                this.listBoxBTOBCustomerPrestashop.ItemsSource = Core.Temp.ListPrestashopCustomerBtoB.Where(p => Core.Temp.ListLocalCustomer.Count(l => l.Pre_Id == p.id_customer) == 0);
            else
            {
                IEnumerable<Model.Prestashop.btob_customer> filter = Core.Temp.ListPrestashopCustomerBtoB.Where(p => Core.Temp.ListLocalCustomer.Count(l => l.Pre_Id == p.id_customer) == 0);
                filter = filter.Where(c => c.lastname.ToLower().Contains(this.textBoxFiltrePrestashopCustomer.Text.ToLower())
                                        || c.firstname.ToLower().Contains(this.textBoxFiltrePrestashopCustomer.Text.ToLower())
                                        || c.email.ToLower().Contains(this.textBoxFiltrePrestashopCustomer.Text.ToLower())
                                        || c.id_customer.ToString().Contains(this.textBoxFiltrePrestashopCustomer.Text));
                this.listBoxBTOBCustomerPrestashop.ItemsSource = filter;
            }
        }

        private void ButtonFiltrerBtoBList_Click(object sender, RoutedEventArgs e)
        {
            this.LoadDataGridCustomer();
        }

        private void RefreshBtoB()
        {
            this.textBoxFiltrePrestashopCustomer.Text = string.Empty;
            this.textBoxFiltreBtoBList.Text = string.Empty;
            this.ListBtoBCustomer = null;
            this.listBoxBTOBCustomerSage.ItemsSource = null;
            this.TextBoxBTOBCustomerBegin.Text = string.Empty;
            this.TextBoxBTOBCustomerIntitule.Text = string.Empty;
            this.TextBoxBTOBCustomerEmail.Text = string.Empty;
            LoadBTOBCustomer();
        }

        #endregion

        #region Creation comptes client Prestashop

        private void BtTransfert_Click(object sender, RoutedEventArgs e)
        {
            if (this.CBClientSage.SelectedItem != null)
            {
                try
                {
                    List<String> log;
                    if (!string.IsNullOrWhiteSpace(Core.Global.GetConfig().TransfertPrestashopCookieKey))
                    {
                        if (this.CBClientSage.SelectedItem != null && this.CBClientSage.SelectedItem is Model.Sage.F_COMPTET_Light)
                        {
                            Model.Sage.F_COMPTET_Light F_COMPTET = (Model.Sage.F_COMPTET_Light)this.CBClientSage.SelectedItem;

                            Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                            if (CustomerRepository.ExistSage(F_COMPTET.cbMarq) == false)
                            {
                                Loading loadscreen = new Loading();
                                loadscreen.Show();
                                Core.Transfert.TransfertSageClient Sync = new Core.Transfert.TransfertSageClient();
                                Sync.Exec(F_COMPTET.cbMarq, out log);
                                loadscreen.Close();
                                if (log.Count > 0)
                                    Core.Log.SendLog(log, Core.Log.LogIdentifier.TransfertClient);

                                this.ButtonSearchSageToPrestashop_Click(this.BtTransfert, new RoutedEventArgs());
                            }
                            else
                                MessageBox.Show("Compte client déjà transféré !", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                            MessageBox.Show("Erreur de sélection du compte client Sage !", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("La clé cookie de votre site Prestashop n'est pas renseignée dans la configuration de Prestaconnect, création du compte client dans Prestashop impossible !", "Clé cookie non renseignée", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    Core.Error.SendMailError(ex.ToString());
                }
            }
        }

        // <JG> 06/09/2012 ajout filtre de recherche
        // <JG> 28/02/2013 modification transfert client avec utilisation des paramètre de la fonctionnalité automatique
        private void ButtonSearchSageToPrestashop_Click(object sender, RoutedEventArgs e)
        {
            this.CBClientSage.ItemsSource = new List<Model.Sage.F_COMPTET_Light>();
            // <JG> 04/12/2012 Correction utilisation des champs de recherche
            if (this.CbGroupeClient.SelectedItem != null)
            {
                //this.CBClientSage.Items.Clear();
                this.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;
                Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                List<Model.Sage.F_COMPTET_Light> ListF_COMPTET;
                ListF_COMPTET = F_COMPTETRepository.ListLight(0, 0);

                if (this.TextBoxSageToPrestashopNumero.Text.ToUpper() != "")
                    ListF_COMPTET = ListF_COMPTET.Where(s => s.CT_Num.StartsWith(this.TextBoxSageToPrestashopNumero.Text.ToUpper())).ToList();

                //// <JG> 26/12/2012 ajout filtre catégorie tarifaire
                //Model.Local.GroupRepository GroupRepository = new Model.Local.GroupRepository();
                //if (GroupRepository.ListCatTarifSage().Count > 0)
                //{
                //    List<Model.Sage.F_COMPTET> temp = new List<Model.Sage.F_COMPTET>();
                //    foreach (Int32 CatTarifID in GroupRepository.ListCatTarifSage())
                //    {
                //        temp.AddRange(ListF_COMPTET.Where(c => c.N_CatTarif == CatTarifID));
                //        ListF_COMPTET.RemoveAll(c => c.N_CatTarif == CatTarifID);
                //    }
                //    ListF_COMPTET = temp;

                // <JG> 19/02/2013 correction filtrage des clients sur le groupe sélectionné
                if (((Model.Local.Group)this.CbGroupeClient.SelectedItem).Grp_CatTarifId != null)
                {
                    ListF_COMPTET = ListF_COMPTET.Where(c => c.N_CatTarif == (int)((Model.Local.Group)this.CbGroupeClient.SelectedItem).Grp_CatTarifId).ToList();

                    // <JG> 06/09/2012 ajout filtre sur l'intitulé
                    if (this.TextBoxSageToPrestashopIntitule.Text != null && this.TextBoxSageToPrestashopIntitule.Text.ToUpper().Trim() != "")
                    {
                        ListF_COMPTET = ListF_COMPTET.Where(cpt => cpt.CT_Intitule.ToUpper().Contains(this.TextBoxSageToPrestashopIntitule.Text.ToUpper().Trim())).ToList();
                    }

                    //ListF_COMPTET = ListF_COMPTET.Where(cpt => cpt.CT_EMail != null && cpt.CT_EMail.Trim() != "").ToList();
                    ListF_COMPTET = ListF_COMPTET.Where(cpt => cpt.CT_EMail.Trim() != "" || cpt.F_LIVRAISON_Principale().Count(a => a.LI_EMail != "") > 0).ToList();

                    List<Model.Local.Customer> LocalCustomer = new Model.Local.CustomerRepository().List();
                    ListF_COMPTET = ListF_COMPTET.Where(s => LocalCustomer.Count(l => l.Sag_Id == s.cbMarq) == 0).ToList();

                    if (ListF_COMPTET.Count == 0)
                    {
                        if (sender != this.BtTransfert)
                            MessageBox.Show("Aucun client dans le résultat de votre recherche !", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if (ListF_COMPTET.Count <= 200)
                    {
                        Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                        Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();
                        this.CBClientSage.ItemsSource = ListF_COMPTET;

                        //foreach (Model.Sage.F_COMPTET_Light F_COMPTET in ListF_COMPTET)
                        //    if (CustomerRepository.ExistSage(F_COMPTET.cbMarq) == false) // && F_LIVRAISONRepository.ExistComptetPrincipal(F_COMPTET.CT_Num, 1))
                        //        this.CBClientSage.Items.Add(F_COMPTET.ComboText);
                    }
                    else
                        MessageBox.Show("Le nombre de résultats de la recherche est trop important, veuillez préciser d'avantage votre recherche !", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }


                Mouse.OverrideCursor = Cursors.Arrow;
                this.IsEnabled = true;
            }
            else
                MessageBox.Show("Veuillez sélectionner un groupe de client !", "Groupe de client", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void CbGroupeClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CBClientSage.ItemsSource = new List<Model.Sage.F_COMPTET_Light>();
        }
        #endregion

        private void listBoxBTOBCustomerSage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ButtonLinkAccount.IsEnabled = (this.listBoxBTOBCustomerPrestashop.SelectedItem != null
                && this.listBoxBTOBCustomerSage.SelectedItem != null);
        }

        private void ButtonOustanding_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Global.GetConfig().ModuleAECCustomerOutstandingActif)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                PRESTACONNECT.AECCustomerOutstanding AECCustomerOutstanding = new AECCustomerOutstanding();
                AECCustomerOutstanding.ShowDialog();

                Loading.Close();
            }
        }

        private void ButtonSynchroGroupCRisque_Click(object sender, RoutedEventArgs e)
        {
            SynchroGroupCRisque form = new SynchroGroupCRisque();
            form.ShowDialog();
        }

        private void buttonAECRepresentative_Click(object sender, RoutedEventArgs e)
        {
            AECRepresentative form = new AECRepresentative();
            form.ShowDialog();
            AECRepresentativeCustomer form2 = new AECRepresentativeCustomer();
            form2.ShowDialog();
        }

        private void buttonAECBalance_Click(object sender, RoutedEventArgs e)
        {
            AECBalanceOutstanding form = new AECBalanceOutstanding();
            form.ShowDialog();
            AECBalanceAccounting form2 = new AECBalanceAccounting();
            form2.ShowDialog();
        }

        private void ButtonPortfolioCustomerEmployee_Click(object sender, RoutedEventArgs e)
        {
            SynchroPortfolioCustomerEmployee form = new SynchroPortfolioCustomerEmployee();
            form.ShowDialog();
        }
    }
}