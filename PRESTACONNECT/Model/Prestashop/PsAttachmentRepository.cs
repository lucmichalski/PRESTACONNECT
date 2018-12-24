using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAttachmentRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsAttachment Obj)
        {
            this.DBPrestashop.PsAttachment.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsAttachment Obj)
        {
            this.DBPrestashop.PsAttachment.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsAttachment> List()
        {
            System.Linq.IQueryable<PsAttachment> Return = from Table in this.DBPrestashop.PsAttachment
                                                     select Table;
            return Return.ToList();
        }

        public Boolean ExistAttachment(UInt32 Attachment)
        {
            if (this.DBPrestashop.PsAttachment.Count(Obj => Obj.IDAttachment == Attachment) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsAttachment ReadAttachment(UInt32 Attachment)
        {
            return this.DBPrestashop.PsAttachment.FirstOrDefault(Obj => Obj.IDAttachment == Attachment);
        }

        public Boolean ExistFile(String FileName)
        {
            return (from Table in this.DBPrestashop.PsAttachment
                    where Table.File == FileName
                    select Table).Count() > 0;
        }
    }
}
