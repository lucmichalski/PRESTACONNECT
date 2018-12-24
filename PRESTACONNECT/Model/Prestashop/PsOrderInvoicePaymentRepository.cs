using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderInvoicePaymentRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOrderInvoicePayment Obj)
        {
            this.DBPrestashop.PsOrderInvoicePayment.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOrderInvoicePayment Obj)
        {
            this.DBPrestashop.PsOrderInvoicePayment.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOrderInvoicePayment> List()
        {
            System.Linq.IQueryable<PsOrderInvoicePayment> Return = from Table in this.DBPrestashop.PsOrderInvoicePayment
                                                            select Table;
            return Return.ToList();
        }

        public List<PsOrderInvoicePayment> ListOrder(UInt32 Order)
        {
            System.Linq.IQueryable<PsOrderInvoicePayment> Return = from Table in this.DBPrestashop.PsOrderInvoicePayment
                                                            where Table.IDOrder == Order
                                                            select Table;
            return Return.ToList();
        }

        public Boolean ExistOrder(UInt32 Order)
        {
            return (from Table in this.DBPrestashop.PsOrderInvoicePayment
                    where Table.IDOrder == Order
                    select Table).Count() > 0;
        }

        public Boolean ExistPayement(UInt32 Payment)
        {
            return (from Table in this.DBPrestashop.PsOrderInvoicePayment
                    where Table.IDOrderPayment == Payment
                    select Table).Count() > 0;
        }
    }
}
