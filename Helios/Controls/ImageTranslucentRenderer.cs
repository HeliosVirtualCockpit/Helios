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
    using System.Windows.Controls;

    public class ImageTranslucentRenderer : HeliosVisualRenderer
    {
        private ImageSource _image;
        private Rect _imageRect;
        private VisualBrush _visualBrush;
        private Image _imageControl;
        private Pen _borderPen;
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_image != null)
            {
                  ImageTranslucent profileImage = Visual as ImageTranslucent;
                DrawImage(drawingContext, _image, _imageRect);
                //drawingContext.DrawRoundedRectangle(_visualBrush, _borderPen, _imageRect, profileImage.CornerRadius, profileImage.CornerRadius);
            }
        }

        protected override void OnRender(DrawingContext drawingContext, double scaleX, double scaleY)
        {
            if (_image != null)
            {

                ImageTranslucent profileImage = Visual as ImageTranslucent;
                drawingContext.PushOpacity(profileImage.ImageOpacity); 
                Rect scaledRect = new Rect(_imageRect.X, _imageRect.Y, _imageRect.Width * scaleX, _imageRect.Height * scaleY);
                DrawImage(drawingContext, _image, scaledRect);
                drawingContext.Pop();
            }            
        }

        protected override void OnRefresh()
        {


            ImageTranslucent profileImage = Visual as ImageTranslucent;
            if (profileImage != null)
            {
                IImageManager3 refreshCapableImage = ConfigManager.ImageManager as IImageManager3;
                LoadImageOptions loadOptions = Visual.ImageRefresh ? LoadImageOptions.ReloadIfChangedExternally : LoadImageOptions.None;

                _image = refreshCapableImage.LoadImage(profileImage.Image, loadOptions);
                _imageRect.Width = profileImage.Width;
                _imageRect.Height = profileImage.Height;

                if (profileImage.BorderThickness > 0d)
                {
                    _borderPen = new Pen(new SolidColorBrush(profileImage.BorderColor), profileImage.BorderThickness);
                }
                else
                {
                    _borderPen = null;
                }



                if (_image == null)
                {
                    _image = ConfigManager.ImageManager.LoadImage("{Helios}/Images/General/missing_image.png");

                    _imageControl = new Image
                    {
                        Source = _image,
                        Width = _image.Width,
                        Height = _image.Height,
                    };
                    _visualBrush = new VisualBrush(_imageControl);

                    _visualBrush.Stretch = Stretch.Fill;
                    _visualBrush.TileMode = TileMode.None;
                    _visualBrush.Viewport = new Rect(0d, 0d, 1d, 1d);
                    _visualBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                }
                else
                {
                    _imageControl = new Image
                    {
                        Source = _image,
                        Width = _image.Width,
                        Height = _image.Height,

                    };


                    _visualBrush = new VisualBrush(_imageControl);

                    switch (profileImage.Alignment)
                    {
                        case ImageAlignment.Centered:
                            _visualBrush.Stretch = Stretch.None;
                            _visualBrush.TileMode = TileMode.None;
                            _visualBrush.Viewport = new Rect(0d, 0d, 1d, 1d);
                            _visualBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                            break;

                        case ImageAlignment.Stretched:
                            _visualBrush.Stretch = Stretch.Fill;
                            _visualBrush.TileMode = TileMode.None;
                            _visualBrush.Viewport = new Rect(0d, 0d, 1d, 1d);
                            _visualBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                            break;

                        case ImageAlignment.Tiled:
                            _visualBrush.Stretch = Stretch.None;
                            _visualBrush.TileMode = TileMode.Tile;
                            _visualBrush.Viewport = new Rect(0d, 0d, _image.Width, _image.Height);
                            _visualBrush.ViewportUnits = BrushMappingMode.Absolute;
                            break;
                    }
                }
            }
            else
            {
                _image = null;
            }
        }
    }
}
