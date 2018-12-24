using System;
using System.Linq;

namespace PRESTACONNECT.Model.Sage
{
    public class F_FAMTARIFRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public F_FAMTARIF ReadReferenceCategorie(String CodeFamille, Int32 Categorie)
        {
            return this.DBSage.F_FAMTARIF.FirstOrDefault(Obj => Obj.FA_CodeFamille.ToUpper() == CodeFamille.ToUpper() && Obj.FT_Categorie == Categorie);
        }
    }
}
