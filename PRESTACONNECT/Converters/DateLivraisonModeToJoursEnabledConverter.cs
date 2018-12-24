using PRESTACONNECT.Core.Parametres;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PRESTACONNECT.Converters
{
	public class DateLivraisonModeToJoursEnabledConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (DateLivraisonMode)value == DateLivraisonMode.DateCommandeInc;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
