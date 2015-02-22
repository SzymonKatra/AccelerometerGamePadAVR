using System;
using System.Collections.Generic;
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

namespace AccelerometerGamePadUtility
{
    /// <summary>
    /// Interaction logic for AxisCallibrationWindow.xaml
    /// </summary>
    public partial class AxisCallibrationWindow : Window
    {
        public AxisCallibrationWindow(AxisCallibrationViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            Global.Dialogs.Register(this, viewModel);
        }
    }
}
