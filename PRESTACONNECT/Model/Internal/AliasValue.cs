using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class AliasValue : INotifyPropertyChanged
    {
        public static string[] AliasValuesText = { "Intitulé compte/adresse",
                                                 "Numéro client",
                                                 "Abrégé client",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.AliasValue _AliasValue;

        public int Marq
        {
            get { return (short)_AliasValue; }
        }

        public string Intitule
        {
            get
            {
                switch (_AliasValue)
                {
                    case Core.Parametres.AliasValue.AbregeClient:
                    case Core.Parametres.AliasValue.IntituleCompteAdresse:
                    case Core.Parametres.AliasValue.NumeroClient:
                        return AliasValuesText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public AliasValue(Core.Parametres.AliasValue AliasValueEnum)
        {
            _AliasValue = AliasValueEnum;
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
