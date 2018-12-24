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
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour SynchronisationCatalogue.xaml
    /// </summary>
    public partial class SynchronisationFournisseur : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public SynchronisationFournisseur()
        {
            this.InitializeComponent();
            Model.Local.SupplierRepository LocalSupplierRepository = new Model.Local.SupplierRepository();
            List<Int32> ListSupplier = LocalSupplierRepository.ListIdSync(true);
            this.ListCount = ListSupplier.Count;
            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListSupplier, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Int32 Supplier)
        {
            this.Semaphore.WaitOne();
            Core.Sync.SynchronisationFournisseur Sync = new Core.Sync.SynchronisationFournisseur();
            Sync.Exec(Supplier);
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