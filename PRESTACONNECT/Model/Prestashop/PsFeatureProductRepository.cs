using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsFeatureProductRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsFeatureProduct Obj)
        {
            this.DBPrestashop.PsFeatureProduct.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsFeatureProduct Obj)
        {
            this.DBPrestashop.PsFeatureProduct.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistFeatureProduct(UInt32 Feature, UInt32 Product)
        {
            if (this.DBPrestashop.PsFeatureProduct.Count(Obj => Obj.IDFeature == Feature && Obj.IDProduct == Product) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsFeatureProduct ReadFeatureProduct(UInt32 Feature, UInt32 Product)
        {
            return this.DBPrestashop.PsFeatureProduct.FirstOrDefault(Obj => Obj.IDFeature == Feature && Obj.IDProduct == Product);
        }
        public List<PsFeatureProduct> List(UInt32 IDProduct)
        {
            System.Linq.IQueryable<PsFeatureProduct> Return = from Table in this.DBPrestashop.PsFeatureProduct
                                                              where Table.IDProduct == IDProduct
                                                              select Table;

            return Return.ToList();
        }
    }
}
