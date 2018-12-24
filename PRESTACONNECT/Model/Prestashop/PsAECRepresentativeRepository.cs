using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECRepresentativeRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcRepresentative Obj)
        {
            this.DBPrestashop.PsAEcRepresentative.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistSage(UInt32 CO_No)
        {
            return this.DBPrestashop.PsAEcRepresentative.Count(Coll => Coll.IDSage == CO_No) > 0;
        }

        public PsAEcRepresentative ReadSage(UInt32 CO_No)
        {
            return this.DBPrestashop.PsAEcRepresentative.FirstOrDefault(Obj => Obj.IDSage == CO_No);
        }
    }
}
