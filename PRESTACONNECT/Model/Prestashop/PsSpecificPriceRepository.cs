using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsSpecificPriceRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void SaveReductionTypeFromDateToDate(PsSpecificPrice Obj)
        {
            /*
            String TxtSQL = "update ps_specific_price set reduction_type = '" + Obj.ReductionType + "' where id_specific_price = " + Obj.IDSpecificPrice;
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            TxtSQL = "update ps_specific_price set ps_specific_price.from = '0000-00-00 00:00:00' where id_specific_price = " + Obj.IDSpecificPrice;
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            TxtSQL = "update ps_specific_price set ps_specific_price.to = '0000-00-00 00:00:00 ' where id_specific_price = " + Obj.IDSpecificPrice;
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            this.Save();
             */
            String TxtSQL = "update ps_specific_price "
                 + " set ps_specific_price.reduction_type = '" + Obj.ReductionType + "', "
                 + " ps_specific_price.from = '0000-00-00 00:00:00', "
                 + " ps_specific_price.to = '0000-00-00 00:00:00' "
                 + " where id_specific_price = " + Obj.IDSpecificPrice;
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }
        public void WriteDateToDate(PsProduct Obj)
        {
            String TxtSQL = "update ps_specific_price "
                 + " set ps_specific_price.from = '0000-00-00 00:00:00', "
                 + " ps_specific_price.to = '0000-00-00 00:00:00' "
                 + " where id_product = " + Obj.IDProduct;
            if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
            TxtSQL += " and ps_specific_price.from = '0001-01-01 00:00:00' "
                + " and ps_specific_price.to = '0001-01-01 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }
        public void WriteReductionType(PsProduct Obj)
        {
            String TxtSQL = "update ps_specific_price "
                 + " set ps_specific_price.reduction_type = 'percentage' "
                 + " where id_product = " + Obj.IDProduct
                 + " and reduction <> 0 ";
            if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
                TxtSQL += " and ps_specific_price.from = '0001-01-01 00:00:00' "
                    + " and ps_specific_price.to = '0001-01-01 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }
        public void WriteDateToDate(PsProduct Obj, UInt32 Group)
        {
            String TxtSQL = "update ps_specific_price "
                 + " set ps_specific_price.from = '0000-00-00 00:00:00', "
                 + " ps_specific_price.to = '0000-00-00 00:00:00' "
                 + " where id_product = " + Obj.IDProduct
                 + " and id_group = " + Group;
            if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
                TxtSQL += " and ps_specific_price.from = '0001-01-01 00:00:00' "
                    + " and ps_specific_price.to = '0001-01-01 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }
        public void WriteReductionType(PsProduct Obj, UInt32 Group)
        {
            String TxtSQL = "update ps_specific_price "
                 + " set ps_specific_price.reduction_type = 'percentage' "
                 + " where id_product = " + Obj.IDProduct
                 + " and reduction <> 0 "
                 + " and id_group = " + Group;
            if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
                TxtSQL += " and ps_specific_price.from = '0001-01-01 00:00:00' "
                    + " and ps_specific_price.to = '0001-01-01 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }

        public void WriteAvailableForOrder(byte available, UInt32 Product)
        {
            String TxtSQL = "update ps_product "
                 + " set ps_product.available_for_order = " + available
                 + " where ps_product.id_product = " + Product + ";"
                 + " update ps_product_shop "
                 + " set ps_product_shop.available_for_order = " + available
                 + " where ps_product_shop.id_product = " + Product;
            this.DBPrestashop.ExecuteCommand(TxtSQL);
        }

        public List<PsSpecificPrice> ListProduct(UInt32 Product)
        {
            return this.DBPrestashop.PsSpecificPrice.Where(s => s.IDProduct == Product).ToList();
        }

        public void DeleteFromProduct(UInt32 Product)
        {
            String TxtSQL = "delete from ps_specific_price where id_product = " + Product;
            if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
                TxtSQL += " and ps_specific_price.from = '0000-00-00 00:00:00' "
                 + " and ps_specific_price.to = '0000-00-00 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            //this.Save();
        }

        public void DeleteFromProductAttribute(UInt32 Product, Int32 ProductAttribute)
        {
            String TxtSQL = "delete from ps_specific_price where id_product = " + Product + " and id_product_attribute = " + ProductAttribute;
            if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
                TxtSQL += " and ps_specific_price.from = '0000-00-00 00:00:00' "
                 + " and ps_specific_price.to = '0000-00-00 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            //this.Save();
        }

        public void DeleteFromProductAttributeGroup(UInt32 Product, Int32 ProductAttribute, UInt32 Group)
        {
            String TxtSQL = "delete from ps_specific_price where id_product = " + Product + " and id_product_attribute = " + ProductAttribute + " and id_group = " + Group;
            if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
                TxtSQL += " and ps_specific_price.from = '0000-00-00 00:00:00' "
                 + " and ps_specific_price.to = '0000-00-00 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            //this.Save();
        }

        public void DeleteFromProductGroup(UInt32 Product, UInt32 Group)
        {
            String TxtSQL = "delete from ps_specific_price where id_product = " + Product + " and id_group = " + Group;
            if (Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop)
                TxtSQL += " and ps_specific_price.from = '0000-00-00 00:00:00' "
                 + " and ps_specific_price.to = '0000-00-00 00:00:00' ";
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            //this.Save();
        }

        public void Add(PsSpecificPrice Obj)
        {
            this.DBPrestashop.PsSpecificPrice.InsertOnSubmit(Obj);
            this.Save();
        }

        public void AddList(IEnumerable<PsSpecificPrice> list)
        {
            this.DBPrestashop.PsSpecificPrice.InsertAllOnSubmit(list);
            this.Save();
        }
    }
}
