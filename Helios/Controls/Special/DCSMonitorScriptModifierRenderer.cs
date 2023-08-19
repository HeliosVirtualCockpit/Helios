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

namespace GadrocsWorkshop.Helios.Controls.Special
{
    internal class DCSMonitorScriptModifierRenderer : ViewportExtentRenderer
    {
        private ImageBrush _imageBrush;
        private Rect _rectangle;
        private Brush _fontBrush;

        protected override void OnRender(DrawingContext drawingContext)
        {
            // only in design mode
            if (ConfigManager.Application.ShowDesignTimeControls)
            {
                if (Visual is DCSMonitorScriptModifier)
                {
                    drawingContext.DrawRectangle(_imageBrush, null, _rectangle);
                    base.OnRender(drawingContext);
                }
            }
        }

        protected override void OnRefresh()
        {
            if (Visual is DCSMonitorScriptModifier control)
            {
                base.OnRefresh();
                _rectangle = new Rect(0, 0, Visual.Width, Visual.Height);
                _imageBrush = CreateImageBrush(control.Image);
            }
        }

        private ImageBrush CreateImageBrush(string imagefile)
        {
            ImageSource image = ConfigManager.ImageManager.LoadImage(imagefile);
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
