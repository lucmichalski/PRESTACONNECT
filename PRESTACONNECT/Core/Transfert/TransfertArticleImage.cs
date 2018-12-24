using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace PRESTACONNECT.Core.Transfert
{
    public class TransfertArticleImage
    {
        public void Exec(Int32 ArticleSend)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                // If the catalog is sync with Prestashop
                if (Article.Catalog != null && Article.Catalog.Pre_Id != null || Article.Catalog.Pre_Id != 0)
                {
                    Model.Prestashop.PsProductRepository ProductRepository = new Model.Prestashop.PsProductRepository();
                    // If the Article have a connection with Prestashop
                    if (Article.Pre_Id != null)
                    {
                        if (ProductRepository.ExistId(Convert.ToUInt32(Article.Pre_Id.Value)))
                        {
                            this.ExecLocalToDistant(Article);
                            this.ExecAttributeImage(Article);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLocalToDistant(Model.Local.Article Article)
        {
            try
            {
                String FTP = Core.Global.GetConfig().ConfigFTPIP;
                String User = Core.Global.GetConfig().ConfigFTPUser;
                String Password = Core.Global.GetConfig().ConfigFTPPassword;

                Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                List<Model.Local.ArticleImage> ListArticleImage = ArticleImageRepository.ListArticle(Article.Art_Id);
                foreach (Model.Local.ArticleImage ArticleImage in ListArticleImage)
                {
                    ExecImage(FTP, User, Password, Article, ArticleImage, ArticleImageRepository);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecImage(string FTP, string User, string Password, Model.Local.Article Article, Model.Local.ArticleImage ArticleImage, Model.Local.ArticleImageRepository ArticleImageRepository)
        {
            Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();
            Model.Prestashop.PsImage PsImage = null;
            Model.Prestashop.PsImageLangRepository PsImageLangRepository = new Model.Prestashop.PsImageLangRepository();
            Model.Prestashop.PsImageLang PsImageLang;
            Boolean isImageLang = false;

            Model.Prestashop.PsImageTypeRepository PsImageTypeRepository = new Model.Prestashop.PsImageTypeRepository();
            List<Model.Prestashop.PsImageType> ListPsImageType = PsImageTypeRepository.ListProduct(1);

            try
            {
                if (ArticleImage.Pre_Id == null)
                {
                    PsImage = new Model.Prestashop.PsImage();

                    PsImage.IDProduct = Convert.ToUInt32(Article.Pre_Id);
                    PsImage.Position = (ushort)ExecPosition((uint)Article.Pre_Id.Value, (uint)ArticleImage.ImaArt_Position, PsImageRepository, PsImage);
                    PsImage.Cover = ExecCover((uint)Article.Pre_Id.Value, ArticleImage.ImaArt_Default, PsImageRepository, PsImage);
                    PsImageRepository.Add(PsImage, Global.CurrentShop.IDShop);

                    #region lang
                    isImageLang = false;
                    PsImageLang = new Model.Prestashop.PsImageLang();
                    if (PsImageLangRepository.ExistImageLang(PsImage.IDImage, Core.Global.Lang))
                    {
                        PsImageLang = PsImageLangRepository.ReadImageLang(PsImage.IDImage, Core.Global.Lang);
                        isImageLang = true;
                    }
                    PsImageLang.Legend = ArticleImage.ImaArt_Name;
                    if (isImageLang == true)
                    {
                        PsImageLangRepository.Save();
                    }
                    else
                    {
                        PsImageLang.IDImage = PsImage.IDImage;
                        PsImageLang.IDLang = Core.Global.Lang;
                        PsImageLangRepository.Add(PsImageLang);
                    }
                    // <JG> 26/12/2012 ajout insertion autres langues actives si non renseignées
                    try
                    {
                        Model.Prestashop.PsLangRepository PsLangRepository = new Model.Prestashop.PsLangRepository();
                        foreach (Model.Prestashop.PsLang PsLang in PsLangRepository.ListActive(1, Global.CurrentShop.IDShop))
                        {
                            if (!PsImageLangRepository.ExistImageLang(PsImage.IDImage, PsLang.IDLang))
                            {
                                PsImageLang = new Model.Prestashop.PsImageLang()
                                {
                                    IDImage = PsImage.IDImage,
                                    IDLang = PsLang.IDLang,
                                    Legend = ArticleImage.ImaArt_Name
                                };
                                PsImageLangRepository.Add(PsImageLang);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Error.SendMailError(ex.ToString());
                    }
                    #endregion

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

                                //System.Net.FtpWebRequest ftp_folder = null;

                                foreach (char directory in PsImage.IDImage.ToString())
                                {
                                    ftpPath += directory + "/";

                                    #region MyRegion
                                    try
                                    {

                                        FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FTP + ftpPath);

                                        request.Credentials = new NetworkCredential(User, Password);
                                        request.UsePassive = true;
                                        request.UseBinary = true;
                                        request.KeepAlive = false;
                                        request.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;

                                        request.Method = WebRequestMethods.Ftp.MakeDirectory;

                                        FtpWebResponse makeDirectoryResponse = (FtpWebResponse)request.GetResponse();

                                    }
                                    catch //Exception ex
                                    {
                                        //System.Windows.MessageBox.Show(ex.ToString());
                                    }
                                    #endregion

                                    //try
                                    //{
                                    //    ftp_folder = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(FTP + ftpPath);

                                    //    ftp_folder.Credentials = new System.Net.NetworkCredential(User, Password);
                                    //    ftp_folder.UseBinary = true;
                                    //    ftp_folder.UsePassive = true;
                                    //    ftp_folder.KeepAlive = false;
                                    //    ftp_folder.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory;

                                    //    System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)ftp_folder.GetResponse();
                                    //    System.IO.Stream ftpStream = response.GetResponseStream();

                                    //    ftpStream.Close();
                                    //    response.Close();
                                    //}
                                    //catch(Exception ex)
                                    //{
                                    //    System.Windows.MessageBox.Show(ex.ToString());
                                    //}
                                }
                                break;
                                #endregion
                        }

                        #region Upload des images
                        String extension = ArticleImage.GetExtension;
                        if (System.IO.File.Exists(ArticleImage.TempFileName))
                        {
                            string ftpfullpath = (Core.Global.GetConfig().ConfigImageStorageMode == Core.Parametres.ImageStorageMode.old_system)
                                ? FTP + ftpPath + PsImage.IDProduct + "-" + PsImage.IDImage + ".jpg"
                                : FTP + ftpPath + PsImage.IDImage + ".jpg";

                            System.Net.FtpWebRequest ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                            ftp.Credentials = new System.Net.NetworkCredential(User, Password);
                            //userid and password for the ftp server to given  

                            ftp.UseBinary = true;
                            ftp.UsePassive = true;
                            ftp.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;
                            ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                            System.IO.FileStream fs = System.IO.File.OpenRead(ArticleImage.TempFileName);
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            fs.Close();
                            System.IO.Stream ftpstream = ftp.GetRequestStream();
                            ftpstream.Write(buffer, 0, buffer.Length);
                            ftpstream.Close();
                            ftp.Abort();
                        }

                        foreach (Model.Prestashop.PsImageType PsImageType in ListPsImageType)
                        {
                            String PathImg = ArticleImage.FileName(PsImageType.Name);
                            if (System.IO.File.Exists(PathImg))
                            {
                                string ftpfullpath = (Core.Global.GetConfig().ConfigImageStorageMode == Core.Parametres.ImageStorageMode.old_system)
                                    ? FTP + ftpPath + PsImage.IDProduct + "-" + PsImage.IDImage + "-" + PsImageType.Name + ".jpg"
                                    : FTP + ftpPath + PsImage.IDImage + "-" + PsImageType.Name + ".jpg";

                                System.Net.FtpWebRequest ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                                ftp.Credentials = new System.Net.NetworkCredential(User, Password);
                                //userid and password for the ftp server to given  

                                ftp.UseBinary = true;
                                ftp.UsePassive = true;
                                ftp.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;
                                ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                                System.IO.FileStream fs = System.IO.File.OpenRead(PathImg);
                                byte[] buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, buffer.Length);
                                fs.Close();
                                System.IO.Stream ftpstream = ftp.GetRequestStream();
                                ftpstream.Write(buffer, 0, buffer.Length);
                                ftpstream.Close();
                                ftp.Abort();
                            }
                        }
                        #endregion

                        ArticleImage.Pre_Id = Convert.ToInt32(PsImage.IDImage);
                        ArticleImageRepository.Save();
                    }
                    catch (Exception ex)
                    {
                        Core.Error.SendMailError("[UPLOAD FTP IMAGE ARTICLE]<br />" + ex.ToString());
                        PsImageLangRepository.DeleteAll(PsImageLangRepository.ListImage(PsImage.IDImage));
                        PsImageRepository.Delete(PsImage);
                    }
                }
                // option car si PAS GADA écrase infos PS
                else if (Core.Global.GetConfig().ConfigImageSynchroPositionLegende)
                {
                    PsImage = new Model.Prestashop.PsImage();
                    if (PsImageRepository.ExistImage((uint)ArticleImage.Pre_Id))
                    {
                        PsImage = PsImageRepository.ReadImage((uint)ArticleImage.Pre_Id);
                        if (PsImage.Position != (UInt16)ArticleImage.ImaArt_Position
                            || PsImage.Cover != 
								#if (PRESTASHOP_VERSION_172 || PRESTASHOP_VERSION_161)
								((ArticleImage.ImaArt_Default) ? Convert.ToByte(ArticleImage.ImaArt_Default) : (byte?)null))
								#else
								Convert.ToByte(ArticleImage.ImaArt_Default))
								#endif
                        {
                            PsImage.Position = (ushort)ExecPosition((uint)Article.Pre_Id, (uint)ArticleImage.ImaArt_Position, PsImageRepository, PsImage);
                            PsImage.Cover = ExecCover((uint)Article.Pre_Id, ArticleImage.ImaArt_Default, PsImageRepository, PsImage);
                            PsImageRepository.Save();

                            Model.Prestashop.PsImageShopRepository PsImageShopRepository = new Model.Prestashop.PsImageShopRepository();
                            Model.Prestashop.PsImageShop PsImageShop = PsImageShopRepository.ReadImage(PsImage.IDImage);
                            if (PsImageShop != null && PsImageShop.Cover != 
								#if (PRESTASHOP_VERSION_160)
								(sbyte)PsImage.Cover
								#else
								PsImage.Cover
								#endif
							)
                            {
								#if (PRESTASHOP_VERSION_160)
                                PsImageShop.Cover = (sbyte)PsImage.Cover;
								#else
                                PsImageShop.Cover = PsImage.Cover;
								#endif
                                PsImageShopRepository.Save();
                            }
                        }

                        #region lang
                        PsImageLang = new Model.Prestashop.PsImageLang();
                        isImageLang = false;
                        if (PsImageLangRepository.ExistImageLang(PsImage.IDImage, Core.Global.Lang))
                        {
                            PsImageLang = PsImageLangRepository.ReadImageLang(PsImage.IDImage, Core.Global.Lang);
                            isImageLang = true;
                        }
                        PsImageLang.Legend = ArticleImage.ImaArt_Name;
                        if (isImageLang == true)
                            PsImageLangRepository.Save();
                        else
                        {
                            PsImageLang.IDImage = PsImage.IDImage;
                            PsImageLang.IDLang = Core.Global.Lang;
                            PsImageLangRepository.Add(PsImageLang);
                        }
                        // <JG> 26/12/2012 ajout insertion autres langues actives si non renseignées
                        try
                        {
                            Model.Prestashop.PsLangRepository PsLangRepository = new Model.Prestashop.PsLangRepository();
                            foreach (Model.Prestashop.PsLang PsLang in PsLangRepository.ListActive(1, Global.CurrentShop.IDShop))
                            {
                                if (!PsImageLangRepository.ExistImageLang(PsImage.IDImage, PsLang.IDLang))
                                {
                                    PsImageLang = new Model.Prestashop.PsImageLang()
                                    {
                                        IDImage = PsImage.IDImage,
                                        IDLang = PsLang.IDLang,
                                        Legend = ArticleImage.ImaArt_Name
                                    };
                                    PsImageLangRepository.Add(PsImageLang);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Core.Error.SendMailError(ex.ToString());
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[SYNCHRO IMAGE ARTICLE]<br />" + ex.ToString());

                if (PsImage != null)
                {
                    PsImageLangRepository.DeleteAll(PsImageLangRepository.ListImage(PsImage.IDImage));
                    PsImageRepository.Delete(PsImage);
                }
            }
        }

        private void ExecAttributeImage(Model.Local.Article Article)
        {
            Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
            List<Model.Local.ArticleImage> ListArticleImage = ArticleImageRepository.ListArticle(Article.Art_Id);

            Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new Model.Local.AttributeArticleImageRepository();
            Model.Local.CompositionArticleImageRepository CompositionArticleImageRepository = new Model.Local.CompositionArticleImageRepository();
            Model.Prestashop.PsProductAttributeImageRepository PsProductAttributeImageRepository = new Model.Prestashop.PsProductAttributeImageRepository();
            Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();
            Model.Local.AttributeArticle AttributeArticle = new Model.Local.AttributeArticle();
            Model.Local.CompositionArticle CompositionArticle = new Model.Local.CompositionArticle();

            foreach (Model.Local.ArticleImage ArticleImage in ListArticleImage)
            {
                if (ArticleImage.Pre_Id != null && PsImageRepository.ExistImage((uint)ArticleImage.Pre_Id.Value))
                {
                    List<Model.Local.AttributeArticleImage> ListAttributeArticleImage = AttributeArticleImageRepository.ListImageArticle(ArticleImage.ImaArt_Id);
                    List<Model.Local.CompositionArticleImage> ListCompositionArticleImage = CompositionArticleImageRepository.ListImageArticle(ArticleImage.ImaArt_Id);

                    #region suppression lien image déclinaison PrestaShop
                    Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                    Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                    // liste des liens images déclinaisons Prestashop
                    foreach (Model.Prestashop.PsProductAttributeImage PsProductAttributeImage in PsProductAttributeImageRepository.ListImage((uint)ArticleImage.Pre_Id))
                    {
                        // si la déclinaison existe dans PrestaConnect
                        if (AttributeArticleRepository.ExistPrestashop((int)PsProductAttributeImage.IDProductAttribute))
                        {
                            AttributeArticle = AttributeArticleRepository.ReadPrestashop((int)PsProductAttributeImage.IDProductAttribute);
                            // si dans prestaconnect la déclinaison n'est pas liée à l'image
                            if (!AttributeArticleImageRepository.ExistAttributeArticleImage(AttributeArticle.AttArt_Id, ArticleImage.ImaArt_Id))
                            {
                                // suppression du lien dans PrestaShop
                                PsProductAttributeImageRepository.Delete(PsProductAttributeImage);
                            }
                        }
                        else if (CompositionArticleRepository.ExistPrestaShop((int)PsProductAttributeImage.IDProductAttribute))
                        {
                            CompositionArticle = CompositionArticleRepository.ReadPrestaShop((int)PsProductAttributeImage.IDProductAttribute);
                            // si dans prestaconnect la déclinaison n'est pas liée à l'image
                            if (!CompositionArticleImageRepository.ExistCompositionArticleImage(CompositionArticle.ComArt_Id, ArticleImage.ImaArt_Id))
                            {
                                // suppression du lien dans PrestaShop
                                PsProductAttributeImageRepository.Delete(PsProductAttributeImage);
                            }
                        }
                    }
                    #endregion

                    // affectation image déclinaison
                    foreach (Model.Local.AttributeArticleImage AttributeArticleImage in ListAttributeArticleImage)
                    {
                        if (AttributeArticleImage.AttributeArticle.Pre_Id != null
                            && AttributeArticleImage.AttributeArticle.Pre_Id != 0)
                        {
                            if (PsProductAttributeImageRepository.ExistProductAttributeImage((UInt32)AttributeArticleImage.AttributeArticle.Pre_Id, (UInt32)AttributeArticleImage.ArticleImage.Pre_Id) == false)
                            {
                                PsProductAttributeImageRepository.Add(new Model.Prestashop.PsProductAttributeImage()
                                {
                                    IDImage = (UInt32)AttributeArticleImage.ArticleImage.Pre_Id,
                                    IDProductAttribute = (UInt32)AttributeArticleImage.AttributeArticle.Pre_Id,
                                });
                            }
                        }
                    }
                    foreach (Model.Local.CompositionArticleImage CompositionArticleImage in ListCompositionArticleImage)
                    {
                        if (CompositionArticleImage.CompositionArticle.Pre_Id != null
                            && CompositionArticleImage.CompositionArticle.Pre_Id != 0)
                        {
                            if (PsProductAttributeImageRepository.ExistProductAttributeImage((UInt32)CompositionArticleImage.CompositionArticle.Pre_Id, (UInt32)CompositionArticleImage.ArticleImage.Pre_Id) == false)
                            {
                                PsProductAttributeImageRepository.Add(new Model.Prestashop.PsProductAttributeImage()
                                {
                                    IDImage = (UInt32)CompositionArticleImage.ArticleImage.Pre_Id,
                                    IDProductAttribute = (UInt32)CompositionArticleImage.CompositionArticle.Pre_Id,
                                });
                            }
                        }
                    }
                }
            }
        }

        private uint ExecPosition(uint pdt, uint pos, Model.Prestashop.PsImageRepository PsImageRepository, Model.Prestashop.PsImage PsImageCurrent)
        {
            // libère la position pour y insérer l'image
            if (PsImageRepository.ExistProductPosition(pdt, pos))
            {
                Model.Prestashop.PsImage PsImagePosition = PsImageRepository.ReadProductPosition(pdt, pos);
                if (PsImagePosition.IDImage != PsImageCurrent.IDImage)
                {
                    PsImagePosition.Position = Model.Prestashop.PsImageRepository.NextPositionProductImage(pdt);
                    PsImageRepository.Save();
                }
            }
            return pos;
        }

		#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
        public byte? ExecCover(uint pdt, bool cov, Model.Prestashop.PsImageRepository PsImageRepository, Model.Prestashop.PsImage PsImageCurrent)
        {
            if (cov)
            {
                if (PsImageRepository.ExistProductCover(pdt, Convert.ToByte(cov)))
                {
                    Model.Prestashop.PsImageShopRepository PsImageShopRepository = new Model.Prestashop.PsImageShopRepository();
                    Model.Prestashop.PsImageShop PsImageShop;
                    foreach (Model.Prestashop.PsImage PsImage in PsImageRepository.ListProduct(pdt))
                    {
                        if (PsImage.IDImage != PsImageCurrent.IDImage)
                        {
                            PsImage.Cover = (byte?)null;
                            if (PsImageShopRepository.ExistImage(PsImage.IDImage))
                            {
                                PsImageShop = PsImageShopRepository.ReadImage(PsImage.IDImage);
                                PsImageShop.Cover = PsImage.Cover;
                            }
                        }
                    }
                    PsImageShopRepository.Save();
                    PsImageRepository.Save();
                }
            }
            return (cov) ? Convert.ToByte(cov) : (byte?)null;
        }
		#else
        public byte ExecCover(uint pdt, bool cov, Model.Prestashop.PsImageRepository PsImageRepository, Model.Prestashop.PsImage PsImageCurrent)
        {
            if (cov)
            {
                if (PsImageRepository.ExistProductCover(pdt, Convert.ToByte(cov)))
                {
                    Model.Prestashop.PsImageShopRepository PsImageShopRepository = new Model.Prestashop.PsImageShopRepository();
                    Model.Prestashop.PsImageShop PsImageShop;
                    foreach (Model.Prestashop.PsImage PsImage in PsImageRepository.ListProduct(pdt))
                    {
                        if (PsImage.IDImage != PsImageCurrent.IDImage)
                        {
                            PsImage.Cover = Convert.ToByte(false);
                            if (PsImageShopRepository.ExistImage(PsImage.IDImage))
                            {
                                PsImageShop = PsImageShopRepository.ReadImage(PsImage.IDImage);
								#if (PRESTASHOP_VERSION_160)
                                PsImageShop.Cover = (sbyte)PsImage.Cover;
								#else
                                PsImageShop.Cover = PsImage.Cover;
								#endif
                            }
                        }
                    }
                    PsImageShopRepository.Save();
                    PsImageRepository.Save();
                }
            }
            return Convert.ToByte(cov);
        }
		#endif
    }
}
