using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class LocalStorageMode : INotifyPropertyChanged
    {
        public static string[] LocalStorageModeText = { "Stockage simple",
                                                          "Stockage avancé",
                                                          "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.LocalStorageMode _LocalStorageMode;

        public int Marq
        {
            get { return (short)_LocalStorageMode; }
        }

        public string Intitule
        {
            get
            {
                switch (_LocalStorageMode)
                {
                    case Core.Parametres.LocalStorageMode.simple_system:
                    case Core.Parametres.LocalStorageMode.advanced_system:
                        return LocalStorageModeText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public LocalStorageMode(Core.Parametres.LocalStorageMode LocalStorageModeEnum)
        {
            _LocalStorageMode = LocalStorageModeEnum;
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
