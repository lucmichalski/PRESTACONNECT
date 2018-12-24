using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductRepository
    {
        private static string ps_fields_light = "P.id_product, PL.name ";
        private static string ps_fields_light_order = "P.id_product, PL.name ";
        private static string ps_fields_update = "P.id_product, PL.name, P.date_upd ";
        private static string ps_fields_update_order = "P.id_product, PL.name, P.date_upd ";
        private static string ps_fields_resume = "P.id_product, PL.name, P.reference, CL.name as default_category ";
        private static string ps_fields_resume_order = "P.id_product, PL.name, P.reference, default_category ";

        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool ExistInShop(UInt32 IDProduct, UInt32 IDShop)
        {
            return (DBPrestashop.PsProductShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDProduct == IDProduct) != null);
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProduct Obj, UInt32 IDShop)
        {
            this.DBPrestashop.PsProduct.InsertOnSubmit(Obj);
            this.Save();

            //Si le produit n'existe pas dans la boutique, il est rajouté.
            if (!ExistInShop(Obj.IDProduct, IDShop))
            {
                DBPrestashop.PsProductShop.InsertOnSubmit(new PsProductShop()
                {
                    Active = Obj.Active,
                    AdditionalShippingCost = Obj.AdditionalShippingCost,
                    AdvancedStockManagement = Obj.AdvancedStockManagement,
                    AvailableDate = Obj.AvailableDate,
                    AvailableForOrder = Obj.AvailableForOrder,
                    CacheDefaultAttribute = Obj.CacheDefaultAttribute,
                    Customizable = Obj.Customizable,
                    DateAdd = Obj.DateAdd,
                    DateUpd = Obj.DateUpd,
                    EcOtAx = Obj.EcOtAx,
                    IDCategoryDefault = Obj.IDCategoryDefault,
                    IDProduct = Obj.IDProduct,
                    IDShop = IDShop,
                    IDTaxRulesGroup = Obj.IDTaxRulesGroup,
                    Indexed = Obj.Indexed,
                    MinimalQuantity = Obj.MinimalQuantity,
                    OnlineOnly = Obj.OnlineOnly,
                    OnSale = Obj.OnSale,
                    Price = Obj.Price,
                    ShowPrice = Obj.ShowPrice,
                    TextFields = Obj.TextFields,
                    UnitPriceRatio = Obj.UnitPriceRatio,
                    Unity = Obj.Unity,
                    UploadAbleFiles = Obj.UploadAbleFiles,
                    WholesalePrice = Obj.WholesalePrice,
					#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
					PackStockType = Obj.PackStockType,
					#endif
                });
                DBPrestashop.SubmitChanges();
            }
        }

        public List<UInt32> ListId(UInt32 IDShop)
        {
            //System.Linq.IQueryable<UInt32> Return = from Table in this.DBPrestashop.PsProduct
            //                                       select Table.IDProduct;
            //return Return.ToList();

            //List<uint> products = new List<uint>();

            //foreach (var product in DBPrestashop.ExecuteQuery<PsProduct>(
            //    "SELECT DISTINCT P.id_product, P.id_supplier, P.id_manufacturer, P.id_category_default, P.id_shop_default, "+
            //    " P.id_tax_rules_group, P.on_sale, P.online_only, P.ean13, P.upc, P.ecotax, P.quantity, P.minimal_quantity, "+
            //    " P.price, P.wholesale_price, P.unity, P.unit_price_ratio, P.additional_shipping_cost, P.reference, P.supplier_reference, "+
            //    " P.location, P.width, P.height, P.depth, P.weight, P.out_of_stock, P.quantity_discount, P.customizable, "+
            //    " P.uploadable_files, P.text_fields, P.active, P.available_for_order, P.available_date, P.show_price, P.indexed, "+
            //    " P.cache_is_pack, P.cache_has_attachments, P.is_virtual, P.cache_default_attribute, P.date_add, P.date_upd, P.advanced_stock_management " +
            //    " FROM ps_product P " +
            //    " INNER JOIN ps_product_shop PS ON PS.id_product = P.id_product " +
            //    " WHERE PS.id_shop = {0} " +
            //    " ", IDShop))
            //    products.Add(product.IDProduct);


            // <JG> 22/02/2013 correction récupération liste des id produits
            List<uint> products = (from t in DBPrestashop.PsProductShop
                                   where t.IDShop == IDShop
                                   select t.IDProduct).ToList();

            return products;
        }

        public Boolean ExistId(UInt32 Id)
        {
            if (this.DBPrestashop.PsProduct.Count(Obj => Obj.IDProduct == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ProductIsActive(UInt32 Id)
        {
            return this.DBPrestashop.PsProduct.Count(Obj => Obj.IDProduct == Id) == 1
                && this.DBPrestashop.PsProduct.FirstOrDefault(Obj => Obj.IDProduct == Id).Active == 1;
        }

        public PsProduct ReadId(UInt32 Id)
        {
            return this.DBPrestashop.PsProduct.FirstOrDefault(Obj => Obj.IDProduct == Id);
        }

        public Boolean ExistDefaultCatalog(Int32 Catalog)
        {
            return this.DBPrestashop.PsProduct.Count(p => p.IDCategoryDefault == Catalog) > 0;
        }

        public List<Model.Prestashop.PsProduct> List()
        {
            return this.DBPrestashop.PsProduct.ToList();
        }

        public List<ProductLight> ListLightPrecommande()
        {
            return DBPrestashop.ExecuteQuery<ProductLight>(
                "SELECT DISTINCT " + ps_fields_light +
                " FROM ps_product P " +
                " INNER JOIN ps_product_lang PL ON P.id_product = PL.id_product " +
                " INNER JOIN ps_product_shop PS ON PS.id_product = P.id_product " +
                " WHERE PL.id_lang = " + Core.Global.Lang +
                " AND PS.id_shop = " + Core.Global.CurrentShop.IDShop +
                " AND P.is_virtual = 1" +
                " AND P.id_tax_rules_group = 0" +
                " ORDER BY " + ps_fields_light_order).ToList();
        }

        public List<ProductLight> ListLight()
        {
            return DBPrestashop.ExecuteQuery<ProductLight>(
                "SELECT DISTINCT " + ps_fields_light +
                " FROM ps_product P " +
                " INNER JOIN ps_product_lang PL ON P.id_product = PL.id_product " +
                " INNER JOIN ps_product_shop PS ON PS.id_product = P.id_product " +
                " WHERE PL.id_lang = " + Core.Global.Lang +
                " AND PS.id_shop = " + Core.Global.CurrentShop.IDShop +
                " ORDER BY " + ps_fields_light_order).ToList();
        }

		public List<ProductUpdate> ListUpdate(List<Model.Local.Article> articles)
		{
			string requete = "SELECT DISTINCT " + ps_fields_update +
						" FROM ps_product P " +
						" INNER JOIN ps_product_lang PL ON P.id_product = PL.id_product " +
						" INNER JOIN ps_product_shop PS ON PS.id_product = P.id_product " +
						" WHERE PL.id_lang = " + Core.Global.Lang +
						" AND PS.id_shop = " + Core.Global.CurrentShop.IDShop +
						(articles.Where(a => a.Pre_Id != null).Count() > 0 ? " AND P.id_product IN (" + articles.Where(a => a.Pre_Id != null).Select(a => a.Pre_Id.ToString()).Aggregate((a, b) => a + ", " + b) + ") " : "") +
						" ORDER BY " + ps_fields_update_order;
			return DBPrestashop.ExecuteQuery<ProductUpdate>(requete).ToList();
		}

        public ProductUpdate ReadUpdate(int idproduct)
        {
            return DBPrestashop.ExecuteQuery<ProductUpdate>(
                "SELECT DISTINCT " + ps_fields_update +
                " FROM ps_product P " +
                " INNER JOIN ps_product_lang PL ON P.id_product = PL.id_product " +
                " INNER JOIN ps_product_shop PS ON PS.id_product = P.id_product " +
                " WHERE P.id_product = " + idproduct +
                " AND PL.id_lang = " + Core.Global.Lang +
                " AND PS.id_shop = " + Core.Global.CurrentShop.IDShop +
                " ORDER BY "  + ps_fields_update_order).FirstOrDefault();
        }

        public List<ProductResume> ListResume()
        {
            return DBPrestashop.ExecuteQuery<ProductResume>(
                "SELECT DISTINCT " + ps_fields_resume +
                " FROM ps_product P " +
                " INNER JOIN ps_product_lang PL ON P.id_product = PL.id_product " +
                " INNER JOIN ps_product_shop PS ON PS.id_product = P.id_product " +
                " INNER JOIN ps_category_lang CL ON P.id_category_default = CL.id_category" +
                " WHERE PL.id_lang = " + Core.Global.Lang +
                " AND CL.id_lang = " + Core.Global.Lang +
                " AND PS.id_shop = " + Core.Global.CurrentShop.IDShop +
                " ORDER BY " + ps_fields_resume_order).ToList();
        }

        public void WriteDate(UInt32 Product)
        {
            String TxtSQL = "update ps_product "
                 + " set ps_product.available_date = '0000-00-00 00:00:00' "
                 + " where ps_product.id_product = " + Product
                 + " and ps_product.available_date = '0001-01-01 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }
    }
}
