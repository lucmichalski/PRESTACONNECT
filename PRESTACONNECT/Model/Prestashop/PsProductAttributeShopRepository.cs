using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductAttributeShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttributeShop Obj)
        {
            this.DBPrestashop.PsProductAttributeShop.InsertOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistPsProductAttributeShop(UInt32 ProductAttribute, UInt32 Shop)
        {
            if (this.DBPrestashop.PsProductAttributeShop.Count(Obj => Obj.IDProductAttribute == ProductAttribute && Obj.IDShop == Shop) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PsProductAttributeShop ReadPsProductAttributeShop(UInt32 ProductAttribute, UInt32 Shop)
        {
            return this.DBPrestashop.PsProductAttributeShop.FirstOrDefault(Obj => Obj.IDProductAttribute == ProductAttribute && Obj.IDShop == Shop);
        }

        public void WriteDate(UInt32 ProductAttribute)
        {
            String TxtSQL = "update ps_product_attribute_shop "
                 + " set ps_product_attribute_shop.available_date = '0000-00-00 00:00:00' "
                 + " where ps_product_attribute_shop.id_product_attribute = " + ProductAttribute
                 + " and ps_product_attribute_shop.available_date = '0001-01-01 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }

        public void EraseDefault(uint Product, uint ProductAttribute)
        {
            try
            {
                String TxtSQL = "update ps_product_attribute_shop "
                     + " set ps_product_attribute_shop.default_on = "
					 #if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
                     + " null where ps_product_attribute_shop.id_product = " + Product
					 #else
                     + " 0 where ps_product_attribute_shop.id_product_attribute in (select ps_product_attribute.id_product_attribute from ps_product_attribute where ps_product_attribute.id_product = " + Product + ") "
					 #endif
                     + " and ps_product_attribute_shop.id_product_attribute != " + ProductAttribute;
                this.DBPrestashop.ExecuteCommand(TxtSQL);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
