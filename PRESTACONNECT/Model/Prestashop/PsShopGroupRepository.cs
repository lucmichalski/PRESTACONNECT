using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsShopGroupRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public PsShopGroup ReadId(UInt32 Id)
        {
            return this.DBPrestashop.PsShopGroup.FirstOrDefault(Obj => Obj.IDShopGroup == Id);
        }

        public int Count()
        {
            return this.DBPrestashop.PsShopGroup.Count();
        }
    }
}


