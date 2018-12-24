using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCategoryShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public bool ExistCategoryShop(UInt32 IDCategory, UInt32 IDShop)
        {
            return DBPrestashop.PsCategoryShop.Count(c => c.IDShop == IDShop && c.IDCategory == IDCategory) > 0;
        }
        public PsCategoryShop ReadCategoryShop(UInt32 IDCategory, UInt32 IDShop)
        {
            return DBPrestashop.PsCategoryShop.FirstOrDefault(c => c.IDShop == IDShop && c.IDCategory == IDCategory);
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCategoryShop Obj)
        {
            this.DBPrestashop.PsCategoryShop.InsertOnSubmit(Obj);
            this.Save();
        }
		
        public void Delete(PsCategoryShop Obj)
        {
            this.DBPrestashop.PsCategoryShop.DeleteOnSubmit(Obj);
            this.Save();
        }
    }
}
