using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using AccelerometerGamePadUtility.CommandUtils;

namespace AccelerometerGamePadUtility
{
    public sealed class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            m_checkConnectionCommand = new RelayCommand(CheckConnection, CanCheckConnection);
            m_selectAxisCommand = new RelayCommand<AxisType>(SelectAxis);
            m_saveSettingsCommand = new RelayCommand(SaveSettings);
            m_reloadSettingsCommand = new RelayCommand(ReloadSettings);
            m_callibrateAxisCommand = new RelayCommand(CallibrateAxis, CanCallibrateAxis);
            m_defaultSettingsCommand = new RelayCommand(DefaultSettings);

            m_axes = new ObservableCollection<AxisViewModel>();
            m_axes.Add(new AxisViewModel(AxisType.AccelerometerHorizontal));
            m_axes.Add(new AxisViewModel(AxisType.AccelerometerVertical));
            m_axes.Add(new AxisViewModel(AxisType.LeftAnalogHorizontal));
            m_axes.Add(new AxisViewModel(AxisType.LeftAnalogVertical));
            m_axes.Add(new AxisViewModel(AxisType.RightAnalogHorizontal));
            m_axes.Add(new AxisViewModel(AxisType.RightAnalogVertical));

            m_profiles = new ObservableCollection<ProfileViewModel>();
            for (int i = 0; i < 16; i++)
            {
                m_profiles.Add(new ProfileViewModel(i));
            }
        }

        private AccelerometerGamePadDevice m_device;

        private ObservableCollection<AxisViewModel> m_axes;
        public ObservableCollection<AxisViewModel> Axes
        {
            get { return m_axes; }
        }

        private ObservableCollection<ProfileViewModel> m_profiles;
        public ObservableCollection<ProfileViewModel> Profiles
        {
            get { return m_profiles; }
        }

        private AxisViewModel m_selectedAxis;
        public AxisViewModel SelectedAxis
        {
            get { return m_selectedAxis; }
            set { m_selectedAxis = value; RaisePropertyChanged("SelectedAxis"); m_callibrateAxisCommand.RaiseCanExecuteChanged(); }
        }

        private ProfileViewModel m_selectedProfile;
        public ProfileViewModel SelectedProfile
        {
            get { return m_selectedProfile; }
            set
            {
                bool changed = false;
                foreach (var axis in m_axes)
                {
                    if (axis.IsChanged)
                    {
                        changed = true;
                        break;
                    }
                }

                if (changed && Global.MessageBox(this, Global.GetStringResource("LangProfileNotSaved"), MessageBoxSuperPredefinedButtons.YesNo) == MessageBoxSuperButton.No) return;

                m_selectedProfile = value;
                RaisePropertyChanged("SelectedProfile");

                try
                {
                    m_device.SelectProfile(new SelectedProfileReport() { Profile = (byte)m_selectedProfile.Id });
                }
                catch
                {
                    OnErrorOccurred();
                    return;
                }

                m_reloadSettingsCommand.Execute(null);
            }
        }

        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
            private set { m_isConnected = value; RaisePropertyChanged("IsConnected"); m_checkConnectionCommand.RaiseCanExecuteChanged(); }
        }

        private RelayCommand m_saveSettingsCommand;
        public ICommand SaveSettingsCommand
        {
            get { return m_saveSettingsCommand; }
        }

        private RelayCommand<AxisType> m_selectAxisCommand;
        public ICommand SelectAxisCommand
        {
            get { return m_selectAxisCommand; }
        }

        private RelayCommand m_checkConnectionCommand;
        public ICommand CheckConnectionCommand
        {
            get { return m_checkConnectionCommand; }
        }

        private RelayCommand m_reloadSettingsCommand;
        public ICommand ReloadSettingsCommand
        {
            get { return m_reloadSettingsCommand; }
        }

        private RelayCommand m_callibrateAxisCommand;
        public ICommand CallibrateAxisCommand
        {
            get { return m_callibrateAxisCommand; }
        }

        private RelayCommand m_defaultSettingsCommand;
        public ICommand DefaultSettingsCommand
        {
            get { return m_defaultSettingsCommand; }
        }

        private void SaveSettings(object e)
        {
            try
            {
                m_device.SelectProfile(new SelectedProfileReport() { Profile = (byte)m_selectedProfile.Id });
                foreach (var axis in m_axes)
                {
                    m_device.SelectAxis(axis.Type);
                    m_device.SetSettings(new SettingsReport() { Settings = axis.ToAxisSettings() });
                }

                m_device.SaveSettings();
            }
            catch
            {
                OnErrorOccurred();
            }
            finally
            {
                ClearIsChanged();
            }
        }

        private void ReloadSettings(object e)
        {
            try
            {
                foreach (var axis in m_axes)
                {
                    m_device.SelectAxis(axis.Type);
                    AxisSettings sett = m_device.GetSettings().Settings;
                    axis.ApplySettings(sett);
                }
            }
            catch
            {
                OnErrorOccurred();
            }
            finally
            {
                ClearIsChanged();
            }
        }

        private void DefaultSettings(object e)
        {
            if (Global.MessageBox(this, Global.GetStringResource("LangPromptDefaultSettings"), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

            foreach (var axis in m_axes)
            {
                axis.ApplyDefaultSettings();
            }
        }

        private void SelectAxis(AxisType type)
        {
            SelectedAxis = m_axes.FirstOrDefault(x => x.Type == type);
        }

        private void CheckConnection(object e)
        {
            if (!AccelerometerGamePadDevice.IsDeviceConnected())
            {
                OnErrorOccurred();
                return;
            }

            m_device = AccelerometerGamePadDevice.OpenDevice();
            IsConnected = true;

            try
            {
                SelectedProfile = m_profiles.FirstOrDefault(x => x.Id == m_device.GetSelectedProfile().Profile);
            }
            catch
            {
                OnErrorOccurred();
                return;
            }
        }
        private bool CanCheckConnection(object e)
        {
            return !m_isConnected;
        }

        private void CallibrateAxis(object e)
        {
            AxisCallibrationViewModel dialogViewModel = new AxisCallibrationViewModel(m_device, SelectedAxis);
            Global.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result == AxisCallibrationViewModel.CallibrationResult.Ok)
            {
                CallibrationReport report = new CallibrationReport();
                report.Callibration = new AxisCallibration() {ZeroValue = (ushort) dialogViewModel.ZeroValue, MinusOneVariation = (ushort) dialogViewModel.MinusVariation, PlusOneVariation = (ushort) dialogViewModel.PlusVariation};

                m_device.SelectAxis(SelectedAxis.Type);
                m_device.SetCallibration(report);
                m_device.SaveCallibration();
            }
            else if (dialogViewModel.Result == AxisCallibrationViewModel.CallibrationResult.ErrorOccurred)
            {
                OnErrorOccurred();
            }
        }
        private bool CanCallibrateAxis(object e)
        {
            return SelectedAxis != null;
        }

        private void OnErrorOccurred()
        {
            Global.MessageBox(this, Global.GetStringResource("LangErrorOccurredWhileCommunication"), MessageBoxSuperPredefinedButtons.OK);
            if (m_device != null) m_device.CloseDevice();
            SelectedAxis = null;
            IsConnected = false;
        }
        private void ClearIsChanged()
        {
            foreach (var axis in m_axes)
            {
                axis.ClearIsChanged();
            }
        }
    }
}
