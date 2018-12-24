using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Sage
{
    public class F_FAMCLIENTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public IEnumerable<F_FAMCLIENT> List(string codeFamille)
        {
            var query = from remise in DBSage.F_FAMCLIENT
                        where remise.FA_CodeFamille.ToUpper() == codeFamille.ToUpper()
                        select remise;

            return query;
        }
    }
}
