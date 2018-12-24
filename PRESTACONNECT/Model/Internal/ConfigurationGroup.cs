using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class ConfigurationGroup : INotifyPropertyChanged
    {
        #region Properties

        public Model.Prestashop.PsGroup PsSource { get; private set; }
        public Model.Local.Group Source { get; private set; }

        private string intitule;
        public string Intitule
        {
            get { return intitule; }
            set
            {
                intitule = value;
                OnPropertyChanged("Intitule");
            }
        }

        public bool Show
        {
            get { return Source.Grp_ShowCatalog; }
            set
            {
                Source.Grp_ShowCatalog = value;
                OnPropertyChanged("Show");
            }
        }
        
        private Model.Sage.P_CATTARIF categorieTarifaire;
        public Model.Sage.P_CATTARIF CategorieTarifaire
        {
            get { return categorieTarifaire; }
            set
            {
                categorieTarifaire = value;
                OnPropertyChanged("CategorieTarifaire");
            }
        }

        #endregion
        #region Constructors

        public ConfigurationGroup(Model.Local.Group local, Model.Prestashop.PsGroup prestashop)
        {
            Source = local;
            PsSource = prestashop;
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
            return ((Source != null) ? Source.ComboText : string.Empty);
        }

        #endregion
    }
}
