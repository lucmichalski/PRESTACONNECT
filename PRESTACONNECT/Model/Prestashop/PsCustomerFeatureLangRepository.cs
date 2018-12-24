using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerFeatureLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsCustomerFeatureLang> ListLang(uint IdLang, uint IdShop)
        {
            //System.Linq.IQueryable<PsCustomerFeatureLang> Return = from Table in this.DBPrestashop.PsCustomerFeatureLang
            //                                             where Table.IDLang == IdLang
            //                                             select Table;
            //return Return.ToList();

            List<PsCustomerFeatureLang> attributes = new List<PsCustomerFeatureLang>(
                DBPrestashop.ExecuteQuery<PsCustomerFeatureLang>(
                "SELECT DISTINCT CFL.id_customer_feature, CFL.id_lang, CFL.name FROM ps_customer_feature_lang CFL " +
                " INNER JOIN ps_customer_feature_shop CFS ON CFS.id_customer_feature = CFL.id_customer_feature " +
                " WHERE CFS.id_shop = {0} and CFL.id_lang = {1} " +
                " ", IdShop, IdLang));

            return attributes;
        }

        public void Add(PsCustomerFeatureLang obj)
        {
            this.DBPrestashop.PsCustomerFeatureLang.InsertOnSubmit(obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistSageInformationLibre(String SageInfoLibre)
        {
            return this.DBPrestashop.PsCustomerFeatureLang.Count(f => f.Name == SageInfoLibre) > 0;
        }
    }
}
