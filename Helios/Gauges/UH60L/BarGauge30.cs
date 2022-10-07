//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.BarGauge.Bargauge30
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    //[HeliosControl("Helios.UH60L.BarGauge30Green", "Bar Gauge 30 Segment Green Display", "UH-60L", typeof(GaugeRenderer), HeliosControlFlags.None)]
    internal class BarGauge30 : BarGauge
    {
        public BarGauge30()
            : base("Bar Gauge Display", new Size(40, 30 * 18), "{Helios}/Images/UH60L/SegmentBarDisplay30Green.xaml", 30)
        {
        }
    }
}
