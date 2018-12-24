using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class AttributeArticleImageRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(AttributeArticleImage Obj)
        {
            this.DBLocal.AttributeArticleImage.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(AttributeArticleImage Obj)
        {
            this.DBLocal.AttributeArticleImage.DeleteOnSubmit(Obj);
            this.Save();
        }

        public void DeleteAll(List<AttributeArticleImage> Obj)
        {
            this.DBLocal.AttributeArticleImage.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public List<AttributeArticleImage> List()
        {
            System.Linq.IQueryable<AttributeArticleImage> Return = from Table in this.DBLocal.AttributeArticleImage
                                                              select Table;
            return Return.ToList();
        }

        public List<AttributeArticleImage> ListAttributeArticle(Int32 AttributeArticle)
        {
            System.Linq.IQueryable<AttributeArticleImage> Return = from Table in this.DBLocal.AttributeArticleImage
                                                              where Table.AttArt_Id == AttributeArticle
                                                              select Table;
            return Return.ToList();
        }

        public List<AttributeArticleImage> ListImageArticle(Int32 ImageArticle)
        {
            System.Linq.IQueryable<AttributeArticleImage> Return = from Table in this.DBLocal.AttributeArticleImage
                                                              where Table.ImaArt_Id == ImageArticle
                                                              select Table;
            return Return.ToList();
        }

        public Boolean ExistAttributeArticleImage(Int32 AttributeArticle, Int32 ImageArticle)
        {
            if (this.DBLocal.AttributeArticleImage.Count(Obj => Obj.AttArt_Id == AttributeArticle && Obj.ImaArt_Id == ImageArticle) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public AttributeArticleImage ReadAttributeArticleImage(Int32 AttributeArticle, Int32 ImageArticle)
        {
            return this.DBLocal.AttributeArticleImage.FirstOrDefault(Obj => Obj.AttArt_Id == AttributeArticle && Obj.ImaArt_Id == ImageArticle);
        }

        public Boolean ExistAttributeArticle(Int32 AttributeArticle)
        {
            if (this.DBLocal.AttributeArticleImage.Count(Obj => Obj.AttArt_Id == AttributeArticle) == 0)
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
