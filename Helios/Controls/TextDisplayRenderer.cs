//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Controls
{
    using System.Windows;
    using System.Windows.Media;

    public class TextDisplayRenderer : HeliosVisualRenderer
    {
        private TextDisplayRect _textDisplay;
        private ImageSource _onImage;
        private Brush _onBrush;
        private Brush _backgroundBrush;
        private Rect _imageRect;

        protected override void OnPropertyChanged(Helios.ComponentModel.PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Visual"))
            {
                _textDisplay = args.NewValue as TextDisplayRect;
            }
            base.OnPropertyChanged(args);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // RenderEffect(drawingContext, _onImage, _imageRect);
            if (_textDisplay.UseBackground)
            {
                drawingContext.DrawRectangle(new SolidColorBrush(_textDisplay.BackgroundColor), null, _imageRect);
            }
            _textDisplay.TextFormat.RenderText(drawingContext, _onBrush , _textDisplay.TextValue, _imageRect);
        }

        protected override void OnRefresh()
        {
            if (_textDisplay != null)
            {
                _imageRect.Width = _textDisplay.Width;
                _imageRect.Height = _textDisplay.Height;
                _onImage = ConfigManager.ImageManager.LoadImage(_textDisplay.OnImage);
                _onBrush = new SolidColorBrush(_textDisplay.OnTextColor);
                _backgroundBrush = new SolidColorBrush(_textDisplay.BackgroundColor);
            }
            else
            {
                _onImage = null;
                _onBrush = null;
            }
        }
    }
}
