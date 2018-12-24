using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductLang Obj)
        {
            this.DBPrestashop.PsProductLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistProductLang(Int32 Product, UInt32 Lang, UInt32 IDShop)
        {
            if (this.DBPrestashop.PsProductLang
                .Count(Obj => Obj.IDProduct == Product && Obj.IDLang == Lang && Obj.IDShop == IDShop) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsProductLang ReadProductLang(Int32 Product, UInt32 Lang, UInt32 IDShop)
        {
            return this.DBPrestashop.PsProductLang
                .FirstOrDefault(Obj => Obj.IDProduct == Product && Obj.IDLang == Lang && Obj.IDShop == IDShop);
        }
    }
}
