using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    class StatistiqueClientRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(StatistiqueClient StatClient)
        {
            this.DBLocal.StatistiqueClient.InsertOnSubmit(StatClient);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(StatistiqueClient StatClient)
        {
            this.DBLocal.StatistiqueClient.DeleteOnSubmit(StatClient);
            this.Save();
        }

        public Boolean ExistStatClient(String StatClient)
        {
            return this.DBLocal.StatistiqueClient.Count(s => s.Sag_StatClient == StatClient) > 0;
        }

        public StatistiqueClient ReadStatClient(String StatClient)
        {
            return this.DBLocal.StatistiqueClient.FirstOrDefault(s => s.Sag_StatClient == StatClient);
        }

        public Boolean ExistFeature(UInt32 Feature)
        {
            return this.DBLocal.StatistiqueClient.Count(s => s.Cha_Id == Feature) > 0;
        }

        public StatistiqueClient ReadFeature(UInt32 Feature)
        {
            return this.DBLocal.StatistiqueClient.FirstOrDefault(s => s.Cha_Id == Feature);
        }

        public List<StatistiqueClient> List()
        {
            return this.DBLocal.StatistiqueClient.ToList();
        }

        public List<StatistiqueClient> ListSync()
        {
            return this.DBLocal.StatistiqueClient.Where(s => s.Inf_Mode != (short)Core.Parametres.InformationLibreValeursMode.NonTransferees && s.Cha_Id != 0).ToList();
        }
    }
}
