using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_JOURNAUXRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_JOURNAUX> ListTypeSommeil(short Type, short Sommeil)
        {
            IQueryable<F_JOURNAUX> Return = from Table in this.DBSage.F_JOURNAUX
                                            where Table.JO_Type == Type && Table.JO_Sommeil == Sommeil
                                            select Table;
            return Return.ToList();
        }
    }
}
