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

    public class GuardedThreeWayToggleRenderer : HeliosVisualRenderer
    {
        private ImageSource _imageOneGuardUp;
        private ImageSource _imageOneGuardDown;
        private ImageSource _imageTwoGuardUp;
        private ImageSource _imageTwoGuardDown;
        private ImageSource _imageThreeGuardUp;
        private ImageSource _imageThreeGuardDown;
        private Rect _imageRect;

        protected override void OnRender(DrawingContext drawingContext)
        {
            GuardedThreeWayToggle toggleSwitch = Visual as GuardedThreeWayToggle;
            if (toggleSwitch != null)
            {
                switch (toggleSwitch.SwitchPosition)
                {
                    case ThreeWayToggleSwitchPosition.One:
                        DrawImage(drawingContext, toggleSwitch.GuardPosition == GuardPosition.Up ? _imageOneGuardUp : _imageOneGuardDown, _imageRect);
                        break;
                    case ThreeWayToggleSwitchPosition.Two:
                        DrawImage(drawingContext, toggleSwitch.GuardPosition == GuardPosition.Up ? _imageTwoGuardUp : _imageTwoGuardDown, _imageRect);
                        break;
                    case ThreeWayToggleSwitchPosition.Three:
                        DrawImage(drawingContext, toggleSwitch.GuardPosition == GuardPosition.Up ? _imageThreeGuardUp : _imageThreeGuardDown, _imageRect);
                        break;
                }
            }
        }

        protected override void OnRefresh()
        {
            GuardedThreeWayToggle toggleSwitch = Visual as GuardedThreeWayToggle;
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
                _imageThreeGuardUp = refreshCapableImage.LoadImage(toggleSwitch.PositionThreeGuardUpImage, loadOptions);
                _imageThreeGuardDown = refreshCapableImage.LoadImage(toggleSwitch.PositionThreeGuardDownImage, loadOptions);
            }
            else
            {
                _imageOneGuardUp = null;
                _imageOneGuardDown = null;
                _imageTwoGuardUp = null;
                _imageTwoGuardDown = null;
                _imageThreeGuardUp = null;
                _imageThreeGuardDown = null;
            }
        }
    }
}
