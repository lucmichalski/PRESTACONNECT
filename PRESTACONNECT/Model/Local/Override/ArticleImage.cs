using System;
using PRESTACONNECT.Core;
using System.IO;

namespace PRESTACONNECT.Model.Local
{
    public partial class ArticleImage
    {
        #region Properties

        private string smallImageTempPath;
        public string SmallImageTempPath
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(smallImageTempPath))
                        smallImageTempPath = System.IO.Path.Combine(Global.GetConfig().Folders.Temp, Guid.NewGuid().ToString("N"));

                    string file = string.Empty;
                    if (!File.Exists(smallImageTempPath))
                    {
                        file = SmallFileName;
                        if (!string.IsNullOrEmpty(file) && File.Exists(file))
                            File.Copy(file, smallImageTempPath);
                        // si l'image de base est introuvable
                        else
                            smallImageTempPath = "/PRESTACONNECT;component/Resources/file_broken_small.png";
                    }
                }
                catch (Exception ex)
                {
                    Core.Error.SendMailError(ex.ToString());
                }
                return smallImageTempPath;
            }
        }

        public string GetExtension
        {
            get
            {
                return Path.GetExtension(ImaArt_Image);
            }
        }

        public string advanced_folder
        {
            get
            {
                string localPath = Core.Global.GetConfig().Folders.RootArticle;
                foreach (char directory in ImaArt_Id.ToString())
                {
                    localPath = System.IO.Path.Combine(localPath, directory.ToString());
                    if (!Directory.Exists(localPath))
                        Directory.CreateDirectory(localPath);
                }
                return localPath;
            }
        }

        public string SmallFileName
        {
            get
            {
                string file = string.Empty;
                switch (Core.Global.GetConfig().ConfigLocalStorageMode)
                {
                    case Core.Parametres.LocalStorageMode.simple_system:
                        file = System.IO.Path.Combine(Global.GetConfig().Folders.SmallArticle, ImaArt_Image);
                        break;
                    case Core.Parametres.LocalStorageMode.advanced_system:
                        file = System.IO.Path.Combine(advanced_folder, String.Format("{0}_small" + GetExtension, ImaArt_Id));
                        break;
                }
                return file;
            }
        }

        public string TempFileName
        {
            get
            {
                string file = string.Empty;
                switch (Core.Global.GetConfig().ConfigLocalStorageMode)
                {
                    case Core.Parametres.LocalStorageMode.simple_system:
                        file = System.IO.Path.Combine(Global.GetConfig().Folders.TempArticle, ImaArt_Image);
                        break;
                    case Core.Parametres.LocalStorageMode.advanced_system:
                        file = System.IO.Path.Combine(advanced_folder, String.Format("{0}" + GetExtension, ImaArt_Id));
                        break;
                }
                return file;
            }
        }

        public string FileName(string PsImageType)
        {
            string file = string.Empty;
            switch (Core.Global.GetConfig().ConfigLocalStorageMode)
            {
                case Core.Parametres.LocalStorageMode.simple_system:
                    file = System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootArticle, String.Format("{0}-{1}" + GetExtension, ImaArt_Id, PsImageType));
                    break;
                case Core.Parametres.LocalStorageMode.advanced_system:
                    file = System.IO.Path.Combine(advanced_folder, String.Format("{0}-{1}" + GetExtension, ImaArt_Id, PsImageType));
                    break;
            }
            return file;
        }

        public string isDefaultImage
        {
            get
            {
                if (ImaArt_Default)
                    return "/PRESTACONNECT;component/Resources/valid.png";
                else
                    return "/PRESTACONNECT;component/Resources/delete.png";
            }
        }

        private Boolean attachedToAttributeArticle = false;
        public Boolean AttachedToAttributeArticle
        {
            get { return attachedToAttributeArticle; }
            set { attachedToAttributeArticle = value; SendPropertyChanged("AttachedToAttributeArticle"); SendPropertyChanged("color"); }
        }
        private Boolean attachedToCompositionArticle = false;
        public Boolean AttachedToCompositionArticle
        {
            get { return attachedToCompositionArticle; }
            set { attachedToCompositionArticle = value; SendPropertyChanged("AttachedToCompositionArticle"); SendPropertyChanged("color"); }
        }
        public string color
        {
            get
            {
                return (AttachedToAttributeArticle || AttachedToCompositionArticle) ? System.Windows.Media.Colors.LightSteelBlue.ToString() : System.Windows.Media.Colors.White.ToString();
            }
        }
        #endregion

        #region Methods

        public override string ToString()
        {
            return ImaArt_Name;
        }

        #endregion

        public void EraseFiles()
        {
            try
            {
                if (System.IO.File.Exists(SmallImageTempPath))
                    System.IO.File.Delete(SmallImageTempPath);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            try
            {
                if (System.IO.File.Exists(TempFileName))
                    System.IO.File.Delete(TempFileName);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            try
            {
                if (System.IO.File.Exists(SmallFileName))
                    System.IO.File.Delete(SmallFileName);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            foreach (var item in new Model.Prestashop.PsImageTypeRepository().ListProduct(1))
            {
                try
                {
                    if (System.IO.File.Exists(FileName(item.Name)))
                        System.IO.File.Delete(FileName(item.Name));
                }
                catch (Exception ex)
                {
                    Core.Error.SendMailError(ex.ToString());
                }
            }
        }
    }
}
