using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_DEVISERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_DEVISE> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_DEVISE> Return = from Table in this.DBSage.P_DEVISE
                                                       where Table.D_Intitule != null && Table.D_Intitule != ""
                                                       select Table;
            return Return.ToList();
        }
    }
}
