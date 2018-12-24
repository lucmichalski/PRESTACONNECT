using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class SageAddressToPrestashop : INotifyPropertyChanged
    {
        #region Properties

        private Model.Prestashop.PsAddress psAddressAdd;

        public Model.Prestashop.PsAddress PsAddressAdd
        {
            get { return psAddressAdd; }
            set 
            {
                psAddressAdd = value;
                OnPropertyChanged("IsAccountMail"); 
            }
        }

        private Core.Parametres.SageAddressType type;

        public Core.Parametres.SageAddressType Type
        {
            get { return type; }
            set 
            {
                type = value;
                OnPropertyChanged("IsAccountMail"); 
            }
        }

        private int _F_LIVRAISON_cbMarq;
        public int F_LIVRAISON_cbMarq
        {
            get { return _F_LIVRAISON_cbMarq; }
            set
            {
                _F_LIVRAISON_cbMarq = value;
                OnPropertyChanged("IsAccountMail");
            }
        }
        #endregion
        
        #region Constructors

        public SageAddressToPrestashop(Model.Prestashop.PsAddress PsAddress, Core.Parametres.SageAddressType AddressType, int cbMarq)
        {
            this.PsAddressAdd = PsAddress;
            this.Type = AddressType;
            this.F_LIVRAISON_cbMarq = cbMarq;
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
