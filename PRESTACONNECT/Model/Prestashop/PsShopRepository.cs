using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsShop> List()
        {
            System.Linq.IQueryable<PsShop> Return = from Table in this.DBPrestashop.PsShop
                                                    select Table;
            return Return.ToList();
        }
        
        public List<PsShop> ListActive(Byte Active)
        {
            System.Linq.IQueryable<PsShop> Return = from Table in this.DBPrestashop.PsShop
                                                    where Table.Active == Active
                                                    select Table;
            return Return.ToList();
        }

        public Boolean ExistId(UInt32 Id)
        {
            if (this.DBPrestashop.PsShop.Count(Obj => Obj.IDShop == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsShop ReadId(UInt32 Id)
        {
            return this.DBPrestashop.PsShop.FirstOrDefault(Obj => Obj.IDShop == Id);
        }

        public int Count()
        {
            return this.DBPrestashop.PsShop.Count();
        }
    }
}


