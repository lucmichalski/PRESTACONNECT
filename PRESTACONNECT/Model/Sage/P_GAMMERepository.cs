using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_GAMMERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_GAMME> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_GAMME> Return = from Table in this.DBSage.P_GAMME
                                                     where Table.G_Intitule != null && Table.G_Intitule != "" && Table.G_Type == 0
                                                     select Table;
            return Return.ToList();
        }

        // <JG> 12/11/2012 correction filtre type de gamme pour import des gammes depuis Sage
        public List<Int32> ListIdIntituleNotNull()
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBSage.P_GAMME
                                                   where Table.G_Intitule != null && Table.G_Intitule != "" && Table.G_Type == 0
                                                   select Table.cbMarq;
            return Return.ToList();
        }

        public P_GAMME ReadGamme(Int32 Id)
        {
            return this.DBSage.P_GAMME.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }
    }
}
