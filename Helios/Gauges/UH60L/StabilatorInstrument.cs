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

    [HeliosControl("Helios.UH60L.Instruments", "Stabilator Position Instrument", "UH-60L", typeof(GaugeRenderer),HeliosControlFlags.None)]
    public class StabInstrument : BaseGauge
    {
        private HeliosValue _stabPosition;
        private GaugeNeedle _needle;
        private HeliosValue _offIndicator;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeImage _giOffIndicator;

        public StabInstrument()
            : base("Stabilator Position", new Size(276,233))
        {
             //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{Helios}/Images/UH60L/StabScale.xaml", new Rect(0d, 0d, 276d, 233d)));

            _giOffIndicator = new GaugeImage("{Helios}/Images/UH60L/StabOffFlag.xaml", new Rect(122d, 132d, 25d, 59d));
            Components.Add(_giOffIndicator);
            _offIndicator = new HeliosValue(this, new BindingValue(0d), "", "Off flag", "Indicator to show instrument is off.", "", BindingValueUnits.Boolean);
            _offIndicator.Execute += new HeliosActionHandler(OffIndicator_Execute);
            Values.Add(_offIndicator);
            Actions.Add(_offIndicator);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 180d, 180d);
            _needleCalibration.Add(new CalibrationPointDouble(360d, 270d));
            _needle = new GaugeNeedle("{Helios}/Images/UH60L/StabNeedle.xaml", new Point(77d, 81d), new Size(171d, 28d), new Point(14d, 14d), 0d);
            Components.Add(_needle);

            _stabPosition = new HeliosValue(this, new BindingValue(0d), "", "Stab Position Needle", "Current position of Stabilator.", "Degrees", BindingValueUnits.Degrees);
            _stabPosition.Execute += new HeliosActionHandler(StabPositionExecute);
            Actions.Add(_stabPosition);
            Components.Add(new GaugeImage("{Helios}/Images/UH60L/StabSurface.xaml", new Rect(0d, 0d, 276d, 233d)));

        }

        void StabPositionExecute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void OffIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giOffIndicator)].IsHidden = e.Value.BoolValue;
        }
    }
}
