using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace PRESTACONNECT
{
	/// <summary>
    /// Logique d'interaction pour NettoyageImage.xaml
	/// </summary>
	public partial class NettoyageImage : Window
	{
		public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public Semaphore Semaphore = new Semaphore(1, 1);

        public NettoyageImage()
        {
            this.InitializeComponent();
            Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();
            List<UInt32> ListPsImage = PsImageRepository.ListId(Core.Global.CurrentShop.IDShop);
            this.ListCount = ListPsImage.Count;
            this.Context = SynchronizationContext.Current;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListPsImage, Clean);
            });
        }

        public void Clean(UInt32 Image)
        {
            this.Semaphore.WaitOne();
            Core.Tools.NettoyageImage Clean = new Core.Tools.NettoyageImage();
            Clean.Exec(Convert.ToInt32(Image));
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
                    System.Threading.Thread.Sleep(1000);
                    this.Close();
                }
            }, null);
        }
    }
}