using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace AccelerometerGamePadUtility
{
    public static class Global
    {
        static Global()
        {
            Dictionary<Type, Func<object, Window>> windowViewModelMapping = new Dictionary<Type, Func<object, Window>>();

            windowViewModelMapping.Add(typeof(AxisCallibrationViewModel), x => new AxisCallibrationWindow((AxisCallibrationViewModel)x));

            m_dialogs = new DialogService(windowViewModelMapping);

            s_interpretationNull = new InterpretationViewModel(AxisInterpretation.InterpretationNull);

            s_interpretations = new ObservableCollection<InterpretationViewModel>();
            s_interpretations.Add(s_interpretationNull);
            s_interpretations.Add(new InterpretationViewModel(AxisInterpretation.InterpretationXAxis));
            s_interpretations.Add(new InterpretationViewModel(AxisInterpretation.InterpretationXRotation));
            s_interpretations.Add(new InterpretationViewModel(AxisInterpretation.InterpretationYAxis));
            s_interpretations.Add(new InterpretationViewModel(AxisInterpretation.InterpretationYRotation));
            s_interpretations.Add(new InterpretationViewModel(AxisInterpretation.InterpretationZAxis));
            s_interpretations.Add(new InterpretationViewModel(AxisInterpretation.InterpretationZRotation));
        }

        private static DialogService m_dialogs;
        public static DialogService Dialogs
        {
            get { return m_dialogs; }
        }

        private static ObservableCollection<InterpretationViewModel> s_interpretations;
        public static ObservableCollection<InterpretationViewModel> Interpretations
        {
            get { return s_interpretations; }
        }

        public static InterpretationViewModel s_interpretationNull;
        public static InterpretationViewModel InterpretationNull
        {
            get { return s_interpretationNull; }
        }

        public static string GetStringResource(object key)
        {
            object result = Application.Current.TryFindResource(key);
            return (result == null ? key + " NOT FOUND - RESOURCE ERROR" : (string)result);
        }
        public static MessageBoxSuperButton MessageBox(object viewModel, string text, MessageBoxSuperPredefinedButtons buttons)
        {
            return MessageBoxSuper.ShowBox(Dialogs.GetWindow(viewModel), text, GetStringResource("LangAppName"), buttons);
        }

        public static string GetAxisInterpretationName(AxisInterpretation interpretation)
        {
            switch (interpretation)
            {
                case AxisInterpretation.InterpretationNull: return Global.GetStringResource("LangNullAxis");
                case AxisInterpretation.InterpretationXAxis: return Global.GetStringResource("LangXAxis");
                case AxisInterpretation.InterpretationXRotation: return Global.GetStringResource("LangXRotationAxis");
                case AxisInterpretation.InterpretationYAxis: return Global.GetStringResource("LangYAxis");
                case AxisInterpretation.InterpretationYRotation: return Global.GetStringResource("LangYRotationAxis");
                case AxisInterpretation.InterpretationZAxis: return Global.GetStringResource("LangZAxis");
                case AxisInterpretation.InterpretationZRotation: return Global.GetStringResource("LangZRotationAxis");
            }

            return "N/A";
        }
    }
}
