using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsCustomer
    {
        // 16/08/2013 move to pscustomerrepository => btoc_customer
        //public string CompteSage
        //{
        //    get
        //    {
        //        string result = "";
        //        Model.Local.CustomerRepository CustomerRepository = new Local.CustomerRepository();
        //        if (CustomerRepository.ExistPrestashop((Int32)this.IDCustomer))
        //        {
        //            Model.Local.Customer Customer = CustomerRepository.ReadPrestashop((Int32)this.IDCustomer);
        //            Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Sage.F_COMPTETRepository();
        //            if (F_COMPTETRepository.ExistId(Customer.Sag_Id))
        //            {
        //                Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.ReadId(Customer.Sag_Id);
        //                result = F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule;
        //                F_COMPTET = null;
        //            }
        //            F_COMPTETRepository = null;
        //        }
        //        CustomerRepository = null;
        //        return result;
        //    }
        //}

        #region Methods

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        #endregion
    }
    public class btob_customer
    {
        public uint id_customer = 0;
        public string firstname = string.Empty;
        public string lastname = string.Empty;
        public string email = string.Empty;
        public string company = string.Empty;

        public string BtoBString
        {
            get
            {
                return id_customer + " - " + firstname + " " + lastname + ((!string.IsNullOrWhiteSpace(company)) ? " / " + company : string.Empty) + " / " + email + "";
            }
        }
        public string BtoBShortString
        {
            get
            {
                return firstname + " " + lastname + ((!string.IsNullOrWhiteSpace(company)) ? " / " + company : string.Empty);
            }
        }

        #region Methods

        public override string ToString()
        {
            return BtoBString;
        }

        #endregion
    }
    public class btoc_customer : INotifyPropertyChanged
    {
        private uint _id_customer = 0;
        public uint id_customer
        {
            get { return _id_customer; }
            set { _id_customer = value; OnPropertyChanged("id_customer"); OnPropertyChanged("CompteSage"); }
        }

        private string _firstname = string.Empty;
        public string firstname
        {
            get { return _firstname; }
            set { _firstname = value; OnPropertyChanged("firstname"); }
        }
        private string _lastname = string.Empty;
        public string lastname
        {
            get { return _lastname; }
            set { _lastname = value; OnPropertyChanged("lastname"); }
        }
        private string _company = string.Empty;
        public string company
        {
            get { return _company; }
            set { _company = value; OnPropertyChanged("company"); }
        }
        private string _email = string.Empty;
        public string email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged("email"); }
        }
        private DateTime _date_add = new DateTime(1900, 01, 01);
        public DateTime date_add
        {
            get { return _date_add; }
            set { _date_add = value; OnPropertyChanged("date_add"); }
        }

        public string Client
        {
            get { return (string.IsNullOrWhiteSpace(company) ? lastname + " " + firstname : company); }
        }

        public string CompteSage
        {
            get
            {
                string result = string.Empty;

                string r = string.Empty;
                if (Core.Temp.ListLocalCustomer.Count(l => l.Pre_Id == (int)id_customer) == 1)
                {
                    Model.Local.Customer local = Core.Temp.ListLocalCustomer.FirstOrDefault(l => l.Pre_Id == (int)id_customer);
                    if (Core.Temp.ListF_COMPTET_BtoB.Count(s => s.cbMarq == local.Sag_Id) == 1)
                    {
                        r = Core.Temp.ListF_COMPTET_BtoB.FirstOrDefault(s => s.cbMarq == local.Sag_Id).ToString();
                    }
                }
                return r;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Methods

        public override string ToString()
        {
            return "Client Prestashop : " + firstname + " " + lastname;
        }

        #endregion
    }
}
