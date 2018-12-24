using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class MailToContactType : INotifyPropertyChanged
    {
        #region Properties

        public Model.Sage.P_CONTACT P_CONTACT;

        public string C_Intitule
        {
            get
            {
                return (P_CONTACT != null) ? this.P_CONTACT.C_Intitule : "";
            }
        }

        private bool _MailTo = false;

        public bool MailTo
        {
            get { return _MailTo; }
            set
            {
                _MailTo = value;
                OnPropertyChanged("MailTo");
            }
        }

        #endregion
        #region Constructors

        public MailToContactType(Model.Sage.P_CONTACT TypeContact)
        {
            P_CONTACT = TypeContact;
            MailTo = Core.Global.GetConfig().TransfertNotifyAccountSageContactType.Contains(P_CONTACT.cbMarq);
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
            return C_Intitule;
        }

        #endregion
    }
}
