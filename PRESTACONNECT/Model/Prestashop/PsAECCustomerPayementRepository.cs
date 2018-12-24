using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECCustomerPayementRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcCustomerPayement Obj)
        {
            this.DBPrestashop.PsAEcCustomerPayement.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCustomerPayement(UInt32 Customer, UInt32 Payement)
        {
            return this.DBPrestashop.PsAEcCustomerPayement.Count(Obj => Obj.IDCustomer == Customer && Obj.IDSage == Payement) > 0;
        }

        public PsAEcCustomerPayement ReadCustomerPayement(UInt32 Customer, UInt32 Payement)
        {
            return this.DBPrestashop.PsAEcCustomerPayement.FirstOrDefault(Obj => Obj.IDCustomer == Customer && Obj.IDSage == Payement);
        }

        public IQueryable<PsAEcCustomerPayement> ListCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsAEcCustomerPayement.Where(Obj => Obj.IDCustomer == Customer);
        }

        public void Delete(PsAEcCustomerPayement Obj)
        {
            String TxtSQL = "delete from ps_aec_customer_payement where id_customer = " + Obj.IDCustomer  + " AND Sag_Id = " + Obj.IDSage;
            this.DBPrestashop.ExecuteCommand(TxtSQL);
            // not working ...
            //this.DBPrestashop.PsAEcCustomerPayement.DeleteOnSubmit(Obj);
        }
    }
}
