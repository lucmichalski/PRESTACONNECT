using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_CONDLIVRRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_CONDLIVR> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_CONDLIVR> Return = from Table in this.DBSage.P_CONDLIVR
                                                        where Table.C_Intitule != null && Table.C_Intitule != ""
                                                        select Table;
            return Return.ToList();
        }
    }
}
