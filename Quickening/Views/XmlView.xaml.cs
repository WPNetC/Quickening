using Quickening.ViewModels;
using System.Windows;
using System.Windows.Controls;
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