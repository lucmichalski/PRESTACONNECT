using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CompositionArticleAttributeRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(CompositionArticleAttribute Obj)
        {
            this.DBLocal.CompositionArticleAttribute.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(CompositionArticleAttribute Obj)
        {
            this.DBLocal.CompositionArticleAttribute.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistCompositionArticle(int ComArt_Id)
        {
            return this.DBLocal.CompositionArticleAttribute.Count(cmp => cmp.Caa_ComArtId == ComArt_Id) > 1;
        }

        public List<CompositionArticleAttribute> ListCompositionArticle(int ComArt_Id)
        {
            return this.DBLocal.CompositionArticleAttribute.Where(cmp => cmp.Caa_ComArtId == ComArt_Id).ToList();
        }

        public Boolean ExistAttributeGroupCompositionArticle(Int32 AttributeGroup, Int32 CompositionArticle)
        {
            if (this.DBLocal.CompositionArticleAttribute.Count(Obj => Obj.Caa_AttributeGroup_PreId == AttributeGroup && Obj.Caa_ComArtId == CompositionArticle) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public CompositionArticleAttribute ReadAttributeGroupCompositionArticle(Int32 AttributeGroup, Int32 CompositionArticle)
        {
            return this.DBLocal.CompositionArticleAttribute.FirstOrDefault(Obj => Obj.Caa_AttributeGroup_PreId == AttributeGroup && Obj.Caa_ComArtId == CompositionArticle);
        }
    }
}
