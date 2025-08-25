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

namespace GadrocsWorkshop.Helios.Gauges.F_16.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F16.ADI", "ADI", "F-16", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class ADI : BaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _slipBall;
        private HeliosValue _turn;
        private HeliosValue _ilsHorizontal;
        private HeliosValue _ilsVertical;

        private HeliosValue _auxFlag;
        private HeliosValue _offFlag;
        private HeliosValue _gsFlag;
        private HeliosValue _locFlag;

        private GaugeImage _auxFlagImage;
        private GaugeImage _offFlagImage;
        private GaugeImage _gsFlagImage;
        private GaugeImage _locFlagImage;

        private GaugeBall _ball;
        private GaugeNeedle _rollNeedle;
        private GaugeNeedle _slipBallNeedle;
        private GaugeNeedle _turnNeedle;
        private GaugeNeedle _ilsHorizontalNeedle;
        private GaugeNeedle _ilsVerticalNeedle;
        private GaugeNeedle _ilsScaleNeedle;

        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _ilsCalibration;
        private CalibrationPointCollectionDouble _slipBallCalibration;
        private CalibrationPointCollectionDouble _turnCalibration;

        public ADI()
            : base("ADI", new Size(350, 350))
        {
            double scale = 250d / 350d;

            _pitchCalibration = new CalibrationPointCollectionDouble(-360d, -180d, 360d, 180d);
            _ball = new GaugeBall("{F-16C}/Gauges/ADI/Viper-ADI-Ball.xaml", new Point(44d, 46d), new Size(250d, 250d), 0d, -90d, 180d, 35d);
            Components.Add(_ball);

            _rollNeedle = new GaugeNeedle("{F-16C}/Gauges/ADI/Viper-ADI-Roll-Arrows.xaml", new Point(175d, 175d), new Size(62.786d * scale, 308.846d * scale), new Point(62.786d * scale / 2d, 308.846d * scale / 2d));
            Components.Add(_rollNeedle);

            Components.Add(new GaugeImage("{F-16C}/Gauges/ADI/Viper-ADI-Wings.xaml", new Rect(50d, 175d - (10.250d / 2d), 352.917d * scale, 87.492d * scale)));

            _ilsCalibration = new CalibrationPointCollectionDouble(-1d, -116d, 1d, 116d);

            _ilsHorizontalNeedle = new GaugeNeedle("{F-16C}/Gauges/ADI/Viper-ADI-Horizontal-GS.xaml", new Point(175d, 175d), new Size(69.167d * scale, 292.250d * scale), new Point(66.001d * scale, (204.921d * scale) - 14d));
            _ilsHorizontalNeedle.HorizontalOffset = _ilsCalibration.Interpolate(-1d);
            Components.Add(_ilsHorizontalNeedle);

            _ilsVerticalNeedle = new GaugeNeedle("{F-16C}/Gauges/ADI/Viper-ADI-Vertical-GS.xaml", new Point(175d, 175d), new Size(299.778d * scale, 7.500d * scale), new Point(182.515d * scale, (3.750d * scale) / 2d));
            _ilsVerticalNeedle.VerticalOffset = _ilsCalibration.Interpolate(-1d);
            Components.Add(_ilsVerticalNeedle);

            Components.Add(new GaugeImage("{F-16C}/Gauges/ADI/adi_inner_ring.xaml", new Rect(0d, 0d, 350d, 350d)));

            Components.Add(new GaugeImage("{F-16C}/Gauges/ADI/adi_outer_ring.xaml", new Rect(0d, 0d, 350d, 350d)));

            _ilsScaleNeedle = new GaugeNeedle("{F-16C}/Gauges/ADI/adi_ils_scale_needle.xaml", new Point(30d, 174d), new Size(14d, 12d), new Point(1d, 6d));
            Components.Add(_ilsScaleNeedle);

            _gsFlagImage = new GaugeImage("{F-16C}/Gauges/ADI/Viper-ADI-Flags-GS.xaml", new Rect(61d, 112d, 22.500d * scale, 44.500d * scale), 1 , -30);
            _gsFlagImage.IsHidden = true;
            Components.Add(_gsFlagImage);

            _locFlagImage = new GaugeImage("{F-16C}/Gauges/ADI/Viper-ADI-Flags-LOC.xaml", new Rect(273d, 93d, 22.500d * scale, 61.500d * scale), 1, 30);
            _locFlagImage.IsHidden = true;
            Components.Add(_locFlagImage);

            _auxFlagImage = new GaugeImage("{F-16C}/Gauges/ADI/Viper-ADI-Flags-AUX.xaml", new Rect(273d, 187d, 22.500d * scale, 61.500d * scale), 1, -30);
            _auxFlagImage.IsHidden = true;
            Components.Add(_auxFlagImage);

            _offFlagImage = new GaugeImage("{F-16C}/Gauges/ADI/Viper-ADI-Flags-OFF.xaml", new Rect(57d, 187d, 22.500d * scale, 61.500d * scale), 1, 30);
            _offFlagImage.IsHidden = true;
            Components.Add(_offFlagImage);

            _slipBallCalibration = new CalibrationPointCollectionDouble(-1d, -44.75d, 1d, 44.75d);

            _slipBallNeedle = new GaugeNeedle("{F-16C}/Gauges/ADI/Viper-ADI-Slip-Ball.xaml", new Point(175d, 309d), new Size(12d, 12d), new Point(6d, 6d));
            Components.Add(_slipBallNeedle);

            Components.Add(new GaugeImage("{F-16C}/Gauges/ADI/adi_bezel.png", new Rect(0d, 0d, 350d, 350d)));

            _turnCalibration = new CalibrationPointCollectionDouble(-1d, -38.5d, 1d, 38.5d);
            _turnNeedle = new GaugeNeedle("{F-16C}/Gauges/ADI/Viper-ADI-Turn-Rate-Pointer.xaml", new Point(175d, 328d), new Size(24.5d, 12.5d), new Point(12.25d, 0d));
            Components.Add(_turnNeedle);


            _turn = new HeliosValue(this, new BindingValue(0d), "", "turn rate", "turn rate indicator offset.", "-1 to 1", BindingValueUnits.Numeric);
            _turn.Execute += new HeliosActionHandler(Turn_Execute);
            Actions.Add(_turn);

            _slipBall = new HeliosValue(this, new BindingValue(0d), "", "side slip", "Side slip indicator offset.", "-1 to 1", BindingValueUnits.Numeric);
            _slipBall.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBall);

            _auxFlag = new HeliosValue(this, new BindingValue(false), "", "aux flag", "Indicates whether the aux flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _auxFlag.Execute += new HeliosActionHandler(AuxFlag_Execute);
            Actions.Add(_auxFlag);

            _offFlag = new HeliosValue(this, new BindingValue(false), "", "off flag", "Indicates whether the off flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _gsFlag = new HeliosValue(this, new BindingValue(false), "", "loc flag", "Indicates whether the loc flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _gsFlag.Execute += new HeliosActionHandler(GsFlag_Execute);
            Actions.Add(_gsFlag);

            _locFlag = new HeliosValue(this, new BindingValue(false), "", "gs flag", "Indicates whether the gs flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _locFlag.Execute += new HeliosActionHandler(LocFlag_Execute);
            Actions.Add(_locFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), "", "pitch", "Current ptich of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _roll = new HeliosValue(this, new BindingValue(0d), "", "roll", "Current roll of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Roll_Execute);
            Actions.Add(_roll);

            _ilsHorizontal = new HeliosValue(this, new BindingValue(1d), "", "ils horizontal deviation", "Current deviation from glide scope.", "-1 to 1", BindingValueUnits.Numeric);
            _ilsHorizontal.Execute += new HeliosActionHandler(ILSHorizontal_Execute);
            Actions.Add(_ilsHorizontal);

            _ilsVertical = new HeliosValue(this, new BindingValue(1d), "", "ils vertical deviation", "Current deviation from ILS side scope.", "-1 to 1", BindingValueUnits.Numeric);
            _ilsVertical.Execute += new HeliosActionHandler(ILSVertical_Execute);
            Actions.Add(_ilsVertical);
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

        void AuxFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _auxFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _auxFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void GsFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _gsFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _gsFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void LocFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _locFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _locFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void ILSVertical_Execute(object action, HeliosActionEventArgs e)
        {
            _ilsVertical.SetValue(e.Value, e.BypassCascadingTriggers);
            _ilsVerticalNeedle.VerticalOffset = _ilsCalibration.Interpolate(e.Value.DoubleValue);
        }

        void ILSHorizontal_Execute(object action, HeliosActionEventArgs e)
        {
            _ilsHorizontal.SetValue(e.Value, e.BypassCascadingTriggers);
            _ilsHorizontalNeedle.HorizontalOffset = _ilsCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.Yaw = _pitchCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Roll_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.Roll = -e.Value.DoubleValue;
            _rollNeedle.Rotation = e.Value.DoubleValue;
        }
        public override void Reset()
        {
            base.Reset();
            _pitch.SetValue(new BindingValue(0d), true);
            _roll.SetValue(new BindingValue(0d), true);
            _slipBall.SetValue(new BindingValue(_slipBallCalibration.Interpolate(0d)), true);
            _turn.SetValue(new BindingValue(_turnCalibration.Interpolate(0d)), true);
            _ilsHorizontal.SetValue(new BindingValue(_ilsCalibration.Interpolate(-1d)), true);
            _ilsVertical.SetValue(new BindingValue(_ilsCalibration.Interpolate(-1d)), true);

            _auxFlag.SetValue(new BindingValue(true), true);
            _offFlag.SetValue(new BindingValue(true), true);
            _gsFlag.SetValue(new BindingValue(true), true);
            _locFlag.SetValue(new BindingValue(true), true);
        }
    }
}
