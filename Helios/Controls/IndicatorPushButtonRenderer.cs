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
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    class IndicatorPushButtonRenderer : HeliosVisualRenderer
    {
        private ImageSource _image;
        private ImageSource _pushedImage;

        private ImageSource _indicatorOnImage;
        private ImageSource _pushedIndicatorOnImage;

        private PathGeometry _glyphPath;

        private Brush _onTextBrush;
        private Brush _offTextBrush;

        private Brush _onGlyphBrush;
        private Pen _onGlyphPen;
        private Brush _offGlyphBrush;
        private Pen _offGlyphPen;

        private Rect _imageRect;

        protected override void OnRender(DrawingContext drawingContext)
        {
            IndicatorPushButton pushButton = Visual as IndicatorPushButton;

            if (pushButton.Pushed && pushButton.Indicator && _pushedImage != null)
            {
                drawingContext.DrawImage(_pushedIndicatorOnImage, _imageRect);
            }
            else if (pushButton.Pushed && !pushButton.Indicator && _pushedImage != null)
            {
                drawingContext.DrawImage(_pushedImage, _imageRect);
            }
            else if (!pushButton.Pushed && pushButton.Indicator && _indicatorOnImage != null)
            {
                drawingContext.DrawImage(_indicatorOnImage, _imageRect);
            }
            else if (_image != null)
            {
                drawingContext.DrawImage(_image, _imageRect);
            }

            if (pushButton.Pushed)
            {
                drawingContext.PushTransform(new TranslateTransform(pushButton.TextPushOffset.X, pushButton.TextPushOffset.Y));
            }

            if (pushButton.Glyph != PushButtonGlyph.None)
            {
                drawingContext.DrawGeometry(pushButton.Indicator ? _onGlyphBrush : _offGlyphBrush, pushButton.Indicator ? _onGlyphPen : _offGlyphPen, _glyphPath);
            }
            pushButton.TextFormat.RenderText(drawingContext, pushButton.Indicator ? _onTextBrush : _offTextBrush, pushButton.Text, _imageRect);

            if (pushButton.Pushed)
            {
                drawingContext.Pop();
            }
        }

        protected override void OnRefresh()
        {
            IndicatorPushButton pushButton = Visual as IndicatorPushButton;

            if (pushButton != null)
            {
                _imageRect.Width = pushButton.Width;
                _imageRect.Height = pushButton.Height;
                _image = ConfigManager.ImageManager.LoadImage(pushButton.Image);
                _pushedImage = ConfigManager.ImageManager.LoadImage(pushButton.PushedImage);
                _indicatorOnImage = ConfigManager.ImageManager.LoadImage(pushButton.IndicatorOnImage);
                _pushedIndicatorOnImage = ConfigManager.ImageManager.LoadImage(pushButton.PushedIndicatorOnImage);
                _onTextBrush = new SolidColorBrush(pushButton.OnTextColor);
                _offTextBrush = new SolidColorBrush(pushButton.TextColor);

                _onGlyphBrush = new SolidColorBrush(pushButton.OnGlyphColor);
                _onGlyphPen = new Pen(_onGlyphBrush, pushButton.GlyphThickness);

                _offGlyphBrush = new SolidColorBrush(pushButton.GlyphColor);
                _offGlyphPen = new Pen(_offGlyphBrush, pushButton.GlyphThickness);

                Point center = new Point(pushButton.Width / 2d, pushButton.Height / 2d);
                _glyphPath = new PathGeometry();
                switch (pushButton.Glyph)
                {
                    case PushButtonGlyph.Circle:
                        double radius = (Math.Min(pushButton.Width, pushButton.Height) / 2d) * pushButton.GlyphScale;
                        EllipseGeometry ellipse = new EllipseGeometry(center, radius, radius);
                        _onGlyphBrush = null;
                        _offGlyphBrush = null;
                        _glyphPath.AddGeometry(ellipse);
                        break;

                    case PushButtonGlyph.RightArrow:
                        _glyphPath.Figures.Add(GetArrowFigure(true, pushButton));
                        break;

                    case PushButtonGlyph.LeftArrow:
                        _glyphPath.Figures.Add(GetArrowFigure(false, pushButton));
                        break;

                    case PushButtonGlyph.UpCaret:
                        double offsetX = center.X * pushButton.GlyphScale;
                        double offsetY = offsetX / 2d;
                        PathFigure figure = new PathFigure();
                        figure.IsClosed = false;
                        figure.IsFilled = false;
                        figure.StartPoint = new Point(center.X - offsetX, center.Y + offsetY);
                        figure.Segments.Add(new LineSegment(new Point(center.X, center.Y - offsetY), true));
                        figure.Segments.Add(new LineSegment(new Point(center.X + offsetX, center.Y + offsetY), true));
                        _glyphPath.Figures.Add(figure);
                        break;
                }
            }
            else
            {
                _image = null;
                _pushedImage = null;
                _onTextBrush = null;
                _offTextBrush = null;
                _onGlyphPen = null;
                _offGlyphPen = null;
                _glyphPath = null;
                _indicatorOnImage = null;
                _pushedIndicatorOnImage = null;
            }
        }

        private PathFigure GetArrowFigure(bool Right, IndicatorPushButton pushButton)
        {
            double y = pushButton.Height / 2d;
            double arrowLength = pushButton.Width * pushButton.GlyphScale;
            double padding = (pushButton.Width - arrowLength) / 2d;
            double arrowLineLength = arrowLength * .6d;
            double headHeightOffset = pushButton.GlyphThickness * 2d;

            Point lineStart, lineEnd, head, head1, head2;

            if (Right)
            {
                lineStart = new Point(padding, y);
                lineEnd = new Point(lineStart.X + arrowLineLength, y);
                head = new Point(pushButton.Width - padding, y);
                head1 = new Point(lineEnd.X, y - headHeightOffset);
                head2 = new Point(lineEnd.X, y + headHeightOffset);
            }
            else
            {
                lineStart = new Point(pushButton.Width - padding, y);
                lineEnd = new Point(lineStart.X - arrowLineLength, y);
                head = new Point(padding, y);
                head2 = new Point(lineEnd.X, y - headHeightOffset);
                head1 = new Point(lineEnd.X, y + headHeightOffset);
            }

            PathFigure arrow = new PathFigure();
            arrow.StartPoint = lineStart;
            arrow.Segments.Add(new LineSegment(lineEnd, true));
            arrow.Segments.Add(new LineSegment(head1, false));
            arrow.Segments.Add(new LineSegment(head, false));
            arrow.Segments.Add(new LineSegment(head2, false));
            arrow.Segments.Add(new LineSegment(lineEnd, false));

            return arrow;
        }
    }
}
