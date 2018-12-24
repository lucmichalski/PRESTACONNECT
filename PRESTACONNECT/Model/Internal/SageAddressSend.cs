using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class SageAddressSend : INotifyPropertyChanged
    {
        public static string[] SageAddressSendText = { "Adresse fiche client (facturation)",
                                                 "Adresse de livraison principale",
                                                 "Autre(s) adresse(s) de livraison",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.SageAddressSend _SageAddressSend;

        public int Marq
        {
            get { return (short)_SageAddressSend; }
        }

        public string Intitule
        {
            get
            {                
                switch (_SageAddressSend)
                {
                    case Core.Parametres.SageAddressSend.AutreAdresse:
                    case Core.Parametres.SageAddressSend.AdressePrincipale:
                    case Core.Parametres.SageAddressSend.AdresseFacturation:
                        return SageAddressSendText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        private bool _ToSend = false;

        public bool ToSend
        {
            get { return _ToSend; }
            set
            {
                _ToSend = value;
                OnPropertyChanged("ToSend");
            }
        }

        #endregion
        #region Constructors

        public SageAddressSend(Core.Parametres.SageAddressSend SageAddressSendEnum)
        {
            _SageAddressSend = SageAddressSendEnum;
            ToSend = Core.Global.GetConfig().TransfertSageAddressSend.Contains(Marq);
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
