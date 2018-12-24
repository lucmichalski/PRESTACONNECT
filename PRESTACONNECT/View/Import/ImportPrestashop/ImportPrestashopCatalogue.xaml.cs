using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace PRESTACONNECT
{
	/// <summary>
    /// Logique d'interaction pour ImportPrestashopCatalogue.xaml
	/// </summary>
	public partial class ImportPrestashopCatalogue : Window
	{

        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public Semaphore Semaphore = new Semaphore(1, 1);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ImportPrestashopCatalogue()
        {
            this.InitializeComponent();
            Model.Prestashop.PsCategoryRepository PsCategoryRepository = new Model.Prestashop.PsCategoryRepository();
            List<UInt32> ListPsCategory = PsCategoryRepository.ListIdOrderByLevelDepth(Core.Global.CurrentShop.IDShop, PsCategoryRepository.ReadId(Core.Global.CurrentShop.IDCategory).LevelDepth);
            this.ListCount = ListPsCategory.Count;
            this.Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
            this.ReportProgress(0);

            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListPsCategory, this.ParallelOptions, Sync);
            });
        }

        public void Sync(UInt32 Category)
        {
            this.Semaphore.WaitOne();
            Core.ImportPrestashop.ImportCatalogue Sync = new Core.ImportPrestashop.ImportCatalogue();
            Sync.Exec(Convert.ToInt32(Category));
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
                this.ProgressBarCatalogue.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
    }
}