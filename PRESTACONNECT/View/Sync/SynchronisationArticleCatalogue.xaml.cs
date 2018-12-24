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
	/// Logique d'interaction pour SynchronisationArticleCatalogue.xaml
	/// </summary>
    /// 
	public partial class SynchronisationArticleCatalogue : Window
	{
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public SynchronisationArticleCatalogue(List<Int32> ArticlesSend = null)
        {
            this.InitializeComponent();
            List<Int32> ListArticle = ArticlesSend;
            if (ArticlesSend == null)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                ListArticle = ArticleRepository.ListIdSyncOrderByPack(true);
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

        public SynchronisationArticleCatalogue(Int32 ArticleSend)
        {
            this.InitializeComponent();
            List<Int32> ListArticle = new List<Int32>();
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
            try
            {
                Core.Sync.SynchronisationArticleCatalogue Sync = new Core.Sync.SynchronisationArticleCatalogue();
                Sync.Exec(Article);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
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
                this.ProgressBarArticle.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
	}
}