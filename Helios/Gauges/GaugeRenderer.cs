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

namespace GadrocsWorkshop.Helios.Gauges
{
    using System.Windows.Media;

    public class GaugeRenderer : HeliosVisualRenderer
    {
        private double _scaleX = 1d;
        private double _scaleY = 1d;

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Visual is IGauge gauge)
            {
                foreach (GaugeComponent component in gauge.Components)
                {
                    component.Render(drawingContext);
                }
            }
        }

        protected override void OnRefresh()
        {
            if (Visual is IGauge gauge)
            {
                _scaleX = gauge.Width / gauge.NativeWidth;
                _scaleY = gauge.Height / gauge.NativeHeight;
                foreach (GaugeComponent component in gauge.Components)
                {
                    component.Refresh(_scaleX, _scaleY);
                    component.ImageRefresh = false;
                }
            }
        }
    }
}
