using System;
using System.ComponentModel;
using PRESTACONNECT.Core;

namespace PRESTACONNECT.Model.Internal
{
    public class PriceCategory : INotifyPropertyChanged
    {
        #region Properties

        public Model.Sage.P_CATTARIF P_CATTARIF;

        public string CT_Intitule
        {
            get
            {
                return (P_CATTARIF != null) ? this.P_CATTARIF.CT_Intitule : string.Empty;
            }
        }

        private bool _TransfertClient;

        public bool TransfertClient
        {
            get { return _TransfertClient; }
            set
            {
                _TransfertClient = value;
                OnPropertyChanged("TransfertClient");
            }
        }

        private bool _Available;

        public bool Available
        {
            get { return _Available; }
            set
            {
                _Available = value;
                if (!_Available)
                    TransfertClient = false;
                OnPropertyChanged("Available");
            }
        }

        #endregion
        #region Constructors

        public PriceCategory(Model.Sage.P_CATTARIF categorieTarifaire)
        {
            P_CATTARIF = categorieTarifaire;
            TransfertClient = Core.Global.GetConfig().TransfertPriceCategoryAvailable.Contains(P_CATTARIF.cbMarq);
            Available = new Model.Local.GroupRepository().CatTarifSageMonoGroupe(P_CATTARIF.cbMarq);
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
            return CT_Intitule;
        }

        #endregion
    }
}
