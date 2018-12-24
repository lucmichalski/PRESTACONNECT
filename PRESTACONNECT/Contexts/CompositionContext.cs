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
    internal sealed class CompositionContext : Context
    {
        #region Properties

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
            set { selectedResultSearchCompositionArticle = value; OnPropertyChanged("SelectedResultSearchCompositionArticle"); this.SelectLocalCatalog(); }
        }

        public Visibility ShowResultAttributeColumn { get { return ((ListResultSearchCompositionArticle != null && ListResultSearchCompositionArticle.Count(ca => ca.F_ARTENUMREF_SagId != null) > 0) ? Visibility.Visible : Visibility.Hidden); } }
        public Visibility ShowResultConditioningColumn { get { return ((ListResultSearchCompositionArticle != null && ListResultSearchCompositionArticle.Count(ca => ca.F_CONDITION_SagId != null) > 0) ? Visibility.Visible : Visibility.Hidden); } }

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

        private Boolean filterComposition = true;
        public Boolean FilterComposition
        {
            get { return filterComposition; }
            set { filterComposition = value; OnPropertyChanged("FilterComposition"); }
        }
        private Boolean filterNothing = false;
        public Boolean FilterNothing
        {
            get { return filterNothing; }
            set { filterNothing = value; OnPropertyChanged("FilterNothing"); }
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

        #endregion

        private string strCompositionFilteredProducts = string.Empty;
        public string StrCompositionFilteredProducts
        {
            get { return strCompositionFilteredProducts; }
            set { strCompositionFilteredProducts = value; OnPropertyChanged("StrCompositionFilteredProducts"); }
        }

        private List<Model.Local.Catalog> listLocalCatalog = new List<Model.Local.Catalog>();
        public List<Model.Local.Catalog> ListLocalCatalog
        {
            get { return listLocalCatalog; }
            set { listLocalCatalog = value; OnPropertyChanged("ListLocalCatalog"); }
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

        private string stringTargetCatalog = string.Empty;
        public string StringTargetCatalog
        {
            get { return stringTargetCatalog; }
            set { stringTargetCatalog = value; OnPropertyChanged("StringTargetCatalog"); }
        }

        private Boolean canAddComposition = false;
        public Boolean CanAddComposition
        {
            get { return canAddComposition; }
            set { canAddComposition = value; OnPropertyChanged("CanAddComposition"); }
        }

        #endregion

        #region Constructors

        public CompositionContext()
            : base()
        {

            ListF_TAXE = new ObservableCollection<Model.Sage.F_TAXE>(new Model.Sage.F_TAXERepository().ListTTauxSens(ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_TTaux.Taux, (short)ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_Sens.Collectee));
            ListF_FAMILLE = new ObservableCollection<Model.Sage.F_FAMILLE>(new Model.Sage.F_FAMILLERepository().List());
            ListP_GAMME = new ObservableCollection<Model.Sage.P_GAMME>(new Model.Sage.P_GAMMERepository().ListIntituleNotNull());
            ListP_CONDITIONNEMENT = new ObservableCollection<Model.Sage.P_CONDITIONNEMENT>(new Model.Sage.P_CONDITIONNEMENTRepository().ListIntituleNotNull());

            ListLocalCatalog = new Model.Local.CatalogRepository().List();
        }

        #endregion

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

                Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();

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
                    if (FilterComposition)
                    {
                        ListF_ARTICLE_SagId = CompositionArticleRepository.ListSageF_ARTICLE();
                        ListF_ARTENUMREF_SagId = CompositionArticleRepository.ListSageF_ARTENUMREF();
                        ListF_CONDITION_SagId = CompositionArticleRepository.ListSageF_CONDITION();
                    }
                    if (FilterArticle)
                    {
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
            FilterComposition = true;
            FilterArticle = true;
        }

        public void CreateCompositionArticle()
        {
            if (SelectedResultSearchCompositionArticle != null)
            {
                Model.Sage.F_ARTICLE F_ARTICLE = new Model.Sage.F_ARTICLERepository().ReadArticle(SelectedResultSearchCompositionArticle.cbMarq);
                if (F_ARTICLE != null)
                {
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();

                    string dft = Core.Global.RemovePurge(SelectedResultSearchCompositionArticle.AR_Design, 255);

                    Model.Local.Article Article = new Model.Local.Article()
                    {
                        Art_Name = dft,
                        Art_Type = (short)Model.Local.Article.enum_TypeArticle.ArticleComposition,
                        Art_Description = SelectedResultSearchCompositionArticle.AR_Design,
                        Art_Description_Short = SelectedResultSearchCompositionArticle.AR_Design,
                        Art_MetaTitle = dft,
                        Art_MetaDescription = dft,
                        Art_MetaKeyword = Core.Global.RemovePurgeMeta(dft, 255),
                        Art_LinkRewrite = Core.Global.ReadLinkRewrite(dft),
                        Art_Active = Core.Global.GetConfig().ImportArticleStatutActif,
                        Art_Date = DateTime.Now,
                        Art_Solde = false,
                        Art_Sync = false,
                        Sag_Id = 0,
                        Cat_Id = 0,
                        Art_RedirectType = new Model.Internal.RedirectType(Core.Parametres.RedirectType.NoRedirect404).Page,
                        Art_RedirectProduct = 0,
                        // champs non renseignés pour les compositions
                        Art_Pack = false,
                        Art_Ref = string.Empty,
                        Art_Ean13 = string.Empty,
                    };

                    Core.ImportSage.ImportArticle ImportArticle = new Core.ImportSage.ImportArticle();
                    Article.Cat_Id = ImportArticle.ReadCatalog(F_ARTICLE);

                    if (Article.Cat_Id == 0 && Core.Temp.selectedcatalog_composition != 0)
                        Article.Cat_Id = Core.Temp.selectedcatalog_composition;

                    if (Article.Cat_Id != 0)
                    {
                        ArticleRepository.Add(Article);

                        ImportArticle.AssignCatalog(0, Article.Cat_Id, Article, Core.Global.GetConfig().ImportArticleRattachementParents);

                        Core.ImportSage.ImportStatInfoLibreArticle ImportStatInfoLibreArticle = new Core.ImportSage.ImportStatInfoLibreArticle();
                        ImportStatInfoLibreArticle.ImportValues(Article, F_ARTICLE.AR_Ref);

                        PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                        Loading.Show();

                        Core.Temp.selected_taxe_composition = SelectedF_TAXE;
                        Core.Temp.reference_sage_composition = F_ARTICLE.AR_Ref;
                        Core.Temp.designation_composition = F_ARTICLE.AR_Design;

                        PRESTACONNECT.Article Form = new Article(Article);
                        Loading.Close();
                        Form.ShowDialog();

                        if (FilterComposition)
                            SearchArticleComposition();
                    }
                    else
                    {
                        MessageBox.Show("Aucune correspondance catalogue n'a été trouvée !", "Lien catalogue absent", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
        }

        public void SelectLocalCatalog()
        {
            CanAddComposition = false;
            if (this.SelectedResultSearchCompositionArticle != null)
            {
                int cat_id = 0;
                if (this.SelectedResultSearchCompositionArticle.Catalogue4_SagId != null && this.SelectedResultSearchCompositionArticle.Catalogue4_SagId != 0)
                    cat_id = this.SelectedResultSearchCompositionArticle.Catalogue4_SagId.Value;
                if (cat_id == 0 && this.SelectedResultSearchCompositionArticle.Catalogue3_SagId != null && this.SelectedResultSearchCompositionArticle.Catalogue3_SagId != 0)
                    cat_id = this.SelectedResultSearchCompositionArticle.Catalogue3_SagId.Value;
                if (cat_id == 0 && this.SelectedResultSearchCompositionArticle.Catalogue2_SagId != null && this.SelectedResultSearchCompositionArticle.Catalogue2_SagId != 0)
                    cat_id = this.SelectedResultSearchCompositionArticle.Catalogue2_SagId.Value;
                if (cat_id == 0 && this.SelectedResultSearchCompositionArticle.Catalogue1_SagId != null && this.SelectedResultSearchCompositionArticle.Catalogue1_SagId != 0)
                    cat_id = this.SelectedResultSearchCompositionArticle.Catalogue1_SagId.Value;

                if (cat_id != 0 && ListLocalCatalog.Count(c => c.Sag_Id == cat_id) > 0)
                {
                    WriteStringTargetCatalog(this.SelectedResultSearchCompositionArticle.Catalogue1_SagId.Value);
                }
                else if (cat_id != 0 && Core.Temp.selectedcatalog_composition == 0)
                {
                    StringTargetCatalog = "Lien pour le catalogue Sage de plus bas niveau introuvable dans PrestaConnect !";
                }
                else if (cat_id != 0 && Core.Temp.selectedcatalog_composition != 0)
                {
                    StringTargetCatalog = "Lien catalogue Sage introuvable dans PrestaConnect ! Le catalogue courant sera utilisé : " + this.ListLocalCatalog.FirstOrDefault(c => c.Cat_Id == Core.Temp.selectedcatalog_composition).ComboText + " !";
                    CanAddComposition = true;
                }
                else if (cat_id == 0 && Core.Temp.selectedcatalog_composition != 0)
                {
                    StringTargetCatalog = "Absence de catalogue Sage ! Le catalogue courant sera utilisé : " + this.ListLocalCatalog.FirstOrDefault(c => c.Cat_Id == Core.Temp.selectedcatalog_composition).ComboText + " !";
                    CanAddComposition = true;
                }
                else
                {
                    StringTargetCatalog = "Absence de catalogue dans Sage et aucun catalogue sélectionné sur la liste des articles !";
                }
            }
            else
            {
                StringTargetCatalog = "Veuillez sélectionner un article Sage !";
            }
        }
        public void WriteStringTargetCatalog(int F_CATALOGUE_cbMarq)
        {
            if (this.ListLocalCatalog.Count(c => c.Sag_Id == F_CATALOGUE_cbMarq) == 1)
            {
                StringTargetCatalog = "Lien trouvé pour le catalogue Sage : " + this.SelectedResultSearchCompositionArticle.Catalogue;
            }
            else
            {
                StringTargetCatalog = "" + this.ListLocalCatalog.Count(c => c.Sag_Id == F_CATALOGUE_cbMarq) + " catalogues PrestaConnect lié au catalogue Sage : " + this.SelectedResultSearchCompositionArticle.Catalogue;
            }
            CanAddComposition = true;
        }
    }
}
