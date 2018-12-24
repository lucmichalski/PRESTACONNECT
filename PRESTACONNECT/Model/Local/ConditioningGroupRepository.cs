using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ConditioningGroupRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(ConditioningGroup Obj)
        {
            this.DBLocal.ConditioningGroup.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public Boolean ExistSage(Int32 Sag_Id)
        {
            if (this.DBLocal.ConditioningGroup.Count(Obj => Obj.Sag_Id == Sag_Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public ConditioningGroup ReadSage(Int32 Sag_Id)
        {
            return this.DBLocal.ConditioningGroup.FirstOrDefault(Obj => Obj.Sag_Id == Sag_Id);
        }

        public List<ConditioningGroup> List()
        {
            System.Linq.IQueryable<ConditioningGroup> Return = from Table in this.DBLocal.ConditioningGroup
                                                     select Table;
            return Return.ToList();
        }
    }
}
