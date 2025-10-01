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

    public class RectangleFillRenderer : HeliosVisualRenderer
    {
        private Rect _imageRect;
        private Brush _fillBrush;
        private Pen _borderPen;

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            RectangleFill profileRectangle = Visual as RectangleFill;
            DrawRoundedRectangle(drawingContext, _fillBrush, _borderPen, _imageRect, profileRectangle.CornerRadius, profileRectangle.CornerRadius);
        }

        protected override void OnRefresh()
        {
            RectangleFill profileRectangle = Visual as RectangleFill;
            if (profileRectangle != null)
            {
                _fillBrush = new SolidColorBrush(profileRectangle.FillColor);
                _imageRect.Width = profileRectangle.Width;
                if (profileRectangle.Inverse_direction!= true)
                { 
                    _imageRect.Y = profileRectangle.Height - (profileRectangle.Height * profileRectangle.FillHeight); // calculate the Y position of the rectangle to draw it from botton to top
                }
                _imageRect.Height = profileRectangle.Height * profileRectangle.FillHeight;

                if (profileRectangle.BorderThickness > 0d)
                {
                    _borderPen = new Pen(new SolidColorBrush(profileRectangle.BorderColor), profileRectangle.BorderThickness);
                }
                else
                {
                    _borderPen = null;
                }
            }
        }
    }
}