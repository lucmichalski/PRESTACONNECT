using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCartRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCart Cart)
        {
            this.DBPrestashop.PsCart.InsertOnSubmit(Cart);
            this.Save();
        }

        public Boolean Exist(UInt32 Cart)
        {
            return this.DBPrestashop.PsCart.Count(c => c.IDCart == Cart) > 0;
        }

        public PsCart Read(UInt32 Cart)
        {
            return this.DBPrestashop.PsCart.FirstOrDefault(c => c.IDCart == Cart);
        }

        public List<PsCart> List()
        {
            return this.DBPrestashop.PsCart.ToList();
        }
    }
}
