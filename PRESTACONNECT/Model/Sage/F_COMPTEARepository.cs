using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_COMPTEARepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_COMPTEA> List()
        {
            return this.DBSage.F_COMPTEA.ToList();
        }

        public List<F_COMPTEA> ListSommeil(short Sommeil)
        {
            System.Linq.IQueryable<F_COMPTEA> Return = from Table in this.DBSage.F_COMPTEA
                                                       where Table.CA_Sommeil == Sommeil
                                                       select Table;
            return Return.ToList();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBSage.F_COMPTEA.Count(Obj => Obj.cbMarq == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_COMPTEA ReadId(Int32 Id)
        {
            return this.DBSage.F_COMPTEA.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }
    }
}
