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
    using CommandLine;
    using System.Windows;
    using System.Windows.Media;

    public class ImageDecorationRenderer : HeliosVisualRenderer
    {
        private ImageSource _image;
        private Rect _imageRect;
        private ImageBrush _imageBrush;
        private Pen _borderPen;

        private DrawingContext _ctx;
        private DrawingGroup _group = new DrawingGroup();

        protected override void OnRender(DrawingContext drawingContext) {
            OnRender(drawingContext, 1d, 1d);
        }
        protected override void OnRender(DrawingContext drawingContext, double scaleX = 1d, double scaleY = 1d)
        {
            if (_image != null)
            {
                ImageDecorationBase profileImage = Visual as ImageDecorationBase;
                Rect scaledRect = new Rect(_imageRect.X, _imageRect.Y, _imageRect.Width * scaleX, _imageRect.Height * scaleY);

                /// Note: We need to use a new DrawingGroup in Profile Editor because the Bounds are cached internally and this
                /// causes problems when moving from a large drawing to a small one.  Primarily this is seen in the Profile Editor preview.
                DrawingGroup group = Visual.DesignMode ? new DrawingGroup() : _group;
                _ctx = group.Open();
                _ctx.DrawRoundedRectangle(_imageBrush, _borderPen, scaledRect, profileImage.CornerRadius, profileImage.CornerRadius);
                _ctx.Close();
                DrawGroup(drawingContext, group);
            }
        }
        protected override void OnRefresh()
        {
            ImageDecorationBase profileImage = Visual as ImageDecorationBase;
            if (profileImage == null || (profileImage.DesignTimeOnly && !ConfigManager.Application.ShowDesignTimeControls))
            {
                _image = null;
                return;
            }

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
                _image = ConfigManager.ImageManager.LoadImage("{Helios}/Images/General/MissingImage.xaml");
                _imageBrush = new ImageBrush(_image);
                _imageBrush.Stretch = Stretch.Fill;
                _imageBrush.TileMode = TileMode.None;
                _imageBrush.Viewport = new Rect(0d, 0d, 1d, 1d);
                _imageBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            }
            else
            {
                _imageBrush = new ImageBrush(_image);
                switch (profileImage.Alignment)
                {
                    case ImageAlignment.Centered:
                        _imageBrush.Stretch = Stretch.None;
                        _imageBrush.TileMode = TileMode.None;
                        _imageBrush.Viewport = new Rect(0d, 0d, 1d, 1d);
                        _imageBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                        break;

                    case ImageAlignment.Stretched:
                        _imageBrush.Stretch = Stretch.Fill;
                        _imageBrush.TileMode = TileMode.None;
                        _imageBrush.Viewport = new Rect(0d, 0d, 1d, 1d);
                        _imageBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                        break;

                    case ImageAlignment.Tiled:
                        _imageBrush.Stretch = Stretch.None;
                        _imageBrush.TileMode = TileMode.Tile;
                        _imageBrush.Viewport = new Rect(0d, 0d, _image.Width, _image.Height);
                        _imageBrush.ViewportUnits = BrushMappingMode.Absolute;
                        break;
                }
            }
        }
    }
}
