using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductExtraFieldRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsProductExtraField Obj)
		{
			if (Exist((int)Obj.IdProduct, (int)Obj.IdShopDefault))
			{
				PsProductExtraField temp = Read((int)Obj.IdProduct, (int)Obj.IdShopDefault);
				Obj.IdProductExtraField = temp.IdProductExtraField;
			}
			else
			{
				Obj.DateAdd = DateTime.Now;
				Obj.DateUpd = Obj.DateAdd;
				this.DBPrestashop.PsProductExtraField.InsertOnSubmit(Obj);
				this.Save();
			}
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public bool Exist(int IdProduct, int IdShopDefault)
		{
			return this.DBPrestashop.PsProductExtraField.ToList().Where(obj => obj.IdProduct == IdProduct && obj.IdShopDefault == IdShopDefault).Count() > 0;
		}

		public PsProductExtraField Read(int IdProduct, int IdShopDefault)
		{
			return this.DBPrestashop.PsProductExtraField.ToList().Where(obj => obj.IdProduct == IdProduct && obj.IdShopDefault == IdShopDefault).FirstOrDefault();
		}

		public void UpdateExtraField(PsProductExtraField Obj, string Field, string Value)
		{
			if (Core.Global.ExistModuleColumns("ps_product_extra_field", new string[] { Field }))
				this.DBPrestashop.ExecuteCommand("UPDATE `ps_product_extra_field` SET `" + Field + "` = '" + Value + "' WHERE `id_product_extra_field` = " + Obj.IdProductExtraField.ToString());
		}
	}
}
