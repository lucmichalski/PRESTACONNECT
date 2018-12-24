using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ARTICLEMEDIARepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistReference(String Reference)
        {
            return this.DBSage.F_ARTICLEMEDIA.Count(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper()) > 0;
        }

        public List<F_ARTICLEMEDIA> ListReference(String Reference)
        {
            return this.DBSage.F_ARTICLEMEDIA.Where(a => a.AR_Ref == Reference).ToList();
        }

        public Boolean Exist(Int32 Media)
        {
            return this.DBSage.F_ARTICLEMEDIA.Count(m => m.cbMarq == Media) > 0;
        }
    }
}
