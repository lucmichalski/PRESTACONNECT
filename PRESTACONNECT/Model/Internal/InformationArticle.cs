using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Internal
{
    public class InformationArticle : INotifyPropertyChanged
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
                if (InfoArticle != null)
                    InfoArticle.Inf_Mode = (short)InfoValeursMode.Marq;
                OnPropertyChanged("InfoValeursMode");
            }
        }

        private SageInfoArticle sageInfoArticle;
        public SageInfoArticle SageInfoArticle
        {
            get { return sageInfoArticle; }
            set
            {
                sageInfoArticle = value;
                OnPropertyChanged("SageInfoArticle");
            }
        }

        private Model.Local.InformationArticle infoArticle;
        public Model.Local.InformationArticle InfoArticle
        {
            get { return infoArticle; }
            set
            {
                infoArticle = value;
                OnPropertyChanged("InfoArticle");
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
                    if (InfoArticle != null)
                        InfoArticle.Cha_Id = (int)Feature.IDFeature;
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

        public InformationArticle(SageInfoArticle SageInformationArticle, ObservableCollection<InformationLibreValeursMode> ListInformationLibreMode, ObservableCollection<Model.Prestashop.PsFeatureLang> ListCharacteristic)
        {
            SageInfoArticle = SageInformationArticle;
            Model.Local.InformationArticleRepository InformationArticleRepository = new Local.InformationArticleRepository();
            InfoArticle = new Local.InformationArticle();
            if (InformationArticleRepository.ExistSageInfoArticle(SageInfoArticle.Marq))
                InfoArticle = InformationArticleRepository.ReadSageInfoArticle(SageInfoArticle.Marq);
            else
                InfoArticle.Sag_InfoArt = SageInfoArticle.Marq;

            InfoValeursMode = ListInformationLibreMode.FirstOrDefault(i => i.Marq == InfoArticle.Inf_Mode);

            if (InfoArticle.Cha_Id != 0 && ListCharacteristic.Count(c => c.IDFeature == InfoArticle.Cha_Id) == 1)
                Feature = ListCharacteristic.FirstOrDefault(c => c.IDFeature == InfoArticle.Cha_Id);
            else
                CanCreateFeature = (!new Model.Prestashop.PsFeatureLangRepository().ExistSageInformationLibre(SageInfoArticle.Intitule)) ? true : false;
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

                //Model.Prestashop.PsFeatureShopRepository PsFeatureShopRepository = new Prestashop.PsFeatureShopRepository();
                //if (!PsFeatureShopRepository.ExistInShop(PsFeature.IDFeature, Core.Global.CurrentShop.IDShop))
                //    PsFeatureShopRepository.Add(new Model.Prestashop.PsFeatureShop() { IDFeature = PsFeature.IDFeature, IDShop = Core.Global.CurrentShop.IDShop });

                Model.Prestashop.PsFeatureLangRepository PsFeatureLangRepository = new Prestashop.PsFeatureLangRepository();
                foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                {
                    Model.Prestashop.PsFeatureLang FeatureLang = new Prestashop.PsFeatureLang()
                    {
                        IDFeature = PsFeature.IDFeature,
                        IDLang = lang.IDLang,
                        Name = this.SageInfoArticle.Intitule,
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
            return SageInfoArticle.Intitule;
        }

        #endregion
    }
}
