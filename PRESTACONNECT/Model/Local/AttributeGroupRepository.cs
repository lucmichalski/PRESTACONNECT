using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class AttributeGroupRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(AttributeGroup Obj)
        {
            this.DBLocal.AttributeGroup.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public Boolean ExistSage(Int32 Sag_Id)
        {
            return this.DBLocal.AttributeGroup.Count(Obj => Obj.Sag_Id == Sag_Id) > 0;
        }

        public AttributeGroup ReadSage(Int32 Sag_Id)
        {
            return this.DBLocal.AttributeGroup.FirstOrDefault(Obj => Obj.Sag_Id == Sag_Id);
        }

        public Boolean ExistPrestashop(Int32 Pre_Id)
        {
            return this.DBLocal.AttributeGroup.Count(Obj => Obj.Pre_Id == Pre_Id) > 0;
        }

        public AttributeGroup ReadPrestashop(Int32 Pre_Id)
        {
            return this.DBLocal.AttributeGroup.FirstOrDefault(Obj => Obj.Pre_Id == Pre_Id);
        }
    }
}
