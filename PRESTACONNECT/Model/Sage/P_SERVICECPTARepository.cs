using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_SERVICECPTARepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public P_SERVICECPTA Read(UInt16 Indice)
        {
            return this.DBSage.P_SERVICECPTA.FirstOrDefault(Obj => Obj.cbIndice == Indice);
        }

        public List<P_SERVICECPTA> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_SERVICECPTA> Return = from Table in this.DBSage.P_SERVICECPTA
                                                           where Table.S_Intitule != null && Table.S_Intitule != ""
                                                           select Table;
            return Return.ToList();
        }

        public List<P_SERVICECPTA> ListIntituleNotNullOrderByIntitule()
        {
            System.Linq.IQueryable<P_SERVICECPTA> Return = from Table in this.DBSage.P_SERVICECPTA
                                                           where Table.S_Intitule != null && Table.S_Intitule != ""
                                                           orderby Table.S_Intitule ascending
                                                           select Table;
            return Return.ToList();
        }
    }
}
