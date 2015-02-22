using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AccelerometerGamePadUtility.Converters
{
    public class InterpretationViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AxisInterpretation interpretation = (AxisInterpretation)value;
            IEnumerable<InterpretationViewModel> viewModelEnumerable = (IEnumerable<InterpretationViewModel>)parameter;

            return viewModelEnumerable.FirstOrDefault(x => x.Type == interpretation);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            InterpretationViewModel viewModel = (InterpretationViewModel)value;
            return viewModel.Type;
        }
    }
}
