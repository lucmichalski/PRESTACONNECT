using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace PRESTACONNECT
{
    // <YH> 20/08/2012
    public partial class TransfertArticleImage : Window
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

        private BackgroundWorker loadWorkerArticle;
        private BackgroundWorker LoadWorkerArticle
        {
            get { return loadWorkerArticle; }
            set
            {
                if (loadWorkerArticle != null)
                {
                    loadWorkerArticle.DoWork -= new DoWorkEventHandler(LoadWorkerArticle_DoWork);
                    loadWorkerArticle.ProgressChanged -= new ProgressChangedEventHandler(LoadWorker_ProgressChanged);
                    loadWorkerArticle.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadWorker_RunWorkerCompleted);
                }

                loadWorkerArticle = value;

                if (loadWorkerArticle != null)
                {
                    loadWorkerArticle.DoWork += new DoWorkEventHandler(LoadWorkerArticle_DoWork);
                    loadWorkerArticle.ProgressChanged += new ProgressChangedEventHandler(LoadWorker_ProgressChanged);
                    loadWorkerArticle.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadWorker_RunWorkerCompleted);
                }
            }
        }
        private BackgroundWorker loadWorkerListArticle;
        private BackgroundWorker LoadWorkerListArticle
        {
            get { return loadWorkerListArticle; }
            set
            {
                if (loadWorkerListArticle != null)
                {
                    loadWorkerListArticle.DoWork -= new DoWorkEventHandler(LoadWorkerListArticle_DoWork);
                    loadWorkerListArticle.ProgressChanged -= new ProgressChangedEventHandler(LoadWorker_ProgressChanged);
                    loadWorkerListArticle.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadWorker_RunWorkerCompleted);
                }

                loadWorkerListArticle = value;

                if (loadWorkerListArticle != null)
                {
                    loadWorkerListArticle.DoWork += new DoWorkEventHandler(LoadWorkerListArticle_DoWork);
                    loadWorkerListArticle.ProgressChanged += new ProgressChangedEventHandler(LoadWorker_ProgressChanged);
                    loadWorkerListArticle.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadWorker_RunWorkerCompleted);
                }
            }
        }
        private Int32 IdArticle = 0;
        private List<Int32> ListIdArticle = new List<int>();

        #endregion
        #region Constructors

        public TransfertArticleImage()
        {
            LoadWorker = new BackgroundWorker();
            LoadWorker.WorkerReportsProgress = true;

            InitializeComponent();
        }

        public TransfertArticleImage(Int32 ArticleSend)
        {
            LoadWorkerArticle = new BackgroundWorker();
            LoadWorkerArticle.WorkerReportsProgress = true;
            this.IdArticle = ArticleSend;
            InitializeComponent();
        }
        public TransfertArticleImage(List<Int32> ListArticle)
        {
            LoadWorkerListArticle = new BackgroundWorker();
            LoadWorkerListArticle.WorkerReportsProgress = true;
            this.ListIdArticle = ListArticle;
            InitializeComponent();
        }

        #endregion
        #region Event methods
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadWorkerListArticle != null)
            {
                LoadWorkerListArticle.RunWorkerAsync(this.ListIdArticle);
            }
            else if (LoadWorkerArticle != null && new Model.Local.ArticleRepository().ExistArticle(this.IdArticle))
            {
                LoadWorkerArticle.RunWorkerAsync(this.IdArticle);
            }
            else if(LoadWorker != null)
            {
                LoadWorker.RunWorkerAsync();
            }
        }

        private void LoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<int> ArticleIds = new Model.Local.ArticleRepository().ListIdSync(true);

            if (ArticleIds != null)
            {
                Core.Transfert.TransfertArticleImage sync = new Core.Transfert.TransfertArticleImage();

                if (!Core.Global.GetConfig().ConfigImageSynchroPositionLegende)
                {
                    Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                    List<int> listarticlewithimagetosync = ArticleImageRepository.ListIDArticleNotSync();
                    ArticleIds = ArticleIds.Where(a => listarticlewithimagetosync.Contains(a)).ToList();
                }

                int count = ArticleIds.Count;

                for (int i = 0; i < count; i++)
                {
                    sync.Exec(ArticleIds[i]);
                    LoadWorker.ReportProgress((((i + 1) * 100) / count));
                }
            }
        }
        private void LoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarArticleImage.Value = e.ProgressPercentage;
            LabelInformation.Content = String.Format("Informations : {0} %", e.ProgressPercentage);
        }
        private void LoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                PRESTACONNECT.Core.Error.SendMailError(e.Error.ToString());

            Close();
        }

        private void LoadWorkerArticle_DoWork(object sender, DoWorkEventArgs e)
        {
            Int32 IdArticleSend = (Int32)e.Argument;

            Core.Transfert.TransfertArticleImage sync = new Core.Transfert.TransfertArticleImage();

            sync.Exec(IdArticleSend);
            LoadWorkerArticle.ReportProgress(100);
        }
        private void LoadWorkerListArticle_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Int32> ListIdArticleSend = (List<Int32>)e.Argument;

            if (ListIdArticleSend != null)
            {
                Core.Transfert.TransfertArticleImage sync = new Core.Transfert.TransfertArticleImage();

                if (!Core.Global.GetConfig().ConfigImageSynchroPositionLegende)
                {
                    Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                    List<int> listarticlewithimagetosync = ArticleImageRepository.ListIDArticleNotSync();
                    ListIdArticleSend = ListIdArticleSend.Where(a => listarticlewithimagetosync.Contains(a)).ToList();
                }

                int count = ListIdArticleSend.Count;

                for (int i = 0; i < count; i++)
                {
                    sync.Exec(ListIdArticleSend[i]);
                    LoadWorkerListArticle.ReportProgress((((i + 1) * 100) / count));
                }
            }
        }
        #endregion
    }
}