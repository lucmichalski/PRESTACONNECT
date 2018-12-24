using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderDetailTaxRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsOrderDetailTax> ListOrderDetail(UInt32 IdOrderDetail)
        {
            return this.DBPrestashop.PsOrderDetailTax.Where(o => o.IDOrderDetail == IdOrderDetail).ToList();
        }

        public Boolean ExistOrderDetail(UInt32 IdOrderDetail)
        {
            return this.DBPrestashop.PsOrderDetailTax.Count(o => o.IDOrderDetail == IdOrderDetail) > 0;
        }
    }
}
