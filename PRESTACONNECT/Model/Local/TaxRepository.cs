using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class TaxRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Tax Obj)
        {
            this.DBLocal.Tax.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Tax Obj)
        {
            this.DBLocal.Tax.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistSage(Int32 Id)
        {
            if (this.DBLocal.Tax.Count(Obj => Obj.Sag_Id == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistPrestashop(Int32 Id)
        {
            if (this.DBLocal.Tax.Count(Obj => Obj.Pre_Id == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistSagePrestashop(Int32 Sage, Int32 Prestashop)
        {
            if (this.DBLocal.Tax.Count(Obj => Obj.Sag_Id == Sage && Obj.Pre_Id == Prestashop) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Tax ReadSage(Int32 Id)
        {
            return this.DBLocal.Tax.FirstOrDefault(Obj => Obj.Sag_Id == Id);
        }

        public Tax ReadPrestashop(Int32 Id)
        {
            return this.DBLocal.Tax.FirstOrDefault(Obj => Obj.Pre_Id == Id);
        }

        public Tax ReadSagePrestashop(Int32 Sage, Int32 Prestashop)
        {
            return this.DBLocal.Tax.FirstOrDefault(Obj => Obj.Sag_Id == Sage && Obj.Pre_Id == Prestashop);
        }

        public List<Tax> List()
        {
            System.Linq.IQueryable<Tax> Return = from Table in this.DBLocal.Tax
                                                    select Table;
            return Return.ToList();
        }

        public List<Tax> ListOrderBySagName()
        {
            System.Linq.IQueryable<Tax> Return = from Table in this.DBLocal.Tax
                                                    orderby Table.Sag_Name ascending
                                                    select Table;
            return Return.ToList();
        }
    }
}
