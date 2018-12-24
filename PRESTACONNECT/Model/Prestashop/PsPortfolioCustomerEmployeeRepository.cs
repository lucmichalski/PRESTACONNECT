using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsPortfolioCustomerEmployeeRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsPortfolioCustomerEmployee Obj)
        {
            this.DBPrestashop.PsPortfolioCustomerEmployee.InsertOnSubmit(Obj);
            this.Save();
        }
        public void Add(List<PsPortfolioCustomerEmployee> list)
        {
            this.DBPrestashop.PsPortfolioCustomerEmployee.InsertAllOnSubmit(list);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean Exist(UInt32 Customer, UInt32 Employee)
        {
            return this.DBPrestashop.PsPortfolioCustomerEmployee.Count(pce => pce.IDCustomer == Customer && pce.IDEmployee == Employee) > 0;
        }

        public Boolean ExistCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsPortfolioCustomerEmployee.Count(pce => pce.IDCustomer == Customer) > 0;
        }
        public PsPortfolioCustomerEmployee ReadCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsPortfolioCustomerEmployee.FirstOrDefault(pce => pce.IDCustomer == Customer);
        }

        public List<PsPortfolioCustomerEmployee> ListCustomer(UInt32 Customer)
        {
            return this.DBPrestashop.PsPortfolioCustomerEmployee.Where(pce => pce.IDCustomer == Customer).ToList();
        }

        public Boolean ExistEmployee(UInt32 Employee)
        {
            return this.DBPrestashop.PsPortfolioCustomerEmployee.Count(pce => pce.IDEmployee == Employee) > 0;
        }
        public List<PsPortfolioCustomerEmployee> ListEmployee(UInt32 Employee)
        {
            return this.DBPrestashop.PsPortfolioCustomerEmployee.Where(pce => pce.IDEmployee == Employee).ToList();
        }

        public void Delete(PsPortfolioCustomerEmployee Obj)
        {
            this.DBPrestashop.PsPortfolioCustomerEmployee.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void Delete(List<PsPortfolioCustomerEmployee> list)
        {
            this.DBPrestashop.PsPortfolioCustomerEmployee.DeleteAllOnSubmit(list);
            this.Save();
        }
    }
}
