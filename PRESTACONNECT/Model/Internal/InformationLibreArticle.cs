using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Internal
{
    public class InformationLibreArticle : INotifyPropertyChanged
    {

        #region Properties

        private Boolean active;
        public Boolean Active
        {
            get { return active; }
            set
            {
                active = value;
                OnPropertyChanged("Active");
            }
        }

        private Model.Sage.cbSysLibre sageInfoLibre;
        public Model.Sage.cbSysLibre SageInfoLibre
        {
            get { return sageInfoLibre; }
            set
            {
                sageInfoLibre = value;
                OnPropertyChanged("SageInfoLibre");
            }
        }

        private String action;
        public String Action
        {
            get { return action; }
            set
            {
                action = value;
                OnPropertyChanged("Action");
            }
        }

        private Model.Local.InformationLibreArticle infoLibreArticle;
        public Model.Local.InformationLibreArticle InfoLibreArticle
        {
            get { return infoLibreArticle; }
            set
            {
                infoLibreArticle = value;
                OnPropertyChanged("InfoLibreArticle");
            }
        }

        #endregion

        #region Constructors

        public InformationLibreArticle(Model.Sage.cbSysLibre SageInformationLibre, ObservableCollection<InformationLibreValeursMode> ListInformationLibreMode)
        {
            SageInfoLibre = SageInformationLibre;
            Model.Local.InformationLibreArticleRepository InformationLibreArticleRepository = new Local.InformationLibreArticleRepository();
            InfoLibreArticle = new Local.InformationLibreArticle();
            if (InformationLibreArticleRepository.ExistInfoLibre(SageInfoLibre.CB_Name))
                InfoLibreArticle = InformationLibreArticleRepository.ReadInfoLibre(SageInfoLibre.CB_Name);
            else
                InfoLibreArticle.Sag_InfoLibreArticle = SageInfoLibre.CB_Name;

            if (InfoLibreArticle.Inf_Catalogue == 2)
            {
                Action = "Création des catalogues enfant pour le catalogue de niveau 1 \"" + InfoLibreArticle.Inf_Parent + "\" déjà existant";
            }
            else if (InfoLibreArticle.Inf_Catalogue == 3)
            {
                Action = "Création des catalogues de niveau 3 enfant des catalogues définis par l'information libre \"" + InfoLibreArticle.Inf_Parent + "\"";
            }
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
            return ((SageInfoLibre != null) ? SageInfoLibre.CB_Name.ToString() : string.Empty);
        }

        #endregion
    }
}
