using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PRESTACONNECT.Core.ImportPrestashop
{
    public class ImportArticleDocument
    {
        public void Exec(Model.Prestashop.PsProductAttachment PsProductAttachment)
        {
            try
            {
                //<YH> 21/08/2012
                string DirAttachment = Global.GetConfig().Folders.RootAttachment;

                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                if (ArticleRepository.ExistPre_Id(Convert.ToInt32(PsProductAttachment.IDProduct)))
                {
                    Model.Local.Article Article = ArticleRepository.ReadPre_Id(Convert.ToInt32(PsProductAttachment.IDProduct));
                    Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                    if (AttachmentRepository.ExistPre_IdArt_Id(Convert.ToInt32(PsProductAttachment.IDAttachment), Article.Art_Id) == false)
                    {
                        Model.Prestashop.PsAttachmentRepository PsAttachmentRepository = new Model.Prestashop.PsAttachmentRepository();
                        Model.Prestashop.PsAttachment PsAttachment = PsAttachmentRepository.ReadAttachment(PsProductAttachment.IDAttachment);
                        Model.Prestashop.PsAttachmentLangRepository PsAttachmentLangRepository = new Model.Prestashop.PsAttachmentLangRepository();
                        if (PsAttachmentLangRepository.ExistAttachmentLang(PsAttachment.IDAttachment, Core.Global.Lang))
                        {
                            Model.Prestashop.PsAttachmentLang PsAttachmentLang = PsAttachmentLangRepository.ReadAttachmentLang(PsAttachment.IDAttachment, Core.Global.Lang);
                            Model.Local.Attachment Attachment = new Model.Local.Attachment()
                            {
                                Att_FileName = PsAttachment.FileName,
                                Att_Description = PsAttachmentLang.Description,
                                Att_Mime = PsAttachment.Mime,
                                Att_Name = PsAttachmentLang.Name,
                                Att_File = this.ReadFile(DirAttachment, PsAttachment.File),
                                Pre_Id = Convert.ToInt32(PsAttachment.IDAttachment),
                                Art_Id = Article.Art_Id    
                            };
                            AttachmentRepository.Add(Attachment);

                            String FTP = Core.Global.GetConfig().ConfigFTPIP;
                            String User = Core.Global.GetConfig().ConfigFTPUser;
                            String Password = Core.Global.GetConfig().ConfigFTPPassword;

                            // <JG> 21/05/2013 correction recherche fichier sur le ftp
                            string ftpfullpath = FTP + "/download/" + PsAttachment.File;
                            System.Net.FtpWebRequest ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                            ftp.Credentials = new System.Net.NetworkCredential(User, Password);
                            ftp.UseBinary = true;
                            ftp.UsePassive = true;
                            ftp.KeepAlive = false;
                            ftp.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;

                            System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)ftp.GetResponse();
                            Stream reader = response.GetResponseStream();

                            MemoryStream memStream = new MemoryStream();
                            byte[] buffer = new byte[1024];
                            byte[] downloadedData = new byte[0];
                            while (true)
                            {
                                int bytesRead = reader.Read(buffer, 0, buffer.Length);
                                if (bytesRead != 0)
                                {
                                    memStream.Write(buffer, 0, bytesRead);
                                }
                                else
                                {
                                    break;
                                }
                                downloadedData = memStream.ToArray();
                            }

                            if (downloadedData != null && downloadedData.Length != 0)
                            {
                                FileStream newFile = new FileStream(DirAttachment + Attachment.Att_File, FileMode.Create);
                                newFile.Write(downloadedData, 0, downloadedData.Length);
                                newFile.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public String ReadFile(String Dir, String File)
        {
            if (System.IO.File.Exists(Dir + File))
            {
                File = Core.Global.GetRandomHexNumber(40).ToLower();
                return this.ReadFile(Dir, File);
            }
            else
            {
                return File;
            }
        }
    }
}
