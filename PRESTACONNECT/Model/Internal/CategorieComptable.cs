using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class CategorieComptable : INotifyPropertyChanged
    {
        
        #region Properties

        private int sageMarq;
        public int SageMarq
        {
            get { return sageMarq; }
            set { sageMarq = value; OnPropertyChanged("SageMarq"); }
        }

        private string intitule;
        public string Intitule
        {
            get { return intitule; }
            set { intitule = value; OnPropertyChanged("Intitule"); }
        }

        #endregion
        #region Constructors

        public CategorieComptable()
        {
            SageMarq = 0;
            Intitule = "";
        }

        #endregion
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Intitule;
        }

        #endregion
    }
}
