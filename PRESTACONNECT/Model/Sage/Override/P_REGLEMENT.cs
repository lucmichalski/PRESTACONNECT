using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public partial class P_REGLEMENT
    {
        public enum _Fields { ComboText};
        public string ComboText
        {
            get
            {
                return this.cbIndice + " - " + this.R_Intitule;
            }
        }

        #region Methods

        public override string ToString()
        {
            return R_Intitule;
        }

        #endregion
    }
}
