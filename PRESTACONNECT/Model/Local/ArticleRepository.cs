using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ArticleRepository
    {
        private static string _fields_progress = "Art_Id, Art_Ref, Art_Name ";

        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Article Obj)
        {
            this.DBLocal.Article.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Article Obj)
        {
            this.DBLocal.Article.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistDefaultCatalog(Int32 Catalog)
        {
            return this.DBLocal.Article.Count(a => a.Cat_Id == Catalog) > 0;
        }


        public Boolean ExistArticle(Int32 Article)
        {
            if (this.DBLocal.Article.Count(Obj => Obj.Art_Id == Article) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Article ReadArticle(Int32 Article)
        {
            return this.DBLocal.Article.FirstOrDefault(Obj => Obj.Art_Id == Article);
        }

        public Article_Progress ReadArticleProgress(Int32 Article)
        {
            return DBLocal.ExecuteQuery<Article_Progress>("SELECT " + _fields_progress + " FROM Article WHERE Art_Id = " + Article).FirstOrDefault();
        }

        public Boolean ExistSag_Id(Int32 Sag_Id)
        {
            if (this.DBLocal.Article.Count(Obj => Obj.Sag_Id == Sag_Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Article ReadSag_Id(Int32 Sag_Id)
        {
            return this.DBLocal.Article.FirstOrDefault(Obj => Obj.Sag_Id == Sag_Id);
        }


        public Boolean ExistPre_Id(Int32 Pre_Id)
        {
            if (this.DBLocal.Article.Count(Obj => Obj.Pre_Id == Pre_Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Article ReadPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Article.FirstOrDefault(Obj => Obj.Pre_Id == Pre_Id);
        }

        public List<Article> List()
        {
            IQueryable<Article> Return = from Table in this.DBLocal.Article
                                         select Table;
            return Return.ToList();
        }

        public List<Article_Light> ListLight()
        {
            return DBLocal.ExecuteQuery<Article_Light>("SELECT Art_Id, Sag_Id FROM Article").ToList();
        }

        public List<Article> ListSync(Boolean Sync)
        {
            IQueryable<Article> Return = from Table in this.DBLocal.Article
                                         where Table.Art_Sync == Sync
                                         select Table;
            return Return.ToList();
        }

        public List<Article> ListActive(Boolean Active)
        {
            IQueryable<Article> Return = from Table in this.DBLocal.Article
                                         where Table.Art_Active == Active
                                         select Table;
            return Return.ToList();
        }

        public List<Article> ListSyncActive(Boolean Sync, Boolean Active)
        {
            return (from Table in this.DBLocal.Article
                    where Table.Art_Sync == Sync && Table.Art_Active == Active
                    select Table).ToList();
        }

        public List<Int32> ListIdSync(Boolean Sync)
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Article
                                       where Table.Art_Sync == Sync
                                       select Table.Art_Id;
            return Return.ToList();
        }
        public List<Int32> ListIdSyncPack(Boolean Sync)
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Article
                                       where Table.Art_Sync == Sync && Table.Art_Pack
                                       select Table.Art_Id;
            return Return.ToList();
        }
        public List<Int32> ListIdSyncPrice(Boolean Sync, Boolean SyncPrice)
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Article
                                       where Table.Art_Sync == Sync && Table.Art_SyncPrice == SyncPrice
                                       select Table.Art_Id;
            return Return.ToList();
        }

        public List<Int32> ListId()
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Article
                                       select Table.Art_Id;
            return Return.ToList();
        }

        public List<Int32> ListPrestashop()
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Article
                                       where Table.Pre_Id != null
                                       select Table.Pre_Id.Value;
            return Return.ToList();
        }

        public List<String> ListSageReference()
        {
            return (from Table in this.DBLocal.Article
                    select Table.Art_Ref).ToList();
        }
        public List<Int32> ListSageId()
        {
            return (from Table in this.DBLocal.Article
                    select Table.Sag_Id).AsParallel().ToList();
        }

        public List<Int32> ListIdSyncOrderByPack(Boolean Sync)
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Article
                                       where Table.Art_Sync == Sync
                                       orderby Table.Art_Pack
                                       select Table.Art_Id;
            return Return.ToList();
        }

        public List<Article> ListCatalog(Int32 Catalog)
        {
            IQueryable<Article> Return = from Table in this.DBLocal.Article
                                         where Table.Cat_Id == Catalog || Table.ArticleCatalog.Count(cat => cat.Cat_Id == Catalog) > 0
                                         select Table;
            return Return.ToList();
        }

        public List<Article> ListWithoutCatalog()
        {
            IQueryable<Article> Return = from Table in this.DBLocal.Article
                                         where Table.Catalog == null && Table.ArticleCatalog.Count(cat => cat.Art_Id == Table.Art_Id) == 0
                                         select Table;
            return Return.ToList();
        }

        public List<Article> ListWithCatalog()
        {
            IQueryable<Article> Return = from Table in this.DBLocal.Article
                                         where Table.Cat_Id != 0 && Table.Catalog != null && Table.ArticleCatalog.Count(cat => cat.Art_Id == Table.Art_Id) > 0
                                         select Table;
            return Return.ToList();
        }

        public List<Article> ListTypeComposition()
        {
            return (from Table in this.DBLocal.Article
                    where Table.Art_Type == (short)Model.Local.Article.enum_TypeArticle.ArticleComposition
                    select Table).AsParallel().ToList();
        }
        public List<Int32> ListIdTypeComposition()
        {
            return (from Table in this.DBLocal.Article
                    where Table.Art_Type == (short)Model.Local.Article.enum_TypeArticle.ArticleComposition
                    select Table.Art_Id).AsParallel().ToList();
        }
    }
}
