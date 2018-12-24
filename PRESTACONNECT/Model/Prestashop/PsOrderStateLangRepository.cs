using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrderStateLangRepository
    {

        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsOrderStateLang Obj)
        {
            this.DBPrestashop.PsOrderStateLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsOrderStateLang Obj)
        {
            this.DBPrestashop.PsOrderStateLang.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsOrderStateLang> ListLang(UInt32 Lang)
        {
            System.Linq.IQueryable<PsOrderStateLang> Return = from Table in this.DBPrestashop.PsOrderStateLang
                                                              where Table.IDLang == Lang
                                                              select Table;
            return Return.ToList();
        }

        public Boolean ExistOrderStateLang(UInt32 OrderState, UInt32 Lang)
        {
            if (this.DBPrestashop.PsOrderStateLang.Count(Obj => Obj.IDOrderState == OrderState && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsOrderStateLang ReadOrderStateLang(UInt32 OrderState, UInt32 Lang)
        {
            return this.DBPrestashop.PsOrderStateLang.FirstOrDefault(Obj => Obj.IDOrderState == OrderState && Obj.IDLang == Lang);
        }
        
    }
}
