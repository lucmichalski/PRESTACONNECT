using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_NOMENCLATRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_NOMENCLAT> ListRef(String Ref)
        {
            IQueryable<F_NOMENCLAT> Return = from Table in this.DBSage.F_NOMENCLAT
                                             where Table.AR_Ref.ToUpper() == Ref.ToUpper()
                                             select Table;
            return Return.ToList();
        }
    }
}
