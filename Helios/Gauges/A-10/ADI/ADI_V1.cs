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
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.A10.ADI.V1", "ADI (V1)", "A-10 Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class ADI_V1 : BaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _slipBall;
        private HeliosValue _bankSteering;
        private HeliosValue _pitchSteering;
        private HeliosValue _gsIndicator;

        private HeliosValue _offFlag;
        private HeliosValue _gsFlag;
        private HeliosValue _courseFlag;

        private GaugeImage _offFlagImage;
        private GaugeImage _gsFlagImage;
        private GaugeImage _courseFlagImage;

        private GaugeNeedle _ball;
        private GaugeNeedle _bankNeedle;
        private GaugeNeedle _slipBallNeedle;
        private GaugeNeedle _pitchSteeringNeedle;
        private GaugeNeedle _bankSteeringNeedle;
        private GaugeNeedle _gsIndicatorNeedle;

        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _pitchBarCalibration;
        private CalibrationPointCollectionDouble _bankBarCalibration;
        private CalibrationPointCollectionDouble _slipBallCalibration;
        private CalibrationPointCollectionDouble _gsCalibration;

        public ADI_V1()
            : base("ADI (V1)", new Size(350, 350))
        {
            Point center = new Point(174d, 163d);

            _pitchCalibration = new CalibrationPointCollectionDouble(-360d, -1066d, 360d, 1066d);
            _ball = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_ball.xaml", center, new Size(225d, 1350d), new Point(112.5d, 677d));
            _ball.Clip = new EllipseGeometry(center, 113d, 113d);
            Components.Add(_ball);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_inner_ring.xaml", new Rect(0d, 0d, 350d, 350d)));

            _slipBallCalibration = new CalibrationPointCollectionDouble(-1d, -26d, 1d, 26d);
            _slipBallNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_slip_ball.xaml", new Point(174d, 297d), new Size(10d, 10d), new Point(5d, 5d));
            Components.Add(_slipBallNeedle);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_guides.xaml", new Rect(0d, 0d, 350d, 350d)));

            _gsFlagImage = new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_gs_flag.xaml", new Rect(42d, 140d, 21d, 43d));
            _gsFlagImage.IsHidden = true;
            Components.Add(_gsFlagImage);

            _offFlagImage = new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_off_flag.xaml", new Rect(58d, 210d, 55d, 56d));
            _offFlagImage.IsHidden = true;
            Components.Add(_offFlagImage);

            _bankNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_bank_pointer.xaml", center, new Size(11d, 221d), new Point(5.5d, 110.5d));
            Components.Add(_bankNeedle);

            _pitchBarCalibration = new CalibrationPointCollectionDouble(-1d, -150d, 1d, 150d);
            _pitchSteeringNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_pitch_steering_bar.xaml", new Point(0d, 163d), new Size(252d, 6d), new Point(0d, 3d));
            _pitchSteeringNeedle.VerticalOffset = _pitchBarCalibration.Interpolate(-1d);
            Components.Add(_pitchSteeringNeedle);

            _bankBarCalibration = new CalibrationPointCollectionDouble(-1d, -128d, 1d, 134d);
            _bankSteeringNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_bank_steering_bar.xaml", new Point(175d, 0d), new Size(24d, 239d), new Point(23d, 0d));
            _bankSteeringNeedle.HorizontalOffset = _bankBarCalibration.Interpolate(-1d);
            Components.Add(_bankSteeringNeedle);

            _courseFlagImage = new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_course_flag.xaml", new Rect(151d, 35d, 44d, 26d));
            _courseFlagImage.IsHidden = true;
            Components.Add(_courseFlagImage);

            _gsCalibration = new CalibrationPointCollectionDouble(-1d, -66d, 1d, 66d);
            _gsIndicatorNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_gs_indicator.xaml", new Point(44d, 163d), new Size(14d, 12d), new Point(1d, 6d));
            Components.Add(_gsIndicatorNeedle);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_outer_ring.xaml", new Rect(0d, 0d, 350d, 350d)));

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_bezel.png", new Rect(0d, 0d, 350d, 350d)));

            _slipBall = new HeliosValue(this, new BindingValue(0d), "", "Slip Ball Offset", "Side slip indicator offset from the center of the tube.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _slipBall.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBall);

            _offFlag = new HeliosValue(this, new BindingValue(false), "", "Off Flag", "Indicates whether the off flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _gsFlag = new HeliosValue(this, new BindingValue(false), "", "Glide Slope Flag", "Indicates whether the glide scope flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _gsFlag.Execute += new HeliosActionHandler(GsFlag_Execute);
            Actions.Add(_gsFlag);

            _courseFlag = new HeliosValue(this, new BindingValue(false), "", "Course Flag", "Indicates whether the course flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _courseFlag.Execute += new HeliosActionHandler(CourseFlag_Execute);
            Actions.Add(_courseFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), "", "Pitch", "Current ptich of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _roll = new HeliosValue(this, new BindingValue(0d), "", "Bank", "Current bank of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

            _bankSteering = new HeliosValue(this, new BindingValue(1d), "", "Bank steering bar offset", "Location of bank steering bar.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _bankSteering.Execute += new HeliosActionHandler(BankSteering_Execute);
            Actions.Add(_bankSteering);

            _pitchSteering = new HeliosValue(this, new BindingValue(1d), "", "Pitch steering bar offset", "Location of pitch steering bar.", "(-1 to 1) 1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchSteering.Execute += new HeliosActionHandler(PitchSteering_Execute);
            Actions.Add(_pitchSteering);

            _gsIndicator = new HeliosValue(this, new BindingValue(0d), "", "Glide Scope Indicator Offset", "Location of glide scope indicator from middle of the scale.", "(-1 to 1) 1 full up and -1 is full down.", BindingValueUnits.Numeric);
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
            _ball.VerticalOffset = _pitchCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _roll.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Rotation = -e.Value.DoubleValue;
            _bankNeedle.Rotation = -e.Value.DoubleValue;
        }
    }
}
