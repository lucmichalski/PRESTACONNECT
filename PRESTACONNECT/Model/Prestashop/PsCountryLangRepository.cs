using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCountryLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistCountryLang(UInt32 Country, UInt32 Lang)
        {
            if (this.DBPrestashop.PsCountryLang.Count(Obj => Obj.IDCountry == Country && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsCountryLang ReadCountryLang(UInt32 Country, UInt32 Lang)
        {
            return this.DBPrestashop.PsCountryLang.FirstOrDefault(Obj => Obj.IDCountry == Country && Obj.IDLang == Lang);
        }

        public Boolean ExistCountryLang(String CountryName, UInt32 Lang)
        {
            return this.DBPrestashop.PsCountryLang.Count(Obj => Obj.Name.ToUpper() == CountryName.ToUpper() && Obj.IDLang == Lang) > 0;
        }

        public PsCountryLang ReadCountryLang(String CountryName, UInt32 Lang)
        {
            return this.DBPrestashop.PsCountryLang.FirstOrDefault(Obj => Obj.Name.ToUpper() == CountryName.ToUpper() && Obj.IDLang == Lang);
        }

        public List<PsCountryLang> ListActive(Byte Active, UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<PsCountryLang>(
                "SELECT DISTINCT CL.id_country, CL.id_lang, CL.name FROM ps_country_lang CL " +
                " INNER JOIN ps_country C " +
                " INNER JOIN ps_country_shop CS " +
                " ON CL.id_country = C.id_country " +
                " WHERE C.active = {0} AND CS.id_shop = {1} " +
                " ORDER BY CL.name " +
				#if (PRESTASHOP_VERSION_172)
				", CL.id_country, CL.id_lang " +
				#endif
				"", Active, IDShop).ToList();
        }
    }
}
