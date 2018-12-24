using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsFeatureShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsFeatureShop> ListShop(uint IdShop)
        {
            return this.DBPrestashop.PsFeatureShop.Where(f => f.IDShop == IdShop).ToList();
        }

        public Boolean ExistInShop(UInt32 Feature, UInt32 Shop)
        {
            return this.DBPrestashop.PsFeatureShop.Count(f => f.IDFeature == Feature && f.IDShop == Shop) > 0;
        }

        public void Add(PsFeatureShop obj)
        {
            this.DBPrestashop.PsFeatureShop.InsertOnSubmit(obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }
    }
}
