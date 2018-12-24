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
    /// Logique d'interaction pour TransfertAttribute.xaml
	/// </summary>
	public partial class TransfertAttribute : Window
	{
        
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);

        private ParallelOptions ParallelOptions = new ParallelOptions();

        public TransfertAttribute(List<Int32> ListArticle = null)
        {
            this.InitializeComponent();
            if (ListArticle == null)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                ListArticle = ArticleRepository.ListIdSync(true);
            }
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
            Core.Transfert.TransfertAttribute transfert = new Core.Transfert.TransfertAttribute();
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