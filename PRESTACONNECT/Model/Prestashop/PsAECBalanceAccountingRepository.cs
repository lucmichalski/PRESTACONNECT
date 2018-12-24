using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECBalanceAccountingRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcBalanceAccounting Obj)
        {
            this.DBPrestashop.PsAEcBalanceAccounting.InsertOnSubmit(Obj);
            this.Save();
        }
        public void Add(List<PsAEcBalanceAccounting> list)
        {
            this.DBPrestashop.PsAEcBalanceAccounting.InsertAllOnSubmit(list);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcBalanceAccounting.Count(ba => ba.IDCustomer == Customer) > 0;
        }
        public PsAEcBalanceAccounting ReadCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcBalanceAccounting.FirstOrDefault(ba => ba.IDCustomer == Customer);
        }

        public List<PsAEcBalanceAccounting> ListCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcBalanceAccounting.Where(ba => ba.IDCustomer == Customer).ToList();
        }

        public void Delete(List<PsAEcBalanceAccounting> list)
        {
            this.DBPrestashop.PsAEcBalanceAccounting.DeleteAllOnSubmit(list);
            this.Save();
        }
    }
}
