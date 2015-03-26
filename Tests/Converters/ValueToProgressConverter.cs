using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Asclepius.Converters
{
    public class ValueToProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            try
            {
                return (Math.Min((double)value, (double)parameter) / (double)parameter * 100);
            }
            catch
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return ((double)value / 100 * (double)parameter);
        }
    }
}
