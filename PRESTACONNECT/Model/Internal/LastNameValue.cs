using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class LastNameValue : INotifyPropertyChanged
    {
        public static string[] LastNameValuesText = { "Intitulé compte/adresse", // 0
                                                 "Numéro client", // 1
                                                 "Abrégé client", // 2
                                                 "Intitulé compte/adresse partie 1 séparateur", // 3
                                                 "Intitulé compte/adresse partie 2 séparateur", // 4
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", // 5-6-7-8-9
                                                 "Non défini", // 10
                                                 "Non défini", "Non défini", "Non défini", "Non défini", // 11-12-13-14
                                                 "Contact", // 15
                                                 "Contact partie 1 séparateur", // 16
                                                 "Contact partie 2 séparateur", // 17
                                                 "Non défini", "Non défini", // 18-19
                                                 "Vide (caractère espace)", // 20
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini"};

        #region Properties

        public Core.Parametres.LastNameValue _LastNameValue;

        public int Marq
        {
            get { return (short)_LastNameValue; }
        }

        public string Intitule
        {
            get
            {                
                switch (_LastNameValue)
                {
                    case Core.Parametres.LastNameValue.AbregeClient:
                    case Core.Parametres.LastNameValue.IntituleCompteAdresse:
                    case Core.Parametres.LastNameValue.NumeroClient:
                    case Core.Parametres.LastNameValue.IntituleCompteAdresseP1:
                    case Core.Parametres.LastNameValue.IntituleCompteAdresseP2:
                    case Core.Parametres.LastNameValue.Contact:
                    case Core.Parametres.LastNameValue.ContactP1:
                    case Core.Parametres.LastNameValue.ContactP2:
                    case Core.Parametres.LastNameValue.Espace:
                        return LastNameValuesText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public LastNameValue(Core.Parametres.LastNameValue LastNameValueEnum)
        {
            _LastNameValue = LastNameValueEnum;
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
