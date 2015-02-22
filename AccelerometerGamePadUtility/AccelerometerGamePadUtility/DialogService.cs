using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AccelerometerGamePadUtility
{
    public class ViewRegisteredEventArgs : EventArgs
    {
        public Window View { get; set; }

        public ViewRegisteredEventArgs(Window view)
        {
            this.View = view;
        }
    }

    public class DialogService
    {
        public DialogService()
            : this(new Dictionary<Type, Func<object, Window>>())
        {
        }
        public DialogService(Dictionary<Type, Func<object, Window>> windowViewModelMappings)
        {
            m_windowViewModelMappings = windowViewModelMappings;
        }

        private Dictionary<Type, Func<object, Window>> m_windowViewModelMappings;
        public Dictionary<Type, Func<object, Window>> WindowViewModelMappings
        {
            get { return m_windowViewModelMappings; }
            set { m_windowViewModelMappings = value; }
        }

        private Dictionary<object, Window> m_registeredViews = new Dictionary<object, Window>();
        private Dictionary<object, Window> m_openedWindows = new Dictionary<object, Window>();

        public event EventHandler<ViewRegisteredEventArgs> ViewRegistered;
        public event EventHandler<ViewRegisteredEventArgs> ViewUnregistered;

        public bool IsViewModelRegistered(object viewModel)
        {
            return m_registeredViews.ContainsKey(viewModel);
        }
        public bool IsViewRegistered(Window view)
        {
            return m_registeredViews.ContainsValue(view);
        }
        public void Register(Window view, object viewModel, bool unregisterOnClose = true)
        {
            if (m_registeredViews.ContainsKey(viewModel)) throw new ArgumentException("This ViewModel is already registered");
            if (m_registeredViews.ContainsValue(view)) throw new ArgumentException("This View is already registered");

            m_registeredViews.Add(viewModel, view);
            OnViewRegistered(new ViewRegisteredEventArgs(view));
            if (unregisterOnClose) view.Closed += view_Closed;
        }
        public void Unregister(Window view)
        {
            UnregisterViaViewModel(m_registeredViews.First((kvp) => (kvp.Value == view)).Key);
        }
        public void UnregisterViaViewModel(object viewModel)
        {
            Window view = m_registeredViews[viewModel];
            m_registeredViews.Remove(viewModel);
            OnViewUnregistered(new ViewRegisteredEventArgs(view));
        }

        public Window GetWindow(object viewModel)
        {
            if (!m_registeredViews.ContainsKey(viewModel)) throw new ArgumentException("ViewModel not registered");
            return m_registeredViews[viewModel];
        }
        public object GetViewModel(Window view)
        {
            if (!m_registeredViews.ContainsValue(view)) throw new ArgumentException("View not registered");
            return m_registeredViews.First(kvp => (kvp.Value == view)).Key;
        }
        public object GetActiveViewModel(object def)
        {
            Window view = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            if (view != null && IsViewRegistered(view))
            {
                return GetViewModel(view);
            }
            else
            {
                return def;
            }
        }

        public bool? ShowDialog(object ownerViewModel, object viewModel)
        {
            if (m_openedWindows.ContainsKey(viewModel)) throw new ArgumentException("This ViewModel is already opened");

            Type viewModelType = viewModel.GetType();

            if (!m_windowViewModelMappings.ContainsKey(viewModelType)) throw new ArgumentException("No function to create View attached to this type of ViewModel");

            Window dialogView = m_windowViewModelMappings[viewModelType](viewModel);
            dialogView.Owner = GetWindow(ownerViewModel);

            m_openedWindows.Add(viewModel, dialogView);

            bool? result = dialogView.ShowDialog();

            m_openedWindows.Remove(viewModel);

            return result;
        }
        public void Close(object viewModel)
        {
            if (!m_openedWindows.ContainsKey(viewModel)) throw new ArgumentException("No opened View for this ViewModel");
            
            m_openedWindows[viewModel].Close();
        }

        private void view_Closed(object sender, EventArgs e)
        {
            Window view = sender as Window;
            if (view == null) return;

            view.Closed -= view_Closed;
            Unregister(view);
        }

        protected virtual void OnViewRegistered(ViewRegisteredEventArgs e)
        {
            EventHandler<ViewRegisteredEventArgs> handler = ViewRegistered;
            if (handler != null) handler(this, e);
        }
        protected virtual void OnViewUnregistered(ViewRegisteredEventArgs e)
        {
            EventHandler<ViewRegisteredEventArgs> handler = ViewUnregistered;
            if (handler != null) handler(this, e);
        }
    }
}
