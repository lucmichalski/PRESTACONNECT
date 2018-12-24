using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsOrders
    {
        private string cart_URL = string.Empty;
        public string Cart_URL
        {
            get { return cart_URL; }
            set { cart_URL = value; }
        }

        private string mail_Invoice_numbers = string.Empty;
        public string Mail_Invoice_numbers
        {
            get { return mail_Invoice_numbers; }
            set { mail_Invoice_numbers = value; }
        }

        private string tracking_Number = string.Empty;
        public string Tracking_Number
        {
            get { return tracking_Number; }
            set { tracking_Number = value; }
        }

        #region Methods

        public override string ToString()
        {
            return "Commande numéro : " + IDOrder.ToString();
        }

        #endregion
    }

    public class idorder
    {
        public uint id_order = 0;
        public uint current_state = 0;
    }
    public class order_payment
    {
        public uint id_order = 0;
        public string reference = string.Empty;
    }

    public class order_address
    {
        public uint id_order = 0;
        public uint id_cart = 0;
        public uint id_shop = 0;
        public uint id_customer = 0;
        public uint id_address_invoice = 0;
        public uint id_address_delivery = 0;
    }

    public class ps_orders_resume : INotifyPropertyChanged
    {
        private uint _id_order = 0;
		private string _reference = string.Empty;
		private uint _current_state = 0;
        private uint _id_customer = 0;
        private string _lastname = string.Empty;
        private string _firstname = string.Empty;
        private string _company = string.Empty;
        private decimal _total_paid_tax_incl = 0;
        private decimal _total_paid_tax_excl = 0;
        private string _payment = string.Empty;
        private string _order_state_name = string.Empty;
        private DateTime _date_add;

        public uint id_order
        {
            get { return _id_order; }
            set { _id_order = value; OnPropertyChanged("id_order"); }
		}
		public string reference
		{
			get { return _reference; }
			set { _reference = value; OnPropertyChanged("reference"); }
		}
		public uint current_state
        {
            get { return _current_state; }
            set { _current_state = value; OnPropertyChanged("current_state"); }
        }
        public uint id_customer
        {
            get { return _id_customer; }
            set { _id_customer = value; OnPropertyChanged("id_customer"); }
        }
        public string lastname
        {
            get { return _lastname; }
            set { _lastname = value; OnPropertyChanged("lastname"); }
        }
        public string firstname
        {
            get { return _firstname; }
            set { _firstname = value; OnPropertyChanged("firstname"); }
        }
        public string company
        {
            get { return _company; }
            set { _company = value; OnPropertyChanged("company"); }
        }
        public decimal total_paid_tax_incl
        {
            get { return _total_paid_tax_incl; }
            set { _total_paid_tax_incl = value; OnPropertyChanged("total_paid_tax_incl"); }
        }
        public decimal total_paid_tax_excl
        {
            get { return _total_paid_tax_excl; }
            set { _total_paid_tax_excl = value; OnPropertyChanged("total_paid_tax_excl"); }
        }
        public string payment
        {
            get { return _payment; }
            set { _payment = value; OnPropertyChanged("payment"); }
        }
        public string order_state_name
        {
            get { return _order_state_name; }
            set { _order_state_name = value; OnPropertyChanged("order_state_name"); }
        }
        public DateTime date_add
        {
            get { return _date_add; }
            set { _date_add = value; OnPropertyChanged("date_add"); }
        }

        public string DOPiece
        {
            get
            {
                string r = string.Empty;
                Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Sage.F_DOCENTETERepository();
                if (F_DOCENTETERepository.ExistWeb(id_order.ToString()))
                    r = F_DOCENTETERepository.ListWeb(id_order.ToString()).First().DO_Piece;
                return r;
            }
        }

        public string Client
        {
            get { return (string.IsNullOrWhiteSpace(company) ? lastname + " " + firstname : company); }
        }

        public Boolean Sync
        {
            get { return new Model.Local.OrderRepository().ExistPrestashop((int)id_order); }
        }

        public Model.Prestashop.PsOrders PsSource
        {
            get { return new Model.Prestashop.PsOrdersRepository().ReadOrder((int)id_order); }
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
    }
}
