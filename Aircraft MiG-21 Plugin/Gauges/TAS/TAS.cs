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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.TAS
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.TAS", "TAS", "MiG-21", typeof(GaugeRenderer))]
    public class TAS : BaseGauge
    {
        private GaugeNeedle _currentTasNeedle;
        private GaugeNeedle _currentMNeedle;

        private CalibrationPointCollectionDouble _needleTasCalibration;
        private CalibrationPointCollectionDouble _needleMCalibration;

        private HeliosValue _currentTAS;
        private HeliosValue _currentM;

        public TAS()
            : base("TAS", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleTasCalibration = new CalibrationPointCollectionDouble(167d, 0d, 833d, 335d);
            _needleTasCalibration.Add(new CalibrationPointDouble(278d, 46d));  
            _needleTasCalibration.Add(new CalibrationPointDouble(417d, 127d)); 
            _needleTasCalibration.Add(new CalibrationPointDouble(555d, 207d));

            _needleMCalibration = new CalibrationPointCollectionDouble(0.0d, 0d, 3.0d, 335d);
            _needleMCalibration.Add(new CalibrationPointDouble(0.6d, 0d));
            _needleMCalibration.Add(new CalibrationPointDouble(1d, 56d));
            _needleMCalibration.Add(new CalibrationPointDouble(1.8d, 167d));
            _needleTasCalibration.Add(new CalibrationPointDouble(2.0d, 223d));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/TAS/tas_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentTasNeedle = new GaugeNeedle("{MiG-21}/Gauges/TAS/tas_needle.xaml", center, new Size(24, 135), new Point(12, 127), 11d);
            Components.Add(_currentTasNeedle);

            _currentMNeedle = new GaugeNeedle("{MiG-21}/Gauges/TAS/m_needle.xaml", center, new Size(32, 185), new Point(16, 127), 11d);
            Components.Add(_currentMNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentTAS = new HeliosValue(this, BindingValue.Empty, "", "TAS", "Current TAS", "", BindingValueUnits.Numeric);
            _currentTAS.Execute += CurrentTas_Execute;
            Actions.Add(_currentTAS);

            _currentM = new HeliosValue(this, BindingValue.Empty, "", "M", "Current M", "", BindingValueUnits.Numeric);
            _currentM.Execute += CurrentM_Execute;
            Actions.Add(_currentM);
        }

        private void CurrentTas_Execute(object action, HeliosActionEventArgs e)
        {
            _currentTAS.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentTasNeedle.Rotation = _needleTasCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void CurrentM_Execute(object action, HeliosActionEventArgs e)
        {
            _currentM.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentMNeedle.Rotation = _needleMCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
