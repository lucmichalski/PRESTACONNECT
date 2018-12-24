using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class SupplyRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Supply Obj)
        {
            this.DBLocal.Supply.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Supply Obj)
        {
            this.DBLocal.Supply.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistSage(Int32 Id)
        {
            if (this.DBLocal.Supply.Count(Obj => Obj.Sag_Id == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Supply ReadSage(Int32 Id)
        {
            return this.DBLocal.Supply.FirstOrDefault(Obj => Obj.Sag_Id == Id);
        }

        public Supply ReadName(String Name)
        {
            return this.DBLocal.Supply.FirstOrDefault(Obj => Obj.Sup_Name == Name);
        }

        public List<Supply> List()
        {
            System.Linq.IQueryable<Supply> Return = from Table in this.DBLocal.Supply
                                                     select Table;
            return Return.ToList();
        }

        public List<Supply> ListOrderByName()
        {
            System.Linq.IQueryable<Supply> Return = from Table in this.DBLocal.Supply
                                                     orderby Table.Sup_Name ascending
                                                     select Table;
            return Return.ToList();
        }

        public List<Supply> ListActive(Boolean Active)
        {
            System.Linq.IQueryable<Supply> Return = from Table in this.DBLocal.Supply
                                                    where Table.Sup_Active == Active
                                                    select Table;
            return Return.ToList();
        }
    }
}
