using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerFeatureValueLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCustomerFeatureValueLang Obj)
        {
            this.DBPrestashop.PsCustomerFeatureValueLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsCustomerFeatureValueLang Obj)
        {
            this.DBPrestashop.PsCustomerFeatureValueLang.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistCustomerFeatureValueLang(UInt32 CustomerFeatureValue, UInt32 Lang)
        {
            if (this.DBPrestashop.PsCustomerFeatureValueLang.Count(Obj => Obj.IDCustomerFeatureValue == CustomerFeatureValue && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistCustomerFeatureValueLang(String CustomerFeatureValue, UInt32 Lang)
        {
            return this.DBPrestashop.PsCustomerFeatureValueLang.Count(Obj => Obj.Value == CustomerFeatureValue && Obj.IDLang == Lang)> 0;
        }

        public PsCustomerFeatureValueLang ReadCustomerFeatureValueLang(UInt32 CustomerFeatureValue, UInt32 Lang)
        {
            return this.DBPrestashop.PsCustomerFeatureValueLang.FirstOrDefault(Obj => Obj.IDCustomerFeatureValue == CustomerFeatureValue && Obj.IDLang == Lang);
        }

        public PsCustomerFeatureValueLang ReadCustomerFeatureValueLang(String CustomerFeatureValue, UInt32 Lang)
        {
            return this.DBPrestashop.PsCustomerFeatureValueLang.FirstOrDefault(Obj => Obj.Value == CustomerFeatureValue && Obj.IDLang == Lang);
        }
    }
}
