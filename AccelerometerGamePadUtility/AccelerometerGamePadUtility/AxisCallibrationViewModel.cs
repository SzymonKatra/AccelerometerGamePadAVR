using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using AccelerometerGamePadUtility.CommandUtils;
using System.Windows.Input;

namespace AccelerometerGamePadUtility
{
    public enum CallibrationCurrentStep
    {
        Ready,
        PlaceInZero,
        AveragingResult,
        Tilt,
        Done,
    }

    public class AxisCallibrationViewModel : ObservableObject
    {
        public enum CallibrationResult
        {
            Ok,
            Cancel,
            ErrorOccurred,
        }

        public AxisCallibrationViewModel(AccelerometerGamePadDevice device, AxisViewModel axis)
        {
            m_nextCommand = new RelayCommand(Next, CanNext);
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);

            m_device = device;
            m_axis = axis;

            m_device.SelectAxis(m_axis.Type);

            m_timer = new Timer(TimerUpdateTime);
            m_timer.AutoReset = true;
            m_timer.Elapsed += m_timer_Elapsed;
            m_timer.Start();
        }

        private const double TimerUpdateTime = 25.0;
        private const double AveragingTime = 2000.0;

        private CallibrationResult m_result = CallibrationResult.Cancel;
        public CallibrationResult Result
        {
            get { return m_result; }
        }

        private Timer m_timer;
        private double m_timerElapsed = 0.0;
        private int m_averageSum = 0;
        private int m_averageCount = 0;

        private AccelerometerGamePadDevice m_device;

        private AxisViewModel m_axis;
        public AxisViewModel Axis
        {
            get { return m_axis; }
        }

        private RelayCommand m_nextCommand;
        public ICommand NextCommand
        {
            get { return m_nextCommand; }
        }

        private RelayCommand m_okCommand;
        public ICommand OkCommand
        {
            get { return m_okCommand; }
        }

        private RelayCommand m_cancelCommand;
        public ICommand CancelCommand
        {
            get { return m_cancelCommand; }
        }

        private int m_currentValue;
        public int CurrentValue
        {
            get { return m_currentValue; }
            private set { m_currentValue = value; RaisePropertyChanged("CurrentValue"); }
        }

        private CallibrationCurrentStep m_currentStep = CallibrationCurrentStep.Ready;
        public CallibrationCurrentStep CurrentStep
        {
            get { return m_currentStep; }
            private set { m_currentStep = value; RaisePropertyChanged("CurrentStep"); m_nextCommand.RaiseCanExecuteChanged(); m_okCommand.RaiseCanExecuteChanged(); }
        }

        private int m_zeroValue;
        public int ZeroValue
        {
            get { return m_zeroValue; }
            set { m_zeroValue = value; RaisePropertyChanged("ZeroValue"); }
        }

        private int m_minusVariation = 0;
        public int MinusVariation
        {
            get { return m_minusVariation; }
            set { m_minusVariation = value; RaisePropertyChanged("MinusVariation"); }
        }

        private int m_plusVariation = 0;
        public int PlusVariation
        {
            get { return m_plusVariation; }
            set { m_plusVariation = value; RaisePropertyChanged("PlusVariation"); }
        }

        private void Next(object e)
        {
            CurrentStep = (CallibrationCurrentStep)((int)CurrentStep + 1);
        }
        private bool CanNext(object e)
        {
            return m_currentStep != CallibrationCurrentStep.Done && m_currentStep != CallibrationCurrentStep.AveragingResult;
        }
        private void Ok(object e)
        {
            m_result = CallibrationResult.Ok;
            Global.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            return m_currentStep == CallibrationCurrentStep.Done;
        }
        private void Cancel(object e)
        {
            m_result = CallibrationResult.Cancel;
            Global.Dialogs.Close(this);
        }

        private void m_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Global.Dialogs.IsViewModelRegistered(this)) // check is window closed
            {
                m_timer.Stop();
                return;
            }

            Global.Dialogs.GetWindow(this).Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    CurrentValue = m_device.GetRawAxis().Value;
                }
                catch
                {
                    m_result = CallibrationResult.ErrorOccurred;
                    if (Global.Dialogs.IsViewModelRegistered(this)) // check is window closed
                    {
                        m_timer.Stop();
                        Global.Dialogs.Close(this);
                        return;
                    }
                }

                if (CurrentStep == CallibrationCurrentStep.PlaceInZero)
                {
                    ZeroValue = CurrentValue;
                }
                else if (CurrentStep == CallibrationCurrentStep.AveragingResult)
                {
                    m_timerElapsed += TimerUpdateTime;

                    m_averageSum += CurrentValue;
                    m_averageCount++;

                    if (m_timerElapsed >= AveragingTime)
                    {
                        ZeroValue = (int)Math.Round((double)m_averageSum / (double)m_averageCount);
                        CurrentStep = CallibrationCurrentStep.Tilt;
                    }
                }
                else if (CurrentStep == CallibrationCurrentStep.Tilt)
                {
                    int val = CurrentValue - ZeroValue;
                    int absVal = Math.Abs(val);
                    if (val < 0 && absVal > MinusVariation) MinusVariation = absVal;
                    else if (val >= 0 && absVal > PlusVariation) PlusVariation = absVal;
                }
            }));
        }
    }
}
