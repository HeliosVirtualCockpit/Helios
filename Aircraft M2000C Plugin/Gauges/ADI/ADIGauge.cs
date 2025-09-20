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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;


    [HeliosControl("Helios.M2000C.ADI.Gauge", "ADI", "M-2000C", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class ADIGauge : BaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _yaw;
        private HeliosValue _pitchAdjustment;
        private HeliosValue _offFlag;
        private HeliosValue _localizerH;
        private HeliosValue _localizerV;
        private HeliosValue _slipBall;
        private HeliosValue _rotationValue;
 
        private GaugeNeedle _offFlagNeedle;
        private GaugeBall _ball;
        private GaugeNeedle _bankNeedle;
        private GaugeNeedle _wingsNeedle;
        private GaugeNeedle _localizerHNeedle;
        private GaugeNeedle _localizerVNeedle;
        private GaugeNeedle _slipBallNeedle;

        private CalibrationPointCollectionDouble _slipCalibration;
        private CalibrationPointCollectionDouble _glideCalibration;
        private CalibrationPointCollectionDouble _pitchAdjustCalibaration;

        private bool _suppressScale = false;

        public ADIGauge(string name, Size size, string device)
            : base(name, size)
        {
            Point center = new Point(200d, 200d);

            _slipCalibration = new CalibrationPointCollectionDouble(-1d, 15d, 1d, -15d);
            _glideCalibration = new CalibrationPointCollectionDouble(-1d, -150d, 1d, 150d);

            _ball = new GaugeBall("{M2000C}/Gauges/ADI/ADI_Ball.xaml", new Point(50d,50d), new Size(300d, 300d), 0d, 0d, 180d, 36d);
            Components.Add(_ball);
            _ball.X = 0.00001d;
            _ball.Z = 0.00001d;
            _ball.Y = 0.00001d;
            _ball.LightingBrightness = 1.0d;

            Components.Add(new GaugeImage("{helios}/Gauges/Common/Circular-Shading.xaml", new Rect(57d, 57d, 286d, 286d)));

            _localizerHNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Localizer-H.xaml", center, new Size(5.939d, 158.540d), new Point(5.939d / 2d, 0d));
            _localizerHNeedle.HorizontalOffset = _glideCalibration.Interpolate(-1d);
            Components.Add(_localizerHNeedle);

            _localizerVNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Localizer-V.xaml", center, new Size(147.794d, 5.939d), new Point(0d, 5.939d / 2d));
            _localizerVNeedle.VerticalOffset = _glideCalibration.Interpolate(-1d);
            Components.Add(_localizerVNeedle);

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(-1.0d, -60d, 1.0d, 60d);
            _wingsNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Wings.xaml", new Point(52.3436d, 56.4174d), new Size(227.823d, 179.686d), new Point(0d, 0d));
            Components.Add(_wingsNeedle);

            _bankNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Roll_Pointer.xaml", center, new Size(23.336d, 137.488d), new Point(23.336d / 2d, 0d));
            Components.Add(_bankNeedle);

            _offFlagNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Off_Flag.xaml", new Point(60.2338d, 107.6953d), new Size(92.279d, 21.828d), new Point(0d, 21.828d / 2d), 0d);
            _offFlagNeedle.Rotation = -45d;
            Components.Add(_offFlagNeedle);

            Components.Add(new GaugeImage("{M2000C}/Gauges/ADI/ADI_Bezel.xaml", new Rect(0d, 0d, 400d, 400d)));

            _slipBallNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Slip_Ball.xaml", center, new Size(20.799d, 199.113d), new Point(20.799d / 2d, 0d));
            Components.Add(_slipBallNeedle);


            _offFlag = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "ADI OFF Flag", "Indicates the position of the OFF flag.", "true if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "ADI Pitch", "Current pitch of the aircraft in degrees.", "(-90 to +90)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _pitchAdjustment = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "ADI Pitch Adjust Knob", "Location of pitch reference wings.", "(-1 to 1) 1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchAdjustment.Execute += new HeliosActionHandler(PitchAdjust_Execute);
            Actions.Add(_pitchAdjustment);

            _roll = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "ADI Bank", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

            _yaw = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "ADI Heading", "Current heading of the aircraft in degrees.", "(0 to +360)", BindingValueUnits.Degrees);
            _yaw.Execute += new HeliosActionHandler(Yaw_Execute);
            Actions.Add(_yaw);

            _rotationValue = new HeliosValue(this, new BindingValue(""), $"{device}_{name}", "ADI Ball Rotation", "X/Y/Z angle changes for the ADI ball.", "Text containing three numbers x;y;z", BindingValueUnits.Text);
            _rotationValue.Execute += new HeliosActionHandler(Rotation_Execute);
            Actions.Add(_rotationValue);

            _slipBall = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "ADI Slip Ball", "Indicates the amount of slip.", "-1 to 1", BindingValueUnits.Numeric);
            _slipBall.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBall);

            _localizerH = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "ADI ILS GS HORIZ SLOPE", "Indicates the Horizontal Glide Slope Deviation.", "-1 to 1", BindingValueUnits.Numeric);
            _localizerH.Execute += new HeliosActionHandler(LocalizerH_Execute);
            Actions.Add(_localizerH);

            _localizerV = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "ADI ILS LOC VERT GLIDE", "Indicates the Vertical Glide Slope Deviation.", "-1 to 1", BindingValueUnits.Numeric);
            _localizerV.Execute += new HeliosActionHandler(LocalizerV_Execute);
            Actions.Add(_localizerV);
        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = e.Value.BoolValue ? 0d : -45d;
        }
        void SlipBall_Execute(object action, HeliosActionEventArgs e)
        {
            _slipBall.SetValue(e.Value, e.BypassCascadingTriggers);
            _slipBallNeedle.Rotation = _slipCalibration.Interpolate(e.Value.DoubleValue);
        }
        void LocalizerH_Execute(object action, HeliosActionEventArgs e)
        {
            _localizerH.SetValue(e.Value, e.BypassCascadingTriggers);
            _localizerHNeedle.HorizontalOffset = _glideCalibration.Interpolate(e.Value.DoubleValue);
        }
        void LocalizerV_Execute(object action, HeliosActionEventArgs e)
        {
            _localizerV.SetValue(e.Value, e.BypassCascadingTriggers);
            _localizerVNeedle.VerticalOffset = _glideCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.X = e.Value.DoubleValue;
        }
        void PitchAdjust_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchAdjustment.SetValue(e.Value, e.BypassCascadingTriggers);
            _wingsNeedle.VerticalOffset = -_pitchAdjustCalibaration.Interpolate(e.Value.DoubleValue);
        }
        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _roll.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Z = -e.Value.DoubleValue;
            _bankNeedle.Rotation = e.Value.DoubleValue;
        }
        void Yaw_Execute(object action, HeliosActionEventArgs e)
        {
            _yaw.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Y = -e.Value.DoubleValue;
        }
        void Rotation_Execute(object action, HeliosActionEventArgs e)
        {
            _rotationValue.SetValue(e.Value, e.BypassCascadingTriggers);
            string[] parts;
            parts = Tokenizer.TokenizeAtLeast(e.Value.StringValue, 3, ';');
            double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double x);
            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double y);
            double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double z);
            _ball.Rotation3D = new Point3D(x, -y, -z);
            _bankNeedle.Rotation = z;
        }
        public override bool EffectsExclusion
        {
            get => base.EffectsExclusion;
            set
            {
                if (!base.EffectsExclusion.Equals(value))
                {
                    base.EffectsExclusion = value;
                    OnPropertyChanged("EffectsExclusion", !value, value, true);
                }
            }
        }
        public override void ScaleChildren(double scaleX, double scaleY)
        {
            if (!_suppressScale)
            {
                _ball.ScaleChildren(scaleX, scaleY);
                _suppressScale = false;
            }
            base.ScaleChildren(scaleX, scaleY);
        }
        protected override void PostUpdateRectangle(Rect previous, Rect current)
        {
            _suppressScale = false;
            if (!previous.Equals(new Rect(0, 0, 0, 0)) && !(previous.Width == current.Width && previous.Height == current.Height))
            {
                _ball.ScaleChildren(current.Width / previous.Width, current.Height / previous.Height);
                _suppressScale = true;
            }
        }
        public override void Reset()
        {
            base.Reset();
            _pitch.SetValue(new BindingValue(0d), true);
            _roll.SetValue(new BindingValue(0d), true);
            _yaw.SetValue(new BindingValue(0d), true);
            _rotationValue.SetValue(new BindingValue("0;0;0"), true);
            _pitchAdjustment.SetValue(new BindingValue(0d), true);
            _localizerV.SetValue(new BindingValue(0d), true);
            _localizerH.SetValue(new BindingValue(0d), true);
            _slipBall.SetValue(new BindingValue(0d), true);
            _offFlag.SetValue(new BindingValue(false), true);
            _pitchAdjustment.SetValue(new BindingValue(0d), true);
        }
    }
}
