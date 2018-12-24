using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_REGLECHRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Model.Sage.F_REGLECH Read(Int32 DR_No)
        {
            return this.DBSage.F_REGLECH.Single(re => re.DR_No == DR_No);
        }
    }
}
