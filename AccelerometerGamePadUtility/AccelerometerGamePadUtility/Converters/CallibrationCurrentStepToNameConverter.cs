using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AccelerometerGamePadUtility.Converters
{
    public class CallibrationCurrentStepToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CallibrationCurrentStep step = (CallibrationCurrentStep)value;

            switch(step)
            {
                case CallibrationCurrentStep.Ready: return Global.GetStringResource("LangCallibrationReady");
                case CallibrationCurrentStep.PlaceInZero: return Global.GetStringResource("LangCallibrationPlaceInZero");
                case CallibrationCurrentStep.AveragingResult: return Global.GetStringResource("LangCallibrationAveragingResult");
                case CallibrationCurrentStep.Tilt: return Global.GetStringResource("LangCallibrationTilt");
                case CallibrationCurrentStep.Done: return Global.GetStringResource("LangCallibrationDone");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
