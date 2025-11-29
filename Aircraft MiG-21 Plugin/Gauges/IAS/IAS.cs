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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.IAS
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.IAS", "IAS", "MiG-21", typeof(GaugeRenderer))]
    public class IAS : BaseGauge
    {

        private GaugeNeedle _currentNeedle;
        private GaugeNeedle _currentBack;

        private CalibrationPointCollectionDouble _needleCalibration;
        private CalibrationPointCollectionDouble _backCalibration;

        private HeliosValue _currentIas;

        public IAS()
            : base("IAS", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 555.55d, 720d);
            _needleCalibration.Add(new CalibrationPointDouble(83.33d, 108d));
            _needleCalibration.Add(new CalibrationPointDouble(166.67d, 216d));
            _needleCalibration.Add(new CalibrationPointDouble(250d, 324d));
            _needleCalibration.Add(new CalibrationPointDouble(333.34d, 432d));
            _needleCalibration.Add(new CalibrationPointDouble(416.67d, 540d));

            _backCalibration = new CalibrationPointCollectionDouble(0d, 0d, 555.55d, 90d);
            _backCalibration.Add(new CalibrationPointDouble(277, 90d));
            _backCalibration.Add(new CalibrationPointDouble(270, 0d));
            _currentBack = new GaugeNeedle("{MiG-21}/Gauges/IAS/ias_back.xaml", center, new Size(340, 340), center, -90);
            Components.Add(_currentBack);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/IAS/ias_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/Common/generic_needle.xaml", center, new Size(32, 185), new Point(16, 127));
            Components.Add(_currentNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentIas = new HeliosValue(this, BindingValue.Empty, "", "IAS", "Current IAS", "", BindingValueUnits.Numeric);
            _currentIas.Execute += CurrentIas_Execute;
            _currentIas.Execute += CurrentBack_Execute;
            Actions.Add(_currentIas);

        }

        private void CurrentIas_Execute(object action, HeliosActionEventArgs e)
        {
            _currentIas.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void CurrentBack_Execute(object action, HeliosActionEventArgs e)
        {
            _currentIas.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentBack.Rotation = _backCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
