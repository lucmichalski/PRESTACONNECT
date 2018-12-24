using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsFeatureLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsFeatureLang> ListLang(uint IdLang, uint IdShop)
        {
            //System.Linq.IQueryable<PsFeatureLang> Return = from Table in this.DBPrestashop.PsFeatureLang
            //                                             where Table.IDLang == IdLang
            //                                             select Table;
            //return Return.ToList();

            List<PsFeatureLang> attributes = new List<PsFeatureLang>(
                DBPrestashop.ExecuteQuery<PsFeatureLang>(
                "SELECT DISTINCT FL.id_feature, FL.id_lang, FL.name FROM ps_feature_lang FL " +
                " INNER JOIN ps_feature_shop FS ON FS.id_feature = FL.id_feature " +
                " WHERE FS.id_shop = {0} and FL.id_lang = {1} " +
                " ", IdShop, IdLang));

            return attributes;
        }

        public void Add(PsFeatureLang obj)
        {
            this.DBPrestashop.PsFeatureLang.InsertOnSubmit(obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistSageInformationLibre(String SageInfoLibre)
        {
            return this.DBPrestashop.PsFeatureLang.Count(f => f.Name == SageInfoLibre) > 0;
        }
    }
}
