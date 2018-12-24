using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderHistoryRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOrderHistory Obj)
        {
            this.DBPrestashop.PsOrderHistory.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOrderHistory Obj)
        {
            this.DBPrestashop.PsOrderHistory.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOrderHistory> List()
        {
            System.Linq.IQueryable<PsOrderHistory> Return = from Table in this.DBPrestashop.PsOrderHistory
                                                        select Table;
            return Return.ToList();
        }

        public List<PsOrderHistory> ListOrder(UInt32 Order)
        {
            System.Linq.IQueryable<PsOrderHistory> Return = from Table in this.DBPrestashop.PsOrderHistory
                                                            where Table.IDOrder == Order
                                                            select Table;
            return Return.ToList();
        }

        public List<PsOrderHistory> ListOrderTopOrderByDateAdd(UInt32 Order, Int32 Top)
        {
            System.Linq.IQueryable<PsOrderHistory> Return = (from Table in this.DBPrestashop.PsOrderHistory
                                                         where Table.IDOrder == Order
                                                         orderby Table.DateAdd descending
                                                         select Table).Take(Top);
            return Return.ToList();
        }

        public Boolean ExistOrderState(UInt32 Order, UInt32 State)
        {
            return (from Table in this.DBPrestashop.PsOrderHistory
                    where Table.IDOrder == Order
                    && Table.IDOrderState == State
                    select Table).Count() > 0;
        }
    }
}
