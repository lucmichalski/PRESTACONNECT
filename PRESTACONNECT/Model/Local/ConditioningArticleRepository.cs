using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ConditioningArticleRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(ConditioningArticle Obj)
        {
            this.DBLocal.ConditioningArticle.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(ConditioningArticle Obj)
        {
            this.DBLocal.Conditioning.DeleteAllOnSubmit(this.DBLocal.Conditioning.Where(con => con.Sag_Id == Obj.Sag_Id));
            this.DBLocal.ConditioningArticle.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteAll(List<ConditioningArticle> list)
        {
            List<int> sag_id = list.Select(conart => conart.Sag_Id).ToList();
            this.DBLocal.Conditioning.DeleteAllOnSubmit(this.DBLocal.Conditioning.Where(con => sag_id.Contains(con.Sag_Id)));
            this.DBLocal.ConditioningArticle.DeleteAllOnSubmit(list);
            this.Save();
        }

        public Boolean ExistSag_Id(Int32 SageId)
        {
            return this.DBLocal.ConditioningArticle.Count(c => c.Sag_Id == SageId) > 0;
        }

        public ConditioningArticle ReadSag_Id(Int32 SageId)
        {
            return this.DBLocal.ConditioningArticle.FirstOrDefault(c => c.Sag_Id == SageId);
        }

        public Boolean ExistArticleDefault(Int32 Article, Boolean Default)
        {
            if (this.DBLocal.ConditioningArticle.Count(Obj => Obj.Art_Id == Article && Obj.ConArt_Default == Default) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistArticleConditioning(Int32 Article, Int32 Conditioning)
        {
            if (this.DBLocal.ConditioningArticle.Count(Obj => Obj.Art_Id == Article && Obj.Con_Id == Conditioning) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public ConditioningArticle ReadArticleConditioning(Int32 Article, Int32 Conditioning)
        {
            return this.DBLocal.ConditioningArticle.FirstOrDefault(c => c.Art_Id == Article && c.Con_Id == Conditioning);
        }

        // <JG> 18/09/2012 modificaiton synchronisation des énumérés
        public Boolean ExistConditioning(Int32 Conditioning)
        {
            return this.DBLocal.ConditioningArticle.Count(Att => Att.Con_Id == Conditioning) > 0;
        }

        public List<ConditioningArticle> List()
        {
            System.Linq.IQueryable<ConditioningArticle> Return = from Table in this.DBLocal.ConditioningArticle
                                                                 select Table;
            return Return.ToList();
        }

        public List<ConditioningArticle> ListSync(Boolean Sync)
        {
            System.Linq.IQueryable<ConditioningArticle> Return = from Table in this.DBLocal.ConditioningArticle
                                                                 where Table.ConArt_Sync == Sync
                                                                 select Table;
            return Return.ToList();
        }

        public List<ConditioningArticle> ListConditioningArticle(Int32 Conditioning, Int32 Article)
        {
            System.Linq.IQueryable<ConditioningArticle> Return = from Table in this.DBLocal.ConditioningArticle
                                                                 where Table.Con_Id == Conditioning && Table.Art_Id == Article
                                                                 select Table;
            return Return.ToList();
        }

        public List<ConditioningArticle> ListArticle(Int32 Article)
        {
            System.Linq.IQueryable<ConditioningArticle> Return = from Table in this.DBLocal.ConditioningArticle
                                                                 where Table.Art_Id == Article
                                                                 select Table;
            return Return.ToList();
        }

        public List<ConditioningArticle> ListArticleSync(Int32 Article, Boolean Sync)
        {
            System.Linq.IQueryable<ConditioningArticle> Return = from Table in this.DBLocal.ConditioningArticle
                                                                 where Table.Art_Id == Article && Table.ConArt_Sync == Sync
                                                                 select Table;
            return Return.ToList();
        }
        
        public Boolean ExistPrestashop(Int32 Prestashop)
        {
            return this.DBLocal.ConditioningArticle.Count(c => c.Pre_Id == Prestashop) == 1;
        }
        public ConditioningArticle ReadPrestashop(Int32 Prestashop)
        {
            return this.DBLocal.ConditioningArticle.FirstOrDefault(c => c.Pre_Id == Prestashop);
        }
    }
}
