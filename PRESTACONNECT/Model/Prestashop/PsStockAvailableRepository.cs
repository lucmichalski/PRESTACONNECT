using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsStockAvailableRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsStockAvailable Obj)
        {
            this.DBPrestashop.PsStockAvailable.InsertOnSubmit(Obj);
            this.Save();
        }

        // <JG> 19/03/2013 correction maj stock par boutique
        public Boolean ExistProductAttributeShop(UInt32 Product, UInt32 Attribute, UInt32 Shop)
        {
            if (this.DBPrestashop.PsStockAvailable.Count(Obj => Obj.IDProduct == Product && Obj.IDProductAttribute == Attribute) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // <JG> 19/03/2013 correction maj stock par boutique
        public PsStockAvailable ReadProductAttributeShop(UInt32 Product, UInt32 Attribute, UInt32 Shop)
        {
            return this.DBPrestashop.PsStockAvailable.FirstOrDefault(Obj => Obj.IDProduct == Product && Obj.IDProductAttribute == Attribute);
        }
    }
}
