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
    /// Logique d'interaction pour ImportSageStatInfoLibre.xaml
	/// </summary>
	public partial class ImportSageStatInfoLibre : Window
    {
        List<string> logs = new List<string>();

        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ImportSageStatInfoLibre(List<Int32> Articles, bool updatedateactive = true)
        {
            this.InitializeComponent();
            this.ListCount = Articles.Count;
            Core.Temp.UpdateDateActive = updatedateactive;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Articles, this.ParallelOptions, Sync);
            });
        }

        public ImportSageStatInfoLibre(Int32 ArticleSend)
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

            try
            {
                Core.ImportSage.ImportStatInfoLibreArticle Sync = new Core.ImportSage.ImportStatInfoLibreArticle();
                Sync.Exec(Article);
            }
            catch (Exception ex)
            {
                lock (this.logs)
                {
                    logs.Add("ISCA20- Erreur import caractéristiques articles : " + ex.ToString());
                }
                Core.Error.SendMailError("[ISCA20] " + ex.ToString());
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
                    if (logs != null && logs.Count > 0)
                    {
                        Core.Log.SendLog(logs, Core.Log.LogIdentifier.ImportAutoStatInfoLibreArticle);
                    }
                    this.Close();
                }
            }, null);
        }
	}
}