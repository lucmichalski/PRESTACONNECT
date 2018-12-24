using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsSupplierLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsSupplierLang Obj)
        {
            this.DBPrestashop.PsSupplierLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsSupplierLang Obj)
        {
            this.DBPrestashop.PsSupplierLang.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsSupplierLang> List()
        {
            System.Linq.IQueryable<PsSupplierLang> Return = from Table in this.DBPrestashop.PsSupplierLang
                                                            select Table;
            return Return.ToList();
        }

        public List<PsSupplierLang> ListSupplier(Int32 Supplier)
        {
            System.Linq.IQueryable<PsSupplierLang> Return = from Table in this.DBPrestashop.PsSupplierLang
                                                            where Table.IDSupplier == Supplier
                                                            select Table;
            return Return.ToList();
        }

        public Boolean ExistSupplierLang(Int32 Supplier, UInt32 Lang)
        {
            if (this.DBPrestashop.PsSupplierLang.Count(Obj => Obj.IDSupplier == Supplier && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsSupplierLang ReadSupplierLang(Int32 Supplier, UInt32 Lang)
        {
            return this.DBPrestashop.PsSupplierLang.FirstOrDefault(Obj => Obj.IDSupplier == Supplier && Obj.IDLang == Lang);
        }
    }
}
