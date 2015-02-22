using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccelerometerGamePadUtility
{
    public class InterpretationViewModel : ObservableObject
    {
        public InterpretationViewModel(AxisInterpretation type)
        {
            m_type = type;
        }

        private AxisInterpretation m_type;
        public AxisInterpretation Type
        {
            get { return m_type; }
            set { m_type = value; RaisePropertyChanged("Type"); RaisePropertyChanged("Name"); }
        }

        public string Name
        {
            get
            {
                return Global.GetAxisInterpretationName(m_type);
            }
        }
    }
}
