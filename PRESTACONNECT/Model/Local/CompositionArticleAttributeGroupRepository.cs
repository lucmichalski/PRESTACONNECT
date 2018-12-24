using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CompositionArticleAttributeGroupRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(CompositionArticleAttributeGroup Obj)
        {
            this.DBLocal.CompositionArticleAttributeGroup.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(CompositionArticleAttributeGroup Obj)
        {
            this.DBLocal.CompositionArticleAttributeGroup.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteAll(List<CompositionArticleAttributeGroup> Obj)
        {
            this.DBLocal.CompositionArticleAttributeGroup.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistArticle(int Art_Id)
        {
            return this.DBLocal.CompositionArticleAttributeGroup.Count(cmp => cmp.Cag_ArtId == Art_Id) > 1;
        }

        public List<CompositionArticleAttributeGroup> ListArticle(int Art_Id)
        {
            return this.DBLocal.CompositionArticleAttributeGroup.Where(cmp => cmp.Cag_ArtId == Art_Id).ToList();
        }
    }
}
