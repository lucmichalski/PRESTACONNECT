using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsFeatureValue
    {
        public string Value
        {
            get
            {
                string value = "#";
                Model.Prestashop.PsFeatureValueLangRepository PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                if (PsFeatureValueLangRepository.ExistFeatureValueLang(IDFeatureValue, Core.Global.Lang))
                {
                    Model.Prestashop.PsFeatureValueLang PsFeatureValueLang = PsFeatureValueLangRepository.ReadFeatureValueLang(IDFeatureValue, Core.Global.Lang);
                    value = PsFeatureValueLang.Value;
                }
                return value;
            }
        }

        public string ComboText
        {
            get
            {
                return IDFeatureValue + " - " + Value;
            }
        }

        #region Methods

        public override string ToString()
        {
            return ComboText;
        }

        #endregion
    }
}
