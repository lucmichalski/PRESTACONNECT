using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PRESTACONNECT.Model.Internal;
using System.Threading.Tasks;

namespace PRESTACONNECT.Contexts
{
    internal sealed class ImportContext : Context
    {
        #region Properties

        #region Catalogues et articles

        private bool ChangeParentToImportIsBusy { get; set; }

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

        private ObservableCollection<Model.Sage.F_CATALOGUE> catalogs;
        public ObservableCollection<Model.Sage.F_CATALOGUE> Catalogs
        {
            get { return catalogs; }
            set
            {
                catalogs = value;
                OnPropertyChanged("Catalogs");
            }
        }

        private Model.Sage.F_CATALOGUE selectedCatalog;
        public Model.Sage.F_CATALOGUE SelectedCatalog
        {
            get { return selectedCatalog; }
            set
            {
                selectedCatalog = value;

                OnPropertyChanged("SelectedCatalog");
            }
        }

        private Boolean searchWithoutCatalogs;
        public Boolean SearchWithoutCatalogs
        {
            get { return searchWithoutCatalogs; }
            set
            {
                searchWithoutCatalogs = value;
                if (value)
                    SelectionLocalCatalog = true;
                OnPropertyChanged("SearchWithoutCatalogs");
                OnPropertyChanged("WithCatalogSelected");
            }
        }

        public Boolean WithCatalogSelected
        {
            get
            {
                return !SearchWithoutCatalogs;
            }
        }

        private ObservableCollection<Model.Sage.F_ARTICLE_Import> selectedCatalogItems;
        public ObservableCollection<Model.Sage.F_ARTICLE_Import> SelectedCatalogItems
        {
            get { return selectedCatalogItems; }
            set
            {
                selectedCatalogItems = value;
                OnPropertyChanged("SelectedCatalogItems");
            }
        }

        private ObservableCollection<Model.Local.Catalog> localCatalogs;
        public ObservableCollection<Model.Local.Catalog> LocalCatalogs
        {
            get { return localCatalogs; }
            set
            {
                localCatalogs = value;
                OnPropertyChanged("LocalCatalogs");
            }
        }

        private Model.Local.Catalog selectedLocalCatalog;
        public Model.Local.Catalog SelectedLocalCatalog
        {
            get { return selectedLocalCatalog; }
            set
            {
                selectedLocalCatalog = value;
                OnPropertyChanged("SelectedLocalCatalog");
            }
        }

        private Boolean selectionLocalCatalog;
        public Boolean SelectionLocalCatalog
        {
            get { return this.selectionLocalCatalog; }
            set
            {
                this.selectionLocalCatalog = value;
                this.OnPropertyChanged("SelectionLocalCatalog");
            }
        }

        // <JG> 12/07/2013 ajout option affichage articles déjà importés
        private Boolean maskImportedProducts = true;
        public Boolean MaskImportedProducts
        {
            get { return this.maskImportedProducts; }
            set
            {
                if (!IsBusy)
                {
                    this.maskImportedProducts = value;
                    this.OnPropertyChanged("MaskImportedProducts");
                    LoadSelectedCatalogItems();
                }
            }
        }
        private String filter;
        public String Filter
        {
            get { return this.filter; }
            set
            {
                if (!IsBusy)
                {
                    this.filter = value;
                    this.OnPropertyChanged("Filter");
                    //LoadSelectedCatalogItems();
                }
            }
        }

        public Boolean AutoSelectCatalogParent
        {
            get { return Core.Global.GetConfig().ImportCatalogueAutoSelectionParents; }
            set
            {
                if (!IsBusy)
                {
                    Core.Global.GetConfig().UpdateImportCatalogueAutoSelectionParents(value);
                    this.OnPropertyChanged("AutoSelectCatalogParent");
                }
            }
        }
        public Boolean AutoSelectCatalogEnfant
        {
            get { return Core.Global.GetConfig().ImportCatalogueAutoSelectionEnfants; }
            set
            {
                if (!IsBusy)
                {
                    Core.Global.GetConfig().UpdateImportCatalogueAutoSelectionEnfants(value);
                    this.OnPropertyChanged("AutoSelectCatalogEnfant");
                }
            }
        }

        public string strNbArticle
        {
            get
            {
                return "Article(s)";
            }
        }
        public string strNbArticleSelec
        {
            get
            {
                return "Sélectioné(s)";
            }
        }
        public string strCurrentCatalog
        {
            get
            {
                return "Catalogue courant : ";
            }
        }

        public Boolean ImportArticleStatut
        {
            get { return Core.Global.GetConfig().ImportArticleStatutActif; }
            set
            {
                Core.Global.GetConfig().UpdateImportArticleStatutActif(value);
                OnPropertyChanged("ImportArticleStatut");
            }
        }
        public Boolean ImportArticleRattachement
        {
            get { return Core.Global.GetConfig().ImportArticleRattachementParents; }
            set
            {
                Core.Global.GetConfig().UpdateImportArticleRattachementParents(value);
                OnPropertyChanged("ImportArticleRattachement");
            }
        }
        // <JG> 22/12/2015 ajout filtre articles standards
        public Boolean OnlyStandardProducts
        {
            get { return Core.Global.GetConfig().ImportArticleOnlyStandardProducts; }
            set
            {
                if (!IsBusy)
                {
                    Core.Global.GetConfig().UpdateImportArticleOnlyStandardProducts(value);
                    this.OnPropertyChanged("OnlyStandardProducts");
                    LoadSelectedCatalogItems();
                }
            }
        }

        private string importSageFilterValue = string.Empty;
        public string ImportSageFilterValue
        {
            get { return importSageFilterValue; }
            set { importSageFilterValue = value; OnPropertyChanged("ImportSageFilterValue"); }
        }
        private ObservableCollection<ImportSageFilterTypeSearchValue> listTypeSearchValue;
        public ObservableCollection<ImportSageFilterTypeSearchValue> ListTypeSearchValue
        {
            get
            {
                return (SelectedTargetData != null && SelectedTargetData._ImportSageFilterTargetData == Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle)
                  ? new ObservableCollection<ImportSageFilterTypeSearchValue>(listTypeSearchValue.Where(t => t.EnabledInfolibre))
                  : (SelectedTargetData != null && SelectedTargetData._ImportSageFilterTargetData != Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle)
                    ? new ObservableCollection<ImportSageFilterTypeSearchValue>(listTypeSearchValue.Where(t => !t.EnabledInfolibre))
                    : listTypeSearchValue;
            }
            set { listTypeSearchValue = value; OnPropertyChanged("ListTypeSearchValue"); }
        }
        private ImportSageFilterTypeSearchValue selectedTypeSearchValue;
        public ImportSageFilterTypeSearchValue SelectedTypeSearchValue
        {
            get { return selectedTypeSearchValue; }
            set { selectedTypeSearchValue = value; OnPropertyChanged("SelectedTypeSearchValue"); }
        }
        private ObservableCollection<ImportSageFilterTargetData> listTargetData;
        public ObservableCollection<ImportSageFilterTargetData> ListTargetData
        {
            get { return listTargetData; }
            set { listTargetData = value; OnPropertyChanged("ListTargetData"); }
        }
        private ImportSageFilterTargetData selectedTargetData;
        public ImportSageFilterTargetData SelectedTargetData
        {
            get { return selectedTargetData; }
            set { selectedTargetData = value; OnPropertyChanged("SelectedTargetData"); OnPropertyChanged("ListTypeSearchValue"); }
        }

        private ObservableCollection<Model.Sage.cbSysLibre> listSageInfolibreArticleTextTable;
        public ObservableCollection<Model.Sage.cbSysLibre> ListSageInfolibreArticleTextTable
        {
            get { return listSageInfolibreArticleTextTable; }
            set
            {
                listSageInfolibreArticleTextTable = value;
                OnPropertyChanged("ListSageInfolibreArticleTextTable");
            }
        }
        private Model.Sage.cbSysLibre selectedSageInfolibreArticleTextTable;
        public Model.Sage.cbSysLibre SelectedSageInfolibreArticleTextTable
        {
            get { return selectedSageInfolibreArticleTextTable; }
            set
            {
                selectedSageInfolibreArticleTextTable = value;
                OnPropertyChanged("SelectedSageInfolibreArticleTextTable");
            }
        }

        private ObservableCollection<Model.Local.ImportSageFilter> listImportSageFilter;
        public ObservableCollection<Model.Local.ImportSageFilter> ListImportSageFilter
        {
            get { return listImportSageFilter; }
            set { listImportSageFilter = value; OnPropertyChanged("ListImportSageFilter"); }
        }
        private Model.Local.ImportSageFilter selectedImportSageFilter;
        public Model.Local.ImportSageFilter SelectedImportSageFilter
        {
            get { return selectedImportSageFilter; }
            set { selectedImportSageFilter = value; OnPropertyChanged("SelectedImportSageFilter"); }
        }

        private string strFilteredProducts = string.Empty;
        public string StrFilteredProducts
        {
            get { return strFilteredProducts; }
            set { strFilteredProducts = value; OnPropertyChanged("StrFilteredProducts"); }
        }

        #endregion

        #region Images/documents/médias

        private string automaticImportFolderPicture;
        public string AutomaticImportFolderPicture
        {
            get { return automaticImportFolderPicture; }
            set
            {
                automaticImportFolderPicture = value;
                OnPropertyChanged("AutomaticImportFolderPicture");
            }
        }
        private string automaticImportFolderDocument;
        public string AutomaticImportFolderDocument
        {
            get { return automaticImportFolderDocument; }
            set
            {
                automaticImportFolderDocument = value;
                OnPropertyChanged("AutomaticImportFolderDocument");
            }
        }
        private string automaticImportFolderMedia;
        public string AutomaticImportFolderMedia
        {
            get { return automaticImportFolderMedia; }
            set { automaticImportFolderMedia = value; OnPropertyChanged("AutomaticImportFolderMedia"); }
        }

        public Boolean ImportMediaIncludePictures
        {
            get { return Core.Global.GetConfig().ImportMediaIncludePictures; }
            set { Core.Global.GetConfig().UpdateConfigImportMediaIncludePictures(value); OnPropertyChanged("ImportMediaIncludePictures"); }
        }
        public Boolean ImportMediaAutoDeleteAttachment
        {
            get { return Core.Global.GetConfig().ImportMediaAutoDeleteAttachment; }
            set { Core.Global.GetConfig().UpdateConfigImportMediaAutoDeleteAttachment(value); OnPropertyChanged("ImportMediaAutoDeleteAttachment"); }
        }

        public Boolean ImportImageReplaceFiles
        {
            get { return Core.Global.GetConfig().ImportImageReplaceFiles; }
            set { Core.Global.GetConfig().UpdateConfigImportImageReplaceFiles(value); OnPropertyChanged("ImportImageReplaceFiles"); }
        }
        public Boolean ImportImageRemoveDeletedFiles
        {
            get { return Core.Global.GetConfig().ImportImageRemoveDeletedFiles; }
            set
            {
                if (!value ||
                    (value && new PRESTACONNECT.View.PrestaMessage("L'activation de cette option entraînera la suppression de toutes les images dans PrestaConnect et PrestaShop"
                         + " pour lesquelles le fichier source ne sera pas présent dans le dossier d'import paramétré !"
                         + "\n\n"
                         + "Dossier actuel : " + Core.Global.GetConfig().AutomaticImportFolderPicture
                         + "\n\n"
                         + "Valider l'activation de cette option ?",
                    "Activation suppression automatique des images", MessageBoxButton.YesNo, MessageBoxImage.Warning, 600).ShowDialog() == true))
                {
                    Core.Global.GetConfig().UpdateConfigImportImageRemoveDeletedFiles(value);
                }
                OnPropertyChanged("ImportImageRemoveDeletedFiles");
            }
        }

        public Boolean ImportImageIncludeReferenceClient
        {
            get { return Core.Global.GetConfig().ImportImageSearchReferenceClient; }
            set { Core.Global.GetConfig().UpdateConfigImportImageSearchReferenceClient(value); OnPropertyChanged("ImportImageIncludeReferenceClient"); }
        }

        public Boolean ImportImageUseSageDatas
        {
            get { return Core.Global.GetConfig().ImportImageUseSageDatas; }
            set { Core.Global.GetConfig().UpdateConfigImportImageUseSageDatas(value); OnPropertyChanged("ImportImageUseSageDatas"); } 
        }


        #region MediaAssignmentRule

        private string mediaRuleSuffixText = string.Empty;
        public string MediaRuleSuffixText
        {
            get { return mediaRuleSuffixText; }
            set { mediaRuleSuffixText = value; OnPropertyChanged("MediaRuleSuffixText"); }
        }
        private string mediaRuleAssignName = string.Empty;
        public string MediaRuleAssignName
        {
            get { return mediaRuleAssignName; }
            set { mediaRuleAssignName = value; OnPropertyChanged("MediaRuleAssignName"); }
        }

        private ObservableCollection<MediaRule> listMediaRule;
        public ObservableCollection<MediaRule> ListMediaRule
        {
            get { return listMediaRule; }
            set { listMediaRule = value; OnPropertyChanged("ListMediaRule"); }
        }
        private MediaRule selectedMediaRule;
        public MediaRule SelectedMediaRule
        {
            get { return selectedMediaRule; }
            set { selectedMediaRule = value; OnPropertyChanged("SelectedMediaRule"); }
        }

        private ObservableCollection<Model.Local.MediaAssignmentRule> listMediaAssignmentRule;
        public ObservableCollection<Model.Local.MediaAssignmentRule> ListMediaAssignmentRule
        {
            get { return listMediaAssignmentRule; }
            set { listMediaAssignmentRule = value; OnPropertyChanged("ListMediaAssignmentRule"); }
        }
        private Model.Local.MediaAssignmentRule selectedMediaAssignmentRule;
        public Model.Local.MediaAssignmentRule SelectedMediaAssignmentRule
        {
            get { return selectedMediaAssignmentRule; }
            set { selectedMediaAssignmentRule = value; OnPropertyChanged("SelectedMediaAssignmentRule"); }
        }

        #endregion

        #endregion

        #region PrestaShop

        private Boolean _ForceImportProduct = false;
        public Boolean ForceImportProduct
        {
            get { return _ForceImportProduct; }
            set { _ForceImportProduct = value; OnPropertyChanged("ForceImportProduct"); }
        }

        private Boolean _ForceImportImage = false;
        public Boolean ForceImportImage
        {
            get { return _ForceImportImage; }
            set { _ForceImportImage = value; OnPropertyChanged("ForceImportImage"); }
        }

        #endregion

        #endregion

        #region Constructors

        public ImportContext()
            : base()
        {
            LoadCatalogsWorker = new BackgroundWorker();
            LoadCatalogsWorker.WorkerReportsProgress = true;

            LoadSelectedCatalogItemsWorker = new BackgroundWorker();
            LoadSelectedCatalogItemsWorker.WorkerReportsProgress = true;

            Catalogs = new ObservableCollection<Model.Sage.F_CATALOGUE>();
            LocalCatalogs = new ObservableCollection<Model.Local.Catalog>();

            SelectedCatalogItems = new ObservableCollection<Model.Sage.F_ARTICLE_Import>();
            //SelectedCatalogItems.GroupDescriptions.Add(new PropertyGroupDescription(""));

            AutomaticImportFolderPicture = Core.Global.GetConfig().AutomaticImportFolderPicture;
            AutomaticImportFolderDocument = Core.Global.GetConfig().AutomaticImportFolderDocument;
            AutomaticImportFolderMedia = Core.Global.GetConfig().AutomaticImportFolderMedia;

            ListMediaRule = new ObservableCollection<MediaRule>();
            foreach (Core.Parametres.MediaRule value in Enum.GetValues(typeof(Core.Parametres.MediaRule)))
                ListMediaRule.Add(new MediaRule(value));
            ListMediaAssignmentRule = new ObservableCollection<Model.Local.MediaAssignmentRule>(new Model.Local.MediaAssignmentRuleRepository().List());


            ListTypeSearchValue = new ObservableCollection<ImportSageFilterTypeSearchValue>();
            foreach (Core.Parametres.ImportSageFilterTypeSearchValue value in Enum.GetValues(typeof(Core.Parametres.ImportSageFilterTypeSearchValue)))
                ListTypeSearchValue.Add(new ImportSageFilterTypeSearchValue(value));

            ListTargetData = new ObservableCollection<ImportSageFilterTargetData>();
            foreach (Core.Parametres.ImportSageFilterTargetData value in Enum.GetValues(typeof(Core.Parametres.ImportSageFilterTargetData)))
                ListTargetData.Add(new ImportSageFilterTargetData(value));
            SelectedTargetData = ListTargetData.FirstOrDefault();

            ListSageInfolibreArticleTextTable = new ObservableCollection<Model.Sage.cbSysLibre>(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable).ToList());

            ListImportSageFilter = new ObservableCollection<Model.Local.ImportSageFilter>(new Model.Local.ImportSageFilterRepository().List());

        }

        #endregion

        #region Overriden methods

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }

        #endregion

        #region Event methods

        private void LoadCatalogsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);

            IsBusy = true;
            LoadingStep = "Chargement des catalogues ...";

            LocalCatalogs = new ObservableCollection<Model.Local.Catalog>(new Model.Local.CatalogRepository().List());
            Core.Temp.ListCatalogLocal = LocalCatalogs.Select(c => (int)c.Sag_Id).ToList();

            Catalogs = new ObservableCollection<Model.Sage.F_CATALOGUE>(new Model.Sage.F_CATALOGUERepository().RootList());
        }
        private void LoadCatalogsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
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

            bool sommeil = Core.Global.GetConfig().ArticleEnSommeil;
            bool nonpublie = Core.Global.GetConfig().ArticleNonPublieSurLeWeb;

            Model.Sage.F_ARTICLERepository F_ArticleRepository = new Model.Sage.F_ARTICLERepository();
            Core.Temp.ListArticleLocal = new Model.Local.ArticleRepository().ListSageId();
            Core.Temp.ListArticleLocal.AddRange(new Model.Local.CompositionArticleRepository().ListF_ARTICLESageId());

            List<Model.Sage.F_ARTICLE_Import> articles = (SelectedCatalog == null)
                ? ((SearchWithoutCatalogs)
                    ? F_ArticleRepository.ListWithoutCatalog()
                    : F_ArticleRepository.ListInCatalog())
                : F_ArticleRepository.ListFullCatalogue(SelectedCatalog.CL_No.Value);

            articles = articles.Where(a => (sommeil || a.AR_Sommeil == 0)
                                        && (nonpublie || a.AR_Publie == 1)
                                        && (!MaskImportedProducts || a.Exist == false)
                                        && (!OnlyStandardProducts || ((a.AR_Gamme1 == null || a.AR_Gamme1 == 0) && (a.AR_Gamme2 == null || a.AR_Gamme2 == 0) && (a.AR_Condition == null || a.AR_Condition == 0)))
                // <JG> 12/07/2013 ajout filtres
                                        && (string.IsNullOrWhiteSpace(Filter)
                                            || (Core.Global.RemoveDiacritics(a.AR_Design.ToLower()).Contains(Filter.ToLower())
                                            || a.AR_Ref.ToLower().Contains(Filter.ToLower())
                                            || a.AF_Ref.ToLower().Contains(Filter.ToLower())
                                            || (!string.IsNullOrWhiteSpace(a.AR_CodeBarre) && a.AR_CodeBarre.ToLower().Contains(Filter))))).ToList();

            // <JG> 02/11/2015 ajout filtres d'import par exclusion
            int filtered_products = articles.Count;
            articles = Core.Tools.FiltreImportSage.ImportSageFilter(articles);
            StrFilteredProducts = (articles.Count < filtered_products) ? ("" + (filtered_products - articles.Count) + " article(s) dans les filtres d'exclusion") : string.Empty;

            SelectedCatalogItems = new ObservableCollection<Model.Sage.F_ARTICLE_Import>(articles);
        }
        private void LoadSelectedCatalogItemsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void LoadSelectedCatalogItemsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingStep = string.Empty;
            IsBusy = false;

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
        }

        #endregion

        #region Methods

        private IEnumerable<Model.Sage.F_CATALOGUE> GetToImportCatalogs(Model.Sage.F_CATALOGUE catalog)
        {
            foreach (var child in catalog.SortChildren)
            {
                if (child.ToImport)
                    yield return child;

                foreach (var current in GetToImportCatalogs(child))
                    yield return current;
            }
        }
        private IEnumerable<Model.Sage.F_CATALOGUE> GetCatalogs(Model.Sage.F_CATALOGUE catalog)
        {
            if (catalog == null)
            {
                foreach (var root in Catalogs)
                {
                    yield return root;

                    foreach (var child in GetCatalogs(root))
                        yield return child;
                }
            }
            else
            {
                foreach (var root in catalog.SortChildren)
                {
                    yield return root;

                    foreach (var child in GetCatalogs(root))
                        yield return child;
                }
            }
        }

        public void LoadCatalogs()
        {
            if (!LoadCatalogsWorker.IsBusy)
                LoadCatalogsWorker.RunWorkerAsync();
        }
        public void LoadSelectedCatalogItems()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                //((List<Model.Sage.F_ARTICLE>)SelectedCatalogItems.SourceCollection).Clear();
                LoadSelectedCatalogItemsWorker.RunWorkerAsync();

                if (SelectedCatalog != null)
                {
                    SelectedLocalCatalog = LocalCatalogs.FirstOrDefault(
                        result => result.Sag_Id == SelectedCatalog.cbMarq);
                }

                if (SelectedCatalog == null || SelectedLocalCatalog == null)
                {
                    SelectedLocalCatalog = LocalCatalogs.FirstOrDefault(
                    result => result.Cat_Id == 0);
                }
            }
        }
        public void CheckAllArticlesToImport()
        {
            if (!LoadSelectedCatalogItemsWorker.IsBusy)
            {
                int importCount = SelectedCatalogItems.Count(result => result.AR_IsCheckedImport && !result.Exist);
                bool toImport = ((SelectedCatalogItems.Count - importCount) >= importCount);

                Parallel.ForEach(SelectedCatalogItems.Where(a => !a.Exist), a => a.AR_IsCheckedImport = toImport);
            }
        }

        public void SavePictureFolder()
        {
            Core.Global.GetConfig().UpdateConfigAutomaticImportFolderPicture(this.AutomaticImportFolderPicture);
        }
        public void SaveDocumentFolder()
        {
            Core.Global.GetConfig().UpdateConfigAutomaticImportFolderDocument(this.AutomaticImportFolderDocument);
        }
        public void SaveMediaFolder()
        {
            Core.Global.GetConfig().UpdateConfigAutomaticImportFolderMedia(this.AutomaticImportFolderMedia);
        }

        public void AddMediaAssignmentRule()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(MediaRuleSuffixText)
                    && SelectedMediaRule != null)
                {
                    Model.Local.MediaAssignmentRuleRepository MediaAssignmentRuleRepository = new Model.Local.MediaAssignmentRuleRepository();
                    if (!MediaAssignmentRuleRepository.ExistSuffix(MediaRuleSuffixText))
                    {
                        Model.Local.MediaAssignmentRule newMediaAssignmentRule = new Model.Local.MediaAssignmentRule()
                        {
                            SuffixText = MediaRuleSuffixText,
                            AssignName = MediaRuleAssignName,
                            Rule = (short)SelectedMediaRule.Marq,
                        };
                        MediaAssignmentRuleRepository.Add(newMediaAssignmentRule);
                        ListMediaAssignmentRule.Add(newMediaAssignmentRule);
                    }
                    else
                    {
                        MessageBox.Show("Ce suffixe est déjà associée à une règle, veuillez supprimer la règle actuelle d'abord !", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez renseigner le suffixe !", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
        }
        public void DeleteMediaAssignmentRule()
        {
            try
            {
                Model.Local.MediaAssignmentRuleRepository MediaAssignmentRuleRepository = new Model.Local.MediaAssignmentRuleRepository();
                if (SelectedMediaAssignmentRule != null && MediaAssignmentRuleRepository.ExistSuffix(SelectedMediaAssignmentRule.SuffixText))
                {
                    MediaAssignmentRuleRepository.Delete(MediaAssignmentRuleRepository.ReadSuffix(SelectedMediaAssignmentRule.SuffixText));
                    ListMediaAssignmentRule.Remove(SelectedMediaAssignmentRule);
                }
                MediaAssignmentRuleRepository = null;
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
        }

        public void AddImportSageFilter()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ImportSageFilterValue))
                    MessageBox.Show("Vous devez renseigner la valeur filtrante !", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
                else if (SelectedTypeSearchValue == null)
                    MessageBox.Show("Vous devez choisir la condition de filtrage !", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
                else if (SelectedTargetData == null)
                    MessageBox.Show("Vous devez choisir les données concernées !", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
                else if (SelectedTargetData._ImportSageFilterTargetData == Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle
                    && SelectedSageInfolibreArticleTextTable == null)
                    MessageBox.Show("Vous devez choisir l'information libre cible !", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    Model.Local.ImportSageFilterRepository ImportSageFilterRepository = new Model.Local.ImportSageFilterRepository();
                    if (!ImportSageFilterRepository.ExistValue(ImportSageFilterValue))
                    {
                        Model.Local.ImportSageFilter newImportSageFilter = new Model.Local.ImportSageFilter()
                        {
                            Imp_Value = (SelectedTypeSearchValue.ValueUppercaseOnly) ? ImportSageFilterValue.ToUpper() : ImportSageFilterValue,
                            Imp_TypeSearchValue = SelectedTypeSearchValue.Marq,
                            Imp_TargetData = SelectedTargetData.Marq,
                            Imp_Active = true,
                        };
                        if (SelectedTargetData._ImportSageFilterTargetData == Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle
                                && SelectedSageInfolibreArticleTextTable != null)
                            newImportSageFilter.Sag_Infolibre = SelectedSageInfolibreArticleTextTable.CB_Name;
                        ImportSageFilterRepository.Add(newImportSageFilter);
                        ListImportSageFilter.Add(newImportSageFilter);
                    }
                    else if (MessageBox.Show("Cette valeur filtrante est déjà associée à un filtre !\n"
                        + "Valider le remplacement de la condition et des cibles ?", "Prestaconnect", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        Model.Local.ImportSageFilter ImportSageFilter = ImportSageFilterRepository.ReadValue(ImportSageFilterValue);
                        ImportSageFilter.Imp_Value = (SelectedTypeSearchValue.ValueUppercaseOnly) ? ImportSageFilterValue.ToUpper() : ImportSageFilterValue;
                        ImportSageFilter.Imp_TypeSearchValue = SelectedTypeSearchValue.Marq;
                        ImportSageFilter.Imp_TargetData = SelectedTargetData.Marq;
                        if (SelectedTargetData._ImportSageFilterTargetData == Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle
                                && SelectedSageInfolibreArticleTextTable != null)
                            ImportSageFilter.Sag_Infolibre = SelectedSageInfolibreArticleTextTable.CB_Name;
                        ImportSageFilterRepository.Save();
                        ImportSageFilter = ListImportSageFilter.FirstOrDefault(imp => imp.Imp_Value.ToLower() == ImportSageFilterValue.ToLower());
                        if (ImportSageFilter != null)
                        {
                            ImportSageFilter.Imp_TypeSearchValue = SelectedTypeSearchValue.Marq;
                            ImportSageFilter.Imp_TargetData = SelectedTargetData.Marq;
                        }
                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
        }
        public void DeleteImportSageFilter()
        {
            try
            {
                Model.Local.ImportSageFilterRepository ImportSageFilterRepository = new Model.Local.ImportSageFilterRepository();
                if (SelectedImportSageFilter != null && ImportSageFilterRepository.ExistValue(SelectedImportSageFilter.Imp_Value))
                {
                    ImportSageFilterRepository.Delete(ImportSageFilterRepository.ReadValue(SelectedImportSageFilter.Imp_Value));
                    ListImportSageFilter.Remove(SelectedImportSageFilter);
                }
                ImportSageFilterRepository = null;
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
        }

        public IEnumerable<Model.Sage.F_CATALOGUE> GetToImportCatalogs()
        {
            foreach (var catalog in Catalogs)
            {
                if (catalog.ToImport)
                    yield return catalog;

                foreach (var child in GetToImportCatalogs(catalog))
                    yield return child;
            }
        }
        public List<int> GetToImportSelectedCatalogItems()
        {
            return SelectedCatalogItems.Where(a => a.AR_IsCheckedImport).Select(a => a.cbMarq).ToList();
        }
        public void ChangeToImportCatalogs(Model.Sage.F_CATALOGUE catalog, bool toImport)
        {
            if (AutoSelectCatalogParent)
            {
                // <JG> 12/07/2013 gestion (dé)sélection auto parents
                Model.Sage.F_CATALOGUE current = catalog;
                if (current.F_CATALOGUE1 != null)
                {
                    do
                    {
                        current = current.F_CATALOGUE1;
                        if ((toImport && current.F_CATALOGUE2.Count(c => c.ExistLocal == false && c.ToImport == toImport) == 1)
                            || (!toImport && current.F_CATALOGUE2.Count(c => c.ExistLocal == false && c.ToImport == !toImport) == 0))
                        {
                            current.CheckChild = false;
                            current.ToImport = toImport;
                            current.CheckChild = true;
                        }
                    }
                    while (current.F_CATALOGUE1 != null);
                }
            }

            if (!catalog.ExistLocal)
                catalog.ToImport = toImport;

            if (AutoSelectCatalogEnfant || !toImport)
            {
                if (catalog.CheckChild)
                    foreach (var child in catalog.SortChildren)
                        ChangeToImportCatalogs(child, toImport);
            }
        }

        #endregion

        #region Tools

        public void ControleMappageArticles(Boolean Actif)
        {
            try
            {
                Loading Loading = new Loading();
                Loading.Show();
                ControlArticle control = new ControlArticle(Actif);
                Loading.Close();
                control.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void ControlURLRewrite(bool rewritePrestashop)
        {
            try
            {
                Loading Loading = new Loading();
                Loading.Show();
                ControlURLRewrite control = new ControlURLRewrite(rewritePrestashop);
                Loading.Close();
                control.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void ClearPsImage()
        {
            try
            {
                Boolean CanClear = true;
                Model.Prestashop.PsConfigurationRepository PsConfigurationRepository = new Model.Prestashop.PsConfigurationRepository();
                if (PsConfigurationRepository.ExistName("PS_LEGACY_IMAGES"))
                {
                    Model.Prestashop.PsConfiguration image_storage_mode = PsConfigurationRepository.ReadName("PS_LEGACY_IMAGES");
                    int value;
                    if (int.TryParse(image_storage_mode.Value, out value))
                    {
                        if (value != (short)Core.Global.GetConfig().ConfigImageStorageMode)
                        {
                            if (MessageBox.Show("Attention le système de stockage paramétré dan PrestaConnect est différent de celui de PrestaShop !\n"
                                + "Êtes-vous sûrs de vouloir nettoyer la table ps_image selon la présence des fichiers sur le FTP ?",
                                "Mode de stockage des images", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                                CanClear = false;
                        }
                    }
                }
                if (CanClear)
                {
                    Loading Loading = new Loading();
                    Loading.Show();
                    NettoyageImage clear = new NettoyageImage();
                    Loading.Close();
                    clear.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void OpenLienConditionnement()
        {
            try
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();
                LienConditionnement form = new LienConditionnement();
                Loading.Close();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        #endregion
    }
}
