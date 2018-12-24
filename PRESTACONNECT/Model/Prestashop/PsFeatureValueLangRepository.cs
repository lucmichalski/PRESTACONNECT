using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsFeatureValueLangRepository
    {
        private static string ps_fields_fat = "FL.id_feature_value, FL.id_lang, FL.value, F.id_feature, F.custom ";
        private static string ps_fields_fat_order = "FL.value, FL.id_feature_value, FL.id_lang, F.id_feature, F.custom ";

        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsFeatureValueLang Obj)
        {
            this.DBPrestashop.PsFeatureValueLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsFeatureValueLang Obj)
        {
            this.DBPrestashop.PsFeatureValueLang.DeleteOnSubmit(Obj);
            this.Save();
        }

        public void DeleteAll(IEnumerable<PsFeatureValueLang> list)
        {
            this.DBPrestashop.PsFeatureValueLang.DeleteAllOnSubmit(list);
            this.Save();
        }

        public Boolean ExistFeatureValueLang(UInt32 FeatureValue, UInt32 Lang)
        {
            if (this.DBPrestashop.PsFeatureValueLang.Count(Obj => Obj.IDFeatureValue == FeatureValue && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsFeatureValueLang ReadFeatureValueLang(UInt32 FeatureValue, UInt32 Lang)
        {
            return this.DBPrestashop.PsFeatureValueLang.FirstOrDefault(Obj => Obj.IDFeatureValue == FeatureValue && Obj.IDLang == Lang);
        }

        public IEnumerable<PsFeatureValueLang> ListLang(UInt32 Lang)
        {
            return DBPrestashop.ExecuteQuery<PsFeatureValueLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_feature_value_lang FL " +
                " INNER JOIN ps_feature_value F ON FL.id_feature_value = F.id_feature_value " +
                " WHERE FL.id_lang = " + Core.Global.Lang +
                " AND F.custom = 0 " +
                " ORDER BY " + ps_fields_fat_order);
        }

        public IEnumerable<PsFeatureValueLang> ListFeatureLang(UInt32 Feature, UInt32 Lang)
        {
            return DBPrestashop.ExecuteQuery<PsFeatureValueLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_feature_value_lang FL " +
                " INNER JOIN ps_feature_value F ON FL.id_feature_value = F.id_feature_value " +
                " WHERE FL.id_lang = " + Core.Global.Lang +
                " AND F.id_feature = " + Feature +
                " AND F.custom = 0 " +
                " ORDER BY " + ps_fields_fat_order);
        }
        public IEnumerable<PsFeatureValueLang> ListFeatureLang(UInt32 Feature, UInt32 Lang, string filter)
        {
            return DBPrestashop.ExecuteQuery<PsFeatureValueLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_feature_value_lang FL " +
                " INNER JOIN ps_feature_value F ON FL.id_feature_value = F.id_feature_value " +
                " WHERE FL.id_lang = " + Core.Global.Lang +
                " AND F.id_feature = " + Feature +
                " AND FL.value like '" + filter + "%'" +
                " AND F.custom = 0 " +
                " ORDER BY " + ps_fields_fat_order);
        }

        public Boolean ExistFeatureValueLang(String FeatureValue, UInt32 Lang, UInt32 Feature)
        {
            // <JG> 30/01/2015 correction requete sql si valeur contient une apostrophe
            FeatureValue = Core.Global.EscapeSqlString(FeatureValue);
            // <JG> 03/01/2017 ajout filtre custom
            return DBPrestashop.ExecuteQuery<PsFeatureValueLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_feature_value_lang FL " +
                " INNER JOIN ps_feature_value F ON FL.id_feature_value = F.id_feature_value " +
                " WHERE FL.id_lang = " + Core.Global.Lang +
                " AND F.id_feature = " + Feature +
                " AND binary FL.value = '" + FeatureValue + "' " +
                " AND F.custom = 0 " +
                " ORDER BY " + ps_fields_fat_order).Count() > 0;
        }

        public PsFeatureValueLang ReadFeatureValueLang(String FeatureValue, UInt32 Lang, UInt32 Feature)
        {
            // <JG> 30/01/2015 correction requete sql si valeur contient une apostrophe
            FeatureValue = Core.Global.EscapeSqlString(FeatureValue);
            // <JG> 03/01/2017 ajout filtre custom
            return DBPrestashop.ExecuteQuery<PsFeatureValueLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_feature_value_lang FL " +
                " INNER JOIN ps_feature_value F ON FL.id_feature_value = F.id_feature_value " +
                " WHERE FL.id_lang = " + Core.Global.Lang +
                " AND F.id_feature = " + Feature +
                " AND binary FL.value = '" + FeatureValue + "' " +
                " AND F.custom = 0 " +
                " ORDER BY " + ps_fields_fat_order).FirstOrDefault();
        }

        public Boolean ExistFeatureValue(uint FeatureValue)
        {
            return DBPrestashop.PsFeatureValueLang.Count(f => f.IDFeatureValue == FeatureValue) > 0;
        }

        public IEnumerable<PsFeatureValueLang> ListFeatureValue(uint FeatureValue)
        {
            return DBPrestashop.PsFeatureValueLang.Where(f => f.IDFeatureValue == FeatureValue);
        }
    }
}
