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

namespace GadrocsWorkshop.Helios.Gauges.C130J.Compass
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.C130J;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.C130J.Compass", "Magnetic Compass", "C-130J Hercules", typeof(GaugeRenderer),HeliosControlFlags.None)]
    public class CompassGauge : CompositeBaseGauge
    {
        private HeliosValue _heading;
        private HeliosValue _roll;
        private HeliosValue _pitch;
        private GaugeNeedle _ball;

        private CalibrationPointCollectionDouble _headingCalibration;
        private CalibrationPointCollectionDouble _rollCalibration;
        private CalibrationPointCollectionDouble _pitchCalibration;

        private readonly string _name;

        public CompassGauge()
            : base("Magnetic Compass", new Size(265.750d, 281.592d))
        {
            SupportedInterfaces = new[] { typeof(C130JInterface) };
            _name = "Magnetic Compass";
            Size size = new Size(265.750d, 281.592d);
            Point center = new Point(132.875d, 149.092d);
            double widthScaling = size.Width / 100d;
            double heightScaling = size.Height / 100d * 0.6d;
            double tapeHeight = 50d * heightScaling;
            double tapeWidth = 556d * widthScaling;
            double tapeWidthCenter = 279.25d * widthScaling;  // this is not necessarily the middle of the tape, but is where the South tick is
            double tapeDeflection = 216d * widthScaling;

            Components.Add(new GaugeImage("{C-130J}/Gauges/Compass/Herc-Mag-Compass-Base.xaml", new Rect(0d, 0d, size.Width, size.Height)));

            _headingCalibration = new CalibrationPointCollectionDouble(0d, -1 * tapeDeflection, 360d, tapeDeflection);
            _ball = new GaugeNeedle("{F-15E}/Gauges/Common/Compass_Tape.xaml", center, new Size(tapeWidth, tapeHeight), new Point(tapeWidthCenter, tapeHeight / 2d));
            _ball.Clip = new EllipseGeometry(center, size.Width / 2d, size.Height / 2d);
            Components.Add(_ball);

            Components.Add(new GaugeImage("{F-15E}/Gauges/Common/vertical_marker.xaml", new Rect(center.X, size.Height * 0.2d, 1d * widthScaling, size.Height * 0.6d)));

            _heading = new HeliosValue(this, new BindingValue(0d), $"{_name}", "Magnetic Compass Heading", "Current compass heading of the aircraft in degrees.", "(0 to 360)", BindingValueUnits.Degrees);
            _heading.Execute += new HeliosActionHandler(Heading_Execute);
            Actions.Add(_heading);

            _rollCalibration = new CalibrationPointCollectionDouble(-180d, -20d, 180d, 20d);  // these values are degrees

            _roll = new HeliosValue(this, new BindingValue(0d), $"{_name}", "Magnetic Compass Roll", "Current roll of the compass rose in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Roll_Execute);
            Actions.Add(_roll);

            _pitchCalibration = new CalibrationPointCollectionDouble(-90d, tapeHeight * 0.67d, 90d, -1 * tapeHeight * 0.67d);

            _pitch = new HeliosValue(this, new BindingValue(0d), $"{_name}", "Magnetic Compass Pitch", "Current pitch of the compass rose in degrees.", "(-90 to +90)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            Components.Add(new GaugeImage("{C-130J}/Gauges/Compass/Herc-Mag-Compass-Faceplate.xaml", new Rect(0d, 0d, size.Width, size.Height)));
            CreateInputBindings();
        }
        void CreateInputBindings()
        {

            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { "Instruments.Magnetic Compass Heading.changed", $"{_name}.set.Magnetic Compass Heading" },
            };

            foreach (string t in bindings.Keys)
            {
                AddDefaultInputBinding(
                    childName: "",
                    interfaceTriggerName: t,
                    deviceActionName: bindings[t]
                    );
            }
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
