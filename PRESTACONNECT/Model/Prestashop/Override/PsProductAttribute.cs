using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Prestashop
{
    partial class PsProductAttribute
    {
        private PsAEcStock aec_stock;
        public PsAEcStock AEC_Stock
        {
            get { return aec_stock; }
            set { aec_stock = value; }
        }
        
        private string combination = string.Empty;
        public string Combination
        {
            get
            {
                Model.Prestashop.PsProductAttributeCombinationRepository PsProductAttributeCombinationRepository = new PsProductAttributeCombinationRepository();
                Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Prestashop.PsAttributeLangRepository();
                Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Prestashop.PsAttributeGroupLangRepository();

                List<PsProductAttributeCombination> list_combination = PsProductAttributeCombinationRepository.ListProductAttribute(this.IDProductAttribute);
                if (string.IsNullOrEmpty(combination) && (list_combination != null && list_combination.Count > 0))
                {
                    string r = string.Empty;
                    foreach (var item in list_combination)
                    {
                        Model.Prestashop.PsAttributeLang PsAttributeLang = PsAttributeLangRepository.ReadAttributeLangFull(item.IDAttribute, Core.Global.Lang);
                        Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang = PsAttributeGroupLangRepository.ReadAttributeGroupLang(PsAttributeLang.id_attribute_group, Core.Global.Lang);
                        if(PsAttributeLang != null)
                        {
                            if (!string.IsNullOrEmpty(r))
                                r += " / ";
                            r += (!string.IsNullOrEmpty(PsAttributeGroupLang.Name)) ? PsAttributeGroupLang.Name : "#GROUP_NULL#";
                            r += " : ";
                            r += (!string.IsNullOrEmpty(PsAttributeLang.Name)) ? PsAttributeLang.Name : "#ATTRIBUTE_NULL#";
                        }
                    }
                    combination = r;
                }
                return combination;
            }
        }
    }
}
