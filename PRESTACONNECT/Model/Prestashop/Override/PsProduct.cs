using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Prestashop
{
    partial class PsProduct
    {
        //public PsProduct()
        //{
        //    IDSupplier = 0;
        //    IDManufacturer = 0;
        //    Upc = "";
        //    Unity = "";
        //    SupplierReference = "";
        //    Location = "";
        //    QuantityDiscount = 0;
        //    CacheDefaultAttribute = 0;
        //}

        public decimal ecotaxe_htsage = 0;
        public bool taxe_famillesage = false;

        public string Name
        {
            get
            {
                string value = string.Empty;
                Model.Prestashop.PsProductLangRepository PsProductLangRepository = new Model.Prestashop.PsProductLangRepository();
                if (PsProductLangRepository.ExistProductLang((int)IDProduct, Core.Global.Lang, Core.Global.CurrentShop.IDShop))
                {
                    Model.Prestashop.PsProductLang PsProductLang = PsProductLangRepository.ReadProductLang((int)IDProduct, Core.Global.Lang, Core.Global.CurrentShop.IDShop);
                    value = PsProductLang.Name;
                }
                return value;
            }
        }

        public string ComboText
        {
            get
            {
                return IDProduct + " - " + Name;
            }
        }

        private PsAEcStock aec_stock;
        public PsAEcStock AEC_Stock
        {
            get { return aec_stock; }
            set { aec_stock = value; }
        }

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }

    public class ProductLight
    {
        public uint id_product;
        public string name;

        public string ComboText
        {
            get { return "[" + id_product.ToString() + "] " + name; }
        }

        #region Methods

        public override string ToString()
        {
            return name;
        }

        #endregion
    }

    public class ProductUpdate : INotifyPropertyChanged
    {
        private uint _id_product;
        public uint id_product
        {
            get { return _id_product; }
            set { _id_product = value; OnPropertyChanged("id_product"); }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("name"); }
        }

        private DateTime? _date_upd;
        public DateTime? date_upd
        {
            get { return _date_upd; }
            set { _date_upd = value; OnPropertyChanged("date_upd"); }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProductResume : INotifyPropertyChanged
    {
        private uint _id_product;
        public uint id_product
        {
            get { return _id_product; }
            set { _id_product = value; OnPropertyChanged("id_product"); }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("name"); }
        }

        private string _reference;
        public string reference
        {
            get { return _reference; }
            set { _reference = value; OnPropertyChanged("reference"); }
        }

        private string _default_category;
        public string default_category
        {
            get { return _default_category; }
            set { _default_category = value; OnPropertyChanged("default_category"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Product_Redirection
    {
        public String redirect_type = string.Empty;
		#if (PRESTASHOP_VERSION_172)
		public uint id_type_redirected = 0;
		#else
		public uint id_product_redirected = 0;
		#endif
	}
}
