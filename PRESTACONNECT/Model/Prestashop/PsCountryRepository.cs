using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCountryRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistIsoCode(String IsoCode)
        {
            return this.DBPrestashop.PsCountry.Count(Obj => Obj.IsoCode.ToUpper() == IsoCode.ToUpper()) == 1;
        }

        public PsCountry ReadIsoCode(String IsoCode)
        {
            return this.DBPrestashop.PsCountry.FirstOrDefault(Obj => Obj.IsoCode.ToUpper() == IsoCode.ToUpper());
        }

        public Boolean Exist(UInt32 Country)
        {
            return this.DBPrestashop.PsCountry.Count(c => c.IDCountry == Country) == 1;
        }
        public PsCountry Read(UInt32 Country)
        {
            return this.DBPrestashop.PsCountry.FirstOrDefault(c => c.IDCountry == Country);
        }
    }
}
