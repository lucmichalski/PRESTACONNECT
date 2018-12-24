using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductAttributeRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool ExistInShop(UInt32 IDProductAttribute, UInt32 IDShop)
        {
            return (DBPrestashop.PsProductAttributeShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDProductAttribute == IDProductAttribute) != null);
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttribute Obj, UInt32 IDShop)
        {
            this.DBPrestashop.PsProductAttribute.InsertOnSubmit(Obj);
            this.Save();

            //Si le productattribute n'existe pas dans la boutique, il est rajouté.
            if (!ExistInShop(Obj.IDProductAttribute, IDShop))
            {
                DBPrestashop.PsProductAttributeShop.InsertOnSubmit(new PsProductAttributeShop()
                {
                    AvailableDate = Obj.AvailableDate,
                    DefaultOn = Obj.DefaultOn,
                    EcOtAx = Obj.EcOtAx,
                    IDProductAttribute = Obj.IDProductAttribute,
                    IDShop = IDShop,
                    MinimalQuantity = Obj.MinimalQuantity,
                    Price = Obj.Price,
                    UnitPriceImpact = Obj.UnitPriceImpact,
                    Weight = Obj.Weight,
                    WholesalePrice = Obj.WholesalePrice,
					#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
                    IDProduct = Obj.IDProduct,
					#endif
                });
                DBPrestashop.SubmitChanges();
                new PsProductAttributeShopRepository().WriteDate(Obj.IDProductAttribute);
            }
        }

        public void Delete(PsProductAttribute Obj)
        {
            this.DBPrestashop.PsProductAttributeCombination.DeleteAllOnSubmit(this.DBPrestashop.PsProductAttributeCombination.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
            this.DBPrestashop.PsProductAttributeImage.DeleteAllOnSubmit(this.DBPrestashop.PsProductAttributeImage.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
            this.DBPrestashop.PsProductAttributeShop.DeleteAllOnSubmit(this.DBPrestashop.PsProductAttributeShop.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
            this.DBPrestashop.PsStockAvailable.DeleteAllOnSubmit(this.DBPrestashop.PsStockAvailable.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
            this.DBPrestashop.PsSpecificPrice.DeleteAllOnSubmit(this.DBPrestashop.PsSpecificPrice.Where(o => o.IDProductAttribute == Obj.IDProductAttribute));
            this.DBPrestashop.PsProductAttribute.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsProductAttribute> List(int articleId, UInt32 IDShop)
        {
            //System.Linq.IQueryable<PsProductAttribute> Return = from Table in this.DBPrestashop.PsProductAttribute
            //                                                    where Table.IDProduct == articleId
            //                                                    select Table;
            //return Return.ToList();

            List<PsProductAttribute> products = new List<PsProductAttribute>(
                DBPrestashop.ExecuteQuery<PsProductAttribute>(
                "SELECT DISTINCT P.*  FROM ps_product_attribute P " +
                " INNER JOIN ps_product_attribute_shop PS ON PS.id_product_attribute = P.id_product_attribute " +
                " WHERE PS.id_shop = {0} " +
                " ", IDShop));

            return products;
        }

        public void EraseDefault(uint Product, uint ProductAttribute)
        {
            try
            {
                String TxtSQL = "update ps_product_attribute "
                     + " set ps_product_attribute.default_on = " 
					 #if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
					 + " null " 
					 #else
					 + " 0 " 
					 #endif
                     + " where ps_product_attribute.id_product = " + Product
                     + " and ps_product_attribute.id_product_attribute != " + ProductAttribute;
                this.DBPrestashop.ExecuteCommand(TxtSQL);

                new Model.Prestashop.PsProductAttributeShopRepository().EraseDefault(Product, ProductAttribute);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public Boolean ExistProductAttribute(UInt32 ProductAttribute)
        {
            if (this.DBPrestashop.PsProductAttribute.Count(Obj => Obj.IDProductAttribute == ProductAttribute) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsProductAttribute ReadProductAttribute(UInt32 ProductAttribute)
        {
            return this.DBPrestashop.PsProductAttribute.FirstOrDefault(Obj => Obj.IDProductAttribute == ProductAttribute);
        }

        public List<PsProductAttribute> List(uint IdProduct)
        {
            System.Linq.IQueryable<PsProductAttribute> Return = from Table in this.DBPrestashop.PsProductAttribute
                                                                where Table.IDProduct == IdProduct
                                                                select Table;
            return Return.ToList();
        }

        public void WriteDate(UInt32 Product)
        {
            String TxtSQL = "update ps_product_attribute "
                 + " set ps_product_attribute.available_date = '0000-00-00 00:00:00' "
                 + " where ps_product_attribute.id_product = " + Product
                 + " and ps_product_attribute.available_date = '0001-01-01 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }
    }
}
