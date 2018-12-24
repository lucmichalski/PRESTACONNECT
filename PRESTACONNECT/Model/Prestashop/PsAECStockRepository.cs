using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECStockRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcStock Obj)
        {
            this.DBPrestashop.PsAEcStock.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean Exist(UInt32 IDStockAvailable)
        {
            return this.DBPrestashop.PsAEcStock.Count(s => s.IDStockAvailable == IDStockAvailable) > 0;
        }

        public PsAEcStock Read(UInt32 IDStockAvailable)
        {
            return this.DBPrestashop.PsAEcStock.FirstOrDefault(s => s.IDStockAvailable == IDStockAvailable);
        }
    }
}
