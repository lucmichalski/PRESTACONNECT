using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_CONDITIONNEMENTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_CONDITIONNEMENT> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_CONDITIONNEMENT> Return = from Table in this.DBSage.P_CONDITIONNEMENT
                                                               where Table.P_Conditionnement != null && Table.P_Conditionnement != ""
                                                               select Table;
            return Return.ToList();
        }

        // <JG> 12/11/2012 correction filtre type de gamme pour import des gammes depuis Sage
        public List<Int32> ListIdIntituleNotNull()
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBSage.P_CONDITIONNEMENT
                                                   where Table.P_Conditionnement != null && Table.P_Conditionnement != ""
                                                   select Table.cbMarq;
            return Return.ToList();
        }
        
        public Boolean ExistConditionnement(Int32 Id)
        {
            return this.DBSage.P_CONDITIONNEMENT.Count(Obj => Obj.P_Conditionnement != null && Obj.cbMarq == Id) > 0;
        }
        public P_CONDITIONNEMENT ReadConditionnement(Int32 Id)
        {
            return this.DBSage.P_CONDITIONNEMENT.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }
    }
}
