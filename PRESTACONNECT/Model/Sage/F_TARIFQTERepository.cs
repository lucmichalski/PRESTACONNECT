using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_TARIFQTERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_TARIFQTE> ListReferenceCategorieType(String Reference, String Categorie, short Type)
        {
            System.Linq.IQueryable<Model.Sage.F_TARIFQTE> Return = from Table in this.DBSage.F_TARIFQTE
                                                                   where Table.AR_Ref.ToUpper() == Reference 
                                                                   && Table.TQ_RefCF.ToUpper() == Categorie.ToUpper() 
                                                                   && Table.TQ_Remise01REM_Type == Type
                                                                   select Table;
            return Return.ToList();
        }

        public List<F_TARIFQTE> ListReferenceCategorie(String Reference, String Categorie)
        {
            System.Linq.IQueryable<Model.Sage.F_TARIFQTE> Return = from Table in this.DBSage.F_TARIFQTE
                                                                   where Table.AR_Ref.ToUpper() == Reference 
                                                                   && Table.TQ_RefCF.ToUpper() == Categorie.ToUpper()
                                                                   select Table;
            return Return.ToList();
        }
    }
}
