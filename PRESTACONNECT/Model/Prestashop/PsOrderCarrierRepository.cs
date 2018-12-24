using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderCarrierRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOrderCarrier Obj)
        {
            this.DBPrestashop.PsOrderCarrier.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOrderCarrier Obj)
        {
            this.DBPrestashop.PsOrderCarrier.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOrderCarrier> List()
        {
            System.Linq.IQueryable<PsOrderCarrier> Return = from Table in this.DBPrestashop.PsOrderCarrier
                                                                   select Table;
            return Return.ToList();
        }

        public List<PsOrderCarrier> ListOrder(UInt32 Order)
        {
            System.Linq.IQueryable<PsOrderCarrier> Return = from Table in this.DBPrestashop.PsOrderCarrier
                                                                   where Table.IDOrder == Order
                                                                   select Table;
            return Return.ToList();
        }

        public Boolean ExistOrder(UInt32 Order)
        {
            return (from Table in this.DBPrestashop.PsOrderCarrier
                    where Table.IDOrder == Order
                    select Table).Count() > 0;
        }

        public Boolean ExistInvoice(UInt32 Invoice)
        {
            return (from Table in this.DBPrestashop.PsOrderCarrier
                    where Table.IDOrderInvoice == Invoice
                    select Table).Count() > 0;
        }
    }
}
