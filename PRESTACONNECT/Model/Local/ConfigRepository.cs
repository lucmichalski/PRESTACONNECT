using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ConfigRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Config Obj)
        {
            this.DBLocal.Config.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Config Obj)
        {
            this.DBLocal.Config.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistName(String Name)
        {
            if (this.DBLocal.Config.Count(Obj => Obj.Con_Name.ToUpper() == Name.ToUpper()) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Config ReadName(String Name)
        {
            return this.DBLocal.Config.FirstOrDefault(Obj => Obj.Con_Name.ToUpper() == Name.ToUpper());
        }

        public List<Config> List()
        {
            System.Linq.IQueryable<Config> Return = from Table in this.DBLocal.Config
                                                     select Table;
            return Return.ToList();
        }
    }
}
