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

namespace Quickening.Views
{
    /// <summary>
    /// Interaction logic for TextInputPopUp.xaml
    /// </summary>
    public partial class TextInputPopUp : Window, IDisposable
    {
        public TextInputPopUp()
        {
            InitializeComponent();
            // Clear any existing values.
            Values = new List<string[]>();
        }

        public TextInputPopUp(params string[] boxes)
            : this()
        {
            for (int ii = 0; ii < boxes.Length; ii++)
            {
                var box = boxes[ii];
                Values.Add(new string[2] { box, box });
            }

            this.Height = (26 + 32) + (56 * boxes.Length);
        }

        /// <summary>
        /// List of the textboxes created and their text content.
        /// <para>[0] is the textbox name. [1] is the text content.</para>
        /// </summary>
        public List<string[]> Values
        {
            get { return (List<string[]>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(List<string[]>), typeof(TextInputPopUp), new PropertyMetadata(null));
        
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Values = null;
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
