using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Sage
{
    public class P_DOSSIERCIALRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean Exist()
        {
            return this.DBSage.P_DOSSIERCIAL.Count() == 1;
        }

        public P_DOSSIERCIAL Read()
        {
            return this.DBSage.P_DOSSIERCIAL.FirstOrDefault();
        }
    }
}
