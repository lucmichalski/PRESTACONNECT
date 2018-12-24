using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CompositionArticleImageRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(CompositionArticleImage Obj)
        {
            this.DBLocal.CompositionArticleImage.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(CompositionArticleImage Obj)
        {
            this.DBLocal.CompositionArticleImage.DeleteOnSubmit(Obj);
            this.Save();
        }

        public void DeleteAll(List<CompositionArticleImage> Obj)
        {
            this.DBLocal.CompositionArticleImage.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public List<CompositionArticleImage> List()
        {
            System.Linq.IQueryable<CompositionArticleImage> Return = from Table in this.DBLocal.CompositionArticleImage
                                                              select Table;
            return Return.ToList();
        }

        public List<CompositionArticleImage> ListCompositionArticle(Int32 CompositionArticle)
        {
            System.Linq.IQueryable<CompositionArticleImage> Return = from Table in this.DBLocal.CompositionArticleImage
                                                                     where Table.ComArt_Id == CompositionArticle
                                                              select Table;
            return Return.ToList();
        }

        public List<CompositionArticleImage> ListImageArticle(Int32 ImageArticle)
        {
            System.Linq.IQueryable<CompositionArticleImage> Return = from Table in this.DBLocal.CompositionArticleImage
                                                              where Table.ImaArt_Id == ImageArticle
                                                              select Table;
            return Return.ToList();
        }

        public Boolean ExistCompositionArticleImage(Int32 CompositionArticle, Int32 ImageArticle)
        {
            if (this.DBLocal.CompositionArticleImage.Count(Obj => Obj.ComArt_Id == CompositionArticle && Obj.ImaArt_Id == ImageArticle) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public CompositionArticleImage ReadCompositionArticleImage(Int32 CompositionArticle, Int32 ImageArticle)
        {
            return this.DBLocal.CompositionArticleImage.FirstOrDefault(Obj => Obj.ComArt_Id == CompositionArticle && Obj.ImaArt_Id == ImageArticle);
        }

        public Boolean ExistCompositionArticle(Int32 AttributeArticle)
        {
            if (this.DBLocal.CompositionArticleImage.Count(Obj => Obj.ComArt_Id == AttributeArticle) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
