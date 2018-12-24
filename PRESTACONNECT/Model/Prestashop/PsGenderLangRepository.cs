using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsGenderLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistGenderLang(UInt32 Gender, UInt32 Lang)
        {
            return this.DBPrestashop.PsGenderLang.Count(Obj => Obj.IDGender == Gender && Obj.IDLang == Lang) > 0;
        }

        public PsGenderLang ReadGenderLang(UInt32 Gender, UInt32 Lang)
        {
            return this.DBPrestashop.PsGenderLang.FirstOrDefault(Obj => Obj.IDGender == Gender && Obj.IDLang == Lang);
        }
    }
}
