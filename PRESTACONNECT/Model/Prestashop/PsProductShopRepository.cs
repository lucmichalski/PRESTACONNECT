using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductShop Obj)
        {
            this.DBPrestashop.PsProductShop.InsertOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistProductShop(UInt32 Product, UInt32 Shop)
        {
            if (this.DBPrestashop.PsProductShop.Count(Obj => Obj.IDProduct == Product && Obj.IDShop == Shop) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PsProductShop ReadProductShop(UInt32 Product, UInt32 Shop)
        {
            return this.DBPrestashop.PsProductShop.FirstOrDefault(Obj => Obj.IDProduct == Product && Obj.IDShop == Shop);
        }

        public void WriteDate(UInt32 Product)
        {
            String TxtSQL = "update ps_product_shop "
                 + " set ps_product_shop.available_date = '0000-00-00 00:00:00' "
                 + " where ps_product_shop.id_product = " + Product
                 + " and ps_product_shop.available_date = '0001-01-01 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }
    }
}
