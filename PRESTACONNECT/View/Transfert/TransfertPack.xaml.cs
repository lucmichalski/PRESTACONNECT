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
    /// Logique d'interaction pour TransfertPack.xaml
	/// </summary>
    public partial class TransfertPack : Window
	{
        
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);

        private ParallelOptions ParallelOptions = new ParallelOptions();

		public TransfertPack()
		{
            this.InitializeComponent();
            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            List<Int32> ListArticle = ArticleRepository.ListIdSyncPack(true);

            // récupération des éléments du pack 
            //foreach (Int32 ArticleSend in ListArticle)
            //{
            //    Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
            //    if (Article.Art_Pack == true)
            //    {
            //        List<Int32> PackElements;
            //        SynchronisationArticle.ReadPackElements(ArticleRepository, Article, out PackElements);
            //        ListArticle.AddRange(PackElements);
            //    }
            //    ListArticle.Add(ArticleSend);
            //}

            this.ListCount = ListArticle.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Transfert);
            });
        }
        public TransfertPack(List<Int32> ArticlesSend)
        {
            this.InitializeComponent();
            List<Int32> ListArticle = new List<int>();
            ListArticle = ArticlesSend;
            // récupération des éléments du pack 
            //Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            //foreach (Int32 ArticleSend in ArticlesSend)
            //{
            //    Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
            //    if (Article.Art_Pack == true)
            //    {
            //        List<Int32> PackElements;
            //        SynchronisationArticle.ReadPackElements(ArticleRepository, Article, out PackElements);
            //        ListArticle.AddRange(PackElements);
            //    }
            //    ListArticle.Add(ArticleSend);
            //}

            this.ListCount = ListArticle.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Transfert);
            });
        }

        public void Transfert(Int32 Article)
        {
            this.Semaphore.WaitOne();
            Core.Transfert.TransfertPack transfert = new Core.Transfert.TransfertPack();
            transfert.Exec(Article);
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