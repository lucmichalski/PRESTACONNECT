using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_LIVRAISONRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public F_LIVRAISON ReadComptet(Int32 Id)
        {
            return this.DBSage.F_LIVRAISON.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }

        public List<F_LIVRAISON> List()
        {
            return this.DBSage.F_LIVRAISON.ToList();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBSage.F_LIVRAISON.Count(Obj => Obj.cbMarq == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistComptetPrincipal(String Comptet, short Principal)
        {
            if (this.DBSage.F_LIVRAISON.Count(Obj => Obj.CT_Num.ToUpper() == Comptet && Obj.LI_Principal == Principal) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistComptetIntitule(String Comptet, String Intitule)
        {
            if (this.DBSage.F_LIVRAISON.Count(Obj => Obj.CT_Num.ToUpper() == Comptet && Obj.LI_Intitule.ToUpper() == Intitule.ToUpper()) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_LIVRAISON ReadComptetPrincipal(String Comptet, short Principal)
        {
            return this.DBSage.F_LIVRAISON.FirstOrDefault(Obj => Obj.CT_Num.ToUpper() == Comptet && Obj.LI_Principal == Principal);
        }

        public F_LIVRAISON ReadComptetIntitule(String Comptet, String Intitule)
        {
            return this.DBSage.F_LIVRAISON.FirstOrDefault(Obj => Obj.CT_Num.ToUpper() == Comptet && Obj.LI_Intitule.ToUpper() == Intitule.ToUpper());
        }

        public F_LIVRAISON ReadId(Int32 Id)
        {
            return this.DBSage.F_LIVRAISON.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }

        public IQueryable<F_LIVRAISON> ListComptetPrincipale(String Comptet, short Principale)
        {
            return this.DBSage.F_LIVRAISON.Where(l => l.CT_Num == Comptet && l.LI_Principal == Principale);
        }
        public IQueryable<F_LIVRAISON> ListComptet(String Comptet)
        {
            return this.DBSage.F_LIVRAISON.Where(l => l.CT_Num == Comptet);
        }
    }
}
