//  Copyright 2018 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.UH60L.Stabilator.Indicator", "Stabilator Position Instrument", "UH-60L", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class StabInstrument : BaseGauge
    {
        private HeliosValue _stabPosition;
        private GaugeNeedle _needle;
        private GaugeNeedle _offFlagNeedle;
        private HeliosValue _offIndicator;
        private CalibrationPointCollectionDouble _needleCalibration;

        public StabInstrument(string name, Size size)
            : base(name, size)
        {
             //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{Helios}/Images/UH60L/StabScale.xaml", new Rect(0d, 0d, 276d, 233d)));

            _needleCalibration = new CalibrationPointCollectionDouble(-1d, -7.5d, 1d, 37.5d);
            _needleCalibration.Add(new CalibrationPointDouble(360d, 270d));
            _needle = new GaugeNeedle("{Helios}/Images/UH60L/StabNeedle.xaml", new Point(77d, 81d), new Size(171d, 28d), new Point(14d, 14d), 0d);
            Components.Add(_needle);
            _stabPosition = new HeliosValue(this, new BindingValue(0d), name, "Stabilator Position", "Current position of Stabilator.", "Number between -1 and 1", BindingValueUnits.Numeric);
            _stabPosition.Execute += new HeliosActionHandler(StabPositionExecute);
            Actions.Add(_stabPosition);

            _offFlagNeedle = new GaugeNeedle("{Helios}/Images/UH60L/StabOffFlag.xaml", new Point(77d, 81d), new Size(25d, 59d), new Point(0d, -80d), -20d);
            Components.Add(_offFlagNeedle);
            _offIndicator = new HeliosValue(this, new BindingValue(0d), name, "Off flag", "Flag to show position indicator is off.", "", BindingValueUnits.Boolean);
            _offIndicator.Execute += new HeliosActionHandler(OffIndicator_Execute);
            Values.Add(_offIndicator);
            Actions.Add(_offIndicator);

            Components.Add(new GaugeImage("{Helios}/Images/UH60L/StabSurface.xaml", new Rect(0d, 0d, 276d, 233d)));

        }

        void StabPositionExecute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void OffIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlagNeedle.Rotation = e.Value.BoolValue ? 30d:0d ;
        }
    }
}
