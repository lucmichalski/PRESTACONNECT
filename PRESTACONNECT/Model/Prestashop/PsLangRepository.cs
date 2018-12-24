using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsLang> ListActive(Byte Active, UInt32 IDShop)
        {
            //System.Linq.IQueryable<PsLang> Return = from Table in this.DBPrestashop.PsLang
            //                                        where Table.Active == Active 
            //                                        select Table;

            //return Return.ToList();

            return DBPrestashop.ExecuteQuery<PsLang>(
                "SELECT DISTINCT L.id_lang, L.name, L.active, L.iso_code, L.language_code, L.date_format_lite, L.date_format_full, L.is_rtl FROM ps_lang L " +
                " INNER JOIN ps_lang_shop LS ON LS.id_lang = L.id_lang " +
                " WHERE L.active = {0} AND LS.id_shop = {1} " +
                " ", Active, IDShop).ToList();
        }

        public Boolean ExistId(UInt32 Id)
        {
            if (this.DBPrestashop.PsLang.Count(Obj => Obj.IDLang == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsLang ReadId(UInt32 Id)
        {
            return this.DBPrestashop.PsLang.FirstOrDefault(Obj => Obj.IDLang == Id);
        }

        public Boolean ExistIso(String Iso)
        {
            if (this.DBPrestashop.PsLang.Count(Obj => Obj.IsoCode == Iso) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsLang ReadIso(String Iso)
        {
            return this.DBPrestashop.PsLang.FirstOrDefault(Obj => Obj.IsoCode == Iso);
        }
    }
}
