using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Local
{
    public class Employee_CollaborateurRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Employee_Collaborateur Obj)
        {
            this.DBLocal.Employee_Collaborateur.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Employee_Collaborateur Obj)
        {
            this.DBLocal.Employee_Collaborateur.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteList(List<Employee_Collaborateur> Obj)
        {
            this.DBLocal.Employee_Collaborateur.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public List<Employee_Collaborateur> List()
        {
            return (from Table in this.DBLocal.Employee_Collaborateur
                   select Table).ToList();
        }

        public Boolean ExistCollaborateur(Int32 CO_No)
        {
            return this.DBLocal.Employee_Collaborateur.Count(t => t.Sage_CO_No == CO_No) > 0;
        }
        public Employee_Collaborateur ReadCollaborateur(Int32 CO_No)
        {
            return this.DBLocal.Employee_Collaborateur.FirstOrDefault(Obj => Obj.Sage_CO_No == CO_No);
        }
    }
}
