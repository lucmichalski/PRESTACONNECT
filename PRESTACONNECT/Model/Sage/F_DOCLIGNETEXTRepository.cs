using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_DOCLIGNETEXTRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public F_DOCLIGNETEXT ReadLast(string glossaire)
        {
            return this.DBSage.F_DOCLIGNETEXT.Where(dt => dt.DT_Text == glossaire).OrderByDescending(dt => dt.cbMarq).FirstOrDefault();
        }

        public F_DOCLIGNETEXT ReadLastODBC()
        {
            return this.DBSage.F_DOCLIGNETEXT.Where(dt => dt.cbCreateur == "ODBC").OrderByDescending(dt => dt.cbMarq).FirstOrDefault();
        }
    }
}
