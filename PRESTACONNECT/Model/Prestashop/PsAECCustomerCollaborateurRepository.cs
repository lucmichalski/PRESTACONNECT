using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECCustomerCollaborateurRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcCustomerCollaborateur Obj)
        {
            this.DBPrestashop.PsAEcCustomerCollaborateur.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCollaborateur(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcCustomerCollaborateur.Count(Coll => Coll.IDCustomer == Customer) > 0;
        }

        public PsAEcCustomerCollaborateur ReadCollaborateur(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcCustomerCollaborateur.FirstOrDefault(Obj => Obj.IDCustomer == Customer);
        }
    }
}
