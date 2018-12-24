using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Contexts
{
    internal sealed class ArticleContext : Context
    {
        #region Properties

        private SyncArticle item;
        public SyncArticle Item
        {
            get { return item; }
            set
            {
                item = value;
                OnPropertyChanged("Item");
            }
        }

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

        private ObservableCollection<AssociateCatalog> catalogs;
        public ObservableCollection<AssociateCatalog> Catalogs
        {
            get { return catalogs; }
            set
            {
                catalogs = value;
                OnPropertyChanged("Catalogs");
            }
        }

        private AssociateCatalog selectedCatalog;
        public AssociateCatalog SelectedCatalog
        {
            get { return selectedCatalog; }
            set
            {
                selectedCatalog = value;
                OnPropertyChanged("SelectedCatalog");
            }
        }

        private ObservableCollection<Model.Local.ConditioningArticle> listConditioningArticle;
        public ObservableCollection<Model.Local.ConditioningArticle> ListConditioningArticle
        {
            get { return listConditioningArticle; }
            set
            {
                listConditioningArticle = value;
                OnPropertyChanged("ListConditioningArticle");
            }
        }

        private Model.Local.ConditioningArticle selectedConditioningArticle;
        public Model.Local.ConditioningArticle SelectedConditioningArticle
        {
            get { return selectedConditioningArticle; }
            set
            {
                selectedConditioningArticle = value;
                OnPropertyChanged("SelectedConditioningArticle");
            }
        }

        private ObservableCollection<Model.Local.AttributeArticle> listAttributeArticle;
        public ObservableCollection<Model.Local.AttributeArticle> ListAttributeArticle
        {
            get { return listAttributeArticle; }
            set
            {
                listAttributeArticle = value;
                OnPropertyChanged("ListAttributeArticle");
            }
        }

        private Model.Local.AttributeArticle selectedAttributeArticle;
        public Model.Local.AttributeArticle SelectedAttributeArticle
        {
            get { return selectedAttributeArticle; }
            set
            {
                selectedAttributeArticle = value;
                OnPropertyChanged("SelectedAttributeArticle");
                SelectedArticleImageGamme = null;
                if (value != null && SelectedAttributeArticle.AttArt_Id != 0)
                {
                    foreach (Model.Local.ArticleImage item in ListArticleImageGamme)
                    {
                        item.AttachedToAttributeArticle = (SelectedAttributeArticle.ListImage.Contains(item.ImaArt_Id));
                    }
                }
                OnPropertyChanged("LabelAttachAttributeArticleImage");
            }
        }

        public string LabelAttachAttributeArticleImage
        {
            get
            {
                return (SelectedAttributeArticle != null)
                    ? "Affectation des images pour la déclinaison : " + SelectedAttributeArticle.EnumereGamme1 + " " + SelectedAttributeArticle.EnumereGamme2
                    : "Affectation des images pour toutes les déclinaisons :";
            }
        }

        public string LabelAttachCompositionArticleImage
        {
            get
            {
                return (SelectedCompositionArticle != null && SelectedCompositionArticle.ComArt_Id != 0)
                    ? "Affectation des images pour la composition : " + SelectedCompositionArticle.Declinaison
                    : "Affectation des images pour toutes les compositions :";
            }
        }

        private ObservableCollection<Model.Local.ArticleImage> listArticleImageGamme;
        public ObservableCollection<Model.Local.ArticleImage> ListArticleImageGamme
        {
            get { return listArticleImageGamme; }
            set
            {
                listArticleImageGamme = value;
                OnPropertyChanged("ListArticleImageGamme");
            }
        }

        private Model.Local.ArticleImage selectedArticleImageGamme;
        public Model.Local.ArticleImage SelectedArticleImageGamme
        {
            get { return selectedArticleImageGamme; }
            set
            {
                selectedArticleImageGamme = value;
                OnPropertyChanged("SelectedArticleImageGamme");
            }
        }

        private ObservableCollection<Model.Local.ArticleImage> listArticleImageComposition;
        public ObservableCollection<Model.Local.ArticleImage> ListArticleImageComposition
        {
            get { return listArticleImageComposition; }
            set
            {
                listArticleImageComposition = value;
                OnPropertyChanged("ListArticleImageComposition");
            }
        }

        private Model.Local.ArticleImage selectedArticleImageComposition;
        public Model.Local.ArticleImage SelectedArticleImageComposition
        {
            get { return selectedArticleImageComposition; }
            set
            {
                selectedArticleImageComposition = value;
                OnPropertyChanged("SelectedArticleImageComposition");
            }
        }

        private ObservableCollection<Model.Prestashop.PsFeatureLang> listFeature;
        public ObservableCollection<Model.Prestashop.PsFeatureLang> ListFeature
        {
            get { return listFeature; }
            set
            {
                listFeature = value;
                OnPropertyChanged("ListFeature");
            }
        }

        private Model.Prestashop.PsFeatureLang selectedFeature = new Model.Prestashop.PsFeatureLang();
        public Model.Prestashop.PsFeatureLang SelectedFeature
        {
            get { return selectedFeature; }
            set
            {
                selectedFeature = value;
                OnPropertyChanged("SelectedFeature");
            }
        }

        private ObservableCollection<Model.Prestashop.PsAttributeGroupLang> listPsAttributeGroupLang;
        public ObservableCollection<Model.Prestashop.PsAttributeGroupLang> ListPsAttributeGroupLang
        {
            get { return listPsAttributeGroupLang; }
            set { listPsAttributeGroupLang = value; OnPropertyChanged("ListPsAttributeGroupLang"); OnPropertyChanged("ListFreePsAttributeGroupLang"); }
        }

        public ObservableCollection<Model.Prestashop.PsAttributeGroupLang> ListFreePsAttributeGroupLang
        {
            get
            {
                List<uint> groups = ListAttachPsAttributeGroupLang.Select(ag => ag.IDAttributeGroup).ToList();
                return new ObservableCollection<Model.Prestashop.PsAttributeGroupLang>(listPsAttributeGroupLang.Where(ag => !groups.Contains(ag.IDAttributeGroup)));
            }
        }

        private Model.Prestashop.PsAttributeGroupLang selectedFreePsAttributeGroupLang = new Model.Prestashop.PsAttributeGroupLang();
        public Model.Prestashop.PsAttributeGroupLang SelectedFreePsAttributeGroupLang
        {
            get { return selectedFreePsAttributeGroupLang; }
            set { selectedFreePsAttributeGroupLang = value; OnPropertyChanged("SelectedFreePsAttributeGroupLang"); }
        }

        private ObservableCollection<Model.Prestashop.PsAttributeGroupLang> listAttachPsAttributeGroupLang = new ObservableCollection<Model.Prestashop.PsAttributeGroupLang>();
        public ObservableCollection<Model.Prestashop.PsAttributeGroupLang> ListAttachPsAttributeGroupLang
        {
            get { return listAttachPsAttributeGroupLang; }
            set { listAttachPsAttributeGroupLang = value; OnPropertyChanged("ListAttachPsAttributeGroupLang"); OnPropertyChanged("ListFreePsAttributeGroupLang"); }
        }

        private Model.Prestashop.PsAttributeGroupLang selectedAttachPsAttributeGroupLang = new Model.Prestashop.PsAttributeGroupLang();
        public Model.Prestashop.PsAttributeGroupLang SelectedAttachPsAttributeGroupLang
        {
            get { return selectedAttachPsAttributeGroupLang; }
            set { selectedAttachPsAttributeGroupLang = value; OnPropertyChanged("SelectedAttachPsAttributeGroupLang"); }
        }

        private ObservableCollection<Model.Sage.F_ARTICLE_Composition> listResultSearchCompositionArticle;
        public ObservableCollection<Model.Sage.F_ARTICLE_Composition> ListResultSearchCompositionArticle
        {
            get { return listResultSearchCompositionArticle; }
            set { listResultSearchCompositionArticle = value; OnPropertyChanged("ListResultSearchCompositionArticle"); OnPropertyChanged("ShowResultAttributeColumn"); OnPropertyChanged("ShowResultConditioningColumn"); }
        }
        private Model.Sage.F_ARTICLE_Composition selectedResultSearchCompositionArticle;
        public Model.Sage.F_ARTICLE_Composition SelectedResultSearchCompositionArticle
        {
            get { return selectedResultSearchCompositionArticle; }
            set { selectedResultSearchCompositionArticle = value; OnPropertyChanged("SelectedResultSearchCompositionArticle"); }
        }

        private ObservableCollection<Model.Local.CompositionArticle> listCompositionArticle;
        public ObservableCollection<Model.Local.CompositionArticle> ListCompositionArticle
        {
            get { return listCompositionArticle; }
            set
            {
                listCompositionArticle = value;
                OnPropertyChanged("ListCompositionArticle");
                OnPropertyChanged("CanDefineCompositionArticleAttributeGroup");
                OnPropertyChanged("ShowAttributeColumn");
                OnPropertyChanged("ShowConditioningColumn");
                SelectedCompositionArticle = new Model.Local.CompositionArticle();
            }
        }
        private Model.Local.CompositionArticle selectedCompositionArticle;
        public Model.Local.CompositionArticle SelectedCompositionArticle
        {
            get { return selectedCompositionArticle; }
            set
            {
                if (selectedCompositionArticle != null)
                    selectedCompositionArticle.SelectedPsAttributeGroupLang = null;
                selectedCompositionArticle = value;
                OnPropertyChanged("SelectedCompositionArticle");

                SelectedArticleImageComposition = null;
                if (value != null && SelectedCompositionArticle.ComArt_Id != 0)
                {
                    foreach (Model.Local.ArticleImage item in ListArticleImageComposition)
                    {
                        item.AttachedToCompositionArticle = (SelectedCompositionArticle.ListImage.Contains(item.ImaArt_Id));
                    }
                }
                OnPropertyChanged("LabelAttachCompositionArticleImage");

            }
        }

        private Boolean redirectComposition = Core.Global.GetConfig().ArticleRedirectionCompositionActif;
        public Boolean RedirectComposition
        {
            get { return redirectComposition; }
            set { redirectComposition = value; OnPropertyChanged("RedirectComposition"); }
        }

        public Visibility ShowGamme2Column { get { return (Item.Local.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMultiGammes) ? Visibility.Visible : Visibility.Hidden; } }
        public Visibility ShowAttributeColumn { get { return ((ListCompositionArticle != null && ListCompositionArticle.Count(ca => ca.ComArt_F_ARTENUMREF_SagId != null) > 0) ? Visibility.Visible : Visibility.Hidden); } }
        public Visibility ShowConditioningColumn { get { return ((ListCompositionArticle != null && ListCompositionArticle.Count(ca => ca.ComArt_F_CONDITION_SagId != null) > 0) ? Visibility.Visible : Visibility.Hidden); } }

        public Visibility ShowGammeActiveColumn { get { return (Core.Global.ExistAECAttributeStatut()) ? Visibility.Visible : Visibility.Hidden; } }
        public Visibility ShowCompositionActiveColumn { get { return (Core.Global.ExistAECAttributeStatut()) ? Visibility.Visible : Visibility.Hidden; } }
        public Boolean EnableGammeActiveColumn { get { return Core.Global.ExistAECAttributeStatut(); } }
        public Boolean EnableCompositionActiveColumn { get { return Core.Global.ExistAECAttributeStatut(); } }
        public Boolean ReadOnlyGammeActiveColumn { get { return !Core.Global.ExistAECAttributeStatut(); } }
        public Boolean ReadOnlyCompositionActiveColumn { get { return !Core.Global.ExistAECAttributeStatut(); } }


        public Visibility ShowResultAttributeColumn { get { return ((ListResultSearchCompositionArticle != null && ListResultSearchCompositionArticle.Count(ca => ca.F_ARTENUMREF_SagId != null) > 0) ? Visibility.Visible : Visibility.Hidden); } }
        public Visibility ShowResultConditioningColumn { get { return ((ListResultSearchCompositionArticle != null && ListResultSearchCompositionArticle.Count(ca => ca.F_CONDITION_SagId != null) > 0) ? Visibility.Visible : Visibility.Hidden); } }

        // WPFTOOLKIT reference if normal DataGrid use System.Windows.Controls.DataGridSelectionMode
        public Microsoft.Windows.Controls.DataGridSelectionMode CompositionSelectionMode { get { return (ListAttachPsAttributeGroupLang.Count == 1) ? Microsoft.Windows.Controls.DataGridSelectionMode.Single : Microsoft.Windows.Controls.DataGridSelectionMode.Extended; } }
        //public Visibility ShowFastSelector { get { return (ListAttachPsAttributeGroupLang.Count > 1) ? Visibility.Visible : Visibility.Collapsed; } }

        private string newAttributeGroupName = string.Empty;
        public string NewAttributeGroupName
        {
            get { return newAttributeGroupName; }
            set { newAttributeGroupName = value; OnPropertyChanged("NewAttributeGroupName"); }
        }

        private string newAttributeValue = string.Empty;
        public string NewAttributeValue
        {
            get { return newAttributeValue; }
            set { newAttributeValue = value; OnPropertyChanged("NewAttributeValue"); }
        }

        public bool CanDefineCompositionArticleAttributeGroup
        {
            get
            {
                return Item.CanDefineCompositionArticleAttributeGroup && ListCompositionArticle.Count == 0;
            }
        }

		private List<Model.Local.ArticleAdditionalField> listResultAdditionnalFieldArticle;
		public List<Model.Local.ArticleAdditionalField> ListResultAdditionnalFieldArticle
		{
			get { return listResultAdditionnalFieldArticle; }
			set { listResultAdditionnalFieldArticle = value; OnPropertyChanged("ListResultAdditionnalFieldArticle"); }
		}

		private Model.Local.ArticleAdditionalField selectedResultAdditionnalFieldArticle;
		public Model.Local.ArticleAdditionalField SelectedResultAdditionnalFieldArticle
		{
			get { return selectedResultAdditionnalFieldArticle; }
			set {
				selectedResultAdditionnalFieldArticle = value;
				OnPropertyChanged("SelectedResultAdditionnalFieldArticle");

				if (value != null)
					SelectedResultAdditionnalFieldArticleType = (int)selectedResultAdditionnalFieldArticle.Type;
			}
		}

		private Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType selectedResultAdditionnalFieldArticleType;
		public int SelectedResultAdditionnalFieldArticleType
		{
			get { return (int)selectedResultAdditionnalFieldArticleType; }
			set
			{
				RateActive = System.Windows.Visibility.Hidden;
				TextAreaActive = System.Windows.Visibility.Hidden;
				TextActive = System.Windows.Visibility.Hidden;
				CheckBoxActive = System.Windows.Visibility.Hidden;
				SelectorActive = System.Windows.Visibility.Hidden;
				switch (value)
				{
					case 0:
						selectedResultAdditionnalFieldArticleType = Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.Rate;
						RateActive = System.Windows.Visibility.Visible;
						break;
					case 1:
						selectedResultAdditionnalFieldArticleType = Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.TextAreaMCE;
						TextActive = System.Windows.Visibility.Visible;
						break;/*
					case 2:
						selectedResultAdditionnalFieldArticleType = Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.Text;
						TextActive = System.Windows.Visibility.Visible;
						break;
					case 3:
						selectedResultAdditionnalFieldArticleType = Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.CheckBox;
						CheckBoxActive = System.Windows.Visibility.Visible;
						break;
					case 4:
						selectedResultAdditionnalFieldArticleType = Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.Selector;
						SelectorActive = System.Windows.Visibility.Visible;
						break;*/
					default:
						selectedResultAdditionnalFieldArticleType = Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.Text;
						TextActive = System.Windows.Visibility.Visible;
						break;
				}
				OnPropertyChanged("SelectedResultAdditionnalFieldArticleType");
			}
		}

		private bool rateActive;
		public System.Windows.Visibility RateActive
		{
			get { return (rateActive ? System.Windows.Visibility.Visible : Visibility.Hidden); }
			set
			{
				rateActive = (value == System.Windows.Visibility.Visible);
				OnPropertyChanged("RateActive");
			}
		}

		private bool textAreaActive;
		public System.Windows.Visibility TextAreaActive
		{
			get { return (textAreaActive ? System.Windows.Visibility.Visible : Visibility.Hidden); }
			set
			{
				textAreaActive = (value == System.Windows.Visibility.Visible);
				OnPropertyChanged("TextAreaActive");
			}
		}

		private bool textActive;
		public System.Windows.Visibility TextActive
		{
			get { return (textActive ? System.Windows.Visibility.Visible : Visibility.Hidden); }
			set
			{
				textActive = (value == System.Windows.Visibility.Visible);
				OnPropertyChanged("TextActive");
			}
		}

		private bool checkBoxActive;
		public System.Windows.Visibility CheckBoxActive
		{
			get { return (checkBoxActive ? System.Windows.Visibility.Visible : Visibility.Hidden); }
			set
			{
				checkBoxActive = (value == System.Windows.Visibility.Visible);
				OnPropertyChanged("CheckBoxActive");
			}
		}

		private bool selectorActive;
		public System.Windows.Visibility SelectorActive
		{
			get { return (selectorActive ? System.Windows.Visibility.Visible : Visibility.Hidden); }
			set
			{
				selectorActive = (value == System.Windows.Visibility.Visible);
				OnPropertyChanged("SelectorActive");
			}
		}

		#region Filtres composition

        private ObservableCollection<Model.Sage.F_TAXE> listF_TAXE = new ObservableCollection<Model.Sage.F_TAXE>();
        public ObservableCollection<Model.Sage.F_TAXE> ListF_TAXE
        {
            get { return listF_TAXE; }
            set { listF_TAXE = value; OnPropertyChanged("ListF_TAXE"); SelectedF_TAXE = null; }
        }
        private Model.Sage.F_TAXE selectedF_TAXE;
        public Model.Sage.F_TAXE SelectedF_TAXE
        {
            get { return selectedF_TAXE; }
            set { selectedF_TAXE = value; OnPropertyChanged("SelectedF_TAXE"); }
        }

        private ObservableCollection<Model.Sage.F_FAMILLE> listF_FAMILLE = new ObservableCollection<Model.Sage.F_FAMILLE>();
        public ObservableCollection<Model.Sage.F_FAMILLE> ListF_FAMILLE
        {
            get { return listF_FAMILLE; }
            set { listF_FAMILLE = value; OnPropertyChanged("ListF_FAMILLE"); SelectedF_FAMILLE = null; }
        }
        private Model.Sage.F_FAMILLE selectedF_FAMILLE;
        public Model.Sage.F_FAMILLE SelectedF_FAMILLE
        {
            get { return selectedF_FAMILLE; }
            set { selectedF_FAMILLE = value; OnPropertyChanged("SelectedF_FAMILLE"); }
        }

        private ObservableCollection<Model.Sage.P_GAMME> listP_GAMME = new ObservableCollection<Model.Sage.P_GAMME>();
        public ObservableCollection<Model.Sage.P_GAMME> ListP_GAMME
        {
            get { return listP_GAMME; }
            set { listP_GAMME = value; OnPropertyChanged("ListP_GAMME"); SelectedP_GAMME1 = null; SelectedP_GAMME2 = null; }
        }
        private Model.Sage.P_GAMME selectedP_GAMME1;
        public Model.Sage.P_GAMME SelectedP_GAMME1
        {
            get { return selectedP_GAMME1; }
            set { selectedP_GAMME1 = value; OnPropertyChanged("SelectedP_GAMME1"); }
        }
        private Model.Sage.P_GAMME selectedP_GAMME2;
        public Model.Sage.P_GAMME SelectedP_GAMME2
        {
            get { return selectedP_GAMME2; }
            set { selectedP_GAMME2 = value; OnPropertyChanged("SelectedP_GAMME2"); }
        }

        private ObservableCollection<Model.Sage.P_CONDITIONNEMENT> listP_CONDITIONNEMENT = new ObservableCollection<Model.Sage.P_CONDITIONNEMENT>();
        public ObservableCollection<Model.Sage.P_CONDITIONNEMENT> ListP_CONDITIONNEMENT
        {
            get { return listP_CONDITIONNEMENT; }
            set { listP_CONDITIONNEMENT = value; OnPropertyChanged("ListP_CONDITIONNEMENT"); SelectedP_CONDITIONNEMENT = null; }
        }
        private Model.Sage.P_CONDITIONNEMENT selectedP_CONDITIONNEMENT;
        public Model.Sage.P_CONDITIONNEMENT SelectedP_CONDITIONNEMENT
        {
            get { return selectedP_CONDITIONNEMENT; }
            set { selectedP_CONDITIONNEMENT = value; OnPropertyChanged("SelectedP_CONDITIONNEMENT"); }
        }

        private Boolean filterReferenceContains = true;
        public Boolean FilterReferenceContains
        {
            get { return filterReferenceContains; }
            set { filterReferenceContains = value; OnPropertyChanged("FilterReferenceContains"); }
        }
        private Boolean filterReferenceStartWith = false;
        public Boolean FilterReferenceStartWith
        {
            get { return filterReferenceStartWith; }
            set { filterReferenceStartWith = value; OnPropertyChanged("FilterReferenceStartWith"); }
        }
        private String filterReferenceValue = string.Empty;
        public String FilterReferenceValue
        {
            get { return filterReferenceValue; }
            set { filterReferenceValue = value; OnPropertyChanged("FilterReferenceValue"); }
        }

        private Boolean filterDesignationContains = true;
        public Boolean FilterDesignationContains
        {
            get { return filterDesignationContains; }
            set { filterDesignationContains = value; OnPropertyChanged("FilterDesignationContains"); }
        }
        private Boolean filterDesignationStartWith = false;
        public Boolean FilterDesignationStartWith
        {
            get { return filterDesignationStartWith; }
            set { filterDesignationStartWith = value; OnPropertyChanged("FilterDesignationStartWith"); }
        }
        private String filterDesignationValue = string.Empty;
        public String FilterDesignationValue
        {
            get { return filterDesignationValue; }
            set { filterDesignationValue = value; OnPropertyChanged("FilterDesignationValue"); }
        }

        private Boolean filterCompositionAll = true;
        public Boolean FilterCompositionAll
        {
            get { return filterCompositionAll; }
            set { filterCompositionAll = value; OnPropertyChanged("FilterCompositionAll"); }
        }
        private Boolean filterCompositionCurrent = false;
        public Boolean FilterCompositionCurrent
        {
            get { return filterCompositionCurrent; }
            set { filterCompositionCurrent = value; OnPropertyChanged("FilterCompositionCurrent"); }
        }
        private Boolean filterCompositionNothing = false;
        public Boolean FilterCompositionNothing
        {
            get { return filterCompositionNothing; }
            set { filterCompositionNothing = value; OnPropertyChanged("FilterCompositionNothing"); }
        }
        private Boolean filterArticle = true;
        public Boolean FilterArticle
        {
            get { return filterArticle; }
            set { filterArticle = value; OnPropertyChanged("FilterArticle"); }
        }

        private Boolean showResultInImportSageFilter = false;
        public Boolean ShowResultInImportSageFilter
        {
            get { return showResultInImportSageFilter; }
            set { showResultInImportSageFilter = value; OnPropertyChanged("ShowResultInImportSageFilter"); SearchArticleComposition(); }
        }

        private String fastSelectionReferenceComposition = string.Empty;
        public String FastSelectionReferenceComposition
        {
            get { return fastSelectionReferenceComposition; }
            set { fastSelectionReferenceComposition = value.ToUpper(); OnPropertyChanged("FastSelectionReferenceComposition"); }
        }
        private String fastSelectionDesignationComposition = string.Empty;
        public String FastSelectionDesignationComposition
        {
            get { return fastSelectionDesignationComposition; }
            set { fastSelectionDesignationComposition = value.ToLower(); OnPropertyChanged("FastSelectionDesignationComposition"); }
        }
        private String fastSelectionAttributeComposition = string.Empty;
        public String FastSelectionAttributeComposition
        {
            get { return fastSelectionAttributeComposition; }
            set { fastSelectionAttributeComposition = value.ToLower(); OnPropertyChanged("FastSelectionAttributeComposition"); }
        }

        #endregion

        private Model.Local.ConditioningArticleRepository ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
        private Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
        private Model.Local.CompositionArticleAttributeGroupRepository CompositionArticleAttributeGroupRepository = new Model.Local.CompositionArticleAttributeGroupRepository();
        private Model.Local.CompositionArticleAttributeRepository CompositionArticleAttributeRepository = new Model.Local.CompositionArticleAttributeRepository();
        private Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();

        private string strCompositionFilteredProducts = string.Empty;
        public string StrCompositionFilteredProducts
        {
            get { return strCompositionFilteredProducts; }
            set { strCompositionFilteredProducts = value; OnPropertyChanged("StrCompositionFilteredProducts"); }
        }

        #endregion
        #region Constructors

        public ArticleContext(Model.Local.Article local)
            : base()
        {
            LoadCatalogsWorker = new BackgroundWorker();
            LoadCatalogsWorker.WorkerReportsProgress = true;

            if (local.Pre_Id != null && !new Model.Prestashop.PsProductRepository().ExistId((uint)local.Pre_Id))
                if (MessageBox.Show("Le produit Prestashop correspondant à l'article " + local.Art_Ref + " n'existe plus !"
                    + "\nSouhaitez-vous recréer l'article ?", "Produit supprimé", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                    local = ArticleRepository.ReadArticle(local.Art_Id);
                    local.Pre_Id = null;
                    local.Art_Date = DateTime.Now;
                    ArticleRepository.Save();

                    Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                    foreach (Model.Local.AttributeArticle AttributeArticle in AttributeArticleRepository.ListArticle(local.Art_Id))
                        AttributeArticle.Pre_Id = null;
                    AttributeArticleRepository.Save();

                    Model.Local.ConditioningArticleRepository ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
                    foreach (Model.Local.ConditioningArticle ConditioningArticle in ConditioningArticleRepository.ListArticle(local.Art_Id))
                        ConditioningArticle.Pre_Id = null;
                    ConditioningArticleRepository.Save();

                    Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                    foreach (Model.Local.ArticleImage ArticleImage in ArticleImageRepository.ListArticle(local.Art_Id))
                        ArticleImage.Pre_Id = null;
                    ArticleImageRepository.Save();

                    Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                    foreach (Model.Local.Attachment Attachment in AttachmentRepository.ListArticle(local.Art_Id))
                        Attachment.Pre_Id = null;
                    AttachmentRepository.Save();

                    Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                    foreach (Model.Local.CompositionArticle CompositionArticle in CompositionArticleRepository.ListArticle(local.Art_Id))
                        CompositionArticle.Pre_Id = null;
                    CompositionArticleRepository.Save();
                }

            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
            Item = new SyncArticle(local, F_ARTICLERepository.ReadArticle(local.Sag_Id));
            Catalogs = new ObservableCollection<AssociateCatalog>();

            Core.Temp.Article = local.Art_Id;

            this.LoadAttributeArticle();
            this.LoadConditioningArticle();

            ListFeature = new ObservableCollection<Model.Prestashop.PsFeatureLang>(new Model.Prestashop.PsFeatureLangRepository().ListLang(Core.Global.Lang, Core.Global.CurrentShop.IDShop));

            #region Composition
            ListF_TAXE = new ObservableCollection<Model.Sage.F_TAXE>(new Model.Sage.F_TAXERepository().ListTTauxSens(ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_TTaux.Taux, (short)ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_Sens.Collectee));
            if (Core.Temp.selected_taxe_composition != null)
                SelectedF_TAXE = ListF_TAXE.FirstOrDefault(t => t.TA_Code == Core.Temp.selected_taxe_composition.TA_Code);
            if (!string.IsNullOrWhiteSpace(Core.Temp.reference_sage_composition))
                FilterReferenceValue = Core.Temp.reference_sage_composition;
            if (!string.IsNullOrWhiteSpace(Core.Temp.designation_composition))
                FilterDesignationValue = Core.Temp.designation_composition;
            ListF_FAMILLE = new ObservableCollection<Model.Sage.F_FAMILLE>(new Model.Sage.F_FAMILLERepository().List());
            ListP_GAMME = new ObservableCollection<Model.Sage.P_GAMME>(new Model.Sage.P_GAMMERepository().ListIntituleNotNull());
            ListP_CONDITIONNEMENT = new ObservableCollection<Model.Sage.P_CONDITIONNEMENT>(new Model.Sage.P_CONDITIONNEMENTRepository().ListIntituleNotNull());

            LoadCompositionArticleAttributeGroup();
            LoadCompositionArticle();
			#endregion

			LoadAdditionnalFieldArticle();
		}

		public void LoadAdditionnalFieldArticle()
		{
			if (Core.Global.GetConfig().ModuleDWFProductGuideratesActif || Core.Global.GetConfig().ModuleDWFProductExtraFieldsActif)
			{
				List<Model.Local.ArticleAdditionalField> ListInfoPrestaConnect = new Model.Local.ArticleAdditionalFieldRepository().ListArticle(Core.Temp.Article);
				ListResultAdditionnalFieldArticle = new List<Model.Local.ArticleAdditionalField>();
				if (Core.Global.GetConfig().ModuleDWFProductGuideratesActif)
				{
					List<Model.Prestashop.PsDWFProductGuiderates> ListeRates = new Model.Prestashop.PsDWFProductGuideratesRepository().ListActive();
					foreach (Model.Prestashop.PsDWFProductGuiderates rate in ListeRates)
					{
						string FieldValue1 = "";
						string FieldValue2 = "";
						if (ListInfoPrestaConnect.Where(obj => obj.FieldName == rate.Name).Count() > 0)
						{
							Model.Local.ArticleAdditionalField info = ListInfoPrestaConnect.Where(obj => obj.FieldName == rate.Name).FirstOrDefault();
							FieldValue1 = info.FieldValue;
							FieldValue2 = info.FieldValue2;
						}
						ListResultAdditionnalFieldArticle.Add(new Model.Local.ArticleAdditionalField()
						{
							Art_id = Core.Temp.Article,
							FieldName = rate.Name,
							FieldNameLang = rate.Name,
							FieldValue = FieldValue1,
							FieldValue2 = FieldValue2,
							Type = Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.Rate,
						});
					}
				}
				if (Core.Global.GetConfig().ModuleDWFProductExtraFieldsActif)
				{
					List<Model.Prestashop.PsDWFProductExtraField> ListExtraField = new Model.Prestashop.PsDWFProductExtraFieldRepository().ListActive();
					foreach (Model.Prestashop.PsDWFProductExtraField extraField in ListExtraField)
					{
						string FieldValue1 = "";
						string FieldValue2 = "";
						string FieldNameLang = (new Model.Prestashop.PsDWFProductExtraFieldsLangRepository().GetDWFProductExtraFieldsLang(extraField.IdDWFProductExtraFields, 1) != null ?
								new Model.Prestashop.PsDWFProductExtraFieldsLangRepository().GetDWFProductExtraFieldsLang(extraField.IdDWFProductExtraFields, 1).Label :
								extraField.FieldName);
						if (ListInfoPrestaConnect.Where(obj => obj.FieldName == extraField.FieldName).Count() > 0)
						{
							Model.Local.ArticleAdditionalField info = ListInfoPrestaConnect.Where(obj => obj.FieldName == extraField.FieldName).FirstOrDefault();
							FieldValue1 = info.FieldValue;
							FieldValue2 = info.FieldValue2;
						}
						ListResultAdditionnalFieldArticle.Add(new Model.Local.ArticleAdditionalField()
						{
							Art_id = Core.Temp.Article,
							FieldName = extraField.FieldName,
							FieldNameLang = FieldNameLang,
							FieldValue = FieldValue1,
							FieldValue2 = FieldValue2,
							Type = Model.Local.ArticleAdditionalField.AffectType(extraField.Type),
						});
					}
				}
				RateActive = Visibility.Hidden;
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
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Catalogs.Clear(); }), null);

            IsBusy = true;
            LoadingStep = "Chargement des associations ...";

            Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
            Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();

            foreach (var root in GetChildren(null, CatalogRepository, ArticleCatalogRepository))
                LoadCatalogsWorker.ReportProgress(0, root);
        }
        private void LoadCatalogsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(
                delegate { Catalogs.Add(e.UserState as AssociateCatalog); }), null);
        }
        private void LoadCatalogsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingStep = string.Empty;
            IsBusy = false;

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
        }

        #endregion
        #region Methods

        private IEnumerable<AssociateCatalog> GetChildren(Model.Local.Catalog parent, Model.Local.CatalogRepository local, Model.Local.ArticleCatalogRepository local2)
        {
            List<Model.Local.Catalog> catalogs = (parent == null) ? local.RootList() : local.ListParent(parent.Cat_Id);

            foreach (var catalog in catalogs)
            {
                AssociateCatalog hierarchicalCatalog = new AssociateCatalog(Item.Local, catalog);

                if (hierarchicalCatalog.ExistToLocal)
                {
                    if (Item == null && hierarchicalCatalog.Id == Item.Local.Cat_Id)
                        hierarchicalCatalog.ToAssociate = true;

                    foreach (var child in GetChildren(catalog, local, local2))
                        hierarchicalCatalog.Children.Add(child);

                    if (Item != null)
                    {
                        if (Item.Local.Art_Type == (short)Model.Local.Article.enum_TypeArticle.ArticleComposition && hierarchicalCatalog.Id == Item.Local.Cat_Id)
                            hierarchicalCatalog.ToAssociate = true;
                        else
                            hierarchicalCatalog.ToAssociate = local2.ExistArticleCatalog(Item.Id, hierarchicalCatalog.Local.Cat_Id);
                    }

                    yield return hierarchicalCatalog;
                }
            }
        }

        private IEnumerable<AssociateCatalog> GetCatalogs(AssociateCatalog catalog)
        {
            foreach (var child in catalog.Children)
            {
                yield return child;

                foreach (var current in GetCatalogs(child))
                    yield return current;
            }
        }

        public void LoadCatalogs()
        {
            if (!LoadCatalogsWorker.IsBusy)
                LoadCatalogsWorker.RunWorkerAsync();
        }

        public void ChangeToAssociateCatalogs(AssociateCatalog catalog, bool toAssociate)
        {
            catalog.ToAssociate = toAssociate;
            Item.HasUpdated = true;

            //foreach (var child in catalog.Children)
            //    ChangeToAssociateCatalogs(child, toAssociate);
        }
        public IEnumerable<AssociateCatalog> GetCatalogs()
        {
            foreach (var child in Catalogs)
            {
                yield return child;

                foreach (var current in GetCatalogs(child))
                    yield return current;
            }
        }
        public void DefaultCatalog(AssociateCatalog defaultCatalog)
        {
            foreach (var catalog in GetCatalogs())
                if (catalog.IsDefault)
                {
                    catalog.Item.Cat_Id = defaultCatalog.Local.Cat_Id;
                    catalog.IsDefault = false;
                    break;
                }

            foreach (var catalog in GetCatalogs())
                if (catalog.Local.Cat_Id == defaultCatalog.Local.Cat_Id)
                {
                    catalog.Item.Cat_Id = defaultCatalog.Local.Cat_Id;
                    catalog.IsDefault = true;
                    break;
                }

            Item.Local.Cat_Id = defaultCatalog.Local.Cat_Id;
            Item.HasUpdated = true;
        }
        public void Update()
        {
            if (!LoadCatalogsWorker.IsBusy)
            {
                if (Item.Local.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition && Item.Local.Art_Id == 0)
                {
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                    Item.Local.Art_Date = DateTime.Now;
                    ArticleRepository.Add(Item.Local);
                    InitUpdated();
                }
                else if (!Item.HasUpdated && Item.HasUpdatedSync)
                {
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                    Model.Local.Article Article = ArticleRepository.ReadArticle(Item.Local.Art_Id);
                    if (Article != null)
                    {
                        Article.Art_Sync = Item.Local.Art_Sync;
                        Article.Art_SyncPrice = Item.Local.Art_SyncPrice;
                    }
                    ArticleRepository.Save();
                }
                else if (Item.Updated)
                {
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                    Model.Local.Article Article = ArticleRepository.ReadArticle(Item.Local.Art_Id);

                    if (Article != null)
                    {
                        Article.Art_Active = Item.Local.Art_Active;
                        Article.Art_Date = DateTime.Now;
                        Article.Art_Description = Item.Local.Art_Description;
                        Article.Art_Description_Short = Item.Local.Art_Description_Short;
                        Article.Art_Ean13 = Item.Local.Art_Ean13;
                        Article.Art_LinkRewrite = Item.Local.Art_LinkRewrite;
                        Article.Art_MetaDescription = Item.Local.Art_MetaDescription;
                        Article.Art_MetaKeyword = Item.Local.Art_MetaKeyword;
                        Article.Art_MetaTitle = Item.Local.Art_MetaTitle;
                        Article.Art_Name = Item.Local.Art_Name;
                        Article.Art_Pack = Item.Local.Art_Pack;
                        Article.Art_Ref = Item.Local.Art_Ref;
                        Article.Art_Solde = Item.Local.Art_Solde;
                        Article.Art_Sync = Item.Local.Art_Sync;
                        Article.Art_SyncPrice = Item.Local.Art_SyncPrice;
                        Article.Cat_Id = Item.Local.Cat_Id;

                        Article.Art_RedirectType = Item.Local.Art_RedirectType;
                        Article.Art_RedirectProduct = Item.Local.Art_RedirectProduct;

                        if (Item.HasUpdatedAttribute)
                        {
                            foreach (Model.Local.AttributeArticle AttributeArticle in ListAttributeArticle)
                            {
                                foreach (int itemadd in AttributeArticle.ListImage)
                                {
                                    if (AttributeArticle.AttributeArticleImage.Count(ati => ati.ImaArt_Id == itemadd) == 0)
                                        AttributeArticle.AttributeArticleImage.Add(new Model.Local.AttributeArticleImage()
                                        {
                                            AttArt_Id = AttributeArticle.AttArt_Id,
                                            ImaArt_Id = itemadd,
                                        });
                                }
                                List<int> current_attach = AttributeArticle.AttributeArticleImage.Select(ati => ati.ImaArt_Id).ToList();
                                foreach (int itemremove in current_attach)
                                {
                                    if (!AttributeArticle.ListImage.Contains(itemremove))
                                        AttributeArticleRepository.DeleteLinkAttributeArticleImage(AttributeArticle.AttArt_Id, itemremove);
                                    //AttributeArticle.AttributeArticleImage.Remove(AttributeArticle.AttributeArticleImage.First(ati => ati.ImaArt_Id == itemremove));
                                }
                                AttributeArticle._HasUpdated = false;
                            }
                            this.AttributeArticleRepository.Save();
                        }

                        if (Item.HasUpdatedConditioning)
                        {
                            this.ConditioningArticleRepository.Save();
                            foreach (Model.Local.ConditioningArticle ConditioningArticle in ListConditioningArticle)
                            {
                                ConditioningArticle._HasUpdated = false;
                            }
                        }

                        if (Item.HasUpdatedCharacteristic)
                            this.SaveFeature();

                        if (Item.HasUpdatedCompositionArticleAttributeGroup)
                            this.SaveCompositionArticleAttributeGroup();

                        if (Item.HasUpdatedCompositionArticle)
                        {
                            this.SaveCompositionArticle();

                            if (!Article.Art_Sync && Article.CompositionComplete)
                            {
                                Article.Art_Sync = true;
                                Article.Art_SyncPrice = true;
                            }
                        }

                        InitUpdated();
                        ArticleRepository.Save();
                    }
                }
            }
        }

        public void UpdateDate()
        {
            if (!LoadCatalogsWorker.IsBusy && !Item.Updated)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(Item.Local.Art_Id);

                if (Article != null)
                {
                    Article.Art_Date = DateTime.Now;
                    ArticleRepository.Save();
                    Item.Local.Art_Date = Article.Art_Date;
                }
            }
        }

        private void InitUpdated()
        {
            Item.HasUpdated = false;
            Item.HasUpdatedSync = false;
            Item.HasUpdatedCharacteristic = false;
            Item.HasUpdatedAttribute = false;
            Item.HasUpdatedConditioning = false;
            Item.HasSelectedImage = false;
            Item.HasSelectedDocument = false;
            Item.HasUpdatedCompositionArticleAttributeGroup = false;
            Item.HasUpdatedCompositionArticle = false;
        }

        public void ImportConditioningArticle()
        {
            if (ListConditioningArticle.Count(c => c._HasUpdated) > 0
                && MessageBox.Show("Les modifications apportées aux conditionnements existants seront perdues lors de l'import, souhaitez-vous les enregistrer ?", "Import conditionnements", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.ConditioningArticleRepository.Save();

            Core.ImportSage.ImportArticle importarticle = new Core.ImportSage.ImportArticle();
            if (importarticle.ExecConditioning(Item.Sage, Item.Local))
                UpdateDate();

            this.LoadConditioningArticle();
        }

        public void ImportAttributeArticle()
        {
            if (ListAttributeArticle.Count(a => a._HasUpdated) > 0
                && MessageBox.Show("Les modifications apportées aux gammes existantes seront perdues lors de l'import, souhaitez-vous les enregistrer ?", "Import gammes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.AttributeArticleRepository.Save();

            Core.ImportSage.ImportArticle importarticle = new Core.ImportSage.ImportArticle();
            if (importarticle.ExecAttribute(Item.Sage, Item.Local))
                UpdateDate();

            this.LoadAttributeArticle();
        }

        private void LoadAttributeArticle()
        {
            this.AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
            ListAttributeArticle = new ObservableCollection<Model.Local.AttributeArticle>(AttributeArticleRepository.ListArticle(Item.Local.Art_Id));
            if (ListAttributeArticle.Count > 0)
                Core.Temp.ListPsAttributeLang = new Model.Prestashop.PsAttributeLangRepository().ListLang(Core.Global.Lang).AsParallel().ToList();

            ListArticleImageGamme = new ObservableCollection<Model.Local.ArticleImage>(new Model.Local.ArticleImageRepository().ListArticle(Item.Local.Art_Id));
            // affiche les images rattachées à la totalité des déclinaisons
            foreach (Model.Local.ArticleImage item in ListArticleImageGamme)
            {
                item.AttachedToAttributeArticle = ListAttributeArticle.Count == ListAttributeArticle.Count(a => a.ListImage.Contains(item.ImaArt_Id));
            }

        }
        private void LoadConditioningArticle()
        {
            this.ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
            ListConditioningArticle = new ObservableCollection<Model.Local.ConditioningArticle>(this.ConditioningArticleRepository.ListArticle(Item.Local.Art_Id));
        }

        public bool ImportSageInformationLibre()
        {
            bool import = false;
            if (MessageBox.Show("Souhaitez-vous importer les valeurs liées aux caractéristiques depuis Sage ?", "Import caractéristiques",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();
                ImportSageStatInfoLibre ImportSageInformationLibre = new ImportSageStatInfoLibre(Item.Id);
                ImportSageInformationLibre.ShowDialog();
                Loading.Close();

                ListFeature = new ObservableCollection<Model.Prestashop.PsFeatureLang>(new Model.Prestashop.PsFeatureLangRepository().ListLang(Core.Global.Lang, Core.Global.CurrentShop.IDShop));
                SelectedFeature = null;

                import = true;
            }
            return import;
        }

        public void SaveFeature()
        {
            Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
            Model.Local.Characteristic Characteristic = new Model.Local.Characteristic();
            foreach (Model.Prestashop.PsFeatureLang PsFeatureLang in ListFeature.Where(f => f.HasUpdated))
            {
                CharacteristicRepository = new Model.Local.CharacteristicRepository();
                Characteristic = new Model.Local.Characteristic();
                Boolean isCharacteristic = false;
                if (CharacteristicRepository.ExistFeatureArticle((int)PsFeatureLang.IDFeature, Item.Local.Art_Id))
                {
                    Characteristic = CharacteristicRepository.ReadFeatureArticle((int)PsFeatureLang.IDFeature, Item.Local.Art_Id);
                    isCharacteristic = true;
                }
                else
                {
                    Characteristic.Cha_IdFeature = (int)PsFeatureLang.IDFeature;
                    Characteristic.Art_Id = Item.Local.Art_Id;
                }
                if (PsFeatureLang.PsFeatureValueLangCustom != null
                    && !string.IsNullOrEmpty(PsFeatureLang.PsFeatureValueLangCustom.Value))
                {
                    if (isCharacteristic && !Characteristic.Cha_Custom)
                        Characteristic.Pre_Id = null;
                    Characteristic.Cha_Custom = true;
                    Characteristic.Cha_Value = PsFeatureLang.PsFeatureValueLangCustom.Value;
                }
                else
                {
                    Characteristic.Pre_Id = (int)PsFeatureLang.PsFeatureValueLang.IDFeatureValue;
                    Characteristic.Cha_Value = PsFeatureLang.PsFeatureValueLang.Value;
                    Characteristic.Cha_Custom = false;
                }
                if (isCharacteristic == true)
                {
                    CharacteristicRepository.Save();
                }
                else
                {
                    CharacteristicRepository.Add(Characteristic);
                }
                PsFeatureLang.HasUpdated = false;
            }
        }

        public void EraseFeatureProduct()
        {
            try
            {
                if (SelectedFeature != null)
                {
                    bool continu = true;
                    if (SelectedFeature.Characteristic != null && SelectedFeature.Characteristic.Pre_Id != null)
                    {
                        if (MessageBox.Show("Êtes-vous sûr de vouloir effacer la valeur de la caractéristique \"" + SelectedFeature.Name + "\" pour cet article ?", "Suppression caractéristique",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            continu = false;
                        }
                    }
                    if (continu)
                    {
                        if (SelectedFeature.Characteristic != null)
                        {
                            if (SelectedFeature.Characteristic.Pre_Id != null && Item.Local.Pre_Id != null)
                            {
                                uint IDFeatureValue = (SelectedFeature.Characteristic.Cha_Custom)
                                    ? SelectedFeature.PsFeatureValueLangCustom.IDFeatureValue
                                    : SelectedFeature.PsFeatureValueLang.IDFeatureValue;

                                // suppression lien dans PrestaShop
                                Model.Prestashop.PsFeatureProductRepository PsFeatureProductRepository = new Model.Prestashop.PsFeatureProductRepository();
                                if (PsFeatureProductRepository.ExistFeatureProduct(SelectedFeature.IDFeature, (uint)Item.Local.Pre_Id))
                                {
                                    Model.Prestashop.PsFeatureProduct PsFeatureProduct = PsFeatureProductRepository.ReadFeatureProduct(SelectedFeature.IDFeature, (uint)Item.Local.Pre_Id);
                                    if (PsFeatureProduct.IDFeatureValue == IDFeatureValue)
                                    {
                                        PsFeatureProductRepository.Delete(PsFeatureProduct);
                                    }
                                    else if (MessageBox.Show("Attention la valeur de caractéristique assignée au produit dans PrestaShop est différente de celle actuellement enregistrée dans PrestaConnect."
                                        + "\nSouhaitez-vous continuer la procédure d'effacement ?", "Caractéristique non synchronisée", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                                    {
                                        PsFeatureProductRepository.Delete(PsFeatureProduct);
                                    }
                                    else
                                    {
                                        continu = false;
                                    }
                                }

                                // suppression featurevalue si valeur personnalisée
                                if (continu && SelectedFeature.Characteristic.Cha_Custom)
                                {
                                    Model.Prestashop.PsFeatureValueLangRepository PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                                    if (PsFeatureValueLangRepository.ExistFeatureValue(SelectedFeature.PsFeatureValueLangCustom.IDFeatureValue))
                                    {
                                        PsFeatureValueLangRepository.DeleteAll(PsFeatureValueLangRepository.ListFeatureValue(SelectedFeature.PsFeatureValueLangCustom.IDFeatureValue));
                                    }

                                    Model.Prestashop.PsFeatureValueRepository PsFeatureValueRepository = new Model.Prestashop.PsFeatureValueRepository();
                                    if (PsFeatureValueRepository.ExistFeatureValue(SelectedFeature.PsFeatureValueLangCustom.IDFeatureValue))
                                        PsFeatureValueRepository.Delete(PsFeatureValueRepository.ReadFeatureValue(SelectedFeature.PsFeatureValueLangCustom.IDFeatureValue));
                                }
                            }

                            // suppression bdd locale
                            if (continu)
                            {
                                Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                                CharacteristicRepository.Delete(CharacteristicRepository.ReadFeatureArticle((int)SelectedFeature.IDFeature, Item.Id));
                            }
                        }

                        SelectedFeature.PsFeatureValueLang = null;
                        SelectedFeature.PsFeatureValueLangCustom = new Model.Prestashop.PsFeatureValueLang();
                        SelectedFeature.HasUpdated = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void FilterFeatureValue()
        {
            if (SelectedFeature != null)
            {
                SelectedFeature.FilterFeatureValue();
            }
        }

        #region CompositionArticleAttributeGroup

        public void LoadCompositionArticleAttributeGroup()
        {
            ListPsAttributeGroupLang = new ObservableCollection<Model.Prestashop.PsAttributeGroupLang>(new Model.Prestashop.PsAttributeGroupLangRepository().List(Core.Global.Lang));
            List<int> ListCompositionArticleAttributeGroup = this.CompositionArticleAttributeGroupRepository.ListArticle(Item.Local.Art_Id).Select(cag => cag.Cag_AttributeGroup_PreId).ToList();
            ListAttachPsAttributeGroupLang = new ObservableCollection<Model.Prestashop.PsAttributeGroupLang>(ListPsAttributeGroupLang.Where(ag => ListCompositionArticleAttributeGroup.Contains((int)ag.IDAttributeGroup)));
        }

        public void AddCompositionArticleAttributeGroup()
        {
            if (SelectedFreePsAttributeGroupLang != null
                && SelectedFreePsAttributeGroupLang.IDAttributeGroup != 0
                && ListFreePsAttributeGroupLang.Contains(SelectedFreePsAttributeGroupLang))
            {
                ListAttachPsAttributeGroupLang.Add(SelectedFreePsAttributeGroupLang);
                OnPropertyChanged("ListFreePsAttributeGroupLang");
                Item.HasUpdatedCompositionArticleAttributeGroup = true;
            }
        }
        public void RemoveCompositionArticleAttributeGroup()
        {
            if (SelectedAttachPsAttributeGroupLang != null
                && SelectedAttachPsAttributeGroupLang.IDAttributeGroup != 0
                && ListAttachPsAttributeGroupLang.Contains(SelectedAttachPsAttributeGroupLang))
            {
                ListAttachPsAttributeGroupLang.Remove(SelectedAttachPsAttributeGroupLang);
                OnPropertyChanged("ListFreePsAttributeGroupLang");
                Item.HasUpdatedCompositionArticleAttributeGroup = true;
            }
        }

        public void SaveCompositionArticleAttributeGroup()
        {
            List<Model.Local.CompositionArticleAttributeGroup> List = this.CompositionArticleAttributeGroupRepository.ListArticle(Item.Local.Art_Id);
            List<int> list_inbase = List.Select(cag => cag.Cag_AttributeGroup_PreId).ToList();
            List<uint> current_selection = ListAttachPsAttributeGroupLang.Select(ag => ag.IDAttributeGroup).ToList();
            foreach (Model.Local.CompositionArticleAttributeGroup item in List.Where(cag => !current_selection.Contains((uint)cag.Cag_AttributeGroup_PreId)))
            {
                this.CompositionArticleAttributeGroupRepository.Delete(item);
            }
            foreach (Model.Prestashop.PsAttributeGroupLang item in ListAttachPsAttributeGroupLang.Where(ag => !list_inbase.Contains((int)ag.IDAttributeGroup)))
            {
                this.CompositionArticleAttributeGroupRepository.Add(new Model.Local.CompositionArticleAttributeGroup()
                {
                    Cag_ArtId = Item.Local.Art_Id,
                    Cag_AttributeGroup_PreId = (int)item.IDAttributeGroup,
                });
            }
        }

        public void AddPsAttributeGroup()
        {
            if (!string.IsNullOrWhiteSpace(NewAttributeGroupName))
            {
                Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Model.Prestashop.PsAttributeGroupLangRepository();
                if (PsAttributeGroupLangRepository.ExistNameLang(NewAttributeGroupName, Core.Global.Lang))
                {
                    MessageBox.Show("Un groupe d'attributs avec le nom \"" + NewAttributeGroupName + "\" existe déjà !", "Groupe d'attributs",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (Item.HasUpdatedCompositionArticleAttributeGroup)
                    {
                        if (MessageBox.Show("Enregistrer les modifications sur les groupes d'attributs affectés à la composition ?", "Composition", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            this.SaveCompositionArticleAttributeGroup();
                        }
                    }
                    this.Item.HasUpdatedCompositionArticleAttributeGroup = false;

                    Model.Prestashop.PsAttributeGroupRepository PsAttributeGroupRepository = new Model.Prestashop.PsAttributeGroupRepository();

                    Model.Prestashop.PsAttributeGroup PsAttributeGroup;
                    Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang;

                    PsAttributeGroup = new Model.Prestashop.PsAttributeGroup()
                    {
                        #if (PRESTASHOP_VERSION_172)
						GroupType = "select",
						#endif
                        IsColorGroup = 0,
                        Position = PsAttributeGroupRepository.NextPosition(),
                    };
                    PsAttributeGroupRepository.Add(PsAttributeGroup, Core.Global.CurrentShop.IDShop);

                    foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                        if (!PsAttributeGroupLangRepository.ExistAttributeGroupLang(PsAttributeGroup.IDAttributeGroup, PsLang.IDLang))
                        {
                            PsAttributeGroupLang = new Model.Prestashop.PsAttributeGroupLang();
                            PsAttributeGroupLang.IDAttributeGroup = PsAttributeGroup.IDAttributeGroup;
                            PsAttributeGroupLang.IDLang = PsLang.IDLang;
                            PsAttributeGroupLang.Name = NewAttributeGroupName;
                            PsAttributeGroupLang.PublicName = NewAttributeGroupName;
                            PsAttributeGroupLangRepository.Add(PsAttributeGroupLang);
                        }
                    LoadCompositionArticleAttributeGroup();
                    NewAttributeGroupName = string.Empty;
                }
            }
        }

        #endregion

        #region CompositionArticle

        public void LoadCompositionArticle()
        {
            ListCompositionArticle = new ObservableCollection<Model.Local.CompositionArticle>(this.CompositionArticleRepository.ListArticle(this.Item.Local.Art_Id));
            foreach (Model.Local.CompositionArticle CompositionArticle in ListCompositionArticle)
            {
                CompositionArticle.ListPsAttributeGroupLang = new List<Model.Prestashop.PsAttributeGroupLang>();
                Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Model.Prestashop.PsAttributeGroupLangRepository();
                foreach (Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang in listAttachPsAttributeGroupLang)
                {
                    Model.Prestashop.PsAttributeGroupLang attachedPsAttributeGroupLang = PsAttributeGroupLangRepository.ReadAttributeGroupLang(PsAttributeGroupLang.IDAttributeGroup, PsAttributeGroupLang.IDLang);
                    attachedPsAttributeGroupLang.IDArticle = CompositionArticle.ComArt_ArtId;
                    attachedPsAttributeGroupLang.IDCompositionArticle = CompositionArticle.ComArt_Id;
                    CompositionArticle.ListPsAttributeGroupLang.Add(attachedPsAttributeGroupLang);
                }
            }
            this.Item.HasUpdatedCompositionArticle = false;

            ListArticleImageComposition = new ObservableCollection<Model.Local.ArticleImage>(new Model.Local.ArticleImageRepository().ListArticle(this.Item.Local.Art_Id));
            // affiche les images rattachées à la totalité des déclinaisons
            foreach (Model.Local.ArticleImage item in ListArticleImageComposition)
            {
                item.AttachedToCompositionArticle = ListCompositionArticle.Count == ListCompositionArticle.Count(a => a.ListImage.Contains(item.ImaArt_Id));
            }
        }

        public void SearchArticleComposition()
        {
            if (SelectedF_TAXE == null)
            {
                MessageBox.Show("Veuillez sélectionner une taxe !", "Recherche article", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                List<Model.Sage.F_ARTICLE_Composition> temp = new Model.Sage.F_ARTICLERepository().ListComposition();

                // Filtres sommeil et publié
                bool sommeil = Core.Global.GetConfig().ArticleEnSommeil;
                bool nonpublie = Core.Global.GetConfig().ArticleNonPublieSurLeWeb;
                // <JG> 19/12/2016 ajout filtre gammes
                temp = temp.Where(ar => (sommeil || ar.AR_Sommeil == 0) && (nonpublie || ar.AR_Publie == 1) && (sommeil || ar.AE_Sommeil != 1)).ToList();

                // FILTRE TAXE OBLIGATOIRE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!                
                #region Taxes
                switch (Core.Global.GetConfig().TaxSageTVA)
                {
                    case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                        temp = temp.Where(a => a.TA_Code1 == SelectedF_TAXE.TA_Code || (string.IsNullOrEmpty(a.TA_Code1) && a.TA_CodeFamille1 == SelectedF_TAXE.TA_Code)).ToList();
                        break;
                    case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                        temp = temp.Where(a => a.TA_Code2 == SelectedF_TAXE.TA_Code || (string.IsNullOrEmpty(a.TA_Code2) && a.TA_CodeFamille2 == SelectedF_TAXE.TA_Code)).ToList();
                        break;
                    case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                        temp = temp.Where(a => a.TA_Code3 == SelectedF_TAXE.TA_Code || (string.IsNullOrEmpty(a.TA_Code3) && a.TA_CodeFamille3 == SelectedF_TAXE.TA_Code)).ToList();
                        break;
                    case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                    default:
                        temp = new List<Model.Sage.F_ARTICLE_Composition>();
                        break;
                }
                #endregion

                if (temp.Count == 0)
                {
                    MessageBox.Show("Aucun résultat pour la taxe sélectionnée !", "Recherche article", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // filtres
                    if (SelectedF_FAMILLE != null)
                        temp = temp.Where(ar => ar.FA_CodeFamille == SelectedF_FAMILLE.FA_CodeFamille).ToList();
                    if (SelectedP_GAMME1 != null)
                        temp = temp.Where(ar => ar.AR_Gamme1 == SelectedP_GAMME1.cbMarq).ToList();
                    if (SelectedP_GAMME2 != null)
                        temp = temp.Where(ar => ar.AR_Gamme2 == SelectedP_GAMME1.cbMarq).ToList();
                    if (SelectedP_CONDITIONNEMENT != null)
                        temp = temp.Where(ar => ar.AR_Condition == SelectedP_CONDITIONNEMENT.cbMarq).ToList();
                    if (!string.IsNullOrWhiteSpace(FilterReferenceValue))
                    {
                        if (FilterReferenceContains)
                            temp = temp.Where(ar => ar.AR_Ref.Contains(FilterReferenceValue)).ToList();
                        if (FilterReferenceStartWith)
                            temp = temp.Where(ar => ar.AR_Ref.StartsWith(FilterReferenceValue)).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(FilterDesignationValue))
                    {
                        if (FilterDesignationContains)
                            temp = temp.Where(ar => ar.AR_Design.ToLower().Contains(FilterDesignationValue.ToLower())).ToList();
                        if (FilterDesignationStartWith)
                            temp = temp.Where(ar => ar.AR_Design.ToLower().StartsWith(FilterDesignationValue.ToLower())).ToList();
                    }

                    List<Int32> ListF_ARTICLE_SagId = null;
                    List<Int32> ListF_ARTENUMREF_SagId = null;
                    List<Int32> ListF_CONDITION_SagId = null;
                    if (FilterCompositionCurrent)
                    {
                        ListF_ARTICLE_SagId = this.CompositionArticleRepository.ListSageF_ARTICLE(Item.Local.Art_Id);
                        ListF_ARTENUMREF_SagId = this.CompositionArticleRepository.ListSageF_ARTENUMREF(Item.Local.Art_Id);
                        ListF_CONDITION_SagId = this.CompositionArticleRepository.ListSageF_CONDITION(Item.Local.Art_Id);
                    }
                    else if (FilterCompositionAll)
                    {
                        ListF_ARTICLE_SagId = this.CompositionArticleRepository.ListSageF_ARTICLE();
                        ListF_ARTENUMREF_SagId = this.CompositionArticleRepository.ListSageF_ARTENUMREF();
                        ListF_CONDITION_SagId = this.CompositionArticleRepository.ListSageF_CONDITION();
                    }

                    // <JG> 21/12/2016 remaniement filtres articles existants
                    if (FilterArticle)
                    {
                        Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                        if (ListF_ARTICLE_SagId != null)
                            ListF_ARTICLE_SagId.AddRange(ArticleRepository.ListSageId());
                        else
                            ListF_ARTICLE_SagId = ArticleRepository.ListSageId();
                    }

                    if (ListF_ARTICLE_SagId != null)
                        temp = temp.Where(ar => !ListF_ARTICLE_SagId.Contains(ar.cbMarq)).ToList();
                    if (ListF_ARTENUMREF_SagId != null)
                        temp = temp.Where(ar => ar.F_ARTENUMREF_SagId == null || (ar.F_ARTENUMREF_SagId != null && !ListF_ARTENUMREF_SagId.Contains(ar.F_ARTENUMREF_SagId.Value))).ToList();
                    if (ListF_CONDITION_SagId != null)
                        temp = temp.Where(ar => ar.F_CONDITION_SagId == null || (ar.F_CONDITION_SagId != null && !ListF_CONDITION_SagId.Contains(ar.F_CONDITION_SagId.Value))).ToList();

                    // filtres exclusion
                    int count = temp.Count;
                    List<Model.Sage.F_ARTICLE_Composition> results = Core.Tools.FiltreImportSage.ImportSageFilter(temp);
                    StrCompositionFilteredProducts = (results.Count < count) ? ((count - results.Count) + " résultats dans les filtres d'exclusion") : string.Empty;
                    if (!ShowResultInImportSageFilter)
                        temp = results;

                    ListResultSearchCompositionArticle = new ObservableCollection<Model.Sage.F_ARTICLE_Composition>(temp);
                }
            }
        }
        public void InitSearchArticleComposition()
        {
            SelectedF_TAXE = null;
            SelectedF_FAMILLE = null;
            SelectedP_GAMME1 = null;
            SelectedP_GAMME2 = null;
            SelectedP_CONDITIONNEMENT = null;
            FilterReferenceValue = string.Empty;
            FilterReferenceContains = true;
            FilterDesignationValue = string.Empty;
            FilterDesignationContains = true;
            FilterCompositionAll = true;
            FilterArticle = true;
        }
        public void CheckAddAllArticleComposition()
        {
            if (ListResultSearchCompositionArticle != null)
            {
                int addCount = ListResultSearchCompositionArticle.Count(result => result.AR_IsCheckedToAdd);
                bool toAdd = ((ListResultSearchCompositionArticle.Count - addCount) >= addCount);
                System.Threading.Tasks.Parallel.ForEach(ListResultSearchCompositionArticle, a => a.AR_IsCheckedToAdd = toAdd);
            }
        }
        public void AddCompositionArticle()
        {
            if (ListResultSearchCompositionArticle.Count(r => r.AR_IsCheckedToAdd) > 0)
            {
                List<Model.Sage.F_ARTICLE_Composition> list_checkedtoadd = ListResultSearchCompositionArticle.Where(r => r.AR_IsCheckedToAdd).ToList();

                List<int> f_artenumref_imported = ListCompositionArticle.Where(comp => comp.ComArt_F_ARTENUMREF_SagId != null).Select(comp => comp.ComArt_F_ARTENUMREF_SagId.Value).ToList();
                List<int> f_condition_imported = ListCompositionArticle.Where(comp => comp.ComArt_F_CONDITION_SagId != null).Select(comp => comp.ComArt_F_CONDITION_SagId.Value).ToList();
                List<int> f_article_imported = ListCompositionArticle.Where(comp => comp.ComArt_F_ARTENUMREF_SagId == null && comp.ComArt_F_CONDITION_SagId == null).Select(comp => comp.ComArt_F_ARTICLE_SagId).ToList();

                int count_exist = list_checkedtoadd.Count(rs => f_article_imported.Contains(rs.cbMarq)
                    || (rs.F_ARTENUMREF_SagId != null && f_artenumref_imported.Contains(rs.F_ARTENUMREF_SagId.Value))
                    || (rs.F_CONDITION_SagId != null && f_condition_imported.Contains(rs.F_CONDITION_SagId.Value)));

                if (count_exist > 0 && MessageBox.Show("Il y a " + count_exist + " résultats qui font déjà partis de la composition !\n"
                    + "Souhaitez-vous les ajouter tout de même ?", "Composition", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    list_checkedtoadd = list_checkedtoadd.Where(rs =>
                        (!f_article_imported.Contains(rs.cbMarq) && rs.F_ARTENUMREF_SagId == null && rs.F_CONDITION_SagId == null)
                        || (rs.F_ARTENUMREF_SagId != null && rs.F_CONDITION_SagId == null && !f_artenumref_imported.Contains(rs.F_ARTENUMREF_SagId.Value))
                        || (rs.F_ARTENUMREF_SagId == null && rs.F_CONDITION_SagId != null && !f_condition_imported.Contains(rs.F_CONDITION_SagId.Value))).ToList();
                }

                bool define_default = ListCompositionArticle.Count(ca => ca.ComArt_Default) == 0;
                foreach (Model.Sage.F_ARTICLE_Composition F_ARTICLE_Composition in list_checkedtoadd)
                {
                    this.CompositionArticleRepository.Add(new Model.Local.CompositionArticle()
                    {
                        ComArt_Active = false,
                        ComArt_ArtId = this.Item.Local.Art_Id,
                        ComArt_F_ARTICLE_SagId = F_ARTICLE_Composition.cbMarq,
                        ComArt_F_ARTENUMREF_SagId = F_ARTICLE_Composition.F_ARTENUMREF_SagId,
                        ComArt_F_CONDITION_SagId = F_ARTICLE_Composition.F_CONDITION_SagId,
                        ComArt_Quantity = 1,
                        ComArt_Sync = true,
                        ComArt_Default = define_default,
                        HasUpdated = false,
                    });
                    F_ARTICLE_Composition.AR_IsCheckedToAdd = false;
                    if (define_default)
                        define_default = false;

                    if (RedirectComposition)
                    {
                        Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                        if (ArticleRepository.ExistSag_Id(F_ARTICLE_Composition.cbMarq))
                        {
                            Model.Local.Article Composition = ArticleRepository.ReadSag_Id(F_ARTICLE_Composition.cbMarq);
                            Composition.Art_Active = false;
                            Composition.Art_RedirectType = new RedirectType(Core.Parametres.RedirectType.RedirectPermanently301).Page;
                            Composition.Art_RedirectProduct = this.Item.Local.Art_Id;
                            Composition.Art_Date = DateTime.Now;
                            ArticleRepository.Save();
                        }
                    }
                    UpdateDate();
                }
                LoadCompositionArticle();
                SearchArticleComposition();
            }
        }

        public void SaveCompositionArticle()
        {
            foreach (Model.Local.CompositionArticle CompositionArticle in ListCompositionArticle)
            {
                foreach (Model.Prestashop.PsAttributeGroupLang group in CompositionArticle.ListPsAttributeGroupLang)
                {
                    if (group.PsAttributeLang != null && group.PsAttributeLang.IDAttribute != 0)
                    {
                        if (CompositionArticle.CompositionArticleAttribute.Count(ag => ag.Caa_AttributeGroup_PreId == (int)group.IDAttributeGroup) == 0)
                        {
                            CompositionArticle.CompositionArticleAttribute.Add(new Model.Local.CompositionArticleAttribute()
                            {
                                Caa_ComArtId = CompositionArticle.ComArt_ArtId,
                                Caa_AttributeGroup_PreId = (int)group.IDAttributeGroup,
                                Caa_Attribute_PreId = (int)group.PsAttributeLang.IDAttribute,
                            });
                        }
                        else
                        {
                            Model.Local.CompositionArticleAttribute CompositionArticleAttribute = CompositionArticle.CompositionArticleAttribute.First(ag => ag.Caa_AttributeGroup_PreId == (int)group.IDAttributeGroup);
                            CompositionArticleAttribute.Caa_Attribute_PreId = (int)group.PsAttributeLang.IDAttribute;
                        }
                    }
                    else if (CompositionArticle.CompositionArticleAttribute.Count(ag => ag.Caa_AttributeGroup_PreId == (int)group.IDAttributeGroup) > 0)
                    {
                        CompositionArticle.CompositionArticleAttribute.Remove(CompositionArticle.CompositionArticleAttribute.First(ag => ag.Caa_AttributeGroup_PreId == (int)group.IDAttributeGroup));
                    }
                }
                List<int> current_attach = CompositionArticle.CompositionArticleAttribute.Select(caa => caa.Caa_AttributeGroup_PreId).ToList();
                foreach (int attributegroup in current_attach)
                {
                    if (CompositionArticle.ListPsAttributeGroupLang.Count(ag => ag.IDAttributeGroup == attributegroup) == 0)
                        CompositionArticleRepository.DeleteLinkCompositionArticleAttribute(CompositionArticle.ComArt_ArtId, attributegroup);
                }

                foreach (int itemadd in CompositionArticle.ListImage)
                {
                    if (CompositionArticle.CompositionArticleImage.Count(ati => ati.ImaArt_Id == itemadd) == 0)
                        CompositionArticle.CompositionArticleImage.Add(new Model.Local.CompositionArticleImage()
                        {
                            ComArt_Id = CompositionArticle.ComArt_Id,
                            ImaArt_Id = itemadd,
                        });
                }
                List<int> current_attach_image = CompositionArticle.CompositionArticleImage.Select(ati => ati.ImaArt_Id).ToList();
                foreach (int itemremove in current_attach_image)
                {
                    if (!CompositionArticle.ListImage.Contains(itemremove))
                        CompositionArticleRepository.DeleteLinkCompositionArticleImage(CompositionArticle.ComArt_Id, itemremove);
                    //AttributeArticle.AttributeArticleImage.Remove(AttributeArticle.AttributeArticleImage.First(ati => ati.ImaArt_Id == itemremove));
                }

                CompositionArticle.HasUpdated = false;
            }
            this.CompositionArticleRepository.Save();
        }
        public void DeleteCompositionArticle()
        {
            if (SelectedCompositionArticle != null && SelectedCompositionArticle.ComArt_Id != 0)
            {
                if (ListCompositionArticle.Count(a => a.Updated) > 0)
                {
                    MessageBox.Show("Des modifications sur les compositions n'ont pas été enregistrées, impossible d'effectuer une suppression !", "Suppression composition", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    bool defaut = (SelectedCompositionArticle.ComArt_Default && ListCompositionArticle.Count > 1);
                    if (SelectedCompositionArticle.Pre_Id == null)
                    {
                        if (MessageBox.Show("Validez la suppression de la composition "
                            + (defaut ? "par défaut" : string.Empty) + " ?", "Suppression composition",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            if (defaut)
                            {
                                Model.Local.CompositionArticle CompositionArticleNext = ListCompositionArticle.First(ca => ca.ComArt_Id != SelectedCompositionArticle.ComArt_Id);
                                CompositionArticleNext.ComArt_Default = true;
                                this.CompositionArticleRepository.Save();
                            }
                            this.CompositionArticleRepository.Delete(SelectedCompositionArticle);
                            LoadCompositionArticle();
                        }
                    }
                    else if (MessageBox.Show("Composition liée à une déclinaison PrestaShop !"
                            + "\n\n" + "Validez la suppression de la composition "
                            + (defaut ? "par défaut" : string.Empty) + " ?", "Suppression composition",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (defaut)
                        {
                            Model.Local.CompositionArticle CompositionArticleNext = ListCompositionArticle.First(ca => ca.ComArt_Id != SelectedCompositionArticle.ComArt_Id);
                            CompositionArticleNext.ComArt_Default = true;
                            this.CompositionArticleRepository.Save();
                        }
                        if (SelectedCompositionArticle.Pre_Id != null)
                            DeletePsProductAttribute(SelectedCompositionArticle.Pre_Id.Value);
                        this.CompositionArticleRepository.Delete(SelectedCompositionArticle);
                        LoadCompositionArticle();
                    }
                }
            }
        }
        public void DeleteAllCompositionArticle()
        {
            if (ListCompositionArticle != null && ListCompositionArticle.Count > 0)
            {
                if (ListCompositionArticle.Count(a => a.Updated) > 0)
                {
                    MessageBox.Show("Des modifications sur les compositions n'ont pas été enregistrées, impossible d'effectuer une suppression !", "Suppression composition", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (ListCompositionArticle.Count(ca => ca.Pre_Id != null) == 0)
                {
                    if (MessageBox.Show("Validez la suppression des compositions ?", "Suppression compositions",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        this.CompositionArticleRepository.DeleteAll(ListCompositionArticle.ToList());
                        LoadCompositionArticle();
                    }
                }
                else if (MessageBox.Show("Compositions liées à des déclinaisons PrestaShop !" + "\n\n" + "Validez la suppression ?", "Suppression compositions",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (Model.Local.CompositionArticle CompositionArticle in ListCompositionArticle)
                        if (CompositionArticle.Pre_Id != null)
                            DeletePsProductAttribute(CompositionArticle.Pre_Id.Value);
                    this.CompositionArticleRepository.DeleteAll(ListCompositionArticle.ToList());
                    LoadCompositionArticle();
                }
            }
        }
        private void DeletePsProductAttribute(int idproductattribute)
        {
            try
            {
                Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
                if (PsProductAttributeRepository.ExistProductAttribute((uint)idproductattribute))
                {
                    PsProductAttributeRepository.Delete(PsProductAttributeRepository.ReadProductAttribute((uint)idproductattribute));
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        public void FilterAttributeValue()
        {
            if (SelectedCompositionArticle != null && SelectedCompositionArticle.SelectedPsAttributeGroupLang != null)
            {
                SelectedCompositionArticle.SelectedPsAttributeGroupLang.FilterAttributeValue();
            }
        }

        public void AddPsAttribute()
        {
            if (SelectedCompositionArticle != null && SelectedCompositionArticle.SelectedPsAttributeGroupLang != null && !string.IsNullOrWhiteSpace(NewAttributeValue))
            {
                Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();
                if (PsAttributeLangRepository.ExistAttributeLang(NewAttributeValue, Core.Global.Lang, SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup))
                {
                    MessageBox.Show("La valeur d'attribut \"" + NewAttributeValue + "\" pour le groupe \"" + SelectedCompositionArticle.SelectedPsAttributeGroupLang.Name + "\" existe déjà !", "Attribut",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (Item.HasUpdatedCompositionArticle || ListCompositionArticle.Count(ca => ca.Updated) > 0)
                    {
                        if (MessageBox.Show("Enregistrer les modifications sur les compositions ?", "Compositions", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            this.SaveCompositionArticle();
                        }
                    }
                    this.Item.HasUpdatedCompositionArticle = false;

                    Model.Prestashop.PsAttributeRepository PsAttributeRepository = new Model.Prestashop.PsAttributeRepository();

                    Model.Prestashop.PsAttribute PsAttribute = new Model.Prestashop.PsAttribute()
                    {
                        IDAttributeGroup = SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup,
                        Color = string.Empty,
                        Position = PsAttributeRepository.NextPosition(),
                    };
                    PsAttributeRepository.Add(PsAttribute, Core.Global.CurrentShop.IDShop);

                    foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                        if (!PsAttributeLangRepository.ExistAttributeLang(PsAttribute.IDAttribute, PsLang.IDLang))
                        {
                            PsAttributeLangRepository.Add(new Model.Prestashop.PsAttributeLang()
                                {
                                    IDAttribute = PsAttribute.IDAttribute,
                                    IDLang = PsLang.IDLang,
                                    Name = NewAttributeValue,
                                });
                        }
                    int selectedcomposition = (SelectedCompositionArticle != null) ? SelectedCompositionArticle.ComArt_Id : 0;
                    uint selectedpsattributegroup = (SelectedCompositionArticle != null && SelectedCompositionArticle.SelectedPsAttributeGroupLang != null) ? SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup : 0;
                    LoadCompositionArticle();
                    if (selectedcomposition != 0)
                    {
                        SelectedCompositionArticle = ListCompositionArticle.FirstOrDefault(ca => ca.ComArt_Id == selectedcomposition);
                        if (selectedpsattributegroup != 0)
                            SelectedCompositionArticle.SelectedPsAttributeGroupLang = SelectedCompositionArticle.ListPsAttributeGroupLang.FirstOrDefault(ag => ag.IDAttributeGroup == selectedpsattributegroup);
                    }

                    NewAttributeValue = string.Empty;
                }
            }
        }

        public void AttachImageToCompositionArticle()
        {
            if (SelectedArticleImageComposition != null)
            {
                if (SelectedCompositionArticle == null || SelectedCompositionArticle.ComArt_Id == 0)
                {
                    if (SelectedArticleImageComposition.AttachedToCompositionArticle)
                    {
                        //if (MessageBox.Show("Voulez-vous attribuer cette image à la totalité des déclinaisons ?",
                        //        "Affectation image", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //System.Threading.Tasks.Parallel.ForEach(ListAttributeArticle, a => AttachImageToAttributeArticle(a));
                            MessageBox.Show("L'image va être attribuée à la totalité des déclinaisons !", "Affectation image", MessageBoxButton.OK, MessageBoxImage.Information);
                            foreach (Model.Local.CompositionArticle CompositionArticle in ListCompositionArticle)
                            {
                                AttachImageToCompositionArticle(CompositionArticle);
                            }
                        }
                        //else
                        //{
                        //    SelectedArticleImageGamme.AttachedToAttributeArticle = !SelectedArticleImageGamme.AttachedToAttributeArticle;
                        //}
                    }
                    else if (!SelectedArticleImageComposition.AttachedToCompositionArticle)
                    {
                        //if (MessageBox.Show("Voulez-vous détacher cette image de la totalité des déclinaisons ?",
                        //        "Détachement image", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            MessageBox.Show("L'image va être détachée de la totalité des déclinaisons !", "Détachement image", MessageBoxButton.OK, MessageBoxImage.Information);
                            foreach (Model.Local.CompositionArticle CompositionArticle in ListCompositionArticle)
                            {
                                AttachImageToCompositionArticle(CompositionArticle);
                            }
                        }
                        //else
                        //{
                        //    SelectedArticleImageGamme.AttachedToAttributeArticle = !SelectedArticleImageGamme.AttachedToAttributeArticle;
                        //}
                    }
                }
                else
                {
                    AttachImageToCompositionArticle(SelectedCompositionArticle);
                }
            }
        }
        private void AttachImageToCompositionArticle(Model.Local.CompositionArticle CompositionArticle)
        {
            if (SelectedArticleImageComposition.AttachedToCompositionArticle)
            {
                if (!CompositionArticle.ListImage.Contains(SelectedArticleImageComposition.ImaArt_Id))
                {
                    CompositionArticle.ListImage.Add(SelectedArticleImageComposition.ImaArt_Id);
                    CompositionArticle.HasUpdated = true;
                }
            }
            else if (CompositionArticle.ListImage.Contains(SelectedArticleImageComposition.ImaArt_Id))
            {
                CompositionArticle.ListImage.Remove(SelectedArticleImageComposition.ImaArt_Id);
                CompositionArticle.HasUpdated = true;
            }
        }

        public void AutoSelectComposition()
        {
            if (!string.IsNullOrWhiteSpace(FastSelectionReferenceComposition)
                || !string.IsNullOrWhiteSpace(FastSelectionDesignationComposition)
                || !string.IsNullOrWhiteSpace(FastSelectionAttributeComposition))
            {
                if (CompositionSelectionMode == Microsoft.Windows.Controls.DataGridSelectionMode.Extended)
                {
                    foreach (Model.Local.CompositionArticle compo in this.ListCompositionArticle)
                    {
                        compo.IsSelected = (string.IsNullOrEmpty(FastSelectionReferenceComposition) || compo.F_ARTICLE_Composition.AR_Ref.Contains(FastSelectionReferenceComposition))
                            && (string.IsNullOrEmpty(FastSelectionDesignationComposition) || compo.F_ARTICLE_Composition.AR_Design.ToLower().Contains(FastSelectionDesignationComposition))
                            && (string.IsNullOrEmpty(FastSelectionAttributeComposition) || (compo.F_ARTICLE_Composition.Gamme.ToLower().Contains(FastSelectionAttributeComposition)
                                || compo.F_ARTICLE_Composition.Conditionnement.ToLower().Contains(FastSelectionAttributeComposition)));
                    }
                }
                else
                {
                    SelectedCompositionArticle = this.ListCompositionArticle.FirstOrDefault(ca => (string.IsNullOrEmpty(FastSelectionReferenceComposition) || ca.F_ARTICLE_Composition.AR_Ref.Contains(FastSelectionReferenceComposition))
                            && (string.IsNullOrEmpty(FastSelectionDesignationComposition) || ca.F_ARTICLE_Composition.AR_Design.ToLower().Contains(FastSelectionDesignationComposition))
                            && (string.IsNullOrEmpty(FastSelectionAttributeComposition) || (ca.F_ARTICLE_Composition.Gamme.ToLower().Contains(FastSelectionAttributeComposition)
                                || ca.F_ARTICLE_Composition.Conditionnement.ToLower().Contains(FastSelectionAttributeComposition))));
                }
            }
            else
            {
                SelectedCompositionArticle = null;
            }
        }
        #endregion

        public void CreateAttributeByGamme1()
        {
            if (SelectedCompositionArticle != null && SelectedCompositionArticle.SelectedPsAttributeGroupLang != null)
            {
                if (MessageBox.Show("Vous allez lancer la création et l'affectation des attributs pour le groupe \"" + SelectedCompositionArticle.SelectedPsAttributeGroupLang.Name
                    + "\" à partir des énumérés de gamme 1 !"
                    + "\n\nCette opération sera enregistrée automatiquement, continuer ?",
                    "Création et affectation attributs", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (Model.Local.CompositionArticle CompositionArticle in ListCompositionArticle)
                    {
                        if (!string.IsNullOrWhiteSpace(CompositionArticle.EnumereGamme1.EG_Enumere))
                        {
                            Model.Prestashop.PsAttributeLang PsAttributeLang = null;
                            Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();

                            if (PsAttributeLangRepository.ExistAttributeLang(CompositionArticle.EnumereGamme1.EG_Enumere.Trim(), Core.Global.Lang, SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup))
                            {
                                PsAttributeLang = PsAttributeLangRepository.ReadAttributeLang(CompositionArticle.EnumereGamme1.EG_Enumere.Trim(), Core.Global.Lang, SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup);
                            }
                            else
                            {
                                Model.Prestashop.PsAttributeRepository PsAttributeRepository = new Model.Prestashop.PsAttributeRepository();
                                Model.Prestashop.PsAttribute PsAttribute = new Model.Prestashop.PsAttribute()
                                {
                                    IDAttributeGroup = SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup,
                                    Color = string.Empty,
                                    Position = PsAttributeRepository.NextPosition(),
                                };
                                PsAttributeRepository.Add(PsAttribute, Core.Global.CurrentShop.IDShop);

                                foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                                    if (!PsAttributeLangRepository.ExistAttributeLang(PsAttribute.IDAttribute, PsLang.IDLang))
                                    {
                                        PsAttributeLangRepository.Add(new Model.Prestashop.PsAttributeLang()
                                        {
                                            IDAttribute = PsAttribute.IDAttribute,
                                            IDLang = PsLang.IDLang,
                                            Name = CompositionArticle.EnumereGamme1.EG_Enumere.Trim(),
                                        });
                                    }

                                PsAttributeLang = PsAttributeLangRepository.ReadAttributeLang(PsAttribute.IDAttribute, Core.Global.Lang);
                            }

                            if (PsAttributeLang != null)
                            {
                                // affectation composition
                                if (CompositionArticle.CompositionArticleAttribute.Count(ag => ag.Caa_AttributeGroup_PreId == (int)SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup) == 0)
                                {
                                    CompositionArticle.CompositionArticleAttribute.Add(new Model.Local.CompositionArticleAttribute()
                                    {
                                        Caa_ComArtId = CompositionArticle.ComArt_ArtId,
                                        Caa_AttributeGroup_PreId = (int)SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup,
                                        Caa_Attribute_PreId = (int)PsAttributeLang.IDAttribute,
                                    });
                                }
                                else
                                {
                                    Model.Local.CompositionArticleAttribute CompositionArticleAttribute = CompositionArticle.CompositionArticleAttribute.First(ag => ag.Caa_AttributeGroup_PreId == (int)SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup);
                                    CompositionArticleAttribute.Caa_Attribute_PreId = (int)PsAttributeLang.IDAttribute;
                                }
                                CompositionArticle.ListPsAttributeGroupLang.First(ag => ag.IDAttributeGroup == SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup).HasUpdated = true;
                                CompositionArticle.ListPsAttributeGroupLang.First(ag => ag.IDAttributeGroup == SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup).PsAttributeLang = PsAttributeLang;
                                CompositionArticle.ReloadStringDeclinaison();
                                CompositionArticle.ListPsAttributeGroupLang.First(ag => ag.IDAttributeGroup == SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup).HasUpdated = false;
                            }
                        }
                    }
                    this.CompositionArticleRepository.Save();
                    SelectedCompositionArticle = null;

                    UpdateDate();
                    LoadCompositionArticle();
                }
            }
        }

        public void AttachImageToAttributeArticle()
        {
            if (SelectedArticleImageGamme != null)
            {
                if (SelectedAttributeArticle == null || SelectedAttributeArticle.AttArt_Id == 0)
                {
                    if (SelectedArticleImageGamme.AttachedToAttributeArticle)
                    {
                        //if (MessageBox.Show("Voulez-vous attribuer cette image à la totalité des déclinaisons ?",
                        //        "Affectation image", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //System.Threading.Tasks.Parallel.ForEach(ListAttributeArticle, a => AttachImageToAttributeArticle(a));
                            MessageBox.Show("L'image va être attribuée à la totalité des déclinaisons !", "Affectation image", MessageBoxButton.OK, MessageBoxImage.Information);
                            foreach (Model.Local.AttributeArticle AttributeArticle in ListAttributeArticle)
                            {
                                AttachImageToAttributeArticle(AttributeArticle);
                            }
                        }
                        //else
                        //{
                        //    SelectedArticleImageGamme.AttachedToAttributeArticle = !SelectedArticleImageGamme.AttachedToAttributeArticle;
                        //}
                    }
                    else if (!SelectedArticleImageGamme.AttachedToAttributeArticle)
                    {
                        //if (MessageBox.Show("Voulez-vous détacher cette image de la totalité des déclinaisons ?",
                        //        "Détachement image", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            MessageBox.Show("L'image va être détachée de la totalité des déclinaisons !", "Détachement image", MessageBoxButton.OK, MessageBoxImage.Information);
                            foreach (Model.Local.AttributeArticle AttributeArticle in ListAttributeArticle)
                            {
                                AttachImageToAttributeArticle(AttributeArticle);
                            }
                        }
                        //else
                        //{
                        //    SelectedArticleImageGamme.AttachedToAttributeArticle = !SelectedArticleImageGamme.AttachedToAttributeArticle;
                        //}
                    }
                }
                else
                {
                    AttachImageToAttributeArticle(SelectedAttributeArticle);
                }
            }
        }
        private void AttachImageToAttributeArticle(Model.Local.AttributeArticle AttributeArticle)
        {
            if (SelectedArticleImageGamme.AttachedToAttributeArticle)
            {
                if (!AttributeArticle.ListImage.Contains(SelectedArticleImageGamme.ImaArt_Id))
                {
                    AttributeArticle.ListImage.Add(SelectedArticleImageGamme.ImaArt_Id);
                    AttributeArticle._HasUpdated = true;
                }
            }
            else if (AttributeArticle.ListImage.Contains(SelectedArticleImageGamme.ImaArt_Id))
            {
                AttributeArticle.ListImage.Remove(SelectedArticleImageGamme.ImaArt_Id);
                AttributeArticle._HasUpdated = true;
            }
        }

        #endregion

        public bool DeleteArticle()
        {
            bool deleted = false;
            if (Item.CanDelete)
            {
                if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer l'article " + Item.Name + ", Cette action sera irréversible ?",
                    "Suppression article", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                        Model.Local.Article Article = ArticleRepository.ReadArticle(Item.Local.Art_Id);

                        Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                        Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new Model.Local.AttributeArticleImageRepository();
                        foreach (Model.Local.AttributeArticle AttributeArticle in AttributeArticleRepository.ListArticle(Article.Art_Id))
                        {
                            AttributeArticleImageRepository.DeleteAll(AttributeArticleImageRepository.ListAttributeArticle(AttributeArticle.AttArt_Id));
                        }
                        AttributeArticleRepository.DeleteAll(AttributeArticleRepository.ListArticle(Article.Art_Id));

                        Model.Local.ConditioningArticleRepository ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
                        ConditioningArticleRepository.DeleteAll(ConditioningArticleRepository.ListArticle(Article.Art_Id));

                        Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                        CompositionArticleRepository.DeleteAll(CompositionArticleRepository.ListArticle(Article.Art_Id));

                        Model.Local.CompositionArticleAttributeGroupRepository CompositionArticleAttributeGroupRepository = new Model.Local.CompositionArticleAttributeGroupRepository();
                        CompositionArticleAttributeGroupRepository.DeleteAll(CompositionArticleAttributeGroupRepository.ListArticle(Article.Art_Id));

                        Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                        foreach (Model.Local.ArticleImage ArticleImage in ArticleImageRepository.ListArticle(Article.Art_Id))
                        {
                            ArticleImage.EraseFiles();
                        }
                        ArticleImageRepository.DeleteAll(ArticleImageRepository.ListArticle(Article.Art_Id));

                        Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                        foreach (Model.Local.Attachment Attachment in AttachmentRepository.ListArticle(Article.Art_Id))
                        {
                            Attachment.EraseFile();
                        }
                        AttachmentRepository.DeleteAll(AttachmentRepository.ListArticle(Article.Art_Id));

                        Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();
                        ArticleCatalogRepository.DeleteAll(ArticleCatalogRepository.ListArticle(Article.Art_Id));

                        Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                        CharacteristicRepository.DeleteAll(CharacteristicRepository.ListArticle(Article.Art_Id));

                        ArticleRepository.Delete(Article);

                        deleted = true;
                    }
                    catch (Exception ex)
                    {
                        Core.Error.SendMailError(ex.ToString());
                    }
                }
            }
            return deleted;
        }

        public bool TransformArticle()
        {
            bool t = false;
            if (Core.UpdateVersion.License.Option2)
            {
                if (Item.CanTransform
                    && MessageBox.Show("Vous allez transformer l'article \"" + Item.Name + "\" en article de composition !\nAttention cette action sera irréversible !\n\nContinuer ?", "Transformation article", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                    Model.Local.Article Article = ArticleRepository.ReadArticle(Item.Local.Art_Id);
                    Article.Sag_Id = 0;
                    Article.Art_Sync = false;
                    Article.Art_Type = (short)Model.Local.Article.enum_TypeArticle.ArticleComposition;
                    Article.Art_Date = DateTime.Now;
                    ArticleRepository.Save();
                    t = true;
                }
            }
            else
            {
                new PRESTACONNECT.View.PrestaMessage("Votre licence actuelle ne permet pas d'utiliser la composition d'article !", "Licence insuffisante", MessageBoxButton.OK, MessageBoxImage.Information).ShowDialog();
            }
            return t;
        }

        public void DeleteAttributeArticle()
        {
            if (SelectedAttributeArticle != null && SelectedAttributeArticle.AttArt_Id != 0)
            {
                if (ListAttributeArticle.Count(a => a._HasUpdated) > 0)
                {
                    MessageBox.Show("Des modifications sur les gammes n'ont pas été enregistrées, impossible d'effectuer une suppression !", "Suppression gamme", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    bool defaut = (SelectedAttributeArticle.AttArt_Default && ListAttributeArticle.Count > 1);
                    if (SelectedAttributeArticle.Pre_Id == null)
                    {
                        if (MessageBox.Show("Validez la suppression de la gamme "
                            + (defaut ? "par défaut" : string.Empty) + " ?", "Suppression gamme",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            if (defaut)
                            {
                                Model.Local.AttributeArticle AttributeArticleNext = ListAttributeArticle.First(ata => ata.AttArt_Id != SelectedAttributeArticle.AttArt_Id);
                                AttributeArticleNext.AttArt_Default = true;
                                this.AttributeArticleRepository.Save();
                            }
                            this.AttributeArticleRepository.Delete(SelectedAttributeArticle);
                            LoadAttributeArticle();
                        }
                    }
                    else if (MessageBox.Show("Gamme liée à une déclinaison PrestaShop !"
                            + "\n\n" + "Attention après la suppression si cette déclinaison est présente dans une commande elle ne pourra plus être écrite dans Sage !"
                            + "\n\n" + "Validez la suppression de la gamme "
                            + (defaut ? "par défaut" : string.Empty) + " ?", "Suppression gamme",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (defaut)
                        {
                            Model.Local.AttributeArticle AttributeArticleNext = ListAttributeArticle.First(ata => ata.AttArt_Id != SelectedAttributeArticle.AttArt_Id);
                            AttributeArticleNext.AttArt_Default = true;
                            this.AttributeArticleRepository.Save();
                        }
                        if (SelectedAttributeArticle.Pre_Id != null)
                            DeletePsProductAttribute(SelectedAttributeArticle.Pre_Id.Value);
                        this.AttributeArticleRepository.Delete(SelectedAttributeArticle);
                        LoadAttributeArticle();
                    }
                }
            }
        }
        public void DeleteAllAttributeArticle()
        {
            if (ListAttributeArticle != null && ListAttributeArticle.Count > 0)
            {
                if (ListAttributeArticle.Count(a => a._HasUpdated) > 0)
                {
                    MessageBox.Show("Des modifications sur les gammes n'ont pas été enregistrées, impossible d'effectuer une suppression !", "Suppression gamme", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (ListAttributeArticle.Count(ca => ca.Pre_Id != null) == 0)
                {
                    if (MessageBox.Show("Validez la suppression des gammes ?", "Suppression gammes",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        this.AttributeArticleRepository.DeleteAll(ListAttributeArticle.ToList());
                        LoadAttributeArticle();
                    }
                }
                else if (MessageBox.Show("Gammes liées à des déclinaisons PrestaShop !"
                    + "\n\n" + "Attention après la suppression si ces déclinaisons sont présentes dans une commande elles ne pourront plus être écrite dans Sage !"
                    + "\n\n" + "Validez la suppression ?", "Suppression gammes",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (Model.Local.AttributeArticle AttributeArticle in ListAttributeArticle)
                        if (AttributeArticle.Pre_Id != null)
                            DeletePsProductAttribute(AttributeArticle.Pre_Id.Value);
                    this.AttributeArticleRepository.DeleteAll(ListAttributeArticle.ToList());
                    LoadAttributeArticle();
                }
            }
        }
        public void DeleteConditioningArticle()
        {
            if (SelectedConditioningArticle != null && SelectedConditioningArticle.ConArt_Id != 0)
            {
                if (ListConditioningArticle.Count(a => a._HasUpdated) > 0)
                {
                    MessageBox.Show("Des modifications sur les conditionnements n'ont pas été enregistrées, impossible d'effectuer une suppression !", "Suppression conditionnement", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    bool defaut = (SelectedConditioningArticle.ConArt_Default && ListConditioningArticle.Count > 1);
                    if (SelectedConditioningArticle.Pre_Id == null)
                    {
                        if (MessageBox.Show("Validez la suppression du conditionnement "
                            + (defaut ? "par défaut" : string.Empty) + " ?", "Suppression conditionnement",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            if (defaut)
                            {
                                Model.Local.ConditioningArticle ConditioningArticleNext = ListConditioningArticle.First(conart => conart.ConArt_Id != SelectedConditioningArticle.ConArt_Id);
                                ConditioningArticleNext.ConArt_Default = true;
                                this.AttributeArticleRepository.Save();
                            }
                            this.ConditioningArticleRepository.Delete(SelectedConditioningArticle);
                            LoadConditioningArticle();
                        }
                    }
                    else if (MessageBox.Show("Conditionnement liée à une déclinaison PrestaShop !"
                            + "\n\n" + "Attention après la suppression si cette déclinaison est présente dans une commande elle ne pourra plus être écrite dans Sage !"
                            + "\n\n" + "Validez la suppression du conditionnement "
                            + (defaut ? "par défaut" : string.Empty) + " ?", "Suppression conditionnement",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (defaut)
                        {
                            Model.Local.ConditioningArticle ConditioningArticleNext = ListConditioningArticle.First(conart => conart.ConArt_Id != SelectedConditioningArticle.ConArt_Id);
                            ConditioningArticleNext.ConArt_Default = true;
                            this.AttributeArticleRepository.Save();
                        }
                        if (SelectedConditioningArticle.Pre_Id != null)
                            DeletePsProductAttribute(SelectedConditioningArticle.Pre_Id.Value);
                        this.ConditioningArticleRepository.Delete(SelectedConditioningArticle);
                        LoadConditioningArticle();
                    }
                }
            }
        }
        public void DeleteAllConditioningArticle()
        {
            if (ListConditioningArticle != null && ListConditioningArticle.Count > 0)
            {
                if (ListConditioningArticle.Count(a => a._HasUpdated) > 0)
                {
                    MessageBox.Show("Des modifications sur les conditionnements n'ont pas été enregistrées, impossible d'effectuer une suppression !", "Suppression conditionnement", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (ListConditioningArticle.Count(ca => ca.Pre_Id != null) == 0)
                {
                    if (MessageBox.Show("Validez la suppression des conditionnements ?", "Suppression conditionnements",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        this.ConditioningArticleRepository.DeleteAll(ListConditioningArticle.ToList());
                        LoadConditioningArticle();
                    }
                }
                else if (MessageBox.Show("Conditionnements liées à des déclinaisons PrestaShop !"
                    + "\n\n" + "Attention après la suppression si ces déclinaisons sont présentes dans une commande elles ne pourront plus être écrite dans Sage !"
                    + "\n\n" + "Validez la suppression ?", "Suppression conditionnements",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (Model.Local.ConditioningArticle ConditioningArticle in ListConditioningArticle)
                        if (ConditioningArticle.Pre_Id != null)
                            DeletePsProductAttribute(ConditioningArticle.Pre_Id.Value);
                    this.ConditioningArticleRepository.DeleteAll(ListConditioningArticle.ToList());
                    LoadConditioningArticle();
                }
            }
        }
    }
}
