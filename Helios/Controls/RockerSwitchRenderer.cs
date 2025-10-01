//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

    public class RockerSwitchRenderer : HeliosVisualRenderer
    {
        private ImageSource _imageOne;
        private ImageSource _imageOneIndicatorOn;
        private ImageSource _imageTwo;
        private ImageSource _imageTwoIndicatorOn;
        private ImageSource _imageThree;
        private ImageSource _imageThreeIndicatorOn;
        private Rect _imageRect;
        private Brush _textBrush;

        protected override void OnRender(DrawingContext drawingContext)
        {
           RockerSwitch toggleSwitch = Visual as RockerSwitch;
            if (toggleSwitch != null)
            {
                switch (toggleSwitch.SwitchPosition)
                {
                    case ThreeWayToggleSwitchPosition.One:
                        if (toggleSwitch.HasIndicator && toggleSwitch.IndicatorOn && _imageOneIndicatorOn != null)
                        {
                            DrawImage(drawingContext, _imageOneIndicatorOn, _imageRect);
                        }
                        else
                        {
                            DrawImage(drawingContext, _imageOne, _imageRect);
                        }
                        drawingContext.PushTransform(new TranslateTransform(toggleSwitch.TextPushOffset.X * -1d, toggleSwitch.TextPushOffset.Y * -1d));

                        break;
                    case ThreeWayToggleSwitchPosition.Two:
                        if (toggleSwitch.HasIndicator && toggleSwitch.IndicatorOn && _imageTwoIndicatorOn != null)
                        {
                            DrawImage(drawingContext, _imageTwoIndicatorOn, _imageRect);
                        }
                        else
                        {
                            DrawImage(drawingContext, _imageTwo, _imageRect);
                        }
                        break;
                    case ThreeWayToggleSwitchPosition.Three:
                        if (toggleSwitch.HasIndicator && toggleSwitch.IndicatorOn && _imageThreeIndicatorOn != null)
                        {
                            DrawImage(drawingContext, _imageThreeIndicatorOn, _imageRect);
                        }
                        else
                        {
                            DrawImage(drawingContext, _imageThree, _imageRect);
                        }
                        drawingContext.PushTransform(new TranslateTransform(toggleSwitch.TextPushOffset.X, toggleSwitch.TextPushOffset.Y));
 
                        break;
                }
                if (!string.IsNullOrEmpty(toggleSwitch.Text))
                {
                    DrawText(drawingContext, toggleSwitch, _textBrush, toggleSwitch.Text, _imageRect);
                }
            }
        }

        protected override void OnRefresh()
        {
            RockerSwitch toggleSwitch = Visual as RockerSwitch;
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
                _textBrush = new SolidColorBrush(toggleSwitch.TextColor);

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
