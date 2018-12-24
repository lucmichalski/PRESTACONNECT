using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Sage
{
    public class P_STATISTIQUERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public ObservableCollection<P_STATISTIQUE> List()
        {
            return new ObservableCollection<P_STATISTIQUE>(this.DBSage.P_STATISTIQUE.ToList().Where(s => !String.IsNullOrWhiteSpace(s.S_Intitule)).ToList());
        }

        public Boolean ExistStatClient(String StatClient)
        {
            return this.DBSage.P_STATISTIQUE.Count(s => s.S_Intitule == StatClient) == 1;
        }

        public P_STATISTIQUE ReadStatClient(String StatClient)
        {
            return this.DBSage.P_STATISTIQUE.FirstOrDefault(s => s.S_Intitule == StatClient);
        }
    }
}
