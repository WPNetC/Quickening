using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Quickening.Globals
{
    public class Colours
    {
        #region Fields
        private static Brush _colHeaderText = Brushes.Gold;
        private static Brush _colLabelText = Brushes.DarkGray;
        private static Brush _colDefaultText = Brushes.LightGray;
        private static Brush _colOtherText = Brushes.Pink;

        private static Brush _colBorder01 = Brushes.DarkGray;

        private static Brush _mainGridBkg = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF202020"));
        private static Brush _defaultGridBkg = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF272727"));
        #endregion

        public Colours() { }

        #region Text Colours
        public static Brush Col_HeaderText
        {
            get
            {
                return _colHeaderText;
            }
            private set
            {
                if (value != _colHeaderText)
                {
                    _colHeaderText = value;
                    OnChanged();
                }
            }
        }

        public static Brush Col_LabelText
        {
            get
            {
                return _colLabelText;
            }
            private set
            {
                if (value != _colLabelText)
                {
                    _colLabelText = value;
                    OnChanged();
                }
            }
        }

        public static Brush Col_DefaultText
        {
            get
            {
                return _colDefaultText;
            }
            private set
            {
                if (value != _colDefaultText)
                {
                    _colDefaultText = value;
                    OnChanged();
                }
            }
        }

        public static Brush Col_OtherText
        {
            get
            {
                return _colOtherText;
            }
            private set
            {
                if (value != _colOtherText)
                {
                    _colOtherText = value;
                    OnChanged();
                }
            }
        }
        #endregion

        #region Border Colours
        public static Brush Col_Border01
        {
            get
            {
                return _colBorder01;
            }
            private set
            {
                if (value != _colBorder01)
                {
                    _colBorder01 = value;
                    OnChanged();
                }
            }
        }
        #endregion

        #region Buy / Sell Colours
        public static Brush BUYCOLOUR { get { return Brushes.DarkGreen; } }
        public static Brush SELLCOLOUR { get { return Brushes.DarkRed; } }
        #endregion

        #region Background Colours
        public static Brush MainGridBackground
        {
            get
            {
                return _mainGridBkg;
            }
            private set
            {
                if (_mainGridBkg != value)
                {
                    _mainGridBkg = value;
                    OnChanged();
                }
            }
        }
        public static Brush DefaultGridBackground
        {
            get
            {
                return _defaultGridBkg;
            }
            private set
            {
                if (_defaultGridBkg != value)
                {
                    _defaultGridBkg = value;
                    OnChanged();
                }
            }
        }
        #endregion

        #region Methods
        public static void SwitchColour(string name, Brush brush)
        {
            if (!string.IsNullOrEmpty(name.Trim()))
            {
                switch (name.ToLower().Trim())
                {
                    case "headertext":
                        {
                            Col_HeaderText = brush;
                            break;
                        }
                    case "labeltext":
                        {
                            Col_LabelText = brush;
                            break;
                        }
                    case "defaulttext":
                        {
                            Col_DefaultText = brush;
                            break;
                        }
                    case "othertext":
                        {
                            Col_OtherText = brush;
                            break;
                        }
                    default:
                        break;
                }
            }
        }
        #endregion

        [field: NonSerialized]
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        private static void OnChanged([CallerMemberName]string p = "")
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(p));
            }
        }
    }
}