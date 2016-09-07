using Quickening.ViewModels;
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
using System.Xml;

namespace Quickening.Views
{
    /// <summary>
    /// Interaction logic for XmlView.xaml
    /// </summary>
    public partial class XmlView : UserControl
    {
        private XmlViewModel vm;

        public XmlView()
        {
            InitializeComponent();
            vm = grdMain.DataContext as XmlViewModel;
        }

        private void dirTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var node = e.NewValue as XmlNode;

            if (node == null || vm == null)
                return;

            vm.SelectedNode = node;
        }
    }
}
