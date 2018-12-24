using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderPaymentRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOrderPayment Obj)
        {
            this.DBPrestashop.PsOrderPayment.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOrderPayment Obj)
        {
            this.DBPrestashop.PsOrderPayment.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistOrderReference(String Reference)
        {
            return (from Table in this.DBPrestashop.PsOrderPayment
                    where Table.OrderReference == Reference
                    select Table).Count() > 0;
        }

        public Int32 CountOrderReference(String Reference)
        {
            return (from Table in this.DBPrestashop.PsOrderPayment
                    where Table.OrderReference == Reference
                    select Table).ToList().Count;
        }

        public List<PsOrderPayment> ReadOrderReference(String Reference)
        {
            return (from Table in this.DBPrestashop.PsOrderPayment
                    where Table.OrderReference == Reference
                    select Table).ToList();
        }

        public List<String> ListPaymentMethod()
        {
            return this.DBPrestashop.PsOrderPayment.Select(t => t.PaymentMethod).Distinct().ToList();
            //return DBPrestashop.ExecuteQuery<string>(
            //    "SELECT DISTINCT OP.payment_method FROM ps_order_payment OP " +
            //    " ").ToList();
        }
    }
}
