using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_CONTACTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public P_CONTACT Read(UInt16 Indice)
        {
            return this.DBSage.P_CONTACT.FirstOrDefault(Obj => Obj.cbIndice == Indice);
        }

        public List<P_CONTACT> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_CONTACT> Return = from Table in this.DBSage.P_CONTACT
                                                       where Table.C_Intitule != null && Table.C_Intitule != ""
                                                       select Table;
            return Return.ToList();
        }

        public List<P_CONTACT> ListIntituleNotNullOrderByIntitule()
        {
            System.Linq.IQueryable<P_CONTACT> Return = from Table in this.DBSage.P_CONTACT
                                                       where Table.C_Intitule != null && Table.C_Intitule != ""
                                                       orderby Table.C_Intitule ascending
                                                       select Table;
            return Return.ToList();
        }

    }
}
