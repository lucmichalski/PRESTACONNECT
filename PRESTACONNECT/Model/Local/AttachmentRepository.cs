using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class AttachmentRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Attachment Obj)
        {
            this.DBLocal.Attachment.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Attachment Obj)
        {
            this.DBLocal.Attachment.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteAll(List<Attachment> Obj)
        {
            this.DBLocal.Attachment.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistAttachment(Int32 Id)
        {
            return this.DBLocal.Attachment.Count(Obj => Obj.Att_Id == Id) > 0;
        }

        public Attachment ReadAttachment(Int32 Id)
        {
            return this.DBLocal.Attachment.FirstOrDefault(Obj => Obj.Att_Id == Id);
        }


        public Boolean ExistPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Attachment.Count(Obj => Obj.Pre_Id == Pre_Id) > 0;
        }

        public Attachment ReadPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Attachment.FirstOrDefault(Obj => Obj.Pre_Id == Pre_Id);
        }

        public Boolean ExistPre_IdArt_Id(Int32 Pre_Id, Int32 Art_Id)
        {
            return this.DBLocal.Attachment.Count(Obj => Obj.Pre_Id == Pre_Id && Obj.Art_Id == Art_Id) > 0;
        }

        public List<Attachment> List()
        {
            return this.DBLocal.Attachment.ToList();
        }

        public List<Attachment> ListArticle(Int32 Article)
        {
            return this.DBLocal.Attachment.Where(a => a.Art_Id == Article).ToList();
        }

        public Boolean ExistFileArticle(String File, Int32 Article)
        {
            return this.DBLocal.Attachment.Count(a => a.Att_FileName == File && a.Art_Id == Article) > 0;
        }

        public Boolean ExistArticle(Int32 Article)
        {
            return this.DBLocal.Attachment.Count(a => a.Art_Id == Article) > 0;
        }

        public List<int> ListIDArticleNotSync()
        {
            return (from Table in this.DBLocal.Attachment
                    where Table.Pre_Id == null && Table.Art_Id != null
                    select Table.Art_Id).ToList();
        }
    }
}
