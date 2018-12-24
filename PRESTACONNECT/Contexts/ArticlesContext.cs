using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PRESTACONNECT.Core;
using PRESTACONNECT.Model.Internal;
using System.Windows.Controls;

namespace PRESTACONNECT.Contexts
{
    internal sealed class ArticlesContext : Context
    {
        #region Properties

        private BackgroundWorker loadCatalogsWorker;
        private BackgroundWorker LoadCatalogsWorker
        {
            get { return loadCatalogsWorker; }
            set
            {
                if (loadCatalogsWorker != null)
                {
                    loadCatalogsWorker.DoWork -= new DoWorkEventHandler(LoadCatalogsWorker_DoWork);
                    loadCatalogsWorker.ProgressChanged -= new ProgressChangedEventHandler(LoadCatalogsWorker_ProgressChanged);
                    loadCatalogsWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadCatalogsWorker_RunWorkerCompleted);
                }

                loadCatalogsWorker = value;

                if (loadCatalogsWorker != null)
                {
                    loadCatalogsWorker.DoWork += new DoWorkEventHandler(LoadCatalogsWorker_DoWork);
                    loadCatalogsWorker.ProgressChanged += new ProgressChangedEventHandler(LoadCatalogsWorker_ProgressChanged);
                    loadCatalogsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadCatalogsWorker_RunWorkerCompleted);
                }
            }
        }

        private BackgroundWorker loadSelectedCatalogItemsWorker;
        private BackgroundWorker LoadSelectedCatalogItemsWorker
        {
            get { return loadSelectedCatalogItemsWorker; }
            set
            {
                if (loadSelectedCatalogItemsWorker != null)
                {
                    loadSelectedCatalogItemsWorker.DoWork -= new DoWorkEventHandler(LoadSelectedCatalogItemsWorker_DoWork);
                    loadSelectedCatalogItemsWorker.ProgressChanged -= new ProgressChangedEventHandler(LoadSelectedCatalogItemsWorker_ProgressChanged);
                    loadSelectedCatalogItemsWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadSelectedCatalogItemsWorker_RunWorkerCompleted);
                }

                loadSelectedCatalogItemsWorker = value;

                if (loadSelectedCatalogItemsWorker != null)
                {
                    loadSelectedCatalogItemsWorker.DoWork += new DoWorkEventHandler(LoadSelectedCatalogItemsWorker_DoWork);
                    loadSelectedCatalogItemsWorker.ProgressChanged += new ProgressChangedEventHandler(LoadSelectedCatalogItemsWorker_ProgressChanged);
                    loadSelectedCatalogItemsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadSelectedCatalogItemsWorker_RunWorkerCompleted);
                }
            }
        }

        private ObservableCollection<Model.Local.Catalog> catalogs;
        public ObservableCollection<Model.Local.Catalog> Catalogs
        {
            get { return catalogs; }
            set
            {
                catalogs = value;
                OnPropertyChanged("Catalogs");
            }
        }

        private Model.Local.Catalog selectedCatalog;
        public Model.Local.Catalog SelectedCatalog
        {
            get { return selectedCatalog; }
            set
            {
                selectedCatalog = value;
                OnPropertyChanged("SelectedCatalog");
            }
        }

        private String searchItem;
        public String SearchItem
        {
            get { return searchItem; }
            set
            {
                searchItem = value;
                OnPropertyChanged("SearchItem");
            }
        }

        private Boolean searchWithoutCatalogs;
        public Boolean SearchWithoutCatalogs
        {
            get { return searchWithoutCatalogs; }
            set
            {
                searchWithoutCatalogs = value;
                OnPropertyChanged("SearchWithoutCatalogs");
            }
        }

        private ObservableCollection<Model.Local.Article> selectedCatalogItems;
        public ObservableCollection<Model.Local.Article> SelectedCatalogItems
        {
            get { return selectedCatalogItems; }
            set
            {
                selectedCatalogItems = value;
                OnPropertyChanged("SelectedCatalogItems");
            }
        }

        private Model.Local.Article selectedItem;
        public Model.Local.Article SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        private Boolean full;
        public Boolean Full
        {
            get { return full; }
            set { full = value; OnPropertyChanged("Full"); }
        }
        private Boolean partial;
        public Boolean Partial
        {
            get { return partial; }
            set { partial = value; OnPropertyChanged("Partial"); }
        }

        private Boolean ftpActive;
        public Boolean FtpActive
        {
            get { return ftpActive; }
            set { ftpActive = value; OnPropertyChanged("FtpActive"); }
        }

        public Boolean ConditioningActive
        {
            get { return Core.Global.GetConfig().ArticleImportConditionnementActif; }
        }

        #region Filtres

        private ObservableCollection<ProductFilterActiveDefault> listProductFilterActiveDefault;
        public ObservableCollection<ProductFilterActiveDefault> ListProductFilterActiveDefault
        {
            get { return listProductFilterActiveDefault; }
            set { listProductFilterActiveDefault = value; OnPropertyChanged("ListProductFilterActiveDefault"); }
        }

        private ProductFilterActiveDefault selectedProductFilterActiveDefault;
        public ProductFilterActiveDefault SelectedProductFilterActiveDefault
        {
            get { return selectedProductFilterActiveDefault; }
            set { selectedProductFilterActiveDefault = value; OnPropertyChanged("SelectedProductFilterActiveDefault"); }
        }

        private DateTime? filtreDateStart = null;
        public DateTime? FiltreDateStart
        {
            get { return filtreDateStart; }
            set { filtreDateStart = value; OnPropertyChanged("FiltreDateStart"); }
        }
        private DateTime? filtreDateEnd = null;
        public DateTime? FiltreDateEnd
        {
            get { return filtreDateEnd; }
            set { filtreDateEnd = value; OnPropertyChanged("FiltreDateEnd"); }
        }

        private Boolean showStandardProducts = true;
        public Boolean ShowStandardProducts
        {
            get { return showStandardProducts; }
            set { showStandardProducts = value; OnPropertyChanged("ShowStandardProducts"); }
        }
        private Boolean showAttributeProducts = true;
        public Boolean ShowAttributeProducts
        {
            get { return showAttributeProducts; }
            set { showAttributeProducts = value; OnPropertyChanged("ShowAttributeProducts"); }
        }
        private Boolean showConditioningProducts = true;
        public Boolean ShowConditioningProducts
        {
            get { return showConditioningProducts; }
            set { showConditioningProducts = value; OnPropertyChanged("ShowConditioningProducts"); }
        }
        private Boolean showCompositionProducts = true;
        public Boolean ShowCompositionProducts
        {
            get { return showCompositionProducts; }
            set { showCompositionProducts = value; OnPropertyChanged("ShowCompositionProducts"); }
        }

        // true = 3e état = indifférent
        private Boolean? showPackProducts = true;
        public Boolean? ShowPackProducts
        {
            get { return showPackProducts; }
            set { showPackProducts = value; OnPropertyChanged("ShowPackProducts"); }
        }

        private Boolean searchInComposition = false;
        public Boolean SearchInComposition
        {
            get { return searchInComposition; }
            set { searchInComposition = value; OnPropertyChanged("SearchInComposition"); }
        }

        // <JG> 30/08/2017
        private Boolean? showToSyncProducts = true;
        public Boolean? ShowToSyncProducts
        {
            get { return showToSyncProducts; }
            set { showToSyncProducts = value; OnPropertyChanged("ShowToSyncProducts"); }
        }

        #endregion

        #region Affichage Colonnes
        // ID
        public Boolean ColonneIDVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneIDVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneIDVisible(value); OnPropertyChanged("ColonneIDVisible"); OnPropertyChanged("ColonneIDVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneIDVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneIDVisible) ? Visibility.Visible : Visibility.Hidden; }
        }
        // Type
        public Boolean ColonneTypeVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneTypeVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneTypeVisible(value); OnPropertyChanged("ColonneTypeVisible"); OnPropertyChanged("ColonneTypeVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneTypeVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneTypeVisible) ? Visibility.Visible : Visibility.Hidden; }
        }
        // Nom
        public Boolean ColonneNomVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneNomVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneNomVisible(value); OnPropertyChanged("ColonneNomVisible"); OnPropertyChanged("ColonneNomVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneNomVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneNomVisible) ? Visibility.Visible : Visibility.Hidden; }
        }
        // Référence
        public Boolean ColonneReferenceVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneReferenceVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneReferenceVisible(value); OnPropertyChanged("ColonneReferenceVisible"); OnPropertyChanged("ColonneReferenceVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneReferenceVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneReferenceVisible) ? Visibility.Visible : Visibility.Hidden; }
        }
        // EAN
        public Boolean ColonneEANVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneEANVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneEANVisible(value); OnPropertyChanged("ColonneEANVisible"); OnPropertyChanged("ColonneEANVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneEANVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneEANVisible) ? Visibility.Visible : Visibility.Hidden; }
        }
        // Actif
        public Boolean ColonneActifVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneActifVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneActifVisible(value); OnPropertyChanged("ColonneActifVisible"); OnPropertyChanged("ColonneActifVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneActifVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneActifVisible) ? Visibility.Visible : Visibility.Hidden; }
        }
        // À synchroniser
        public Boolean ColonneSyncVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneSyncVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneSyncVisible(value); OnPropertyChanged("ColonneSyncVisible"); OnPropertyChanged("ColonneSyncVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneSyncVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneSyncVisible) ? Visibility.Visible : Visibility.Hidden; }
        }
        // Maj Prix
        public Boolean ColonneSyncPriceVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneSyncPriceVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneSyncPriceVisible(value); OnPropertyChanged("ColonneSyncPriceVisible"); OnPropertyChanged("ColonneSyncPriceVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneSyncPriceVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneSyncPriceVisible) ? Visibility.Visible : Visibility.Hidden; }
        }
        // Date modification
        public Boolean ColonneDateVisible
        {
            get { return Core.Global.GetConfig().ListeArticleColonneDateVisible; }
            set { Core.Global.GetConfig().UpdateListeArticleColonneDateVisible(value); OnPropertyChanged("ColonneDateVisible"); OnPropertyChanged("ColonneDateVisibility"); OnPropertyChanged("HeaderVisibility"); }
        }
        public Visibility ColonneDateVisibility
        {
            get { return (Core.Global.GetConfig().ListeArticleColonneDateVisible) ? Visibility.Visible : Visibility.Hidden; }
        }

        public DataGridHeadersVisibility HeaderVisibility
        {
            get
            {
                DataGridHeadersVisibility result = (!Core.Global.GetConfig().ListeArticleColonneIDVisible
                    && !Core.Global.GetConfig().ListeArticleColonneTypeVisible
                    && !Core.Global.GetConfig().ListeArticleColonneNomVisible
                    && !Core.Global.GetConfig().ListeArticleColonneReferenceVisible
                    && !Core.Global.GetConfig().ListeArticleColonneEANVisible
                    && !Core.Global.GetConfig().ListeArticleColonneActifVisible
                    && !Core.Global.GetConfig().ListeArticleColonneSyncVisible
                    && !Core.Global.GetConfig().ListeArticleColonneSyncPriceVisible
                    && !Core.Global.GetConfig().ListeArticleColonneDateVisible)
                ? DataGridHeadersVisibility.None
                : DataGridHeadersVisibility.Column;

                if (result == DataGridHeadersVisibility.None)
                {
                    result = DataGridHeadersVisibility.Column;
                    ColonneIDVisible = true;
                }
                return result;
            }
        }

        #endregion

        #endregion
        #region Constructors

        public ArticlesContext()
            : base()
        {
            #region Filtres

            ListProductFilterActiveDefault = new ObservableCollection<ProductFilterActiveDefault>();
            foreach (Core.Parametres.ProductFilterActiveDefault filter in Enum.GetValues(typeof(Core.Parametres.ProductFilterActiveDefault)))
                ListProductFilterActiveDefault.Add(new ProductFilterActiveDefault(filter));
            SelectedProductFilterActiveDefault = ListProductFilterActiveDefault.FirstOrDefault(r => r._ProductFilterActiveDefault == Core.Global.GetConfig().UIProductFilterActiveDefault);

            #endregion

            LoadCatalogsWorker = new BackgroundWorker();
            LoadCatalogsWorker.WorkerReportsProgress = true;

            LoadSelectedCatalogItemsWorker = new BackgroundWorker();
            LoadSelectedCatalogItemsWorker.WorkerReportsProgress = true;

            Catalogs = new ObservableCollection<Model.Local.Catalog>();

            SelectedCatalogItems = new ObservableCollection<Model.Local.Article>();
            //SelectedCatalogItems.GroupDescriptions.Add(new PropertyGroupDescription(""));
            Full = true;

            FtpActive = false;
            if (Core.Global.GetConfig().ConfigFTPActive && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPIP) && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPUser) && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPPassword))
            {
                FtpActive = true;
            }
        }

        #endregion
        #region Overrriden methods

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }

        #endregion
        #region Event methods

        private void LoadCatalogsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);
            //Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Catalogs.Clear(); }), null);

            IsBusy = true;
            LoadingStep = "Chargement des catalogues ...";

            Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
            Catalogs = new ObservableCollection<Model.Local.Catalog>(CatalogRepository.RootList());

            //foreach (var root in GetChildren(null, CatalogRepository))
            //    LoadCatalogsWorker.ReportProgress(0, root);
        }
        private void LoadCatalogsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Application.Current.Dispatcher.BeginInvoke(new Action(
            //    delegate { Catalogs.Add(e.UserState as ImportCatalog); }), null);
        }
        private void LoadCatalogsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingStep = string.Empty;
            IsBusy = false;

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
        }

        private void LoadSelectedCatalogItemsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);

            IsBusy = true;
            LoadingStep = "Chargement des articles du catalogue ...";

            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();

            List<Model.Local.Article> articles = (SelectedCatalog == null)
                ? ((SearchWithoutCatalogs)
                    ? ArticleRepository.ListWithoutCatalog()
                    : ArticleRepository.ListWithCatalog())
                : ArticleRepository.ListCatalog(SelectedCatalog.Cat_Id);

            string search = Global.RemoveDiacritics(SearchItem);

            if (SelectedCatalog == null && string.IsNullOrEmpty(search) && FiltreDateStart == null && FiltreDateEnd == null
                && ShowStandardProducts && ShowAttributeProducts && ShowConditioningProducts && ShowCompositionProducts
                && ShowPackProducts == true)
                this.Full = true;
            else
                this.Partial = true;

			if (SearchInComposition)
            {
                // désactivation requête en parallel si recherche dans les éléments composition
                articles = articles.Where(result =>
                    (string.IsNullOrEmpty(search)
                        || (result.Art_Id.ToString().Contains(search)
                            || Global.RemoveDiacritics(result.Art_Name.ToLower()).Contains(search.ToLower())
                            || result.Art_Ref.ToLower().Contains(search.ToLower())
                            || (!string.IsNullOrEmpty(result.Art_Ean13) && result.Art_Ean13.ToLower().Contains(search)))
                        || (SearchInComposition && result.CompositionArticle != null && result.CompositionArticle.Count > 0
                                && result.CompositionArticle.Count(ca => ca.F_ARTICLE_Composition.AR_Ref.ToLower().Contains(search.ToLower())) > 0))
                    && (SelectedProductFilterActiveDefault._ProductFilterActiveDefault == Core.Parametres.ProductFilterActiveDefault.AllProducts
                        || (SelectedProductFilterActiveDefault._ProductFilterActiveDefault == Core.Parametres.ProductFilterActiveDefault.ActiveProducts && result.Art_Active)
                        || (SelectedProductFilterActiveDefault._ProductFilterActiveDefault == Core.Parametres.ProductFilterActiveDefault.InactiveProducts && !result.Art_Active))
                    && ((filtreDateStart == null || result.Art_Date.Date >= filtreDateStart.Value.Date)
                        && (filtreDateEnd == null || result.Art_Date.Date <= FiltreDateEnd.Value.Date))
                    && ((ShowStandardProducts && result.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleSimple)
                        || (ShowAttributeProducts && (result.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMonoGamme || result.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMultiGammes))
                        || (ShowConditioningProducts && result.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleConditionnement)
                        || (ShowCompositionProducts && result.Art_Type == (short)Model.Local.Article.enum_TypeArticle.ArticleComposition))
                    && ((ShowPackProducts != null && ShowPackProducts == true)
                        || (ShowPackProducts == null && result.Art_Pack)
                        || (ShowPackProducts != null && ShowPackProducts == false && result.Art_Pack == false))
                    && ((ShowToSyncProducts != null && ShowToSyncProducts == true)
                        || (ShowToSyncProducts == null && result.Art_Sync)
                        || (ShowToSyncProducts != null && ShowToSyncProducts == false && result.Art_Sync == false))
                        ).ToList();
            }
            else
            {
                articles = articles.AsParallel().Where(result =>
                    (string.IsNullOrEmpty(search)
                        || (result.Art_Id.ToString().Contains(search)
                            || Global.RemoveDiacritics(result.Art_Name.ToLower()).Contains(search.ToLower())
                            || result.Art_Ref.ToLower().Contains(search.ToLower())
                            || (!string.IsNullOrEmpty(result.Art_Ean13) && result.Art_Ean13.ToLower().Contains(search))))
                    && (SelectedProductFilterActiveDefault._ProductFilterActiveDefault == Core.Parametres.ProductFilterActiveDefault.AllProducts
                        || (SelectedProductFilterActiveDefault._ProductFilterActiveDefault == Core.Parametres.ProductFilterActiveDefault.ActiveProducts && result.Art_Active)
                        || (SelectedProductFilterActiveDefault._ProductFilterActiveDefault == Core.Parametres.ProductFilterActiveDefault.InactiveProducts && !result.Art_Active))
                    && ((filtreDateStart == null || result.Art_Date.Date >= filtreDateStart.Value.Date)
                        && (filtreDateEnd == null || result.Art_Date.Date <= FiltreDateEnd.Value.Date))
                    && ((ShowStandardProducts && result.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleSimple)
                        || (ShowAttributeProducts && (result.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMonoGamme || result.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMultiGammes))
                        || (ShowConditioningProducts && result.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleConditionnement)
                        || (ShowCompositionProducts && result.Art_Type == (short)Model.Local.Article.enum_TypeArticle.ArticleComposition))
                    && ((ShowPackProducts != null && ShowPackProducts == true)
                        || (ShowPackProducts == null && result.Art_Pack)
                        || (ShowPackProducts != null && ShowPackProducts == false && result.Art_Pack == false))
                    && ((ShowToSyncProducts != null && ShowToSyncProducts == true)
                        || (ShowToSyncProducts == null && result.Art_Sync)
                        || (ShowToSyncProducts != null && ShowToSyncProducts == false && result.Art_Sync == false))
                        ).ToList();
            }

			// <JG> 11/07/2013 suppression foreach liste articles
			//Core.Temp.ListPrestashopProducts = new Model.Prestashop.PsProductRepository().List();
			//int selected_art = (selectedItem != null) ? selected_art = SelectedItem.Art_Id : 0;

			// <JG> ajout préchargement dateupd Prestashop
			Core.Temp.ListProductUpdate = new Model.Prestashop.PsProductRepository().ListUpdate(articles);
			SelectedCatalogItems = new ObservableCollection<Model.Local.Article>(articles);

            //System.Threading.Thread.Sleep(75);
            //if (selected_art != 0)
            //    SelectedItem = SelectedCatalogItems.FirstOrDefault(a => a.Art_Id == selected_art);
        }
        private void LoadSelectedCatalogItemsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Application.Current.Dispatcher.BeginInvoke(new Action(
            //    delegate { (SelectedCatalogItems.Add(e.UserState as Model.Local.Article); }), null);
        }
        private void LoadSelectedCatalogItemsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingStep = string.Empty;
            IsBusy = false;

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
        }

        #endregion
        #region Methods

        //private IEnumerable<ImportCatalog> GetChildren(Model.Local.Catalog parent, Model.Local.CatalogRepository local)
        //{
        //    List<Model.Local.Catalog> catalogs = (parent == null) ? local.RootList() : local.ListParent(parent.Cat_Id);

        //    foreach (var catalog in catalogs)
        //    {
        //        ImportCatalog hierarchicalCatalog = new ImportCatalog(catalog);

        //        foreach (var child in GetChildren(catalog, local))
        //            hierarchicalCatalog.Children.Add(child);

        //        yield return hierarchicalCatalog;
        //    }
        //}

        public void LoadCatalogs()
        {
            if (!LoadCatalogsWorker.IsBusy)
                LoadCatalogsWorker.RunWorkerAsync();
        }
        public void LoadSelectedCatalogItems()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                //((List<Model.Local.Article>)SelectedCatalogItems.SourceCollection).Clear();
                LoadSelectedCatalogItemsWorker.RunWorkerAsync();
            }
        }
        public void AllToActive()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                List<Model.Local.Article> items = SelectedCatalogItems.ToList();

                int activeCount = items.Count(result => result.Art_Active);
                bool toActive = ((items.Count - activeCount) >= activeCount);

                foreach (var item in items)
                {
                    Model.Local.Article itembdd = ArticleRepository.ReadArticle(item.Art_Id);
                    item.Art_Active = toActive;
                    item.Art_Date = DateTime.Now;
                    itembdd.Art_Active = item.Art_Active;
                    itembdd.Art_Date = item.Art_Date;
                    ArticleRepository.Save();
                }

                //SelectedCatalogItems.Refresh();
            }
        }
        public void AllToSync()
        {
            if (!loadSelectedCatalogItemsWorker.IsBusy)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                List<Model.Local.Article> items = SelectedCatalogItems.Where(i => i.CanSync).ToList();

                int syncCount = items.Count(result => result.Art_Sync);
                bool toSync = ((items.Count - syncCount) >= syncCount);

                foreach (var item in items)
                {
                    Model.Local.Article itembdd = ArticleRepository.ReadArticle(item.Art_Id);
                    item.Art_Sync = toSync;
                    //item.Art_Date = DateTime.Now;
                    itembdd.Art_Sync = item.Art_Sync;
                    //itembdd.Art_Date = item.Art_Date;
                    //if (toSync == false)
                    //{
                    //    item.Art_SyncPrice = toSync;
                    //    itembdd.Art_SyncPrice = item.Art_SyncPrice;
                    //}
                    ArticleRepository.Save();
                }

                //SelectedCatalogItems.Refresh();
            }
        }
        public void AllToSyncPrice()
        {
            if (!loadSelectedCatalogItemsWorker.IsBusy)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                List<Model.Local.Article> items = SelectedCatalogItems.Where(i => i.CanSync).ToList();

                int syncPriceCount = items.Count(result => result.Art_SyncPrice);
                bool toSyncPrice = ((items.Count - syncPriceCount) >= syncPriceCount);

                foreach (var item in items)
                {
                    Model.Local.Article itembdd = ArticleRepository.ReadArticle(item.Art_Id);
                    item.Art_SyncPrice = toSyncPrice;
                    //item.Art_Date = DateTime.Now;
                    itembdd.Art_SyncPrice = item.Art_SyncPrice;
                    //itembdd.Art_Date = item.Art_Date;
                    ArticleRepository.Save();
                }

                //SelectedCatalogItems.Refresh();
            }
        }
        public void SelectedItemToActive()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article itembdd = ArticleRepository.ReadArticle(SelectedItem.Art_Id);
                SelectedItem.Art_Date = DateTime.Now;
                itembdd.Art_Active = SelectedItem.Art_Active;
                itembdd.Art_Date = SelectedItem.Art_Date;
                ArticleRepository.Save();
            }
        }
        public void SelectedItemToSync()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article itembdd = ArticleRepository.ReadArticle(SelectedItem.Art_Id);
                //SelectedItem.Art_Date = DateTime.Now;
                itembdd.Art_Sync = SelectedItem.Art_Sync;
                //if (SelectedItem.Art_Sync == false)
                //{
                //    SelectedItem.Art_SyncPrice = SelectedItem.Art_Sync;
                //    itembdd.Art_SyncPrice = SelectedItem.Art_Sync;
                //}
                //itembdd.Art_Date = SelectedItem.Art_Date;
                ArticleRepository.Save();
            }
        }
        public void SelectedItemToSyncPrice()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article itembdd = ArticleRepository.ReadArticle(SelectedItem.Art_Id);
                //SelectedItem.Art_Date = DateTime.Now;
                itembdd.Art_SyncPrice = SelectedItem.Art_SyncPrice;
                //itembdd.Art_Date = SelectedItem.Art_Date;
                ArticleRepository.Save();
            }
        }

        public void ImportSageInformationLibreValeurs()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                if (MessageBox.Show("Souhaitez-vous importer les valeurs liées aux caractéristiques depuis Sage ?", "Import caractéristiques",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                    Loading.Show();

                    List<int> items = new List<int>();

                    if (Full)
                        items = new Model.Local.ArticleRepository().ListIdSync(true);
                    else if (Partial)
                        items = (from Table in SelectedCatalogItems
                                 where Table.Art_Sync
                                 select Table.Art_Id).ToList();

                    ImportSageStatInfoLibre ImportSageInformationLibre = new ImportSageStatInfoLibre(items);
                    Loading.Close();
                    ImportSageInformationLibre.ShowDialog();

                    LoadSelectedCatalogItems();
                }
            }
        }

        public void ImportConditioningArticle()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {

                if (MessageBox.Show("Êtes-vous sûr de vouloir importer les conditionnements depuis Sage ?", "Import conditionnements",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                    Loading.Show();

                    List<int> items = new List<int>();

                    if (Full)
                        items = new Model.Local.ArticleRepository().ListIdSync(true);
                    else if (Partial)
                        items = (from Table in SelectedCatalogItems
                                 where Table.Art_Sync
                                 select Table.Art_Id).ToList();

                    ImportSageArticleConditionnement ImportSageArticleConditionnement = new ImportSageArticleConditionnement(items);
                    Loading.Close();
                    ImportSageArticleConditionnement.ShowDialog();

                    LoadSelectedCatalogItems();
                }
            }
        }

        public void ReimportSage()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                if (MessageBox.Show("Êtes-vous sûr de vouloir réimporter vos données depuis Sage ?", "Réimport Sage",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    List<int> items = new List<int>();

                    if (Full)
                        items = new Model.Local.ArticleRepository().ListIdSync(true);
                    else if (Partial)
                        items = (from Table in SelectedCatalogItems
                                 where Table.Art_Sync
                                 select Table.Art_Id).ToList();

                    View.Module.ReimportSage ReimportSage = new View.Module.ReimportSage(items);
                    ReimportSage.ShowDialog();

                    LoadSelectedCatalogItems();
                }
            }
        }
        public void GestionStatutArticle()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                if (MessageBox.Show("Êtes-vous sûr de vouloir actualiser les statuts actif/inactif des produits" 
                    + (Core.Global.ExistAECAttributeStatut() ? "/déclinaisons" : string.Empty ) + " ?",
                    "Gestion statut actif/inactif directe",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    List<int> items = new List<int>();

                    if (Full)
                        items = new Model.Local.ArticleRepository().ListId();
                    else if (Partial)
                        items = (from Table in SelectedCatalogItems
                                 where Table.Art_Sync
                                 select Table.Art_Id).ToList();

                    GestionStatutArticle GestionStatutArticle = new GestionStatutArticle(items);
                    GestionStatutArticle.ShowDialog();

                    LoadSelectedCatalogItems();
                }
            }
        }

        public void SyncArticle()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                SynchronisationFournisseur SyncFournisseur = new SynchronisationFournisseur();
                SyncFournisseur.ShowDialog();

                List<int> items = new List<int>();

                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSyncOrderByPack(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync
                             select Table.Art_Id).ToList();

                // selon type synchro récupération de la totalité des catalogues ou seulement de ceux liés aux articles et de leurs parents
                // liste complète des catalogues récupérée dans le backgroundworker de SynchronisationCatalogue
                List<Int32> catalogues = (Full) ? null
                    : new Model.Local.ArticleCatalogRepository().ListCataloguesArticles(items);

                SynchronisationCatalogue SyncCatalogue = new SynchronisationCatalogue(catalogues);
                SyncCatalogue.ShowDialog();

                //SynchronisationGamme SyncGamme = new SynchronisationGamme();
                //SyncGamme.ShowDialog();

                //SynchronisationGammeEnumere SyncGammeEnumere = new SynchronisationGammeEnumere(items);
                //SyncGammeEnumere.ShowDialog();

                //SynchronisationConditionnement SyncConditionnement = new SynchronisationConditionnement();
                //SyncConditionnement.ShowDialog();

                //SynchronisationConditionnementEnumere SyncConditionnementEnumere = new SynchronisationConditionnementEnumere(items);
                //SyncConditionnementEnumere.ShowDialog();

                SynchronisationArticle Sync = new SynchronisationArticle(items);
                Loading.Close();
                Sync.ShowDialog();

                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                if (Core.Global.GetConfig().ConfigFTPActive
                    && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPIP)
                    && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPUser)
                    && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPPassword))
                {
                    TransfertArticleImage SyncArticleImage = new TransfertArticleImage(items);
                    SyncArticleImage.ShowDialog();
                    TransfertArticleDocument SyncArticleDocument = new TransfertArticleDocument(items);
                    SyncArticleDocument.ShowDialog();
                }

                LoadSelectedCatalogItems();
            }
        }
        public void TransfertStock()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                List<int> items = new List<int>();
                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSync(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync
                             select Table.Art_Id).ToList();

                TransfertStock SyncStock = new TransfertStock(items);
                Loading.Close();
                SyncStock.ShowDialog();
            }
        }
        public void ImportCatalogueInfoLibre()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();
                List<int> items = new List<int>();
                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSync(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync
                             select Table.Art_Id).ToList();

                SynchronisationCatalogueInfoLibre SynchronisationCatalogueInfoLibre = new SynchronisationCatalogueInfoLibre(items);
                Loading.Close();
                SynchronisationCatalogueInfoLibre.ShowDialog();
            }
        }
        public void TransfertStockPrice()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                List<int> items = new List<int>();
                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSync(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync
                             select Table.Art_Id).ToList();

                TransfertStockPrice SyncStockPrice = new TransfertStockPrice(items);
                Loading.Close();
                SyncStockPrice.ShowDialog();
            }
        }

        public void TransfertImages()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                List<int> items = new List<int>();
                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSync(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync
                             select Table.Art_Id).ToList();

                TransfertArticleImage SyncArticleImage = new TransfertArticleImage(items);
                Loading.Close();
                SyncArticleImage.ShowDialog();
                Loading = new PRESTACONNECT.Loading();
                Loading.Show();
                TransfertArticleDocument SyncArticleDocument = new TransfertArticleDocument(items);
                Loading.Close();
                SyncArticleDocument.ShowDialog();
            }
        }

        public void SyncArticleCatalogue()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                List<int> items = new List<int>();
                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSync(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync
                             select Table.Art_Id).ToList();

                SynchronisationArticleCatalogue SyncArticleCatalogue = new SynchronisationArticleCatalogue(items);
                Loading.Close();
                SyncArticleCatalogue.ShowDialog();
            }
        }
        public void TransfertAttribute()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                List<int> items = new List<int>();
                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSync(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync
                             select Table.Art_Id).ToList();

                TransfertAttribute SyncAttribute = new TransfertAttribute(items);
                Loading.Close();
                SyncAttribute.ShowDialog();
            }
        }
        public void TransfertPack()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                List<int> items = new List<int>();
                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSyncPack(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync && Table.Art_Pack
                             select Table.Art_Id).ToList();

                TransfertPack SyncPack = new TransfertPack(items);
                Loading.Close();
                SyncPack.ShowDialog();
            }
        }
        public void TransfertFeature()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                List<int> items = new List<int>();
                if (Full)
                    items = new Model.Local.ArticleRepository().ListIdSync(true);
                else if (Partial)
                    items = (from Table in SelectedCatalogItems
                             where Table.Art_Sync
                             select Table.Art_Id).ToList();

                TransfertFeature SyncPack = new TransfertFeature(items);
                Loading.Close();
                SyncPack.ShowDialog();
            }
        }

        public void CreateArticleComposition()
        {
            if (Core.UpdateVersion.License.Option2)
            {
                MessageBoxResult result = MessageBox.Show("Souhaitez-vous créer une composition en vous basant sur un article Sage ?", "Création composition", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                    Loading.Show();
                    Core.Temp.selectedcatalog_composition = (SelectedCatalog != null) ? SelectedCatalog.Cat_Id : 0;
                    PRESTACONNECT.View.Composition Composition = new View.Composition();
                    Loading.Close();
                    Composition.ShowDialog();
                }
                else if (result == MessageBoxResult.No)
                {
                    PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                    Loading.Show();
                    PRESTACONNECT.Article Article = new Article(new Model.Local.Article()
                    {
                        Art_Name = "Article de composition",
                        Art_Date = DateTime.Now,
                        Art_Type = (short)Model.Local.Article.enum_TypeArticle.ArticleComposition,
                        Art_Ref = string.Empty,
                        Art_RedirectType = new Model.Internal.RedirectType(Core.Parametres.RedirectType.NoRedirect404).Page,
                        Art_Solde = false,
                        Art_Description = "Article de composition",
                        Art_Description_Short = "Article de composition",
                        Art_Ean13 = string.Empty,
                        Art_LinkRewrite = string.Empty,
                        Art_MetaDescription = string.Empty,
                        Art_MetaKeyword = string.Empty,
                        Art_MetaTitle = string.Empty,
                        Cat_Id = (SelectedCatalog != null) ? SelectedCatalog.Cat_Id : 0,
                    });
                    Loading.Close();
                    Article.ShowDialog();
                }
                if (result != MessageBoxResult.Cancel)
                    LoadSelectedCatalogItems();
            }
            else
            {
                new PRESTACONNECT.View.PrestaMessage("Votre licence actuelle ne permet pas d'utiliser la composition d'article !", "Licence insuffisante", MessageBoxButton.OK, MessageBoxImage.Information).ShowDialog();
            }
        }

        #endregion
    }
}
