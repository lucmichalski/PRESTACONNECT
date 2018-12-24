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
    /// Logique d'interaction pour ImportPrestashopArticleDocument.xaml
    /// </summary>
    public partial class ImportPrestashopArticleDocument : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ImportPrestashopArticleDocument()
        {
            this.InitializeComponent();
            Model.Prestashop.PsProductAttachmentRepository PsProductAttachmentRepository = new Model.Prestashop.PsProductAttachmentRepository();
            List<Model.Prestashop.PsProductAttachment> ListPsProductAttachment = PsProductAttachmentRepository.List();

            Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
            List<Int32?> ListAttachment = AttachmentRepository.List().Select(a => a.Pre_Id).ToList();
            ListPsProductAttachment = ListPsProductAttachment.Where(i => ListAttachment.Count(a => a != null && a.Value == i.IDAttachment) == 0).ToList();

            this.ListCount = ListPsProductAttachment.Count;
            this.Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListPsProductAttachment, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Model.Prestashop.PsProductAttachment PsProductAttachment)
        {
            this.Semaphore.WaitOne();
            Core.ImportPrestashop.ImportArticleDocument Sync = new Core.ImportPrestashop.ImportArticleDocument();
            Sync.Exec(PsProductAttachment);
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
                this.ProgressBarDocument.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
    }
}