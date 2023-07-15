//  Copyright 2018 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F15E
{
    using GadrocsWorkshop.Helios.Gauges;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;

    [HeliosControl("Helios.F15E.EngineMonitor", "Engine Monitor Needles & Flags", "F-15E Strike Eagle", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class EngineMonitorNozzleGauge : BaseGauge
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private double _glassReflectionOpacity;
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 1.0;

        private Color _textColor = Color.FromArgb(0xff,220, 220, 220);
        private string _imageLocation = "{Helios}/Gauges/F-15E/EngineMonitor/";
        private GaugeNeedle _gnLeftNoz;
        private HeliosValue _leftNozzle;
        private HeliosValue _leftNozzleNeedle;
        private CalibrationPointCollectionDouble _needleLeftCalibration;
        private GaugeNeedle _gnRightNoz;
        private HeliosValue _rightNozzle;
        private HeliosValue _rightNozzleNeedle;
        private CalibrationPointCollectionDouble _needleRightCalibration;
        private GaugeImage _gibackground;
        private GaugeImage _gireflection;
        private GaugeImage _giGaugeMarksL;
        private HeliosValue _indicatorMarksLeft;
        private GaugeImage _giGaugeMarksR;
        private HeliosValue _indicatorMarksRight;
        private GaugeImage _giGaugeLegends;
        private HeliosValue _indicatorGaugeLegends;

        public EngineMonitorNozzleGauge()
            : base("Engine Nozzle Gauge", new Size(470, 437))
        {
            // adding the control buttons


            // Add various image components to the gauge
            //_gibackground = new GaugeImage($"{_imageLocation}EngineMonitorBackground.png", new Rect(0d, 0d, 470d, 437d));
            //Components.Add(_gibackground);
            //_gibackground.IsHidden = true;  // This is to make sure that we do not mask anything while developing

            _giGaugeMarksL = new GaugeImage($"{_imageLocation}Nozzle Gauge Marks.xaml", new Rect(91d, 219d, 100d, 100d));
            Components.Add(_giGaugeMarksL);
            _giGaugeMarksR = new GaugeImage($"{_imageLocation}Nozzle Gauge Marks.xaml", new Rect(278d, 219d, 100d, 100d));
            Components.Add(_giGaugeMarksR);
            _giGaugeLegends = new GaugeImage($"{_imageLocation}Engine Panel Legends.xaml", new Rect(141d, 68d, 188d, 270d));
            Components.Add(_giGaugeLegends);
            _needleLeftCalibration = new CalibrationPointCollectionDouble(0d, 0d, 100d, 90d);
            _gnLeftNoz = new GaugeNeedle($"{_imageLocation}Needle.xaml", new Point(91d, 219d), new Size(70d, 4d), new Point(0d, 2d));
            Components.Add(_gnLeftNoz);
            _gnLeftNoz.IsHidden = true;
            _needleRightCalibration = new CalibrationPointCollectionDouble(0d, 0d, 100d, 90d);
            _gnRightNoz = new GaugeNeedle($"{_imageLocation }Needle.xaml", new Point(278d, 219d), new Size(70d, 4d), new Point(0d, 2d));
            Components.Add(_gnRightNoz);
            _gnRightNoz.IsHidden = true;
            _leftNozzle = new HeliosValue(this, BindingValue.Empty, "Engine Monitor Panel_Engine Nozzle Gauge", "Left Engine Nozzle Position", "Left Nozzle Position in %.", "", BindingValueUnits.Numeric);
            _leftNozzle.Execute += new HeliosActionHandler(LeftNozzlePosition_Execute);
            Actions.Add(_leftNozzle);
            _rightNozzle = new HeliosValue(this, BindingValue.Empty, "Engine Monitor Panel_Engine Nozzle Gauge", "Right Engine Nozzle Position", "Right Nozzle Position in %.", "", BindingValueUnits.Numeric);
            _rightNozzle.Execute += new HeliosActionHandler(RightNozzlePosition_Execute);
            Actions.Add(_rightNozzle);

            //_gireflection = new GaugeImage(_imageLocation + "IFEI Reflections.png", new Rect(0d, 0d, 779d, 702d));
            //Components.Add(_gireflection);
            //_gireflection.IsHidden = false;
            _giGaugeMarksL.IsHidden = true;
            _giGaugeMarksR.IsHidden = true;
            _giGaugeLegends.IsHidden = true;

            _indicatorGaugeLegends = new HeliosValue(this, new BindingValue(0d), "Engine Monitor Panel_Engine Nozzle Gauge", "Panel State ON/OFF", "True when panel is Off", "", BindingValueUnits.Boolean);
            _indicatorGaugeLegends.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorGaugeLegends);

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
                if(_gireflection != null)
                {
                    // clamp to max opacity
                    double newValue = Math.Min(value, 1.0);

                    double oldValue = _glassReflectionOpacity;
                    if (newValue != oldValue)
                    {
                        _glassReflectionOpacity = newValue;

                        // don't render at all if fully transparent
                        _gireflection.IsHidden = (newValue == 0.0);

                        // render at this opacity, if applicable
                        _gireflection.Opacity = newValue;

                        // notify change after change is made
                        OnPropertyChanged("GlassReflectionOpacity", oldValue, newValue, true);
                    }

                }
            }
        }
        #endregion

        protected override void OnProfileChanged(HeliosProfile oldProfile) {
            base.OnProfileChanged(oldProfile);
        }

        void LeftNozzlePosition_Execute(object action, HeliosActionEventArgs e)
        {          
            _gnLeftNoz.Rotation = _needleLeftCalibration.Interpolate(e.Value.DoubleValue);
        }

        void RightNozzlePosition_Execute(object action, HeliosActionEventArgs e)
        {
            _gnRightNoz.Rotation = _needleRightCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Indicator_Execute(object action, HeliosActionEventArgs e)
        {
            HeliosValue _haction = (HeliosValue)action;
            String _hactionVal = e.Value.StringValue;
            switch (_haction.Name)
            {
                case "Panel State ON/OFF":
                    _giGaugeMarksL.IsHidden = (_hactionVal == "1") ? false : true;
                    _giGaugeMarksR.IsHidden = (_hactionVal == "1") ? false : true;
                    _giGaugeLegends.IsHidden = (_hactionVal == "1") ? false : true;
                    _gnRightNoz.IsHidden = (_hactionVal == "1") ? false : true;
                    _gnLeftNoz.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                default:
                    break;
            }
        }
        public override bool HitTest(Point location)
        {
            if (_scaledScreenRect.Contains(location))
            {
                return false;
            }

            return true;
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
