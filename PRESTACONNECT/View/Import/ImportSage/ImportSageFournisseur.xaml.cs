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
    /// Logique d'interaction pour ImportSageFournisseur.xaml
	/// </summary>
	public partial class ImportSageFournisseur : Window
	{
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ImportSageFournisseur()
        {
            this.InitializeComponent();
            Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
            List<Int32> ListF_COMPTET = F_COMPTETRepository.ListIdType(1);
            this.ListCount = ListF_COMPTET.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListF_COMPTET, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Int32 F_COMPTET)
        {
            this.Semaphore.WaitOne();
            Core.ImportSage.ImportFournisseur Sync = new Core.ImportSage.ImportFournisseur();
            Sync.Exec(F_COMPTET);
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
                this.ProgressBarFournisseur.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
	}
}