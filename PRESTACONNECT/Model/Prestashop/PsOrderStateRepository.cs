using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderStateRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsOrderState Obj)
        {
            this.DBPrestashop.PsOrderState.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Delete(PsOrderState Obj)
        {
            this.DBPrestashop.PsOrderState.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOrderState> List()
        {
            System.Linq.IQueryable<PsOrderState> Return = from Table in this.DBPrestashop.PsOrderState
                                                               select Table;
            return Return.ToList();
        }

        public Boolean ExistState(UInt32 State)
        {
            return (from Table in this.DBPrestashop.PsOrderState
                    where Table.IDOrderState == State
                    select Table).Count() > 0;
        }

        public PsOrderState ReadState(UInt32 State)
        {
            return (from Table in this.DBPrestashop.PsOrderState
                    where Table.IDOrderState == State
                    select Table).FirstOrDefault();
        }
    }
}
