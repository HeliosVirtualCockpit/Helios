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

namespace GadrocsWorkshop.Helios.Gauges.A_10.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    [HeliosControl("Helios.A10.ADI", "ADI", "A-10 Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class ADI : AltImageGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _rotationValue;
        private HeliosValue _slipBall, _turn;
        private HeliosValue _bankSteering;
        private HeliosValue _pitchSteering;
        private HeliosValue _gsIndicator;

        private HeliosValue _offFlag;
        private HeliosValue _gsFlag;
        private HeliosValue _courseFlag;

        private GaugeImage _offFlagImage;
        private GaugeImage _gsFlagImage;
        private GaugeImage _courseFlagImage;

        private GaugeBall _ball;
        private GaugeNeedle _bankNeedle;
        private GaugeNeedle _slipBallNeedle;
        private GaugeNeedle _turnNeedle;
        private GaugeNeedle _pitchSteeringNeedle;
        private GaugeNeedle _bankSteeringNeedle;
        private GaugeNeedle _gsIndicatorNeedle;

        private CalibrationPointCollectionDouble _pitchBarCalibration;
        private CalibrationPointCollectionDouble _bankBarCalibration;
        private CalibrationPointCollectionDouble _slipBallCalibration, _turnCalibration;
        private CalibrationPointCollectionDouble _gsCalibration;

        private bool _suppressScale = false;

        public ADI()
            : base("ADI", new Size(350, 350))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.A10C.A10C1Interface), typeof(Interfaces.DCS.A10C.A10C2Interface), typeof(Interfaces.DCS.A10C.A10CInterface) };

            Point center = new Point(174d, 163d);

            _ball = new GaugeBall("{Helios}/Gauges/A-10/ADI/ADI-Ball.xaml", new Point(center.X- 112.5d, center.Y- 112.5d), new Size(225d, 225d), 0d, 0d, -90d, 35d);
            _ball.Clip = new EllipseGeometry(center, 112.5d, 112.5d);
            _ball.Y = -0.001d;
            _ball.Z = 0.001d;
            _ball.LightingColor = Colors.White;
            _ball.LightingBrightness = 1.0d;
            Components.Add(_ball);

            Components.Add(new GaugeImage("{helios}/Gauges/Common/Circular-Shading.xaml", new Rect(64d, 51d, 220d, 220d)));

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/ADI-Inner-Ring-and-Wings.xaml", new Rect(17d, 18d, 294.604d, 303.500d)));

            _slipBallCalibration = new CalibrationPointCollectionDouble(-1d, -28d, 1d, 28d);
            _slipBallNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/ADI-Slip-Ball.xaml", new Point(173.5, 296d), new Size(13d, 13d), new Point(13d / 2, 13d / 2d));
            Components.Add(_slipBallNeedle);

            _turnCalibration = new CalibrationPointCollectionDouble(-1d, -32d, 1d, 32d);
            _turnNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/ADI-Turn-Marker.xaml", new Point(173.5, 316.5d), new Size(14.250d, 8.104d), new Point(14.250d / 2d, 8.104d / 2));
            Components.Add(_turnNeedle);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/ADI-Tube-Marks.xaml", new Rect(17d, 17d, 193.678d, 286.069d)));

            _bankNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/ADI-Roll-Markers.xaml", new Point(center.X, center.Y - 1d), new Size(9.413d, 219.967d), new Point(9.413d / 2d, 219.967d / 2d));
            Components.Add(_bankNeedle);

            _pitchBarCalibration = new CalibrationPointCollectionDouble(-1d, -150d, 1d, 150d) { new CalibrationPointDouble(0d, 0d) };
            _pitchSteeringNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/ADI-GlideSlope-V.xaml", new Point(174d, 164d - (6d / 2d)), new Size(213.747d, 6d), new Point(81.600d, 6d / 2d),0d);
            _pitchSteeringNeedle.VerticalOffset = _pitchBarCalibration.Interpolate(1d);
            Components.Add(_pitchSteeringNeedle);

            _bankBarCalibration = new CalibrationPointCollectionDouble(-1d, -128d, 1d, 134d) { new CalibrationPointDouble(0d, 0d) };
            _bankSteeringNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/ADI-GlideSlope-H.xaml", new Point(174d - (6d / 2d) , 163d), new Size(6d, 213.747d), new Point(6d / 2d, 143.807d));
            _bankSteeringNeedle.HorizontalOffset = _bankBarCalibration.Interpolate(1d);
            Components.Add(_bankSteeringNeedle);

            _gsCalibration = new CalibrationPointCollectionDouble(-1d, -60d, 1d, 60d);
            _gsIndicatorNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/ADI-GS-Arrow.xaml", new Point(44d, 163d), new Size(14d, 12d), new Point(1d, 6d));
            Components.Add(_gsIndicatorNeedle);

            _courseFlagImage = new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_course_flag.xaml", new Rect(151d, 35d, 44d, 26d));
            _courseFlagImage.IsHidden = true;
            Components.Add(_courseFlagImage);

            _offFlagImage = new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_off_flag.xaml", new Rect(58d, 210d, 55d, 56d));
            _offFlagImage.IsHidden = true;
            Components.Add(_offFlagImage);

            _gsFlagImage = new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_gs_flag.xaml", new Rect(42d, 140d, 21d, 43d));
            _gsFlagImage.IsHidden = true;
            Components.Add(_gsFlagImage);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/ADI-Outer-Ring.xaml", new Rect(17d, 18d, 321.000d, 321.000d)));

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/ADI-Bezel.png", new Rect(0d, 0d, 350d, 350d)));

            _slipBall = new HeliosValue(this, new BindingValue(0d), "", "Slip Ball Offset", "Side slip indicator offset from the center of the tube.", "-1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _slipBall.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBall);
            
            _turn = new HeliosValue(this, new BindingValue(0d), "", "Turn Indicator Offset", "Turn indicator offset from the center of the tube.", "-1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _turn.Execute += new HeliosActionHandler(Turn_Execute);
            Actions.Add(_turn);

            _offFlag = new HeliosValue(this, new BindingValue(false), "", "Off Flag", "Indicates whether the off flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _gsFlag = new HeliosValue(this, new BindingValue(false), "", "Glide Slope Flag", "Indicates whether the glide scope flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _gsFlag.Execute += new HeliosActionHandler(GsFlag_Execute);
            Actions.Add(_gsFlag);

            _courseFlag = new HeliosValue(this, new BindingValue(false), "", "Course Flag", "Indicates whether the course flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _courseFlag.Execute += new HeliosActionHandler(CourseFlag_Execute);
            Actions.Add(_courseFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), "", "Pitch", "Current pitch of the aircraft in degrees.", "-90 to +90", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _roll = new HeliosValue(this, new BindingValue(0d), "", "Bank", "Current bank of the aircraft in degrees", "-180 to +180", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

            _rotationValue = new HeliosValue(this, new BindingValue(""), "", "ADI ball rotation", "X/Y/Z angle changes for the ADI ball.", "Text containing three numbers x;y;z", BindingValueUnits.Text);
            _rotationValue.Execute += new HeliosActionHandler(Rotation_Execute);
            Actions.Add(_rotationValue);

            _bankSteering = new HeliosValue(this, new BindingValue(1d), "", "Bank steering bar offset", "Location of bank steering bar.", "-1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _bankSteering.Execute += new HeliosActionHandler(BankSteering_Execute);
            Actions.Add(_bankSteering);

            _pitchSteering = new HeliosValue(this, new BindingValue(1d), "", "Pitch steering bar offset", "Location of pitch steering bar.", "1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchSteering.Execute += new HeliosActionHandler(PitchSteering_Execute);
            Actions.Add(_pitchSteering);

            _gsIndicator = new HeliosValue(this, new BindingValue(0d), "", "Glide Scope Indicator Offset", "Location of glide scope indicator from middle of the scale.", "1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _gsIndicator.Execute += new HeliosActionHandler(GsIndicator_Execute);
            Actions.Add(_gsIndicator);
        }

        void GsIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _gsIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
            _gsIndicatorNeedle.VerticalOffset = -_gsCalibration.Interpolate(e.Value.DoubleValue);
        }

        void SlipBall_Execute(object action, HeliosActionEventArgs e)
        {
            _slipBall.SetValue(e.Value, e.BypassCascadingTriggers);
            _slipBallNeedle.HorizontalOffset = _slipBallCalibration.Interpolate(e.Value.DoubleValue);
        }
        void Turn_Execute(object action, HeliosActionEventArgs e)
        {
            _turn.SetValue(e.Value, e.BypassCascadingTriggers);
            _turnNeedle.HorizontalOffset = _turnCalibration.Interpolate(e.Value.DoubleValue);
        }

        void GsFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _gsFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _gsFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void CourseFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _courseFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _courseFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void PitchSteering_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchSteering.SetValue(e.Value, e.BypassCascadingTriggers);
            _pitchSteeringNeedle.VerticalOffset = -_pitchBarCalibration.Interpolate(e.Value.DoubleValue);
        }

        void BankSteering_Execute(object action, HeliosActionEventArgs e)
        {
            _bankSteering.SetValue(e.Value, e.BypassCascadingTriggers);
            _bankSteeringNeedle.HorizontalOffset = _bankBarCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.X = e.Value.DoubleValue;
        }

        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _roll.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Z = e.Value.DoubleValue;
            _bankNeedle.Rotation = -e.Value.DoubleValue;
        }
        void Rotation_Execute(object action, HeliosActionEventArgs e)
        {
            _rotationValue.SetValue(e.Value, e.BypassCascadingTriggers);
            string[] parts;
            parts = Tokenizer.TokenizeAtLeast(e.Value.StringValue, 3, ';');
            double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double x);
            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double y);
            double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double z);
            _ball.Rotation3D = new Point3D(-x, y, z);
            _bankNeedle.Rotation = -z;
        }
        public override bool EnableAlternateImageSet
        {
            get => base.EnableAlternateImageSet;

            set
            {
                bool newValue = value;
                bool oldValue = base.EnableAlternateImageSet;

                if (newValue != oldValue)
                {
                    base.EnableAlternateImageSet = newValue;
                }
            }
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
        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
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
            base.EnableAlternateImageSet = false;
            _ball.Reset();
            _bankNeedle.Rotation = 0d;
            _gsIndicatorNeedle.VerticalOffset = 0d;
            _slipBallNeedle.HorizontalOffset = 0d;
            _turnNeedle.HorizontalOffset = 0d;
            _offFlagImage.IsHidden = true;
            _gsFlagImage.IsHidden = true;
            _courseFlagImage.IsHidden = true;
            _pitchSteeringNeedle.VerticalOffset = -_pitchBarCalibration.Interpolate(1d);
            _bankSteeringNeedle.HorizontalOffset = _bankBarCalibration.Interpolate(1d);

            _pitchSteering.SetValue(new BindingValue(1d), true);
            _pitch.SetValue(new BindingValue(0d), true);
            _roll.SetValue(new BindingValue(0d), true);
            _rotationValue.SetValue(new BindingValue("0;0;0"), true);
            _slipBall.SetValue(new BindingValue(0d), true);
            _turn.SetValue(new BindingValue(0d), true);
            _offFlag.SetValue(new BindingValue(false), true);
            _bankSteering.SetValue(new BindingValue(1d), true);

        }

    }
}
