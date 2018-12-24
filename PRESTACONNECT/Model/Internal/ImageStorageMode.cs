using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class ImageStorageMode : INotifyPropertyChanged
    {
        public static string[] ImageStorageModeText = { "Nouveau système (Prestashop 1.5 uniquement)",
                                                 "Ancien système",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.ImageStorageMode _ImageStorageMode;

        public int Marq
        {
            get { return (short)_ImageStorageMode; }
        }

        public string Intitule
        {
            get
            {
                switch (_ImageStorageMode)
                {
                    case Core.Parametres.ImageStorageMode.new_system:
                    case Core.Parametres.ImageStorageMode.old_system:
                        return ImageStorageModeText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public ImageStorageMode(Core.Parametres.ImageStorageMode ImageStorageModeEnum)
        {
            _ImageStorageMode = ImageStorageModeEnum;
        }

        #endregion
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Intitule;
        }

        #endregion
    }
}
