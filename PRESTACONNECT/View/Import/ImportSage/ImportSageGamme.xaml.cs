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
    /// Logique d'interaction pour ImportSageGamme.xaml
	/// </summary>
	public partial class ImportSageGamme : Window
	{
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public Semaphore Semaphore = new Semaphore(1, 1);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ImportSageGamme(List<short?> gammes)
        {
            this.InitializeComponent();
            this.ListCount = gammes.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(gammes, this.ParallelOptions, Sync);
            });
        }

        public void Sync(short? gamme)
        {
            this.Semaphore.WaitOne();
            if (gamme != null)
            {
                Core.ImportSage.ImportArticle ImportArticle = new Core.ImportSage.ImportArticle();
                ImportArticle.CreateAttributeGroup((int)gamme);
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
                this.ProgressBarGamme.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
	}
}