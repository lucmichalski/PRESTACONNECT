using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsManufacturerRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool ExistInShop(UInt32 IDManufacturer, UInt32 IDShop)
        {
            return (DBPrestashop.PsManufacturerShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDManufacturer == IDManufacturer) != null);
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsManufacturer Obj, UInt32 IDShop)
        {
            this.DBPrestashop.PsManufacturer.InsertOnSubmit(Obj);
            this.Save();

            //Si le fournisseur n'existe pas dans la boutique, il est rajouté.
            if (!ExistInShop(Obj.IDManufacturer, IDShop))
            {
                DBPrestashop.PsManufacturerShop.InsertOnSubmit(new PsManufacturerShop()
                {
                    IDManufacturer = Obj.IDManufacturer,
                    IDShop = IDShop,
                });
                DBPrestashop.SubmitChanges();
            }
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBPrestashop.PsManufacturer.Count(Obj => Obj.IDManufacturer == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsManufacturer ReadId(Int32 Id)
        {
            return this.DBPrestashop.PsManufacturer.FirstOrDefault(Obj => Obj.IDManufacturer == Id);
        }

        public bool Exist(string Name)
        {
            return this.DBPrestashop.PsManufacturer.Count(s => s.Name == Name) > 0;
        }

        public PsManufacturer Read(string Name)
        {
            return this.DBPrestashop.PsManufacturer.FirstOrDefault(s => s.Name == Name);
        }
    }
}
