using Quickening.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System;
using System.Windows.Input;
using Quickening.ICommands;
using Quickening.Globals;
using System.Runtime.CompilerServices;
using Quickening.Services;

namespace Quickening.ViewModels
{
    public class ConfiguratorViewModel : ViewModelBase
    {
        private UserControl _currentView;
        private ObservableCollection<string> _xmlFiles;
        private string _currentXmlFile;
        private ICommand _cmdNewXmlFile;
        private ICommand _cmdSetAsDefault;
        private ICommand _cmdImportExport;
        private ObservableCollection<Tuple<string, UserControl>> _views;

        public ConfiguratorViewModel()
        {

        }

        public ObservableCollection<Tuple<string, UserControl>> Views
        {
            get
            {
                if (_views == null)
                    _views = new ObservableCollection<Tuple<string, UserControl>>
                    {
                        {new Tuple<string, UserControl>("Layout Editor", new XmlView())},
                        {new Tuple<string, UserControl>("Angular Editor", new AngularView())}
                    };
                return _views;
            }
            private set
            {
                if (value != _views)
                {
                    _views = value;
                    CurrentView = _views?.Count() > 0 ? _views.FirstOrDefault().Item2 : null;
                    OnChanged();
                }
            }
        }
        public UserControl CurrentView
        {
            get
            {
                if (_currentView == null && Views?.Count > 0)
                    _currentView = Views.FirstOrDefault().Item2;

                return _currentView;
            }
            set
            {
                if (value != _currentView)
                {
                    _currentView = value;
                    UpdateXmlFileList();
                    OnChanged();
                }
            }
        }

        public ObservableCollection<string> XmlFiles
        {
            get
            {
                if (_xmlFiles == null)
                    _xmlFiles = new ObservableCollection<string>();
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

        public bool CanSetAsDefault
        {
            get
            {
                return CurrentXmlFile != null;
            }
        }

        public ICommand cmdNewXmlFile
        {
            get
            {
                if (_cmdNewXmlFile == null)
                    _cmdNewXmlFile = new NewXmlFile(this);
                return _cmdNewXmlFile;
            }
        }
        public ICommand cmdSetAsDefault
        {
            get
            {
                if (_cmdSetAsDefault == null)
                    _cmdSetAsDefault = new SetAsDefault(this);
                return _cmdSetAsDefault;
            }
        }
        public ICommand cmdImportExport
        {
            get
            {
                if (_cmdImportExport == null)
                    _cmdImportExport = new ImportExport(this);
                return _cmdImportExport;
            }
        }

        internal void UpdateXmlFileList(bool updateUI = true)
        {
            var dir = GetXmlDirectoryFromView();
            if (dir == null)
                return;

            var filter = Strings.LAYOUT_FILE_FILTER.Split('|')[1]?.Trim();
            if (filter == null)
                return;

            var dInf = new DirectoryInfo(dir);
            if (updateUI)
                XmlFiles = new ObservableCollection<string>(dInf.GetFiles(filter).Select(p => p.Name));
            else
                _xmlFiles = new ObservableCollection<string>(dInf.GetFiles(filter).Select(p => p.Name));
        }

        internal void NewXmlFile()
        {
            var dir = GetXmlDirectoryFromView();
            if (dir == null)
                return;

            FileService.SaveNewXmlFile(dir);

            UpdateXmlFileList();
        }

        internal void SetAsDefault()
        {
            if (string.IsNullOrEmpty(CurrentXmlFile))
                return;

            Strings.DefaultXmlFile = CurrentXmlFile;
        }

        internal void ImportExport(object param)
        {
            var value = Convert.ToString(param);
            if (string.IsNullOrEmpty(value))
                return;

            var dir = GetXmlDirectoryFromView();
            if (dir == null)
                return;

            var fileName = CurrentXmlFile;

            FileService.ImportExportXmlFile(value, dir, fileName);

            if (value.ToLower() == "import")
                UpdateXmlFileList();
        }

        private string GetXmlDirectoryFromView()
        {
            if (CurrentView == null || !Views.Any())
                return null;

            var view = Views.FirstOrDefault(p => p.Item2.Name == CurrentView.Name).Item1;
            view = view?.Split(' ')[0]?.ToLower();
            if (string.IsNullOrEmpty(view?.Trim()))
                return null;

            var path = "";

            switch (view)
            {
                case "layout":
                    path = Globals.Strings.LayoutsDirectory;
                    break;
                case "angular":
                    path = Globals.Strings.AngularDirectory;
                    break;
                default:
                    return null;
            }

            return path;
        }
    }
}