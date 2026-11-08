//  Copyright 2014 Craig Courtney
//  Copyright 2026 Helios Contributors
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

    public class DualPotentiometerRenderer : HeliosVisualRenderer
    {
        private ImageSource _image1;
        private ImageSource _image2;
        private Rect _rect1;
        private Rect _rect2;
        private Point _center;
        private readonly DrawingGroup _group = new DrawingGroup();

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!(Visual is DualPotentiometer pots))
            {
                return;
            }

            DrawingContext ctx = _group.Open();

            // keep the extent of the drawing equal to the control, so that it does not move
            // or get rescaled as rotated images change the bounding box
            ctx.DrawRectangle(Brushes.Transparent, null, new Rect(0d, 0d, pots.Width, pots.Height));

            if (pots.Potentiometer2OnTop)
            {
                DrawPotentiometer(ctx, _image1, _rect1, pots.Potentiometer1Rotation);
                DrawPotentiometer(ctx, _image2, _rect2, pots.Potentiometer2Rotation);
            }
            else
            {
                DrawPotentiometer(ctx, _image2, _rect2, pots.Potentiometer2Rotation);
                DrawPotentiometer(ctx, _image1, _rect1, pots.Potentiometer1Rotation);
            }
            ctx.Close();
            DrawGroup(drawingContext, _group);
        }

        private void DrawPotentiometer(DrawingContext ctx, ImageSource image, Rect rect, double rotation)
        {
            if (image == null)
            {
                return;
            }
            ctx.PushTransform(new RotateTransform(rotation, _center.X, _center.Y));
            ctx.DrawImage(image, rect);
            ctx.Pop();
        }

        protected override void OnRefresh()
        {
            if (!(Visual is DualPotentiometer pots))
            {
                _image1 = null;
                _image2 = null;
                return;
            }

            IImageManager3 imageManager = ConfigManager.ImageManager as IImageManager3;
            LoadImageOptions loadOptions = pots.ImageRefresh ? LoadImageOptions.ReloadIfChangedExternally : LoadImageOptions.None;

            _center = new Point(pots.Width / 2d, pots.Height / 2d);
            _rect1 = CenteredRect(pots, pots.Potentiometer1Scale);
            _rect2 = CenteredRect(pots, pots.Potentiometer2Scale);

            _image1 = string.IsNullOrEmpty(pots.Potentiometer1Image) ? null : imageManager?.LoadImage(pots.Potentiometer1Image, loadOptions);
            _image2 = string.IsNullOrEmpty(pots.Potentiometer2Image) ? null : imageManager?.LoadImage(pots.Potentiometer2Image, loadOptions);
        }

        private static Rect CenteredRect(DualPotentiometer pots, double scale)
        {
            scale = double.IsNaN(scale) || scale < 0d ? 0d : scale;
            double width = pots.Width * scale;
            double height = pots.Height * scale;
            return new Rect((pots.Width - width) / 2d, (pots.Height - height) / 2d, width, height);
        }
    }
}
