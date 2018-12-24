using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCarrierTaxRulesGroupShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistCarrierShop(UInt32 Carrier, UInt32 Shop)
        {
            return this.DBPrestashop.PsCarrierTaxRulesGroupShop.Count(Obj => Obj.IDCarrier == Carrier && Obj.IDShop == Shop) == 1;
        }

        public PsCarrierTaxRulesGroupShop ReadCarrierShop(UInt32 Carrier, UInt32 Shop)
        {
            return this.DBPrestashop.PsCarrierTaxRulesGroupShop.FirstOrDefault(Obj => Obj.IDCarrier == Carrier && Obj.IDShop == Shop);
        }
    }
}
