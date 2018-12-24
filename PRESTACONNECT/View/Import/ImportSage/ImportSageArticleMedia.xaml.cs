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
    /// Logique d'interaction pour ImportSageArticleMedia.xaml
    /// </summary>
    public partial class ImportSageArticleMedia : Window
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

        public ImportSageArticleMedia(String DirDoc)
        {
            this.DirDoc = DirDoc;
            this.InitializeComponent();
            List<int> ListArticles = new Model.Local.ArticleRepository().ListId();
            this.ListCount = ListArticles.Count;
            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListArticles, this.ParallelOptions, Sync);
            });
        }

        public void Sync(int IdArticle)
        {
            this.Semaphore.WaitOne();

            try
            {
                Model.Sage.F_ARTICLEMEDIARepository F_ARTICLEMEDIARepository = new Model.Sage.F_ARTICLEMEDIARepository();
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = new Model.Local.Article();
                if (ArticleRepository.ExistArticle(IdArticle))
                {
                    Article = ArticleRepository.ReadArticle(IdArticle);

                    // <JG> 24/03/2015 ajout option suppression auto 
                    if (Core.Global.GetConfig().ImportMediaAutoDeleteAttachment)
                    {
                        Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                        if (AttachmentRepository.ExistArticle(IdArticle))
                        {
                            List<Model.Local.Attachment> ListArticle = AttachmentRepository.ListArticle(IdArticle);
                            ListArticle = ListArticle.Where(at => at.Sag_Id != null).ToList();

                            foreach (Model.Local.Attachment Attachment in ListArticle)
                            {
                                if (!F_ARTICLEMEDIARepository.Exist(Attachment.Sag_Id.Value))
                                {
                                    if (System.IO.File.Exists(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootAttachment, Attachment.Att_File)))
                                        File.Delete(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootAttachment, Attachment.Att_File));

                                    if (Attachment.Pre_Id != null && Attachment.Pre_Id > 0)
                                    {
                                        // Suppression de l'occurence du document sur prestashop 
                                        Model.Prestashop.PsAttachmentRepository psAttachmentRepository = new Model.Prestashop.PsAttachmentRepository();
                                        Model.Prestashop.PsAttachmentLangRepository psAttachmentLangRepository = new Model.Prestashop.PsAttachmentLangRepository();
                                        Model.Prestashop.PsProductAttachmentRepository psProductAttachmentRepository = new Model.Prestashop.PsProductAttachmentRepository();

                                        Model.Prestashop.PsAttachment psAttachment = psAttachmentRepository.ReadAttachment(Convert.ToUInt32(Attachment.Pre_Id.Value));

                                        string distant_file = string.Empty;
                                        if (psAttachment != null)
                                        {
                                            distant_file = psAttachment.File;
                                            psProductAttachmentRepository.Delete(psProductAttachmentRepository.ListAttachment(psAttachment.IDAttachment));
                                            psAttachmentLangRepository.Delete(psAttachmentLangRepository.ListAttachment(psAttachment.IDAttachment));
                                            psAttachmentRepository.Delete(psAttachment);
                                        }

                                        if (Core.Global.GetConfig().ConfigFTPActive)
                                        {
                                            String FTP = Core.Global.GetConfig().ConfigFTPIP;
                                            String User = Core.Global.GetConfig().ConfigFTPUser;
                                            String Password = Core.Global.GetConfig().ConfigFTPPassword;

                                            string ftpfullpath = FTP + "/download/" + distant_file;

                                            if (Core.Ftp.ExistFile(ftpfullpath, User, Password))
                                            {
                                                try
                                                {
                                                    System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                                                    request.Credentials = new System.Net.NetworkCredential(User, Password);
                                                    request.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                                                    request.UseBinary = true;
                                                    request.UsePassive = true;
                                                    request.KeepAlive = false;

                                                    System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();
                                                    response.Close();
                                                }
                                                catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
                                            }
                                        }
                                    }

                                    AttachmentRepository.Delete(Attachment);
                                }
                            }
                        }
                    }

                    if (F_ARTICLEMEDIARepository.ExistReference(Article.Art_Ref))
                    {
                        foreach (Model.Sage.F_ARTICLEMEDIA F_ARTICLEMEDIA in F_ARTICLEMEDIARepository.ListReference(Article.Art_Ref))
                        {
                            String File = (System.IO.File.Exists(F_ARTICLEMEDIA.ME_Fichier))
                                        ? F_ARTICLEMEDIA.ME_Fichier
                                        : Path.Combine(DirDoc, F_ARTICLEMEDIA.ME_Fichier.Substring(2));
                            if (System.IO.File.Exists(File))
                            {
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
                                            switch (mediarule.Rule)
                                            {
                                                case (short)Core.Parametres.MediaRule.AsAttachment:
                                                    Core.ImportSage.ImportArticleDocument Sync = new Core.ImportSage.ImportArticleDocument();
                                                    Sync.Exec(File, Article.Art_Id, (!string.IsNullOrEmpty(mediarule.AssignName) ? mediarule.AssignName : F_ARTICLEMEDIA.ME_Commentaire), null, F_ARTICLEMEDIA.cbMarq);
                                                    break;
                                                case (short)Core.Parametres.MediaRule.AsPicture:
                                                    if (Core.Img.imageExtensions.Contains(extension))
                                                    {
                                                        int position, AttributeArticle;
                                                        Core.Global.SearchReference(filename, out position, out AttributeArticle);
                                                        Core.ImportSage.ImportArticleImage ImportImage = new Core.ImportSage.ImportArticleImage();
                                                        ImportImage.Exec(File, Article.Art_Id, position, AttributeArticle);
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Core.Img.imageExtensions.Contains(extension))
                                    {
                                        if (Core.Global.GetConfig().ImportMediaIncludePictures)
                                        {
                                            int position, AttributeArticle;
                                            Core.Global.SearchReference(filename, out position, out AttributeArticle);
                                            Core.ImportSage.ImportArticleImage ImportImage = new Core.ImportSage.ImportArticleImage();
                                            ImportImage.Exec(File, Article.Art_Id, position, AttributeArticle);
                                        }
                                    }
                                    else
                                    {
                                        Core.ImportSage.ImportArticleDocument Sync = new Core.ImportSage.ImportArticleDocument();
                                        Sync.Exec(File, Article.Art_Id, F_ARTICLEMEDIA.ME_Commentaire, null, F_ARTICLEMEDIA.cbMarq);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IM] " + ex.ToString());
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