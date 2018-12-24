using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class LigneRemiseMode : INotifyPropertyChanged
    {
        public static string[] LigneRemiseModeText = { "Lignes articles en prix net",
                                                 "Lignes articles en prix brut plus ligne de remise",
                                                 "Lignes articles en prix brut avec remise pourcentage",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.LigneRemiseMode _LigneRemiseMode;

        public int Marq
        {
            get { return (short)_LigneRemiseMode; }
        }

        public string Intitule
        {
            get
            {
                switch (_LigneRemiseMode)
                {
                    case Core.Parametres.LigneRemiseMode.PrixNets:
                    case Core.Parametres.LigneRemiseMode.LigneRemise:
                    case Core.Parametres.LigneRemiseMode.PrixBrutEtRemise:
                        return LigneRemiseModeText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public LigneRemiseMode(Core.Parametres.LigneRemiseMode LigneRemiseModeEnum)
        {
            _LigneRemiseMode = LigneRemiseModeEnum;
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
