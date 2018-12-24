using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class InformationArticleRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(InformationArticle Obj)
        {
            this.DBLocal.InformationArticle.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(InformationArticle Obj)
        {
            this.DBLocal.InformationArticle.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistSageInfoArticle(Int32 SageInfoArticle)
        {
            return this.DBLocal.InformationArticle.Count(Obj => Obj.Sag_InfoArt == SageInfoArticle) > 0;
        }

        public InformationArticle ReadSageInfoArticle(Int32 SageInfoArticle)
        {
            return this.DBLocal.InformationArticle.FirstOrDefault(Obj => Obj.Sag_InfoArt == SageInfoArticle);
        }

        public Boolean ExistFeature(UInt32 Feature)
        {
            return this.DBLocal.InformationArticle.Count(Obj => Obj.Cha_Id == Feature) > 0;
        }

        public InformationArticle ReadFeature(UInt32 Feature)
        {
            return this.DBLocal.InformationArticle.FirstOrDefault(Obj => Obj.Cha_Id == Feature);
        }

        public List<InformationArticle> List()
        {
            return this.DBLocal.InformationArticle.ToList();
        }

        public List<InformationArticle> ListSync()
        {
            return this.DBLocal.InformationArticle.Where(i => i.Inf_Mode != (short)Core.Parametres.InformationLibreValeursMode.NonTransferees && i.Cha_Id != 0).ToList();
        }
    }
}
