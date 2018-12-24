using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ArticleCatalogRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(ArticleCatalog Obj)
        {
            this.DBLocal.ArticleCatalog.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(ArticleCatalog Obj)
        {
            this.DBLocal.ArticleCatalog.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteAll(List<ArticleCatalog> Obj)
        {
            this.DBLocal.ArticleCatalog.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistArticleCatalog(Int32 Article, Int32 Catalog)
        {
            if (this.DBLocal.ArticleCatalog.Count(Obj => Obj.Art_Id == Article && Obj.Cat_Id == Catalog) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public ArticleCatalog ReadArticleCatalog(Int32 Article, Int32 Catalog)
        {
            return this.DBLocal.ArticleCatalog.FirstOrDefault(Obj => Obj.Art_Id == Article && Obj.Cat_Id == Catalog);
        }

        public List<Int32> ListCataloguesArticles(List<Int32> Articles)
        {
            List<Int32> result = new List<int>();

            List<Model.Local.Catalog> Catalogs = new Model.Local.CatalogRepository().List();
            // récupération des ids des catalogues rattachés aux articles sélectionnés
            result.AddRange(Catalogs.Where(c => c.Article.Count(a => Articles.Contains(a.Art_Id)) > 0
                                            || c.ArticleCatalog.Count(ac => Articles.Contains(ac.Art_Id)) > 0)
                                    .Select(c => c.Cat_Id));
            // récupération des catalogues parents de ceux déjà filtrés
            Catalogs = Catalogs.Where(c => c.ChildrenContainsCatalog(result)).ToList();
            result.AddRange(Catalogs.Select(c => c.Cat_Id));

            // tri des doublons
            result = result.Distinct().ToList();

            return result;
        }
        public List<Int32> ListCataloguesArticle(Int32 Article)
        {
            List<Int32> result = new List<int>();

            List<Model.Local.Catalog> Catalogs = new Model.Local.CatalogRepository().List();
            // récupération des ids des catalogues rattachés aux articles sélectionnés
            result.AddRange(Catalogs.Where(c => c.Article.Count(a => a.Art_Id == Article) > 0
                                            || c.ArticleCatalog.Count(ac => ac.Art_Id == Article) > 0)
                                    .Select(c => c.Cat_Id));
            // récupération des catalogues parents de ceux déjà filtrés
            Catalogs = Catalogs.Where(c => c.ChildrenContainsCatalog(result)).ToList();
            result.AddRange(Catalogs.Select(c => c.Cat_Id));

            // tri des doublons
            result = result.Distinct().ToList();

            return result;
        }

        public List<ArticleCatalog> ListArticle(Int32 Article)
        {
            IQueryable<ArticleCatalog> Return = from Table in this.DBLocal.ArticleCatalog
                                                where Table.Art_Id == Article
                                                select Table;
            return Return.ToList();
        }

        public List<ArticleCatalog> ListCatalog(Int32 Catalog)
        {
            IQueryable<ArticleCatalog> Return = from Table in this.DBLocal.ArticleCatalog
                                                where Table.Cat_Id == Catalog
                                                select Table;
            return Return.ToList();
        }
    }
}
