using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class FirstNameValue : INotifyPropertyChanged
    {
        public static string[] FirstNameValuesText = { "Abrégé client", // 0
                                                 "Numéro client", // 1
                                                 "Contact", // 2
                                                 "Intitulé compte/adresse partie 1 séparateur", // 3
                                                 "Intitulé compte/adresse partie 2 séparateur", // 4 
                                                 "Vide (caractère espace)", // 5
                                                 "Non défini", "Non défini", "Non défini", "Non défini", // 6-7-8-9
                                                 "Non défini", // 10
                                                 "Non défini", "Non défini", "Non défini", "Non défini", // 11-12-13-14
                                                 "Non défini", // 15
                                                 "Contact partie 1 séparateur", // 16
                                                 "Contact partie 2 séparateur", // 17
                                                 "Non défini", "Non défini", // 18-19
                                                 "Non défini", // 20
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini"};

        #region Properties

        public Core.Parametres.FirstNameValue _FirstNameValue;

        public int Marq
        {
            get { return (short)_FirstNameValue; }
        }

        public string Intitule
        {
            get
            {                
                switch (_FirstNameValue)
                {
                    case Core.Parametres.FirstNameValue.AbregeClient:
                    case Core.Parametres.FirstNameValue.Contact:
                    case Core.Parametres.FirstNameValue.NumeroClient:
                    case Core.Parametres.FirstNameValue.IntituleCompteAdresseP1:
                    case Core.Parametres.FirstNameValue.IntituleCompteAdresseP2:
                    case Core.Parametres.FirstNameValue.Espace:
                    case Core.Parametres.FirstNameValue.ContactP1:
                    case Core.Parametres.FirstNameValue.ContactP2:
                        return FirstNameValuesText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public FirstNameValue(Core.Parametres.FirstNameValue FirstNameValueEnum)
        {
            _FirstNameValue = FirstNameValueEnum;
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
