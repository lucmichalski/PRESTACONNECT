using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCategoryProductRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistCategory(UInt32 Category)
        {
            return this.DBPrestashop.PsCategoryProduct.Count(cp => cp.IDCategory == Category) > 0;
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCategoryProduct Obj)
        {
            MySqlConnection Connection = new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString);
            Connection.Open();
            MySqlCommand Command = Connection.CreateCommand();
            Command.CommandText = "insert into ps_category_product (id_category, id_product, position) values (" + Obj.IDCategory + ", " + Obj.IDProduct + ", " + Obj.Position + ")";
            Command.ExecuteNonQuery();
            Connection.Close();
        }

        public void Delete(PsCategoryProduct Obj)
        {
            this.DBPrestashop.PsCategoryProduct.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistCategoryProduct(Int32 Category, UInt32 Product)
        {
            if (this.DBPrestashop.PsCategoryProduct.Count(Obj => Obj.IDProduct == Product && Obj.IDCategory == Category) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsCategoryProduct ReadCategoryProduct(Int32 Category, UInt32 Product)
        {
            return this.DBPrestashop.PsCategoryProduct.FirstOrDefault(Obj => Obj.IDProduct == Product && Obj.IDCategory == Category);
        }

        public Boolean ExistCategoryPosition(Int32 Category, Int32 Position)
        {
            if (this.DBPrestashop.PsCategoryProduct.Count(Obj => Obj.Position == Position && Obj.IDCategory == Category) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsCategoryProduct ReadCategoryPosition(Int32 Category, Int32 Position)
        {
            return this.DBPrestashop.PsCategoryProduct.FirstOrDefault(Obj => Obj.Position == Position && Obj.IDCategory == Category);
        }

        public List<PsCategoryProduct> ListProduct(UInt32 Product)
        {
            IQueryable<PsCategoryProduct> Return = from Table in this.DBPrestashop.PsCategoryProduct
                                                   where Table.IDProduct == Product
                                                   select Table;
            return Return.ToList();
        }

        public static UInt32 NextPositionProductCatalog(UInt32 Category)
        {
            DataClassesPrestashop DBPs 
                = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
            if (DBPs.PsCategoryProduct.Count(Obj => Obj.IDCategory == Category) > 0)
            {
                return DBPs.PsCategoryProduct.Where(Obj => Obj.IDCategory == Category).ToList().Max(cp => cp.Position) + 1;
            }
            else
                return 1;
        }
    }
}
