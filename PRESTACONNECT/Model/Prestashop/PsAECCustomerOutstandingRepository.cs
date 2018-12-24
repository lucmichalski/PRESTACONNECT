using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECCustomerOutstandingRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcCustomerOutstanding Obj)
        {
            this.DBPrestashop.PsAEcCustomerOutstanding.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcCustomerOutstanding.Count(co => co.IDCustomer == Customer) > 0;
        }
        public PsAEcCustomerOutstanding ReadCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcCustomerOutstanding.FirstOrDefault(co => co.IDCustomer == Customer);
        }
    }
}
