using System.Linq;
using System.Collections.Generic;

namespace PRESTACONNECT.Model.Sage
{
    public class F_FAMILLERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public F_FAMILLE Read(F_ARTICLE article)
        {
            return this.DBSage.F_FAMILLE.FirstOrDefault(Obj => Obj.FA_CodeFamille == article.FA_CodeFamille);
        }

        public List<Model.Sage.F_FAMILLE> List()
        {
            return this.DBSage.F_FAMILLE.Where(fa => fa.FA_Type == (short)ABSTRACTION_SAGE.F_FAMILLE.Obj._Enum_FA_Type.Detail).ToList();
        }
    }
}
