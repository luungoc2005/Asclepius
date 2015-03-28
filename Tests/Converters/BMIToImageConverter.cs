using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Asclepius.Converters
{
    public sealed class BMIToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double v = (double)value;
                if (v < 18.5) return new BitmapImage(new Uri("/Resources/images/BMI/bmi1.png",UriKind.Relative));
                else if (v < 24.9) return new BitmapImage(new Uri("/Resources/images/BMI/bmi2.png", UriKind.Relative));
                else if (v < 29.9) return new BitmapImage(new Uri("/Resources/images/BMI/bmi3.png", UriKind.Relative));
                else return new BitmapImage(new Uri("/Resources/images/BMI/bmi4.png",UriKind.Relative));
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
