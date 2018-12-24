using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsGroupLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsGroupLang> ListLang(uint IdLang)
        {
            System.Linq.IQueryable<PsGroupLang> Return = from Table in this.DBPrestashop.PsGroupLang
                                                         where Table.IDLang == IdLang
                                                         select Table;
            return Return.ToList();
        }

        public PsGroupLang Read(uint IdLang, uint IDGroup)
        {
            return DBPrestashop.PsGroupLang.FirstOrDefault(result => (result.IDLang == IdLang && result.IDGroup == IDGroup));
        }

        public bool Exist(uint IdLang, uint IDGroup)
        {
            return DBPrestashop.PsGroupLang.Count(result => (result.IDLang == IdLang && result.IDGroup == IDGroup)) > 0;
        }
    }
}
