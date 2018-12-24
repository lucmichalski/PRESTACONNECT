using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsFeatureLang
    {
        private bool _HasUpdated = false;
        public bool HasUpdated
        {
            get { return _HasUpdated; }
            set { _HasUpdated = value; SendPropertyChanged("HasUpdated"); }
        }

        public string ComboText
        {
            get
            {
                return IDFeature + " - " + Name;
            }
        }

        private string filterPsFeatureValueLang = string.Empty;
        public string FilterPsFeatureValueLang
        {
            get { return filterPsFeatureValueLang; }
            set
            {
                filterPsFeatureValueLang = value;
                if (string.IsNullOrEmpty(filterPsFeatureValueLang))
                    FilterFeatureValue();
                SendPropertyChanged("FilterPsFeatureValueLang");
                SendPropertyChanged("ListPsFeatureValueLang");
            }
        }

        public void FilterFeatureValue()
        {
            if (!string.IsNullOrEmpty(FilterPsFeatureValueLang))
                listPsFeatureValueLang = _tempListPsFeatureValueLang.Where(fv => fv.Value.Contains(FilterPsFeatureValueLang));
            else
                listPsFeatureValueLang = _tempListPsFeatureValueLang;

            SendPropertyChanged("ListPsFeatureValueLang");
        }

        private IEnumerable<PsFeatureValueLang> _tempListPsFeatureValueLang;
        private IEnumerable<PsFeatureValueLang> listPsFeatureValueLang;
        public IEnumerable<PsFeatureValueLang> ListPsFeatureValueLang
        {
            get
            {
                if (Core.Temp.PsFeatureValueLangRepository == null)
                {
                    Core.Temp.PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                }
                if (_tempListPsFeatureValueLang == null)
                {
                    _tempListPsFeatureValueLang = Core.Temp.PsFeatureValueLangRepository.ListFeatureLang(IDFeature, Core.Global.Lang);
                    listPsFeatureValueLang = _tempListPsFeatureValueLang;
                }

                return listPsFeatureValueLang;
            }
            set
            {
                ListPsFeatureValueLang = value;
                SendPropertyChanged("ListPsFeatureValueLang");
            }
        }

        public Model.Local.Characteristic Characteristic
        {
            get
            {
                Model.Local.Characteristic r = null;
                Model.Local.CharacteristicRepository CharacteristicRepository = new Local.CharacteristicRepository();
                if (Core.Temp.Article != 0 && CharacteristicRepository.ExistFeatureArticle((int)IDFeature, Core.Temp.Article))
                {
                    r = CharacteristicRepository.ReadFeatureArticle((int)IDFeature, Core.Temp.Article);
                }
                return r;
            }
            set
            {
                //SendPropertyChanged("PsFeatureValueLang");
                //SendPropertyChanged("PsFeatureValueLangCustom");
            }
        }

        private Model.Prestashop.PsFeatureValueLang psFeatureValueLang;
        public Model.Prestashop.PsFeatureValueLang PsFeatureValueLang
        {
            get
            {
                if (Core.Temp.PsFeatureValueLangRepository == null)
                {
                    Core.Temp.PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                }
                if (PsFeatureValueLangCustom == null || string.IsNullOrEmpty(PsFeatureValueLangCustom.Value))
                {
                    if (psFeatureValueLang == null)
                    {
                        if (Characteristic != null && Characteristic.Cha_Custom == false && Characteristic.Pre_Id != null)
                        {
                            psFeatureValueLang = Core.Temp.PsFeatureValueLangRepository.ReadFeatureValueLang((uint)Characteristic.Pre_Id, Core.Global.Lang); //Core.Temp.ListPsFeatureValueLang.FirstOrDefault(fv => fv.IDFeatureValue == Characteristic.Pre_Id);
                        }
                    }
                }
                return psFeatureValueLang;
            }
            set
            {
                psFeatureValueLang = value;
                if (value != null && value.IDFeatureValue != 0)
                    PsFeatureValueLangCustom = new Prestashop.PsFeatureValueLang();
                HasUpdated = true;
                SendPropertyChanged("PsFeatureValueLang");
            }
        }

        private Model.Prestashop.PsFeatureValueLang psFeatureValueLangCustom;
        public Model.Prestashop.PsFeatureValueLang PsFeatureValueLangCustom
        {
            get
            {
                if (Core.Temp.PsFeatureValueLangRepository == null)
                {
                    Core.Temp.PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                }
                if (psFeatureValueLangCustom == null)
                    if (Characteristic != null && Characteristic.Cha_Custom == true)
                    {
                        if (Characteristic.Pre_Id != null)
                        {
                            psFeatureValueLangCustom = Core.Temp.PsFeatureValueLangRepository.ReadFeatureValueLang((uint)Characteristic.Pre_Id, Core.Global.Lang);//Core.Temp.ListPsFeatureValueLang.FirstOrDefault(fv => fv.IDFeatureValue == Characteristic.Pre_Id);
                        }
                        // si énuméré en attente de synchronisation
                        else
                        {
                            psFeatureValueLangCustom = new PsFeatureValueLang()
                            {
                                custom = 1,
                                id_feature = IDFeature,
                                IDFeatureValue = 0,
                                IDLang = Core.Global.Lang,
                                Value = Characteristic.Cha_Value,
                            };
                        }
                    }
                if (psFeatureValueLangCustom == null)
                    psFeatureValueLangCustom = new PsFeatureValueLang();
                return psFeatureValueLangCustom;
            }
            set
            {
                psFeatureValueLangCustom = value;
                SendPropertyChanged("PsFeatureValueLangCustom");
                SendPropertyChanged("CustomValue");
            }
        }

        public String CustomValue
        {
            get { return PsFeatureValueLangCustom.Value; }
            set
            {
                PsFeatureValueLangCustom.Value = value;
                if (!string.IsNullOrEmpty(value))
                    PsFeatureValueLang = null;
                HasUpdated = true;
                SendPropertyChanged("CustomValue");
                SendPropertyChanged("PsFeatureValueLangCustom");
            }
        }

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
