using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAttachmentLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsAttachmentLang Obj)
        {
            this.DBPrestashop.PsAttachmentLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsAttachmentLang Obj)
        {
            this.DBPrestashop.PsAttachmentLang.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void Delete(List<PsAttachmentLang> Obj)
        {
            this.DBPrestashop.PsAttachmentLang.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public List<PsAttachmentLang> List()
        {
            System.Linq.IQueryable<PsAttachmentLang> Return = from Table in this.DBPrestashop.PsAttachmentLang
                                                         select Table;
            return Return.ToList();
        }
        public List<PsAttachmentLang> ListAttachment(UInt32 Attachment)
        {
            System.Linq.IQueryable<PsAttachmentLang> Return = from Table in this.DBPrestashop.PsAttachmentLang
                                                              where Table.IDAttachment == Attachment
                                                              select Table;
            return Return.ToList();
        }

        public Boolean ExistAttachmentLang(UInt32 Attachment, UInt32 Lang)
        {
            if (this.DBPrestashop.PsAttachmentLang.Count(Obj => Obj.IDAttachment == Attachment && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsAttachmentLang ReadAttachmentLang(UInt32 Attachment, UInt32 Lang)
        {
            return this.DBPrestashop.PsAttachmentLang.FirstOrDefault(Obj => Obj.IDAttachment == Attachment && Obj.IDLang == Lang);
        }
    }
}
