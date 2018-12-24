using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Prestashop
{
    partial class PsAEcInvoiceHistory
    {
    }
    public class PsAEcInvoiceHistory_Light
    {
        public uint id_customer = 0;
        public string invoice_number = string.Empty;

        #region Methods

        public override string ToString()
        {
            return "customer : " + id_customer + " / piece : " + invoice_number;
        }

        #endregion
    }
}
