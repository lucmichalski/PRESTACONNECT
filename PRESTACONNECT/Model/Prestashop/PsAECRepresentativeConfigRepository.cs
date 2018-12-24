using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECRepresentativeConfigRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsAEcRepresentative> List()
        {
            return this.DBPrestashop.PsAEcRepresentative.ToList();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }
    }
}
