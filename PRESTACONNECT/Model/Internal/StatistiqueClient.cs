using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Internal
{
    public class StatistiqueClient : INotifyPropertyChanged
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
                if (StatClient != null)
                    StatClient.Inf_Mode = (short)InfoValeursMode.Marq;
                OnPropertyChanged("InfoValeursMode");
            }
        }

        private Model.Sage.P_STATISTIQUE sageStatistique;
        public Model.Sage.P_STATISTIQUE SageStatistique
        {
            get { return sageStatistique; }
            set
            {
                sageStatistique = value;
                OnPropertyChanged("SageStatistique");
            }
        }

        private Model.Local.StatistiqueClient statClient;
        public Model.Local.StatistiqueClient StatClient
        {
            get { return statClient; }
            set
            {
                statClient = value;
                OnPropertyChanged("StatClient");
            }
        }

        private Model.Prestashop.PsCustomerFeatureLang customerFeature;
        public Model.Prestashop.PsCustomerFeatureLang CustomerFeature
        {
            get { return customerFeature; }
            set
            {
                customerFeature = value;
                if (StatClient != null)
                    StatClient.Cha_Id = (int)CustomerFeature.IDCustomerFeature;
                if (value != null)
                    Active = true;

                OnPropertyChanged("CustomerFeature");
            }
        }

        private Boolean canCreateCustomerFeature;
        public Boolean CanCreateCustomerFeature
        {
            get { return canCreateCustomerFeature; }
            set
            {
                canCreateCustomerFeature = value;
                OnPropertyChanged("CanCreateCustomerFeature");
            }
        }

        #endregion

        #region Constructors

        public StatistiqueClient(Model.Sage.P_STATISTIQUE SageStatistiqueClient, ObservableCollection<InformationLibreValeursMode> ListInformationLibreMode, ObservableCollection<Model.Prestashop.PsCustomerFeatureLang> ListCustomerFeature)
        {
            SageStatistique = SageStatistiqueClient;
            Model.Local.StatistiqueClientRepository StatistiqueClientRepository = new Local.StatistiqueClientRepository();
            StatClient = new Local.StatistiqueClient();
            if (StatistiqueClientRepository.ExistStatClient(SageStatistique.S_Intitule))
                StatClient = StatistiqueClientRepository.ReadStatClient(SageStatistique.S_Intitule);
            else
                StatClient.Sag_StatClient = SageStatistique.S_Intitule;

            InfoValeursMode = ListInformationLibreMode.FirstOrDefault(i => i.Marq == StatClient.Inf_Mode);

            if (StatClient.Cha_Id != 0 && ListCustomerFeature.Count(c => c.IDCustomerFeature == StatClient.Cha_Id) == 1)
                CustomerFeature = ListCustomerFeature.FirstOrDefault(c => c.IDCustomerFeature == StatClient.Cha_Id);
            else
                CanCreateCustomerFeature = (!new Model.Prestashop.PsCustomerFeatureLangRepository().ExistSageInformationLibre(SageStatistique.S_Intitule)) ? true : false;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CreateCustomerFeature(out Model.Prestashop.PsCustomerFeatureLang newCharacteristic)
        {
            newCharacteristic = null;
            if (CanCreateCustomerFeature)
            {
                Model.Prestashop.PsCustomerFeatureRepository PsCustomerFeatureRepository = new Prestashop.PsCustomerFeatureRepository();
                Model.Prestashop.PsCustomerFeature PsCustomerFeature = new Prestashop.PsCustomerFeature()
                {
                    Position = Prestashop.PsCustomerFeatureRepository.NextPosition()
                };
                PsCustomerFeatureRepository.Add(PsCustomerFeature);
                
                new Prestashop.PsCustomerFeatureShopRepository().Add(new Model.Prestashop.PsCustomerFeatureShop() { IDCustomerFeature = PsCustomerFeature.IDCustomerFeature, IDShop = Core.Global.CurrentShop.IDShop });
                
                Model.Prestashop.PsCustomerFeatureLangRepository PsCustomerFeatureLangRepository = new Prestashop.PsCustomerFeatureLangRepository();
                foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                {
                    Model.Prestashop.PsCustomerFeatureLang CustomerFeatureLang = new Prestashop.PsCustomerFeatureLang()
                    {
                        IDCustomerFeature = PsCustomerFeature.IDCustomerFeature,
                        IDLang = lang.IDLang,
                        Name = this.StatClient.Sag_StatClient
                    };
                    PsCustomerFeatureLangRepository.Add(CustomerFeatureLang);
                    if (lang.IDLang == Core.Global.Lang)
                        newCharacteristic = CustomerFeatureLang;
                }
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return "Statistique client Sage : " + ((SageStatistique != null) ? SageStatistique.S_Intitule.ToString() : string.Empty);
        }

        #endregion
    }
}
