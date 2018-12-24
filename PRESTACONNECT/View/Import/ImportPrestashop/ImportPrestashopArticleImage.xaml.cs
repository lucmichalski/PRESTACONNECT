using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace PRESTACONNECT
{
	/// <summary>
    /// Logique d'interaction pour ImportPrestashopArticleImage.xaml
	/// </summary>
	public partial class ImportPrestashopArticleImage : Window
	{
		public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ImportPrestashopArticleImage(bool forceImportImage)
        {
            this.InitializeComponent();
            Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();
            List<UInt32> ListPsImage = PsImageRepository.ListId(Core.Global.CurrentShop.IDShop);

            if (!forceImportImage)
            {
                Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                List<Int32?> ListArticleImage = ArticleImageRepository.List().Select(ai => ai.Pre_Id).ToList();
                ListPsImage = ListPsImage.Where(i => ListArticleImage.Count(ai => ai != null && ai.Value == i) == 0).ToList();
            }

            this.ListCount = ListPsImage.Count;
            this.Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListPsImage, this.ParallelOptions, Sync);
            });
        }

        public void Sync(UInt32 Image)
        {
            this.Semaphore.WaitOne();
            Core.ImportPrestashop.ImportArticleImage Sync = new Core.ImportPrestashop.ImportArticleImage();
            Sync.Exec(Convert.ToInt32(Image));
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
                    this.Close();
                }
            }, null);
        }
    }
}