using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderInvoiceTaxRepository
    {
        public enum Type { shipping };

        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOrderInvoiceTax Obj)
        {
            this.DBPrestashop.PsOrderInvoiceTax.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOrderInvoiceTax Obj)
        {
            this.DBPrestashop.PsOrderInvoiceTax.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOrderInvoiceTax> List()
        {
            System.Linq.IQueryable<PsOrderInvoiceTax> Return = from Table in this.DBPrestashop.PsOrderInvoiceTax
                                                            select Table;
            return Return.ToList();
        }

        public List<PsOrderInvoiceTax> ListOrder(UInt32 Invoice)
        {
            System.Linq.IQueryable<PsOrderInvoiceTax> Return = from Table in this.DBPrestashop.PsOrderInvoiceTax
                                                               where Table.IDOrderInvoice == Invoice
                                                            select Table;
            return Return.ToList();
        }

        public Boolean ExistInvoice(UInt32 Invoice)
        {
            return (from Table in this.DBPrestashop.PsOrderInvoiceTax
                    where Table.IDOrderInvoice == Invoice
                    select Table).Count() > 0;
        }

        public Boolean ExistInvoiceTax(UInt32 Invoice, UInt32 Tax)
        {
            return (from Table in this.DBPrestashop.PsOrderInvoiceTax
                    where Table.IDOrderInvoice == Invoice && Table.IDTax == Tax
                    select Table).Count() > 0;
        }

        public PsOrderInvoiceTax ReadInvoiceTaxType(UInt32 Invoice, UInt32 Tax, Model.Prestashop.PsOrderInvoiceTaxRepository.Type Type)
        {
            return (from Table in this.DBPrestashop.PsOrderInvoiceTax
                    where Table.IDOrderInvoice == Invoice && Table.IDTax == Tax && Table.Type.ToLower() == Type.ToString().ToLower()
                    select Table).FirstOrDefault();
        }
    }
}
