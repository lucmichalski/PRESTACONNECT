using System;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsHookRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistHookPayment()
        {
            if (this.DBPrestashop.PsHook.Count(Obj => Obj.Name.ToUpper().Contains("PAYMENT")) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<PsHook> ListHookPayment()
        {
            return this.DBPrestashop.PsHook.Where(Obj => Obj.Name.ToUpper().Contains("PAYMENT")).ToList();
        }
    }
}
