using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class MailNotification : INotifyPropertyChanged
    {
        public static string[] MailNotificationText = { "A. Au mail du compte",
                                               "B. [A] + mail de l'adresse principale ou mail de la fiche client",
                                               "C. [B] + mail des adresses de livraisons non principales",
                                               "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.MailNotification _MailNotification;

        public int Marq
        {
            get { return (short)_MailNotification; }
        }

        public string Intitule
        {
            get
            {
                switch (_MailNotification)
                {
                    case Core.Parametres.MailNotification.AccountMail:
                    case Core.Parametres.MailNotification.AccountMailAndAlternative:
                    case Core.Parametres.MailNotification.AllMail:
                        return MailNotificationText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public MailNotification(Core.Parametres.MailNotification MailNotificationEnum)
        {
            _MailNotification = MailNotificationEnum;
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
