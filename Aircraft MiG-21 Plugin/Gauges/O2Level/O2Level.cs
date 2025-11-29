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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.O2Level
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.O2Level", "O2 Level", "MiG-21", typeof(GaugeRenderer))]
    public class O2Level : BaseGauge
    {
        private GaugeNeedle _currentNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private HeliosValue _currentO2Level;
        private GaugeImage _rLungImage;
        private GaugeImage _lLungImage;
        private HeliosValue _lLungFlag;

        public O2Level()
            : base("O2Level", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0, 0d, 150d, 158d);
            _needleCalibration.Add(new CalibrationPointDouble(10d, 4d));
            _needleCalibration.Add(new CalibrationPointDouble(50d, 47.8d));
            _needleCalibration.Add(new CalibrationPointDouble(100d, 103d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/O2Level/o2_level_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/Common/small_needle.xaml", center, new Size(23, 164), new Point(11.5, 127), 277.5d);
            Components.Add(_currentNeedle);

            _lLungImage = new GaugeImage("{MiG-21}/Gauges/O2Level/o2_level_blinker_l.xaml", new Rect(136, 194, 25, 63.9));
            Components.Add(_lLungImage);

            _rLungImage = new GaugeImage("{MiG-21}/Gauges/O2Level/o2_level_blinker_r.xaml", new Rect(180, 194, 25, 63.9));
            Components.Add(_rLungImage);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentO2Level = new HeliosValue(this, BindingValue.Empty, "", "O2 Level", "Current O2 Level", "", BindingValueUnits.Numeric);
            _currentO2Level.Execute += CurrentO2Level_Execute;
            Actions.Add(_currentO2Level);

            _lLungFlag = new HeliosValue(this, new BindingValue(false), "", "Lung Blinkers", "White when working", "True if displayed.", BindingValueUnits.Boolean);
            _lLungFlag.Execute += LFlags_Execute;
            Actions.Add(_lLungFlag);
        }

        private void CurrentO2Level_Execute(object action, HeliosActionEventArgs e)
        {
            _currentO2Level.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void LFlags_Execute(object action, HeliosActionEventArgs e)
        {
            _lLungImage.IsHidden = !e.Value.BoolValue;
            _rLungImage.IsHidden = !e.Value.BoolValue;
        }
    }
}
