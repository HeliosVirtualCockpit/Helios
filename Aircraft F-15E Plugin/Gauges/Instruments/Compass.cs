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

namespace GadrocsWorkshop.Helios.Gauges.F15E.Instruments.Compass
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F15E.Instruments.Compass", "Compass", "F-15E Strike Eagle", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class CompassGauge : BaseGauge
    {
        private HeliosValue _heading;
        private HeliosValue _roll;
        private HeliosValue _pitch;
        private GaugeNeedle _ball;

        private CalibrationPointCollectionDouble _headingCalibration;
        private CalibrationPointCollectionDouble _rollCalibration;
        private CalibrationPointCollectionDouble _pitchCalibration;

        public CompassGauge(string name, Size size, string device)
            : base(name, new Size(200,200))
        {
            Point center = new Point(100d, 100d);

            _headingCalibration = new CalibrationPointCollectionDouble(0d, -432d, 360d, 432d);
            _ball = new GaugeNeedle("{F-15E}/Gauges/Common/Compass_Tape.xaml", center, new Size(1112d, 60d), new Point(279.25d * 2d, 60d / 2d));
            _ball.Clip = new EllipseGeometry(center, 100d, 100d);
            Components.Add(_ball);


            _heading = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "Magnetic Compass Heading", "Current compass heading of the aircraft in degrees.", "(0 to 360)", BindingValueUnits.Degrees);
            _heading.Execute += new HeliosActionHandler(Heading_Execute);
            Actions.Add(_heading);

            _rollCalibration = new CalibrationPointCollectionDouble(-180d, -20d, 180d, 20d);

            _roll = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "Magnetic Compass Roll", "Current roll of the compass rose in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Roll_Execute);
            Actions.Add(_roll);

            _pitchCalibration = new CalibrationPointCollectionDouble(-90d, 60d, 90d, -60d);

            _pitch = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "Magnetic Compass Pitch", "Current pitch of the compass rose in degrees.", "(-90 to +90)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

        }


        void Heading_Execute(object action, HeliosActionEventArgs e)
        {
            _heading.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.HorizontalOffset = _headingCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Roll_Execute(object action, HeliosActionEventArgs e)
        {
            _roll.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Rotation = _rollCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.VerticalOffset = _pitchCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
