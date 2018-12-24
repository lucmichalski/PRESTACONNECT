using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_COMPTEGRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_COMPTEG> List()
        {
            return this.DBSage.F_COMPTEG.ToList();
        }

        public List<F_COMPTEG> ListSommeilStartWithCompte(short Sommeil, String Compte)
        {
            System.Linq.IQueryable<F_COMPTEG> Return = from Table in this.DBSage.F_COMPTEG
                                                       where Table.CG_Sommeil == Sommeil && Table.CG_Num.StartsWith(Compte)
                                                       select Table;
            return Return.ToList();
        }

        public List<F_COMPTEG> ListSommeilNature(short Sommeil, ABSTRACTION_SAGE.F_COMPTEG.Obj._Enum_N_Nature Nature)
        {
            return DBSage.F_COMPTEG.Where(CG => CG.CG_Sommeil == Sommeil && CG.N_Nature == (short)Nature).ToList();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBSage.F_COMPTEG.Count(Obj => Obj.cbMarq == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_COMPTEG ReadId(Int32 Id)
        {
            return this.DBSage.F_COMPTEG.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }
    }
}
