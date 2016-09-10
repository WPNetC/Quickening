using Quickening.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Quickening.ViewModels
{
    public class ConfiguratorViewModel : ViewModelBase
    {
        private UserControl _currentView;
        private ObservableCollection<string> _xmlFiles;
        private string _currentXmlFile;

        public ConfiguratorViewModel()
        {

        }

        public UserControl CurrentView
        {
            get
            {
                if (_currentView == null)
                    _currentView = new XmlView();

                return _currentView;
            }
            private set
            {
                if (value != _currentView)
                {
                    _currentView = value;
                    OnChanged();
                }
            }
        }

        public ObservableCollection<string> XmlFiles
        {
            get
            {
                if (_xmlFiles == null)
                    LoadDataSets(false);
                return _xmlFiles;
            }
            private set
            {
                if (value != _xmlFiles)
                {
                    _xmlFiles = value;
                    OnChanged();
                }
            }
        }
        public string CurrentXmlFile
        {
            get
            {
                return _currentXmlFile;
            }
            set
            {
                if (value != _currentXmlFile)
                {
                    _currentXmlFile = value;
                    OnChanged();

                    if (!string.IsNullOrEmpty(_currentXmlFile))
                    {
                        var view = CurrentView as XmlView;
                        if (view == null)
                            return;

                        var vm = view.DataContext as XmlViewModel;
                        if (vm == null)
                            return;

                        vm.LoadXml(_currentXmlFile);
                    }
                }
            }
        }
        internal void LoadDataSets(bool update = true)
        {
            if (!Directory.Exists(Globals.Strings.XmlDirectory))
                return;

            var dInf = new DirectoryInfo(Globals.Strings.XmlDirectory);
            if (update)
                XmlFiles = new ObservableCollection<string>(dInf.GetFiles().Select(p => p.Name));
            else
                _xmlFiles = new ObservableCollection<string>(dInf.GetFiles().Select(p => p.Name));
        }
    }
}
