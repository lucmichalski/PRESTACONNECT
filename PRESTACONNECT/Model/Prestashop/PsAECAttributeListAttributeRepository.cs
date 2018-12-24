using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAECAttributeListAttributeRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Add(PsAEcAttributeListAttribute Obj)
        {
            this.DBPrestashop.PsAEcAttributeListAttribute.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean Exist(UInt32 id)
        {
            return this.DBPrestashop.PsAEcAttributeListAttribute.Count(s => s.IDAEcAttributeListAttribute == id) > 0;
        }

        public PsAEcAttributeListAttribute Read(UInt32 id)
        {
            return this.DBPrestashop.PsAEcAttributeListAttribute.FirstOrDefault(s => s.IDAEcAttributeListAttribute == id);
        }

        public Boolean ExistProduct(UInt32 product)
        {
            return this.DBPrestashop.PsAEcAttributeListAttribute.Count(s => s.IDProduct == product) > 0;
        }

        public IQueryable<PsAEcAttributeListAttribute> ListProduct(UInt32 product)
        {
            return this.DBPrestashop.PsAEcAttributeListAttribute.Where(s => s.IDProduct == product);
        }

        public Boolean ExistProductAttribute(UInt32 productattribute)
        {
            return this.DBPrestashop.PsAEcAttributeListAttribute.Count(s => s.IDProductAttribute == productattribute) > 0;
        }

        public PsAEcAttributeListAttribute ReadProductAttribute(UInt32 productattribute)
        {
            return this.DBPrestashop.PsAEcAttributeListAttribute.FirstOrDefault(s => s.IDProductAttribute == productattribute);
        }
    }
}
