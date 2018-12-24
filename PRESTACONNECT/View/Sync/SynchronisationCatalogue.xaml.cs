using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.ComponentModel;

namespace PRESTACONNECT
{
    public partial class SynchronisationCatalogue : Window
    {
        #region Properties

        private BackgroundWorker loadWorker;
        private BackgroundWorker LoadWorker
        {
            get { return loadWorker; }
            set
            {
                if (loadWorker != null)
                {
                    loadWorker.DoWork -= new DoWorkEventHandler(LoadWorker_DoWork);
                    loadWorker.ProgressChanged -= new ProgressChangedEventHandler(LoadWorker_ProgressChanged);
                    loadWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadWorker_RunWorkerCompleted);
                }

                loadWorker = value;

                if (loadWorker != null)
                {
                    loadWorker.DoWork += new DoWorkEventHandler(LoadWorker_DoWork);
                    loadWorker.ProgressChanged += new ProgressChangedEventHandler(LoadWorker_ProgressChanged);
                    loadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadWorker_RunWorkerCompleted);
                }
            }
        }

        private List<Int32> ListCatalogue;

        #endregion
        #region Constructors

        public SynchronisationCatalogue()
        {
            LoadWorker = new BackgroundWorker();
            LoadWorker.WorkerReportsProgress = true;

            InitializeComponent();
        }

        public SynchronisationCatalogue(List<Int32> ListCataloguesArticles)
        {
            ListCatalogue = ListCataloguesArticles;
            LoadWorker = new BackgroundWorker();
            LoadWorker.WorkerReportsProgress = true;

            InitializeComponent();
        }

        #endregion
        #region Event methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(ListCatalogue != null)
                LoadWorker.RunWorkerAsync(ListCatalogue);
            else
                LoadWorker.RunWorkerAsync();
        }

        private void LoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<int> CatalogIds = new Model.Local.CatalogRepository().ListIdSyncOrderByLevel(true);

            if (CatalogIds != null)
            {
                if(e.Argument != null && e.Argument is List<Int32>)
                {
                    List<Int32> list = (List<Int32>)e.Argument;
                    CatalogIds = CatalogIds.Where(c => list.Count(sc => sc == c) > 0).ToList();
                }

                Core.Sync.SynchronisationCatalogue sync = new Core.Sync.SynchronisationCatalogue();

                int count = CatalogIds.Count;

                for (int i = 0; i < count; i++)
                {
                    sync.Exec(CatalogIds[i]);
                    LoadWorker.ReportProgress((((i + 1) * 100) / count));
                }
            }
        }
        private void LoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarCatalog.Value = e.ProgressPercentage;
            LabelInformation.Content = String.Format("Informations : {0} %", e.ProgressPercentage);
        }
        private void LoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                PRESTACONNECT.Core.Error.SendMailError(e.Error.ToString());

            if (Core.Temp.SyncCatalogue_ClearSmartyCache)
                Core.Global.LaunchAlternetis_ClearSmartyCache();
            if (Core.Temp.SyncCatalogue_RegenerateTree)
                Core.Global.LaunchAlternetis_RegenerateCategoryTree();

            Core.Temp.SyncCatalogue_ClearSmartyCache = false;
            Core.Temp.SyncCatalogue_RegenerateTree = false;

            Close();
        }

        #endregion
    }

    //public partial class SynchronisationCatalogue : Window
    //{

    //    public Int32 CurrentCount = 0;
    //    public Int32 ListCount = 0;
    //    public SynchronizationContext Context;
    //    public Semaphore Semaphore = new Semaphore(1, 4);

    //    public SynchronisationCatalogue()
    //    {
    //        this.InitializeComponent();
    //    }

    //    public void Sync(Int32 Catalog)
    //    {
    //        this.Semaphore.WaitOne();
    //        Core.Sync.SynchronisationCatalogue Sync = new Core.Sync.SynchronisationCatalogue();
    //        Sync.Exec(Catalog);
    //        this.CurrentCount += 1;
    //        this.ReportProgress(this.CurrentCount * 100 / this.ListCount);
    //        this.Semaphore.Release();
    //    }

    //    public void ReportProgress(Int32 Percentage)
    //    {
    //        Context.Post(state =>
    //        {
    //            this.ProgressBarCatalog.Value = Percentage;
    //            this.LabelInformation.Content = "Informations : " + Percentage + " %";
    //            if (this.CurrentCount == this.ListCount)
    //            {
    //                this.Close();
    //            }
    //        }, null);
    //    }

    //    private void Window_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
    //        List<Int32> ListCatalog = CatalogRepository.ListIdSyncOrderByLevel(true);
    //        this.ListCount = ListCatalog.Count;
    //        Context = SynchronizationContext.Current;
    //        this.ReportProgress(0);
    //        Task.Factory.StartNew(() =>
    //        {
    //            Parallel.ForEach(ListCatalog, Sync);
    //        });
    //    }
    //}
}