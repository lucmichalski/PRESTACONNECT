using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PRESTACONNECT.Contexts;
using PRESTACONNECT.Core;

namespace PRESTACONNECT
{
    public partial class MainWindow : Window
    {
        #region Properties

        internal new MainContext DataContext
        {
            get { return (MainContext)base.DataContext; }
            private set
            {
                if (((MainContext)base.DataContext) != null)
                {
                    ((MainContext)base.DataContext).ShopsLoaded -= new EventHandler(DataContext_ShopsLoaded);
                }

                base.DataContext = value;

                if (((MainContext)base.DataContext) != null)
                {
                    ((MainContext)base.DataContext).ShopsLoaded += new EventHandler(DataContext_ShopsLoaded);
                }
            }
        }

        #endregion
        #region Event methods

        private void DataContext_ShopsLoaded(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Core.Error.SendMailError(ex.ToString());
            }
        }

        #endregion

        public MainWindow()
        {
            try
            {
                Core.UpdateVersion.LicenseIsActive = ReadLicence();
                if (Core.UpdateVersion.LicenseIsActive == UpdateVersion.LicenceActivation.enabled)
                {
                    DataContext = new MainContext();

                    InitializeComponent();

                    this.Title = (!Core.UpdateVersion.License.ExtranetOnly)
                        ? "Prestaconnect pour Sage " + Core.UpdateVersion.SageFolder(Core.UpdateVersion.CurrentSageVersion)
                            + " et Prestashop " + Core.UpdateVersion.PrestaShopFolder(Core.UpdateVersion.CurrentPrestaShopVersion)
                        : "Module Extranet pour Sage " + Core.UpdateVersion.SageFolder(Core.UpdateVersion.CurrentSageVersion);

                    if (Core.UpdateVersion.License.ExtranetOnly)
                    {
                        Core.Global.GetConfig().UpdateConfigBtoCBtoB(false, true);
                        Core.Global.GetConfig().UpdateConfigMailActive(true);
                    }

                    this.SyncSupply();

                    String[] Args = Environment.GetCommandLineArgs();
                    if (Args.Length > 1)
                    {
                        Core.Global.UILaunch = false;
                        this.ExecWithArgs(Args);
                    }
                    else
                    {
                        Core.Global.UILaunch = true;

                        #region Theme

                        Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                        if (ConfigRepository.ExistName(Core.Global.ConfigTheme))
                        {
                            Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigTheme);
                            foreach (ComboBoxItem ComboBoxItem in this.ComboBoxTheme.Items)
                            {
                                if (ComboBoxItem.Content.ToString() == Config.Con_Value)
                                {
                                    this.ComboBoxTheme.SelectedItem = ComboBoxItem;
                                    break;
                                }
                            }
                        }

                        #endregion

                        #region Infos Prestashop

                        Model.Prestashop.PsConfigurationRepository PsConfigurationRepository = new Model.Prestashop.PsConfigurationRepository();
                        Model.Prestashop.PsConfiguration PsConfiguration = new Model.Prestashop.PsConfiguration();

                        PsConfiguration = new Model.Prestashop.PsConfiguration();

                        Model.Prestashop.PsShopURLRepository PsShopURLRepository = new Model.Prestashop.PsShopURLRepository();
                        Model.Prestashop.PsShopURL PsShopURL = PsShopURLRepository.ReadShopId(Global.CurrentShop.IDShop);

                        //TODO vérification du domaine pour création hyperlien
                        if (PsShopURL != null)
                        {
                            Global.URL_Prestashop = String.Format("http://{0}{1}{2}", PsShopURL.Domain, PsShopURL.PhysicalUri, PsShopURL.VirtualUri);
                            while (Global.URL_Prestashop.EndsWith("/"))
                            {
                                Global.URL_Prestashop = Global.URL_Prestashop.Substring(0, Global.URL_Prestashop.Length - 1);
                            }

                            string logoUri = string.Empty;
                            //try
                            //{
                            //    if (PsConfigurationRepository.ExistNameShop(Core.Global.PrestaShopLogo))
                            //    {
                            //        logoUri = "img/" + PsConfigurationRepository.ReadNameShop(Core.Global.PrestaShopLogo).Value;
                            //    }
                            //}
                            //catch (Exception)
                            {
                                if (PsConfigurationRepository.ExistName(Core.Global.PrestaShopLogo))
                                {
                                    logoUri = "/img/" + PsConfigurationRepository.ReadName(Core.Global.PrestaShopLogo).Value;
                                }
                                else if (PsConfigurationRepository.ExistNameShop(Core.Global.PrestaShopLogo))
                                {
                                    logoUri = "/img/" + PsConfigurationRepository.ReadNameShop(Core.Global.PrestaShopLogo).Value;
                                }
                                else
                                {
                                    logoUri = (PsShopURL.IDShop == 1) ? "/img/logo.jpg" : "/img/logo-" + PsShopURL.IDShop + ".jpg";
                                }
                            }

                            LabelPrestashopLink.NavigateUri = new Uri(Global.URL_Prestashop);
                            LabelPrestashopLink.ToolTip = Global.URL_Prestashop;
                            ImagePrestashopLogo.Source = new BitmapImage(new Uri(Global.URL_Prestashop + logoUri));
                            ImagePrestashopLogo.ToolTip = Global.URL_Prestashop;
                        }

                        #endregion

                        #region liste commandes/clients

                        this.LoadOrdersAndCustomers();

                        #endregion

                        this.ReadVersion();

                        this.ActiveSupplierMenuItem();

                        if (Core.Global.GetConfig().UIMaximizeWindow)
                        {
                            this.WindowState = System.Windows.WindowState.Maximized;
                            Core.Temp.Current = System.Windows.WindowState.Maximized;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext.LoadShops();
        }

        #region Theme
        private void ComboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String Theme = this.ComboBoxTheme.SelectedItem.ToString();
            Theme = Theme.Replace("System.Windows.Controls.ComboBoxItem: ", "");
            string uri = @"\ReuxablesLegacy;component\" + Theme + ".xaml";
            ((App)Application.Current).ChangeTheme(new Uri(uri, UriKind.Relative));
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            Boolean isConfig = false;
            if (ConfigRepository.ExistName(Core.Global.ConfigTheme))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigTheme);
                isConfig = true;
            }
            Config.Con_Value = Theme;
            if (isConfig == true)
            {
                ConfigRepository.Save();
            }
            else
            {
                Config.Con_Name = Core.Global.ConfigTheme;
                ConfigRepository.Add(Config);
            }
        }
        #endregion

        #region Boutique

        private void HyperlinkLabelPrestashopLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Global.GoShop();
            e.Handled = true;
        }

        private void Shops_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Global.CurrentShop = DataContext.SelectedShop;

            //Model.Prestashop.PsShopGroupRepository PsShopGroupRepository = new Model.Prestashop.PsShopGroupRepository();
            //Global.CurrentGroup = PsShopGroupRepository.ReadId(Global.CurrentShop.IDShopGroup);
        }

        #endregion

        #region Button

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ActualiserDernieresCommandes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;

                this.LoadOrdersAndCustomers();
            }
            finally
            {
                Mouse.OverrideCursor = null;
                this.IsEnabled = true;
            }
        }

        private void LoadOrdersAndCustomers()
        {
            PRESTACONNECT.Commande.ReadCommandePrestashop(DataGridCommande, 20);

            Core.Temp.ListLocalCustomer = new Model.Local.CustomerRepository().List();
            Core.Temp.ListF_COMPTET_BtoB = new Model.Sage.F_COMPTETRepository().ListBtoB((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client);

            Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
            this.DataGridClient.ItemsSource = (Core.Global.GetConfig().ConfigClientFiltreCommande)
                ? PsCustomerRepository.ListTopActiveOrderByDateAdd(20, 1, Core.Global.CurrentShop.IDShop)
                : PsCustomerRepository.ListTopActiveOrderByDateAddWithOrder(20, 1, Global.CurrentShop.IDShop);
        }

        #endregion

        #region Menu

        private void MenuItemFournisseur_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Fournisseur Fournisseur = new Fournisseur();
            Loading.Close();
            Fournisseur.ShowDialog();
        }
        private void MenuItemCatalogue_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Catalogue Catalogue = new Catalogue();
            Loading.Close();
            Catalogue.ShowDialog();
        }
        private void MenuItemGamme_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Gamme Gamme = new Gamme();
            Loading.Close();
            Gamme.ShowDialog();
        }
        private void MenuItemArticle_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            //PRESTACONNECT.ArticleListe ArticleListe = new ArticleListe();
            //ArticleListe.Show();
            PRESTACONNECT.Articles view = new Articles();
            Loading.Close();
            view.ShowDialog();
        }
        private void MenuItemGADA_Click(object sender, RoutedEventArgs e)
        {
            string gada = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "GADA\\GADA.exe");
            if (System.IO.File.Exists(gada))
            {
                System.Diagnostics.Process.Start(gada);
            }
        }
        private void MenuItemClient_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Client Client = new Client();
            Loading.Close();
            Client.ShowDialog();
        }
        private void MenuItemCommande_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Commande Commande = new Commande();
            Loading.Close();
            Commande.ShowDialog();
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Import Import = new Import();
            Loading.Close();
            Import.ShowDialog();
        }
        private void MenuItemConfiguration_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Configuration Configuration = new Configuration();
            Loading.Close();
            Configuration.ShowDialog();

            this.ActiveSupplierMenuItem();
        }
        private void ButtonABout_Click(object sender, RoutedEventArgs e)
        {
            About About = new About();
            About.ShowDialog();
        }

        private void ActiveSupplierMenuItem()
        {
            if (Core.Global.GetConfig().ArticleTransfertInfosFournisseurActif
                && this.MenuItemArticle.Width != 160
                && !Core.UpdateVersion.License.ExtranetOnly)
            {
                this.GridMenu.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(20, GridUnitType.Star),
                });
                this.MenuItemArticle.Width = 160;
                this.MenuItemCatalogue.Width = 160;
                this.MenuItemClient.Width = 160;
                this.MenuItemGamme.Width = 160;
                this.MenuItemCommande.Width = 160;
                this.MenuItemFournisseur.Visibility = System.Windows.Visibility.Visible;
            }
            else if (!Core.Global.GetConfig().ArticleTransfertInfosFournisseurActif
                && this.MenuItemArticle.Width != 170)
            {
                this.GridMenu.ColumnDefinitions.Remove(this.GridMenu.ColumnDefinitions.Last());
                this.MenuItemArticle.Width = 170;
                this.MenuItemCatalogue.Width = 170;
                this.MenuItemClient.Width = 170;
                this.MenuItemGamme.Width = 170;
                this.MenuItemCommande.Width = 170;
                this.MenuItemFournisseur.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        #endregion

        #region Sync
        private void SyncSupply()
        {
            ArrayList ArrayListSupply = new ArrayList();
            Model.Local.Supply Supply;
            Model.Sage.F_DEPOTRepository F_DEPOTRepository = new Model.Sage.F_DEPOTRepository();
            List<Model.Sage.F_DEPOT> ListDepot = F_DEPOTRepository.ListOrderByIntitule();
            Model.Local.SupplyRepository SupplyRepository = new Model.Local.SupplyRepository();
            foreach (Model.Sage.F_DEPOT Depot in ListDepot)
            {
                Supply = new Model.Local.Supply();
                if (SupplyRepository.ExistSage(Depot.DE_No == null ? 0 : Depot.DE_No))
                {
                    Supply = SupplyRepository.ReadSage(Depot.DE_No == null ? 0 : Depot.DE_No);
                    Supply.Sup_Name = Depot.DE_Intitule;
                    SupplyRepository.Save();
                }
                else
                {
                    Supply.Sup_Name = Depot.DE_Intitule;
                    Supply.Sup_Active = false;
                    Supply.Sag_Id = Depot.DE_No == null ? 0 : Depot.DE_No; 
                    SupplyRepository.Add(Supply);
                }
                ArrayListSupply.Add(Depot.DE_No);
            }
            List<Model.Local.Supply> ListSupply = SupplyRepository.List();
            foreach (Model.Local.Supply SupplyContains in ListSupply)
            {
                if (ArrayListSupply.Contains(SupplyContains.Sag_Id) == false)
                {
                    SupplyRepository.Delete(SupplyContains);
                }
            }
        }
        #endregion

        #region ExecWithArgs
        private void ExecWithArgs(String[] Args)
        {
            foreach (String Arg in Args)
            {
                try
                {
                    switch (Arg)
                    {
                        case Core.Task.TaskExecSQL:
                            Core.Task.ExecSQL();
                            break;

                        case Core.Task.TaskSynchroStock:
                            Core.Task.ExecArgsStock();
                            break;
                        case Core.Task.TaskSynchroPrice:
                        case Core.Task.TaskSynchroStockPrice:
                            Core.Task.ExecArgsStockPrice();
                            break;
                        case Core.Task.TaskSynchroClient:
                            Core.Task.ExecArgsClient();
                            break;
                        case Core.Task.TaskSynchroArticle:
                            Core.Task.ExecArgsArticle();
                            break;
                        case Core.Task.TaskSynchroGamme:
                            Core.Task.ExecArgsGamme();
                            break;
                        case Core.Task.TaskSynchroConditionnement:
                            Core.Task.ExecArgsConditionnement();
                            break;
                        case Core.Task.TaskSynchroCatalogue:
                            Core.Task.ExecArgsCatalogue();
                            break;
                        case Core.Task.TaskSynchroFournisseur:
                            Core.Task.ExecArgsFournisseur();
                            break;
                        case Core.Task.TaskSynchroDocument:
                            Core.Task.ExecArgsDocument();
                            break;
                        case Core.Task.TaskSynchroDocumentSageJour:
                            Core.Task.ExecArgsDocumentSage(DateTime.Now.Date);
                            break;
                        case Core.Task.TaskSynchroDocumentSageSemaine:
                            Core.Task.ExecArgsDocumentSage(DateTime.Now.Date.AddDays(-7));
                            break;
                        case Core.Task.TaskSynchroDocumentSageMois:
                            Core.Task.ExecArgsDocumentSage(DateTime.Now.Date.AddMonths(-1));
                            break;
                        case Core.Task.TaskSynchroStatutCommande:
                            Core.Task.ExecArgsStatutCommande();
                            break;
                        case Core.Task.TaskSynchroStatutCommandeNJours:
                            Core.Task.ExecArgsStatutCommandeNJours();
                            break;
                        case Core.Task.TaskSynchroGlobale:
                            Core.Task.ExecArgsSynchroGlobale();
                            break;
                        case Core.Task.TaskSynchroImageMedia:
                            Core.Task.ExecArgsSynchroImageMedia();
                            break;

                        case Core.Task.TaskRelanceAnnulationCommandePrestashop:
                            Core.Task.ExecArgsRelanceAnnulationCommandePrestashop();
                            break;

                        case Core.Task.TaskSynchronisationPaiementDiffere:
                            Core.Task.ExecArgsSynchronisationPaiementDiffere();
                            break;
                        case Core.Task.TaskSynchroPaiementJour:
                            Core.Task.ExecArgsSynchronisationPaiementDiffere(DateTime.Now.Date);
                            break;
                        case Core.Task.TaskSynchroPaiementSemaine:
                            Core.Task.ExecArgsSynchronisationPaiementDiffere(DateTime.Now.Date.AddDays(-7));
                            break;
                        case Core.Task.TaskSynchroPaiementMois:
                            Core.Task.ExecArgsSynchronisationPaiementDiffere(DateTime.Now.Date.AddMonths(-1));
                            break;

                        case Core.Task.TaskSynchroGroupCatTarif:
                            Core.Task.ExecArgsSynchroGroupCatTarif();
                            break;
                        case Core.Task.TaskSynchroEncoursClient:
                            Core.Task.ExecArgsSynchroEncoursClient();
                            break;
                        case Core.Task.TaskSynchroGroupCodeRisque:
                            Core.Task.ExecArgsSynchroGroupCodeRisque();
                            break;
                        case Core.Task.TaskSynchroPortfolioCustomerEmployee:
                            Core.Task.ExecArgsSynchroPortfolio();
                            break;

                        //import automatique
                        case Core.Task.TaskImportSageCatalogue:
                            Core.Task.ExecArgsImportSageCatalogue();
                            break;
                        case Core.Task.TaskImportSageArticle:
                            Core.Task.ExecArgsImportSageArticle();
                            break;
                        case Core.Task.TaskImportCompositionGammes:
                            Core.Task.ExecArgsImportCompositionGammes();
                            break;
                        case Core.Task.TaskImportImage:
                            Core.Task.ExecArgsImportImage();
                            break;
                        case Core.Task.TaskImportDocument:
                            Core.Task.ExecArgsImportDocument();
                            break;
                        case Core.Task.TaskImportSageComplet:
                            Core.Task.ExecArgsImportSageComplet();
                            break;
                        case Core.Task.TaskImportSageMedia:
                            Core.Task.ExecArgsImportSageMedia();
                            break;
                        case Core.Task.TaskImportSageStatInfolibreArticle:
                            Core.Task.ExecArgsImportSageStatInfolibreArticle();
                            break;
                        case Core.Task.TaskImportSageStatInfolibreClient:
                            Core.Task.ExecArgsTransfertSageStatInfolibreClient();
                            break;
                        case Core.Task.TaskImportSageCatalogueInfolibre:
                            Core.Task.ExecArgsImportSageCatalogueInfolibre();
                            break;

                        case Core.Task.TaskGestionStatutArticle:
                            Core.Task.ExecArgsGestionStatutArticle();
                            break;

                        // <JG> 21/08/2017
                        case Core.Task.TaskTransfertAttribute:
                            Core.Task.ExecArgsTransfertAttribute();
                            break;
                        case Core.Task.TaskTransfertFeature:
                            Core.Task.ExecArgsTransfertFeature();
                            break;

                        // <JG> 28/02/2013
                        case Core.Task.TaskTransfertSageClient:
                            Core.Task.ExecArgsTransfertSageClient();
                            break;

                        // Import depuis PrestaShop
                        case Core.Task.TaskImportPrestashopCaracteristiqueArticle:
                            Core.Task.ExecArgsImportPrestashopCaracteristiqueArticle();
                            break;

                        // Export des factures en PDF
                        case Core.Task.TaskExportFacture:
                            Core.Task.ExecArgsExportFacture();
                            break;
                        case Core.Task.TaskExportFactureSemaine:
                            Core.Task.ExecArgsExportFacture(DateTime.Now.Date.AddDays(-8), DateTime.Now.Date.AddDays(-1));
                            break;
                        case Core.Task.TaskExportFactureMois:
                            Core.Task.ExecArgsExportFacture(DateTime.Now.Date.AddMonths(-1), DateTime.Now.Date.AddDays(-1));
                            break;
                        case Core.Task.TaskExportFactureAnnee:
                            Core.Task.ExecArgsExportFacture(DateTime.Now.Date.AddYears(-1), DateTime.Now.Date.AddDays(-1));
                            break;

                        // Export des informations des collaborateurs
                        case Core.Task.TaskExportCollaborateur:
                            Core.Task.ExecArgsExportCollaborateur();
                            break;

                        // Export des informations de paiement
                        case Core.Task.TaskExportPayement:
                            Core.Task.ExecArgsExportPayement();
                            break;

                        case Core.Task.TaskExportInfosClient:
                            Core.Task.ExecArgsExportInfosClient();
                            break;

                        case Core.Task.TaskExportAECRepresentative:
                            Core.Task.ExecArgsExportAECRepresentative();
                            break;

                        case Core.Task.TaskExportAECBalance:
                            Core.Task.ExecArgsExportAECBalance();
                            break;

                        // <JG> 21/08/2017 traitements des arguments avec options
                        default:
                            if (Arg.StartsWith(Core.Task.TaskImportSageStatInfolibreArticle))
                            {
                                Core.Task.ExecArgsImportSageStatInfolibreArticleFiltres(Arg);
                            }
                            if (Arg.StartsWith(Core.Task.TaskTransfertFeature))
                            {
                                Core.Task.ExecArgsTransfertFeatureFilters(Arg);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Core.Error.SendMailError(ex.ToString());
                }
            }
            try
            {
                Core.Log.CloseLog(Core.Log.LogStreamType.LogStream);
                Core.Log.CloseLog(Core.Log.LogStreamType.LogChronoStream);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        #endregion

        #region WS

        private Core.UpdateVersion.LicenceActivation ReadLicence()
        {
            Core.UpdateVersion.LicenceActivation isLicence = UpdateVersion.LicenceActivation.disabled;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(Core.Global.ValidateRemoteCertificate);
                WSKEY.WSKeySoapClient WSKEY = new WSKEY.WSKeySoapClient();
                WSKEY.ClientCredentials.UserName.UserName = "wsprestaconnect";
                WSKEY.ClientCredentials.UserName.Password = "WpaE@PHlya-98s!#G";
                Boolean ExistKey = WSKEY.ExistKey(Properties.Settings.Default.LICENCEKEY);
                if (ExistKey == true)
                {
                    DateTime Now = DateTime.Now.Date;
                    Core.UpdateVersion.License = WSKEY.ReadKey(Properties.Settings.Default.LICENCEKEY);
                    if (Core.UpdateVersion.License.DUADate.AddDays(1).Date > Now)
                    {
                        if (Core.UpdateVersion.License.BDDPrestaconnect != Core.Global.GetConnectionInfos().PrestaconnectDatabase
                            || Core.UpdateVersion.License.BDDSage != Core.Global.GetConnectionInfos().SageDatabase
                            || Core.UpdateVersion.License.BDDPrestashop != Core.Global.GetConnectionInfos().PrestashopDatabase
                            || Core.UpdateVersion.License.Domain != Core.Global.GetConnectionInfos().PrestashopServer)
                        {
                            isLicence = Core.UpdateVersion.LicenceActivation.incorrect;
                        }
                        else
                        {
                            isLicence = Core.UpdateVersion.LicenceActivation.enabled;
                        }
                    }
                    else
                    {
                        isLicence = Core.UpdateVersion.LicenceActivation.expired;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
                Core.Error.SendMailWS(ex);
            }
            // WSRESCUE
            if (isLicence == UpdateVersion.LicenceActivation.disabled)
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(Core.Global.ValidateRemoteCertificate);
                    WSKEYRESCUE.WSKeySoapClient WSKEYRESCUE = new WSKEYRESCUE.WSKeySoapClient();
                    WSKEYRESCUE.ClientCredentials.UserName.UserName = "wsprestaconnect";
                    WSKEYRESCUE.ClientCredentials.UserName.Password = "F78Ro4laUw3";
                    Boolean ExistKey = WSKEYRESCUE.ExistKey(Properties.Settings.Default.LICENCEKEY);
                    if (ExistKey == true)
                    {
                        DateTime Now = DateTime.Now.Date;
                        PRESTACONNECT.WSKEYRESCUE.Key Key = WSKEYRESCUE.ReadKey(Properties.Settings.Default.LICENCEKEY);
                        // recréation d'un objet licence selon la définition du serveur principal (nécessaire pour les fonctionnalités se basant sur les droits de la licence)
                        Core.UpdateVersion.License = new WSKEY.Key()
                        {
                            BDDPrestaconnect = Key.BDDPrestaconnect,
                            BDDPrestashop = Key.BDDPrestashop,
                            BDDSage = Key.BDDSage,
                            Dealer = Key.Dealer,
                            Domain = Key.Domain,
                            DUADate = Key.DUADate,
                            LicenseKey = Key.LicenseKey,
                            Option1 = Key.Option1,
                            Option2 = Key.Option2,
                            Option3 = Key.Option3,
                            Organization = Key.Organization,
                            Prestashop = Key.Prestashop,
                            Product = Key.Product,
                            Sage = Key.Sage,
                        };
                        if (Key.DUADate.AddDays(1).Date >= Now)
                        {
                            if (Core.UpdateVersion.License.BDDPrestaconnect != Core.Global.GetConnectionInfos().PrestaconnectDatabase
                                || Core.UpdateVersion.License.BDDSage != Core.Global.GetConnectionInfos().SageDatabase
                                || Core.UpdateVersion.License.BDDPrestashop != Core.Global.GetConnectionInfos().PrestashopDatabase
                                || Core.UpdateVersion.License.Domain != Core.Global.GetConnectionInfos().PrestashopServer)
                            {
                                isLicence = Core.UpdateVersion.LicenceActivation.incorrect;
                            }
                            else
                            {
                                isLicence = Core.UpdateVersion.LicenceActivation.enabled;
                            }
                        }
                        else
                        {
                            isLicence = Core.UpdateVersion.LicenceActivation.expired;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Core.Error.SendMailError(ex.ToString());
                }
            }

            return isLicence;
        }

        private void ReadVersion()
        {
            try
            {
                if (Core.UpdateVersion.License != null)
                {
                    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(Core.Global.ValidateRemoteCertificate);
                    WSVERSION.WSVersionSoapClient WSVERSION = new WSVERSION.WSVersionSoapClient();
                    WSVERSION.ClientCredentials.UserName.UserName = "couscousparty\\wsprestaconnect";
                    WSVERSION.ClientCredentials.UserName.Password = "WpaE@PHlya-98s!#G";
                    string product_name = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
                    if (WSVERSION.ExistVersionProduct(product_name, Core.UpdateVersion.CurrentVersion, Core.UpdateVersion.License.Sage, Core.UpdateVersion.License.Prestashop))
                    {
                        if (CountProcessus() > 1)
                        {
                            View.PrestaMessage MsgProcessus = new View.PrestaMessage("Mise-à-jour du logiciel détectée !"
                                + "\n\rFermez toutes les instances pour permettre son installation !",
                                "Mise à jour détectée", MessageBoxButton.OK, MessageBoxImage.Warning);
                            MsgProcessus.ShowDialog();
                        }
                        else
                        {
                            View.PrestaMessage MsgBox = new View.PrestaMessage("Mise-à-jour du logiciel disponible, souhaitez-vous l'installer ?"
                                + "\n\rL'application s'exécutera après le téléchargement et l'installation des mises à jours.",
                                "Mise à jour disponible", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            MsgBox.ShowDialog();
                            if (MsgBox.DialogResult == true)
                            {
                                Core.UpdateVersion.LaunchUpdate = true;
                                PRESTACONNECT.WSVERSION.Version version = WSVERSION.ReadVersionProduct(product_name, Core.UpdateVersion.CurrentVersion, Core.UpdateVersion.License.Sage, Core.UpdateVersion.License.Prestashop);
                                System.Diagnostics.Process Process = new System.Diagnostics.Process();
                                Process.StartInfo.FileName = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Alternetis_Updater.exe");
                                Process.StartInfo.Arguments = Core.UpdateVersion.UpdaterArguments(version);
                                Process.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private int CountProcessus()
        {
            int cpt = 0;
            List<System.Diagnostics.Process> list = new List<System.Diagnostics.Process>(System.Diagnostics.Process.GetProcesses());
            foreach (System.Diagnostics.Process proc in list.Where(p => p.ProcessName.StartsWith("PRESTACONNECT")))
            {
                if (proc.MainModule.FileName == System.Windows.Forms.Application.ExecutablePath)
                    cpt += 1;
            }
            return cpt;
        }

        #endregion

        private void Window_StateChanged(object sender, EventArgs e)
        {
            Core.Temp.Current = this.WindowState;
        }

        private void ImagePrestashopLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Global.GoShop();
        }
    }
}