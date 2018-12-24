using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Sage
{
    public class P_CATTARIFRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();
                
        public P_CATTARIF Read(UInt16 Indice)
        {
            return this.DBSage.P_CATTARIF.FirstOrDefault(Obj => Obj.cbIndice == Indice);
        }

        public List<P_CATTARIF> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_CATTARIF> Return = from Table in this.DBSage.P_CATTARIF
                                                        where Table.CT_Intitule != null && Table.CT_Intitule != ""
                                                        select Table;
            return Return.ToList();
        }

        public List<P_CATTARIF> ListIntituleNotNullOrderByIntitule()
        {
            System.Linq.IQueryable<P_CATTARIF> Return = from Table in this.DBSage.P_CATTARIF
                                                        where Table.CT_Intitule != null && Table.CT_Intitule != ""
                                                        orderby Table.CT_Intitule ascending
                                                        select Table;
            return Return.ToList();
        }
    }
}
