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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.NPP
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.MiG-21.NPP", "NPP", "MiG-21", typeof(GaugeRenderer))]
    public class NPP : BaseGauge
    {
        private GaugeNeedle _glideslopeDeviationNeedle;
        private GaugeNeedle _courseDeviationNeedle;
        private GaugeNeedle _compassCard;
        private GaugeNeedle _bearingNeedle;
        private GaugeNeedle _innerBezelMarkers;
        private GaugeNeedle _courseNeedle;
        private GaugeImage _kFlagImage;
        private GaugeImage _gFlagImage;

        private HeliosValue _glideslopeDeviation;
        private HeliosValue _courseDeviation;
        private HeliosValue _heading;
        private HeliosValue _bearing;
        private HeliosValue _course;
        private HeliosValue _kFlag;
        private HeliosValue _gFlag;

        private CalibrationPointCollectionDouble _deviationScale = new CalibrationPointCollectionDouble(-1, -50, 1, 50);

        public NPP()
            : base("NPP", new Size(400, 400))
        {
            Point center = new Point(200, 200);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/NPP/deviation_card.xaml", new Rect(145, 145, 110, 110)));

            Components.Add(new GaugeImage("{MiG-21}/Gauges/NPP/black_blinker.xaml", new Rect(170, 170, 10, 20)));
            Components.Add(new GaugeImage("{MiG-21}/Gauges/NPP/black_blinker.xaml", new Rect(220, 210, 10, 20)));

            //course (3, K -“kurs”) and glide slope (8, G - “glisada”) 
            _kFlagImage = new GaugeImage("{MiG-21}/Gauges/NPP/k_flag.xaml", new Rect(170, 170, 10, 20));
            Components.Add(_kFlagImage);

            _kFlag = new HeliosValue(this, new BindingValue(false), "", "Course (K) flag", "White when lit", "True if displayed.", BindingValueUnits.Boolean);
            _kFlag.Execute += KFlag_Execute;
            Actions.Add(_kFlag);

            _gFlagImage = new GaugeImage("{MiG-21}/Gauges/NPP/g_flag.xaml", new Rect(220, 210, 10, 20));
            Components.Add(_gFlagImage);

            _gFlag = new HeliosValue(this, new BindingValue(false), "", "Glide (G) flag", "White when lit", "True if displayed.", BindingValueUnits.Boolean);
            _gFlag.Execute += GFlag_Execute;
            Actions.Add(_gFlag);

            _glideslopeDeviationNeedle = new GaugeNeedle("{MiG-21}/Gauges/NPP/glideslope_deviation_needle.xaml", center, new Size(120, 5), new Point(60, 2.5));
            Components.Add(_glideslopeDeviationNeedle);

            _glideslopeDeviation = new HeliosValue(this, new BindingValue(1d), "", "Glideslope Deviaiton", "Offset of the glideslope deviation needle from center.", "-1 to 1 where -1 is full down.", BindingValueUnits.Numeric);
            _glideslopeDeviation.Execute += new HeliosActionHandler(GlideslopeDeviation_Execute);
            Actions.Add(_glideslopeDeviation);

            _courseDeviationNeedle = new GaugeNeedle("{MiG-21}/Gauges/NPP/course_deviation_needle.xaml", center, new Size(5, 120), new Point(3, 60));
            Components.Add(_courseDeviationNeedle);

            _courseDeviation = new HeliosValue(this, BindingValue.Empty, "", "Course Deviaiton Needle", "Offset of the course deviation needle from center.", "-1 to 1 where -1 is full right.", BindingValueUnits.Numeric);
            _courseDeviation.Execute += new HeliosActionHandler(CourseDeviation_Execute);
            Actions.Add(_courseDeviation);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/NPP/compass_card_bezel.xaml", new Rect(73, 73, 254, 254)));

            _compassCard = new GaugeNeedle("{MiG-21}/Gauges/NPP/compass_card.xaml", center, new Size(251, 251), new Point(125.5, 125.5));
            Components.Add(_compassCard);

            _heading = new HeliosValue(this, BindingValue.Empty, "", "Heading", "Current heading of the aircraft", "", BindingValueUnits.Degrees);
            _heading.Execute += Heading_Execute;
            Actions.Add(_heading);

            _bearingNeedle = new GaugeNeedle("{MiG-21}/Gauges/NPP/bearing_needle.xaml", center, new Size(42, 238), new Point(21, 128));
            Components.Add(_bearingNeedle);

            _bearing = new HeliosValue(this, BindingValue.Empty, "", "Bearing", "Current direction the bearing needle is pointing.", "", BindingValueUnits.Degrees);
            _bearing.Execute += Bearing_Execute;
            Actions.Add(_bearing);

            _courseNeedle = new GaugeNeedle("{MiG-21}/Gauges/NPP/course_needle.xaml", center, new Size(42, 238), new Point(21, 128));
            Components.Add(_courseNeedle);

            _course = new HeliosValue(this, BindingValue.Empty, "", "Commanded Course", "Current commanded course.", "", BindingValueUnits.Degrees);
            _course.Execute += Course_Execute;
            Actions.Add(_course);

            _innerBezelMarkers = new GaugeNeedle("{MiG-21}/Gauges/NPP/inner_bezel_markers.xaml", center, new Size(166, 166), new Point(83, 83));
            Components.Add(_innerBezelMarkers);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/NPP/outer_bezel.xaml", new Rect(0, 0, 400, 400)));
        }

        private void GlideslopeDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _glideslopeDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _glideslopeDeviationNeedle.VerticalOffset = _deviationScale.Interpolate(e.Value.DoubleValue) * -1;
        }

        private void CourseDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _courseDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _courseDeviationNeedle.HorizontalOffset = _deviationScale.Interpolate(e.Value.DoubleValue);
        }

        private void Heading_Execute(object action, HeliosActionEventArgs e)
        {
            _heading.SetValue(e.Value, e.BypassCascadingTriggers);
            _compassCard.Rotation = e.Value.DoubleValue;
        }

        private void Bearing_Execute(object action, HeliosActionEventArgs e)
        {
            _bearing.SetValue(e.Value, e.BypassCascadingTriggers);
            _bearingNeedle.Rotation = e.Value.DoubleValue;
        }

        private void Course_Execute(object action, HeliosActionEventArgs e)
        {
            _course.SetValue(e.Value, e.BypassCascadingTriggers);
            _courseNeedle.Rotation = e.Value.DoubleValue - 180;
            _innerBezelMarkers.Rotation = e.Value.DoubleValue - 180;
        }

        private void KFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _kFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _kFlagImage.IsHidden = !e.Value.BoolValue;
        }

        private void GFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _gFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _gFlagImage.IsHidden = !e.Value.BoolValue;
        }


    }
}
