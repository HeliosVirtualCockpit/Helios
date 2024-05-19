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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.Clock
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [HeliosControl("Helios.F5E.Instruments.Clock", "Clock", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class Clock : BaseGauge
    {
        private readonly HeliosValue _clockHours;
        private readonly HeliosValue _clockMinutes; 
        private readonly HeliosValue _clockStopwatchSeconds;
        private readonly HeliosValue _clockStopwatchMinutes;
        private readonly GaugeNeedle _needleHours;
        private readonly GaugeNeedle _needleMinutes;
        private readonly GaugeNeedle _needleStopwatchSeconds;
        private readonly GaugeNeedle _needleStopwatchMinutes;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly CalibrationPointCollectionDouble _calibrationPoints2;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public Clock(): base("Clock", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Clock_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));

            _needleStopwatchMinutes = new GaugeNeedle($"{_gaugeImagePath}F-5E_Clock_Needle_4.xaml", new Point(this.Height / 2, this.Width / 2), new Size(22d, 120d), new Point(11d, 109d), 0d);
            Components.Add(_needleStopwatchMinutes);

            _needleHours = new GaugeNeedle($"{_gaugeImagePath}F-5E_Clock_Needle_1.xaml", new Point(this.Height / 2, this.Width / 2), new Size(22d, 93d), new Point(11d, 82d), 0d);
            Components.Add(_needleHours);
            
            _needleMinutes = new GaugeNeedle($"{_gaugeImagePath}F-5E_Clock_Needle_2.xaml", new Point(this.Height / 2, this.Width / 2), new Size(22d, 121d), new Point(11d , 110d), 0d);
            Components.Add(_needleMinutes);

            _needleStopwatchSeconds = new GaugeNeedle($"{_gaugeImagePath}F-5E_Clock_Needle_3.xaml", new Point(this.Height / 2, this.Width / 2), new Size(22d, 189d), new Point(11d, 120d), 0d);
            Components.Add(_needleStopwatchSeconds);

            _clockHours = new HeliosValue(this, new BindingValue(0d), "", "Clock Hours", "Numeric", "(0 to 12)", BindingValueUnits.Numeric);
            _clockHours.Execute += new HeliosActionHandler(Clock_Execute);
            Actions.Add(_clockHours);

            _clockMinutes = new HeliosValue(this, new BindingValue(0d), "", "Clock Minutes", "Numeric", "(0 to 60)", BindingValueUnits.Numeric);
            _clockMinutes.Execute += new HeliosActionHandler(Clock_Execute);
            Actions.Add(_clockMinutes);

            _clockStopwatchMinutes = new HeliosValue(this, new BindingValue(0d), "", "Clock Stopwatch Minutes", "Numeric", "(0 to 60)", BindingValueUnits.Numeric);
            _clockStopwatchMinutes.Execute += new HeliosActionHandler(Clock_Execute);
            Actions.Add(_clockStopwatchMinutes);

            _clockStopwatchSeconds = new HeliosValue(this, new BindingValue(0d), "", "Clock Stopwatch Seconds", "Numeric", "(0 to 60)", BindingValueUnits.Numeric);
            _clockStopwatchSeconds.Execute += new HeliosActionHandler(Clock_Execute);
            Actions.Add(_clockStopwatchSeconds);



            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 12d, 360d);
            _calibrationPoints2 = new CalibrationPointCollectionDouble(0d, 0d, 60d, 360d);
        }
        void Clock_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosValue hv)
            {
                switch (hv.Name)
                {
                    case "Clock Hours":
                        _needleHours.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Clock Minutes":
                        _needleMinutes.Rotation = _calibrationPoints2.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Clock Stopwatch Seconds":
                        _needleStopwatchSeconds.Rotation = _calibrationPoints2.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Clock Stopwatch Minutes":
                        _needleStopwatchMinutes.Rotation = _calibrationPoints2.Interpolate(e.Value.DoubleValue);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
