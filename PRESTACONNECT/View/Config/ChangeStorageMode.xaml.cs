using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.IO;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour ChangeStorageMode.xaml
    /// </summary>
    public partial class ChangeStorageMode : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public ChangeStorageMode()
        {
            this.InitializeComponent();

            Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
            List<Model.Local.ArticleImage> ListImage = ArticleImageRepository.List();
            this.ListCount = ListImage.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListImage, this.ParallelOptions, ExecImage);
            });
        }

        public void ExecImage(Model.Local.ArticleImage ArticleImage)
        {
            this.Semaphore.WaitOne();

            try
            {
                string localPath = ArticleImage.advanced_folder;

                String extension = Path.GetExtension(ArticleImage.ImaArt_Image);
                String FileName = String.Format("{0}" + extension, ArticleImage.ImaArt_Id);
                String PathImgTmp = System.IO.Path.Combine(Core.Global.GetConfig().Folders.TempArticle, FileName);
                if (System.IO.File.Exists(PathImgTmp))
                    File.Move(PathImgTmp, System.IO.Path.Combine(localPath, FileName));

                String PathImgSmall = System.IO.Path.Combine(Core.Global.GetConfig().Folders.SmallArticle, FileName);
                if(System.IO.File.Exists(PathImgSmall))
                    File.Move(PathImgSmall, System.IO.Path.Combine(localPath, FileName.Replace(extension, "_small" + extension)));

                List<String> FileList = System.IO.Directory.GetFiles(Core.Global.GetConfig().Folders.RootArticle, (ArticleImage.ImaArt_Id + "-*")).ToList();
                foreach(String file in FileList)
                    File.Move(file, file.Replace(Core.Global.GetConfig().Folders.RootArticle, localPath));
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
                this.ProgressBar.Value = Percentage;
                this.LabelInformation.Content = "URL des catalogues : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
    }
}