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
    /// Logique d'interaction pour ImportSageArticle.xaml
    /// </summary>
    public partial class ImportSageArticle : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public List<string> Logs = new List<string>();

        public List<Int32> ListArticle;
        public Int32 CatId;

        public ImportSageArticle(List<Int32> ListArticleSend, Model.Local.Catalog Catalogue)
        {
            this.InitializeComponent();
            this.ListArticle = ListArticleSend;
            this.CatId = (Catalogue != null) ? Catalogue.Cat_Id : 0;

            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
            List<Model.Sage.F_ARTICLE_Light> ListF_ARTICLE = F_ARTICLERepository.ListLight();

            ListF_ARTICLE = ListF_ARTICLE.AsParallel().Where(a => ListArticleSend.Contains(a.cbMarq)).AsParallel().ToList();

            ListF_ARTICLE = Core.Tools.FiltreImportSage.ImportSageFilter(ListF_ARTICLE);

            List<string> list_arref = ListF_ARTICLE.Select(a => a.AR_Ref).ToList();

            // ajout pré-import gammes / conditionnements
            // !!! traitement en parallèle déconseillé !!!
            // occurences d'énumérés pouvant se recouper entre les articles

            List<short?> gammes = new List<short?>();
            gammes.AddRange(ListF_ARTICLE.Where(a => a.AR_Gamme1 != null && a.AR_Gamme1 != 0).Select(a => a.AR_Gamme1).Distinct());
            gammes.AddRange(ListF_ARTICLE.Where(a => a.AR_Gamme2 != null && a.AR_Gamme2 != 0).Select(a => a.AR_Gamme2).Distinct());

            // import gammes
            ImportSageGamme importsagegamme = new ImportSageGamme(gammes);
            importsagegamme.ShowDialog();

            // import énumérés de gammes
            List<Model.Sage.F_ARTENUMREF> declinaisons = new Model.Sage.F_ARTENUMREFRepository().List().ToList();
			#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
			declinaisons = declinaisons.Where(d => d.AE_Sommeil == 0).ToList();
			#endif

            declinaisons = Core.Tools.FiltreImportSage.ImportSageFilter(declinaisons);

            List<Model.Sage.F_ARTGAMME> enumeres_gammes = new Model.Sage.F_ARTGAMMERepository().List();
            List<int> localAttribute = new Model.Local.AttributeRepository().List().Select(a => a.Sag_Id).AsParallel().ToList();
            enumeres_gammes = enumeres_gammes.AsParallel().Where(e => list_arref.Contains(e.AR_Ref)
                 && !localAttribute.Contains(e.cbMarq)
                 && declinaisons.Count(d => d.AG_No1 == e.AG_No || d.AG_No2 == e.AG_No) > 0).ToList();

            enumeres_gammes = Core.Tools.FiltreImportSage.ImportSageFilter(enumeres_gammes);

            ImportSageGammeEnumere importsagegammeenumere = new ImportSageGammeEnumere(enumeres_gammes);
            importsagegammeenumere.ShowDialog();

            // import conditionnements
            if (Core.Global.GetConfig().ArticleImportConditionnementActif)
            {
                List<short?> conditionnements = ListF_ARTICLE.Where(a => a.AR_Condition != null && a.AR_Condition != 0).Select(a => a.AR_Condition).Distinct().ToList();
                ImportSageConditionnement importsageconditionnement = new ImportSageConditionnement(conditionnements);
                importsageconditionnement.ShowDialog();

                List<Model.Sage.F_CONDITION> enumeres_conditionnement = new Model.Sage.F_CONDITIONRepository().List();
                List<int> localConditioning = new Model.Local.ConditioningRepository().List().Select(c => c.Sag_Id).AsParallel().ToList();
                enumeres_conditionnement = enumeres_conditionnement.AsParallel().Where(e => list_arref.Contains(e.AR_Ref)
                    && !localConditioning.Contains(e.cbMarq)).ToList();

                enumeres_conditionnement = Core.Tools.FiltreImportSage.ImportSageFilter(enumeres_conditionnement);

                ImportSageConditionnementEnumere importsageconditionnementenumere = new ImportSageConditionnementEnumere(enumeres_conditionnement);
                importsageconditionnementenumere.ShowDialog();
            }
            // fin

            this.ListCount = ListF_ARTICLE.Count;
            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListF_ARTICLE, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Model.Sage.F_ARTICLE_Light F_ARTICLE)
        {
            this.Semaphore.WaitOne();

            Core.ImportSage.ImportArticle Sync = new Core.ImportSage.ImportArticle();
            List<string> log;
            Sync.Exec(F_ARTICLE.cbMarq, CatId, out log);
            if (log != null && log.Count > 0)
                lock (this.Logs)
                {
                    Logs.AddRange(log);
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
                    if (!Core.Global.UILaunch && Logs != null && Logs.Count > 0)
                        Core.Log.SendLog(Logs, Core.Log.LogIdentifier.ImportAutoArticle);

                    this.Close();
                }
            }, null);
        }
    }
}