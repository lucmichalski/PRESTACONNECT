using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_COLLABORATEURRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_COLLABORATEUR> List()
        {
            return this.DBSage.F_COLLABORATEUR.ToList();
        }

        public List<F_COLLABORATEUR> ListVendeur()
        {
            return this.DBSage.F_COLLABORATEUR.Where(c => c.CO_Vendeur != null && c.CO_Vendeur == (short)ABSTRACTION_SAGE.F_COLLABORATEUR.Obj._Enum_Boolean.Oui).ToList();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBSage.F_COLLABORATEUR.Count(Obj => Obj.cbMarq == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_COLLABORATEUR ReadId(Int32 Id)
        {
            return this.DBSage.F_COLLABORATEUR.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }
    }
}
