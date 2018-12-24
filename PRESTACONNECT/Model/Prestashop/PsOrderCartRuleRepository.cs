using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderCartRuleRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOrderCartRule Obj)
        {
            this.DBPrestashop.PsOrderCartRule.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOrderCartRule Obj)
        {
            this.DBPrestashop.PsOrderCartRule.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOrderCartRule> List()
        {
            System.Linq.IQueryable<PsOrderCartRule> Return = from Table in this.DBPrestashop.PsOrderCartRule
                                                                   select Table;
            return Return.ToList();
        }

        public List<PsOrderCartRule> ListOrder(UInt32 Order)
        {
            System.Linq.IQueryable<PsOrderCartRule> Return = from Table in this.DBPrestashop.PsOrderCartRule
                                                                   where Table.IDOrder == Order
                                                                   select Table;
            return Return.ToList();
        }

        public Boolean ExistOrder(UInt32 Order)
        {
            return (from Table in this.DBPrestashop.PsOrderCartRule
                    where Table.IDOrder == Order
                    select Table).Count() > 0;
        }
    }
}
