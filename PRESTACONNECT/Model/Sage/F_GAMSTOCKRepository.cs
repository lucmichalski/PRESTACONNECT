using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_GAMSTOCKRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistReferenceGamme1Gamme2Depot(String Reference, Int32 Gamme1, Int32 Gamme2, Int32 Depot)
        {
            if (this.DBSage.F_GAMSTOCK.Count(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.DE_No == Depot && Obj.AG_No1 == Gamme1 && Obj.AG_No2 == Gamme2) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_GAMSTOCK ReadReferenceGamme1Gamme2Depot(String Reference, Int32 Gamme1, Int32 Gamme2, Int32 Depot)
        {
            return this.DBSage.F_GAMSTOCK.FirstOrDefault(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.DE_No == Depot && Obj.AG_No1 == Gamme1 && Obj.AG_No2 == Gamme2);
        }
    }
}
