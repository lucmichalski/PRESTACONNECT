using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsMessageRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistOrder(UInt32 Order)
        {
            return this.DBPrestashop.PsMessage.Count(m => m.IDOrder == Order) > 0;
        }

        public List<PsMessage> ListOrder(UInt32 Order)
        {
            return this.DBPrestashop.PsMessage.Where(m => m.IDOrder == Order).ToList();
        }

        public List<PsMessage> ListOrderPrivate(UInt32 Order, byte Private)
        {
            return this.DBPrestashop.PsMessage.Where(m => m.IDOrder == Order && m.Private == Private && m.IDCustomer != 0).ToList();
        }
    }
}
