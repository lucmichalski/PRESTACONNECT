using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCartRuleRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsCartRule Obj)
        {
            this.DBPrestashop.PsCartRule.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsCartRule Obj)
        {
            this.DBPrestashop.PsCartRule.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsCartRule> List()
        {
            System.Linq.IQueryable<PsCartRule> Return = from Table in this.DBPrestashop.PsCartRule
                                                                   select Table;
            return Return.ToList();
        }

        public PsCartRule ReadCartRule(UInt32 CartRule)
        {
            return DBPrestashop.PsCartRule.FirstOrDefault(cr => cr.IDCartRule == CartRule);
        }

        public Boolean ExistCartRule(UInt32 CartRule)
        {
            return (from Table in this.DBPrestashop.PsCartRule
                    where Table.IDCartRule == CartRule
                    select Table).Count() > 0;
        }
    }
}
