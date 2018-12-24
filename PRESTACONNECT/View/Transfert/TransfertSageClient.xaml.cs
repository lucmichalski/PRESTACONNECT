using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour TransfertSageClient.xaml
    /// </summary>
    public partial class TransfertSageClient : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();
        public List<string> logs = new List<string>();

        public TransfertSageClient(List<Int32> ListClientSync)
        {
            this.InitializeComponent();
            this.ListCount = ListClientSync.Count;

            this.Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListClientSync, this.ParallelOptions, Transfert);
            });
        }

        public void Transfert(Int32 Client)
        {
            this.Semaphore.WaitOne();
            try
            {
                Core.Transfert.TransfertSageClient transfert = new Core.Transfert.TransfertSageClient();
                List<string> log;
                transfert.Exec(Client, out log);
                if (log.Count > 0)
                {
                    log.Add(Core.Log.LogLineSeparator);
                    lock (this.logs)
                    {
                        logs.AddRange(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
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
                this.ProgressBarSage.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    Core.Log.SendLog(this.logs, Core.Log.LogIdentifier.TransfertClient);
                    this.Close();
                }
            }, null);
        }

    }
}