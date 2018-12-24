using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerFeatureValueRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCustomerFeatureValue Obj)
        {
            this.DBPrestashop.PsCustomerFeatureValue.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsCustomerFeatureValue Obj)
        {
            this.DBPrestashop.PsCustomerFeatureValue.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistFeatureValue(UInt32 CustomerFeatureValue)
        {
            if (this.DBPrestashop.PsCustomerFeatureValue.Count(Obj => Obj.IDCustomerFeatureValue == CustomerFeatureValue) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsCustomerFeatureValue ReadFeatureValue(UInt32 CustomerFeatureValue)
        {
            return this.DBPrestashop.PsCustomerFeatureValue.FirstOrDefault(Obj => Obj.IDCustomerFeatureValue == CustomerFeatureValue);
        }

        public List<PsCustomerFeatureValue> ListFeature(UInt32 CustomerFeature)
        {
            System.Linq.IQueryable<PsCustomerFeatureValue> Return;
            Return = from Table in this.DBPrestashop.PsCustomerFeatureValue
                                                        where Table.IDCustomerFeature == CustomerFeature
                                                        select Table;
            return Return.ToList();
        }
    }
}
