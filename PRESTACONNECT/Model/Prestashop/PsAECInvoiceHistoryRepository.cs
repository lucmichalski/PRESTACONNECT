using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECInvoiceHistoryRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private static string ps_fields_light = " id_customer, invoice_number ";

        public void Add(PsAEcInvoiceHistory Obj)
        {
            this.DBPrestashop.PsAEcInvoiceHistory.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCustomerInvoice(UInt32 Customer, string Invoice)
        {
            return this.DBPrestashop.PsAEcInvoiceHistory.Count(ih => ih.IDCustomer == Customer && ih.InvoiceNumber == Invoice) > 0;
        }
        public Boolean ExistFile(String file)
        {
            return this.DBPrestashop.PsAEcInvoiceHistory.Count(ih => ih.File == file) > 0;
        }

        public List<PsAEcInvoiceHistory_Light> ListLight(uint Customer, uint TypeDoc)
        {
            return DBPrestashop.ExecuteQuery<PsAEcInvoiceHistory_Light>(
                "SELECT " + ps_fields_light +
                " FROM ps_aec_invoice_history " +
                " WHERE id_customer = {0} and type_document = {1}", Customer, TypeDoc).ToList();
        }

        public IQueryable<PsAEcInvoiceHistory> ListCustomerType(uint Customer, uint TypeDoc)
        {
            return this.DBPrestashop.PsAEcInvoiceHistory.Where(ih => ih.IDCustomer == Customer && ih.TypeDocument == TypeDoc);
        }

        public void DeleteCustomerType(uint Customer, uint TypeDoc)
        {
            this.DBPrestashop.PsAEcInvoiceHistory.DeleteAllOnSubmit(ListCustomerType(Customer, TypeDoc));
            this.DBPrestashop.SubmitChanges();
        }

        public void DeleteSelection(IQueryable<Model.Prestashop.PsAEcInvoiceHistory> list)
        {
            this.DBPrestashop.PsAEcInvoiceHistory.DeleteAllOnSubmit(list);
            this.DBPrestashop.SubmitChanges();
        }
    }
}
