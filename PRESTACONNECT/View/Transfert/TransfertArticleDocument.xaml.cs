using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour TransfertArticleDocument.xaml
    /// </summary>
    public partial class TransfertArticleDocument : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public TransfertArticleDocument(List<Int32> ListArticle = null)
        {
            this.InitializeComponent();
            if (ListArticle == null)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                ListArticle = ArticleRepository.ListIdSync(true);
            }

            Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
            List<int> listarticlewithdoctosync = AttachmentRepository.ListIDArticleNotSync();
            ListArticle = ListArticle.Where(a => listarticlewithdoctosync.Contains(a)).ToList();

            this.ListCount = ListArticle.Count;
            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Sync);
            });
        }

        public TransfertArticleDocument(Int32 ArticleSend)
        {
            this.InitializeComponent();
            List<Int32> ListArticle = new List<int>();
            ListArticle.Add(ArticleSend);
            this.ListCount = ListArticle.Count;
            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Int32 Article)
        {
            this.Semaphore.WaitOne();
            Core.Transfert.TransfertArticleDocument Sync = new Core.Transfert.TransfertArticleDocument();
            Sync.Exec(Article);
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
                    this.ProgressBarArticleDocument.Value = Percentage;
                    this.LabelInformation.Content = "Informations : " + Percentage + " %";
                    if (this.CurrentCount == this.ListCount)
                    {
                        this.Close();
                    }
                }, null);
        }
    }
}