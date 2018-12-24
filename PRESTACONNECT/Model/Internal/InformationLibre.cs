using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Internal
{
    public class InformationLibre : INotifyPropertyChanged
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
                if (InfoLibre != null)
                    InfoLibre.Inf_Mode = (short)InfoValeursMode.Marq;
                OnPropertyChanged("InfoValeursMode");
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

        private Model.Local.InformationLibre infoLibre;
        public Model.Local.InformationLibre InfoLibre
        {
            get { return infoLibre; }
            set
            {
                infoLibre = value;
                OnPropertyChanged("InfoLibre");
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
                    if (InfoLibre != null)
                        InfoLibre.Cha_Id = (int)Feature.IDFeature;
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

        public InformationLibre(Model.Sage.cbSysLibre SageInformationLibre, ObservableCollection<InformationLibreValeursMode> ListInformationLibreMode, ObservableCollection<Model.Prestashop.PsFeatureLang> ListCharacteristic)
        {
            SageInfoLibre = SageInformationLibre;
            Model.Local.InformationLibreRepository InformationLibreRepository = new Local.InformationLibreRepository();
            InfoLibre = new Local.InformationLibre();
            if (InformationLibreRepository.ExistInfoLibre(SageInfoLibre.CB_Name))
                InfoLibre = InformationLibreRepository.ReadInfoLibre(SageInfoLibre.CB_Name);
            else
                InfoLibre.Sag_InfoLibre = SageInfoLibre.CB_Name;

            InfoValeursMode = ListInformationLibreMode.FirstOrDefault(i => i.Marq == InfoLibre.Inf_Mode);

            if (InfoLibre.Cha_Id != 0 && ListCharacteristic.Count(c => c.IDFeature == InfoLibre.Cha_Id) == 1)
                Feature = ListCharacteristic.FirstOrDefault(c => c.IDFeature == InfoLibre.Cha_Id);
            else
                CanCreateFeature = (!new Model.Prestashop.PsFeatureLangRepository().ExistSageInformationLibre(SageInfoLibre.CB_Name)) ? true : false;
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
                        Name = this.InfoLibre.Sag_InfoLibre
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
            return ((SageInfoLibre != null) ? SageInfoLibre.CB_Name.ToString() : string.Empty);
        }

        #endregion
    }
}
