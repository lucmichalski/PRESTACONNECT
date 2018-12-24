using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_UNITERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_UNITE> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_UNITE> Return = from Table in this.DBSage.P_UNITE
                                                     where Table.U_Intitule != null && Table.U_Intitule != ""
                                                     select Table;
            return Return.ToList();
        }

        // <JG> 12/11/2012 correction filtre type de gamme pour import des gammes depuis Sage
        public List<Int32> ListIdIntituleNotNull()
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBSage.P_UNITE
                                                   where Table.U_Intitule != null && Table.U_Intitule != ""
                                                   select Table.cbMarq;
            return Return.ToList();
        }

        public Boolean ExistUnite(Int32 Id)
        {
            return this.DBSage.P_UNITE.Count(Obj => Obj.cbMarq == Id) > 0;
        }

        public P_UNITE ReadUnite(Int32 Id)
        {
            return this.DBSage.P_UNITE.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }
    }
}
