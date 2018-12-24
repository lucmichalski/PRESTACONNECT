using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Contexts
{
    internal sealed class GammeContext : Context
    {
        #region Properties

        private ObservableCollection<Model.Prestashop.PsAttributeGroupLang> listPsAttributeGroupLang;
        public ObservableCollection<Model.Prestashop.PsAttributeGroupLang> ListPsAttributeGroupLang
        {
            get { return listPsAttributeGroupLang; }
            set { listPsAttributeGroupLang = value; OnPropertyChanged("ListPsAttributeGroupLang"); }
        }

        private Model.Prestashop.PsAttributeGroupLang selectedPsAttributeGroupLang = new Model.Prestashop.PsAttributeGroupLang();
        public Model.Prestashop.PsAttributeGroupLang SelectedPsAttributeGroupLang
        {
            get { return selectedPsAttributeGroupLang; }
            set { selectedPsAttributeGroupLang = value; OnPropertyChanged("SelectedPsAttributeGroupLang"); }
        }

        private string newAttributeGroupName = string.Empty;
        public string NewAttributeGroupName
        {
            get { return newAttributeGroupName; }
            set { newAttributeGroupName = value; OnPropertyChanged("NewAttributeGroupName"); }
        }

        private string newAttributeValue = string.Empty;
        public string NewAttributeValue
        {
            get { return newAttributeValue; }
            set { newAttributeValue = value; OnPropertyChanged("NewAttributeValue"); }
        }

        #endregion

        #region Constructors

        public GammeContext()
            : base()
        {
            LoadAttributeGroup();
        }

        #endregion

        #region Overrriden methods

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }

        #endregion

        #region Methods

        public void LoadAttributeGroup()
        {
            ListPsAttributeGroupLang = new ObservableCollection<Model.Prestashop.PsAttributeGroupLang>(new Model.Prestashop.PsAttributeGroupLangRepository().List(Core.Global.Lang));
        }

        public void AddPsAttributeGroup()
        {
            if (!string.IsNullOrWhiteSpace(NewAttributeGroupName))
            {
                Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Model.Prestashop.PsAttributeGroupLangRepository();
                if (PsAttributeGroupLangRepository.ExistNameLang(NewAttributeGroupName, Core.Global.Lang))
                {
                    MessageBox.Show("Un groupe d'attributs avec le nom \"" + NewAttributeGroupName + "\" existe déjà !", "Groupe d'attributs",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Model.Prestashop.PsAttributeGroupRepository PsAttributeGroupRepository = new Model.Prestashop.PsAttributeGroupRepository();

                    Model.Prestashop.PsAttributeGroup PsAttributeGroup;
                    Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang;

                    PsAttributeGroup = new Model.Prestashop.PsAttributeGroup()
                    {
                        #if (PRESTASHOP_VERSION_172)
						GroupType = "select",
						#endif
                        IsColorGroup = 0,
                        Position = PsAttributeGroupRepository.NextPosition(),
                    };
                    PsAttributeGroupRepository.Add(PsAttributeGroup, Core.Global.CurrentShop.IDShop);

                    foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                        if (!PsAttributeGroupLangRepository.ExistAttributeGroupLang(PsAttributeGroup.IDAttributeGroup, PsLang.IDLang))
                        {
                            PsAttributeGroupLang = new Model.Prestashop.PsAttributeGroupLang();
                            PsAttributeGroupLang.IDAttributeGroup = PsAttributeGroup.IDAttributeGroup;
                            PsAttributeGroupLang.IDLang = PsLang.IDLang;
                            PsAttributeGroupLang.Name = NewAttributeGroupName;
                            PsAttributeGroupLang.PublicName = NewAttributeGroupName;
                            PsAttributeGroupLangRepository.Add(PsAttributeGroupLang);
                        }
                    LoadAttributeGroup();
                    NewAttributeGroupName = string.Empty;
                }
            }
        }

        public void FilterAttributeValue()
        {
            if (SelectedPsAttributeGroupLang != null)
            {
                SelectedPsAttributeGroupLang.FilterAttributeValue();
            }
        }

        public void AddPsAttribute()
        {
            if (SelectedPsAttributeGroupLang != null && !string.IsNullOrWhiteSpace(NewAttributeValue))
            {
                Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();
                if (PsAttributeLangRepository.ExistAttributeLang(NewAttributeValue, Core.Global.Lang, SelectedPsAttributeGroupLang.IDAttributeGroup))
                {
                    MessageBox.Show("La valeur d'attribut \"" + NewAttributeValue + "\" pour le groupe \"" + SelectedPsAttributeGroupLang.Name + "\" existe déjà !", "Attribut",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Model.Prestashop.PsAttributeRepository PsAttributeRepository = new Model.Prestashop.PsAttributeRepository();

                    Model.Prestashop.PsAttribute PsAttribute = new Model.Prestashop.PsAttribute()
                    {
                        IDAttributeGroup = SelectedPsAttributeGroupLang.IDAttributeGroup,
                        Color = string.Empty,
                        Position = PsAttributeRepository.NextPosition(),
                    };
                    PsAttributeRepository.Add(PsAttribute, Core.Global.CurrentShop.IDShop);

                    foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                        if (!PsAttributeLangRepository.ExistAttributeLang(PsAttribute.IDAttribute, PsLang.IDLang))
                        {
                            PsAttributeLangRepository.Add(new Model.Prestashop.PsAttributeLang()
                                {
                                    IDAttribute = PsAttribute.IDAttribute,
                                    IDLang = PsLang.IDLang,
                                    Name = NewAttributeValue,
                                });
                        }
                    uint selectedpsattributegroup = (SelectedPsAttributeGroupLang != null) ? SelectedPsAttributeGroupLang.IDAttributeGroup : 0;
                    LoadAttributeGroup();
                    if (selectedpsattributegroup != 0)
                    {
                            SelectedPsAttributeGroupLang = ListPsAttributeGroupLang.FirstOrDefault(ag => ag.IDAttributeGroup == selectedpsattributegroup);
                    }

                    NewAttributeValue = string.Empty;
                }
            }
        }

        #endregion
    }
}
