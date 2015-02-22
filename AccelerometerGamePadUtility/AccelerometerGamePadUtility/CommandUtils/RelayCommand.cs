using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;

namespace AccelerometerGamePadUtility.CommandUtils
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute)
            : base(execute)
        {
        }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        {
        }
    }

    public class RelayCommand<T> : ICommand
    {
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            m_execute = execute;
            m_canExecute = canExecute;
        }

        private Action<T> m_execute;
        private Predicate<T> m_canExecute;
        
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (m_canExecute == null) return true;
            if (parameter == null && typeof(T).IsValueType)
            {
                return m_canExecute(default(T));
            }
            else return m_canExecute((T)parameter);
        }
        public void Execute(object parameter)
        {
            if (m_execute == null) return;

            T param;

            if (parameter == null && typeof(T).IsValueType)
            {
                param = default(T);
            }
            else
            {
                param = (T)parameter;
            }

            if (CanExecute(param))
            {
                m_execute(param);
            }
            //else Debug.Assert(true, "RelayCommand - CanExecute == false while calling Execute method. m_execute not raised");
        }
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, e);
        }
    }
}
