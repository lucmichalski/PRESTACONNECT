using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    class F_ENUMSTATRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistEnumere(String Value, Int16 Champ)
        {
            return this.DBSage.F_ENUMSTAT.Count(obj => obj.N_Statistique == Champ && obj.ES_Intitule == Value) > 0;
        }

        public F_ENUMSTAT ReadEnumere(String Value, Int16 Champ)
        {
            return this.DBSage.F_ENUMSTAT.First(obj => obj.N_Statistique == Champ && obj.ES_Intitule == Value);
        }
    }
}
