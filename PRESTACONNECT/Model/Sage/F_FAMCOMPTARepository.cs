using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_FAMCOMPTARepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistFamilleChampType(String Famille, Int32 Champ, short Type)
        {
            if (this.DBSage.F_FAMCOMPTA.Count(Obj => Obj.FA_CodeFamille.ToUpper() == Famille.ToUpper() && Obj.FCP_Champ == Champ && Obj.FCP_Type == Type) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_FAMCOMPTA ReadFamilleChampType(String Famille, Int32 Champ, short Type)
        {
            return this.DBSage.F_FAMCOMPTA.FirstOrDefault(Obj => Obj.FA_CodeFamille.ToUpper() == Famille.ToUpper() && Obj.FCP_Champ == Champ && Obj.FCP_Type == Type);
        }
    }
}
