using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductAttributeCombinationRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttributeCombination Obj)
        {
            this.DBPrestashop.PsProductAttributeCombination.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsProductAttributeCombination Obj)
        {
            this.DBPrestashop.PsProductAttributeCombination.DeleteOnSubmit(Obj);
            this.Save();
        }

        public void DeleteAll(List<PsProductAttributeCombination> list)
        {
            this.DBPrestashop.PsProductAttributeCombination.DeleteAllOnSubmit(list);
            this.Save();
        }

        public List<PsProductAttributeCombination> List()
        {
            System.Linq.IQueryable<PsProductAttributeCombination> Return = from Table in this.DBPrestashop.PsProductAttributeCombination
                                                                select Table;
            return Return.ToList();
        }

        public List<PsProductAttributeCombination> ListProductAttribute(UInt32 ProductAttribute)
        {
            System.Linq.IQueryable<PsProductAttributeCombination> Return = from Table in this.DBPrestashop.PsProductAttributeCombination
                                                                           where Table.IDProductAttribute == ProductAttribute
                                                                           select Table;
            return Return.ToList();
        }

        public Boolean ExistAttributeProductAttribute(UInt32 Attribute, UInt32 ProductAttribute)
        {
            if (this.DBPrestashop.PsProductAttributeCombination.Count(Obj => Obj.IDAttribute == Attribute && Obj.IDProductAttribute == ProductAttribute) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsProductAttributeCombination ReadAttributeProductAttribute(UInt32 Attribute, UInt32 ProductAttribute)
        {
            return this.DBPrestashop.PsProductAttributeCombination.FirstOrDefault(Obj => Obj.IDAttribute == Attribute && Obj.IDProductAttribute == ProductAttribute);
        }
    }
}
