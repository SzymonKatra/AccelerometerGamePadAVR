using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AccelerometerGamePadUtility.CommandUtils;

namespace AccelerometerGamePadUtility
{
    public enum MessageBoxSuperButton
    {
        Abort,
        Retry,
        Ignore,
        OK,
        Cancel,
        Yes,
        No,
    }

    public enum MessageBoxSuperPredefinedButtons
    {
        AbortRetryIgnore,
        OK,
        OKCancel,
        RetryCancel,
        YesNo,
        YesNoCancel,
    }

    /// <summary>
    /// Interaction logic for MessageBoxSuper.xaml
    /// </summary>
    public partial class MessageBoxSuper : Window
    {
        public class MappedButtonContent
        {
            private MessageBoxSuperButton m_button;
            public MessageBoxSuperButton Button
            {
                get { return m_button; }
            }

            private string m_content;
            public string Content
            {
                get { return m_content; }
            }

            public MappedButtonContent(MessageBoxSuperButton button, string content)
            {
                m_button = button;
                m_content = content;
            }
        }

        
         public MessageBoxSuper(string text, string caption, List<MessageBoxSuperButton> buttons)
            : this(text, caption, buttons, DefaultButtonsContents)
        {
        }
        public MessageBoxSuper(string text, string caption, List<MessageBoxSuperButton> buttons, Dictionary<MessageBoxSuperButton, string> buttonsContents)
        {
            m_clickCommand = new RelayCommand<MessageBoxSuperButton>(Click);

            m_text = text;
            m_caption = caption;

            foreach (MessageBoxSuperButton button in buttons)
            {
                m_buttons.Add(new MappedButtonContent(button, buttonsContents[button]));
            }

            this.DataContext = this;

            InitializeComponent();
        }

        private static Dictionary<MessageBoxSuperButton, string> m_defaultButtonsContents;
        protected static Dictionary<MessageBoxSuperButton, string> DefaultButtonsContents
        {
            get
            {
                if (m_defaultButtonsContents == null)
                {
                    m_defaultButtonsContents = new Dictionary<MessageBoxSuperButton, string>();
                    m_defaultButtonsContents.Add(MessageBoxSuperButton.Abort, "Przerwij");
                    m_defaultButtonsContents.Add(MessageBoxSuperButton.Retry, "Ponów próbę");
                    m_defaultButtonsContents.Add(MessageBoxSuperButton.Ignore, "Ignoruj");
                    m_defaultButtonsContents.Add(MessageBoxSuperButton.OK, "OK");
                    m_defaultButtonsContents.Add(MessageBoxSuperButton.Cancel, "Anuluj");
                    m_defaultButtonsContents.Add(MessageBoxSuperButton.Yes, "Tak");
                    m_defaultButtonsContents.Add(MessageBoxSuperButton.No, "Nie");
                }

                return m_defaultButtonsContents;
            }
        }

        private ObservableCollection<MappedButtonContent> m_buttons = new ObservableCollection<MappedButtonContent>();
        public ObservableCollection<MappedButtonContent> Buttons
        {
            get { return m_buttons; }
        }

        private string m_text;
        public string Text
        {
            get { return m_text; }
        }

        private string m_caption;
        public string Caption
        {
            get { return m_caption; }
        }

        private MessageBoxSuperButton m_result;
        public MessageBoxSuperButton Result
        {
            get { return m_result; }
        }

        private RelayCommand<MessageBoxSuperButton> m_clickCommand;
        public ICommand ClickCommand
        {
            get { return m_clickCommand; }
        }

        private void Click(MessageBoxSuperButton button)
        {
            m_result = button;
            this.Close();
        }

        public static MessageBoxSuperButton ShowBox(Window owner, string text, string caption, MessageBoxSuperPredefinedButtons buttons)
        {
            List<MessageBoxSuperButton> btns = new List<MessageBoxSuperButton>();
            switch(buttons)
            {
                case MessageBoxSuperPredefinedButtons.AbortRetryIgnore:
                    btns.Add(MessageBoxSuperButton.Abort);
                    btns.Add(MessageBoxSuperButton.Retry);
                    btns.Add(MessageBoxSuperButton.Ignore);
                    break;
                    
                case MessageBoxSuperPredefinedButtons.OK:
                    btns.Add(MessageBoxSuperButton.OK);
                    break;

                case MessageBoxSuperPredefinedButtons.OKCancel:
                    btns.Add(MessageBoxSuperButton.OK);
                    btns.Add(MessageBoxSuperButton.Cancel);
                    break;

                case MessageBoxSuperPredefinedButtons.RetryCancel:
                    btns.Add(MessageBoxSuperButton.Retry);
                    btns.Add(MessageBoxSuperButton.Cancel);
                    break;

                case MessageBoxSuperPredefinedButtons.YesNo:
                    btns.Add(MessageBoxSuperButton.Yes);
                    btns.Add(MessageBoxSuperButton.No);
                    break;

                case MessageBoxSuperPredefinedButtons.YesNoCancel:
                    btns.Add(MessageBoxSuperButton.Yes);
                    btns.Add(MessageBoxSuperButton.No);
                    btns.Add(MessageBoxSuperButton.Cancel);
                    break;
            }

            MessageBoxSuper dialog = new MessageBoxSuper(text, caption, btns);
            dialog.Owner = owner;
            dialog.ShowDialog();
            return dialog.m_result;
        }
    }
}
