using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_REGLEMENTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_REGLEMENT> List()
        {
            IQueryable<P_REGLEMENT> Return = from Table in this.DBSage.P_REGLEMENT
                                             where Table.R_Intitule != ""
                                             select Table;
            return Return.ToList();
        }
    }
}
