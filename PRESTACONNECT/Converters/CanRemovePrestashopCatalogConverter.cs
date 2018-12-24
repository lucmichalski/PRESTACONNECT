using System;
using System.Windows.Data;
using PRESTACONNECT.Contexts;
using PRESTACONNECT.Model.Local;

namespace PRESTACONNECT.Converters
{
    internal sealed class CanRemovePrestashopCatalogConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            else
            {
                Catalog catalog = value as Catalog;
                return catalog.CanRemovePrestashop;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
