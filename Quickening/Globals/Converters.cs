using System;
using System.Globalization;
using System.Windows.Data;

namespace Quickening.Globals.Converters
{
    public class ElementToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var s = value.ToString();
            if (string.IsNullOrEmpty(s))
                return false;

            return s.ToLower() != Strings.ROOT_TAG;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Two way binding not supported.");
        }
    }
}
