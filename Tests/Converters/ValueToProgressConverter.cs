using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Asclepius.Converters
{
    public sealed class ValueToProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return (Math.Min(System.Convert.ToDouble(value), System.Convert.ToDouble(parameter)) / System.Convert.ToDouble(parameter) * 100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return ((double)value / 100 * (double)parameter);
        }
    }
}
