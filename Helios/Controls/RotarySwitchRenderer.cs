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
    using AvalonDock.Controls;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class RotarySwitchRenderer : HeliosVisualRenderer
    {
        private struct SwitchPositionLabel
        {
            public Point Location;
            public FormattedText Text;

            public SwitchPositionLabel(FormattedText text, Point location)
            {
                Location = location;
                Text = text;
            }
        }

        private List<SwitchPositionLabel> _labels = new List<SwitchPositionLabel>();

        private GeometryDrawing _lines;
        private ImageSource _image;
        private Rect _imageRect;
        private Point _center;
        private VisualBrush _visualBrush = new VisualBrush();
        private Image _imageControl = new Image();
        private DrawingContext _ctx;
        private DrawingGroup _group = new DrawingGroup();


        private static readonly Pen DragPen = new Pen(Brushes.White, 1.0)
        {
            DashStyle = new DashStyle(new[] { 6d, 6d }, 0d)
        };
        private static readonly Pen HeadingPen = new Pen(Brushes.White, 1.0);

        protected override void OnRender(DrawingContext drawingContext)
        {
            bool needsEffect = NeedsEffect;
            RotarySwitch rotarySwitch = Visual as RotarySwitch;
            if (rotarySwitch != null)
            {
                _ctx = _group.Open();
                if (rotarySwitch.DrawLines)
                {
                    _ctx.DrawDrawing(_lines);
                }
                foreach (SwitchPositionLabel label in _labels)
                {
                    _ctx.DrawText(label.Text, label.Location);
                }

                _ctx.PushTransform(new RotateTransform(rotarySwitch.KnobRotation, _center.X, _center.Y));
                _ctx.DrawRectangle(_visualBrush, null, _imageRect);

                if (rotarySwitch.VisualizeDragging)
                {
                    double length = (rotarySwitch.DragPoint - _center).Length;
                    _ctx.DrawLine(HeadingPen, _center, _center + new Vector(0d, -length));
                }
                _ctx.Pop();

                if (rotarySwitch.VisualizeDragging)
                {
                    _ctx.DrawLine(DragPen, _center, rotarySwitch.DragPoint);
                }
                _ctx.Close();
                DrawGroup(drawingContext, _group);
            }
        }

        protected override void OnRefresh()
        {
            RotarySwitch rotarySwitch = Visual as RotarySwitch;
            if (rotarySwitch != null)
            {
                IImageManager3 refreshCapableImage = ConfigManager.ImageManager as IImageManager3;
                LoadImageOptions loadOptions = rotarySwitch.ImageRefresh ? LoadImageOptions.ReloadIfChangedExternally : LoadImageOptions.None;

                _imageRect.Width = rotarySwitch.Width;
                _imageRect.Height = rotarySwitch.Height;
                _image = refreshCapableImage.LoadImage(rotarySwitch.KnobImage, loadOptions);
                _imageControl.Source = _image;
                _visualBrush.Visual = _imageControl;
                _center = new Point(rotarySwitch.Width / 2d, rotarySwitch.Height / 2d);

                _lines = new GeometryDrawing();
                _lines.Pen = new Pen(new SolidColorBrush(rotarySwitch.LineColor), rotarySwitch.LineThickness);

                _labels.Clear();

                Vector v1 = new Point(_center.X, 0) - _center;
                double lineLength = v1.Length * rotarySwitch.LineLength;
                double labelDistance = v1.Length * rotarySwitch.LabelDistance;
                v1.Normalize();
                GeometryGroup lineGroup = new GeometryGroup();
                Brush labelBrush = new SolidColorBrush(rotarySwitch.LabelColor);
                foreach (RotarySwitchPosition position in rotarySwitch.Positions)
                {
                    Matrix m1 = new Matrix();
                    m1.Rotate(position.Rotation);

                    if (rotarySwitch.DrawLines)
                    {
                        Vector v2 = v1 * m1;

                        Point startPoint = _center;
                        Point endPoint = startPoint + (v2 * lineLength);

                        lineGroup.Children.Add(new LineGeometry(startPoint, endPoint));
                    }

                    if (rotarySwitch.DrawLabels)
                    {
                        FormattedText labelText = rotarySwitch.LabelFormat.GetFormattedText(labelBrush, position.Name);
                        labelText.TextAlignment = TextAlignment.Center;
                        labelText.MaxTextWidth = rotarySwitch.Width;
                        labelText.MaxTextHeight = rotarySwitch.Height;

                        if (rotarySwitch.MaxLabelHeight > 0d && rotarySwitch.MaxLabelHeight < rotarySwitch.Height)
                        {
                            labelText.MaxTextHeight = rotarySwitch.MaxLabelHeight;
                        }
                        if (rotarySwitch.MaxLabelWidth > 0d && rotarySwitch.MaxLabelWidth < rotarySwitch.Width)
                        {
                            labelText.MaxTextWidth = rotarySwitch.MaxLabelWidth;
                        }

                        Point location = _center + (v1 * m1 * labelDistance);
                        if (position.Rotation <= 10d || position.Rotation >= 350d)
                        {
                            location.X -= labelText.MaxTextWidth / 2d;
                            location.Y -= labelText.Height;
                        }
                        else if (position.Rotation < 170d)
                        {
                            location.X -= (labelText.MaxTextWidth - labelText.Width) / 2d;
                            location.Y -= labelText.Height / 2d;
                        }
                        else if (position.Rotation <= 190d)
                        {
                            location.X -= labelText.MaxTextWidth / 2d;
                        }
                        else
                        {
                            location.X -= (labelText.MaxTextWidth + labelText.Width) / 2d;
                            location.Y -= labelText.Height / 2d;
                        }

                        _labels.Add(new SwitchPositionLabel(labelText, location));
                    }
                }
                _lines.Geometry = lineGroup;
                _lines.Freeze();
            }
            else
            {
                _image = null;
                _lines = null;
                _labels.Clear();
            }
        }
    }
}
