using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour TransfertStockPrice.xaml
    /// </summary>
    public partial class TransfertStockPrice : Window
    {
        List<string> Log_Chrono = new List<string>();

        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;

        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public TransfertStockPrice(List<Int32> ListArticle = null)
        {
            this.InitializeComponent();
            if (ListArticle == null)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                ListArticle = ArticleRepository.ListIdSync(true);
            }
            this.ListCount = ListArticle.Count;

            Core.Temp.LoadListesClients();

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Int32 Article)
        {
            this.Semaphore.WaitOne();
            ReportInfosSynchro(Article, Core.Temp._action_information_synchro.debut);
            uint pre_id = 0;

            List<string> log;
            Core.Transfert.TransfertStockPrice transfert = new Core.Transfert.TransfertStockPrice();
            transfert.Exec(Article, out log, out pre_id);

            if (pre_id != 0 && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CronArticleURL))
            {
                string url = Core.Global.GetConfig().CronArticleURL.Replace(Core.Global.GetConfig().CronArticleBalise, pre_id.ToString());
                Core.Global.LaunchCron(url, Core.Global.GetConfig().CronArticleTimeout);
            }

            lock (this)
            {
                if (Core.Global.GetConfig().ChronoSynchroStockPriceActif && log != null && log.Count > 0)
                    Core.Log.WriteChronoLog(log, Core.Log.LogIdentifier.ChronoSynchroStockPrice);
                this.CurrentCount += 1;
            }
            ReportInfosSynchro(Article, Core.Temp._action_information_synchro.fin);
            this.ReportProgress(this.CurrentCount * 100 / this.ListCount);
            this.Semaphore.Release();
        }

        public void ReportProgress(Int32 Percentage)
        {
            Context.Post(state =>
            {
                this.ProgressBarStockPrice.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";

                if (this.CurrentCount == this.ListCount)
                {
                    if (!Core.Global.GetConfig().ConfigRefreshTempCustomerListDisabled)
                        Core.Temp.InitListesClients();

                    Core.Log.CloseLog(Core.Log.LogStreamType.LogChronoStream);

                    this.Close();
                }
            }, null);
        }

        public void ReportInfosSynchro(Int32 Article, Core.Temp._action_information_synchro action)
        {
            Context.Post(state =>
            {
                string art_ref = new Model.Local.ArticleRepository().ReadArticleProgress(Article).Art_Ref;
                switch (action)
                {
                    case PRESTACONNECT.Core.Temp._action_information_synchro.debut:
                        this.listBoxReference.Items.Add(art_ref);
                        break;
                    case PRESTACONNECT.Core.Temp._action_information_synchro.fin:
                        this.listBoxReference.Items.Remove(art_ref);
                        break;
                    default:
                        break;
                }
            }, null);
        }
    }
}