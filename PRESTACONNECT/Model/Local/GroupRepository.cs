using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Local
{
    public class GroupRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Group Obj)
        {
            this.DBLocal.Group.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Group Obj)
        {
            this.DBLocal.Group.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean Exist(Int32 PreId)
        {
            return !(this.DBLocal.Group.Count(Obj => Obj.Grp_Pre_Id == PreId) == 0);
        }
        public Group Read(Int32 PreId)
        {
            return this.DBLocal.Group.FirstOrDefault(Obj => Obj.Grp_Pre_Id == PreId);
        }
        
        public List<Group> List()
        {
            System.Linq.IQueryable<Group> Return = from Table in this.DBLocal.Group
                                                   select Table;
            return Return.ToList();
        }

        public List<Group> ListGroupesLies()
        {
            System.Linq.IQueryable<Group> Return = from Table in this.DBLocal.Group
                                                   where Table.Grp_CatTarifId != null && Table.Grp_Pre_Id > 2
                                                   select Table;
            return Return.ToList();
        }

        public List<Int32?> ListCatTarifSage()
        {
            System.Linq.IQueryable<Int32?> Return = from Table in this.DBLocal.Group
                                                   where Table.Grp_CatTarifId != null && Table.Grp_Pre_Id > 2
                                                   select Table.Grp_CatTarifId;
            return Return.ToList(); 
        }

        public List<int> ListPrestaShop()
        {
            return (from Table in this.DBLocal.Group
                    where Table.Grp_Pre_Id > 0 && Table.Grp_CatTarifId != null
                    select Table.Grp_Pre_Id).Distinct().ToList();
        }

        public Boolean CatTarifSageMonoGroupe(int N_CatTarif)
        {
            return this.DBLocal.Group.Count(t => t.Grp_CatTarifId == N_CatTarif && t.Grp_Pre_Id > 2) == 1;
        }

        public int SearchIdGroupCatTarifSage(int N_CatTarif)
        {
            return this.DBLocal.Group.First(t => t.Grp_CatTarifId == N_CatTarif && t.Grp_Pre_Id > 2).Grp_Pre_Id;
        }
        
        public Boolean ExistCatTarif(int N_CatTarif)
        {
            return this.DBLocal.Group.Count(t => t.Grp_CatTarifId == N_CatTarif) > 0;
        }
    }
}
