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


namespace GadrocsWorkshop.Helios.Gauges.MiG21.DA200
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.DA200", "DA200", "MiG-21", typeof(GaugeRenderer))]
    public class DA200 : BaseGauge
    {
        private GaugeNeedle _vviNeedle;
        private GaugeNeedle _slipBallNeedle;
        private GaugeNeedle _turnNeedle;

        private CalibrationPointCollectionDouble _vvineedleCalibration;
        private CalibrationPointCollectionDouble _slipBallCalibration;
        private CalibrationPointCollectionDouble _turnneedleCalibration;


        private HeliosValue _vviE;
        private HeliosValue _slipBallE;
        private HeliosValue _turnE;

        public DA200()
            : base("DA200", new Size(340, 340))
        {
            Point center = new Point(170, 170);
            Point bottomCenter = new Point(170, 240);
            Point veryBottomCenter = new Point(170, 310);

            _vvineedleCalibration = new CalibrationPointCollectionDouble(-400d, -135d, 400d, 135d);
            _vvineedleCalibration.Add(new CalibrationPointDouble(-200d, -121d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(-100d, -102d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(-50d, -80d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(-20d, -70d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(-10d, -35d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(0d, 0d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(10d, 35d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(20d, 70d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(50d, 80d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(100d, 102d));
            _vvineedleCalibration.Add(new CalibrationPointDouble(200d, 121d));

            _vviNeedle = new GaugeNeedle("{MiG-21}/Gauges/DA200/da200vvi_needle.xaml", center, new Size(32, 185), new Point(16, 127), 270);

            _slipBallCalibration = new CalibrationPointCollectionDouble(-1d, 57d, 1d, -57d);
            _slipBallCalibration.Add(new CalibrationPointDouble(0d, 0d));
            _slipBallNeedle = new GaugeNeedle("{MiG-21}/Gauges/DA200/da200_slip_ball.xaml", bottomCenter, new Size(23, 23), new Point(11.5, 11.5));

            _turnneedleCalibration = new CalibrationPointCollectionDouble(-0.04433d, -26d, 0.04433d, 26d);
            _turnNeedle = new GaugeNeedle("{MiG-21}/Gauges/DA200/da200bank_needle.xaml", veryBottomCenter, new Size(34, 218), new Point(17, 208));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/DA200/da200_faceplate.xaml", new Rect(0, 0, 340, 340)));
            Components.Add(new GaugeImage("{MiG-21}/Gauges/DA200/da200slip_faceplate.xaml", new Rect(99, 222, 140, 36)));
            Components.Add(_slipBallNeedle);
            Components.Add(_turnNeedle);
            Components.Add(_vviNeedle);
            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _vviE = new HeliosValue(this, BindingValue.Empty, "", "VVI", "Velocity", "", BindingValueUnits.Numeric);
            _vviE.Execute += vvi_Execute;
            Actions.Add(_vviE);

            _slipBallE = new HeliosValue(this, BindingValue.Empty, "", "Slip", "Slip Ball", "", BindingValueUnits.Numeric);
            _slipBallE.Execute += SlipBall_Execute;
            Actions.Add(_slipBallE);

            _turnE = new HeliosValue(this, BindingValue.Empty, "", "Turn", "Turn", "", BindingValueUnits.Numeric);
            _turnE.Execute += turn_Execute;
            Actions.Add(_turnE);

        }

        private void vvi_Execute(object action, HeliosActionEventArgs e)
        {
            _vviE.SetValue(e.Value, e.BypassCascadingTriggers);
            _vviNeedle.Rotation = _vvineedleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void SlipBall_Execute(object action, HeliosActionEventArgs e)
        {
            _slipBallE.SetValue(e.Value, e.BypassCascadingTriggers);
            _slipBallNeedle.HorizontalOffset = _slipBallCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void turn_Execute(object action, HeliosActionEventArgs e)
        {
            _turnE.SetValue(e.Value, e.BypassCascadingTriggers);
            _turnNeedle.Rotation = _turnneedleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
