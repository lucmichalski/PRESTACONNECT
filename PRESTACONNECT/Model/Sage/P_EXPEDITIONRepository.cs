using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_EXPEDITIONRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_EXPEDITION> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_EXPEDITION> Return = from Table in this.DBSage.P_EXPEDITION
                                                        where Table.E_Intitule != null && Table.E_Intitule != ""
                                                        select Table;
            return Return.ToList();
        }

        public P_EXPEDITION Read(int cbMarq)
        {
            return DBSage.P_EXPEDITION.FirstOrDefault(e => e.cbMarq == cbMarq);
        }
    }
}
