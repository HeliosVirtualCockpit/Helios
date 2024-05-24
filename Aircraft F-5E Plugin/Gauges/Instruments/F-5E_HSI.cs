//  Copyright 2014 Craig Courtney
//  Copyright 2024 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.HSI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F5E.Instruments.HSI", "HSI", "F-5E Tiger II", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class HSIGauge : BaseGauge
    {
        private readonly HeliosValue _outerDial;
        private readonly HeliosValue _innerDial;
        private readonly HeliosValue _offFlag;
        private readonly HeliosValue _dfFlag;
        private readonly HeliosValue _toFrom;
        private readonly HeliosValue _headingBug;
        private readonly HeliosValue _bearingBug;
        private readonly HeliosValue _courseDeviation;
        private readonly HeliosValue _courseOffFlag;
        private readonly HeliosValue _rangeOffFlag;
        private readonly HeliosValue _courseValue;
        private readonly HeliosValue _rangeValue;

        private readonly GaugeNeedle _offFlagNeedle;
        private readonly GaugeNeedle _dfFlagNeedle;
        private readonly GaugeNeedle _toFromNeedle;
        private readonly GaugeNeedle _innerDialNeedle;
        private readonly GaugeNeedle _outerDialNeedle;
        private readonly GaugeNeedle _headingBugNeedle;
        private readonly GaugeNeedle _bearingBugNeedle;
        private readonly GaugeNeedle _courseDeviationNeedle;
        private readonly GaugeImage  _courseOffFlagImage;
        private readonly GaugeImage  _rangeOffFlagImage;

        private readonly GaugeDrumCounter _rangeDrum;
        private readonly GaugeDrumCounter _courseDrum;
        private readonly GaugeDrumCounter _courseDrum1;


        private readonly CalibrationPointCollectionDouble _degreesCalibration;

        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public HSIGauge() : this("HSI",new Size(418d,420.5d),"Instruments") { }
        public HSIGauge(string name, Size size, string device)
            : base(name, size)
        {
            Point center = new Point(209.000d, 237.250d);

            _degreesCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1d, 360d) {
            };
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_HSI_Background.xaml", new Rect(22d, 35d, 396d, 42d)));

            _rangeDrum = new GaugeDrumCounter($"{_gaugeImagePath}F-5E_HSI_Drum_Tape_Units.xaml", new Point(28d, 40d), "##%", new Size(24.508d, 453.405d / 12d), new Size(88d / 3d * 0.9d, 35d * 0.9d));
            _rangeDrum.Value = 0d;
            _rangeDrum.Clip = new RectangleGeometry(new Rect(28d, 35d, 88d, 41d));
            Components.Add(_rangeDrum);

            _courseDrum1 = new GaugeDrumCounter($"{_gaugeImagePath}F-5E_HSI_Drum_Tape_Hundreds_and_Tens.xaml", new Point(308d, 41d), "#0", new Size(53.955d, 37.88696389d), new Size(88d / 3d * 2d, 35d * 0.9d), 36d);
            _courseDrum1.Value = 0d;
            _courseDrum1.Clip = new RectangleGeometry(new Rect(306d, 35d, 88d / 3d * 2d, 41d));
            Components.Add(_courseDrum1);

            _courseDrum = new GaugeDrumCounter($"{_gaugeImagePath}F-5E_HSI_Drum_Tape_Units.xaml", new Point(306d + (88d / 3d * 2d), 41d), "%", new Size(24.508d, 453.405d / 12d), new Size(88d * 0.9d / 3d, 35d * 0.9d));
            _courseDrum.Value = 0d;
            _courseDrum.Clip = new RectangleGeometry(new Rect(306d, 35d, 86d, 41d));
            Components.Add(_courseDrum);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_HSI_BackPlate.xaml", new Rect(0d, 0d, 418d, 420.5d)));

            _outerDialNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Outer_Dial.xaml", center, new Size(300d, 300d), new Point(150d, 150d));
            Components.Add(_outerDialNeedle);

            _toFromNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_To_From_Marker.xaml", center, new Size(22.759d, 44.000d), new Point(center.X - 232.7931d, center.Y - 215.25d));
            Components.Add(_toFromNeedle);

            _dfFlagNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_DF_Flag.xaml", center, new Size(38.6132d, 42d), new Point(center.X - 145.8966d, center.Y - 162.4914d));
            Components.Add(_dfFlagNeedle);

            _innerDialNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Inner_Dial.xaml", center, new Size(229.655d, 269.304d), new Point(114.828d, 138.169d));
            Components.Add(_innerDialNeedle);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_HSI_Aircraft_Symbol.xaml", new Rect(center.X - 49.780d, center.Y - 17.695, 99.560d, 50.940d)));

            _courseDeviationNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Course_Deviation_Line.xaml", center, new Size(6.457d, 149.216d), new Point(6.457d / 2d, 149.216d / 2d));
            Components.Add(_courseDeviationNeedle);

            _bearingBugNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Bearing_Indicator.xaml", center, new Size(17.658d, 327.751d), new Point(8.829d, 159.744d));
            Components.Add(_bearingBugNeedle);

            _headingBugNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Heading_Bug.xaml", center, new Size(24.043d, 149.820d), new Point(24.043d / 2d, 149.820d));
            Components.Add(_headingBugNeedle);

            _offFlagNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Off_Flag.xaml", new Point(363.352d + 9.7542d, 119.25d + 178.766d), new Size(50.5665d, 188.119d), new Point(9.7542d, 178.766d), 0d);
            Components.Add(_offFlagNeedle);

            _rangeOffFlagImage = new GaugeImage($"{_gaugeImagePath}F-5E_HSI_Drum_Off_Flag.xaml", new Rect(25d, 43d, 84d, 26d));
            _rangeOffFlagImage.IsHidden = true;
            Components.Add(_rangeOffFlagImage);

            _courseOffFlagImage = new GaugeImage($"{_gaugeImagePath}F-5E_HSI_Drum_Off_Flag.xaml", new Rect(309d, 43d, 84d, 26d));
            _courseOffFlagImage.IsHidden = true;
            Components.Add(_courseOffFlagImage);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_HSI_Flag_Cover.xaml", new Rect(374d, 120d, 41d, 100d)));

            _outerDial = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Compass Ring", "Number representing the rotation of the compass card.", "(0 to 1)", BindingValueUnits.Numeric);
            _outerDial.Execute += new HeliosActionHandler(OuterDial_Execute);
            Actions.Add(_outerDial);

            _innerDial = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Course Arrow", "Number representing the rotation of the course ring.", "(0 to 1)", BindingValueUnits.Numeric);
            _innerDial.Execute += new HeliosActionHandler(InnerDial_Execute);
            Actions.Add(_innerDial);

            _dfFlag = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Deviation DF Flag", "Number representing the position of the flag", "(0 to 1)", BindingValueUnits.Numeric);
            _dfFlag.Execute += new HeliosActionHandler(DFFlag_Execute);
            Actions.Add(_dfFlag);

            _toFrom = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI To/From Flag", "Number representing position of the flag.", "(-1 to +1)", BindingValueUnits.Numeric);
            _toFrom.Execute += new HeliosActionHandler(ToFrom_Execute);
            Actions.Add(_toFrom);

            _headingBug = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Heading Bug", "Number representing the angle.", "(0 to 1)", BindingValueUnits.Numeric);
            _headingBug.Execute += new HeliosActionHandler(HeadingBug_Execute);
            Actions.Add(_headingBug);

            _bearingBug = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Bearing Bug", "Number representing the angle.", "(0 to 1)", BindingValueUnits.Numeric);
            _bearingBug.Execute += new HeliosActionHandler(BearingBug_Execute);
            Actions.Add(_bearingBug);

            _courseDeviation = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Course Deviation Line", "Number representing the offset.", "(-1 to +1)", BindingValueUnits.Numeric);
            _courseDeviation.Execute += new HeliosActionHandler(CourseDeviation_Execute);
            Actions.Add(_courseDeviation);

            _offFlag = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "HSI Off Flag", "Number representing position of the flag.", "(0 to 1)", BindingValueUnits.Numeric);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _rangeValue = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "HSI Range to Beacon Drum", "Miles to beacon / waypoint.", "(0 to 999)", BindingValueUnits.NauticalMiles);
            _rangeValue.Execute += new HeliosActionHandler(RangeValue_Execute);
            Actions.Add(_rangeValue);

            _rangeOffFlag = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "HSI Range to Beacon Off Flag", "Boolean representing position of the flag.", "True / False", BindingValueUnits.Boolean);
            _rangeOffFlag.Execute += new HeliosActionHandler(RangeOffFlag_Execute);
            Actions.Add(_rangeOffFlag);

            _courseValue = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "HSI Course Drum", "Course setting in degrees.", "(0 to 360)", BindingValueUnits.Degrees);
            _courseValue.Execute += new HeliosActionHandler(CourseValue_Execute);
            Actions.Add(_courseValue);

            _courseOffFlag = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "HSI Course Off Flag", "Boolean representing position of the flag.", "True / False", BindingValueUnits.Boolean);
            _courseOffFlag.Execute += new HeliosActionHandler(CourseOffFlag_Execute);
            Actions.Add(_courseOffFlag);

        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = e.Value.DoubleValue * -30d;
        }

        void DFFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _dfFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _dfFlagNeedle.VerticalOffset = (1 - e.Value.DoubleValue) * 42d;
        }

        void ToFrom_Execute(object action, HeliosActionEventArgs e)
        {
            _toFrom.SetValue(e.Value, e.BypassCascadingTriggers);
            _toFromNeedle.VerticalOffset = (e.Value.DoubleValue * 11d);
        }

        void HeadingBug_Execute(object action, HeliosActionEventArgs e)
        {
            _headingBug.SetValue(e.Value, e.BypassCascadingTriggers);
            _headingBugNeedle.Rotation = _degreesCalibration.Interpolate(e.Value.DoubleValue);
        }

        void BearingBug_Execute(object action, HeliosActionEventArgs e)
        {
            _bearingBug.SetValue(e.Value, e.BypassCascadingTriggers);
            _bearingBugNeedle.Rotation = _degreesCalibration.Interpolate(e.Value.DoubleValue);
        }

        void CourseDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _courseDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _courseDeviationNeedle.HorizontalOffset = e.Value.DoubleValue * 85d;
        }

        void OuterDial_Execute(object action, HeliosActionEventArgs e)
        {
            _outerDial.SetValue(e.Value, e.BypassCascadingTriggers);
            _outerDialNeedle.Rotation = _degreesCalibration.Interpolate(e.Value.DoubleValue);
        }

        void InnerDial_Execute(object action, HeliosActionEventArgs e)
        {
            _innerDial.SetValue(e.Value, e.BypassCascadingTriggers);
            _innerDialNeedle.Rotation = _toFromNeedle.Rotation = _courseDeviationNeedle.Rotation = _dfFlagNeedle.Rotation = _degreesCalibration.Interpolate(e.Value.DoubleValue);
        }

        void RangeValue_Execute(object action, HeliosActionEventArgs e)
        {
            _rangeValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _rangeDrum.Value = e.Value.DoubleValue;
        }

        void RangeOffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _rangeOffFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _rangeOffFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void CourseValue_Execute(object action, HeliosActionEventArgs e)
        {
            _courseValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _courseDrum.Value = e.Value.DoubleValue % 10d;
            _courseDrum1.Value = e.Value.DoubleValue;
        }

        void CourseOffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _courseOffFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _courseOffFlagImage.IsHidden = !e.Value.BoolValue;
        }
    }
}
