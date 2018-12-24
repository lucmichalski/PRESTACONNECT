using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsDWFProductExtraFieldsLangRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsDWFProductExtraFieldsLang Obj)
        {
            this.DBPrestashop.PsDWFProductExtraFieldsLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public List<Model.Prestashop.PsDWFProductExtraFieldsLang> List()
		{
			return this.DBPrestashop.PsDWFProductExtraFieldsLang.ToList();
		}

		public Model.Prestashop.PsDWFProductExtraFieldsLang GetDWFProductExtraFieldsLang(uint IdDWFProductExtraFields, int IdLang)
		{
			return List().Where(obj => obj.IdDWFProductExtraFields == IdDWFProductExtraFields && obj.IdLang == IdLang).FirstOrDefault();
		}
	}
}
