using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsSupplierRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool ExistInShop(UInt32 IDSupplier, UInt32 IDShop)
        {
            return (DBPrestashop.PsSupplierShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDSupplier == IDSupplier) != null);
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsSupplier Obj, UInt32 IDShop)
        {
            this.DBPrestashop.PsSupplier.InsertOnSubmit(Obj);
            this.Save();

            //Si le fournisseur n'existe pas dans la boutique, il est rajouté.
            if (!ExistInShop(Obj.IDSupplier, IDShop))
            {
                DBPrestashop.PsSupplierShop.InsertOnSubmit(new PsSupplierShop()
                {
                    IDSupplier = Obj.IDSupplier,
                    IDShop = IDShop,
                });
                DBPrestashop.SubmitChanges();
            }
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBPrestashop.PsSupplier.Count(Obj => Obj.IDSupplier == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsSupplier ReadId(Int32 Id)
        {
            return this.DBPrestashop.PsSupplier.FirstOrDefault(Obj => Obj.IDSupplier == Id);
        }

        public bool Exist(string Name)
        {
            return this.DBPrestashop.PsSupplier.Count(s => s.Name == Name) > 0;
        }

        public PsSupplier Read(string Name)
        {
            return this.DBPrestashop.PsSupplier.FirstOrDefault(s => s.Name == Name);
        }
    }
}
