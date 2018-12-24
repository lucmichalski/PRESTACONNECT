using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsAttributeLang
    {
        public uint id_attribute_group = 0;
        public uint position = 0;
        
        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
