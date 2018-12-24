using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class InformationLibreArticleRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(InformationLibreArticle Obj)
        {
            this.DBLocal.InformationLibreArticle.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(InformationLibreArticle Obj)
        {
            this.DBLocal.InformationLibreArticle.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistInfoLibre(String InfoLibreArticle, Boolean IsStat)
        {
            return this.DBLocal.InformationLibreArticle.Count(Obj => Obj.Sag_InfoLibreArticle == InfoLibreArticle && Obj.Inf_IsStat == IsStat) > 0;
        }

        public Boolean ExistInfoLibreLevel(String InfoLibreArticle, Int32 Level)
        {
            return this.DBLocal.InformationLibreArticle.Count(Obj => Obj.Sag_InfoLibreArticle == InfoLibreArticle && Obj.Inf_Catalogue == Level) > 0;
        }


        public InformationLibreArticle ReadInfoLibre(String InfoLibreArticle, Int32 Level)
        {
            return this.DBLocal.InformationLibreArticle.FirstOrDefault(Obj => Obj.Sag_InfoLibreArticle == InfoLibreArticle && Obj.Inf_Catalogue == Level);
        }

        public List<InformationLibreArticle> List()
        {
            return this.DBLocal.InformationLibreArticle.ToList();
        }
    }
}
