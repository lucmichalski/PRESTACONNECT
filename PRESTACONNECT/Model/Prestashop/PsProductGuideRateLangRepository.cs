using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductGuideRateLangRepository
	{
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsProductGuideRateLang Obj)
        {
			if (Exist((int)Obj.IdProductGuideRate, (int)Obj.IdShop, (int)Obj.IdLang))
			{
				PsProductGuideRateLang temp = Read((int)Obj.IdProductGuideRate, (int)Obj.IdShop, (int)Obj.IdLang);
				temp.Comment = Obj.Comment;
			}
			else
			{
				this.DBPrestashop.PsProductGuideRateLang.InsertOnSubmit(Obj);
			}
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
		}

		public bool Exist(int IdProductGuideRate, int IdShop, int IdLang)
		{
			return this.DBPrestashop.PsProductGuideRateLang.ToList().Where(obj => obj.IdProductGuideRate == IdProductGuideRate && obj.IdShop == IdShop && obj.IdLang == IdLang).Count() > 0;
		}

		public PsProductGuideRateLang Read(int IdProductGuideRate, int IdShop, int IdLang)
		{
			return this.DBPrestashop.PsProductGuideRateLang.ToList().Where(obj => obj.IdProductGuideRate == IdProductGuideRate && obj.IdShop == IdShop && obj.IdLang == IdLang).FirstOrDefault();
		}
	}
}
