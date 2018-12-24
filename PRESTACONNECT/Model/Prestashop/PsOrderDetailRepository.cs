using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderDetailRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
        
        public List<PsOrderDetail> ListOrder(UInt32 Order)
        {
            System.Linq.IQueryable<PsOrderDetail> Return = from Table in this.DBPrestashop.PsOrderDetail
                                                           where Table.IDOrder == Order
                                                           select Table;
            return Return.ToList();
        }

        public Boolean ExistOrderProduct(UInt32 Order, String Product)
        {
            return this.DBPrestashop.PsOrderDetail.Count(od => od.IDOrder == Order && od.ProductReference == Product) == 1;
        }

        public PsOrderDetail ReadOrderProduct(UInt32 Order, String Product)
        {
            return this.DBPrestashop.PsOrderDetail.FirstOrDefault(od => od.IDOrder == Order && od.ProductReference == Product);
        }

        public void DeleteAll(List<PsOrderDetail> ListObj)
        {
            this.DBPrestashop.PsOrderDetail.DeleteAllOnSubmit(ListObj);
            this.Save();
        }
        
        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }
    }
}
