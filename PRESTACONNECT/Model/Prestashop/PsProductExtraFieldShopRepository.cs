using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductExtraFieldShopRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsProductExtraFieldShop Obj)
        {
			if (!Exist((int)Obj.IdProductExtraField, (int)Obj.IdShop))
			{
				this.DBPrestashop.PsProductExtraFieldShop.InsertOnSubmit(Obj);
			}
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public bool Exist(int IdProductExtraField, int IdShop)
		{
			return this.DBPrestashop.PsProductExtraFieldShop.ToList().Where(obj => obj.IdProductExtraField == IdProductExtraField && obj.IdShop == IdShop).Count() > 0;
		}

		public PsProductExtraFieldShop Read(int IdProductExtraField, int IdShop)
		{
			return this.DBPrestashop.PsProductExtraFieldShop.ToList().Where(obj => obj.IdProductExtraField == IdProductExtraField && obj.IdShop == IdShop).FirstOrDefault();
		}

		public void UpdateExtraField(PsProductExtraFieldShop Obj, string Field, string Value)
		{
			if (Core.Global.ExistModuleColumns("ps_product_extra_field_shop", new string[] { Field }))
				this.DBPrestashop.ExecuteCommand("UPDATE `ps_product_extra_field_shop` SET `" + Field + "` = '" + Value + "' WHERE `id_product_extra_field` = " + Obj.IdProductExtraField.ToString() + " AND `id_shop` = " + Obj.IdShop.ToString());
		}
	}
}
