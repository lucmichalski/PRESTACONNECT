using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAttributeLangRepository
    {
        private static string ps_fields_fat = "AL.id_attribute, AL.id_lang, AL.name, A.id_attribute_group, A.position ";
        private static string ps_fields_fat_OrderName = "AL.name, AL.id_attribute, AL.id_lang, A.id_attribute_group, A.position ";
        private static string ps_fields_fat_OrderPosition = "A.position, AL.id_attribute, AL.id_lang, AL.name, A.id_attribute_group ";

        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsAttributeLang Obj)
        {
            this.DBPrestashop.PsAttributeLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsAttributeLang Obj)
        {
            this.DBPrestashop.PsAttributeLang.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsAttributeLang> List()
        {
            System.Linq.IQueryable<PsAttributeLang> Return = from Table in this.DBPrestashop.PsAttributeLang
                                                             select Table;
            return Return.ToList();
        }

        public Boolean ExistAttributeLang(UInt32 Attribute, UInt32 Lang)
        {
            return this.DBPrestashop.PsAttributeLang.Count(Obj => Obj.IDAttribute == Attribute && Obj.IDLang == Lang) > 0;
        }

        public PsAttributeLang ReadAttributeLang(UInt32 Attribute, UInt32 Lang)
        {
            return this.DBPrestashop.PsAttributeLang.FirstOrDefault(Obj => Obj.IDAttribute == Attribute && Obj.IDLang == Lang);
        }

        public IEnumerable<PsAttributeLang> ListAttributeLang(UInt32 AttributeGroup, UInt32 Lang)
        {
            return DBPrestashop.ExecuteQuery<PsAttributeLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_attribute_lang AL " +
                " INNER JOIN ps_attribute A ON AL.id_attribute = A.id_attribute " +
                " WHERE AL.id_lang = " + Core.Global.Lang +
                " AND A.id_attribute_group = " + AttributeGroup +
                " ORDER BY " + ps_fields_fat_OrderName);
        }

        public IEnumerable<PsAttributeLang> ListLang(UInt32 Lang)
        {
            return DBPrestashop.ExecuteQuery<PsAttributeLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_attribute_lang AL " +
                " INNER JOIN ps_attribute A ON AL.id_attribute = A.id_attribute " +
                " WHERE AL.id_lang = " + Core.Global.Lang +
                " ORDER BY " + ps_fields_fat_OrderName);
        }

        public Boolean ExistAttributeLang(String value, UInt32 Lang, UInt32 AttributeGroup)
        {
            value = Core.Global.EscapeSqlString(value);
            return DBPrestashop.ExecuteQuery<PsAttributeLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_attribute_lang AL " +
                " INNER JOIN ps_attribute A ON AL.id_attribute = A.id_attribute " +
                " WHERE AL.id_lang = " + Core.Global.Lang +
                " AND A.id_attribute_group = " + AttributeGroup +
                " AND AL.name = '" + value + "' " +
                " ORDER BY " + ps_fields_fat_OrderPosition).Count() > 0;
        }

        public PsAttributeLang ReadAttributeLang(String value, UInt32 Lang, UInt32 AttributeGroup)
        {
            value = Core.Global.EscapeSqlString(value);
            return DBPrestashop.ExecuteQuery<PsAttributeLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_attribute_lang AL " +
                " INNER JOIN ps_attribute A ON AL.id_attribute = A.id_attribute " +
                " WHERE AL.id_lang = " + Core.Global.Lang +
                " AND A.id_attribute_group = " + AttributeGroup +
                " AND AL.name = '" + value + "' " +
                " ORDER BY " + ps_fields_fat_OrderPosition).FirstOrDefault();
        }

        public PsAttributeLang ReadAttributeLangFull(UInt32 Attribute, UInt32 Lang)
        {
            return DBPrestashop.ExecuteQuery<PsAttributeLang>(
                "SELECT DISTINCT " + ps_fields_fat +
                " FROM ps_attribute_lang AL " +
                " INNER JOIN ps_attribute A ON AL.id_attribute = A.id_attribute " +
                " WHERE AL.id_lang = " + Core.Global.Lang +
                " AND AL.id_attribute = " + Attribute +
                " ORDER BY " + ps_fields_fat_OrderPosition).FirstOrDefault();
        }
    }
}
