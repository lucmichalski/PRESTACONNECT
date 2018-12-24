using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ARTSTOCKRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistReferenceDepot(String Reference, Int32 Depot)
        {
            if (this.DBSage.F_ARTSTOCK.Count(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.DE_No == Depot) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_ARTSTOCK ReadReferenceDepot(String Reference, Int32 Depot)
        {
            return this.DBSage.F_ARTSTOCK.FirstOrDefault(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.DE_No == Depot);
        }
    }
}
