using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Internal
{
    public class StatistiqueArticle : INotifyPropertyChanged
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

        private InformationLibreValeursMode infoValeursMode;
        public InformationLibreValeursMode InfoValeursMode
        {
            get { return infoValeursMode; }
            set
            {
                infoValeursMode = value;
                if (StatArt != null)
                    StatArt.Inf_Mode = (short)InfoValeursMode.Marq;
                OnPropertyChanged("InfoValeursMode");
            }
        }

        private Model.Sage.P_INTSTATART sageStatistique;
        public Model.Sage.P_INTSTATART SageStatistique
        {
            get { return sageStatistique; }
            set
            {
                sageStatistique = value;
                OnPropertyChanged("SageStatistique");
            }
        }

        private Model.Local.StatistiqueArticle statArt;
        public Model.Local.StatistiqueArticle StatArt
        {
            get { return statArt; }
            set
            {
                statArt = value;
                OnPropertyChanged("StatArt");
            }
        }

        private Model.Prestashop.PsFeatureLang feature;
        public Model.Prestashop.PsFeatureLang Feature
        {
            get { return feature; }
            set
            {
                feature = value;
                if (value != null)
                {
                    if (StatArt != null)
                        StatArt.Cha_Id = (int)Feature.IDFeature;
                }
                Active = (value != null);

                OnPropertyChanged("Feature");
            }
        }

        private Boolean canCreateFeature;
        public Boolean CanCreateFeature
        {
            get { return canCreateFeature; }
            set
            {
                canCreateFeature = value;
                OnPropertyChanged("CanCreateFeature");
            }
        }

        #endregion

        #region Constructors

        public StatistiqueArticle(Model.Sage.P_INTSTATART SageStatistiqueArticle, ObservableCollection<InformationLibreValeursMode> ListInformationLibreMode, ObservableCollection<Model.Prestashop.PsFeatureLang> ListCharacteristic)
        {
            SageStatistique = SageStatistiqueArticle;
            Model.Local.StatistiqueArticleRepository StatistiqueArticleRepository = new Local.StatistiqueArticleRepository();
            StatArt = new Local.StatistiqueArticle();
            if (StatistiqueArticleRepository.ExistStatArticle(SageStatistique.P_IntStatArt1))
                StatArt = StatistiqueArticleRepository.ReadStatArticle(SageStatistique.P_IntStatArt1);
            else
                StatArt.Sag_StatArt = SageStatistique.P_IntStatArt1;

            InfoValeursMode = ListInformationLibreMode.FirstOrDefault(i => i.Marq == StatArt.Inf_Mode);

            if (StatArt.Cha_Id != 0 && ListCharacteristic.Count(c => c.IDFeature == StatArt.Cha_Id) == 1)
                Feature = ListCharacteristic.FirstOrDefault(c => c.IDFeature == StatArt.Cha_Id);
            else
                CanCreateFeature = (!new Model.Prestashop.PsFeatureLangRepository().ExistSageInformationLibre(SageStatistique.P_IntStatArt1)) ? true : false;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CreateFeature(out Model.Prestashop.PsFeatureLang newCharacteristic)
        {
            newCharacteristic = null;
            if (CanCreateFeature)
            {
                Model.Prestashop.PsFeatureRepository PsFeatureRepository = new Prestashop.PsFeatureRepository();
                Model.Prestashop.PsFeature PsFeature = new Prestashop.PsFeature()
                {
                    Position = Prestashop.PsFeatureRepository.NextPosition()
                };
                PsFeatureRepository.Add(PsFeature);
                
                new Prestashop.PsFeatureShopRepository().Add(new Model.Prestashop.PsFeatureShop() { IDFeature = PsFeature.IDFeature, IDShop = Core.Global.CurrentShop.IDShop });

                Model.Prestashop.PsFeatureLangRepository PsFeatureLangRepository = new Prestashop.PsFeatureLangRepository();
                foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                {
                    Model.Prestashop.PsFeatureLang FeatureLang = new Prestashop.PsFeatureLang()
                    {
                        IDFeature = PsFeature.IDFeature,
                        IDLang = lang.IDLang,
                        Name = this.StatArt.Sag_StatArt
                    };
                    PsFeatureLangRepository.Add(FeatureLang);
                    if (lang.IDLang == Core.Global.Lang)
                        newCharacteristic = FeatureLang;
                }
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return "Statistique article Sage : " + ((SageStatistique != null) ? SageStatistique.P_IntStatArt1.ToString() : string.Empty);
        }

        #endregion
    }
}
