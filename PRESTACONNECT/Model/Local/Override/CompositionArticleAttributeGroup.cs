using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public partial class CompositionArticleAttributeGroup
    {
        public Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang
        {
            get
            {
                Model.Prestashop.PsAttributeGroupLang r = new Prestashop.PsAttributeGroupLang();
                if (this.Cag_AttributeGroup_PreId != 0)
                {
                    Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Prestashop.PsAttributeGroupLangRepository();
                    if (PsAttributeGroupLangRepository.ExistAttributeGroupLang((uint)this.Cag_AttributeGroup_PreId, Core.Global.Lang))
                    {
                        r = PsAttributeGroupLangRepository.ReadAttributeGroupLang((uint)this.Cag_AttributeGroup_PreId, Core.Global.Lang);
                    }
                }
                return r;
            }
        }
    }
}