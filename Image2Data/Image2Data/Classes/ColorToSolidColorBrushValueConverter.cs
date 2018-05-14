using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Image2Data.Classes
{
    public class ColorToSolidColorBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }
            // For a more sophisticated converter, check also the targetType and react accordingly..
            if (value is System.Drawing.Color)
            {
                System.Drawing.Color color = (System.Drawing.Color)value;
                var goodColor = new System.Windows.Media.Color();
                goodColor.A = color.A;
                goodColor.R = color.R;
                goodColor.G = color.G;
                goodColor.B = color.B;
                return new SolidColorBrush(goodColor);
            }
            else if (value is string)
            {
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml((string) value);
                var goodColor = new System.Windows.Media.Color();
                goodColor.A = color.A;
                goodColor.R = color.R;
                goodColor.G = color.G;
                goodColor.B = color.B;
                return new SolidColorBrush(goodColor);
            }
            // You can support here more source types if you wish
            // For the example I throw an exception

            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
