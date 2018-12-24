using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class MailAccountIdentification : INotifyPropertyChanged
    {
        public static string[] MailIdentificationText = { "Mail fiche client",
                                                 "Mail adresse principale",
                                                 "Mail contact suivant service", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.MailAccountIdentification _MailAccountIdentification;

        public int Marq
        {
            get { return (short)_MailAccountIdentification; }
        }

        public string Intitule
        {
            get
            {
                switch (_MailAccountIdentification)
                {
                    case Core.Parametres.MailAccountIdentification.MailFicheClient:
                    case Core.Parametres.MailAccountIdentification.MailAdressePrincipale:
                    case Core.Parametres.MailAccountIdentification.MailContactService:
                        return MailIdentificationText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public MailAccountIdentification(Core.Parametres.MailAccountIdentification MailAccountIdentificationEnum)
        {
            _MailAccountIdentification = MailAccountIdentificationEnum;
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
