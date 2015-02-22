using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccelerometerGamePadUtility
{
    public class AxisViewModel : ObservableObject
    {
        public AxisViewModel()
        {
        }
        public AxisViewModel(AxisType type)
        {
            m_type = type;
        }
        public AxisViewModel(AxisType type, AxisSettings settings)
        {
            m_type = type;
            ApplySettings(settings);
        }

        private AxisType m_type;
        public AxisType Type
        {
            get { return m_type; }
            private set { m_type = value; RaisePropertyChanged("Type"); RaisePropertyChanged("Name"); }
        }

        private bool m_isChanged;
        public bool IsChanged
        {
            get { return m_isChanged; }
            private set { m_isChanged = value; RaisePropertyChanged("IsChanged"); }
        }

        private int m_zeroDeadZone;
        public int ZeroDeadZone
        {
            get { return m_zeroDeadZone; }
            set
            {
                m_zeroDeadZone = value;
                if (m_zeroDeadZone > m_oneDeadZone) m_zeroDeadZone = m_oneDeadZone;
                IsChanged = true;
                RaisePropertyChanged("ZeroDeadZone");
            }
        }

        private int m_oneDeadZone;
        public int OneDeadZone
        {
            get { return m_oneDeadZone; }
            set
            {
                m_oneDeadZone = value;
                if (m_oneDeadZone < m_zeroDeadZone) m_oneDeadZone = m_zeroDeadZone;
                IsChanged = true;
                RaisePropertyChanged("OneDeadZone");
            }
        }

        private bool m_truncate;
        public bool Truncate
        {
            get { return m_truncate; }
            set { m_truncate = value; IsChanged = true; RaisePropertyChanged("Truncate"); }
        }

        private bool m_invert;
        public bool Invert
        {
            get { return m_invert; }
            set { m_invert = value; IsChanged = true; RaisePropertyChanged("Invert"); }
        }

        private InterpretationViewModel m_interpretation;
        public InterpretationViewModel Interpretation
        {
            get { return m_interpretation; }
            set { m_interpretation = value; IsChanged = true; RaisePropertyChanged("Interpretation"); }
        }

        public string Name
        {
            get
            {
                switch(m_type)
                {
                    case AxisType.AccelerometerHorizontal: return Global.GetStringResource("LangAccelerometerHorizontal");
                    case AxisType.AccelerometerVertical: return Global.GetStringResource("LangAccelerometerVertical");
                    case AxisType.LeftAnalogHorizontal: return Global.GetStringResource("LangLeftAnalogHorizontal");
                    case AxisType.LeftAnalogVertical: return Global.GetStringResource("LangLeftAnalogVertical");
                    case AxisType.RightAnalogHorizontal: return Global.GetStringResource("LangRightAnalogHorizontal");
                    case AxisType.RightAnalogVertical: return Global.GetStringResource("LangRightAnalogVertical");
                }

                return string.Empty;
            }
        }

        public void ClearIsChanged()
        {
            IsChanged = false;
        }

        public void ApplyDefaultSettings()
        {
            switch(m_type)
            {
                case AxisType.AccelerometerHorizontal:
                    {
                        m_zeroDeadZone = 20;
                        m_oneDeadZone = 80;
                        m_truncate = false;
                        m_invert = false;
                        m_interpretation = Global.Interpretations.FirstOrDefault(x => x.Type == AxisInterpretation.InterpretationXAxis);
                    }
                    break;

                case AxisType.AccelerometerVertical:
                    {
                        m_zeroDeadZone = 20;
                        m_oneDeadZone = 80;
                        m_truncate = false;
                        m_invert = false;
                        m_interpretation = Global.Interpretations.FirstOrDefault(x => x.Type == AxisInterpretation.InterpretationYAxis);
                    }
                    break;

                case AxisType.LeftAnalogHorizontal:
                    {
                        m_zeroDeadZone = 5;
                        m_oneDeadZone = 95;
                        m_truncate = false;
                        m_invert = false;
                        m_interpretation = Global.Interpretations.FirstOrDefault(x => x.Type == AxisInterpretation.InterpretationZAxis);
                    }
                    break;

                case AxisType.LeftAnalogVertical:
                    {
                        m_zeroDeadZone = 5;
                        m_oneDeadZone = 95;
                        m_truncate = false;
                        m_invert = false;
                        m_interpretation = Global.Interpretations.FirstOrDefault(x => x.Type == AxisInterpretation.InterpretationZRotation);
                    }
                    break;

                case AxisType.RightAnalogHorizontal:
                    {
                        m_zeroDeadZone = 5;
                        m_oneDeadZone = 95;
                        m_truncate = false;
                        m_invert = false;
                        m_interpretation = Global.Interpretations.FirstOrDefault(x => x.Type == AxisInterpretation.InterpretationXRotation);
                    }
                    break;

                case AxisType.RightAnalogVertical:
                    {
                        m_zeroDeadZone = 5;
                        m_oneDeadZone = 95;
                        m_truncate = false;
                        m_invert = false;
                        m_interpretation = Global.Interpretations.FirstOrDefault(x => x.Type == AxisInterpretation.InterpretationYRotation);
                    }
                    break;
            }

            if (m_interpretation == null) m_interpretation = Global.InterpretationNull;
            RaiseAllSettingsChanged();
        }

        public void ApplySettings(AxisSettings settings)
        {
            m_zeroDeadZone = settings.ZeroDeadZoneRaw;
            m_oneDeadZone = settings.OneDeadZoneRaw;
            m_truncate = ((settings.Flags & (byte)AxisSettingsFlags.Truncate) > 0);
            m_invert = ((settings.Flags & (byte)AxisSettingsFlags.Invert) > 0);
            m_interpretation = Global.Interpretations.FirstOrDefault(x => x.Type == settings.Interpretation);
            if (m_interpretation == null) m_interpretation = Global.InterpretationNull;

            RaiseAllSettingsChanged();
        }

        public AxisSettings ToAxisSettings()
        {
            AxisSettings result = new AxisSettings();
            result.ZeroDeadZoneRaw = (byte)m_zeroDeadZone;
            result.OneDeadZoneRaw = (byte)m_oneDeadZone;
            if (m_truncate) result.Flags |= (byte)AxisSettingsFlags.Truncate;
            if (m_invert) result.Flags |= (byte)AxisSettingsFlags.Invert;
            result.Interpretation = m_interpretation.Type;

            return result;
        }

        private void RaiseAllSettingsChanged()
        {
            RaisePropertyChanged("ZeroDeadZone");
            RaisePropertyChanged("OneDeadZone");
            RaisePropertyChanged("Truncate");
            RaisePropertyChanged("Invert");
            RaisePropertyChanged("Interpretation");
        }
    }
}
