using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Local
{
	public partial class ArticleAdditionalField
	{
		public int PsId = 0;

		public enum ArticleAdditionalFieldType
		{
			Rate = 0,
			TextAreaMCE,
			TextArea,
			Text,
			CheckBox,
			Selector,
			Integer,
			Decimal,
			Price,
			Date,
			DateTime,
			Image
		}

		private string fieldNameLang;
		public string FieldNameLang
		{
			get { return fieldNameLang; }
			set { fieldNameLang = value; }
		}

		public static ArticleAdditionalFieldType AffectType(string type)
		{
			switch (type)
			{
				case "textarea_mce":
					return ArticleAdditionalFieldType.TextAreaMCE;
				case "textarea":
					return ArticleAdditionalFieldType.TextArea;
				case "text":
					return ArticleAdditionalFieldType.Text;
				case "checkbox":
					return ArticleAdditionalFieldType.CheckBox;
				case "selector":
					return ArticleAdditionalFieldType.Selector;
				case "integer":
					return ArticleAdditionalFieldType.Integer;
				case "decimal":
					return ArticleAdditionalFieldType.Decimal;
				case "price":
					return ArticleAdditionalFieldType.Price;
				case "date":
					return ArticleAdditionalFieldType.Date;
				case "datetime":
					return ArticleAdditionalFieldType.DateTime;
				case "image":
					return ArticleAdditionalFieldType.Image;
				default:
					return ArticleAdditionalFieldType.Rate;
			}
		}

		public ArticleAdditionalFieldType Type;

		public string FieldValueAll
		{
			get
			{
				if (!string.IsNullOrEmpty(FieldValue2))
					return FieldValue + " | " + FieldValue2;
				else
					return FieldValue;
			}
		}
    }
}
