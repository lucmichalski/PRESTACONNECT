using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace PRESTACONNECT
{
	/// <summary>
    /// Logique d'interaction pour ControlArticle.xaml
	/// </summary>
	public partial class ControlArticle : Window
	{
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        Boolean Active;

        public ControlArticle(Boolean Actif)
        {
            this.InitializeComponent();

            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            List<Int32> ListArticle = ArticleRepository.ListId();
            this.ListCount = ListArticle.Count;

            this.Active = Actif;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticle, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Int32 ArticleSend)
        {
            this.Semaphore.WaitOne();

            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
                if (Article.Pre_Id != null && !PsProductRepository.ExistId((uint)Article.Pre_Id))
                {
                    Article.Pre_Id = null;
                    Article.Art_Active = this.Active;
                    ArticleRepository.Save();
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
                    this.Close();
                }
            }, null);
        }
    }
}