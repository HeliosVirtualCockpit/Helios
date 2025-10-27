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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.HSI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.KA50.HSI", "HSI", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class HSI : BaseGauge
    {
        private GaugeNeedle _longDeviationNeedle;
        private GaugeNeedle _latDeviationNeedle;
        private GaugeNeedle _compassCard;
        private GaugeNeedle _bearingNeedle;
        private GaugeNeedle _dtaNeedle;
        private GaugeNeedle _headingBug;
        private GaugeDrumCounter _distanceDrum;
        private GaugeDrumCounter _dtaDrum;
        private GaugeImage _kFlagImage;
        private GaugeImage _lFlagImage;
        private GaugeImage _kcFlagImage;

        private HeliosValue _longDeviation;
        private HeliosValue _latDeviation;
        private HeliosValue _heading;
        private HeliosValue _bearing;
        private HeliosValue _course;
        private HeliosValue _commandedHeading;
        private HeliosValue _range;
        private HeliosValue _kFlag;
        private HeliosValue _lFlag;
        private HeliosValue _kcFlag;

        private CalibrationPointCollectionDouble _deviationScale = new CalibrationPointCollectionDouble(-1, -60, 1, 60);

        public HSI()
            : base("HSI", new Size(400, 400))
        {
            Point center = new Point(200, 200);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/HSI/deviation_card.xaml", new Rect(145, 145, 110, 110)));

            _longDeviationNeedle = new GaugeNeedle("{Ka-50}/Gauges/HSI/long_deviation_needle.xaml", center, new Size(120, 5), new Point(60, 2.5));
            Components.Add(_longDeviationNeedle);

            _longDeviation = new HeliosValue(this, BindingValue.Empty, "", "Longitudinal Deviaiton", "Offset of the longitundinal deviation needle from center.", "-1 to 1 where -1 is full left.", BindingValueUnits.Numeric);
            _longDeviation.Execute += new HeliosActionHandler(LongDeviation_Execute);
            Actions.Add(_longDeviation);

            _latDeviationNeedle = new GaugeNeedle("{Ka-50}/Gauges/HSI/lat_deviation_needle.xaml", center, new Size(5, 120), new Point(3, 60));
            Components.Add(_latDeviationNeedle);

            _latDeviation = new HeliosValue(this, BindingValue.Empty, "", "Lateral Deviaiton", "Offset of the Lateral deviation needle from center.", "-1 to 1 where -1 is full up.", BindingValueUnits.Numeric);
            _latDeviation.Execute += new HeliosActionHandler(LatDeviation_Execute);
            Actions.Add(_latDeviation);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/HSI/compass_card_bezel.xaml", new Rect(73, 73, 254, 254)));

            _compassCard = new GaugeNeedle("{Ka-50}/Gauges/HSI/compass_card.xaml", center, new Size(251, 251), new Point(125.5, 125.5));
            Components.Add(_compassCard);

            _heading = new HeliosValue(this, BindingValue.Empty, "", "Heading", "Current heading of the aircraft", "", BindingValueUnits.Degrees);
            _heading.Execute += Heading_Execute;
            Actions.Add(_heading);

            _bearingNeedle = new GaugeNeedle("{Ka-50}/Gauges/HSI/bearing_needle.xaml", center, new Size(15, 235), new Point(7.5, 120.5));
            Components.Add(_bearingNeedle);

            _bearing = new HeliosValue(this, BindingValue.Empty, "", "Bearing", "Current direction the bearing needle is pointing.", "", BindingValueUnits.Degrees);
            _bearing.Execute += Bearing_Execute;
            Actions.Add(_bearing);

            _dtaNeedle = new GaugeNeedle("{Ka-50}/Gauges/HSI/dta_needle.xaml", center, new Size(42, 238), new Point(21, 128));
            Components.Add(_dtaNeedle);

            _course = new HeliosValue(this, BindingValue.Empty, "", "Commanded Course", "Current commanded course.", "", BindingValueUnits.Degrees);
            _course.Execute += Course_Execute;
            Actions.Add(_course);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/HSI/inner_bezel.xaml", new Rect(0, 0, 400, 400)));

            _headingBug = new GaugeNeedle("{Ka-50}/Gauges/HSI/heading_bug.xaml", center, new Size(40, 15), new Point(20, 142));
            Components.Add(_headingBug);

            _commandedHeading = new HeliosValue(this, BindingValue.Empty, "", "Commanded Heading", "Current commanded course.", "", BindingValueUnits.Degrees);
            _commandedHeading.Execute += ComandedHeading_Execute;
            Actions.Add(_commandedHeading);

            _distanceDrum = new GaugeDrumCounter("{Ka-50}/Gauges/Common/drum_tape.xaml", new Point(40, 38), "##%", new Size(10, 15), new Size(18, 27));
            _distanceDrum.Clip = new RectangleGeometry(new Rect(40, 35, 59, 32));
            Components.Add(_distanceDrum);

            _range = new HeliosValue(this, BindingValue.Empty, "", "Range", "Distance to current beacon.", "", BindingValueUnits.Kilometers);
            _range.Execute += Range_Execute;
            Actions.Add(_range);

            _dtaDrum = new GaugeDrumCounter("{Ka-50}/Gauges/Common/drum_tape.xaml", new Point(309, 38), "##%", new Size(10, 15), new Size(18, 27));
            _dtaDrum.Clip = new RectangleGeometry(new Rect(309, 35, 59, 32));
            Components.Add(_dtaDrum);

            _kFlagImage = new GaugeImage("{Ka-50}/Gauges/HSI/k_flag.xaml", new Rect(6, 160, 28, 90));
            Components.Add(_kFlagImage);

            _kFlag = new HeliosValue(this, new BindingValue(false), "", "Glide (K) flag", "Indicates navigation computer failure.", "True if displayed.", BindingValueUnits.Boolean);
            _kFlag.Execute += KFlag_Execute;
            Actions.Add(_kFlag);

            _lFlagImage = new GaugeImage("{Ka-50}/Gauges/HSI/l_flag.xaml", new Rect(366, 160, 28, 90));
            Components.Add(_lFlagImage);

            _lFlag = new HeliosValue(this, new BindingValue(false), "", "Course (L) flag", "Indicates navigation computer failure.", "True if displayed.", BindingValueUnits.Boolean);
            _lFlag.Execute += LFlag_Execute;
            Actions.Add(_lFlag);

            _kcFlagImage = new GaugeImage("{Ka-50}/Gauges/HSI/kc_flag.xaml", new Rect(146, 24, 108, 26));
            Components.Add(_kcFlagImage);

            _kcFlag = new HeliosValue(this, new BindingValue(false), "", "Heading (KC) flag", "Indicates INU failure or lack of power.", "True if displayed.", BindingValueUnits.Boolean);
            _kcFlag.Execute += KCFlag_Execute;
            Actions.Add(_kcFlag);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/HSI/outer_bezel.xaml", new Rect(0, 0, 400, 400)));
        }

        private void LongDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _longDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _longDeviationNeedle.VerticalOffset = _deviationScale.Interpolate(e.Value.DoubleValue);
        }

        private void LatDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _latDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _latDeviationNeedle.HorizontalOffset = _deviationScale.Interpolate(e.Value.DoubleValue);
        }

        private void Heading_Execute(object action, HeliosActionEventArgs e)
        {
            _heading.SetValue(e.Value, e.BypassCascadingTriggers);
            _compassCard.Rotation = -e.Value.DoubleValue;
            _dtaNeedle.Rotation = _compassCard.Rotation + _course.Value.DoubleValue;
            _headingBug.Rotation = _compassCard.Rotation + _commandedHeading.Value.DoubleValue;
        }

        private void Bearing_Execute(object action, HeliosActionEventArgs e)
        {
            _bearing.SetValue(e.Value, e.BypassCascadingTriggers);
            _bearingNeedle.Rotation = e.Value.DoubleValue;
        }

        private void Course_Execute(object action, HeliosActionEventArgs e)
        {
            _course.SetValue(e.Value, e.BypassCascadingTriggers);
            _dtaDrum.Value = e.Value.DoubleValue;
            _dtaNeedle.Rotation = _compassCard.Rotation + e.Value.DoubleValue;
        }

        private void ComandedHeading_Execute(object action, HeliosActionEventArgs e)
        {
            _commandedHeading.SetValue(e.Value, e.BypassCascadingTriggers);
            _headingBug.Rotation = _compassCard.Rotation + e.Value.DoubleValue;
        }

        private void Range_Execute(object action, HeliosActionEventArgs e)
        {
            _range.SetValue(e.Value, e.BypassCascadingTriggers);
            _distanceDrum.Value = e.Value.DoubleValue;
        }

        private void KFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _kFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _kFlagImage.IsHidden = !e.Value.BoolValue;
        }

        private void LFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _lFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _lFlagImage.IsHidden = !e.Value.BoolValue;
        }

        private void KCFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _kcFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _kcFlagImage.IsHidden = !e.Value.BoolValue;
        }

    }
}
