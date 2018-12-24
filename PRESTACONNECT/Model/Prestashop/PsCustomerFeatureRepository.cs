using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerFeatureRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCustomerFeature CustomerFeature)
        {
            this.DBPrestashop.PsCustomerFeature.InsertOnSubmit(CustomerFeature);
            this.Save();
        }

        public Boolean Exist(UInt32 CustomerFeature)
        {
            return this.DBPrestashop.PsCustomerFeature.Count(f => f.IDCustomerFeature == CustomerFeature) > 0;
        }

        public PsCustomerFeature Read(UInt32 CustomerFeature)
        {
            return this.DBPrestashop.PsCustomerFeature.FirstOrDefault(f => f.IDCustomerFeature == CustomerFeature);
        }

        public List<PsCustomerFeature> List()
        {
            return this.DBPrestashop.PsCustomerFeature.ToList();
        }

        public static UInt32 NextPosition()
        {
            DataClassesPrestashop DBPs = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
            if (DBPs.PsCustomerFeature.Count() > 0)
            {
                return DBPs.PsCustomerFeature.ToList().Max(cp => cp.Position) + 1;
            }
            else
                return 1;
        }
    }
}
