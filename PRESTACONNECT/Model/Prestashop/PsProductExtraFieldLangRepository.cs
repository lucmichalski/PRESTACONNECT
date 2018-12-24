using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductExtraFieldLangRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsProductExtraFieldLang Obj)
        {
			if (!Exist((int)Obj.IdProductExtraField, (int)Obj.IdShop, (int)Obj.IdLang))
			{
				this.DBPrestashop.PsProductExtraFieldLang.InsertOnSubmit(Obj);
			}
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public bool Exist(int IdProductExtraField, int IdShop, int IdLang)
		{
			return this.DBPrestashop.PsProductExtraFieldLang.ToList().Where(obj => obj.IdProductExtraField == IdProductExtraField && obj.IdShop == IdShop && obj.IdLang == IdLang).Count() > 0;
		}

		public PsProductExtraFieldLang Read(int IdProductExtraField, int IdShop, int IdLang)
		{
			return this.DBPrestashop.PsProductExtraFieldLang.ToList().Where(obj => obj.IdProductExtraField == IdProductExtraField && obj.IdShop == IdShop && obj.IdLang == IdLang).FirstOrDefault();
		}

		public void UpdateExtraField(PsProductExtraFieldLang Obj, string Field, string Value)
		{
			if (Core.Global.ExistModuleColumns("ps_product_extra_field_lang", new string[]{ Field }))
				this.DBPrestashop.ExecuteCommand("UPDATE `ps_product_extra_field_lang` SET `" + Field + "` = '" + Value + "' WHERE `id_product_extra_field` = " + Obj.IdProductExtraField.ToString() + " AND `id_shop` = " + Obj.IdShop.ToString());
		}
	}
}
