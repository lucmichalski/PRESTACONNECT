using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCategoryLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCategoryLang Obj)
        {
            this.DBPrestashop.PsCategoryLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public List<PsCategoryLang> ListCategory(Int32 Category)
        {
            System.Linq.IQueryable<PsCategoryLang> Return = from Table in this.DBPrestashop.PsCategoryLang
                                                             where Table.IDCategory == Category
                                                             select Table;
            return Return.ToList();
        }
		
        public Boolean ExistCategoryLang(Int32 Category, UInt32 Lang, UInt32 IDShop)
        {
            if (this.DBPrestashop.PsCategoryLang.Count(Obj => Obj.IDCategory == Category && Obj.IDLang == Lang && Obj.IDShop == IDShop) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsCategoryLang ReadCategoryLang(Int32 Category, UInt32 Lang, UInt32 IDShop)
        {
            return this.DBPrestashop.PsCategoryLang.FirstOrDefault(Obj => Obj.IDCategory == Category && Obj.IDLang == Lang && Obj.IDShop == IDShop);
        }
    }
}
