using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PRESTACONNECT.Core.Transfert
{
    public class TransfertArticleDocument
    {
        public void Exec(Int32 ArticleSend)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                // If the catalog is sync with Prestashop
                if (Article.Catalog.Pre_Id != null || Article.Catalog.Pre_Id != 0)
                {
                    Model.Prestashop.PsProductRepository ProductRepository = new Model.Prestashop.PsProductRepository();
                    Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
                    // If the Article have a connection with Prestashop
                    if (Article.Pre_Id != null)
                    {
                        if (ProductRepository.ExistId(Convert.ToUInt32(Article.Pre_Id.Value)))
                        {
                            this.ExecLocalToDistant(Article);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        //<YH> 23/08/2012
        private void ExecLocalToDistant(Model.Local.Article Article)
        {
            try
            {
                Model.Prestashop.PsAttachmentRepository PsAttachmentRepository = new Model.Prestashop.PsAttachmentRepository();
                Model.Prestashop.PsAttachment PsAttachment;

                Model.Prestashop.PsAttachmentLangRepository PsAttachmentLangRepository = new Model.Prestashop.PsAttachmentLangRepository();
                Model.Prestashop.PsAttachmentLang PsAttachmentLang;

                Model.Prestashop.PsProductAttachmentRepository PsProductAttachmentRepository = new Model.Prestashop.PsProductAttachmentRepository();
                Model.Prestashop.PsProductAttachment PsProductAttachment;

                Boolean isAttachmentLang = false;

                String FTP = Core.Global.GetConfig().ConfigFTPIP;
                String User = Core.Global.GetConfig().ConfigFTPUser;
                String Password = Core.Global.GetConfig().ConfigFTPPassword;

                Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                List<Model.Local.Attachment> ListAttachment = AttachmentRepository.ListArticle(Article.Art_Id);
                foreach (Model.Local.Attachment Attachment in ListAttachment)
                {
                    try
                    {
                        if (Attachment.Pre_Id == null)
                        {
                            String PathAttachment = Path.Combine(Global.GetConfig().Folders.RootAttachment, Attachment.Att_File);
                            if (System.IO.File.Exists(PathAttachment))
                            {

                                #region infos document joint
                                PsAttachment = new Model.Prestashop.PsAttachment();
                                PsAttachment.File = Attachment.Att_File;
                                PsAttachment.FileName = Attachment.Att_FileName;
                                PsAttachment.Mime = Attachment.Att_Mime;
                                PsAttachmentRepository.Add(PsAttachment);

                                isAttachmentLang = false;
                                PsAttachmentLang = new Model.Prestashop.PsAttachmentLang();
                                if (PsAttachmentLangRepository.ExistAttachmentLang(PsAttachment.IDAttachment, Core.Global.Lang))
                                {
                                    PsAttachmentLang = PsAttachmentLangRepository.ReadAttachmentLang(PsAttachment.IDAttachment, Core.Global.Lang);
                                    isAttachmentLang = true;
                                }
                                PsAttachmentLang.Name = Attachment.Att_Name;
                                PsAttachmentLang.Description = Attachment.Att_Description;
                                if (isAttachmentLang == true)
                                {
                                    PsAttachmentLangRepository.Save();
                                }
                                else
                                {
                                    PsAttachmentLang.IDAttachment = PsAttachment.IDAttachment;
                                    PsAttachmentLang.IDLang = Core.Global.Lang;
                                    PsAttachmentLangRepository.Add(PsAttachmentLang);
                                } 
                                #endregion

                                // <JG> 24/05/2013 ajout insertion autres langues actives si non renseignées
                                #region Multi-langues
                                try
                                {
                                    Model.Prestashop.PsLangRepository PsLangRepository = new Model.Prestashop.PsLangRepository();
                                    foreach (Model.Prestashop.PsLang PsLang in PsLangRepository.ListActive(1, Global.CurrentShop.IDShop))
                                    {
                                        if (!PsAttachmentLangRepository.ExistAttachmentLang(PsAttachment.IDAttachment, PsLang.IDLang))
                                        {
                                            PsAttachmentLang = new Model.Prestashop.PsAttachmentLang()
                                            {
                                                IDAttachment = PsAttachment.IDAttachment,
                                                IDLang = PsLang.IDLang,
                                                Name = Attachment.Att_Name,
                                                Description = Attachment.Att_Description
                                            };
                                            PsAttachmentLangRepository.Add(PsAttachmentLang);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Core.Error.SendMailError(ex.ToString());
                                } 
                                #endregion

                                // affectation au produit
                                PsProductAttachment = new Model.Prestashop.PsProductAttachment();
                                if (PsProductAttachmentRepository.ExistProductAttachment(Convert.ToUInt32(Article.Pre_Id), PsAttachment.IDAttachment) == false)
                                {
                                    PsProductAttachment.IDProduct = Convert.ToUInt32(Article.Pre_Id);
                                    PsProductAttachment.IDAttachment = PsAttachment.IDAttachment;
                                    PsProductAttachmentRepository.Add(PsProductAttachment);
                                }

                                try
                                {
                                    #region upload

                                    string ftpfullpath = FTP + "/download/" + Attachment.Att_File;
                                    System.Net.FtpWebRequest ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                                    ftp.Credentials = new System.Net.NetworkCredential(User, Password);
                                    //userid and password for the ftp server to given  

                                    ftp.UseBinary = true;
                                    ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                                    System.IO.FileStream fs = System.IO.File.OpenRead(PathAttachment);
                                    byte[] buffer = new byte[fs.Length];
                                    fs.Read(buffer, 0, buffer.Length);
                                    fs.Close();
                                    System.IO.Stream ftpstream = ftp.GetRequestStream();
                                    ftpstream.Write(buffer, 0, buffer.Length);
                                    ftpstream.Close();
                                    ftp.Abort();

                                    #endregion

                                    #region update Product field cache_as_attachements

                                    Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
                                    Model.Prestashop.PsProduct PsProduct = PsProductRepository.ReadId(PsProductAttachment.IDProduct);
                                    if (PsProduct.CacheHasAttachments == 0)
                                    {
                                        PsProduct.CacheHasAttachments = (sbyte)1;
                                        PsProductRepository.Save();
                                    }

                                    #endregion

                                    Attachment.Pre_Id = Convert.ToInt32(PsAttachment.IDAttachment);
                                    AttachmentRepository.Save();
                                }
                                catch (Exception ex)
                                {
                                    Core.Error.SendMailError("[UPLOAD FTP DOCUMENT]<br />" + ex.ToString());
                                    PsProductAttachmentRepository.Delete(PsProductAttachmentRepository.ListAttachment(PsAttachment.IDAttachment));
                                    PsAttachmentLangRepository.Delete(PsAttachmentLangRepository.ListAttachment(PsAttachment.IDAttachment));
                                    PsAttachmentRepository.Delete(PsAttachment);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Error.SendMailError("[SYNCHRO DOCUMENT ARTICLE]<br />" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
