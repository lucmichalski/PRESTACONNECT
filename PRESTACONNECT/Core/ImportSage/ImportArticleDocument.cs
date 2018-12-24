using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.ImportSage
{
    public class ImportArticleDocument
    {
        public Boolean Exec(String PathDoc, Int32 ArticleSend)
        {
            return Exec(PathDoc, ArticleSend, null, null, null);
        }
        public Boolean Exec(String PathDoc, Int32 ArticleSend, String Name, String Description, int? cbMarqSageMedia)
        {
            Boolean result = false;
            Int32 IdArticleDocument = 0;
            try
            {
                Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                Model.Local.Attachment Attachment = new Model.Local.Attachment();

                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);

                Attachment.Att_File = Core.Global.GetRandomHexNumber(40).ToLower();

                String[] ArrayFileName = PathDoc.Split('\\');
                Attachment.Att_FileName = ArrayFileName[ArrayFileName.Length - 1];
                if (AttachmentRepository.ExistFileArticle(Attachment.Att_FileName, ArticleSend) == false)
                {
                    string name = (!string.IsNullOrWhiteSpace(Name)) ? Name : Attachment.Att_FileName;
                    Attachment.Att_Name = (name.Length > 32) ? name.Substring(0, 32) : name;
                    string description = (!string.IsNullOrWhiteSpace(Description)) ? Description : name;
                    Attachment.Att_Description = description;

                    Attachment.Att_Mime = Attachment.GetMimeType(Attachment.Att_FileName);

                    Attachment.Art_Id = Article.Art_Id;
                    Attachment.Sag_Id = cbMarqSageMedia;
                    AttachmentRepository.Add(Attachment);
                    IdArticleDocument = Attachment.Att_Id;

                    this.CopyFile(Attachment, AttachmentRepository, Global.GetConfig().Folders.RootAttachment, PathDoc);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
                if (ex.ToString().Contains("System.UnauthorizedAccessException") && IdArticleDocument != 0)
                {
                    Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                    AttachmentRepository.Delete(AttachmentRepository.ReadAttachment(IdArticleDocument));
                }
            }
            return result;
        }

        public void CopyFile(Model.Local.Attachment Attachment, Model.Local.AttachmentRepository AttachementRepository, String DirAttachment, String PathDoc)
        {
            String Dir = DirAttachment + Attachment.Att_File;
            Dir = Dir.Replace("File:///", "");
            Dir = Dir.Replace("file:///", "");
            Dir = Dir.Replace("File://", "\\\\");
            Dir = Dir.Replace("file://", "\\\\");
            Dir = Dir.Replace("/", "\\");
            String Uri = PathDoc;
            Uri = Uri.Replace("File:///", "");
            Uri = Uri.Replace("file:///", "");
            Uri = Uri.Replace("File://", "\\\\");
            Uri = Uri.Replace("file://", "\\\\");
            Uri = Uri.Replace("/", "\\");
            if (System.IO.File.Exists(Dir))
            {
                Attachment.Att_File = Core.Global.GetRandomHexNumber(40).ToLower();
                AttachementRepository.Save();
                this.CopyFile(Attachment, AttachementRepository, DirAttachment, PathDoc);
            }
            else
            {
                System.IO.File.Copy(Uri, Dir);
            }
        }
    }
}
