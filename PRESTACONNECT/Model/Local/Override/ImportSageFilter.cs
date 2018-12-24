using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public partial class ImportSageFilter
    {
        #region Properties

        public string GetTypeSearchText
        {
            get
            {
                return new Internal.ImportSageFilterTypeSearchValue((Core.Parametres.ImportSageFilterTypeSearchValue)this.Imp_TypeSearchValue).Intitule;
            }
        }

        public string GetTargetDataText
        {
            get
            {
                return new Internal.ImportSageFilterTargetData((Core.Parametres.ImportSageFilterTargetData)this.Imp_TargetData).Intitule;
            }
        }

        #endregion

        partial void OnImp_TypeSearchValueChanged() { SendPropertyChanged("GetTypeSearchText"); }
        partial void OnImp_TargetDataChanged() { SendPropertyChanged("GetTargetDataText"); }
    }
}
