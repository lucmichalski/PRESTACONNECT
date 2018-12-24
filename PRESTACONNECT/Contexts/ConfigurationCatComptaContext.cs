using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Contexts
{
    internal sealed class ConfigurationCatComptaContext : Context
    {
        #region Properties

        private Model.Local.CountryRepository CountryRepository;

        private ObservableCollection<CategorieComptable> listCategorieComptable;
        public ObservableCollection<CategorieComptable> ListCategorieComptable
        {
            get { return listCategorieComptable; }
            set
            {
                listCategorieComptable = value;
                OnPropertyChanged("ListCategorieComptable");
            }
        }

        private ObservableCollection<Model.Local.Country> listPays;
        public ObservableCollection<Model.Local.Country> ListPays
        {
            get { return listPays; }
            set
            {
                listPays = value;
                OnPropertyChanged("ListPays");
            }
        }

        private Model.Local.Country selectedPays;
        public Model.Local.Country SelectedPays
        {
            get { return selectedPays; }
            set
            {
                selectedPays = value;
                OnPropertyChanged("SelectedPays");
            }
        }

        #endregion

        #region Constructors

        public ConfigurationCatComptaContext()
            : base()
        {
            CountryRepository = new Model.Local.CountryRepository();

            ListCategorieComptable = new ObservableCollection<CategorieComptable>(new Model.Sage.P_CATCOMPTARepository().ListCatComptaVente());
            Core.Temp.ListCategorieComptable = ListCategorieComptable.ToList();

            Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
            Core.Temp.ListPsCountryLang = PsCountryLangRepository.ListActive(1, Core.Global.CurrentShop.IDShop).Where(c => c.IDLang == Core.Global.Lang).ToList();
            foreach (Model.Prestashop.PsCountryLang PsCountryLang in Core.Temp.ListPsCountryLang)
                if (!CountryRepository.ExistCountry(PsCountryLang.IDCountry))
                    CountryRepository.Add(new Model.Local.Country()
                    {
                        Pre_IdCountry = (int)PsCountryLang.IDCountry,
                    });
            ListPays = new ObservableCollection<Model.Local.Country>(CountryRepository.List());
        }

        #endregion

        #region Methods

        public void SaveCatCompta()
        {
            CountryRepository.Save();
            MessageBox.Show("Paramètres enregistrés", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void UnselectCatCompta()
        {
            if (SelectedPays != null && SelectedPays.Sage_CatCompta != null)
            {
                SelectedPays.Sage_CatCompta = null;
            }
        }
        public void UnselectCatComptaPro()
        {
            if (SelectedPays != null && SelectedPays.Sage_CatComptaPro != null)
            {
                SelectedPays.Sage_CatComptaPro = null;
            }
        }

        #endregion
    }
}
