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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.SAI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AH64D.SAI.V1", "Standby Attitude Indicator (V1)", "AH-64D Apache", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class SAI_V1 : BaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _pitchAdjustment;
        private HeliosValue _slipBall;
        private HeliosValue _turnIndicator;

        private HeliosValue _offFlag;

        private GaugeImage _offFlagImage;

        private GaugeNeedle _ball;
        private GaugeNeedle _bankNeedle;
        private GaugeNeedle _wingsNeedle;
        private GaugeNeedle _slipBallNeedle;
        private GaugeNeedle _TurnMarker;

        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _pitchAdjustCalibaration;
        private CalibrationPointCollectionDouble _slipBallCalibration;

        public SAI_V1()
            : base("Standby Attitude Indicator (V1)", new Size(350, 350))
        {
            Point center = new Point(177d, 163d);

            _pitchCalibration = new CalibrationPointCollectionDouble(-360d, -1066d, 360d, 1066d);
            _ball = new GaugeNeedle("{AH-64D}/Images/SAI/V1/adi_ball.png", center, new Size(198d, 1160d), new Point(99d, 580d));
            _ball.Clip = new EllipseGeometry(center, 99d, 99d);
            Components.Add(_ball);

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(0.11d, -36d, 0.89d, 36d);
            _wingsNeedle = new GaugeNeedle("{AH-64D}/Images/SAI/V1/adi_wings.xaml", new Point(99d, 158d), new Size(157d, 31d), new Point(0d, 0d));
            Components.Add(_wingsNeedle);

            Components.Add(new GaugeImage("{AH-64D}/Images/SAI/V1/adi_innermost_ring.xaml", new Rect(65d, 52d, 224d, 224d)));
            Components.Add(new GaugeImage("{AH-64D}/Images/SAI/V1/adi_inner_ring.xaml", new Rect(30d, 23d, 287d, 305d)));

            _bankNeedle = new GaugeNeedle("{AH-64D}/Images/SAI/V1/adi_arrow.xaml", center, new Size(17d, 110d), new Point(8.5d, 110d));
            Components.Add(_bankNeedle);

            _slipBallCalibration = new CalibrationPointCollectionDouble(-1d, -26d, 1d, 26d);
            _slipBallNeedle = new GaugeNeedle("{AH-64D}/Images/SAI/V1/adi_slip_ball.xaml", new Point(176d, 297d), new Size(14d, 14d), new Point(7d, 7d));
            Components.Add(_slipBallNeedle);

            _TurnMarker = new GaugeNeedle("{AH-64D}/Images/SAI/V1/adi_turn_marker.xaml", new Point(178d, 315d), new Size(12d, 9d), new Point(7d, 0d));
            Components.Add(_TurnMarker);

            Components.Add(new GaugeImage("{AH-64D}/Images/SAI/V1/adi_guides.xaml", new Rect(66d, 54d, 222d, 250d)));

            _offFlagImage = new GaugeImage("{AH-64D}/Images/SAI/V1/adi_off_flag.png", new Rect(270d, 40d, 44d, 166d));
            _offFlagImage.IsHidden = true;
            Components.Add(_offFlagImage);

            Components.Add(new GaugeImage("{AH-64D}/Images/SAI/V1/adi_outer_ring.xaml", new Rect(10d, 9d, 336d, 336d)));

            Components.Add(new GaugeImage("{AH-64D}/Images/SAI/V1/adi_bezel.png", new Rect(0d, 0d, 350d, 350d)));

            _slipBall = new HeliosValue(this, new BindingValue(0d), "Standby Attitude Indicator", "Slip Ball Offset", "Side slip indicator offset from the center of the tube.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _slipBall.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBall);

            _turnIndicator = new HeliosValue(this, new BindingValue(0d), "Standby Attitude Indicator", "Turn Indicator Offset", "Turn indicator offset from the center of the gauge.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _turnIndicator.Execute += new HeliosActionHandler(turnIndicator_Execute);
            Actions.Add(_turnIndicator);

            _offFlag = new HeliosValue(this, new BindingValue(false), "Standby Attitude Indicator", "Off Flag", "Indicates whether the off flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), "Standby Attitude Indicator", "Pitch", "Current ptich of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _pitchAdjustment = new HeliosValue(this, new BindingValue(0d), "Standby Attitude Indicator", "SAI Pitch adjustment offset", "Location of pitch reference wings.", "(-1 to 1) 1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchAdjustment.Execute += new HeliosActionHandler(PitchAdjust_Execute);
            Actions.Add(_pitchAdjustment);

            _roll = new HeliosValue(this, new BindingValue(0d), "Standby Attitude Indicator", "Bank", "Current bank of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);
        }


        void SlipBall_Execute(object action, HeliosActionEventArgs e)
        {
            _slipBall.SetValue(e.Value, e.BypassCascadingTriggers);
            _slipBallNeedle.HorizontalOffset = _slipBallCalibration.Interpolate(e.Value.DoubleValue);
        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.VerticalOffset = _pitchCalibration.Interpolate(e.Value.DoubleValue);
        }
        void PitchAdjust_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchAdjustment.SetValue(e.Value, e.BypassCascadingTriggers);
            _wingsNeedle.VerticalOffset = -_pitchAdjustCalibaration.Interpolate(e.Value.DoubleValue);
        }
        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _roll.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Rotation = -e.Value.DoubleValue;
            _bankNeedle.Rotation = -e.Value.DoubleValue;
        }
        void turnIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _turnIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
            _TurnMarker.HorizontalOffset = _slipBallCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
