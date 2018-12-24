using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_DEPOTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public F_DEPOT ReadDepot(Int32 Id)
        {
            return this.DBSage.F_DEPOT.FirstOrDefault(Obj => Obj.DE_No == Id);
        }

        public List<F_DEPOT> ListOrderByIntitule()
        {
            System.Linq.IQueryable<F_DEPOT> Return = from Table in this.DBSage.F_DEPOT
                                                     orderby Table.DE_Intitule ascending
                                                     select Table;
            return Return.ToList();
        }

        public List<F_DEPOT> List()
        {
            return this.DBSage.F_DEPOT.ToList();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBSage.F_DEPOT.Count(Obj => Obj.cbMarq == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_DEPOT ReadId(Int32 Id)
        {
            return this.DBSage.F_DEPOT.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }
    }
}
