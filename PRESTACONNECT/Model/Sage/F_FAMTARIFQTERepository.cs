using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Sage
{
    public class F_FAMTARIFQTERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_FAMTARIFQTE> ListReferenceCategorieType(String CodeFamille, String Categorie)
        {
            System.Linq.IQueryable<Model.Sage.F_FAMTARIFQTE> Return = from Table in this.DBSage.F_FAMTARIFQTE
                                                                   where Table.FA_CodeFamille.ToUpper() == CodeFamille && Table.FQ_RefCF.ToUpper() == Categorie.ToUpper()
                                                                   select Table;
            return Return.ToList();
        }
    }
}
