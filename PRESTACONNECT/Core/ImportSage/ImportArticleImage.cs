using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PRESTACONNECT.Core.ImportSage
{
    public class ImportArticleImage
    {
        public List<string> logs = new List<string>();

        private Int32 ReadNextPosition(Model.Local.Article Article, int position)
        {
            Int32 Position = (position > 0) ? position : 1;
            try
            {
                List<Int32> ListPosition = new Model.Local.ArticleImageRepository().ListPositionsArticle(Article.Art_Id);
                while (ListPosition.Contains(Position))
                    Position += 1;
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return Position;
        }

        public Boolean Exec(String PathImg, Int32 ArticleSend)
        {
            return this.Exec(PathImg, ArticleSend, 1, 0);
        }
        public Boolean Exec(String PathImg, Int32 ArticleSend, int Declination)
        {
            return this.Exec(PathImg, ArticleSend, 1, Declination);
        }

        public Boolean Exec(String PathImg, Int32 ArticleSend, int position, int Declination)
        {
            Boolean result = false;
            Int32 IdArticleImage = 0;
            try
            {
                String extension = Path.GetExtension(PathImg);
                String FileName = Path.GetFileName(PathImg);

                Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();

                if (!ArticleImageRepository.ExistArticleFile(ArticleSend, FileName))
                {
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                    Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                    Model.Local.ArticleImage ArticleImage = new Model.Local.ArticleImage()
                    {
                        Art_Id = Article.Art_Id,
                        ImaArt_Name = Article.Art_Name,
                        ImaArt_Image = "",
                        ImaArt_DateAdd = DateTime.Now,
                        ImaArt_SourceFile = FileName
                    };
                    ArticleImage.ImaArt_Position = this.ReadNextPosition(Article, position);
                    ArticleImage.ImaArt_Default = !(new Model.Local.ArticleImageRepository().ExistArticleDefault(Article.Art_Id, true));
                    ArticleImageRepository.Add(ArticleImage);
                    ArticleImage.ImaArt_Image = String.Format("{0}" + extension, ArticleImage.ImaArt_Id);
                    ArticleImageRepository.Save();
                    IdArticleImage = ArticleImage.ImaArt_Id;

                    string uri = PathImg.Replace("File:///", "").Replace("file:///", "").Replace("File://", "\\\\").Replace("file://", "\\\\").Replace("/", "\\");

                    System.IO.File.Copy(uri, ArticleImage.TempFileName);

                    Model.Prestashop.PsImageTypeRepository PsImageTypeRepository = new Model.Prestashop.PsImageTypeRepository();
                    List<Model.Prestashop.PsImageType> ListPsImageType = PsImageTypeRepository.ListProduct(1);

                    System.Drawing.Image img = System.Drawing.Image.FromFile(ArticleImage.TempFileName);

                    foreach (Model.Prestashop.PsImageType PsImageType in ListPsImageType)
                        Core.Img.resizeImage(img, Convert.ToInt32(PsImageType.Width), Convert.ToInt32(PsImageType.Height),
                            ArticleImage.FileName(PsImageType.Name));

                    Core.Img.resizeImage(img, Core.Global.GetConfig().ConfigImageMiniatureWidth, Core.Global.GetConfig().ConfigImageMiniatureHeight,
                            ArticleImage.SmallFileName);

                    img.Dispose();

                    // <JG> 28/10/2015 ajout attribution gamme/images
                    if (Declination != 0)
                    {
                        if (ArticleImage.Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMonoGamme
                                      || ArticleImage.Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMultiGammes)
                        {
                            Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                            if (AttributeArticleRepository.Exist(Declination))
                            {
                                Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new Model.Local.AttributeArticleImageRepository();
                                if (!AttributeArticleImageRepository.ExistAttributeArticleImage(Declination, ArticleImage.ImaArt_Id))
                                {
                                    AttributeArticleImageRepository.Add(new Model.Local.AttributeArticleImage()
                                    {
                                        AttArt_Id = Declination,
                                        ImaArt_Id = ArticleImage.ImaArt_Id,
                                    });
                                }
                            }
                        }
                        else if (ArticleImage.Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition)
                        {
                            Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                            if (CompositionArticleRepository.Exist(Declination))
                            {
                                Model.Local.CompositionArticleImageRepository CompositionArticleImageRepository = new Model.Local.CompositionArticleImageRepository();
                                if (!CompositionArticleImageRepository.ExistCompositionArticleImage(Declination, ArticleImage.ImaArt_Id))
                                {
                                    CompositionArticleImageRepository.Add(new Model.Local.CompositionArticleImage()
                                    {
                                        ComArt_Id = Declination,
                                        ImaArt_Id = ArticleImage.ImaArt_Id,
                                    });
                                }
                            }
                        }
                    }

                    result = true;
                }
                else if (Core.Global.GetConfig().ImportImageReplaceFiles)
                {
                    FileInfo importfile = new FileInfo(PathImg);
                    Model.Local.ArticleImage ArticleImage = ArticleImageRepository.ReadArticleFile(ArticleSend, FileName);
                    FileInfo existfile = new FileInfo(ArticleImage.TempFileName);
                    if ((ArticleImage.ImaArt_DateAdd == null || importfile.LastWriteTime > ArticleImage.ImaArt_DateAdd)
                        || importfile.Length != existfile.Length)
                    {
                        try
                        {
                            // import nouveau fichier
                            string uri = PathImg.Replace("File:///", "").Replace("file:///", "").Replace("File://", "\\\\").Replace("file://", "\\\\").Replace("/", "\\");
                            System.IO.File.Copy(uri, ArticleImage.TempFileName, true);

                            Model.Prestashop.PsImageTypeRepository PsImageTypeRepository = new Model.Prestashop.PsImageTypeRepository();
                            List<Model.Prestashop.PsImageType> ListPsImageType = PsImageTypeRepository.ListProduct(1);

                            System.Drawing.Image img = System.Drawing.Image.FromFile(ArticleImage.TempFileName);

                            foreach (Model.Prestashop.PsImageType PsImageType in ListPsImageType)
                                Core.Img.resizeImage(img, Convert.ToInt32(PsImageType.Width), Convert.ToInt32(PsImageType.Height),
                                    ArticleImage.FileName(PsImageType.Name));

                            Core.Img.resizeImage(img, Core.Global.GetConfig().ConfigImageMiniatureWidth, Core.Global.GetConfig().ConfigImageMiniatureHeight,
                                    ArticleImage.SmallFileName);
                            Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();

                            if (ArticleImage.Pre_Id != null && PsImageRepository.ExistImage((uint)ArticleImage.Pre_Id))
                            {
                                String FTP = Core.Global.GetConfig().ConfigFTPIP;
                                String User = Core.Global.GetConfig().ConfigFTPUser;
                                String Password = Core.Global.GetConfig().ConfigFTPPassword;

                                Model.Prestashop.PsImage PsImage = PsImageRepository.ReadImage((uint)ArticleImage.Pre_Id);

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

                                                System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(FTP + ftpPath);

                                                request.Credentials = new System.Net.NetworkCredential(User, Password);
                                                request.UsePassive = true;
                                                request.UseBinary = true;
                                                request.KeepAlive = false;

                                                request.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory;

                                                System.Net.FtpWebResponse makeDirectoryResponse = (System.Net.FtpWebResponse)request.GetResponse();

                                            }
                                            catch //Exception ex
                                            {
                                                //System.Windows.MessageBox.Show(ex.ToString());
                                            }
                                            #endregion
                                        }
                                        break;
                                        #endregion
                                }

                                #region Upload des images
                                extension = ArticleImage.GetExtension;
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
                                    String localfile = ArticleImage.FileName(PsImageType.Name);
                                    if (System.IO.File.Exists(localfile))
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
                                        System.IO.FileStream fs = System.IO.File.OpenRead(localfile);
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
                            }

                            ArticleImage.ImaArt_DateAdd = DateTime.Now;
                            ArticleImageRepository.Save();

                            // <JG> 28/10/2015 ajout attribution gamme/images
                            if (Declination != 0)
                            {
                                if (ArticleImage.Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMonoGamme
                                    || ArticleImage.Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleMultiGammes)
                                {
                                    Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                                    if (AttributeArticleRepository.Exist(Declination))
                                    {
                                        Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new Model.Local.AttributeArticleImageRepository();
                                        if (!AttributeArticleImageRepository.ExistAttributeArticleImage(Declination, ArticleImage.ImaArt_Id))
                                        {
                                            AttributeArticleImageRepository.Add(new Model.Local.AttributeArticleImage()
                                            {
                                                AttArt_Id = Declination,
                                                ImaArt_Id = ArticleImage.ImaArt_Id,
                                            });
                                        }

                                        // réaffectation côté PrestaShop
                                        Model.Local.AttributeArticle AttributeArticle = AttributeArticleRepository.Read(Declination);
                                        if (AttributeArticle.Pre_Id != null
                                            && AttributeArticle.Pre_Id != 0)
                                        {
                                            Model.Prestashop.PsProductAttributeImageRepository PsProductAttributeImageRepository = new Model.Prestashop.PsProductAttributeImageRepository();
                                            if (PsProductAttributeImageRepository.ExistProductAttributeImage((UInt32)AttributeArticle.Pre_Id, (UInt32)ArticleImage.Pre_Id) == false)
                                            {
                                                PsProductAttributeImageRepository.Add(new Model.Prestashop.PsProductAttributeImage()
                                                {
                                                    IDImage = (UInt32)ArticleImage.Pre_Id,
                                                    IDProductAttribute = (UInt32)AttributeArticle.Pre_Id,
                                                });
                                            }
                                        }
                                    }
                                }
                                else if (ArticleImage.Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition)
                                {
                                    Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                                    if (CompositionArticleRepository.Exist(Declination))
                                    {
                                        Model.Local.CompositionArticleImageRepository CompositionArticleImageRepository = new Model.Local.CompositionArticleImageRepository();
                                        if (!CompositionArticleImageRepository.ExistCompositionArticleImage(Declination, ArticleImage.ImaArt_Id))
                                        {
                                            CompositionArticleImageRepository.Add(new Model.Local.CompositionArticleImage()
                                            {
                                                ComArt_Id = Declination,
                                                ImaArt_Id = ArticleImage.ImaArt_Id,
                                            });
                                        }

                                        // réaffectation côté PrestaShop
                                        Model.Local.CompositionArticle CompositionArticle = CompositionArticleRepository.Read(Declination);
                                        if (CompositionArticle.Pre_Id != null
                                            && CompositionArticle.Pre_Id != 0)
                                        {
                                            Model.Prestashop.PsProductAttributeImageRepository PsProductAttributeImageRepository = new Model.Prestashop.PsProductAttributeImageRepository();
                                            if (PsProductAttributeImageRepository.ExistProductAttributeImage((UInt32)CompositionArticle.Pre_Id, (UInt32)ArticleImage.Pre_Id) == false)
                                            {
                                                PsProductAttributeImageRepository.Add(new Model.Prestashop.PsProductAttributeImage()
                                                {
                                                    IDImage = (UInt32)ArticleImage.Pre_Id,
                                                    IDProductAttribute = (UInt32)CompositionArticle.Pre_Id,
                                                });
                                            }
                                        }
                                    }
                                }
                            }

                            result = true;

                            logs.Add("II30- Remplacement de l'image " + ArticleImage.ImaArt_SourceFile + " en position " + ArticleImage.ImaArt_Position + " pour l'article " + ArticleImage.Article.Art_Ref);
                        }
                        catch (Exception ex)
                        {
                            Core.Error.SendMailError(ex.ToString());
                            logs.Add("II39- Erreur lors du remplacement de l'image " + ArticleImage.ImaArt_SourceFile + " en position " + ArticleImage.ImaArt_Position + " pour l'article " + ArticleImage.Article.Art_Ref);
                            logs.Add(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
                if (ex.ToString().Contains("System.UnauthorizedAccessException") && IdArticleImage != 0)
                {
                    Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                    ArticleImageRepository.Delete(ArticleImageRepository.ReadArticleImage(IdArticleImage));
                }
            }
            return result;
        }
    }
}
