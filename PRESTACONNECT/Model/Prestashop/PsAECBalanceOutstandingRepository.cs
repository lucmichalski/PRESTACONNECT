using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECBalanceOutstandingRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcBalanceOutstanding Obj)
        {
            this.DBPrestashop.PsAEcBalanceOutstanding.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcBalanceOutstanding.Count(co => co.IDCustomer == Customer) > 0;
        }
        public PsAEcBalanceOutstanding ReadCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcBalanceOutstanding.FirstOrDefault(co => co.IDCustomer == Customer);
        }
    }
}
