using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Data;

namespace AccelerometerGamePadUtility.Converters
{
    public class IsSelectedAxisToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Color.FromArgb(0x40, 0xBE, 0xE6, 0xFD));

            AxisViewModel axis = (AxisViewModel)value;
            AxisType checkType = (AxisType)parameter;

            return (axis.Type == checkType ? new SolidColorBrush(Color.FromArgb(0xC0, 0xC4, 0xE5, 0x00)) : new SolidColorBrush(Color.FromArgb(0x40, 0xBE, 0xE6, 0xFD)));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
