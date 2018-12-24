using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace PRESTACONNECT.Core.Tools
{
    public class SuppressionImage
    {
        /// <summary>
        /// ExecArticle - Supprime toutes les images d'un article
        /// </summary>
        /// <param name="ArticleSend"></param>
        public void ExecArticle(Int32 ArticleSend, Boolean OnlyIfNotSourceExist, out List<string> log_out)
        {
            log_out = new List<string>();
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                if (ArticleRepository.ExistArticle(ArticleSend))
                {
                    Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                    List<Model.Local.ArticleImage> List = ArticleImageRepository.ListArticle(ArticleSend);
                    if (List != null)
                    {
                        Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                        foreach (Model.Local.ArticleImage ArticleImage in List)
                        {
                            List<string> log = null;
                            if (OnlyIfNotSourceExist)
                            {
                                if (!System.IO.File.Exists(System.IO.Path.Combine(Core.Global.GetConfig().AutomaticImportFolderPicture, ArticleImage.ImaArt_SourceFile)))
                                    Exec(ArticleImage, Article, out log);
                            }
                            else
                            {
                                Exec(ArticleImage, Article, out log);
                            }
                            if (log != null && log.Count > 0)
                                log_out.AddRange(log);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void Exec(Model.Local.ArticleImage ArticleImage, Model.Local.Article Article, out List<string> log_out)
        {
            log_out = new List<string>();
            try
            {
                // Suppression de l'affectation de l'image à la gamme en local
                Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new Model.Local.AttributeArticleImageRepository();
                AttributeArticleImageRepository.DeleteAll(AttributeArticleImageRepository.ListImageArticle(ArticleImage.ImaArt_Id));

                // 07/04/2016 ajout suppression liens compositions
                Model.Local.CompositionArticleImageRepository CompositionArticleImageRepository = new Model.Local.CompositionArticleImageRepository();
                CompositionArticleImageRepository.DeleteAll(CompositionArticleImageRepository.ListImageArticle(ArticleImage.ImaArt_Id));

                // Suppression des fichiers images en local
                if (File.Exists(ArticleImage.SmallFileName))
                    try { File.Delete(ArticleImage.SmallFileName); }
                    catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
                if (File.Exists(ArticleImage.TempFileName))
                    try { File.Delete(ArticleImage.TempFileName); }
                    catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }

                string folder = (Core.Global.GetConfig().ConfigLocalStorageMode == Core.Parametres.LocalStorageMode.advanced_system)
                    ? ArticleImage.advanced_folder
                    : Core.Global.GetConfig().Folders.RootArticle;

                foreach (var fileName in Directory.GetFiles(folder, String.Format("{0}-*" + ArticleImage.GetExtension, ArticleImage.ImaArt_Id)))
                    try { File.Delete(fileName); }
                    catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }

                if (ArticleImage.Pre_Id != null && ArticleImage.Pre_Id > 0)
                {
                    Model.Prestashop.PsProductAttributeImageRepository PsProductAttributeImageRepository = new Model.Prestashop.PsProductAttributeImageRepository();
                    PsProductAttributeImageRepository.DeleteAll(PsProductAttributeImageRepository.ListImage((uint)ArticleImage.Pre_Id));

                    Model.Prestashop.PsImageLangRepository PsImageLangRepository = new Model.Prestashop.PsImageLangRepository();
                    PsImageLangRepository.DeleteAll(PsImageLangRepository.ListImage((uint)ArticleImage.Pre_Id));

                    // Suppression de l'occurence de l'image sur prestashop
                    Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();
                    Model.Prestashop.PsImage PsImage = PsImageRepository.ReadImage((uint)ArticleImage.Pre_Id);

                    if (PsImage != null)
                    {
                        PsImageRepository.Delete(PsImage);

                        try
                        {
                            if (PsImage.Cover == 1
                                && !PsImageRepository.ExistProductCover(PsImage.IDProduct, 1)
                                && PsImageRepository.ExistProduct(PsImage.IDProduct))
                            {
                                List<Model.Prestashop.PsImage> Others = PsImageRepository.ListProduct(PsImage.IDProduct);
                                Model.Prestashop.PsImage NewCover = Others.OrderBy(i => i.Position).FirstOrDefault();
                                if (NewCover != null)
                                {
                                    NewCover.Cover = 1;
                                    PsImageRepository.Save();

                                    Core.Transfert.TransfertArticleImage syncimage = new Core.Transfert.TransfertArticleImage();
                                    syncimage.ExecCover(PsImage.IDProduct, true, PsImageRepository, NewCover);

                                    Model.Prestashop.PsImageShopRepository PsImageShopRepository = new Model.Prestashop.PsImageShopRepository();
                                    foreach (Model.Prestashop.PsImageShop PsImageShop in PsImageShopRepository.List(NewCover.IDImage))
                                        PsImageShop.Cover = 1;
                                    PsImageShopRepository.Save();
                                }
                            }
                        }
                        catch
                        {
                            log_out.Add("II47- Erreur durant la réaffectation de l'image de couverture dans PrestaShop pour l'article " + Article.Art_Ref);
                        }
                        try
                        {
                            foreach (Model.Prestashop.PsImage Image in PsImageRepository.ListProduct(PsImage.IDProduct).Where(i => i.Position > PsImage.Position))
                                Image.Position--;
                            PsImageRepository.Save();
                        }
                        catch
                        {
                            log_out.Add("II48- Erreur durant le recalcul des positions dans PrestaShop pour les images de l'article " + Article.Art_Ref);
                        }

                        // Suppression des fichiers images sur prestashop
                        if (Core.Global.GetConfig().ConfigFTPActive)
                        {
                            String FTP = Core.Global.GetConfig().ConfigFTPIP;
                            String User = Core.Global.GetConfig().ConfigFTPUser;
                            String Password = Core.Global.GetConfig().ConfigFTPPassword;

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

                                    foreach (char directory in ArticleImage.Pre_Id.ToString())
                                        ftpPath += directory + "/";
                                    break;
                                    #endregion
                            }

                            // <JG> 21/05/2013 correct for 1.5 storage mode
                            string ftpfullpath = (Core.Global.GetConfig().ConfigImageStorageMode == Core.Parametres.ImageStorageMode.old_system)
                                ? FTP + ftpPath + Article.Pre_Id + "-" + ArticleImage.Pre_Id + ".jpg"
                                : FTP + ftpPath + ArticleImage.Pre_Id + ".jpg";

                            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
                            request.Credentials = new System.Net.NetworkCredential(User, Password);
                            request.Method = WebRequestMethods.Ftp.DeleteFile;
                            request.UseBinary = true;
                            request.UsePassive = true;
                            request.KeepAlive = false;

                            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                            response.Close();

                            Model.Prestashop.PsImageTypeRepository PsImageTypeRepository = new Model.Prestashop.PsImageTypeRepository();
                            List<Model.Prestashop.PsImageType> ListPsImageType = PsImageTypeRepository.ListProduct(1);

                            foreach (Model.Prestashop.PsImageType PsImageType in ListPsImageType)
                            {
                                try
                                {
                                    // <JG> 21/05/2013 correct for 1.5 storage mode
                                    ftpfullpath = (Core.Global.GetConfig().ConfigImageStorageMode == Core.Parametres.ImageStorageMode.old_system)
                                        ? FTP + ftpPath + Article.Pre_Id + "-" + ArticleImage.Pre_Id + "-" + PsImageType.Name + ".jpg"
                                        : FTP + ftpPath + ArticleImage.Pre_Id + "-" + PsImageType.Name + ".jpg";

                                    request = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                                    request.Credentials = new System.Net.NetworkCredential(User, Password);
                                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                                    request.UseBinary = true;
                                    request.UsePassive = true;
                                    request.KeepAlive = false;

                                    response = (FtpWebResponse)request.GetResponse();
                                    response.Close();
                                }
                                catch {}
                            }
                        }
                    }
                }

                // Suppression de l'occurence de l'image en local
                Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                Model.Local.ArticleImage articleImage = ArticleImageRepository.ReadArticleImage(ArticleImage.ImaArt_Id);

                if (articleImage != null)
                {
                    ArticleImageRepository.Delete(articleImage);

                    try
                    {
                        if (articleImage.ImaArt_Default
                                && !ArticleImageRepository.ExistArticleDefault(Article.Art_Id, true)
                                && ArticleImageRepository.ExistArticle(Article.Art_Id))
                        {
                            Model.Local.ArticleImage NewCover = ArticleImageRepository.ListArticle(Article.Art_Id).OrderBy(i => i.ImaArt_Position).FirstOrDefault();
                            if (NewCover != null)
                            {
                                NewCover.ImaArt_Default = true;
                                ArticleImageRepository.Save();
                            }
                        }
                    }
                    catch
                    {
                        log_out.Add("II43- Erreur durant la réaffectation de l'image de couverture dans PrestaConnect pour l'article " + Article.Art_Ref);
                    }
                    try
                    {
                        foreach (Model.Local.ArticleImage Image in ArticleImageRepository.ListArticle(Article.Art_Id).Where(i => i.ImaArt_Position > articleImage.ImaArt_Position))
                            Image.ImaArt_Position--;
                        ArticleImageRepository.Save();
                    }
                    catch
                    {
                        log_out.Add("II44- Erreur durant le recalcul des positions dans PrestaConnect pour les images de l'article " + Article.Art_Ref);
                    }
                }

                log_out.Add("II40- Suppression de l'image " + ArticleImage.ImaArt_SourceFile + " en position " + ArticleImage.ImaArt_Position + " pour l'article " + Article.Art_Ref);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
