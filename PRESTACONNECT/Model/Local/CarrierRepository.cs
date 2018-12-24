using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CarrierRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Carrier Obj)
        {
            this.DBLocal.Carrier.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Carrier Obj)
        {
            this.DBLocal.Carrier.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistSage(Int32 Id)
        {
            if (this.DBLocal.Carrier.Count(Obj => Obj.Sag_Id == Id) == 0)
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
            if (this.DBLocal.Carrier.Count(Obj => Obj.Pre_Id == Id) == 0)
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
            if (this.DBLocal.Carrier.Count(Obj => Obj.Sag_Id == Sage && Obj.Pre_Id == Prestashop) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Carrier ReadSage(Int32 Id)
        {
            return this.DBLocal.Carrier.FirstOrDefault(Obj => Obj.Sag_Id == Id);
        }

        public Carrier ReadPrestashop(Int32 Id)
        {
            return this.DBLocal.Carrier.FirstOrDefault(Obj => Obj.Pre_Id == Id);
        }

        public Carrier ReadSagePrestashop(Int32 Sage, Int32 Prestashop)
        {
            return this.DBLocal.Carrier.FirstOrDefault(Obj => Obj.Sag_Id == Sage && Obj.Pre_Id == Prestashop);
        }

        public List<Carrier> List()
        {
            System.Linq.IQueryable<Carrier> Return = from Table in this.DBLocal.Carrier
                                                 select Table;
            return Return.ToList();
        }

        public List<Carrier> ListOrderBySagName()
        {
            System.Linq.IQueryable<Carrier> Return = from Table in this.DBLocal.Carrier
                                                 orderby Table.Sag_Name ascending
                                                 select Table;
            return Return.ToList();
        }
    }
}
