using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ARTFOURNISSRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistArticle(String Article)
        {
            return this.DBSage.F_ARTFOURNISS.Count(Obj => Obj.AR_Ref == Article) > 0;
        }

        public Boolean ExistArticlePrincipal(String Article)
        {
            return this.DBSage.F_ARTFOURNISS.Count(Obj => Obj.AR_Ref == Article && Obj.AF_Principal == 1) == 1;
        }

        public F_ARTFOURNISS ReadArticlePrincipal(String Article)
        {
            return this.DBSage.F_ARTFOURNISS.FirstOrDefault(Obj => Obj.AR_Ref == Article && Obj.AF_Principal == 1);
        }

        public Boolean ExistReference(String reference)
        {
            return this.DBSage.F_ARTFOURNISS.Count(Obj => Obj.AF_RefFourniss == reference) == 1;
        }

        public F_ARTFOURNISS ReadReference(String reference)
        {
            return this.DBSage.F_ARTFOURNISS.FirstOrDefault(Obj => Obj.AF_RefFourniss == reference);
        }
    }
}
