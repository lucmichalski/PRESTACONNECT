using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCartProductRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCartProduct CartProduct)
        {
            // insert not work because table haven't primary key
            //this.DBPrestashop.PsCartProduct.InsertOnSubmit(CartProduct);
            String TxtSQL = "INSERT INTO `ps_cart_product` " +
                "(`id_cart`, `id_product`, `id_address_delivery`, "+
                "`id_shop`, `id_product_attribute`, `quantity`, `date_add`) VALUES " +
                "(" + CartProduct.IDCart + ", " + CartProduct.IDProduct + ", " + CartProduct.IDAddressDelivery +
                ", " + CartProduct.IDShop + ", " + CartProduct.IDProductAttribute + ", " + CartProduct.Quantity +
                ", '" + CartProduct.DateAdd.ToString("yyyy-MM-dd HH:mm:ss") +"')";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            this.Save();
        }

        public Boolean ExistCart(UInt32 Cart)
        {
            return this.DBPrestashop.PsCartProduct.Count(c => c.IDCart == Cart) > 0;
        }

        public PsCartProduct ReadCart(UInt32 Cart)
        {
            return this.DBPrestashop.PsCartProduct.FirstOrDefault(c => c.IDCart == Cart);
        }

        public Boolean ExistProduct(UInt32 Product)
        {
            return this.DBPrestashop.PsCartProduct.Count(c => c.IDProduct == Product) > 0;
        }

        public PsCartProduct ReadProduct(UInt32 Product)
        {
            return this.DBPrestashop.PsCartProduct.FirstOrDefault(c => c.IDProduct == Product);
        }

        public List<PsCartProduct> List()
        {
            return this.DBPrestashop.PsCartProduct.ToList();
        }

        public List<PsCartProduct> ListCart(UInt32 Cart)
        {
            return this.DBPrestashop.PsCartProduct.Where(c => c.IDCart == Cart).ToList();
        }
    }
}
