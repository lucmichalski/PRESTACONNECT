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
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour ImportSageArticleConditionnement.xaml
    /// </summary>
    public partial class ImportSageArticleConditionnement : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ImportSageArticleConditionnement(List<Int32> Articles)
        {
            this.InitializeComponent();
            this.ListCount = Articles.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Articles, this.ParallelOptions, Sync);
            });
        }

        public ImportSageArticleConditionnement(Int32 ArticleSend)
        {
            this.InitializeComponent();

            List<Int32> Articles = new List<int>();
            Articles.Add(ArticleSend);
            this.ListCount = Articles.Count;
            Context = SynchronizationContext.Current;
            this.ReportProgress(0);

            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Articles, Sync);
            });
        }

        public void Sync(Int32 Article)
        {
            this.Semaphore.WaitOne();

            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            if (ArticleRepository.ExistArticle(Article))
            {
                Model.Local.Article Item = ArticleRepository.ReadArticle(Article);
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                Model.Sage.F_ARTICLE F_ARTICLE = (F_ARTICLERepository.ExistArticle(Item.Sag_Id) ? F_ARTICLERepository.ReadArticle(Item.Sag_Id) : null);
                if (F_ARTICLE != null)
                {
                    if (Core.Global.GetConfig().ArticleImportConditionnementActif
                        && F_ARTICLE.AR_Condition != null && F_ARTICLE.AR_Condition != 0)
                    {
                        Core.ImportSage.ImportArticle importarticle = new Core.ImportSage.ImportArticle();
                        if (importarticle.ExecConditioning(F_ARTICLE, Item))
                        {
                            Item.Art_Date = DateTime.Now;
                            ArticleRepository.Save();
                        }
                    }
                }
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
                    this.Close();
                }
            }, null);
        }
    }
}