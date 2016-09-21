using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quickening.Views
{
    /// <summary>
    /// Interaction logic for TextInputPopUp.xaml
    /// </summary>
    public partial class TextInputPopUp : Window
    {
        public TextInputPopUp()
        {
            InitializeComponent();
        }

        public string InputResult
        {
            get { return (string)GetValue(InputResultProperty); }
            set { SetValue(InputResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputResult.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputResultProperty =
            DependencyProperty.Register("InputResult", typeof(string), typeof(TextInputPopUp), new PropertyMetadata(string.Empty));

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
