using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_SOUCHEVENTERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_SOUCHEVENTE> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_SOUCHEVENTE> Return = from Table in this.DBSage.P_SOUCHEVENTE
                                                      where Table.S_Intitule != null && Table.S_Intitule != ""
                                                      select Table;
            return Return.ToList();
        }
    }
}
