using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CatalogImageRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(CatalogImage Obj)
        {
            this.DBLocal.CatalogImage.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(CatalogImage Obj)
        {
            Obj.EraseFiles();
            this.DBLocal.CatalogImage.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<CatalogImage> List()
        {
            System.Linq.IQueryable<CatalogImage> Return = from Table in this.DBLocal.CatalogImage
                                                          select Table;
            return Return.ToList();
        }

        public List<CatalogImage> ListCatalog(Int32 Catalog)
        {
            System.Linq.IQueryable<CatalogImage> Return = from Table in this.DBLocal.CatalogImage
                                                          where Table.Cat_Id == Catalog
                                                          select Table;
            return Return.ToList();
        }

        public Boolean ExistCatalogImage(Int32 CatalogImage)
        {
            if (this.DBLocal.CatalogImage.Count(Obj => Obj.ImaCat_Id == CatalogImage) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public CatalogImage ReadCatalogImage(Int32 CatalogImage)
        {
            return this.DBLocal.CatalogImage.FirstOrDefault(Obj => Obj.ImaCat_Id == CatalogImage);
        }

        public CatalogImage ReadCatalogImageByCatalog(Int32 Catalog)
        {
            return this.DBLocal.CatalogImage.FirstOrDefault(Obj => Obj.Cat_Id == Catalog);
        }

        public CatalogImage ReadCatalog(Int32 Catalog, Boolean Default)
        {
            return this.DBLocal.CatalogImage.FirstOrDefault(Obj => Obj.Cat_Id == Catalog);
        }
    }
}
