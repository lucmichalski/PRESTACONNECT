using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class LockPhoneNumber : INotifyPropertyChanged
    {
        public static string[] LockPhoneNumberText = { "1) Reprise numéros première adresse (facturation [ou] principale livraison)",
                                                 "2) Saisie fictive",
                                                 "3) Blocage du transfert de l'adresse",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.LockPhoneNumber _LockPhoneNumber;

        public int Marq
        {
            get { return (short)_LockPhoneNumber; }
        }

        public string Intitule
        {
            get
            {
                switch (_LockPhoneNumber)
                {
                    case Core.Parametres.LockPhoneNumber.RecopyFirst:
                    case Core.Parametres.LockPhoneNumber.ReplaceEntry:
                    case Core.Parametres.LockPhoneNumber.LockAddress:
                        return LockPhoneNumberText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        private bool _Active = false;

        public bool Active
        {
            get { return _Active; }
            set
            {
                _Active = value;
                OnPropertyChanged("Active");
            }
        }

        #endregion
        #region Constructors

        public LockPhoneNumber(Core.Parametres.LockPhoneNumber LockPhoneNumberEnum)
        {
            _LockPhoneNumber = LockPhoneNumberEnum;
            Active = Core.Global.GetConfig().TransfertLockPhoneNumber.Contains(Marq);
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
