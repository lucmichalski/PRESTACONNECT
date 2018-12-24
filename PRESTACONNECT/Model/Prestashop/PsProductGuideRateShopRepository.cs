using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductGuideRateShopRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsProductGuideRateShop Obj)
        {
			if (!Exist((int)Obj.IdProductGuideRate, (int)Obj.IdShop))
			{
				this.DBPrestashop.PsProductGuideRateShop.InsertOnSubmit(Obj);
			}
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public bool Exist(int IdProductGuideRate, int IdShop)
		{
			return this.DBPrestashop.PsProductGuideRateShop.ToList().Where(obj => obj.IdProductGuideRate == IdProductGuideRate && obj.IdShop == IdShop).Count() > 0;
		}

		public PsProductGuideRateShop Read(int IdProductGuideRate, int IdShop)
		{
			return this.DBPrestashop.PsProductGuideRateShop.ToList().Where(obj => obj.IdProductGuideRate == IdProductGuideRate && obj.IdShop == IdShop).FirstOrDefault();
		}
	}
}
