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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.BarGauge
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    //[HeliosControl("Helios.UH60L.BarGauge41Green", "Bar Gauge 41 Segment Green Display", "UH-60L", typeof(GaugeRenderer), HeliosControlFlags.None)]
    internal class BarGauge41 : BarGauge
    {
        public BarGauge41()
            : base("Bar Gauge Display", new Size(40, 41 * 18), "{Helios}/Images/UH60L/SegmentBarDisplay41Green.xaml", 41)
        {
        }
    }
}
