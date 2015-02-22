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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccelerometerGamePadUtility
{
    /// <summary>
    /// Interaction logic for SliderWithButtons.xaml
    /// </summary>
    public partial class SliderWithButtons : UserControl
    {
        public SliderWithButtons()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(SliderWithButtons));
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(SliderWithButtons));
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(SliderWithButtons));
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            Slider.Value--;
        }
        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            Slider.Value++;
        }
    }
}
