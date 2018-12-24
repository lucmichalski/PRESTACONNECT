using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderInvoiceRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOrderInvoice Obj)
        {
            this.DBPrestashop.PsOrderInvoice.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOrderInvoice Obj)
        {
            this.DBPrestashop.PsOrderInvoice.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOrderInvoice> List()
        {
            System.Linq.IQueryable<PsOrderInvoice> Return = from Table in this.DBPrestashop.PsOrderInvoice
                                                            select Table;
            return Return.ToList();
        }

        public List<PsOrderInvoice> ListOrder(UInt32 Order)
        {
            System.Linq.IQueryable<PsOrderInvoice> Return = from Table in this.DBPrestashop.PsOrderInvoice
                                                            where Table.IDOrder == Order
                                                            select Table;
            return Return.ToList();
        }

        public Boolean ExistOrder(UInt32 Order)
        {
            return (from Table in this.DBPrestashop.PsOrderInvoice
                    where Table.IDOrder == Order
                    select Table).Count() > 0;
        }

        public PsOrderInvoice ReadOrder(UInt32 Order)
        {
            return (from Table in this.DBPrestashop.PsOrderInvoice
                    where Table.IDOrder == Order
                    select Table).FirstOrDefault();
        }
    }
}
