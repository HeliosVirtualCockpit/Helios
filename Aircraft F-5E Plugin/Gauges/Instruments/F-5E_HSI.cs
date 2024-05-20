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

    [HeliosControl("Helios.F5E.Instruments.HSI", "HSI", "F-5E Tiger II", typeof(GaugeRenderer),HeliosControlFlags.None)]
    public class HSIGauge : BaseGauge
    {
        private readonly HeliosValue _outerDial;
        private readonly HeliosValue _innerDial;
        private readonly HeliosValue _offFlag;
        private readonly HeliosValue _dfFlag;
        private readonly HeliosValue _toFrom;
        private readonly HeliosValue _headingBug;
        private readonly HeliosValue _tacanBug;
        private readonly HeliosValue _courseDeviation;

        private readonly GaugeNeedle _offFlagNeedle;
        private readonly GaugeNeedle _dfFlagNeedle;
        private readonly GaugeNeedle _toFromNeedle;
        private readonly GaugeNeedle _innerDialNeedle;
        private readonly GaugeNeedle _outerDialNeedle;
        private readonly GaugeNeedle _headingBugNeedle;
        private readonly GaugeNeedle _tacanBugNeedle;
        private readonly GaugeNeedle _courseDeviationNeedle;


        private readonly CalibrationPointCollectionDouble _outerDialCalibration;

        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public HSIGauge() : this("HSI",new Size(418d,420.5d),"Instruments") { }
        public HSIGauge(string name, Size size, string device)
            : base(name, size)
        {
            Point center = new Point(209.000d, 237.250d);

            _outerDialCalibration = new CalibrationPointCollectionDouble(-90d, -1, 90d,1) {
            };
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_HSI_BackPlate.xaml", new Rect(0d, 0d, 418d, 420.5d)));

            _outerDialNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Outer_Dial.xaml", center, new Size(300d, 300d), new Point(150d, 150d));
            Components.Add(_outerDialNeedle);

            _toFromNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_To_From_Marker.xaml", center, new Size(22.759d, 44.000d), new Point(center.X - 232.7931d, center.Y - 215.25d));
            Components.Add(_toFromNeedle);

            _dfFlagNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_DF_Flag.xaml", center, new Size(38.6132d, 21.9265d), new Point(center.X - 145.8966d, center.Y - 184.4914d));
            Components.Add(_dfFlagNeedle);

            _innerDialNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Inner_Dial.xaml", center, new Size(229.655d, 269.304d), new Point(114.828d, 138.169d));
            Components.Add(_innerDialNeedle);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_HSI_Aircraft_Symbol.xaml", new Rect(center.X - 49.780d, center.Y - 17.695, 99.560d, 50.940d)));

            _courseDeviationNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Course_Deviation_Line.xaml", center, new Size(6.457d, 149.216d), new Point(6.457d / 2d, 149.216d / 2d));
            Components.Add(_courseDeviationNeedle);

            _tacanBugNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_TACAN_Indicator.xaml", center, new Size(17.658d, 327.751d), new Point(8.829d, 159.744d));
            Components.Add(_tacanBugNeedle);

            _headingBugNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Heading_Bug.xaml", center, new Size(24.043d, 9.234d), new Point(11.8966, 149.3197d));
            Components.Add(_headingBugNeedle);

            _offFlagNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_HSI_Off_Flag.xaml", new Point(363.352d + 9.7542d, 119.25d + 178.766d), new Size(50.5665d, 188.119d), new Point(9.7542d, 178.766d), 0d);
            Components.Add(_offFlagNeedle);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_HSI_Flag_Cover.xaml", new Rect(374d, 120d, 41d, 100d)));

            _outerDial = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Aircraft Pitch Angle", "Current pitch of the aircraft in degrees.", "(-90 to +90)", BindingValueUnits.Degrees);
            _outerDial.Execute += new HeliosActionHandler(OuterDial_Execute);
            Actions.Add(_outerDial);

            _innerDial = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Aircraft Bank Angle", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _innerDial.Execute += new HeliosActionHandler(InnerDial_Execute);
            Actions.Add(_innerDial);

            _dfFlag = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Aircraft Bank Angle", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _dfFlag.Execute += new HeliosActionHandler(DFFlag_Execute);
            Actions.Add(_dfFlag);

            _toFrom = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Aircraft Bank Angle", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _toFrom.Execute += new HeliosActionHandler(ToFrom_Execute);
            Actions.Add(_toFrom);

            _headingBug = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Aircraft Bank Angle", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _headingBug.Execute += new HeliosActionHandler(HeadingBug_Execute);
            Actions.Add(_headingBug);

            _tacanBug = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Aircraft Bank Angle", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _tacanBug.Execute += new HeliosActionHandler(TacanBug_Execute);
            Actions.Add(_tacanBug);

            _courseDeviation = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "HSI Aircraft Bank Angle", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _courseDeviation.Execute += new HeliosActionHandler(CourseDeviation_Execute);
            Actions.Add(_courseDeviation);

            _offFlag = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "HSI Off Flag", "Indicates the position of the off flag.", "1.0 if displayed.", BindingValueUnits.Numeric);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);
        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = (e.Value.DoubleValue) * -90d;
        }

        void DFFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = (e.Value.DoubleValue) * -90d;
        }

        void ToFrom_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = (e.Value.DoubleValue) * -90d;
        }

        void HeadingBug_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = (e.Value.DoubleValue) * -90d;
        }

        void TacanBug_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = (e.Value.DoubleValue) * -90d;
        }

        void CourseDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = (e.Value.DoubleValue) * -90d;
        }

        void OuterDial_Execute(object action, HeliosActionEventArgs e)
        {
            _outerDial.SetValue(e.Value, e.BypassCascadingTriggers);
            //_ball.VerticalOffset = _outerDialCalibration.Interpolate(e.Value.DoubleValue);
        }

        void InnerDial_Execute(object action, HeliosActionEventArgs e)
        {
            _innerDial.SetValue(e.Value, e.BypassCascadingTriggers);
            //_ball.Rotation = e.Value.DoubleValue;
            _innerDialNeedle.Rotation = e.Value.DoubleValue;
        }
    }
}
