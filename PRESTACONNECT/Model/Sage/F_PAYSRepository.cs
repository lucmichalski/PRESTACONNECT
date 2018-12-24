using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_PAYSRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<F_PAYS> ListIntituleNotNull()
        {
            System.Linq.IQueryable<F_PAYS> Return = from Table in this.DBSage.F_PAYS
                                                    where Table.PA_Intitule != null && Table.PA_Intitule != ""
                                                     select Table;
            return Return.ToList();
        }

        public Boolean ExistPays(Int32 Id)
        {
            return this.DBSage.F_PAYS.Count(p => p.cbMarq == Id) > 0;
        }

        public F_PAYS ReadPays(Int32 Id)
        {
            return this.DBSage.F_PAYS.FirstOrDefault(p => p.cbMarq == Id);
        }

        public Boolean ExistPays(String pays)
        {
            return this.DBSage.F_PAYS.Count(p => p.PA_Intitule == pays) > 0;
        }

        public F_PAYS ReadPays(String pays)
        {
            return this.DBSage.F_PAYS.FirstOrDefault(p => p.PA_Intitule == pays);
        }

		public Boolean ExistPaysIso2(string iso2)
		{
			return DBSage.F_PAYS.Count(p => p.PA_CodeISO2 == iso2) > 0;
		}

		public F_PAYS ReadPaysIso2(string iso2)
		{
			return DBSage.F_PAYS.FirstOrDefault(p => p.PA_CodeISO2 == iso2);
		}
	}
}
