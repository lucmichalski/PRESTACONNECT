using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour ControlURLRewrite.xaml
    /// </summary>
    public partial class ControlURLRewrite : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        private Boolean RewritePrestaShop = false;

        public ControlURLRewrite(bool rewritePrestashop)
        {
            this.InitializeComponent();

            Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
            List<Int32> ListCatalog = CatalogRepository.ListId();
            this.ListCount = ListCatalog.Count;

            this.RewritePrestaShop = rewritePrestashop;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgressCatalogue(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListCatalog, this.ParallelOptions, ExecCatalog);
            });
        }
        public void ExecCatalog(Int32 CatalogueSend)
        {
            this.Semaphore.WaitOne();

            try
            {
                Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                Model.Local.Catalog Catalog = CatalogRepository.ReadId(CatalogueSend);
                string url = Core.Global.ReadLinkRewrite(Catalog.Cat_LinkRewrite);
                if (url != Catalog.Cat_LinkRewrite)
                {
                    Catalog.Cat_LinkRewrite = url;
                    if (!this.RewritePrestaShop)
                        Catalog.Cat_Date = DateTime.Now;
                    CatalogRepository.Save();

                    if (this.RewritePrestaShop && Catalog.Pre_Id != null)
                    {
                        Model.Prestashop.PsCategoryLangRepository PsCategoryLangRepository = new Model.Prestashop.PsCategoryLangRepository();
                        if (PsCategoryLangRepository.ExistCategoryLang(Catalog.Pre_Id.Value, Core.Global.Lang, Core.Global.CurrentShop.IDShop))
                        {
                            Model.Prestashop.PsCategoryLang PsCategoryLang = PsCategoryLangRepository.ReadCategoryLang(Catalog.Pre_Id.Value, Core.Global.Lang, Core.Global.CurrentShop.IDShop);
                            PsCategoryLang.LinkRewrite = url;
                            PsCategoryLangRepository.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }

            lock (this)
            {
                this.CurrentCount += 1;
            }
            this.ReportProgressCatalogue(this.CurrentCount * 100 / this.ListCount);
            this.Semaphore.Release();
        }
        public void ReportProgressCatalogue(Int32 Percentage)
        {
            Context.Post(state =>
            {
                this.ProgressBarURL.Value = Percentage;
                this.LabelInformation.Content = "URL des catalogues : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.ControlURLRewriteArticle();
                }
            }, null);
        }

        public void ControlURLRewriteArticle()
        {
            this.CurrentCount = 0;
            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            List<Int32> ListArticles = ArticleRepository.ListId();
            this.ListCount = ListArticles.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            this.ReportProgressArticle(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticles, this.ParallelOptions, ExecArticle);
            });
        }
        public void ExecArticle(Int32 ArticleSend)
        {
            this.Semaphore.WaitOne();

            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                string url = Core.Global.ReadLinkRewrite(Article.Art_LinkRewrite);
                if (url != Article.Art_LinkRewrite)
                {
                    Article.Art_LinkRewrite = url;
                    if (!this.RewritePrestaShop)
                        Article.Art_Date = DateTime.Now;
                    ArticleRepository.Save();

                    if (this.RewritePrestaShop && Article.Pre_Id != null)
                    {
                        Model.Prestashop.PsProductLangRepository PsProductLangRepository = new Model.Prestashop.PsProductLangRepository();
                        if (PsProductLangRepository.ExistProductLang(Article.Pre_Id.Value, Core.Global.Lang, Core.Global.CurrentShop.IDShop))
                        {
                            Model.Prestashop.PsProductLang PsProductLang = PsProductLangRepository.ReadProductLang(Article.Pre_Id.Value, Core.Global.Lang, Core.Global.CurrentShop.IDShop);
                            PsProductLang.LinkRewrite = url;
                            PsProductLangRepository.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }

            lock (this)
            {
                this.CurrentCount += 1;
            }
            this.ReportProgressArticle(this.CurrentCount * 100 / this.ListCount);
            this.Semaphore.Release();
        }
        public void ReportProgressArticle(Int32 Percentage)
        {
            Context.Post(state =>
            {
                this.ProgressBarURL.Value = Percentage;
                this.LabelInformation.Content = "URL des articles : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
    }
}