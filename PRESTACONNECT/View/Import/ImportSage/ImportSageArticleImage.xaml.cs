using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour ImportSageArticleImage.xaml
    /// </summary>
    public partial class ImportSageArticleImage : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public List<string> Logs = new List<string>();

        private String _DirImg;
        public String DirImg
        {
            get { return this._DirImg; }
            set { this._DirImg = value; }
        }

        // <JG> 20/04/2015 désactivation du parallèle afin de respecter les positions et la couverture par rapport à l'ordre de nommage des fichiers
        // à réactiver après ajout de règles de gestion multimages par produit

        //public ImportSageArticleImage(String DirImg)
        //{
        //    this.DirImg = DirImg;
        //    this.InitializeComponent();
        //    String[] Files = System.IO.Directory.GetFiles(this.DirImg);
        //    this.ListCount = Files.Length;
        //    Context = SynchronizationContext.Current;
        //    this.ParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
        //    this.ReportProgress(0);
        //    Task.Factory.StartNew(() =>
        //    {
        //        Parallel.ForEach(Files, this.ParallelOptions, Sync);
        //    });
        //}

        //public void Sync(String File)
        //{
        //    this.Semaphore.WaitOne();
        //    try
        //    {
        //        String extension = Path.GetExtension(File).ToLower();
        //        if (Core.Img.imageExtensions.Contains(extension))
        //        {
        //            String ValuesImg = Path.GetFileNameWithoutExtension(File);
        //            String AR_Ref = ValuesImg;

        //            int position, AttributeArticle;
        //            int Article = Core.Global.SearchReference(ValuesImg, out AR_Ref, out position, out AttributeArticle);
        //            if (Article != 0)
        //            {
        //                Core.ImportSage.ImportArticleImage import = new Core.ImportSage.ImportArticleImage();
        //                import.Exec(File, Article, position, AttributeArticle);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Core.Error.SendMailError("[IMPORT IMAGE SAGE]<br />" + ex.ToString());
        //    }
        //    lock (this)
        //    {
        //        this.CurrentCount += 1;
        //    }
        //    this.ReportProgress(this.CurrentCount * 100 / this.ListCount);
        //    this.Semaphore.Release();
        //}

        //public void ReportProgress(Int32 Percentage)
        //{
        //    Context.Post(state =>
        //    {
        //        this.ProgressBarArticleImage.Value = Percentage;
        //        this.LabelInformation.Content = "Informations : " + Percentage + " %";
        //        if (this.CurrentCount == this.ListCount)
        //        {
        //            this.Close();
        //        }
        //    }, null);
        //}

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
        
        #endregion
        #region Constructors

        public ImportSageArticleImage(String ImportPath)
        {
            this.DirImg = ImportPath;

            LoadWorker = new BackgroundWorker();
            LoadWorker.WorkerReportsProgress = true;

            InitializeComponent();
        }

        #endregion
        #region Event methods
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(LoadWorker != null)
            {
                LoadWorker.RunWorkerAsync();
            }
        }

        private void LoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // parcours des données Sage
            if (Core.Global.GetConfig().ImportImageUseSageDatas)
            {
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                List<Model.Sage.F_ARTICLE_Photo> ListPhoto = F_ARTICLERepository.ListPhoto();

                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                List<int> ListSageLocal = ArticleRepository.ListSageId();

                ListPhoto = ListPhoto.Where(ap => ListSageLocal.Contains(ap.cbMarq)).ToList();

                ListCount = ListPhoto.Count;
                CurrentCount = 0;

                foreach (Model.Sage.F_ARTICLE_Photo F_ARTICLE_Photo in ListPhoto)
                {
                    string file = null;
                    if (!string.IsNullOrWhiteSpace(F_ARTICLE_Photo.AC_Photo))
                    {
                        file = System.IO.Path.Combine(Core.Global.GetConfig().AutomaticImportFolderPicture, F_ARTICLE_Photo.AC_Photo);
                    }
                    else if (!string.IsNullOrWhiteSpace(F_ARTICLE_Photo.AR_Photo))
                    {
                        file = System.IO.Path.Combine(Core.Global.GetConfig().AutomaticImportFolderPicture, F_ARTICLE_Photo.AR_Photo);
                    }
                    if (file != null && System.IO.File.Exists(file))
                    {
                        Model.Local.Article Article = ArticleRepository.ReadSag_Id(F_ARTICLE_Photo.cbMarq);
                        String extension = Path.GetExtension(file).ToLower();
                        if (Core.Img.imageExtensions.Contains(extension))
                        {
                            Core.ImportSage.ImportArticleImage import = new Core.ImportSage.ImportArticleImage();
                            import.Exec(file, Article.Art_Id);
                            if (import.logs != null && import.logs.Count > 0)
                                lock (this.Logs)
                                {
                                    Logs.AddRange(import.logs);
                                }
                            lock (this)
                            {
                                this.CurrentCount += 1;
                                LoadWorker.ReportProgress(this.CurrentCount * 100 / this.ListCount);
                            }
                        }
                    }
                }
            }
            // fonction par défaut = parcours dossier local ou réseau
            else
            {
                String[] Files = System.IO.Directory.GetFiles(this.DirImg);
                this.ListCount = Files.Length;

                foreach (string file in Files)
                {
                    exec(file);
                    lock (this)
                    {
                        this.CurrentCount += 1;
                        LoadWorker.ReportProgress(this.CurrentCount * 100 / this.ListCount);
                    }
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

            if (Logs != null && Logs.Count > 0)
            {
                Core.Log.WriteSpecificLog(Logs, Core.Log.LogIdentifier.ImportAutoImage);
            }

            Close();
        }

        private void exec(string File)
        {
            try
            {
                String extension = Path.GetExtension(File).ToLower();
                if (Core.Img.imageExtensions.Contains(extension))
                {
                    String ValuesImg = Path.GetFileNameWithoutExtension(File);
                    String AR_Ref = ValuesImg;

                    int position, AttributeArticle;
                    int Article = Core.Global.SearchReference(ValuesImg, out AR_Ref, out position, out AttributeArticle);
                    if (Article != 0)
                    {
                        Core.ImportSage.ImportArticleImage import = new Core.ImportSage.ImportArticleImage();
                        import.Exec(File, Article, position, AttributeArticle);
                        if (import.logs != null && import.logs.Count > 0)
                            lock (this.Logs)
                            {
                                Logs.AddRange(import.logs);
                            }
                    }

                    if (Core.Global.GetConfig().ImportImageSearchReferenceClient)
                    {
                        List<int> listID = Core.Global.SearchListReference(ValuesImg);
                        if (listID != null && listID.Count > 0)
                        {
                            foreach (int id_article_local in listID)
                            {
                                Core.ImportSage.ImportArticleImage import = new Core.ImportSage.ImportArticleImage();
                                import.Exec(File, id_article_local);
                                if (import.logs != null && import.logs.Count > 0)
                                    lock (this.Logs)
                                    {
                                        Logs.AddRange(import.logs);
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT IMAGE SAGE]<br />" + ex.ToString());
            }
        }
        #endregion
    }
}