using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsHookModuleRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
		
        public List<PsHookModule> ListHook(UInt32 Hook, UInt32 IDShop)
        {
            IQueryable<PsHookModule> Return = from Table in this.DBPrestashop.PsHookModule
                                              where Table.IDHook == Hook && Table.IDShop == IDShop
                                              select Table;
            return Return.ToList();
        }
    }
}
