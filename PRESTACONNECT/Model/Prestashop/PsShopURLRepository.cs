using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsShopURLRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public PsShopURL ReadShopId(UInt32 IDShop)
        {
            return this.DBPrestashop.PsShopURL.FirstOrDefault(Obj => Obj.IDShop == IDShop);
        }
    }
}


