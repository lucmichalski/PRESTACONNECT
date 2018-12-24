using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsFeatureValueLang
    {
        //private bool _HasUpdated = false;
        //public bool HasUpdated
        //{
        //    get { return _HasUpdated; }
        //    set { _HasUpdated = value; SendPropertyChanged("HasUpdated"); }
        //}

        public uint id_feature = 0;
        public byte custom = 0;

        #region Methods
        
        //partial void OnValueChanged() { HasUpdated = true; }

        public override string ToString()
        {
            return Value;
        }

        #endregion
    }
}
