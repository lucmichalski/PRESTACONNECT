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

namespace PRESTACONNECT
{
	/// <summary>
    /// Logique d'interaction pour SynchronisationCatalogueInfoLibre.xaml.xaml
	/// </summary>
	public partial class SynchronisationCatalogueInfoLibre : Window
	{
        private BackgroundWorker worker = new BackgroundWorker();
        public Int32 CurrentCount = 0;

		public SynchronisationCatalogueInfoLibre()
		{
            this.InitializeComponent();
            this.worker.WorkerReportsProgress = true;

            // Insérez le code requis pour la création d’objet sous ce point.this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                List<Int32> ListArticle = ArticleRepository.ListIdSync(true);

                foreach (Int32 Article in ListArticle)
                {
                    Core.Import.ImportCatalogueInfoLibre Sync = new Core.Import.ImportCatalogueInfoLibre();
                    Sync.Exec(Article);
                    lock (this)
                    {
                        this.CurrentCount += 1;
                    }
                    this.worker.ReportProgress((this.CurrentCount * 100 / ListArticle.Count));
                }
            };

            this.worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
            {
                this.ProgressBarStock.Value = args.ProgressPercentage;
                this.LabelInformation.Content = "Informations : " + args.ProgressPercentage + " %";
            };

            this.worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                this.Close();
            };

            // Insérez le code requis pour la création d’objet sous ce point.
            this.worker.RunWorkerAsync();
		}

        public SynchronisationCatalogueInfoLibre(List<int> ListArticle)
        {
            this.InitializeComponent();
            this.worker.WorkerReportsProgress = true;

            // Insérez le code requis pour la création d’objet sous ce point.this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {

                foreach (Int32 Article in ListArticle)
                {
                    Core.Import.ImportCatalogueInfoLibre Sync = new Core.Import.ImportCatalogueInfoLibre();
                    Sync.Exec(Article);
                    lock (this)
                    {
                        this.CurrentCount += 1;
                    }
                    this.worker.ReportProgress((this.CurrentCount * 100 / ListArticle.Count));
                }
            };

            this.worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
            {
                this.ProgressBarStock.Value = args.ProgressPercentage;
                this.LabelInformation.Content = "Informations : " + args.ProgressPercentage + " %";
            };

            this.worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                this.Close();
            };

            // Insérez le code requis pour la création d’objet sous ce point.
            this.worker.RunWorkerAsync();
        }
	}
}