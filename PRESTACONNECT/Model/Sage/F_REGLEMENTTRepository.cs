using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_REGLEMENTTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_REGLEMENTT> ListCLient(String CT_NUM)
        {
            IQueryable<F_REGLEMENTT> Return = from Table in this.DBSage.F_REGLEMENTT
                                              where Table.CT_Num == CT_NUM
                                              select Table;
            return Return.ToList();
        }

        public Boolean Exist(int Id)
        {
            if (this.DBSage.F_REGLEMENTT.Count(Obj => Obj.cbMarq == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_REGLEMENTT Read(Int32 Id)
        {
            return this.DBSage.F_REGLEMENTT.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }


        public List<F_REGLEMENTT> List()
        {
            return this.DBSage.F_REGLEMENTT.ToList();
        }
  
    }
}
