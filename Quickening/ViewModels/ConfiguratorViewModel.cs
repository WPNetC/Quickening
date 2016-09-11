using Quickening.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System;
using System.Windows.Input;
using Quickening.ICommands;
using Quickening.Globals;

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

        public ConfiguratorViewModel()
        {
        }

        public UserControl CurrentView
        {
            get
            {
                // Create a default view. May be removed if other views are added.
                if (_currentView == null)
                {
                    var view = new XmlView();
                    _currentView = view;
                }

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
                {
                    UpdateXmlFileList(false);
                }
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
            if (!Directory.Exists(Globals.Strings.XmlDirectory))
                return;

            var dInf = new DirectoryInfo(Globals.Strings.XmlDirectory);
            if (updateUI)
                XmlFiles = new ObservableCollection<string>(dInf.GetFiles().Select(p => p.Name));
            else
                _xmlFiles = new ObservableCollection<string>(dInf.GetFiles().Select(p => p.Name));
        }

        internal void NewXmlFile()
        {
            var sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = Strings.LAYOUT_FILE_FILTER;
            sfd.DefaultExt = ".xml";
            sfd.InitialDirectory = Strings.XmlDirectory;

            var result = sfd.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    var path = sfd.FileName;
                    if (string.IsNullOrEmpty(path))
                        return;

                    // Ensure we copy to the Xml directory.
                    if (Path.GetDirectoryName(path) != Strings.XmlDirectory)
                    {
                        path = Path.Combine(Strings.XmlDirectory, Path.GetFileName(sfd.FileName));
                    }

                    int num = 0;
                    while (File.Exists(path))
                    {
                        // If file already exists increment a version number to prevent exception.
                        path = Path.Combine(Strings.XmlDirectory, $"{sfd.FileName.Replace(".xml", "")}_{++num}.xml");
                    }

                    // Write base tag to new file.
                    File.WriteAllText(path, Strings.BaseXmlText);

                    var dr2 = System.Windows.MessageBox.Show($"Layout file created: {path}{Environment.NewLine}Copy path to clipboard?.",
                            "Success",
                            System.Windows.MessageBoxButton.YesNo);

                    if (dr2 == System.Windows.MessageBoxResult.Yes)
                        System.Windows.Forms.Clipboard.SetText(path);

                    UpdateXmlFileList();
                    break;
                default:
                    return;
            }
        }

        internal void SetAsDefault()
        {
            if (string.IsNullOrEmpty(CurrentXmlFile))
                return;

            Strings.DefaultXmlFile = CurrentXmlFile;
        }

        internal void ImportExport(object param)
        {
            var value = param.ToString();
            if (string.IsNullOrEmpty(value))
                return;

            if (value.ToLower() == "import")
            {
                var ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.Filter = Strings.LAYOUT_FILE_FILTER;
                var result = ofd.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                    case System.Windows.Forms.DialogResult.Yes:
                        var path = Path.Combine(Strings.XmlDirectory, ofd.SafeFileName);
                        int num = 0;
                        while (File.Exists(path))
                        {
                            // If file already exists increment a version number to prevent exception.
                            path = Path.Combine(Strings.XmlDirectory, $"{ofd.SafeFileName.Replace(".xml", "")}_{++num}.xml");
                        }
                        File.Copy(ofd.FileName, path);
                        UpdateXmlFileList();
                        break;
                    default:
                        return;
                }
            }
            else if (value.ToLower() == "export")
            {
                var sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = Strings.LAYOUT_FILE_FILTER;
                var result = sfd.ShowDialog();
                switch (result)
                {
                    case System.Windows.Forms.DialogResult.OK:
                    case System.Windows.Forms.DialogResult.Yes:
                        var path = sfd.FileName;
                        if (string.IsNullOrEmpty(path))
                            return;

                        int num = 0;
                        while (File.Exists(path))
                        {
                            // If file already exists increment a version number to prevent exception.
                            path = Path.Combine(Strings.XmlDirectory, $"{sfd.FileName.Replace(".xml", "")}_{++num}.xml");
                        }
                        var xmlFile = Path.Combine(Strings.XmlDirectory, CurrentXmlFile);
                        if (!File.Exists(xmlFile))
                        {
                            var dr = System.Windows.MessageBox.Show(
                                $"Cannot find source file: {xmlFile}{Environment.NewLine}Copy path to clipboard?.",
                                "Error",
                                System.Windows.MessageBoxButton.YesNo,
                                System.Windows.MessageBoxImage.Error);

                            if (dr == System.Windows.MessageBoxResult.Yes)
                                System.Windows.Forms.Clipboard.SetText(xmlFile);

                            return;
                        }

                        File.Copy(xmlFile, path);
                        var dr2 = System.Windows.MessageBox.Show($"Layout file exported to: {xmlFile}{Environment.NewLine}Copy path to clipboard?.",
                                "Success",
                                System.Windows.MessageBoxButton.YesNo);

                        if (dr2 == System.Windows.MessageBoxResult.Yes)
                            System.Windows.Forms.Clipboard.SetText(xmlFile);
                        break;
                    default:
                        return;
                }
            }
            else
            {
                throw new InvalidOperationException("Could not understand parameter: " + value);
            }
        }
    }
}