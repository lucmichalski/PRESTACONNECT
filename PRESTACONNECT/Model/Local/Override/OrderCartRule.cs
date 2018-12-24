using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Local
{
    public partial class OrderCartRule
    {
		public string PrestaShopCode
		{
			get
			{
				if (Pre_id > 0 && new Model.Prestashop.PsCartRuleRepository().ExistCartRule((uint)(Pre_id)))
					return (new Model.Prestashop.PsCartRuleRepository().ReadCartRule((uint)(Pre_id))).Code;
				else
					return "";
			}
		}

		public string SageArticle
		{
			get
			{
				if (Sag_id != null && Sag_id > 0 && new Model.Sage.F_ARTICLERepository().ExistArticle(Sag_id ?? 0))
					return (new Model.Sage.F_ARTICLERepository().ReadArticle(Sag_id ?? 0)).AR_Ref;
				else
					return "";
			}
		}
	}
}
