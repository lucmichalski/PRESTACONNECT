using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour ImportSageArticleDocument.xaml
    /// </summary>
    public partial class ImportSageArticleDocument : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        private String _DirDoc;

        public String DirDoc
        {
            get { return this._DirDoc; }
            set { this._DirDoc = value; }
        }


        public ImportSageArticleDocument(String DirDoc)
        {
            this.DirDoc = DirDoc;
            this.InitializeComponent();
            String[] Files = System.IO.Directory.GetFiles(this.DirDoc);
            this.ListCount = Files.Length;
            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Files, this.ParallelOptions, Sync);
            });
        }

        public void Sync(string File)
        {
            this.Semaphore.WaitOne();

            try
            {
                int Article = 0;
                string extension = Path.GetExtension(File).ToLower();
                string filename = Path.GetFileNameWithoutExtension(File);

                Model.Local.MediaAssignmentRuleRepository MediaAssignmentRuleRepository = new Model.Local.MediaAssignmentRuleRepository();
                List<Model.Local.MediaAssignmentRule> list = MediaAssignmentRuleRepository.List();
                if (list.Count(r => filename.EndsWith(r.SuffixText)) > 0)
                {
                    foreach (Model.Local.MediaAssignmentRule mediarule in list.Where(r => filename.EndsWith(r.SuffixText)))
                    {
                        if (filename.EndsWith(mediarule.SuffixText))
                        {
                            string ref_art = filename.Substring(0, filename.Length - mediarule.SuffixText.Length);
                            Article = Core.Global.SearchReference(ref_art);
                            if (Article != 0)
                            {
                                switch (mediarule.Rule)
                                {
                                    case (short)Core.Parametres.MediaRule.AsAttachment:

                                        Core.ImportSage.ImportArticleDocument Sync = new Core.ImportSage.ImportArticleDocument();
                                        Sync.Exec(File, Article, (!string.IsNullOrEmpty(mediarule.AssignName) ? mediarule.AssignName : filename), null, null);
                                        break;
                                    case (short)Core.Parametres.MediaRule.AsPicture:
                                        if (Core.Img.imageExtensions.Contains(extension))
                                        {
                                            int position, AttributeArticle;
                                            Core.Global.SearchReference(filename, out position, out AttributeArticle);
                                            Core.ImportSage.ImportArticleImage ImportImage = new Core.ImportSage.ImportArticleImage();
                                            ImportImage.Exec(File, Article, position, AttributeArticle);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Article = Core.Global.SearchReference(filename);
                    if (Article != 0)
                    {
                        if (Core.Img.imageExtensions.Contains(extension))
                        {
                            // Unactive import
                            //Core.ImportSage.ImportArticleImage ImportImage = new Core.ImportSage.ImportArticleImage();
                            //ImportImage.Exec(File, Article);
                        }
                        else
                        {
                            Core.ImportSage.ImportArticleDocument Sync = new Core.ImportSage.ImportArticleDocument();
                            Sync.Exec(File, Article);
                        }
                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }

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
                this.ProgressBarArticleDocument.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }


    }
}