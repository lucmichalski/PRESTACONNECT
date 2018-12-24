using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOleaPromoRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOleaPromo Obj)
        {
            this.DBPrestashop.PsOleaPromo.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOleaPromo Obj)
        {
            this.DBPrestashop.PsOleaPromo.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOleaPromo> List()
        {
            System.Linq.IQueryable<PsOleaPromo> Return = from Table in this.DBPrestashop.PsOleaPromo
                                                                   select Table;
            return Return.ToList();
        }

        public PsOleaPromo ReadOleAPromo(UInt32 OleAPromo)
        {
            return DBPrestashop.PsOleaPromo.FirstOrDefault(cr => cr.IDOleAPromo == OleAPromo);
        }

        public Boolean ExistOleAPromo(UInt32 OleAPromo)
        {
            return (from Table in this.DBPrestashop.PsOleaPromo
                    where Table.IDOleAPromo == OleAPromo
                    select Table).Count() > 0;
        }
    }
}
