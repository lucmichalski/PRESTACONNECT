using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductGuideRateRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsProductGuideRate Obj)
		{
			if (Exist((int)Obj.IdProduct, (int)Obj.IdShopDefault, (int)Obj.IdDwfproductguiderates))
			{
				PsProductGuideRate temp = Read((int)Obj.IdProduct, (int)Obj.IdShopDefault, (int)Obj.IdDwfproductguiderates);
				temp.Rate = Obj.Rate;
				temp.DateUpd = DateTime.Now;
				Obj.IdProductGuideRate = temp.IdProductGuideRate;
			}
			else
			{
				Obj.DateAdd = DateTime.Now;
				Obj.DateUpd = Obj.DateAdd;
				this.DBPrestashop.PSProductGuideRate.InsertOnSubmit(Obj);
			}
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public bool Exist(int IdProduct, int IdShopDefault, int IdDwfproductguiderates)
		{
			return this.DBPrestashop.PSProductGuideRate.ToList().Where(obj => obj.IdProduct == IdProduct && obj.IdShopDefault == IdShopDefault && obj.IdDwfproductguiderates == IdDwfproductguiderates).Count() > 0;
		}

		public PsProductGuideRate Read(int IdProduct, int IdShopDefault, int IdDwfproductguiderates)
		{
			return this.DBPrestashop.PSProductGuideRate.ToList().Where(obj => obj.IdProduct == IdProduct && obj.IdShopDefault == IdShopDefault && obj.IdDwfproductguiderates == IdDwfproductguiderates).FirstOrDefault();
		}
	}
}
