using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ARTCLIENTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistReferenceCategorie(String Reference, Int32 Categorie)
        {
            if (this.DBSage.F_ARTCLIENT.Count(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.AC_Categorie == Categorie) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_ARTCLIENT ReadReferenceCategorie(String Reference, Int32 Categorie)
        {
            return this.DBSage.F_ARTCLIENT.FirstOrDefault(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.AC_Categorie == Categorie);
        }
        public F_ARTCLIENT Read(String Reference, String NumeroClient)
        {
            return this.DBSage.F_ARTCLIENT.FirstOrDefault(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper() && Obj.AC_Categorie == 0 && Obj.CT_Num == NumeroClient);
        }

        public Boolean ExistReferenceClient(string ref_client)
        {
            return this.DBSage.F_ARTCLIENT.Count(ac => ac.AC_Categorie == 0 && ac.CT_Num != null && ac.CT_Num != string.Empty && ac.AC_RefClient == ref_client) > 0;
        }
        public List<F_ARTCLIENT> ListClien(string ref_client)
        {
            return this.DBSage.F_ARTCLIENT.Where(ac => ac.AC_Categorie == 0 && ac.CT_Num != null && ac.CT_Num != string.Empty && ac.AC_RefClient == ref_client).ToList();
        }

        public IEnumerable<F_ARTCLIENT> List(string reference)
        {
            var query = from remise in DBSage.F_ARTCLIENT
                        where ((remise.AR_Ref.ToUpper() == reference.ToUpper()) && (remise.AC_Categorie == 0))
                        select remise;

            return query;
        }
    }
}
