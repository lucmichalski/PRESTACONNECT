using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECBalanceConfigRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsAEcBalanceConFig> List()
        {
            return DBPrestashop.PsAEcBalanceConFig.ToList();
        }

        public void Save()
        {
            DBPrestashop.SubmitChanges();
        }
    }
}
