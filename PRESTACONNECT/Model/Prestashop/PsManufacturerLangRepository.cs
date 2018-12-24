using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsManufacturerLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsManufacturerLang Obj)
        {
            this.DBPrestashop.PsManufacturerLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsManufacturerLang Obj)
        {
            this.DBPrestashop.PsManufacturerLang.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsManufacturerLang> List()
        {
            System.Linq.IQueryable<PsManufacturerLang> Return = from Table in this.DBPrestashop.PsManufacturerLang
                                                            select Table;
            return Return.ToList();
        }

        public List<PsManufacturerLang> ListManufacturer(Int32 Manufacturer)
        {
            System.Linq.IQueryable<PsManufacturerLang> Return = from Table in this.DBPrestashop.PsManufacturerLang
                                                            where Table.IDManufacturer == Manufacturer
                                                            select Table;
            return Return.ToList();
        }

        public Boolean ExistManufacturerLang(Int32 Manufacturer, UInt32 Lang)
        {
            if (this.DBPrestashop.PsManufacturerLang.Count(Obj => Obj.IDManufacturer == Manufacturer && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsManufacturerLang ReadManufacturerLang(Int32 Manufacturer, UInt32 Lang)
        {
            return this.DBPrestashop.PsManufacturerLang.FirstOrDefault(Obj => Obj.IDManufacturer == Manufacturer && Obj.IDLang == Lang);
        }
    }
}
