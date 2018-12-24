using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_DOSSIERRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean Exist()
        {
            return DBSage.P_DOSSIER.Count() == 1;
        }
        public P_DOSSIER Read()
        {
            return DBSage.P_DOSSIER.FirstOrDefault();
        }
    }
}
