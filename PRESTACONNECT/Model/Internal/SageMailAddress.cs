using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class SageMailAddress : INotifyPropertyChanged
    {
        #region Properties
        private String mailAddress;

        public String MailAddress
        {
            get { return mailAddress; }
            set 
            {
                mailAddress = value;
                OnPropertyChanged("MailAddress");
            }
        }

        private Core.Parametres.SageAddressType type;

        public Core.Parametres.SageAddressType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }


        private Boolean isAccountMail;
        public Boolean IsAccountMail
        {
            get { return isAccountMail; }
            set
            {
                isAccountMail = value;
                OnPropertyChanged("IsAccountMail");
            }
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
    }
}
