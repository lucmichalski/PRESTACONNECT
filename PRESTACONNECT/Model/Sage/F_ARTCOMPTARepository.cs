using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ARTCOMPTARepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistReferenceChampType(String Reference, Int32 Champ, short Type)
        {
            if (this.DBSage.F_ARTCOMPTA.Count(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.ACP_Champ == Champ && Obj.ACP_Type == Type) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_ARTCOMPTA ReadReferenceChampType(String Reference, Int32 Champ, short Type)
        {
            return this.DBSage.F_ARTCOMPTA.FirstOrDefault(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.ACP_Champ == Champ && Obj.ACP_Type == Type);
        }
    }
}
