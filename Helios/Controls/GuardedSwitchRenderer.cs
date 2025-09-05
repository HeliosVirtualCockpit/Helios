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

    public class GuardedSwitchRenderer : HeliosVisualRenderer
    {
        private ImageSource _imageOneGuardUp;
        private ImageSource _imageTwoGuardDown;
        private Rect _imageRect;

        protected override void OnRender(DrawingContext drawingContext)
        {
            GuardedSwitch toggleSwitch = Visual as GuardedSwitch;
            if (toggleSwitch != null)
            {
				if (toggleSwitch.GuardPosition == GuardPosition.Up)
				{
					RenderEffect(drawingContext, _imageOneGuardUp, _imageRect);
				}
				else
				{
					RenderEffect(drawingContext, _imageTwoGuardDown, _imageRect);
				}
				
			}
        }

        protected override void OnRefresh()
        {
            GuardedSwitch toggleSwitch = Visual as GuardedSwitch;
            if (toggleSwitch != null)
            {
                _imageRect.Width = toggleSwitch.Width;
                _imageRect.Height = toggleSwitch.Height;
                _imageOneGuardUp = ConfigManager.ImageManager.LoadImage(toggleSwitch.PositionOneGuardUpImage);
                _imageTwoGuardDown = ConfigManager.ImageManager.LoadImage(toggleSwitch.PositionTwoGuardDownImage);
            }
            else
            {
                _imageOneGuardUp = null;
                _imageTwoGuardDown = null;
            }
        }
    }
}
