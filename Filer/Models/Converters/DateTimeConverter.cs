using System;
using System.Globalization;
using System.Windows.Data;

namespace Filer.Models.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null
                ? default(DateTime).ToString("yy/MM/dd HH:mm")
                : ((DateTime)value).ToString("yy/MM/dd HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}