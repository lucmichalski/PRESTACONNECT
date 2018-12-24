using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_CONDITIONRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_CONDITION> ListArticle(String Article)
        {
            System.Linq.IQueryable<Model.Sage.F_CONDITION> Return = from Table in this.DBSage.F_CONDITION
                                                                    where Table.AR_Ref.ToUpper() == Article.ToUpper()
                                                                    select Table;
            return Return.ToList();
        }

        public List<F_CONDITION> List()
        {
            return this.DBSage.F_CONDITION.ToList();
        }

        public Boolean ExistId(Int32 cbMarq)
        {
            if (this.DBSage.F_CONDITION.Count(Obj => Obj.cbMarq == cbMarq) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_CONDITION ReadId(Int32 cbMarq)
        {
            return this.DBSage.F_CONDITION.FirstOrDefault(Obj => Obj.cbMarq == cbMarq);
        }

        public Boolean ExistReferenceConditionnement(string Conditionnement)
        {
            return this.DBSage.F_CONDITION.Count(c => c.CO_Ref == Conditionnement) == 1;
        }

        public F_CONDITION ReadReferenceConditionnement(string Conditionnement)
        {
            return this.DBSage.F_CONDITION.FirstOrDefault(c => c.CO_Ref == Conditionnement);
        }

        public Boolean ExistEnumereConditionnement(string Conditionnement)
        {
            return this.DBSage.F_CONDITION.Count(c => c.EC_Enumere == Conditionnement) == 1;
        }

        public F_CONDITION ReadEnumereConditionnement(string Conditionnement)
        {
            return this.DBSage.F_CONDITION.FirstOrDefault(c => c.EC_Enumere == Conditionnement);
        }
    }
}
