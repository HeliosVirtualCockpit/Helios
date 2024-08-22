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
            : base("FAT", new Size(300, 300))
        {

            Components.Add(new GaugeImage("{CH-47F}/Gauges/FAT/Images/fat_faceplate.xaml", new Rect(0d, 0d, 300, 300)));

            _needleCalibration = new CalibrationPointCollectionDouble(-70d, 0d, 50d, 322d)
            {
                new CalibrationPointDouble(-50d, 40d),
                new CalibrationPointDouble(-30d, 95d),
                new CalibrationPointDouble(-10d, 172d),
                new CalibrationPointDouble(0d, 180d),
                new CalibrationPointDouble(10d, 212d),
                new CalibrationPointDouble(20d, 241d),
                new CalibrationPointDouble(30d, 270d)
            };
            _needle = new GaugeNeedle("{Helios}/Gauges/A-10/Common/needle_a.xaml", new Point(150d, 150d), new Size(22, 165), new Point(11, 130), -162d);
            Components.Add(_needle);

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
