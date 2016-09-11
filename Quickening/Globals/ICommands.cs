using Quickening.ViewModels;
using System;
using System.Windows.Input;

namespace Quickening.ICommands
{
    #region Xml View Model Commands

    internal sealed class SaveNode : ICommand
    {
        public SaveNode(XmlViewModel vm)
        {
            this._vm = vm;
        }

        private XmlViewModel _vm;

        bool ICommand.CanExecute(object parameter)
        {
            return _vm.CanSave;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            _vm.SaveNode();
        }
    }

    internal sealed class CreateNewTemplate : ICommand
    {
        public CreateNewTemplate(XmlViewModel vm)
        {
            this._vm = vm;
        }

        private XmlViewModel _vm;

        bool ICommand.CanExecute(object parameter)
        {
            return _vm.CanUseTemplate;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            _vm.CreateNewTemplate();
        }
    }

    internal sealed class EditTemplate : ICommand
    {
        public EditTemplate(XmlViewModel vm)
        {
            this._vm = vm;
        }

        private XmlViewModel _vm;

        bool ICommand.CanExecute(object parameter)
        {
            return _vm.CanEditTemplate;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            _vm.EditTemplate();
        }
    }

    #endregion Xml View Model Commands

    #region Context Menu Commands

    internal sealed class AddItem : ICommand
    {
        public AddItem(XmlViewModel vm)
        {
            this._vm = vm;
        }

        private XmlViewModel _vm;

        bool ICommand.CanExecute(object parameter)
        {
            return true; // _vm.CanAddItem;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            _vm.AddItem(parameter);
        }
    }

    internal sealed class RemoveItem : ICommand
    {
        public RemoveItem(XmlViewModel vm)
        {
            this._vm = vm;
        }

        private XmlViewModel _vm;

        bool ICommand.CanExecute(object parameter)
        {
            // As we are in a context menu, we must have a node selected, so can always remove.
            return true;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            _vm.RemoveItem();
        }
    }

    #endregion Context Menu Commands

    #region Main Window Commands
    internal sealed class NewXmlFile : ICommand
    {
        public NewXmlFile(ConfiguratorViewModel vm)
        {
            this._vm = vm;
        }

        private ConfiguratorViewModel _vm;

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            _vm.NewXmlFile();
        }
    }
    internal sealed class SetAsDefault : ICommand
    {
        public SetAsDefault(ConfiguratorViewModel vm)
        {
            this._vm = vm;
        }

        private ConfiguratorViewModel _vm;

        bool ICommand.CanExecute(object parameter)
        {
            return _vm.CanSetAsDefault;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            _vm.SetAsDefault();
        }
    }
    internal sealed class ImportExport : ICommand
    {
        public ImportExport(ConfiguratorViewModel vm)
        {
            this._vm = vm;
        }

        private ConfiguratorViewModel _vm;

        bool ICommand.CanExecute(object parameter)
        {
            return parameter?.ToString().ToLower() == "import" ?
                true :
                _vm.CanSetAsDefault;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            _vm.ImportExport(parameter);
        }
    }
    #endregion
}