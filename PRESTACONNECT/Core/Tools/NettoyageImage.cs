using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PRESTACONNECT.Core.Tools
{
    public class NettoyageImage
    {
        public void Exec(Int32 ImageSend)
        {
            try
            {
                Model.Prestashop.PsImageRepository PsImageRepository = new Model.Prestashop.PsImageRepository();
                Model.Prestashop.PsImageShopRepository PsImageShopRepository = new Model.Prestashop.PsImageShopRepository();
                if (PsImageRepository.ExistImage((UInt32)ImageSend))
                {
                    Model.Prestashop.PsImage PsImage = PsImageRepository.ReadImage(Convert.ToUInt32(ImageSend));

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

                            foreach (char directory in PsImage.IDImage.ToString())
                                ftpPath += directory + "/";
                            break;
                            #endregion
                    }

                    string ftpfullpath = (Core.Global.GetConfig().ConfigImageStorageMode == Core.Parametres.ImageStorageMode.old_system)
                            ? FTP + ftpPath + PsImage.IDProduct + "-" + PsImage.IDImage + ".jpg"
                            : FTP + ftpPath + PsImage.IDImage + ".jpg";

                    if (!Core.Ftp.ExistFile(ftpfullpath, User, Password))
                    {
                        PsImageShopRepository.DeleteAll(PsImageShopRepository.List(PsImage.IDImage));
                        PsImageRepository.Delete(PsImage);
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
