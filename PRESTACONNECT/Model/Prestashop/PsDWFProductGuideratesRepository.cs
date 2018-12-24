using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsDWFProductGuideratesRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsDWFProductGuiderates Obj)
        {
            this.DBPrestashop.PsDWFProductGuiderates.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public List<Model.Prestashop.PsDWFProductGuiderates> List()
		{
			return this.DBPrestashop.PsDWFProductGuiderates.ToList();
		}

		public Model.Prestashop.PsDWFProductGuiderates SelectField(string fieldName)
		{
			return this.List().Where(obj => obj.Name == fieldName).FirstOrDefault();
		}

		public List<Model.Prestashop.PsDWFProductGuiderates> ListActive()
		{
			return this.List().Where(obj=> obj.Active == 1).ToList();
		}
	}
}
