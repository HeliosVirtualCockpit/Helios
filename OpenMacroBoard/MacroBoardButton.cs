//  Copyright 2021 Helios Contributors
//
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GadrocsWorkshop.Helios.Interfaces.OpenMacroBoard
{
    [Serializable]
    public class MacroBoardButton : INotifyPropertyChanged
    {
        private int _row;
        private int _column;
        private bool _isPressed;
        private string _buttonText;
        private string _backgroundImageUri;
        private bool _backgroundImageEnabled;
        private BitmapImage _bitmapImage;
        private RenderTargetBitmap _buttonImage;

        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MacroBoardButton(int row, int column, string text, string backgroundImageUri)
        {
            _row = row;
            _column = column;
            _buttonText = text;
            _isPressed = false;
            _backgroundImageUri = backgroundImageUri;
            _backgroundImageEnabled = true;
            _bitmapImage = null;
            UpdateImage();
        }

        public MacroBoardButton(int row, int column) : this(row, column, "", "")
        {
        }

        public MacroBoardButton() : this(0, 0)
        {
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdateImage()
        {
            var targetImage = new RenderTargetBitmap(72, 72, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var r = visual.RenderOpen())
            {
                r.DrawRectangle(new SolidColorBrush(Colors.Black), new Pen(new SolidColorBrush(Colors.Black), 2), new Rect(0, 0, 72, 72));
                if (BackgroundImageEnabled && _bitmapImage == null)
                {
                    try
                    {
                        _bitmapImage = new BitmapImage();
                        _bitmapImage.BeginInit();
                        _bitmapImage.UriSource = new Uri(BackgroundImageUri, UriKind.Absolute);
                        _bitmapImage.EndInit();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Image rendering failed");
                    }
                }

                if (BackgroundImageEnabled)
                {
                    r.DrawImage(_bitmapImage, new Rect(0, 0, 72, 72));
                }

                FormattedText formattedText = new FormattedText(
                    _buttonText,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    18,
                    Brushes.White, VisualTreeHelper.GetDpi(visual).PixelsPerDip);

                formattedText.MaxTextHeight = 64;
                formattedText.MaxTextWidth = 64;

                formattedText.TextAlignment = TextAlignment.Center;

                r.DrawText(formattedText, new Point(4, 36 - formattedText.Height / 2));
            }

            targetImage.Render(visual);
            targetImage.Freeze();
            _buttonImage = targetImage;
            NotifyPropertyChanged("ButtonImage");
        }

        #region Properties
        public int Row
        {
            get => _row;
            set
            {
                _row = value;
            }
        }

        public int Column
        {
            get => _column;
            set
            {
                _column = value;
            }
        }

        public string Text
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public string BackgroundImageUri
        {
            get => _backgroundImageUri;
            set
            {
                _backgroundImageUri = value;

                if (_backgroundImageUri == "")
                {
                    BackgroundImageEnabled = false;
                }
                _bitmapImage = null;

                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public bool BackgroundImageEnabled
        {
            get => _backgroundImageEnabled;
            set
            {
                _backgroundImageEnabled = value;
                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public RenderTargetBitmap ButtonImage
        {
            get
            {
                if (_buttonImage == null)
                {
                    UpdateImage();
                }
                return _buttonImage;
            }
        }

        public bool IsPressed
        {
            get => _isPressed;
            set
            {
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
