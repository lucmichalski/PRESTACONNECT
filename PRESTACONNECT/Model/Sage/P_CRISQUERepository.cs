using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_CRISQUERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_CRISQUE> ListIntituleNotNull()
        {
            System.Linq.IQueryable<P_CRISQUE> Return = from Table in this.DBSage.P_CRISQUE
                                                       where Table.R_Intitule != null && Table.R_Intitule != ""
                                                       select Table;
            return Return.ToList();
        }

        public Boolean ExistCRisque(short cbIndice)
        {
            return this.DBSage.P_CRISQUE.Count(Obj => Obj.cbIndice == cbIndice) == 1;
        }

        public P_CRISQUE ReadCRisque(short cbIndice)
        {
            return this.DBSage.P_CRISQUE.SingleOrDefault(Obj => Obj.cbIndice == cbIndice);
        }
    }
}
