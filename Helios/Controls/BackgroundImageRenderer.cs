//  Copyright 2014 Craig Courtney
//  Copyright 2019 Helios Contributors
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

using System.Windows;
using System.Windows.Media;
using GadrocsWorkshop.Helios.Controls.Capabilities;

namespace GadrocsWorkshop.Helios.Controls
{
    public class BackgroundImageRenderer : HeliosVisualRenderer
    {
        private ImageBrush _backgroundBrush;
        private Rect _backgroundRectangle;

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Visual is IBackgroundImage && _backgroundBrush != null)
            {
                DrawImage(drawingContext, _backgroundBrush.ImageSource, _backgroundRectangle);
            }
        }

        protected override void OnRefresh()
        {
            if (Visual is IBackgroundImage control)
            {
                _backgroundRectangle = new Rect(0, 0, Visual.Width, Visual.Height);
                _backgroundBrush = CreateImageBrush(control.BackgroundImage, Visual.ImageRefresh);
            }
            else
            {
                _backgroundBrush = null;
            }
        }

        private ImageBrush CreateImageBrush(string imagefile, bool ImageRefresh)
        {
            IImageManager3 refreshCapableImage = ConfigManager.ImageManager as IImageManager3;
            LoadImageOptions loadOptions = ImageRefresh ? LoadImageOptions.ReloadIfChangedExternally : LoadImageOptions.None;

            ImageSource image = refreshCapableImage.LoadImage(imagefile, loadOptions);
            if (image == null)
            {
                return null;
            }

            ImageBrush imageBrush = new ImageBrush(image)
            {
                Stretch = Stretch.Fill,
                TileMode = TileMode.None,
                Viewport = new Rect(0d, 0d, 1d, 1d),
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox
            };
            return imageBrush;
        }
    }
}
