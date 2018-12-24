using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ECRITURECRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_ECRITUREC> ListTiers(string CT_Num, DateTime debutexo, DateTime finexo)
        {
            return DBSage.F_ECRITUREC.Where(ecr => ecr.CT_Num == CT_Num && ecr.JM_Date >= debutexo && ecr.JM_Date <= finexo).ToList();
        }
    }    
}
