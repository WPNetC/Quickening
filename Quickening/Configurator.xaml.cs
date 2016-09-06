using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Quickening
{
    /// <summary>
    /// Interaction logic for Configurator.xaml.
    /// </summary>
    [ProvideToolboxControl("Quickening.Configurator", true)]
    public partial class Configurator : UserControl
    {
        public Configurator()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "We are inside {0}.Button1_Click()", this.ToString()));
        }
    }
}
