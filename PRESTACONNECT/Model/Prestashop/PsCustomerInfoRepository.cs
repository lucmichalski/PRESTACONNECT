using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerInfoRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsCustomerInfo Obj)
        {
            this.DBPrestashop.PsCustomerInfo.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsCustomerInfo.Count(ci => ci.IDCustomer == Customer) > 0;
        }

        public PsCustomerInfo ReadCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsCustomerInfo.FirstOrDefault(ci => ci.IDCustomer == Customer);
        }
    }
}
