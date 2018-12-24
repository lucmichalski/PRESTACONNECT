using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class CompanyValue : INotifyPropertyChanged
    {
        public static string[] CompanyValuesText = { "'Vide'",
                                                 "Contact",
                                                 "Abrégé client",
                                                 "Intitulé compte/adresse",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.CompanyValue _CompanyValue;

        public int Marq
        {
            get { return (short)_CompanyValue; }
        }

        public string Intitule
        {
            get
            {                
                switch (_CompanyValue)
                {
                    case Core.Parametres.CompanyValue.AbregeClient:
                    case Core.Parametres.CompanyValue.IntituleCompteAdresse:
                    case Core.Parametres.CompanyValue.Empty:
                    case Core.Parametres.CompanyValue.Contact:
                        return CompanyValuesText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public CompanyValue(Core.Parametres.CompanyValue CompanyValueEnum)
        {
            _CompanyValue = CompanyValueEnum;
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
