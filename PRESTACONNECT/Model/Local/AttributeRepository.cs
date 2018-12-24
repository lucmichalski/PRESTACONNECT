using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class AttributeRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Attribute Obj)
        {
            this.DBLocal.Attribute.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Attribute Obj)
        {
            this.DBLocal.Attribute.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistSag_Id(Int32 Sag_Id)
        {
            return this.DBLocal.Attribute.Count(Obj => Obj.Sag_Id == Sag_Id) > 0;
        }

        public Boolean ExistPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Attribute.Count(Obj => Obj.Pre_Id == Pre_Id) > 0;
        }

        public Attribute ReadSag_Id(Int32 Sag_Id)
        {
            return this.DBLocal.Attribute.FirstOrDefault(Obj => Obj.Sag_Id == Sag_Id);
        }

        public List<Attribute> ListPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Attribute.Where(Obj => Obj.Pre_Id == Pre_Id).ToList();
        }

        public List<Attribute> List()
        {
            return this.DBLocal.Attribute.ToList();
        }
    }
}
