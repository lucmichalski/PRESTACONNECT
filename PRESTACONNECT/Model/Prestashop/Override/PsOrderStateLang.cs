using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsOrderStateLang
    {
        public string ComboText
        {
            get
            {
                return IDOrderState + " - " + Name;
            }
        }

        #region Methods

        public override string ToString()
        {
            return IDOrderState + " - " + Name;
        }

        #endregion
    }
}
