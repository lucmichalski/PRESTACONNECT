using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public partial class Group
    {
        public String ComboText
        {
            get
            {
                string t = "Indisponible";
                Model.Prestashop.PsGroupLangRepository PsGroupLang_Repository = new Prestashop.PsGroupLangRepository();
                if (PsGroupLang_Repository.Exist(Core.Global.Lang, (uint)this.Grp_Pre_Id))
                {
                    t = PsGroupLang_Repository.Read(Core.Global.Lang, (uint)this.Grp_Pre_Id).Name;

                    Model.Sage.P_CATTARIFRepository P_CATTARIF_Repository = new Sage.P_CATTARIFRepository();
                    if (this.Grp_CatTarifId != null && P_CATTARIF_Repository.Read((ushort)this.Grp_CatTarifId) != null)
                    {
                        t += " <> " + P_CATTARIF_Repository.Read((ushort)this.Grp_CatTarifId).CT_Intitule;
                    }
                }
                return t;
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
