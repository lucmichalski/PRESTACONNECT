using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_TAXERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistCode(String Code)
        {
            if (this.DBSage.F_TAXE.Count(Obj => Obj.TA_Code.ToUpper() == Code.ToUpper()) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_TAXE ReadCode(String Code)
        {
            return this.DBSage.F_TAXE.FirstOrDefault(Obj => Obj.TA_Code.ToUpper() == Code.ToUpper());
        }

        public List<F_TAXE> List()
        {
            return this.DBSage.F_TAXE.ToList();
        }

        public List<F_TAXE> ListTTauxSens(ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_TTaux TTaux, short Sens)
        {
            System.Linq.IQueryable<F_TAXE> Return = from Table in this.DBSage.F_TAXE
                                                  where Table.TA_TTaux == (short)TTaux && Table.TA_Sens == Sens
                                                  select Table;
            return Return.ToList();

        }

        public F_TAXE Read(Int32 Id)
        {
            return this.DBSage.F_TAXE.First(tax => tax.cbMarq == Id);
        }

        public Boolean Exist(Int32 Id)
        {
            return this.DBSage.F_TAXE.Count(tax => tax.cbMarq == Id)>0;
        }
    }
}
