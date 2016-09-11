using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Quickening.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public ViewModelBase()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private delegate void OnChangedDelegate(string p);
        protected virtual void OnChanged([CallerMemberName]string p = "")
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
            }
            else
            {
                var del = new OnChangedDelegate(OnChanged);
                Application.Current.Dispatcher.Invoke(del, new[] { p });
            }
        }

        #region IDisposable Support

        protected bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PropertyChanged = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}