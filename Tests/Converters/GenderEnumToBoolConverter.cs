using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Asclepius.Converters
{
    public class GenderEnumToBoolConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return (value.ToString()==parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            if (((bool)value) == true)
            {
                return parameter;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
