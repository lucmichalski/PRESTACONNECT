using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ConditioningRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Conditioning Obj)
        {
            this.DBLocal.Conditioning.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Conditioning Obj)
        {
            this.DBLocal.Conditioning.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<Conditioning> List()
        {
            return this.DBLocal.Conditioning.AsParallel().ToList();
        }

        public Boolean ExistSag_Id(Int32 Sag_Id)
        {
            return this.DBLocal.Conditioning.Count(Obj => Obj.Sag_Id == Sag_Id) > 0;
        }

        public Boolean ExistPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Conditioning.Count(Obj => Obj.Pre_Id == Pre_Id) > 0;
        }

        public Conditioning ReadSag_Id(Int32 Sag_Id)
        {
            return this.DBLocal.Conditioning.FirstOrDefault(Obj => Obj.Sag_Id == Sag_Id);
        }

        public Conditioning ReadPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Conditioning.FirstOrDefault(Obj => Obj.Pre_Id == Pre_Id);
        }
    }
}
