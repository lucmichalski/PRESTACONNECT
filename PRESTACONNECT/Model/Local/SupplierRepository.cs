using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class SupplierRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Supplier Obj)
        {
            this.DBLocal.Supplier.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Supplier Obj)
        {
            this.DBLocal.Supplier.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBLocal.Supplier.Count(Obj => Obj.Sup_Id == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Supplier ReadId(Int32 Id)
        {
            return this.DBLocal.Supplier.FirstOrDefault(Obj => Obj.Sup_Id == Id);
        }

        public Boolean ExistSag_Id(Int32 Sag_Id)
        {
            if (this.DBLocal.Supplier.Count(Obj => Obj.Sag_Id == Sag_Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Supplier ReadSag_Id(Int32 Sag_Id)
        {
            return this.DBLocal.Supplier.FirstOrDefault(Obj => Obj.Sag_Id == Sag_Id);
        }

        public List<Supplier> List()
        {
            System.Linq.IQueryable<Supplier> Return = from Table in this.DBLocal.Supplier
                                                     select Table;
            return Return.ToList();
        }

        public List<Supplier> ListSync(Boolean Sync)
        {
            System.Linq.IQueryable<Supplier> Return = from Table in this.DBLocal.Supplier
                                                      where Table.Sup_Sync == Sync
                                                     select Table;
            return Return.ToList();
        }

        public List<Int32> ListIdSync(Boolean Sync)
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBLocal.Supplier
                                                      where Table.Sup_Sync == Sync
                                                      select Table.Sup_Id;
            return Return.ToList();
        }
    }
}
