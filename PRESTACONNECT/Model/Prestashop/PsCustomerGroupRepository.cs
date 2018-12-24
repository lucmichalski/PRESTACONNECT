using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerGroupRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        // <JG> 06/09/2012 ajout attribution du groupe par défaut au nouveau compte client Prestasop créé depuis Sage
        public void Add(PsCustomerGroup NouveauCompte)
        {
            this.DBPrestashop.PsCustomerGroup.InsertOnSubmit(NouveauCompte);
            this.Save();
        }

        private void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public bool Exist(uint IDCustomer, uint IDGroup)
        {
            return this.DBPrestashop.PsCustomerGroup.Count(cg => cg.IDCustomer == IDCustomer && cg.IDGroup == IDGroup) > 0;
        }

        public void Delete(uint IDCustomer, uint IDGroup)
        {
            this.DBPrestashop.PsCustomerGroup.DeleteOnSubmit(Read(IDCustomer, IDGroup));
            this.Save();
        }

        public PsCustomerGroup Read(uint IDCustomer, uint IDGroup)
        {
            return this.DBPrestashop.PsCustomerGroup.FirstOrDefault(cg => cg.IDCustomer == IDCustomer && cg.IDGroup == IDGroup);
        }
    }
}
