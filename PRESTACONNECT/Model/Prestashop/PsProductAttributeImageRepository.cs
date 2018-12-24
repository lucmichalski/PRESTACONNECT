using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsProductAttributeImageRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsProductAttributeImage Obj)
        {
            this.DBPrestashop.PsProductAttributeImage.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsProductAttributeImage Obj)
        {
            this.DBPrestashop.PsProductAttributeImage.DeleteOnSubmit(Obj);
            this.Save();
        }

        public void DeleteAll(List<PsProductAttributeImage> Obj)
        {
            this.DBPrestashop.PsProductAttributeImage.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public List<PsProductAttributeImage> List()
        {
            System.Linq.IQueryable<PsProductAttributeImage> Return = from Table in this.DBPrestashop.PsProductAttributeImage
                                                                     select Table;
            return Return.ToList();
        }

        public List<PsProductAttributeImage> ListProductAttribute(UInt32 ProductAttribute)
        {
            System.Linq.IQueryable<PsProductAttributeImage> Return = from Table in this.DBPrestashop.PsProductAttributeImage
                                                                     where Table.IDProductAttribute == ProductAttribute
                                                                     select Table;
            return Return.ToList();
        }

        public List<PsProductAttributeImage> ListImage(uint Image)
        {
            return this.DBPrestashop.PsProductAttributeImage.Where(o => o.IDImage == Image).ToList();
        }

        public Boolean ExistProductAttributeImage(UInt32 ProductAttribute, UInt32 Image)
        {
            if (this.DBPrestashop.PsProductAttributeImage.Count(Obj => Obj.IDProductAttribute == ProductAttribute && Obj.IDImage == Image) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistProductAttribute(UInt32 ProductAttribute)
        {
            return (from Table in this.DBPrestashop.PsProductAttributeImage
                    where Table.IDProductAttribute == ProductAttribute
                    select Table).Count() > 0;
        }
    }
}
