using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_PERIODRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_PERIOD> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_PERIOD> Return = from Table in this.DBSage.P_PERIOD
                                                     where Table.P_Period1 != null && Table.P_Period1 != ""
                                                     select Table;
            return Return.ToList();
        }
    }
}
