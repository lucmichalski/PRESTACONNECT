using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsImageLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsImageLang Obj)
        {
            this.DBPrestashop.PsImageLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsImageLang Obj)
        {
            this.DBPrestashop.PsImageLang.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteAll(List<PsImageLang> Obj)
        {
            this.DBPrestashop.PsImageLang.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public List<PsImageLang> List()
        {
            System.Linq.IQueryable<PsImageLang> Return = from Table in this.DBPrestashop.PsImageLang
                                                            select Table;
            return Return.ToList();
        }

        public List<PsImageLang> ListImage(UInt32 Image)
        {
            System.Linq.IQueryable<PsImageLang> Return = from Table in this.DBPrestashop.PsImageLang
                                                         where Table.IDImage == Image
                                                         select Table;
            return Return.ToList();
        }

        public Boolean ExistImageLang(UInt32 Image, UInt32 Lang)
        {
            if (this.DBPrestashop.PsImageLang.Count(Obj => Obj.IDImage == Image && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsImageLang ReadImageLang(UInt32 Image, UInt32 Lang)
        {
            return this.DBPrestashop.PsImageLang.FirstOrDefault(Obj => Obj.IDImage == Image && Obj.IDLang == Lang);
        }
    }
}
