using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    class F_ENUMSTATARTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistEnumere(String Value, Int16 Champ)
        {
            return this.DBSage.F_ENUMSTATART.Count(obj => obj.SA_Champ == Champ && obj.SA_Enumere == Value) > 0;
        }

        public F_ENUMSTATART ReadEnumere(String Value, Int16 Champ)
        {
            return this.DBSage.F_ENUMSTATART.First(obj => obj.SA_Champ == Champ && obj.SA_Enumere == Value);
        }
    }
}
