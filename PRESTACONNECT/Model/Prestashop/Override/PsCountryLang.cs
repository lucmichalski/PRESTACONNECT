using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsCountryLang
    {
        private bool _replace_isocode = false;
        public bool ReplaceISOCode
        {
            get { return _replace_isocode; }
            set { _replace_isocode = value; SendPropertyChanged("ReplaceISOCode"); }
        }

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
