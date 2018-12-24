using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsDWFProductExtraFieldRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsDWFProductExtraField Obj)
        {
            this.DBPrestashop.PsDWFProductExtraField.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public List<Model.Prestashop.PsDWFProductExtraField> List()
		{
			return this.DBPrestashop.PsDWFProductExtraField.ToList();
		}

		public List<Model.Prestashop.PsDWFProductExtraField> ListActive()
		{
			return this.List().Where(obj=> obj.Active == 1).ToList();
		}
	}
}
