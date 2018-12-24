using PRESTACONNECT.Core.Parametres;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PRESTACONNECT.Converters
{
	public class DateLivraisonModeToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((DateLivraisonMode)value)
			{
				case DateLivraisonMode.DateLivraison:
					return (string)parameter == "DateLivraison";
				case DateLivraisonMode.DateCommandeInc:
				default:
					return (string)parameter == "DateCommandeInc";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((string)parameter)
			{
				case "DateLivraison":
					return (bool)value ? DateLivraisonMode.DateLivraison : DateLivraisonMode.DateCommandeInc;
				case "DateCommandeInc":
				default:
					return (bool)value ? DateLivraisonMode.DateCommandeInc : DateLivraisonMode.DateLivraison;
			}
		}
	}
}
