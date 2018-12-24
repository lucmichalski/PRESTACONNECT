using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public partial class MediaAssignmentRule
    {
        #region Properties

        public string GetRuleText
        {
            get
            {
                return new Internal.MediaRule((Core.Parametres.MediaRule)this.Rule).Intitule;
            }
        }

        #endregion
        
        #region Methods

        public override string ToString()
        {
            return GetRuleText;
        }

        #endregion
    }
}
