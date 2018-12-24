using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerFeatureCustomerRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCustomerFeatureCustomer Obj)
        {
            this.DBPrestashop.PsCustomerFeatureCustomer.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsCustomerFeatureCustomer Obj)
        {
            this.DBPrestashop.PsCustomerFeatureCustomer.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistCustomerFeatureCustomer(UInt32 CustomerFeature, UInt32 Customer)
        {
            if (this.DBPrestashop.PsCustomerFeatureCustomer.Count(Obj => Obj.IDCustomerFeature == CustomerFeature && Obj.IDCustomer == Customer) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsCustomerFeatureCustomer ReadCustomerFeatureCustomer(UInt32 CustomerFeature, UInt32 Customer)
        {
            return this.DBPrestashop.PsCustomerFeatureCustomer.FirstOrDefault(Obj => Obj.IDCustomerFeature == CustomerFeature && Obj.IDCustomer == Customer);
        }
        public List<PsCustomerFeatureCustomer> List(UInt32 IDCustomer)
        {
            System.Linq.IQueryable<PsCustomerFeatureCustomer> Return = from Table in this.DBPrestashop.PsCustomerFeatureCustomer
                                                              where Table.IDCustomer == IDCustomer
                                                              select Table;

            return Return.ToList();
        }
    }
}
