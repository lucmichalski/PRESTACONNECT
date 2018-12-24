using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Sage
{
    public partial class F_CONDITION : INotifyPropertyChanged
    {
        public String ComboText
        {
            get
            {
                return cbMarq + " - " + EC_Enumere;
            }
        }

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return EC_Enumere;
        }

        #endregion
    }
}
