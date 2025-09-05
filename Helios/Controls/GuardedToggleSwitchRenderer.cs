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

    public class GuardedToggleSwitchRenderer : HeliosVisualRenderer
    {
        private ImageSource _imageOneGuardUp;
        private ImageSource _imageOneGuardDown;
        private ImageSource _imageTwoGuardUp;
        private ImageSource _imageTwoGuardDown;
        private Rect _imageRect;

        protected override void OnRender(DrawingContext drawingContext)
        {
            GuardedToggleSwitch toggleSwitch = Visual as GuardedToggleSwitch;
            if (toggleSwitch != null)
            {
                switch (toggleSwitch.SwitchPosition)
                {
                    case ToggleSwitchPosition.One:
                        if (toggleSwitch.GuardPosition == GuardPosition.Up)
                        {
                            RenderEffect(drawingContext, _imageOneGuardUp, _imageRect);
                        }
                        else
                        {
                            RenderEffect(drawingContext, _imageOneGuardDown, _imageRect);
                        }
                        break;
                    case ToggleSwitchPosition.Two:
                        if (toggleSwitch.GuardPosition == GuardPosition.Up)
                        {
                            RenderEffect(drawingContext, _imageTwoGuardUp, _imageRect);
                        }
                        else
                        {
                            RenderEffect(drawingContext, _imageTwoGuardDown, _imageRect);
                        }
                        break;
                }
            }
        }

        protected override void OnRefresh()
        {
            GuardedToggleSwitch toggleSwitch = Visual as GuardedToggleSwitch;
            if (toggleSwitch != null)
            {
                IImageManager3 refreshCapableImage = ConfigManager.ImageManager as IImageManager3;
                LoadImageOptions loadOptions = toggleSwitch.ImageRefresh ? LoadImageOptions.ReloadIfChangedExternally : LoadImageOptions.None;

                _imageRect.Width = toggleSwitch.Width;
                _imageRect.Height = toggleSwitch.Height;
                _imageOneGuardUp = refreshCapableImage.LoadImage(toggleSwitch.PositionOneGuardUpImage, loadOptions);
                _imageOneGuardDown = refreshCapableImage.LoadImage(toggleSwitch.PositionOneGuardDownImage, loadOptions);
                _imageTwoGuardUp = refreshCapableImage.LoadImage(toggleSwitch.PositionTwoGuardUpImage, loadOptions);
                _imageTwoGuardDown = refreshCapableImage.LoadImage(toggleSwitch.PositionTwoGuardDownImage, loadOptions);
            }
            else
            {
                _imageOneGuardUp = null;
                _imageOneGuardDown = null;
                _imageTwoGuardUp = null;
                _imageTwoGuardDown = null;
            }
        }
    }
}
