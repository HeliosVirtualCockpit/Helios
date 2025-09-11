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

    public class MetricRenderer : HeliosVisualRenderer
    {
        private ImageSource _image;
        private ImageBrush _brush;
        private Rect _imageRect;
        private Point _center;

        protected override void OnRender ( System.Windows.Media.DrawingContext drawingContext )
        {
            Metric metric = Visual as Metric;
            if ( metric != null )
            {
                drawingContext.PushTransform( new TranslateTransform( metric.MetricTranslationX, metric.MetricTranslationY ) );
                drawingContext.PushTransform( new RotateTransform( metric.MetricRotation, _center.X, _center.Y - metric.MetricTranslationY ) );                
                DrawImage(drawingContext, _image, _imageRect );
                drawingContext.Pop( );
            }
        }

        protected override void OnRefresh ( )
        {
            Metric rotary = Visual as Metric;
            if ( rotary != null )
            {
                _imageRect.Width = rotary.Width;
                _imageRect.Height = rotary.Height;
                _image = ConfigManager.ImageManager.LoadImage( rotary.MetricImage );
                _brush = new ImageBrush( _image );
                _center = new Point( rotary.Width / 2d, rotary.Height / 2d );
            }
            else
            {
                _image = null;
                _brush = null;
            }
        }
    }
}
