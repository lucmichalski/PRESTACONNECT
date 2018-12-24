using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECRepresentativeCustomerRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcRepresentativeCustomer Obj)
        {
            this.DBPrestashop.PsAEcRepresentativeCustomer.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean Exist(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcRepresentativeCustomer.Count(Coll => Coll.IDCustomer == Customer) > 0;
        }

        public PsAEcRepresentativeCustomer Read(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcRepresentativeCustomer.FirstOrDefault(Obj => Obj.IDCustomer == Customer);
        }
    }
}
