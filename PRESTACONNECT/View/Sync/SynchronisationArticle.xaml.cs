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
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour SynchronisationArticle.xaml
    /// </summary>
    /// 
    public partial class SynchronisationArticle : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public SynchronisationArticle()
        {
            this.InitializeComponent();
            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            List<Int32> ListArticle = ArticleRepository.ListIdSyncOrderByPack(true);
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

        public SynchronisationArticle(Int32 ArticleSend)
        {
            this.InitializeComponent();
            List<Int32> ListArticle = new List<Int32>();

            // récupération des éléments du pack 
            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
            if (Article.Art_Pack == true)
            {
                List<Int32> PackElements;
                ReadPackElements(ArticleRepository, Article, out PackElements);
                ListArticle.AddRange(PackElements);
            }
            ListArticle.Add(ArticleSend);

            this.ListCount = ListArticle.Count;

            Core.Temp.LoadListesClients();

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Sync);
            });
        }
        public SynchronisationArticle(List<Int32> ArticlesSend)
        {
            this.InitializeComponent();
            List<Int32> ListArticle = new List<Int32>();

            // récupération des éléments du pack 
            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            foreach (Int32 ArticleSend in ArticlesSend)
            {
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                if (Article.Art_Pack == true)
                {
                    List<Int32> PackElements;
                    ReadPackElements(ArticleRepository, Article, out PackElements);
                    ListArticle.AddRange(PackElements);
                }
                ListArticle.Add(ArticleSend);
            }

            this.ListCount = ListArticle.Count;

            Core.Temp.LoadListesClients();

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Sync);
            });
        }

        /// <summary>
        /// ReadPackElements
        /// </summary>
        /// <param name="ArticleRepository"></param>
        /// <param name="Article"></param>
        /// <param name="PackElements"></param>
        /// <remarks>Récupération des id des éléments du pack</remarks>
        internal static void ReadPackElements(Model.Local.ArticleRepository ArticleRepository, Model.Local.Article Article, out List<Int32> PackElements)
        {
            PackElements = new List<Int32>();
            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
            Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);
            Model.Sage.F_ARTICLE F_ARTICLENOMENCLAT;
            Model.Local.Article ArticleNomenclat;
            foreach (Model.Sage.F_NOMENCLAT F_NOMENCLAT in new Model.Sage.F_NOMENCLATRepository().ListRef(F_ARTICLE.AR_Ref))
            {
                if (F_ARTICLERepository.ExistReference(F_NOMENCLAT.NO_RefDet))
                {
                    F_ARTICLENOMENCLAT = F_ARTICLERepository.ReadReference(F_NOMENCLAT.NO_RefDet);
                    if (ArticleRepository.ExistSag_Id(F_ARTICLENOMENCLAT.cbMarq)
							&& F_ARTICLENOMENCLAT.AR_SuiviStock != (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_SuiviStock.Aucun)
							// pour ne pas prendre en compte les articles non suivi en stock
					{
						ArticleNomenclat = ArticleRepository.ReadSag_Id(F_ARTICLENOMENCLAT.cbMarq);
                        if (ArticleNomenclat.Art_Sync == true
                            || (ArticleNomenclat.Pre_Id != null && ArticleNomenclat.Pre_Id.Value != 0))
                        {
                            if (ArticleNomenclat.Art_Pack)
                            {
                                List<Int32> PackPackElements = new List<Int32>();
                                ReadPackElements(ArticleRepository, ArticleNomenclat, out PackPackElements);
                                PackElements.AddRange(PackPackElements);
                            }
                            PackElements.Add(ArticleNomenclat.Art_Id);
                        }
                    }
                }
            }
        }

        public void Sync(Int32 Article)
        {
            this.Semaphore.WaitOne();
            uint pre_id = 0;
            try
            {
                Core.Sync.SynchronisationArticle Sync = new Core.Sync.SynchronisationArticle();
                Sync.Exec(Article, out pre_id);

                if (pre_id != 0 && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CronArticleURL))
                {
                    string url = Core.Global.GetConfig().CronArticleURL.Replace(Core.Global.GetConfig().CronArticleBalise, pre_id.ToString());
                    Core.Global.LaunchCron(url, Core.Global.GetConfig().CronArticleTimeout);
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
            Context.Post(state =>
            {
                this.ProgressBarArticle.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";

                if (this.CurrentCount == this.ListCount)
                {
                    if (!Core.Global.GetConfig().ConfigRefreshTempCustomerListDisabled)
                        Core.Temp.InitListesClients();

                    if (Core.Temp.SyncArticle_LaunchIndex)
                        Core.Global.LaunchCron(Core.Global.GetConfig().ConfigCronSynchroArticleURL);

                    Core.Temp.SyncArticle_LaunchIndex = false;

                    this.Close();
                }
            }, null);
        }
    }
}