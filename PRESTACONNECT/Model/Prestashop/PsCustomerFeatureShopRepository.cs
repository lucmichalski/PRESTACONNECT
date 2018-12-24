using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerFeatureShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsCustomerFeatureShop> ListShop(uint IdShop)
        {
            return this.DBPrestashop.PsCustomerFeatureShop.Where(f => f.IDShop == IdShop).ToList();
        }

        public Boolean ExistInShop(UInt32 CustomerFeature, UInt32 Shop)
        {
            return this.DBPrestashop.PsCustomerFeatureShop.Count(f => f.IDCustomerFeature == CustomerFeature && f.IDShop == Shop) > 0;
        }

        public void Add(PsCustomerFeatureShop obj)
        {
            this.DBPrestashop.PsCustomerFeatureShop.InsertOnSubmit(obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }
    }
}
