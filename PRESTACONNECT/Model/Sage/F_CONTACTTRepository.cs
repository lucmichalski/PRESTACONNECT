using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_CONTACTTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_CONTACTT> List()
        {
            return this.DBSage.F_CONTACTT.ToList();
        }

        public List<F_CONTACTT> ListClient(String Client, Boolean HasMail)
        {
            return DBSage.F_CONTACTT.Where(c => c.CT_Num == Client && ((HasMail && c.CT_EMail.Trim() != "") || !HasMail)).ToList();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBSage.F_CONTACTT.Count(Obj => Obj.cbMarq == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_CONTACTT ReadId(Int32 Id)
        {
            return this.DBSage.F_CONTACTT.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }

        public bool ExistContactService(string Client, int Service)
        {
            if (this.DBSage.F_CONTACTT.Count(Obj => Obj.CT_Num == Client && Obj.N_Service == Service) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_CONTACTT ReadContactService(string Client, int Service)
        {
            return this.DBSage.F_CONTACTT.FirstOrDefault(Obj => Obj.CT_Num == Client && Obj.N_Service == Service);
        }
    }
}
