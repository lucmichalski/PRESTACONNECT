using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CompositionArticleRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(CompositionArticle Obj)
        {
            this.DBLocal.CompositionArticle.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(CompositionArticle Obj)
        {
            this.DBLocal.CompositionArticleImage.DeleteAllOnSubmit(this.DBLocal.CompositionArticleImage.Where(cai => cai.ComArt_Id == Obj.ComArt_Id));
            this.DBLocal.CompositionArticleAttribute.DeleteAllOnSubmit(this.DBLocal.CompositionArticleAttribute.Where(caa => caa.Caa_ComArtId == Obj.ComArt_Id));
            this.DBLocal.CompositionArticle.DeleteOnSubmit(Obj);
            this.Save();
        }

        public void DeleteAll(List<CompositionArticle> List)
        {
            List<int> list_ca = List.Select(ca => ca.ComArt_Id).ToList();
            this.DBLocal.CompositionArticleImage.DeleteAllOnSubmit(this.DBLocal.CompositionArticleImage.Where(cai => list_ca.Contains(cai.ComArt_Id)));
            this.DBLocal.CompositionArticleAttribute.DeleteAllOnSubmit(this.DBLocal.CompositionArticleAttribute.Where(caa => list_ca.Contains(caa.Caa_ComArtId)));
            this.DBLocal.CompositionArticle.DeleteAllOnSubmit(List);
            this.Save();
        }

        public Boolean ExistArticle(int Art_Id)
        {
            return this.DBLocal.CompositionArticle.Count(cmp => cmp.ComArt_ArtId == Art_Id) > 1;
        }

        public List<CompositionArticle> ListArticle(int Art_Id)
        {
            return this.DBLocal.CompositionArticle.Where(cmp => cmp.ComArt_ArtId == Art_Id).ToList();
        }

        public void DeleteLinkCompositionArticleAttribute(Int32 CompositionArticle, Int32 AttributeGroup)
        {
            if (this.DBLocal.CompositionArticleAttribute.Count(caa => caa.Caa_AttributeGroup_PreId == AttributeGroup && caa.Caa_ComArtId == CompositionArticle) > 0)
                this.DBLocal.CompositionArticleAttribute.DeleteOnSubmit(this.DBLocal.CompositionArticleAttribute.First(caa => caa.Caa_AttributeGroup_PreId == AttributeGroup && caa.Caa_ComArtId == CompositionArticle));
        }

        public Boolean ExistPrestaShop(int ProductAttribute)
        {
            return this.DBLocal.CompositionArticle.Count(ca => ca.Pre_Id != null && ca.Pre_Id == ProductAttribute) > 0;
        }
        public CompositionArticle ReadPrestaShop(int ProductAttribute)
        {
            return this.DBLocal.CompositionArticle.FirstOrDefault(ca => ca.Pre_Id != null && ca.Pre_Id == ProductAttribute);
        }

        public Boolean ExistSageGamme(Int32 sagegamme)
        {
            return this.DBLocal.CompositionArticle.Count(ca => ca.ComArt_F_ARTENUMREF_SagId == sagegamme) == 1;
        }
        public CompositionArticle ReadSageGamme(Int32 sagegamme)
        {
            return this.DBLocal.CompositionArticle.FirstOrDefault(ca => ca.ComArt_F_ARTENUMREF_SagId == sagegamme);
        }

        public Boolean ExistSage(Int32 sage)
        {
            return this.DBLocal.CompositionArticle.Count(ca => ca.ComArt_F_ARTICLE_SagId == sage) > 0;
        }
        public CompositionArticle ReadSage(Int32 sage)
        {
            return this.DBLocal.CompositionArticle.FirstOrDefault(ca => ca.ComArt_F_ARTICLE_SagId == sage);
        }

        public Boolean Exist(Int32 CompositionArticle)
        {
            return this.DBLocal.CompositionArticle.Count(ca => ca.ComArt_Id == CompositionArticle) == 1;
        }
        public CompositionArticle Read(Int32 CompositionArticle)
        {
            return this.DBLocal.CompositionArticle.FirstOrDefault(ca => ca.ComArt_Id == CompositionArticle);
        }

        public List<Int32> ListSageF_ARTICLE()
        {
            return this.DBLocal.CompositionArticle.Where(ca => ca.ComArt_F_ARTENUMREF_SagId == null && ca.ComArt_F_CONDITION_SagId == null).Select(ca => ca.ComArt_F_ARTICLE_SagId).ToList();
        }
        public List<Int32> ListSageF_ARTENUMREF()
        {
            return this.DBLocal.CompositionArticle.Where(ca => ca.ComArt_F_ARTENUMREF_SagId != null).Select(ca => ca.ComArt_F_ARTENUMREF_SagId.Value).ToList();
        }
        public List<Int32> ListSageF_CONDITION()
        {
            return this.DBLocal.CompositionArticle.Where(ca => ca.ComArt_F_CONDITION_SagId != null).Select(ca => ca.ComArt_F_CONDITION_SagId.Value).ToList();
        }
        public List<Int32> ListSageF_ARTICLE(Int32 Article)
        {
            return this.DBLocal.CompositionArticle.Where(ca => ca.ComArt_ArtId == Article && ca.ComArt_F_ARTENUMREF_SagId == null && ca.ComArt_F_CONDITION_SagId == null).Select(ca => ca.ComArt_F_ARTICLE_SagId).ToList();
        }
        public List<Int32> ListSageF_ARTENUMREF(Int32 Article)
        {
            return this.DBLocal.CompositionArticle.Where(ca => ca.ComArt_ArtId == Article && ca.ComArt_F_ARTENUMREF_SagId != null).Select(ca => ca.ComArt_F_ARTENUMREF_SagId.Value).ToList();
        }
        public List<Int32> ListSageF_CONDITION(Int32 Article)
        {
            return this.DBLocal.CompositionArticle.Where(ca => ca.ComArt_ArtId == Article && ca.ComArt_F_CONDITION_SagId != null).Select(ca => ca.ComArt_F_CONDITION_SagId.Value).ToList();
        }

        public List<Int32> ListF_ARTICLESageId()
        {
            return this.DBLocal.CompositionArticle.Select(ca => ca.ComArt_F_ARTICLE_SagId).ToList();
        }

        public void DeleteLinkCompositionArticleImage(Int32 CompositionArticle, Int32 Image)
        {
            if (this.DBLocal.CompositionArticleImage.Count(cai => cai.ImaArt_Id == Image && cai.ComArt_Id == CompositionArticle) > 0)
                this.DBLocal.CompositionArticleImage.DeleteOnSubmit(this.DBLocal.CompositionArticleImage.First(cai => cai.ImaArt_Id == Image && cai.ComArt_Id == CompositionArticle));
        }
    }
}
