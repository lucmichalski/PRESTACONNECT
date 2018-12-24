using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ARTGAMMERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_ARTGAMME> List()
        {
            return this.DBSage.F_ARTGAMME.ToList();
        }

        public List<F_ARTGAMME> ListArticle (String Article)
        {
            System.Linq.IQueryable<Model.Sage.F_ARTGAMME> Return = from Table in this.DBSage.F_ARTGAMME
                                                                   where Table.AR_Ref.ToUpper() == Article.ToUpper()
                                                                   select Table;
            return Return.ToList();
        }

        public List<F_ARTGAMME> ListArticleType(String Article, short Type)
        {
            System.Linq.IQueryable<Model.Sage.F_ARTGAMME> Return = from Table in this.DBSage.F_ARTGAMME
                                                                   where Table.AR_Ref.ToUpper() == Article.ToUpper() && Table.AG_Type == Type
                                                                   select Table;
            return Return.ToList();
        }

        public Boolean ExistId(Int32 cbMarq)
        {
            return this.DBSage.F_ARTGAMME.Count(Obj => Obj.cbMarq == cbMarq) >0;
        }
        public F_ARTGAMME ReadId(Int32 cbMarq)
        {
            return this.DBSage.F_ARTGAMME.FirstOrDefault(Obj => Obj.cbMarq == cbMarq);
        }

        public Boolean Exist(int AG_No, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type AG_Type)
        {
            return this.DBSage.F_ARTGAMME.Count(g => g.AG_No == AG_No && g.AG_Type == (short)AG_Type) > 0;
        }
        public F_ARTGAMME Read(int AG_No, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type AG_Type)
        {
            return this.DBSage.F_ARTGAMME.FirstOrDefault(g => g.AG_No == AG_No && g.AG_Type == (short)AG_Type);
        }
    }
}
