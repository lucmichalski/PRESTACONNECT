using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Sage
{
    public class P_PARAMETRECIALRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();
        
        public Boolean Exist()
        {
            return this.DBSage.P_PARAMETRECIAL.Count() == 1;
        }

        public P_PARAMETRECIAL Read()
        {
            return this.DBSage.P_PARAMETRECIAL.FirstOrDefault();
        }
    }
}
