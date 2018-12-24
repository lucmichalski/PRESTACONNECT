using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Local
{
    public class Group_CRisqueRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Group_CRisque Obj)
        {
            this.DBLocal.Group_CRisque.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Group_CRisque Obj)
        {
            this.DBLocal.Group_CRisque.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteList(List<Group_CRisque> Obj)
        {
            this.DBLocal.Group_CRisque.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public List<Group_CRisque> List()
        {
            return (from Table in this.DBLocal.Group_CRisque
                   select Table).ToList();
        }

        public Boolean ExistCRisque(Int32 N_Risque)
        {
            return this.DBLocal.Group_CRisque.Count(t => t.Sag_CRisque == N_Risque) > 0;
        }
        public Group_CRisque ReadCRisque(Int32 N_Risque)
        {
            return this.DBLocal.Group_CRisque.FirstOrDefault(Obj => Obj.Sag_CRisque == N_Risque);
        }
    }
}
