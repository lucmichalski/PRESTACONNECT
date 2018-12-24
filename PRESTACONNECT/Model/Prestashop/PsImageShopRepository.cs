using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsImageShopRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool Exist(UInt32 IDImage, UInt32 IDShop)
        {
            return (DBPrestashop.PsImageShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDImage == IDImage) != null);
        }

        public List<PsImageShop> List(UInt32 Image)
        {
            return DBPrestashop.PsImageShop.Where(i => i.IDImage == Image).ToList();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistImage(UInt32 Image)
        {
            return this.DBPrestashop.PsImageShop.Count(imgs => imgs.IDImage == Image) > 0;
        }

        public PsImageShop ReadImage(UInt32 Image)
        {
            return this.DBPrestashop.PsImageShop.FirstOrDefault(imgs => imgs.IDImage == Image);
        }

        public void DeleteAll(List<PsImageShop> Obj)
        {
            this.DBPrestashop.PsImageShop.DeleteAllOnSubmit(Obj);
            this.Save();
        }
    }
}
