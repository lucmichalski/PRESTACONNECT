using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductSupplierRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductSupplier Obj)
        {
            this.DBPrestashop.PsProductSupplier.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsProductSupplier Obj)
        {
            this.DBPrestashop.PsProductSupplier.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteAll(List<PsProductSupplier> list)
        {
            this.DBPrestashop.PsProductSupplier.DeleteAllOnSubmit(list);
            this.Save();
        }

        public Boolean ExistProductSupplier(UInt32 Product, UInt32 Supplier)
        {
            return this.DBPrestashop.PsProductSupplier.Count(Obj => Obj.IDProduct == Product && Obj.IDSupplier == Supplier) > 0;
        }

        public PsProductSupplier ReadProductSupplier(UInt32 Product, UInt32 Supplier)
        {
            return this.DBPrestashop.PsProductSupplier.FirstOrDefault(Obj => Obj.IDProduct == Product && Obj.IDSupplier == Supplier);
        }

        public Boolean ExistProduct(UInt32 Product)
        {
            return this.DBPrestashop.PsProductSupplier.Count(ps => ps.IDProduct == Product) > 0;
        }
        public List<PsProductSupplier> ListProduct(UInt32 Product)
        {
            return (from Table in this.DBPrestashop.PsProductSupplier
                     where Table.IDProduct == Product
                     select Table).ToList();
        }

        public Boolean ExistProductAttributeSupplier(UInt32 Product, UInt32 Supplier, UInt32 ProductAttribute)
        {
            return this.DBPrestashop.PsProductSupplier.Count(Obj => Obj.IDProduct == Product && Obj.IDSupplier == Supplier && Obj.IDProductAttribute == ProductAttribute) > 0;
        }

        public PsProductSupplier ReadProductAttributeSupplier(UInt32 Product, UInt32 Supplier, UInt32 ProductAttribute)
        {
            return this.DBPrestashop.PsProductSupplier.FirstOrDefault(Obj => Obj.IDProduct == Product && Obj.IDSupplier == Supplier && Obj.IDProductAttribute == ProductAttribute);
        }
    }
}
