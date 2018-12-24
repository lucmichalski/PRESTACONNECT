using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class MailToContactService : INotifyPropertyChanged
    {
        #region Properties

        public Model.Sage.P_SERVICECPTA P_SERVICECPTA;

        public string S_Intitule
        {
            get
            {
                return (P_SERVICECPTA != null) ? this.P_SERVICECPTA.S_Intitule : "";
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

        public MailToContactService(Model.Sage.P_SERVICECPTA ServiceContact)
        {
            P_SERVICECPTA = ServiceContact;
            MailTo = Core.Global.GetConfig().TransfertNotifyAccountSageContactService.Contains(P_SERVICECPTA.cbMarq);
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
            return S_Intitule;
        }

        #endregion
    }
}
