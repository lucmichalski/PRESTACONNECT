using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
	/// <summary>
    /// Logique d'interaction pour ImportPrestashopArticle.xaml
	/// </summary>
	public partial class ImportPrestashopArticle : Window
	{
		public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ImportPrestashopArticle(bool forceImportProduct)
        {
            this.InitializeComponent();
            Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
            List<UInt32> ListPsProduct = PsProductRepository.ListId(Core.Global.CurrentShop.IDShop);

            if (!forceImportProduct)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                List<Int32> listlocal = ArticleRepository.ListPrestashop();
                ListPsProduct = ListPsProduct.Where(ps => !listlocal.Contains((int)ps)).ToList();
            }

            this.ListCount = ListPsProduct.Count;
            this.Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListPsProduct, this.ParallelOptions, Sync);
            });
        }

        public void Sync(UInt32 Product)
        {
            this.Semaphore.WaitOne();
            Core.ImportPrestashop.ImportArticle Sync = new Core.ImportPrestashop.ImportArticle();
            Sync.Exec(Convert.ToInt32(Product));
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