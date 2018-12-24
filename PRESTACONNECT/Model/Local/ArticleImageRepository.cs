using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ArticleImageRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(ArticleImage Obj)
        {
            this.DBLocal.ArticleImage.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(ArticleImage Obj)
        {
            Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new AttributeArticleImageRepository();
            AttributeArticleImageRepository.DeleteAll(AttributeArticleImageRepository.ListImageArticle(Obj.ImaArt_Id));
            Model.Local.CompositionArticleImageRepository CompositionArticleImageRepository = new CompositionArticleImageRepository();
            CompositionArticleImageRepository.DeleteAll(CompositionArticleImageRepository.ListImageArticle(Obj.ImaArt_Id));

            this.DBLocal.ArticleImage.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteAll(List<ArticleImage> list)
        {
            foreach (var Obj in list)
            {
                Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new AttributeArticleImageRepository();
                AttributeArticleImageRepository.DeleteAll(AttributeArticleImageRepository.ListImageArticle(Obj.ImaArt_Id));
                Model.Local.CompositionArticleImageRepository CompositionArticleImageRepository = new CompositionArticleImageRepository();
                CompositionArticleImageRepository.DeleteAll(CompositionArticleImageRepository.ListImageArticle(Obj.ImaArt_Id));
            }
            this.DBLocal.ArticleImage.DeleteAllOnSubmit(list);
            this.Save();
        }

        public List<ArticleImage> List()
        {
            System.Linq.IQueryable<ArticleImage> Return = from Table in this.DBLocal.ArticleImage
                                                          select Table;
            return Return.ToList();
        }

        public List<ArticleImage> ListArticle(Int32 Article)
        {
            System.Linq.IQueryable<ArticleImage> Return = from Table in this.DBLocal.ArticleImage
                                                          where Table.Art_Id == Article
                                                          orderby Table.ImaArt_Position
                                                          select Table;
            return Return.ToList();
        }
        public List<Int32> ListPositionsArticle(Int32 Article)
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBLocal.ArticleImage
                                                   where Table.Art_Id == Article
                                                   select Table.ImaArt_Position;
            return Return.ToList();
        }

        public List<ArticleImage> ListArticlePrestashop(Int32 Article, Int32 Prestashop)
        {
            System.Linq.IQueryable<ArticleImage> Return = from Table in this.DBLocal.ArticleImage
                                                          where Table.Art_Id == Article && Table.Pre_Id == Prestashop
                                                          select Table;
            return Return.ToList();
        }

        public Boolean ExistArticleImage(Int32 ArticleImage)
        {
            if (this.DBLocal.ArticleImage.Count(Obj => Obj.ImaArt_Id == ArticleImage) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistPre_Id(Int32 Pre_Id)
        {
            if (this.DBLocal.ArticleImage.Count(Obj => Obj.Pre_Id == Pre_Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistArticleDefault(Int32 Article, Boolean Default)
        {
            if (this.DBLocal.ArticleImage.Count(Obj => Obj.ImaArt_Default == Default && Obj.Art_Id == Article) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistArticlePosition(Int32 Article, Int32 Position)
        {
            if (this.DBLocal.ArticleImage.Count(Obj => Obj.Art_Id == Article && Obj.ImaArt_Position == Position) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public ArticleImage ReadArticleImage(Int32 ArticleImage)
        {
            return this.DBLocal.ArticleImage.FirstOrDefault(Obj => Obj.ImaArt_Id == ArticleImage);
        }

        //<JG> 28/05/2012
        public Boolean ExistArticleFile(Int32 Article, String SourceFile)
        {
            return this.DBLocal.ArticleImage.Count(Obj => Obj.Art_Id == Article && Obj.ImaArt_SourceFile == SourceFile) > 0;
        }

        public ArticleImage ReadArticleFile(Int32 Article, String SourceFile)
        {
            return this.DBLocal.ArticleImage.FirstOrDefault(Obj => Obj.Art_Id == Article && Obj.ImaArt_SourceFile == SourceFile);
        }

        public Boolean ExistArticle(Int32 Article)
        {
            return this.DBLocal.ArticleImage.Count(Obj => Obj.Art_Id == Article) > 0;
        }

        public Boolean ExistPrestaShop(Int32 IDImage)
        {
            return this.DBLocal.ArticleImage.Count(Obj => Obj.Pre_Id == IDImage) > 0;
        }
        public ArticleImage ReadPrestaShop(Int32 IDImage)
        {
            return this.DBLocal.ArticleImage.FirstOrDefault(Obj => Obj.Pre_Id == IDImage);
        }

        public List<int> ListIDArticleNotSync()
        {
            return (from Table in this.DBLocal.ArticleImage
                    where Table.Pre_Id == null && Table.Art_Id != null
                    select Table.Art_Id.Value).ToList();
        }
    }
}
