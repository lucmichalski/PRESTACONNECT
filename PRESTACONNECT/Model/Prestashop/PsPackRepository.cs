using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsPackRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsPack Obj)
        {
            this.DBPrestashop.PsPack.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsPack Obj)
        {
            this.DBPrestashop.PsPack.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsPack> List()
        {
            IQueryable<PsPack> Return = from Table in this.DBPrestashop.PsPack
                                        select Table;
            return Return.ToList();
        }

        public List<PsPack> ListProductPack(UInt32 IdProductPack)
        {
            IQueryable<PsPack> Return = from Table in this.DBPrestashop.PsPack
                                        where Table.IDProductPack == IdProductPack
                                        select Table;
            return Return.ToList();
        }
    }
}
