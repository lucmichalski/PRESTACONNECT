using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_CREGLEMENTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public F_CREGLEMENT Read(Int32 RG_No)
        {
            return this.DBSage.F_CREGLEMENT.Single(cr => cr.RG_No == RG_No);
        }

        public void SaveSQL()
        {
            this.DBSage.SubmitChanges();
        }
    }
}
