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

    public class KneeboardSwitchRenderer : HeliosVisualRenderer
    {
        private ImageSource _imageOne;
        private ImageSource _imageOneIndicatorOn;
        private ImageSource _imageTwo;
        private ImageSource _imageTwoIndicatorOn;
        private ImageSource _imageThree;
        private ImageSource _imageThreeIndicatorOn;
        private Rect _imageRect;

        protected override void OnRender(DrawingContext drawingContext)
        {
            KneeboardSwitch toggleSwitch = Visual as KneeboardSwitch;
            if (toggleSwitch != null)
            {
                switch (toggleSwitch.SwitchPosition)
                {
                    case KneeboardSwitchPosition.One:
                        if (toggleSwitch.HasIndicator && toggleSwitch.IndicatorOn && _imageOneIndicatorOn != null)
                        {
                            RenderEffect(drawingContext, _imageOneIndicatorOn, _imageRect);
                        }
                        else
                        {
                            RenderEffect(drawingContext, _imageOne, _imageRect);
                        }
                        break;
                    case KneeboardSwitchPosition.Two:
                        if (toggleSwitch.HasIndicator && toggleSwitch.IndicatorOn && _imageTwoIndicatorOn != null)
                        {
                            RenderEffect(drawingContext, _imageTwoIndicatorOn, _imageRect);
                        }
                        else
                        {
                            RenderEffect(drawingContext, _imageTwo, _imageRect);
                        }
                        break;
                    case KneeboardSwitchPosition.Three:
                        if (toggleSwitch.HasIndicator && toggleSwitch.IndicatorOn && _imageThreeIndicatorOn != null)
                        {
                            RenderEffect(drawingContext, _imageThreeIndicatorOn, _imageRect);
                        }
                        else
                        {
                            RenderEffect(drawingContext, _imageThree, _imageRect);
                        }
                        break;
                }            
            }
        }

        protected override void OnRefresh()
        {
            KneeboardSwitch toggleSwitch = Visual as KneeboardSwitch;
            if (toggleSwitch != null)
            {
                IImageManager3 refreshCapableImage = ConfigManager.ImageManager as IImageManager3;
                LoadImageOptions loadOptions = toggleSwitch.ImageRefresh ? LoadImageOptions.ReloadIfChangedExternally : LoadImageOptions.None;

                _imageRect.Width = toggleSwitch.Width;
                _imageRect.Height = toggleSwitch.Height;
                _imageOne = refreshCapableImage.LoadImage(toggleSwitch.PositionOneImage, loadOptions);
                _imageOneIndicatorOn = refreshCapableImage.LoadImage(toggleSwitch.PositionOneIndicatorOnImage, loadOptions);

                _imageTwo = refreshCapableImage.LoadImage(toggleSwitch.PositionTwoImage, loadOptions);
                _imageTwoIndicatorOn = refreshCapableImage.LoadImage(toggleSwitch.PositionTwoIndicatorOnImage, loadOptions);

                _imageThree = refreshCapableImage.LoadImage(toggleSwitch.PositionThreeImage, loadOptions);
                _imageThreeIndicatorOn = refreshCapableImage.LoadImage(toggleSwitch.PositionThreeIndicatorOnImage, loadOptions);

            }
            else
            {
                _imageOne = null;
                _imageTwo = null;
                _imageThree = null;
            }
        }
    }
}
