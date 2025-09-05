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

    public class HatSwitchRenderer : HeliosVisualRenderer
    {
        private HatSwitch _switch;

        private ImageSource _upImage;
        private ImageSource _downImage;
        private ImageSource _leftImage;
        private ImageSource _rightImage;
        private ImageSource _centerImage;

        private Rect _imageRect;

        protected override void OnPropertyChanged(Helios.ComponentModel.PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Visual"))
            {
                _switch = args.NewValue as HatSwitch;
            }
            base.OnPropertyChanged(args);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_switch != null)
            {
                switch (_switch.SwitchPosition)
                {
                    case HatSwitchPosition.Center:
                        RenderEffect(drawingContext, _centerImage, _imageRect);
                        break;
                    case HatSwitchPosition.Up:
                        RenderEffect(drawingContext, _upImage, _imageRect);
                        break;
                    case HatSwitchPosition.Down:
                        RenderEffect(drawingContext, _downImage, _imageRect);
                        break;
                    case HatSwitchPosition.Left:
                        RenderEffect(drawingContext, _leftImage, _imageRect);
                        break;
                    case HatSwitchPosition.Right:
                        RenderEffect(drawingContext, _rightImage, _imageRect);
                        break;
                }
            }
        }

        protected override void OnRefresh()
        {
            if (_switch != null)
            {
                _imageRect.Width = _switch.Width;
                _imageRect.Height = _switch.Height;
                _upImage = ConfigManager.ImageManager.LoadImage(_switch.UpImage);
                _downImage = ConfigManager.ImageManager.LoadImage(_switch.DownImage);
                _leftImage = ConfigManager.ImageManager.LoadImage(_switch.LeftImage);
                _rightImage = ConfigManager.ImageManager.LoadImage(_switch.RightImage);
                _centerImage = ConfigManager.ImageManager.LoadImage(_switch.CenterImage);
            }
            else
            {
                _upImage = null;
                _downImage = null;
                _leftImage = null;
                _rightImage = null;
                _centerImage = null;
            }
        }
    }
}
