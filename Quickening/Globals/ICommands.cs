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
    #endregion
}
