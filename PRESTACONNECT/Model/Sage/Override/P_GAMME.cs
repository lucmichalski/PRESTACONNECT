using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Sage
{
    public partial class P_GAMME
    {
        #region Properties

        private bool _WeightConversion;
        public bool WeightConversion
        {
            get { return _WeightConversion; }
            set
            {
                _WeightConversion = value;
                OnPropertyChanged("WeightConversion");
            }
        }

        #endregion

        #region Events

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return G_Intitule;
        }

        #endregion
    }
}
