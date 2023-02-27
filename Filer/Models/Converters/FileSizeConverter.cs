using System;
using System.Globalization;
using System.Windows.Data;

namespace Filer.Models.Converters
{
    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileSize = (long)value;

            const int kiloByte = 1000;
            const int megaByte = kiloByte * kiloByte;

            if (fileSize >= megaByte)
            {
                return Math.Floor((double)fileSize / megaByte) + " M";
            }

            if (fileSize >= kiloByte)
            {
                return Math.Floor((double)fileSize / kiloByte) + " K";
            }

            return fileSize + " B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}