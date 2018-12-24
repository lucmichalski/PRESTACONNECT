using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Sage
{
    public class P_INTSTATARTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public ObservableCollection<P_INTSTATART> List()
        {
            return new ObservableCollection<P_INTSTATART>(this.DBSage.P_INTSTATART.ToList().Where(s => !String.IsNullOrWhiteSpace(s.P_IntStatArt1)).ToList());
        }

        public Boolean ExistStatArt(String StatArt)
        {
            return this.DBSage.P_INTSTATART.Count(s => s.P_IntStatArt1 == StatArt) == 1;
        }

        public P_INTSTATART ReadStatArt(String StatArt)
        {
            return this.DBSage.P_INTSTATART.FirstOrDefault(s => s.P_IntStatArt1 == StatArt);
        }
    }
}
