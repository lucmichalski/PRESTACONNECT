using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    class StatistiqueArticleRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(StatistiqueArticle StatArticle)
        {
            this.DBLocal.StatistiqueArticle.InsertOnSubmit(StatArticle);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(StatistiqueArticle StatArticle)
        {
            this.DBLocal.StatistiqueArticle.DeleteOnSubmit(StatArticle);
            this.Save();
        }

        public Boolean ExistStatArticle(String StatArticle)
        {
            return this.DBLocal.StatistiqueArticle.Count(s => s.Sag_StatArt == StatArticle) > 0;
        }

        public StatistiqueArticle ReadStatArticle(String StatArticle)
        {
            return this.DBLocal.StatistiqueArticle.FirstOrDefault(s => s.Sag_StatArt == StatArticle);
        }

        public Boolean ExistCharacteristic(UInt32 Characteristic)
        {
            return this.DBLocal.StatistiqueArticle.Count(s => s.Cha_Id == Characteristic) > 0;
        }

        public StatistiqueArticle ReadCharacteristic(UInt32 Characteristic)
        {
            return this.DBLocal.StatistiqueArticle.FirstOrDefault(s => s.Cha_Id == Characteristic);
        }

        public List<StatistiqueArticle> List()
        {
            return this.DBLocal.StatistiqueArticle.ToList();
        }

        public List<StatistiqueArticle> ListSync()
        {
            return this.DBLocal.StatistiqueArticle.Where(s => s.Inf_Mode != (short)Core.Parametres.InformationLibreValeursMode.NonTransferees && s.Cha_Id != 0).ToList();
        }
    }
}
