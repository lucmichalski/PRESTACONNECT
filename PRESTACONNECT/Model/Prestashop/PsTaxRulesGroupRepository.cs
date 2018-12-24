using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsTaxRulesGroupRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsTaxRulesGroup> ListActive(Byte Active)
        {
            System.Linq.IQueryable<PsTaxRulesGroup> Return = from Table in this.DBPrestashop.PsTaxRulesGroup
                                                   where Table.Active == Active
                                                   select Table;
            return Return.ToList();
        }
    }
}
