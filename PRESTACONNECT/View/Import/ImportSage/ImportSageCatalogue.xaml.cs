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
    /// Logique d'interaction pour ImportSageCatalogue.xaml
	/// </summary>
	public partial class ImportSageCatalogue : Window
	{
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public Semaphore Semaphore = new Semaphore(1, 1);
        private ParallelOptions ParallelOptions = new ParallelOptions();
        public List<Int32> ListCatalogue;

        public ImportSageCatalogue(List<Int32> ListCatalogueSync)
        {
            this.InitializeComponent();
            this.ListCatalogue = ListCatalogueSync;
            Model.Sage.F_CATALOGUERepository F_CATALOGUERepository = new Model.Sage.F_CATALOGUERepository();
            List<Int32> ListF_CATALOGUE = F_CATALOGUERepository.ListIdOrderByNiveauIntitule();
            this.ListCount = ListF_CATALOGUE.Count;

            this.Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListF_CATALOGUE, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Int32 Catalogue)
        {
            this.Semaphore.WaitOne();
            if (this.ListCatalogue.Contains(Catalogue))
            {
                Core.ImportSage.ImportCatalogue Sync = new Core.ImportSage.ImportCatalogue();
                List<string> log;
                Sync.Exec(Catalogue, out log);
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
            this.Context.Post(state =>
            {
                this.ProgressBarCatalog.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }

	}
}