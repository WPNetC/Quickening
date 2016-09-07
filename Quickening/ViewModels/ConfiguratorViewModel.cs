using Quickening.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Quickening.ViewModels
{
    public class ConfiguratorViewModel : ViewModelBase
    {
        private UserControl _currentView;

        public ConfiguratorViewModel()
        {

        }
        
        public UserControl CurrentView
        {
            get
            {
#if DEBUG
                if (_currentView == null)
                    _currentView = new XmlView();
#endif
                return _currentView;
            }
            private set
            {
                if(value != _currentView)
                {
                    _currentView = value;
                    OnChanged();
                }
            }
        }
    }
}
