using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsTaxRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsTax> ListActive(Byte Active)
        {
            System.Linq.IQueryable<PsTax> Return = from Table in this.DBPrestashop.PsTax
                                                   where Table.Active == Active
                                                   select Table;
            return Return.ToList();
        }

        public Boolean ExistTaxe(UInt32 Taxe)
        {
            if (this.DBPrestashop.PsTax.Count(Obj => Obj.IDTax == Taxe) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsTax ReadTax(UInt32 Tax)
        {
            return this.DBPrestashop.PsTax.FirstOrDefault(Obj => Obj.IDTax == Tax);
        }
    }
}
