using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductAttachmentRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttachment Obj)
        {
            this.DBPrestashop.PsProductAttachment.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsProductAttachment Obj)
        {
            this.DBPrestashop.PsProductAttachment.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void Delete(List<PsProductAttachment> Obj)
        {
            this.DBPrestashop.PsProductAttachment.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public List<PsProductAttachment> List()
        {
            System.Linq.IQueryable<PsProductAttachment> Return = from Table in this.DBPrestashop.PsProductAttachment
                                                         select Table;
            return Return.ToList();
        }
        public List<PsProductAttachment> ListAttachment(UInt32 Attachment)
        {
            System.Linq.IQueryable<PsProductAttachment> Return = from Table in this.DBPrestashop.PsProductAttachment
                                                                 where Table.IDAttachment == Attachment
                                                                 select Table;
            return Return.ToList();
        }

        public Boolean ExistProductAttachment(UInt32 Product, UInt32 Attachment)
        {
            if (this.DBPrestashop.PsProductAttachment.Count(Obj => Obj.IDProduct == Product && Obj.IDAttachment == Attachment) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsProductAttachment ReadProductAttachment(UInt32 Product, UInt32 Attachment)
        {
            return this.DBPrestashop.PsProductAttachment.FirstOrDefault(Obj => Obj.IDProduct == Product && Obj.IDAttachment == Attachment);
        }
    }
}
