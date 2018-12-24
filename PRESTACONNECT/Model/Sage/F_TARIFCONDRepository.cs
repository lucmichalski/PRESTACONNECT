using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_TARIFCONDRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistReferenceCategorieConditionnement(String Reference, String Categorie, Int32 Conditionnement)
        {
            if (this.DBSage.F_TARIFCOND.Count(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.TC_RefCF.ToUpper() == Categorie.ToUpper() && Obj.CO_No == Conditionnement) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_TARIFCOND ReadReferenceCategorieConditionnement(String Reference, String Categorie, Int32 Conditionnement)
        {
            return this.DBSage.F_TARIFCOND.FirstOrDefault(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.TC_RefCF.ToUpper() == Categorie.ToUpper() && Obj.CO_No == Conditionnement);
        }
    }
}
