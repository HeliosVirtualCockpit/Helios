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

namespace GadrocsWorkshop.Helios.Gauges.A10C
{ 
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.A10.DigitalClockGauge", "Digital Clock", "A-10C Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class DigitalClockGauge : BaseGauge
    {
        private HeliosValue _clockHandSeconds;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private double _glassReflectionOpacity;
        private GaugeImage _giScale;
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 1.0;
        private string _imageLocation = "{A-10C}/Images/A-10C/";


        public DigitalClockGauge()
            : base("DigitalClockGauge", new Size(414d, 405d))
        {
            Components.Add(new GaugeImage(_imageLocation + "A-10C_Digital_Clock_Dial.png", new Rect(0d, 0d, 414d, 405d)));
            _giScale = new GaugeImage(_imageLocation + "A-10C_Digital_Clock_Dial_Numbers.png", new Rect(0d, 0d, 414d, 405d));
            Components.Add(_giScale);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 60d, 360d) {
                new CalibrationPointDouble(100d, 34d)
            };
            _needle = new GaugeNeedle("{Helios}/Gauges/A-10/DigitalClock/ClockSecondHand.xaml", new Point(207d, 208d), new Size(22, 165), new Point(11, 130), 0d);
            Components.Add(_needle);
            _clockHandSeconds = new HeliosValue(this, new BindingValue(0d), "", "DigitalClock_Second Hand", "Text Value of Seconds", "(0 - 60)", BindingValueUnits.Text);
            _clockHandSeconds.Execute += new HeliosActionHandler(ClockHandSeconds_Execute);
            Actions.Add(_clockHandSeconds);



            // initialize opacity value and related visual
            GlassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;
        }
        #region Properties
        public double GlassReflectionOpacity
        {
            get
            {
                return _glassReflectionOpacity;
            }
            set
            {
                // clamp to max opacity
                double newValue = Math.Min(value, 1.0);

                double oldValue = _glassReflectionOpacity;
                if (newValue != oldValue)
                {
                    _glassReflectionOpacity = newValue;

                    // don't render at all if fully transparent
                    _giScale.IsHidden = (newValue == 0.0);

                    // render at this opacity, if applicable
                    _giScale.Opacity = newValue;

                    // notify change after change is made
                    OnPropertyChanged("GlassReflectionOpacity", oldValue, newValue, true);
                }
            }
        }
        #endregion

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
        }
        void ClockHandSeconds_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        public override bool HitTest(Point location)
        {
            //if (_scaledScreenRect.Contains(location))
            //{
            //    return false;
            //}

            //return true;
            return false;
        }

        public override void MouseDown(Point location)
        {
            // No-Op
        }

        public override void MouseDrag(Point location)
        {
            // No-Op
        }

        public override void MouseUp(Point location)
        {
            // No-Op
        }
    }
}
