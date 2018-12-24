using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsSoDeliveryRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsSoDelivery Obj)
        {
            this.DBPrestashop.PsSoDelivery.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCart(UInt32 idcart)
        {
            return this.DBPrestashop.PsSoDelivery.Count(s => s.CartID == idcart) > 0;
        }

        public PsSoDelivery ReadCart(UInt32 idcart)
        {
            return this.DBPrestashop.PsSoDelivery.FirstOrDefault(s => s.CartID == idcart);
        }

    }
}
