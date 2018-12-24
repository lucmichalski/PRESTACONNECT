using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCartPreOrderRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsCartPreOrder Obj)
        {
            this.DBPrestashop.PsCartPreOrder.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistOrder(int Order)
        {
            return this.DBPrestashop.PsCartPreOrder.Count(cp => cp.IDPreOrder == Order) > 0;
        }
        public Boolean ExistCart(uint Cart)
        {
            return this.DBPrestashop.PsCartPreOrder.Count(cp => cp.IDCart == Cart) > 0;
        }

        public PsCartPreOrder ReadOrder(int Order)
        {
            return this.DBPrestashop.PsCartPreOrder.FirstOrDefault(cp => cp.IDPreOrder == Order);
        }
        public PsCartPreOrder ReadCart(uint Cart)
        {
            return this.DBPrestashop.PsCartPreOrder.FirstOrDefault(cp => cp.IDCart == Cart);
        }
    }
}
