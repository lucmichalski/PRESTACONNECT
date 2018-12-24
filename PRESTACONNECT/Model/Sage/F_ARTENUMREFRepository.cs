using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ARTENUMREFRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public IQueryable<F_ARTENUMREF> List()
        {
            return this.DBSage.F_ARTENUMREF;
        }

        public Boolean ExistReferenceGamme1Gamme2(String Reference, Int32 Gamme1, Int32 Gamme2)
        {
            return this.DBSage.F_ARTENUMREF.Count(g => g.AR_Ref.ToUpper() == Reference.ToUpper() 
                                                    && g.AG_No1 == Gamme1 
                                                    && g.AG_No2 == Gamme2) > 0;
        }

        public F_ARTENUMREF ReadReferenceGamme1Gamme2(String Reference, Int32 Gamme1, Int32 Gamme2)
        {
            return this.DBSage.F_ARTENUMREF.FirstOrDefault(g => g.AR_Ref.ToUpper() == Reference.ToUpper()
                                                    && g.AG_No1 == Gamme1
                                                    && g.AG_No2 == Gamme2);
        }

        public List<F_ARTENUMREF> ListReference(String Reference)
        {
            return this.DBSage.F_ARTENUMREF.Where(g => g.AR_Ref == Reference).ToList();
        }

        public Boolean Exist(Int32 enumref)
        {
            return this.DBSage.F_ARTENUMREF.Count(g => g.cbMarq == enumref) > 0;
        }

        public F_ARTENUMREF Read(Int32 enumref)
        {
            return this.DBSage.F_ARTENUMREF.FirstOrDefault(g => g.cbMarq == enumref);
        }

        public Boolean ExistReference(string reference)
        {
            return this.DBSage.F_ARTENUMREF.Count(g => g.AE_Ref == reference) > 0;
        }
        public F_ARTENUMREF ReadReference(string reference)
        {
            return this.DBSage.F_ARTENUMREF.FirstOrDefault(g => g.AE_Ref == reference);
        }
    }
}
