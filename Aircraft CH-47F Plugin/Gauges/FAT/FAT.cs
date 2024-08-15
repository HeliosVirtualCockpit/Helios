//  Copyright 2014 Craig Courtney
//  Copyright 2021 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.CH47F.FAT
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.CH47F.FAT", "Free Air Temp Gauge", "CH-47F Chinook", typeof(GaugeRenderer),HeliosControlFlags.None)]
    public class FAT : BaseGauge
    {
        private HeliosValue _FreeAirTemp;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;

        public FAT()
            : base("FAT", new Size(364, 376))
        {

            Components.Add(new GaugeImage("{AH-64D}/Images/FAT/fat_faceplate.xaml", new Rect(32d, 38d, 300, 300)));

            _needleCalibration = new CalibrationPointCollectionDouble(-70d, -161d, 50d, 160d)
            {
                //new CalibrationPointDouble(50d, 0d)
            };
            _needle = new GaugeNeedle("{Helios}/Gauges/A-10/Common/needle_a.xaml", new Point(182d, 188d), new Size(22, 165), new Point(11, 130), 0d);
            Components.Add(_needle);

            //Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            _FreeAirTemp = new HeliosValue(this, new BindingValue(0d), "", "Free Air Temperature", "Current Free Air Temperature.", "(-70 - +50)", BindingValueUnits.Celsius);
            _FreeAirTemp.Execute += new HeliosActionHandler(FreeAirTemp_Execute);
            Actions.Add(_FreeAirTemp);
        }

        void FreeAirTemp_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
