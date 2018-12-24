using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Internal
{
    public class InformationLibreClient : INotifyPropertyChanged
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
                if (InfoLibreClient != null)
                    InfoLibreClient.Inf_Mode = (short)InfoValeursMode.Marq;
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

        private Model.Local.InformationLibreClient infoLibreClient;
        public Model.Local.InformationLibreClient InfoLibreClient
        {
            get { return infoLibreClient; }
            set
            {
                infoLibreClient = value;
                OnPropertyChanged("InfoLibreClient");
            }
        }

        private Model.Prestashop.PsCustomerFeatureLang customerFeature;
        public Model.Prestashop.PsCustomerFeatureLang CustomerFeature
        {
            get { return customerFeature; }
            set
            {
                customerFeature = value;
                if (InfoLibreClient != null)
                    InfoLibreClient.Cha_Id = (int)CustomerFeature.IDCustomerFeature;
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

        public InformationLibreClient(Model.Sage.cbSysLibre SageInformationLibre, ObservableCollection<InformationLibreValeursMode> ListInformationLibreMode, ObservableCollection<Model.Prestashop.PsCustomerFeatureLang> ListCharacteristic)
        {
            SageInfoLibre = SageInformationLibre;
            Model.Local.InformationLibreClientRepository InformationLibreClientRepository = new Local.InformationLibreClientRepository();
            InfoLibreClient = new Local.InformationLibreClient();
            if (InformationLibreClientRepository.ExistInfoLibre(SageInfoLibre.CB_Name))
                InfoLibreClient = InformationLibreClientRepository.ReadInfoLibre(SageInfoLibre.CB_Name);
            else
                InfoLibreClient.Sag_InfoLibreClient = SageInfoLibre.CB_Name;

            InfoValeursMode = ListInformationLibreMode.FirstOrDefault(i => i.Marq == InfoLibreClient.Inf_Mode);

            if (InfoLibreClient.Cha_Id != 0 && ListCharacteristic.Count(c => c.IDCustomerFeature == InfoLibreClient.Cha_Id) == 1)
                CustomerFeature = ListCharacteristic.FirstOrDefault(c => c.IDCustomerFeature == InfoLibreClient.Cha_Id);
            else
                CanCreateCustomerFeature = (!new Model.Prestashop.PsCustomerFeatureLangRepository().ExistSageInformationLibre(SageInfoLibre.CB_Name)) ? true : false;
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
                        Name = this.InfoLibreClient.Sag_InfoLibreClient
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
            return ((SageInfoLibre != null) ? SageInfoLibre.CB_Name.ToString() : string.Empty);
        }

        #endregion
    }
}
