using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class AttributeArticleRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(AttributeArticle Obj)
        {
            this.DBLocal.AttributeArticle.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(AttributeArticle Obj)
        {
            if (Obj.EnumereGamme1.AG_No.HasValue)
                this.DBLocal.Attribute.DeleteAllOnSubmit(this.DBLocal.Attribute.Where(at => at.Sag_Id == Obj.EnumereGamme1.cbMarq));
            if (Obj.EnumereGamme2.AG_No.HasValue)
                this.DBLocal.Attribute.DeleteAllOnSubmit(this.DBLocal.Attribute.Where(at => at.Sag_Id == Obj.EnumereGamme2.cbMarq));

            this.DBLocal.AttributeArticleImage.DeleteAllOnSubmit(this.DBLocal.AttributeArticleImage.Where(ati => ati.AttArt_Id == Obj.AttArt_Id));
            this.DBLocal.AttributeArticle.DeleteOnSubmit(Obj);
            this.Save();
        }

        public void DeleteAll(List<AttributeArticle> list)
        {
            List<int> list_ata = list.Select(ata => ata.AttArt_Id).ToList();
            List<int> list_agno = new List<int>();
            list_agno.AddRange(list.Where(ata => ata.EnumereGamme1.AG_No.HasValue).Select(ata => ata.EnumereGamme1.cbMarq));
            list_agno.AddRange(list.Where(ata => ata.EnumereGamme2.AG_No.HasValue).Select(ata => ata.EnumereGamme2.cbMarq));

            this.DBLocal.Attribute.DeleteAllOnSubmit(this.DBLocal.Attribute.Where(at => list_agno.Contains(at.Sag_Id)));
            this.DBLocal.AttributeArticleImage.DeleteAllOnSubmit(this.DBLocal.AttributeArticleImage.Where(ati => list_ata.Contains(ati.AttArt_Id)));
            this.DBLocal.AttributeArticle.DeleteAllOnSubmit(list);
            this.Save();
        }

        public Boolean ExistArticleDefault(Int32 Article, Boolean Default)
        {
            if (this.DBLocal.AttributeArticle.Count(Obj => Obj.Art_Id == Article && Obj.AttArt_Default == Default) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistArticleFirstAttribute(Int32 Article, Int32 FirstAttribute)
        {
            if (this.DBLocal.AttributeArticle.Count(Obj => Obj.Art_Id == Article && Obj.Att_IdFirst == FirstAttribute) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistArticleFirstAttributeSecondAttribute(Int32 Article, Int32 FirstAttribute, Int32 SecondAttribute)
        {
            if (this.DBLocal.AttributeArticle.Count(Obj => Obj.Art_Id == Article && Obj.Att_IdFirst == FirstAttribute && Obj.Att_IdSecond == SecondAttribute) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // <JG> 18/09/2012 modificaiton synchronisation des énumérés
        public Boolean ExistAttribute(Int32 Attribute)
        {
            return this.DBLocal.AttributeArticle.Count(Att => Att.Att_IdFirst == Attribute || Att.Att_IdSecond == Attribute) > 0;
        }

        public List<AttributeArticle> List()
        {
            System.Linq.IQueryable<AttributeArticle> Return = from Table in this.DBLocal.AttributeArticle
                                                              select Table;
            return Return.ToList();
        }

        public List<AttributeArticle> ListSync(Boolean Sync)
        {
            System.Linq.IQueryable<AttributeArticle> Return = from Table in this.DBLocal.AttributeArticle
                                                              where Table.AttArt_Sync == Sync
                                                              select Table;
            return Return.ToList();
        }

        public List<AttributeArticle> ListAttributeFirstArticle(Int32 AttributeFirst, Int32 Article)
        {
            System.Linq.IQueryable<AttributeArticle> Return = from Table in this.DBLocal.AttributeArticle
                                                              where Table.Att_IdFirst == AttributeFirst && Table.Art_Id == Article
                                                              select Table;
            return Return.ToList();
        }

        public List<AttributeArticle> ListArticle(Int32 Article)
        {
            System.Linq.IQueryable<AttributeArticle> Return = from Table in this.DBLocal.AttributeArticle
                                                              where Table.Art_Id == Article
                                                              select Table;
            return Return.ToList();
        }
        public List<Int32> ListSag_IdArticle(Int32 Article)
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBLocal.AttributeArticle
                                                   where Table.Art_Id == Article
                                                   select Table.Sag_Id;
            return Return.ToList();
        }

        public List<AttributeArticle> ListArticleSync(Int32 Article, Boolean Sync)
        {
            System.Linq.IQueryable<AttributeArticle> Return = from Table in this.DBLocal.AttributeArticle
                                                              where Table.Art_Id == Article && Table.AttArt_Sync == Sync
                                                              select Table;
            return Return.ToList();
        }

        public Boolean ExistPrestashop(Int32 Prestashop)
        {
            return this.DBLocal.AttributeArticle.Count(a => a.Pre_Id == Prestashop) == 1;
        }
        public AttributeArticle ReadPrestashop(Int32 Prestashop)
        {
            return this.DBLocal.AttributeArticle.FirstOrDefault(a => a.Pre_Id == Prestashop);
        }

        public Boolean ExistSage(Int32 sage)
        {
            return this.DBLocal.AttributeArticle.Count(a => a.Sag_Id == sage) == 1;
        }
        public AttributeArticle ReadSage(Int32 sage)
        {
            return this.DBLocal.AttributeArticle.FirstOrDefault(a => a.Sag_Id == sage);
        }

        public Boolean Exist(Int32 AttributeArticle)
        {
            return this.DBLocal.AttributeArticle.Count(a => a.AttArt_Id == AttributeArticle) == 1;
        }
        public AttributeArticle Read(Int32 AttributeArticle)
        {
            return this.DBLocal.AttributeArticle.FirstOrDefault(a => a.AttArt_Id == AttributeArticle);
        }

        public void DeleteLinkAttributeArticleImage(Int32 AttributeArticle, Int32 Image)
        {
            if (this.DBLocal.AttributeArticleImage.Count(ati => ati.ImaArt_Id == Image && ati.AttArt_Id == AttributeArticle) > 0)
                this.DBLocal.AttributeArticleImage.DeleteOnSubmit(this.DBLocal.AttributeArticleImage.First(ati => ati.ImaArt_Id == Image && ati.AttArt_Id == AttributeArticle));
        }
    }
}
