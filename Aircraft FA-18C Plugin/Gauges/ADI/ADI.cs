//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//,HeliosControlFlags.NotShownInUI
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GadrocsWorkshop.Helios.Gauges.FA18C.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using NLog.Targets;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.FA18C.ADI", "ADI", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class ADI : AltImageGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _pitchAdjustment;
        private HeliosValue _slipBall;
        private HeliosValue _turnIndicator;
        private HeliosValue _bankSteering;
        private HeliosValue _pitchSteering;

        private HeliosValue _offFlag;

        private GaugeImage _offFlagImage;

        private GaugeNeedle _ball;
        private GaugeNeedle _bankNeedle;
        private GaugeNeedle _wingsNeedle;
        private GaugeNeedle _slipBallNeedle;
        private GaugeNeedle _TurnMarker;
        private GaugeNeedle _pitchSteeringNeedle;
        private GaugeNeedle _bankSteeringNeedle;

        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _pitchAdjustCalibaration;
        private CalibrationPointCollectionDouble _slipBallCalibration;
        private CalibrationPointCollectionDouble _pitchBarCalibration;
        private CalibrationPointCollectionDouble _bankBarCalibration;

        public ADI()
            : base("ADI", new Size(350, 350), "Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();
            
            Point center = new Point(177d, 163d);

            _pitchCalibration = new CalibrationPointCollectionDouble(-360d, -1066d, 360d, 1066d);
            _ball = new GaugeNeedle("{FA-18C}/Gauges/ADI/adi_ball.png", center, new Size(198d, 1160d), new Point(99d, 580d));
            _ball.Clip = new EllipseGeometry(center, 99d, 99d);
            Components.Add(_ball);

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(-1.0d, -45d, 1.0d, 45d);
            _wingsNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/adi_wings.xaml", new Point(99d, 158d), new Size(157d, 31d), new Point(0d, 0d));
            Components.Add(_wingsNeedle);

            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/adi_innermost_ring.xaml", new Rect(65d, 52d, 224d, 224d)));
            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/adi_inner_ring.xaml", new Rect(30d, 23d, 287d, 305d)));

            _bankNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/adi_arrow.xaml", center, new Size(17d, 110d), new Point(8.5d, 110d));
            Components.Add(_bankNeedle);

            _slipBallCalibration = new CalibrationPointCollectionDouble(-1d, -26d, 1d, 26d);
            _slipBallNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/adi_slip_ball.xaml", new Point(176d, 297d), new Size(14d, 14d), new Point(7d, 7d));
            Components.Add(_slipBallNeedle);

            _TurnMarker = new GaugeNeedle("{FA-18C}/Gauges/ADI/adi_turn_marker.xaml", new Point(178d, 315d), new Size(12d, 9d), new Point(7d, 0d));
            Components.Add(_TurnMarker);

            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/adi_guides.xaml", new Rect(66d, 54d, 222d, 250d)));

            _pitchBarCalibration = new CalibrationPointCollectionDouble(-1d, -150d, 1d, 150d);
            _pitchSteeringNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/adi_pitch_steering_bar.xaml", new Point(0d, 163d), new Size(252d, 6d), new Point(0d, 3d));
            _pitchSteeringNeedle.VerticalOffset = _pitchBarCalibration.Interpolate(-1d);
            Components.Add(_pitchSteeringNeedle);

            _bankBarCalibration = new CalibrationPointCollectionDouble(-1d, -134d, 1d, 134d);
            _bankSteeringNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/adi_bank_steering_bar.xaml", new Point(175d, 0d), new Size(24d, 252d), new Point(23d, 0d));
            _bankSteeringNeedle.HorizontalOffset = _bankBarCalibration.Interpolate(-1d);
            Components.Add(_bankSteeringNeedle);

            _offFlagImage = new GaugeImage("{FA-18C}/Gauges/ADI/adi_off_flag.png", new Rect(270d, 40d, 44d, 166d));
            _offFlagImage.IsHidden = true;
            Components.Add(_offFlagImage);

            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/adi_outer_ring.xaml", new Rect(10d, 9d, 336d, 336d)));
            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/adi_bezel.png", new Rect(0d, 0d, 350d, 350d)));

            _slipBall = new HeliosValue(this, new BindingValue(0d), "", "Slip Ball Offset", "Side slip indicator offset from the center of the tube.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _slipBall.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBall);

            _turnIndicator = new HeliosValue(this, new BindingValue(0d), "", "Turn Indicator Offset", "Turn indicator offset from the center of the gauge.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _turnIndicator.Execute += new HeliosActionHandler(turnIndicator_Execute);
            Actions.Add(_turnIndicator);

            _offFlag = new HeliosValue(this, new BindingValue(false), "", "Off Flag", "Indicates whether the off flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), "", "Pitch", "Current pitch of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _pitchAdjustment = new HeliosValue(this, new BindingValue(0d), "", "Pitch adjustment offset", "Location of pitch reference wings.", "(-1 to 1) 1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchAdjustment.Execute += new HeliosActionHandler(PitchAdjust_Execute);
            Actions.Add(_pitchAdjustment);

            _roll = new HeliosValue(this, new BindingValue(0d), "", "Bank", "Current bank of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

            _bankSteering = new HeliosValue(this, new BindingValue(1d), "", "Bank steering bar offset", "Location of bank steering bar.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _bankSteering.Execute += new HeliosActionHandler(BankSteering_Execute);
            Actions.Add(_bankSteering);

            _pitchSteering = new HeliosValue(this, new BindingValue(1d), "", "Pitch steering bar offset", "Location of pitch steering bar.", "(-1 to 1) 1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchSteering.Execute += new HeliosActionHandler(PitchSteering_Execute);
            Actions.Add(_pitchSteering);

        }

        void CreateInputBindings()
        {
            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "Cockpit Lights.MODE Switch.changed",
                deviceActionName: "set.Enable Alternate Image Set",
                deviceTriggerName: "",
                triggerBindingValue: new BindingValue("return TriggerValue<3"),
                triggerBindingSource: BindingValueSources.LuaScript
                );

            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { "SAI.Pitch.changed", "set.Pitch" },
                { "SAI.Bank.changed", "set.Bank" },
                { "SAI.Warning Flag.changed", "set.Off Flag" },
                { "SAI.Rate of Turn.changed", "set.Turn Indicator Offset" },
                { "SAI.Slip Ball.changed", "set.Slip Ball Offset" },
                { "SAI.Pitch Adjustment.changed", "set.Pitch adjustment offset" },
                { "SAI.Bank Steering Bar.changed", "set.Bank steering bar offset" },
                { "SAI.Pitch Steering Bar.changed", "set.Pitch steering bar offset" }
            };

            foreach (string t in bindings.Keys)
            {
                AddDefaultInputBinding(
                    childName: "",
                    interfaceTriggerName: t,
                    deviceActionName: bindings[t]
                    ) ;
            }
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

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
        }
    }
}
