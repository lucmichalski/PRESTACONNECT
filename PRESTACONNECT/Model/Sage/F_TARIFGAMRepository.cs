using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_TARIFGAMRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistReferenceCategorieGamme1Gamme2(String Reference, String Categorie, Int32 Gamme1, Int32 Gamme2)
        {
            return this.DBSage.F_TARIFGAM.Count(Obj => Obj.AR_Ref == Reference
                                                    && Obj.TG_RefCF == Categorie
                                                    && Obj.AG_No1 == Gamme1
                                                    && Obj.AG_No2 == Gamme2) > 0;
        }

        public F_TARIFGAM ReadReferenceCategorieGamme1Gamme2(String Reference, String Categorie, Int32 Gamme1, Int32 Gamme2)
        {
            return this.DBSage.F_TARIFGAM.FirstOrDefault(Obj => Obj.AR_Ref == Reference 
                                                            && Obj.TG_RefCF == Categorie 
                                                            && Obj.AG_No1 == Gamme1 
                                                            && Obj.AG_No2 == Gamme2);
        }

        public Boolean ExistReferenceFournisseur(String Reference, String Fournisseur)
        {
            return this.DBSage.F_TARIFGAM.Count(Obj => Obj.AR_Ref == Reference
                                                    && Obj.TG_RefCF == Fournisseur) > 0;
        }

        public Boolean ExistReference(string reference)
        {
            return this.DBSage.F_TARIFGAM.Count(g => g.TG_Ref == reference) > 0;
        }
        public F_TARIFGAM ReadReference(string reference)
        {
            return this.DBSage.F_TARIFGAM.FirstOrDefault(g => g.TG_Ref == reference);
        }
    }
}
