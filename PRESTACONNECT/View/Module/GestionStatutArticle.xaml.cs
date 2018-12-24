using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour GestionStatutArticle.xaml
    /// </summary>
    public partial class GestionStatutArticle : Window
    {
        ObservableCollection<Model.Local.Article_Progress> list_progress = new ObservableCollection<Model.Local.Article_Progress>();

        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public List<string> logs = new List<string>();
        Boolean Sommeil = Core.Global.GetConfig().ArticleEnSommeil;
        Boolean NonPublie = Core.Global.GetConfig().ArticleNonPublieSurLeWeb;

        public GestionStatutArticle(List<Int32> ListArticle = null)
        {
            this.InitializeComponent();
            if (ListArticle == null)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                ListArticle = ArticleRepository.ListId();
            }
            this.ListCount = ListArticle.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Int32 ArticleSend)
        {
            this.Semaphore.WaitOne();

            ReportInfosSynchro(ArticleSend, Core.Temp._action_information_synchro.debut);

            List<string> log = new List<string>();
            try
            {

                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();

                Model.Prestashop.PsProductRepository PsProductRepository;
                Model.Prestashop.PsProduct PsProduct;

                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);

                int sag_id = Article.Sag_Id;
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                if (!F_ARTICLERepository.ExistArticle(Article.Sag_Id)
                    && Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition
                    && Article.CompositionArticle != null && Article.CompositionArticle.Count > 0)
                {
                    sag_id = (from Table in Article.CompositionArticle
                              orderby Table.ComArt_Default descending
                              select Table.ComArt_F_ARTICLE_SagId).FirstOrDefault();
                }

                Boolean check_attributes = ((Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMonoGamme || Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMultiGammes)
                    && Article.AttributeArticle != null && Article.AttributeArticle.Count > 0);

                Boolean check_composition = (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition
                    && Article.CompositionArticle != null && Article.CompositionArticle.Count > 0);

                if (F_ARTICLERepository.ExistArticle(sag_id))
                {
                    Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(sag_id);

                    bool importsagefilterexclusion = Core.Tools.FiltreImportSage.ImportSageFilterExclude(F_ARTICLE);
                    // si on affiche pas les sommeil et que article en sommeil = True
                    // si on affiche pas les non publie et que article non publie = True
                    // si affiche sommeil ou non publie = False
                    if (Article.Art_Active && ((!Sommeil && F_ARTICLE.AR_Sommeil == 1) || (!NonPublie && F_ARTICLE.AR_Publie == 0) || importsagefilterexclusion))
                    {
                        Article.Art_Active = false;
                        if (Article.Pre_Id != null)
                        {
                            PsProductRepository = new Model.Prestashop.PsProductRepository();
                            if (PsProductRepository.ExistId((uint)Article.Pre_Id))
                            {
                                PsProduct = PsProductRepository.ReadId((uint)Article.Pre_Id);
                                PsProduct.Active = 0;
                                PsProductRepository.Save();

                                //<JG> 23/01/2014 ajout modif sur PsProductShop
                                Model.Prestashop.PsProductShopRepository PsProductShopRepository = new Model.Prestashop.PsProductShopRepository();
                                if (PsProductShopRepository.ExistProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop))
                                {
                                    Model.Prestashop.PsProductShop PsProductShop = PsProductShopRepository.ReadProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop);
                                    PsProductShop.Active = PsProduct.Active;
                                    PsProductShopRepository.Save();
                                }
                            }
                        }
                        ArticleRepository.Save();

                        log.Add("SA10- Passage à l'état \"Inactif\" pour l'article [ " + Article.Art_Ref + " - " + Article.Art_Name + " ]" + (importsagefilterexclusion ? "(filtre d'exclusion)" : string.Empty));
                    }
                    else if (Article.Art_Active == false && ((F_ARTICLE.AR_Sommeil == 0 || Sommeil) && (F_ARTICLE.AR_Publie == 1 || NonPublie) && !importsagefilterexclusion))
                    {
                        Article.Art_Active = true;
                        if (Article.Pre_Id != null)
                        {
                            PsProductRepository = new Model.Prestashop.PsProductRepository();
                            if (PsProductRepository.ExistId((uint)Article.Pre_Id))
                            {
                                PsProduct = PsProductRepository.ReadId((uint)Article.Pre_Id);
                                PsProduct.Active = 1;
                                PsProductRepository.Save();

                                //<JG> 23/01/2014 ajout modif sur PsProductShop
                                Model.Prestashop.PsProductShopRepository PsProductShopRepository = new Model.Prestashop.PsProductShopRepository();
                                if (PsProductShopRepository.ExistProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop))
                                {
                                    Model.Prestashop.PsProductShop PsProductShop = PsProductShopRepository.ReadProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop);
                                    PsProductShop.Active = PsProduct.Active;
                                    PsProductShopRepository.Save();
                                }
                            }
                        }
                        ArticleRepository.Save();

                        log.Add("SA11- Passage à l'état \"Actif\" pour l'article [ " + Article.Art_Ref + " - " + Article.Art_Name + " ]");
                    }

                    Boolean ExistAECAttributeStatut = Core.Global.ExistAECAttributeStatut();
                    // 09/12/2016 si le module de désactivation des gammes est installé
                    if (ExistAECAttributeStatut)
                    {
                        if (check_attributes)
                        {
                            Core.ImportSage.ImportArticle ImportArticle = new Core.ImportSage.ImportArticle();
                            bool onlyattributestatut = true;
                            // gestion statuts en local
                            ImportArticle.ExecAttribute(F_ARTICLE, Article, onlyattributestatut);

                            // gestion statuts dans PrestaShop
                            Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                            List<Model.Local.AttributeArticle> ListAttributeArticle = AttributeArticleRepository.ListArticle(Article.Art_Id);
                            int cpt = 0;
                            foreach (Model.Local.AttributeArticle AttributeArticle in ListAttributeArticle)
                            {
                                cpt++;
                                if (AttributeArticle.Pre_Id != null)
                                {
                                    string combination;
                                    if (CheckStatutProductAttribute((uint)AttributeArticle.Pre_Id, (sbyte)(AttributeArticle.AttArt_Active ? 1 : 0), out combination))
                                        log.Add("SA51- " + "Référence : " + Article.Art_Ref + " / "
                                            + (AttributeArticle.AttArt_Active ? "Activation" : "Désactivation")
                                            + " déclinaison [" + combination + "] dans Prestashop ");
                                }
                                ReportInfosSynchro(ArticleSend, Core.Temp._action_information_synchro.refresh, "Déclinaison : " + cpt + "/" + ListAttributeArticle.Count);
                            }
                        }
                        else if (check_composition)
                        {
                            List<Model.Sage.F_ARTICLE_Composition> list_temp_filtre = new List<Model.Sage.F_ARTICLE_Composition>();
                            Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                            List<Model.Local.CompositionArticle> ListCompositionArticle = CompositionArticleRepository.ListArticle(Article.Art_Id);
                            int cpt = 0;
                            foreach (Model.Local.CompositionArticle compo in ListCompositionArticle)
                            {
                                cpt++;
                                List<Model.Sage.F_ARTICLE_Composition> list_temp = new List<Model.Sage.F_ARTICLE_Composition>();
                                Model.Sage.F_ARTICLE_Composition F_ARTICLE_Composition = compo.F_ARTICLE_Composition;
                                list_temp.Add(F_ARTICLE_Composition);

                                list_temp = Core.Tools.FiltreImportSage.ImportSageFilter(list_temp);
                                bool filter_exclusion = (list_temp.Count == 0);

                                bool enable = compo.ComArt_Active;
                                if (compo.ComArt_F_ARTENUMREF_SagId == null && compo.ComArt_F_CONDITION_SagId == null)
                                {
                                    enable = ((F_ARTICLE_Composition.AR_Sommeil == 0 || Sommeil) && (F_ARTICLE_Composition.AR_Publie == 1 || NonPublie) && !filter_exclusion);
                                }
                                else if (compo.ComArt_F_ARTENUMREF_SagId != null)
                                {
                                    Model.Sage.F_ARTENUMREF F_ARTENUMREF = compo.EnumereF_ARTENUMREF;
									#if (SAGE_VERSION_16 || SAGE_VERSION_17)
                                    enable = ((F_ARTICLE_Composition.AR_Sommeil == 0 || Sommeil) && (F_ARTICLE_Composition.AR_Publie == 1 || NonPublie) && !filter_exclusion
                                            && (Sommeil));
									#else
                                    enable = ((F_ARTICLE_Composition.AR_Sommeil == 0 || Sommeil) && (F_ARTICLE_Composition.AR_Publie == 1 || NonPublie) && !filter_exclusion
                                            && ((F_ARTENUMREF.AE_Sommeil != null && F_ARTENUMREF.AE_Sommeil == 0) || Sommeil));
									#endif
                                }

                                if (enable != compo.ComArt_Active)
                                {
                                    // gestion statut local
                                    compo.ComArt_Active = enable;
                                    CompositionArticleRepository.Save();
                                }
                                // gestion statut PrestaShop
                                if (compo.Pre_Id != null)
                                {
                                    string combination;
                                    if (CheckStatutProductAttribute((uint)compo.Pre_Id, (sbyte)(compo.ComArt_Active ? 1 : 0), out combination))
                                        log.Add("SA51- " + "Composition : " + Article.Art_Name + " / "
                                            + (compo.ComArt_Active ? "Activation" : "Désactivation")
                                            + " déclinaison [" + combination + "] dans Prestashop ");
                                }
                                ReportInfosSynchro(ArticleSend, Core.Temp._action_information_synchro.refresh, "Déclinaison : " + cpt + "/" + ListCompositionArticle.Count);
                            }
                        }
                    }
                }
                else if (Article.Art_Active || Article.Art_Sync)
                {
                    Article.Art_Active = false;
                    Article.Art_Sync = false;
                    if (Article.Pre_Id != null)
                    {
                        PsProductRepository = new Model.Prestashop.PsProductRepository();
                        if (PsProductRepository.ExistId((uint)Article.Pre_Id))
                        {
                            PsProduct = PsProductRepository.ReadId((uint)Article.Pre_Id);
                            PsProduct.Active = 0;
                            PsProductRepository.Save();

                            //<JG> 23/01/2014 ajout modif sur PsProductShop
                            Model.Prestashop.PsProductShopRepository PsProductShopRepository = new Model.Prestashop.PsProductShopRepository();
                            if (PsProductShopRepository.ExistProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop))
                            {
                                Model.Prestashop.PsProductShop PsProductShop = PsProductShopRepository.ReadProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop);
                                PsProductShop.Active = PsProduct.Active;
                                PsProductShopRepository.Save();
                            }
                        }
                    }
                    ArticleRepository.Save();

                    log.Add("SA12- Article Sage introuvable désactivation complète de l'article [ " + Article.Art_Ref + " - " + Article.Art_Name + " ]");
                }
            }
            catch (Exception ex)
            {
                log.Add("SA01- Gestion statut article : " + ex.Message);
            }

            if (log.Count > 0)
            {
                log.Add(Core.Log.LogLineSeparator);
                lock (this.logs)
                    logs.AddRange(log);
            }

            lock (this)
            {
                this.CurrentCount += 1;
            }
            ReportInfosSynchro(ArticleSend, Core.Temp._action_information_synchro.fin);
            this.ReportProgress(this.CurrentCount * 100 / this.ListCount);
            this.Semaphore.Release();
        }

        private Boolean CheckStatutProductAttribute(uint idproductattribute, sbyte active, out string combination)
        {
            Boolean insert_log = false;
            combination = string.Empty;
            Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
            Model.Prestashop.PsAECAttributeListAttributeRepository PsAECAttributeListAttributeRepository = new Model.Prestashop.PsAECAttributeListAttributeRepository();
            if (PsProductAttributeRepository.ExistProductAttribute(idproductattribute))
            {
                Model.Prestashop.PsProductAttribute PsProductAttribute = PsProductAttributeRepository.ReadProductAttribute(idproductattribute);
                if (!PsAECAttributeListAttributeRepository.ExistProductAttribute(idproductattribute))
                {
                    PsAECAttributeListAttributeRepository.Add(new Model.Prestashop.PsAEcAttributeListAttribute()
                    {
                        IDProduct = PsProductAttribute.IDProduct,
                        IDProductAttribute = PsProductAttribute.IDProductAttribute,
                        Packing = 1, // valeur par défaut
                        Active = active,
                    });
                    insert_log = true;
                }
                else
                {
                    Model.Prestashop.PsAEcAttributeListAttribute PsAEcAttributeListAttribute = PsAECAttributeListAttributeRepository.ReadProductAttribute(PsProductAttribute.IDProductAttribute);
                    if (active != PsAEcAttributeListAttribute.Active)
                    {
                        PsAEcAttributeListAttribute.Active = active;
                        PsAECAttributeListAttributeRepository.Save();
                        insert_log = true;
                    }
                }
                combination = PsProductAttribute.Combination;
            }
            return insert_log;
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
                        Core.Log.SendLog(logs, Core.Log.LogIdentifier.GestionStatutArticle);
                    }
                    this.Close();
                }
            }, null);
        }

        public void ReportInfosSynchro(Int32 Article, Core.Temp._action_information_synchro action, string comment = null)
        {
            Context.Post(state =>
            {
                switch (action)
                {
                    case PRESTACONNECT.Core.Temp._action_information_synchro.debut:
                        Model.Local.Article_Progress Article_Progress = new Model.Local.ArticleRepository().ReadArticleProgress(Article);
                        this.list_progress.Add(Article_Progress);
                        break;
                    case PRESTACONNECT.Core.Temp._action_information_synchro.fin:
                        this.list_progress.Remove(list_progress.FirstOrDefault(p => p.Art_Id == Article));
                        break;
                    case PRESTACONNECT.Core.Temp._action_information_synchro.refresh:
                        this.list_progress.FirstOrDefault(p => p.Art_Id == Article).Comment = comment;
                        break;
                    default:
                        break;
                }
                listBoxProgress.ItemsSource = list_progress;
            }, null);
        }
    }
}