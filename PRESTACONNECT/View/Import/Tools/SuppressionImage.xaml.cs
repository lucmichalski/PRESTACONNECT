using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace PRESTACONNECT
{
	/// <summary>
    /// Logique d'interaction pour SuppressionImage.xaml
	/// </summary>
	public partial class SuppressionImage : Window
	{
		public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        List<string> logs = new List<string>();

        bool DeleteOnlyNotSource = true;

        public SuppressionImage(Boolean DeleteOnlyIfSourceNotExist)
        {
            this.InitializeComponent();

            DeleteOnlyNotSource = DeleteOnlyIfSourceNotExist;

            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            List<int> ListArticle = ArticleRepository.ListId();
            this.ListCount = ListArticle.Count;
            this.Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Exec);
            });
        }

        public void Exec(int Article)
        {
            this.Semaphore.WaitOne();

            List<string> log;
            Core.Tools.SuppressionImage delete = new Core.Tools.SuppressionImage();
            delete.ExecArticle(Article, DeleteOnlyNotSource, out log);
            if (log != null && log.Count > 0)
                lock (this.logs)
                    logs.AddRange(log);

            lock (this)
            {
                this.CurrentCount += 1;
            }
            this.ReportProgress(this.CurrentCount * 100 / this.ListCount);
            this.Semaphore.Release();
        }

        public void ReportProgress(Int32 Percentage)
        {
            this.Context.Post(state =>
            {
                this.ProgressBarImage.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    if (logs != null && logs.Count > 0)
                        Core.Log.WriteSpecificLog(logs, Core.Log.LogIdentifier.ImportAutoImage);

                    this.Close();
                }
            }, null);
        }
    }
}