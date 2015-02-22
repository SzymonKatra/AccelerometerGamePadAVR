using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccelerometerGamePadUtility
{
    public sealed class ProfileViewModel : ObservableObject
    {
        public ProfileViewModel(int id)
        {
            m_id = id;
        }

        private readonly int m_id;
        public int Id
        {
            get { return m_id; }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; RaisePropertyChanged("Name"); RaisePropertyChanged("Displayed"); }
        }

        public string Displayed
        {
            get
            {
                return string.Format("{0}: {1}", (m_id + 1).ToString(), m_name);
            }
        }
    }
}
