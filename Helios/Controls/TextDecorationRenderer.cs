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
    using System;
    using System.Windows;
    using System.Windows.Media;
    
    public class TextDecorationRenderer : HeliosVisualRenderer
    {
        private Rect _rectangle;
        private Brush _backgroundBrush;
        private Brush _fontBrush;

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            TextDecoration profileText = Visual as TextDecoration;
            if (profileText.FillBackground && profileText.BackgroundColor.A > 0)
            {
                DrawRectangle(drawingContext, _backgroundBrush, null, _rectangle);
            }
            DrawText(drawingContext, profileText, _fontBrush, profileText.Text, _rectangle);
        }

        //protected override void OnRender(DrawingContext drawingContext, double scaleX, double scaleY)
        //{
        //    TextDecoration profileText = Visual as TextDecoration;
        //    Rect scaledRect = new Rect(_rectangle.X, _rectangle.Y, _rectangle.Width * scaleX, _rectangle.Height * scaleY);
        //    if (profileText.FillBackground && profileText.BackgroundColor.A > 0)
        //    {
        //        drawingContext.DrawRectangle(_backgroundBrush, null, scaledRect);
        //    }
        //    profileText.Format.RenderText(drawingContext, _fontBrush, profileText.Text, scaledRect);         
        //}

        protected override void OnRefresh()
        {
            TextDecoration profileText = Visual as TextDecoration;
            if (profileText != null)
            {
                _backgroundBrush = new SolidColorBrush(profileText.BackgroundColor);
                _fontBrush = new SolidColorBrush(profileText.FontColor);
                _rectangle.Width = profileText.Width;
                _rectangle.Height = profileText.Height;
            }
        }
    }
}
