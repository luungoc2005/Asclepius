using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Asclepius.Converters
{
    public sealed class BMIToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double v = (double)value;
                string p = (string)parameter;
                string ret;
                if (v<18.5) ret="Underweight";
                else if (v<24.9) ret="Normal";
                else if (v<29.9) ret="Overweight";
                else ret="Obese";
                return String.Format(p, ret);
            }
            catch
            {
                return parameter.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }
    }
}
