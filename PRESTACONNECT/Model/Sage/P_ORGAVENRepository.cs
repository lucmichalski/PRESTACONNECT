using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Sage
{
    public class P_ORGAVENRepository
    {
        public enum Doc_Type
        {
            Devis = 1,
            BonCommande = 2,
        }

        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public ObservableCollection<P_ORGAVEN> List()
        {
            return new ObservableCollection<P_ORGAVEN>(this.DBSage.P_ORGAVEN);
        }

        public Boolean Exist(Doc_Type TypeDoc)
        {
            return this.DBSage.P_ORGAVEN.Count(p => p.cbMarq == (int)TypeDoc) == 1;
        }

        public P_ORGAVEN Read(Doc_Type TypeDoc)
        {
            return this.DBSage.P_ORGAVEN.FirstOrDefault(p => p.cbMarq == (int)TypeDoc);
        }
    }
}
