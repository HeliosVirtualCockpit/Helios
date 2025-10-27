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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.Clock
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.KA50.Clock", "Clock", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class Clock : BaseGauge
    {
        private GaugeImage _flightTimeIndicatorLight;

        private GaugeNeedle _stopWatchMinutesNeedle;
        private GaugeNeedle _stopWatchSecondsNeedle;
        private GaugeNeedle _flightTimeHourNeedle;
        private GaugeNeedle _flightTimeMinutesNeedle;
        private GaugeNeedle _hourNeedle;
        private GaugeNeedle _minuteNeedle;
        private GaugeNeedle _secondNeedle;

        private HeliosValue _currentTimeHours;
        private HeliosValue _currentTimeMinutes;
        private HeliosValue _currentTimeSeconds;

        private HeliosValue _flighTimeHours;
        private HeliosValue _flightTimeMinutes;
        private HeliosValue _flightTimeIndicator;

        private HeliosValue _stopWatchMinutes;
        private HeliosValue _stopWatchSeconds;

        private CalibrationPointCollectionDouble _hourCalibration = new CalibrationPointCollectionDouble(0d, 0d, 12d, 360d);
        private CalibrationPointCollectionDouble _minSecCalibration = new CalibrationPointCollectionDouble(0d, 0d, 60d, 360d);
        private CalibrationPointCollectionDouble _stopWatchCalibartion = new CalibrationPointCollectionDouble(0d, 0d, 30d, 360d);

        public Clock()
            : base("Clock", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/Clock/clock_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _flightTimeIndicatorLight = new GaugeImage("{Ka-50}/Gauges/Clock/clock_flight_time_indicator.xaml", new Rect(162, 113, 16, 16));
            Components.Add(_flightTimeIndicatorLight);

            _stopWatchMinutesNeedle = new GaugeNeedle("{Ka-50}/Gauges/Clock/clock_small_hour_needle.xaml", new Point(170, 245), new Size(8, 56), new Point(4, 51));
            Components.Add(_stopWatchMinutesNeedle);

            _stopWatchSecondsNeedle = new GaugeNeedle("{Ka-50}/Gauges/Clock/clock_small_min_needle.xaml", new Point(170, 245), new Size(8, 56), new Point(4, 46));
            Components.Add(_stopWatchSecondsNeedle);

            _flightTimeHourNeedle = new GaugeNeedle("{Ka-50}/Gauges/Clock/clock_small_hour_needle.xaml", new Point(170, 100), new Size(8, 56), new Point(4, 51));
            Components.Add(_flightTimeHourNeedle);

            _flightTimeMinutesNeedle = new GaugeNeedle("{Ka-50}/Gauges/Clock/clock_small_min_needle.xaml", new Point(170, 100), new Size(8, 56), new Point(4, 46));
            Components.Add(_flightTimeMinutesNeedle);

            _hourNeedle = new GaugeNeedle("{Ka-50}/Gauges/Clock/clock_hour_needle.xaml", center, new Size(20, 135), new Point(10, 125));
            Components.Add(_hourNeedle);

            _minuteNeedle = new GaugeNeedle("{Ka-50}/Gauges/Clock/clock_min_needle.xaml", center, new Size(20, 135), new Point(10, 125));
            Components.Add(_minuteNeedle);

            _secondNeedle = new GaugeNeedle("{Ka-50}/Gauges/Clock/clock_seconds_needle.xaml", center, new Size(13, 166), new Point(6.5, 141));
            Components.Add(_secondNeedle);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/Clock/clock_bezel.xaml", new Rect(0, 0, 340, 340)));

            _currentTimeHours = new HeliosValue(this, BindingValue.Empty, "", "Current Time Hours", "Hour hands current time.", "", BindingValueUnits.Numeric);
            _currentTimeHours.Execute += CurrentTimeHours_Execute;
            Actions.Add(_currentTimeHours);

            _currentTimeMinutes = new HeliosValue(this, BindingValue.Empty, "", "Current Time Minutes", "Minutes hands current time.", "", BindingValueUnits.Numeric);
            _currentTimeMinutes.Execute += CurrentTimeMinutes_Execute;
            Actions.Add(_currentTimeMinutes);

            _currentTimeSeconds = new HeliosValue(this, BindingValue.Empty, "", "Current Time Seconds", "Seconds hands current time.", "", BindingValueUnits.Numeric);
            _currentTimeSeconds.Execute += CurrentTimeSeconds_Execute;
            Actions.Add(_currentTimeSeconds);

            _flighTimeHours = new HeliosValue(this, BindingValue.Empty, "", "Flight Time Hours", "Hour hands flight time.", "", BindingValueUnits.Numeric);
            _flighTimeHours.Execute += FlightTimeHours_Execute;
            Actions.Add(_flighTimeHours);

            _flightTimeMinutes = new HeliosValue(this, BindingValue.Empty, "", "Flight Time Minutes", "Minutes hands flight time.", "", BindingValueUnits.Numeric);
            _flightTimeMinutes.Execute += FlightTimeMinutes_Execute;
            Actions.Add(_flightTimeMinutes);

            _stopWatchMinutes = new HeliosValue(this, BindingValue.Empty, "", "Stop Watch Minutes", "Minutes hands stop watch.", "", BindingValueUnits.Numeric);
            _stopWatchMinutes.Execute += StopWatchMinutes_Execute;
            Actions.Add(_stopWatchMinutes);

            _stopWatchSeconds = new HeliosValue(this, BindingValue.Empty, "", "Stop Watch Seconds", "Seconds hands stop watch.", "", BindingValueUnits.Numeric);
            _stopWatchSeconds.Execute += StopWatchSeconds_Execute;
            Actions.Add(_stopWatchSeconds);

            _flightTimeIndicator = new HeliosValue(this, BindingValue.Empty, "", "Flight Time Indicator", "Lit when flight time counter is engaged", "", BindingValueUnits.Boolean);
            _flightTimeIndicator.Execute += FlightTimeIndicator_Execute;
            Actions.Add(_flightTimeIndicator);
        }

        void CurrentTimeHours_Execute(object action, HeliosActionEventArgs e)
        {
            _currentTimeHours.SetValue(e.Value, e.BypassCascadingTriggers);
            _hourNeedle.Rotation = _hourCalibration.Interpolate(e.Value.DoubleValue);
        }

        void CurrentTimeMinutes_Execute(object action, HeliosActionEventArgs e)
        {
            _currentTimeMinutes.SetValue(e.Value, e.BypassCascadingTriggers);
            _minuteNeedle.Rotation = _minSecCalibration.Interpolate(e.Value.DoubleValue);
        }

        void CurrentTimeSeconds_Execute(object action, HeliosActionEventArgs e)
        {
            _currentTimeSeconds.SetValue(e.Value, e.BypassCascadingTriggers);
            _secondNeedle.Rotation = _minSecCalibration.Interpolate(e.Value.DoubleValue);
        }

        void FlightTimeHours_Execute(object action, HeliosActionEventArgs e)
        {
            _flighTimeHours.SetValue(e.Value, e.BypassCascadingTriggers);
            _flightTimeHourNeedle.Rotation = _hourCalibration.Interpolate(e.Value.DoubleValue);
        }

        void FlightTimeMinutes_Execute(object action, HeliosActionEventArgs e)
        {
            _flightTimeMinutes.SetValue(e.Value, e.BypassCascadingTriggers);
            _flightTimeMinutesNeedle.Rotation = _minSecCalibration.Interpolate(e.Value.DoubleValue);
        }

        void StopWatchMinutes_Execute(object action, HeliosActionEventArgs e)
        {
            _stopWatchMinutes.SetValue(e.Value, e.BypassCascadingTriggers);
            _stopWatchMinutesNeedle.Rotation = _stopWatchCalibartion.Interpolate(e.Value.DoubleValue);
        }

        void StopWatchSeconds_Execute(object action, HeliosActionEventArgs e)
        {
            _stopWatchSeconds.SetValue(e.Value, e.BypassCascadingTriggers);
            _stopWatchSecondsNeedle.Rotation = _stopWatchCalibartion.Interpolate(e.Value.DoubleValue);
        }

        private void FlightTimeIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _flightTimeIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
            _flightTimeIndicatorLight.IsHidden = !e.Value.BoolValue;
        }
    }
}
