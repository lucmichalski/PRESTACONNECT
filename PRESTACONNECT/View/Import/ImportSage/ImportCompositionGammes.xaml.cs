using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour ImportCompositionGammes.xaml
    /// </summary>
    public partial class ImportCompositionGammes : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;

        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public List<string> Logs = new List<string>();

        public ImportCompositionGammes()
        {
            this.InitializeComponent();
            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            List<Int32> ListLocal = ArticleRepository.ListIdTypeComposition();
            this.ListCount = ListLocal.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListLocal, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Int32 id)
        {
            this.Semaphore.WaitOne();

            List<string> log = new List<string>();

            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            Model.Local.Article ArticleCompo = ArticleRepository.ReadArticle(id);
            if (ArticleCompo != null)
            {
                Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                List<Model.Local.CompositionArticle> ListCompo = CompositionArticleRepository.ListArticle(ArticleCompo.Art_Id);
                if (ListCompo.Count > 0)
                {
                    Model.Local.CompositionArticle CompoDefaut = ListCompo.FirstOrDefault(c => c.ComArt_Default);
                    if (CompoDefaut == null || CompoDefaut.ComArt_Id == 0)
                        CompoDefaut = ListCompo.First();
                    // Si toutes les compos sont liées à la même réference Sage
                    // ET qu'elles sont toutes liées à une gamme ou un conditionnement
                    if (ListCompo.Count == ListCompo.Count(c => c.ComArt_F_ARTICLE_SagId == CompoDefaut.ComArt_F_ARTICLE_SagId
                        && (c.ComArt_F_ARTENUMREF_SagId != null)))// ^ c.ComArt_F_CONDITION_SagId != null)))
                    {
                        // on récupère la liste des énumérés de l'article Sage
                        Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                        List<Model.Sage.F_ARTICLE_Composition> ListF_ARTICLE_Composition = F_ARTICLERepository.ListComposition(CompoDefaut.F_ARTICLE_Light.AR_Ref);
                        // si on n'affiche pas les données en sommeil alors on filtre les gammes
                        if (!Core.Global.GetConfig().ArticleEnSommeil)
                            ListF_ARTICLE_Composition = ListF_ARTICLE_Composition.Where(c => c.AE_Sommeil == 0).ToList();
                        ListF_ARTICLE_Composition = Core.Tools.FiltreImportSage.ImportSageFilter(ListF_ARTICLE_Composition);
                        // on filtre sur ceux déjà présents dans les compos
                        List<Int32?> listidsagecompo = new List<Int32?>();
                        if (CompoDefaut.ComArt_F_ARTENUMREF_SagId != null)
                        {
                            listidsagecompo = ListCompo.Select(c => c.ComArt_F_ARTENUMREF_SagId).ToList();
                            ListF_ARTICLE_Composition = ListF_ARTICLE_Composition.Where(c => !listidsagecompo.Contains(c.F_ARTENUMREF_SagId) && !string.IsNullOrWhiteSpace(c.EnumereGamme1)).ToList();
                        }
                        //else if (CompoDefaut.ComArt_F_CONDITION_SagId != null)
                        //{
                        //    listidsagecompo = ListCompo.Select(c => c.ComArt_F_CONDITION_SagId).ToList();
                        //    ListF_ARTICLE_Composition = ListF_ARTICLE_Composition.Where(c => !listidsagecompo.Contains(c.F_CONDITION_SagId)).ToList();
                        //}

                        // si on a une déclinaison à créer
                        if (ListF_ARTICLE_Composition.Count > 0)
                        {
                            // on identifie le groupe d'attribut Prestashop
                            // fonctionne uniquement pour les articles mono-gamme
                            Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Model.Prestashop.PsAttributeGroupLangRepository();
                            Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang = null;
                            if (ArticleCompo.CompositionArticleAttributeGroup != null && ArticleCompo.CompositionArticleAttributeGroup.Count == 1)
                            {
                                Model.Local.CompositionArticleAttributeGroup CAAG = ArticleCompo.CompositionArticleAttributeGroup.First();
                                if (PsAttributeGroupLangRepository.ExistAttributeGroupLang((uint)CAAG.Cag_AttributeGroup_PreId, Core.Global.Lang))
                                {
                                    PsAttributeGroupLang = PsAttributeGroupLangRepository.ReadAttributeGroupLang((uint)CAAG.Cag_AttributeGroup_PreId, Core.Global.Lang);
                                }
                            }

                            if (PsAttributeGroupLang != null)
                            {
                                foreach (Model.Sage.F_ARTICLE_Composition F_ARTICLE_Composition in ListF_ARTICLE_Composition)
                                {
                                    // création de la composition dans l'article Prestaconnect
                                    Model.Local.CompositionArticle CompositionArticle = new Model.Local.CompositionArticle()
                                    {
                                        ComArt_Active = false,
                                        ComArt_ArtId = ArticleCompo.Art_Id,
                                        ComArt_F_ARTICLE_SagId = F_ARTICLE_Composition.cbMarq,
                                        ComArt_F_ARTENUMREF_SagId = F_ARTICLE_Composition.F_ARTENUMREF_SagId,
                                        ComArt_F_CONDITION_SagId = F_ARTICLE_Composition.F_CONDITION_SagId,
                                        ComArt_Quantity = 1,
                                        ComArt_Sync = true,
                                        ComArt_Default = false,
                                        HasUpdated = false,
                                    };
                                    CompositionArticleRepository.Add(CompositionArticle);
                                    log.Add("ICG10- Import auto déclinaison gamme [ " + CompoDefaut.F_ARTICLE_Light.AR_Ref + " / " + F_ARTICLE_Composition.EnumereGamme1 + " ] pour la composition : " + ArticleCompo.Art_Name + "");

                                    // création et affectation automatique des attributs
                                    Model.Prestashop.PsAttributeLang PsAttributeLang = null;
                                    Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();

                                    lock (Core.Temp.ListAttributeGroupLocked)
                                    {
                                        // ajout de la gestion d'une liste de locked attribute group au moment de la création pour éviter les doublons //
                                        while (Core.Temp.ListAttributeGroupLocked.Contains(PsAttributeGroupLang.IDAttributeGroup))
                                            System.Threading.Thread.Sleep(20);

                                        Core.Temp.ListAttributeGroupLocked.Add(PsAttributeGroupLang.IDAttributeGroup);

                                        if (PsAttributeLangRepository.ExistAttributeLang(F_ARTICLE_Composition.EnumereGamme1.Trim(), Core.Global.Lang, PsAttributeGroupLang.IDAttributeGroup))
                                        {
                                            PsAttributeLang = PsAttributeLangRepository.ReadAttributeLang(F_ARTICLE_Composition.EnumereGamme1.Trim(), Core.Global.Lang, PsAttributeGroupLang.IDAttributeGroup);
                                        }
                                        else
                                        {
                                            Model.Prestashop.PsAttributeRepository PsAttributeRepository = new Model.Prestashop.PsAttributeRepository();
                                            Model.Prestashop.PsAttribute PsAttribute = new Model.Prestashop.PsAttribute()
                                            {
                                                IDAttributeGroup = PsAttributeGroupLang.IDAttributeGroup,
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
                                                        Name = F_ARTICLE_Composition.EnumereGamme1.Trim(),
                                                    });
                                                }

                                            PsAttributeLang = PsAttributeLangRepository.ReadAttributeLang(PsAttribute.IDAttribute, Core.Global.Lang);
                                        }

                                        Core.Temp.ListAttributeGroupLocked.Remove(PsAttributeGroupLang.IDAttributeGroup);
                                    }

                                    if (PsAttributeLang != null)
                                    {
                                        // affectation composition
                                        if (CompositionArticle.CompositionArticleAttribute.Count(ag => ag.Caa_AttributeGroup_PreId == (int)PsAttributeGroupLang.IDAttributeGroup) == 0)
                                        {
                                            CompositionArticle.CompositionArticleAttribute.Add(new Model.Local.CompositionArticleAttribute()
                                            {
                                                Caa_ComArtId = CompositionArticle.ComArt_ArtId,
                                                Caa_AttributeGroup_PreId = (int)PsAttributeGroupLang.IDAttributeGroup,
                                                Caa_Attribute_PreId = (int)PsAttributeLang.IDAttribute,
                                            });
                                        }
                                        else
                                        {
                                            Model.Local.CompositionArticleAttribute CompositionArticleAttribute = CompositionArticle.CompositionArticleAttribute.First(ag => ag.Caa_AttributeGroup_PreId == (int)PsAttributeGroupLang.IDAttributeGroup);
                                            CompositionArticleAttribute.Caa_Attribute_PreId = (int)PsAttributeLang.IDAttribute;
                                        }

                                        // si on a pu affecter l'attribut alors on active la déclinaison
                                        CompositionArticle.ComArt_Active = true;
                                    }

                                    CompositionArticleRepository.Save();

                                    // Marquage maj article compo
                                    ArticleCompo.Art_Date = DateTime.Now;
                                    ArticleRepository.Save();
                                }
                            }
                        }
                    }
                }
            }
            lock (Logs)
            {
                if (log != null && log.Count > 0)
                    Logs.AddRange(log);
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
                    if (Logs != null && Logs.Count > 0)
                        Core.Log.SendLog(Logs, Core.Log.LogIdentifier.ImportAutoCompositionGammes);

                    this.Close();
                }
            }, null);
        }
    }
}
