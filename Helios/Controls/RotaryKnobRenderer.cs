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
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    public class RotaryKnobRenderer : HeliosVisualRenderer
    {
        private ImageSource _image;
        private ImageBrush _brush;
        private Rect _imageRect;
        private Point _center;
        private static readonly Pen DragPen = new Pen(Brushes.White, 1.0)
        {
            DashStyle = new DashStyle(new [] {6d, 6d}, 0d)
        };
        private static readonly Pen HeadingPen = new Pen(Brushes.White, 1.0);

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            RotaryKnob rotary = Visual as RotaryKnob;
            if (rotary != null)
            {
                drawingContext.PushTransform(new RotateTransform(rotary.KnobRotation, _center.X, _center.Y));
                DrawImage(drawingContext, _image, _imageRect);
                if (rotary.VisualizeDragging)
                {
                    double length = (rotary.DragPoint - _center).Length;
                    drawingContext.DrawLine(HeadingPen, _center, _center + new Vector(0d, -length));
                }
                drawingContext.Pop();

                if (rotary.VisualizeDragging)
                {
                    drawingContext.DrawLine(DragPen, _center, rotary.DragPoint);
                }
            }
        }

        protected override void OnRefresh()
        {
            RotaryKnob rotary = Visual as RotaryKnob;
            if (rotary != null)
            {
                IImageManager3 refreshCapableImage = ConfigManager.ImageManager as IImageManager3;
                LoadImageOptions loadOptions = rotary.ImageRefresh ? LoadImageOptions.ReloadIfChangedExternally : LoadImageOptions.None;
                _imageRect.Width = rotary.Width;
                _imageRect.Height = rotary.Height;

                _image = refreshCapableImage.LoadImage(rotary.KnobImage, loadOptions);
                _brush = new ImageBrush(_image);
                _center = new Point(rotary.Width / 2d, rotary.Height / 2d);
            }
            else
            {
                _image = null;
                _brush = null;
            }
        }
    }
}
