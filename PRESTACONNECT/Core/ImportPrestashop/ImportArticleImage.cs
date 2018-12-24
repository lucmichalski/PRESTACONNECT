using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace PRESTACONNECT.Core.ImportPrestashop
{
    public class ImportArticleImage
    {
        public void Exec(Int32 ImageSend)
        {
            try
            {
                Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                if (ArticleImageRepository.ExistPre_Id(ImageSend) == false)
                {
                    Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();
                    Model.Prestashop.PsImage PsImage = PsImageRepository.ReadImage(Convert.ToUInt32(ImageSend));
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                    if (ArticleRepository.ExistPre_Id(Convert.ToInt32(PsImage.IDProduct)))
                    {
                        Model.Local.Article Article = ArticleRepository.ReadPre_Id(Convert.ToInt32(PsImage.IDProduct));

                        Model.Prestashop.PsImageLangRepository PsImageLangRepository = new Model.Prestashop.PsImageLangRepository();
                        if (PsImageLangRepository.ExistImageLang(Convert.ToUInt32(ImageSend), Core.Global.Lang))
                        {
                            string extension = Core.Img.jpgExtension;
                            String FTP = Core.Global.GetConfig().ConfigFTPIP;
                            String User = Core.Global.GetConfig().ConfigFTPUser;
                            String Password = Core.Global.GetConfig().ConfigFTPPassword;

                            Model.Prestashop.PsImageLang PsImageLang = PsImageLangRepository.ReadImageLang(Convert.ToUInt32(ImageSend), Core.Global.Lang);
                            Model.Local.ArticleImage ArticleImage = new Model.Local.ArticleImage()
                            {
                                ImaArt_Name = (!String.IsNullOrEmpty(PsImageLang.Legend)) ? PsImageLang.Legend : Article.Art_Ref,
                                ImaArt_Position = PsImage.Position,
                                ImaArt_Default = Convert.ToBoolean(PsImage.Cover),
                                Pre_Id = Convert.ToInt32(PsImage.IDImage),
                                Art_Id = Article.Art_Id,
                                ImaArt_Image = string.Empty,
                                ImaArt_SourceFile = SearchFreeNameFile(Article.Art_Id, Article.Art_Ref, extension),
                                ImaArt_DateAdd = DateTime.Now
                            };
                            ArticleImageRepository.Add(ArticleImage);

                            Boolean import_img = false;
                            try
                            {
                                // <JG> 10/04/2013 gestion système d'images
                                string ftpPath = "/img/p/";
                                switch (Core.Global.GetConfig().ConfigImageStorageMode)
                                {
                                    case Core.Parametres.ImageStorageMode.old_system:
                                        #region old_system
                                        // no action on path
                                        break;
                                        #endregion

                                    case Core.Parametres.ImageStorageMode.new_system:
                                    default:
                                        #region new_system

                                        foreach (char directory in PsImage.IDImage.ToString())
                                            ftpPath += directory + "/";
                                        break;
                                        #endregion
                                }

                                // <JG> 21/05/2013 import image originale
                                Boolean import_img_tmp = false;
                                try
                                {
                                    string ftpfullpath = (Core.Global.GetConfig().ConfigImageStorageMode == Core.Parametres.ImageStorageMode.old_system)
                                        ? FTP + ftpPath + PsImage.IDProduct + "-" + PsImage.IDImage + Core.Img.jpgExtension
                                        : FTP + ftpPath + PsImage.IDImage + Core.Img.jpgExtension;

                                    bool existfile = Core.Ftp.ExistFile(ftpfullpath, User, Password);
                                    if (existfile)
                                    {
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
                                                memStream.Write(buffer, 0, bytesRead);
                                            else
                                                break;
                                            downloadedData = memStream.ToArray();
                                        }

                                        string target_folder = (Core.Global.GetConfig().ConfigLocalStorageMode == Parametres.LocalStorageMode.simple_system)
                                            ? Global.GetConfig().Folders.TempArticle
                                            : ArticleImage.advanced_folder;

                                        if (downloadedData != null && downloadedData.Length != 0)
                                        {
                                            FileStream newFile = new FileStream(
                                                System.IO.Path.Combine(target_folder, String.Format("{0}" + extension, ArticleImage.ImaArt_Id)),
                                                FileMode.Create);
                                            newFile.Write(downloadedData, 0, downloadedData.Length);
                                            newFile.Close();
                                            newFile.Dispose();
                                            memStream.Dispose();
                                            downloadedData = new byte[0];
                                        }
                                        string local_file_tmp = System.IO.Path.Combine(target_folder, String.Format("{0}" + extension, ArticleImage.ImaArt_Id));

                                        // <JG> 30/09/2013 détection encodage PNG lors de l'import
                                        Boolean rename_to_png = false;
                                        System.Drawing.Image img = System.Drawing.Image.FromFile(local_file_tmp);
                                        var imgguid = img.RawFormat.Guid;
                                        img.Dispose();
                                        System.Drawing.Imaging.ImageCodecInfo search;
                                        foreach (System.Drawing.Imaging.ImageCodecInfo codec in System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders())
                                            if (codec.FormatID == imgguid)
                                            {
                                                search = codec;
                                                if (search.FormatDescription == "PNG")
                                                    rename_to_png = true;
                                                break;
                                            }
                                        if (rename_to_png)
                                        {
                                            if (System.IO.File.Exists(local_file_tmp))
                                            {
                                                extension = Core.Img.pngExtension;
                                                System.IO.File.Move(local_file_tmp, System.IO.Path.Combine(target_folder, String.Format("{0}" + extension, ArticleImage.ImaArt_Id)));
                                            }
                                            ArticleImage.ImaArt_SourceFile = SearchFreeNameFile(Article.Art_Id, Article.Art_Ref, extension);
                                        }
                                        ArticleImage.ImaArt_Image = String.Format("{0}" + extension, ArticleImage.ImaArt_Id);
                                        ArticleImageRepository.Save();

                                        Core.Img.resizeImage(new System.Drawing.Bitmap(ArticleImage.TempFileName),
                                            Core.Global.GetConfig().ConfigImageMiniatureWidth,
                                            Core.Global.GetConfig().ConfigImageMiniatureHeight,
                                            ArticleImage.SmallFileName);
                                        import_img_tmp = true;
                                    }
                                }
                                catch (Exception)
                                {
                                    // Not implemented
                                }
                                if (import_img_tmp)
                                {
                                    Model.Prestashop.PsImageTypeRepository PsImageTypeRepository = new Model.Prestashop.PsImageTypeRepository();
                                    List<Model.Prestashop.PsImageType> ListPsImageType = PsImageTypeRepository.ListProduct(1);
                                    foreach (Model.Prestashop.PsImageType PsImageType in ListPsImageType)
                                    {
                                        string ftpfullpath = (Core.Global.GetConfig().ConfigImageStorageMode == Core.Parametres.ImageStorageMode.old_system)
                                            ? FTP + ftpPath + PsImage.IDProduct + "-" + PsImage.IDImage + "-" + PsImageType.Name + Core.Img.jpgExtension
                                            : FTP + ftpPath + PsImage.IDImage + "-" + PsImageType.Name + Core.Img.jpgExtension;

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
                                                memStream.Write(buffer, 0, bytesRead);
                                            else
                                                break;
                                            downloadedData = memStream.ToArray();
                                        }

                                        if (downloadedData != null && downloadedData.Length != 0)
                                        {
                                            FileStream newFile = new FileStream(ArticleImage.FileName(PsImageType.Name), FileMode.Create);
                                            newFile.Write(downloadedData, 0, downloadedData.Length);
                                            newFile.Close();
                                        }
                                    }
                                    import_img = true;

                                    // gestion image par défaut
                                    if (ArticleImage.ImaArt_Default)
                                    {
                                        List<Model.Local.ArticleImage> ListArticleImage = ArticleImageRepository.ListArticle(ArticleImage.Art_Id.Value);
                                        if (ListArticleImage.Count(i => i.ImaArt_Default == true && i.ImaArt_Id != ArticleImage.ImaArt_Id) > 0)
                                            foreach (Model.Local.ArticleImage ArticleImageDefault in ListArticleImage.Where(i => i.ImaArt_Default == true && i.ImaArt_Id != ArticleImage.ImaArt_Id))
                                            {
                                                ArticleImageDefault.ImaArt_Default = false;
                                                ArticleImageRepository.Save();
                                            }
                                    }

                                    // liens images déclinaisons
                                    ExecAttributeImage(PsImage, ArticleImage);
                                }
                            }
                            catch (Exception ex)
                            {
                                Core.Error.SendMailError("[DOWNLOAD FTP IMAGE ARTICLE]<br />" + ex.ToString());
                                ArticleImageRepository.Delete(ArticleImage);
                            }
                            if (!import_img)
                                ArticleImageRepository.Delete(ArticleImage);
                        }
                    }
                }
                else if (ArticleImageRepository.ExistPrestaShop(ImageSend))
                {
                    // import des affectations aux déclinaisons
                    Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();
                    Model.Prestashop.PsImage PsImage = PsImageRepository.ReadImage(Convert.ToUInt32(ImageSend));
                    Model.Local.ArticleImage ArticleImage = ArticleImageRepository.ReadPrestaShop(ImageSend);
                    ExecAttributeImage(PsImage, ArticleImage);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        // <JG> 08/08/2013 ajout nom fichier auto avec gestion multi image par référence
        private string SearchFreeNameFile(int Article, string Reference, string extension)
        {
            string result = Reference + extension;
            Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
            int compteur = 0;
            while (ArticleImageRepository.ExistArticleFile(Article, result))
            {
                compteur += 1;
                result = Reference + "_" + compteur + extension;
            }
            return result;
        }

        private void ExecAttributeImage(Model.Prestashop.PsImage PsImage, Model.Local.ArticleImage ArticleImage)
        {
            Model.Prestashop.PsProductAttributeImageRepository PsProductAttributeImageRepository = new Model.Prestashop.PsProductAttributeImageRepository();
            List<Model.Prestashop.PsProductAttributeImage> ListPsProductAttributeImage = PsProductAttributeImageRepository.ListImage(PsImage.IDImage);

            Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new Model.Local.AttributeArticleImageRepository();
            Model.Local.CompositionArticleImageRepository CompositionArticleImageRepository = new Model.Local.CompositionArticleImageRepository();
            Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
            Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();

            #region suppression lien image déclinaison
            List<Model.Local.AttributeArticleImage> ListAttributeArticleImage = AttributeArticleImageRepository.ListImageArticle(ArticleImage.ImaArt_Id);
            List<Model.Local.CompositionArticleImage> ListCompositionArticleImage = CompositionArticleImageRepository.ListImageArticle(ArticleImage.ImaArt_Id);

            Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
            // liste des liens images déclinaisons PrestaConnect
            foreach (Model.Local.AttributeArticleImage AttributeArticleImage in ListAttributeArticleImage)
            {
                // si la déclinaison PrestaConnectexiste dans PrestaConnect
                if (AttributeArticleRepository.Exist(AttributeArticleImage.AttArt_Id))
                {
                    Model.Local.AttributeArticle AttributeArticle = AttributeArticleRepository.Read(AttributeArticleImage.AttArt_Id);
                    if (AttributeArticle.Pre_Id != null && AttributeArticle.Pre_Id != 0)
                    {
                        // si dans PrestaShop la déclinaison n'est pas liée à l'image
                        if (ListPsProductAttributeImage.Count(pai => pai.IDProductAttribute == (uint)AttributeArticle.Pre_Id.Value) == 0)
                        {
                            // suppression du lien dans PrestaConnect
                            AttributeArticleImageRepository.Delete(AttributeArticleImage);
                        }
                    }
                }
            }
            foreach (Model.Local.CompositionArticleImage CompositionArticleImage in ListCompositionArticleImage)
            {
                if (CompositionArticleRepository.Exist(CompositionArticleImage.ComArt_Id))
                {
                    Model.Local.CompositionArticle CompositionArticle = CompositionArticleRepository.Read(CompositionArticleImage.ComArt_Id);
                    if (CompositionArticle.Pre_Id != null && CompositionArticle.Pre_Id != 0)
                    {
                        // si dans PrestaShop la déclinaison n'est pas liée à l'image
                        if (ListPsProductAttributeImage.Count(pai => pai.IDProductAttribute == (uint)CompositionArticle.Pre_Id.Value) == 0)
                        {
                            // suppression du lien dans PrestaConnect
                            CompositionArticleImageRepository.Delete(CompositionArticleImage);
                        }
                    }
                }
            }
            #endregion

            foreach (Model.Prestashop.PsProductAttributeImage PsProductAttributeImage in ListPsProductAttributeImage)
            {
                if (AttributeArticleRepository.ExistPrestashop((int)PsProductAttributeImage.IDProductAttribute))
                {
                    Model.Local.AttributeArticle AttributeArticle = AttributeArticleRepository.ReadPrestashop((int)PsProductAttributeImage.IDProductAttribute);
                    if (!AttributeArticleImageRepository.ExistAttributeArticleImage(AttributeArticle.AttArt_Id, ArticleImage.ImaArt_Id))
                    {
                        AttributeArticleImageRepository.Add(new Model.Local.AttributeArticleImage()
                        {
                            AttArt_Id = AttributeArticle.AttArt_Id,
                            ImaArt_Id = ArticleImage.ImaArt_Id,
                        });
                    }
                }
                else if (CompositionArticleRepository.ExistPrestaShop((int)PsProductAttributeImage.IDProductAttribute))
                {
                    Model.Local.CompositionArticle CompositionArticle = CompositionArticleRepository.ReadPrestaShop((int)PsProductAttributeImage.IDProductAttribute);
                    if (!AttributeArticleImageRepository.ExistAttributeArticleImage(CompositionArticle.ComArt_Id, ArticleImage.ImaArt_Id))
                    {
                        CompositionArticleImageRepository.Add(new Model.Local.CompositionArticleImage()
                        {
                            ComArt_Id = CompositionArticle.ComArt_Id,
                            ImaArt_Id = ArticleImage.ImaArt_Id,
                        });
                    }
                }
            }
        }
    }
}
