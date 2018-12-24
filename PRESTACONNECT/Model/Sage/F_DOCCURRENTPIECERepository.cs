using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_DOCCURRENTPIECERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public F_DOCCURRENTPIECE ReadDomaineColSouche(Int32 Domaine, short Col, short Souche)
        {
            return this.DBSage.F_DOCCURRENTPIECE.FirstOrDefault(Obj => Obj.DC_Domaine == 0 && Obj.DC_IdCol == Col && Obj.DC_Souche == Souche);
        }
    }
}
