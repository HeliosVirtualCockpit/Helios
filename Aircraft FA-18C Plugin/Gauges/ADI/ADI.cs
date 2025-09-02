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

    [HeliosControl("Helios.FA18C.ADI", "ADI 1", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
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
        private HeliosValue _altLightValue;

        private GaugeBall _ball;
        private GaugeNeedle _offFlagImage;
        private GaugeNeedle _bankNeedle;
        private GaugeNeedle _wingsNeedle;
        private GaugeNeedle _slipBallNeedle;
        private GaugeNeedle _TurnMarker;
        private GaugeNeedle _pitchSteeringNeedle;
        private GaugeNeedle _bankSteeringNeedle;

        private CalibrationPointCollectionDouble _pitchAdjustCalibaration;
        private CalibrationPointCollectionDouble _slipBallCalibration;
        private CalibrationPointCollectionDouble _pitchBarCalibration;
        private CalibrationPointCollectionDouble _bankBarCalibration;

        private bool _suppressScale = false;

        public ADI()
            : base("ADI", new Size(350, 350), "Alt")
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();
            
            Point center = new Point(177d, 163d);

            _ball = new GaugeBall("{FA-18C}/Gauges/ADI/ADI-Ball.xaml", new Point(71d, 57d), new Size(210d, 210d), 0d, -90d, 180d, 35d);
            Components.Add(_ball);
            _ball.LightingColorAlt = Color.FromArgb(0xff, 0x00, 0xff, 0x00);
            _ball.LightingColor = Color.FromArgb(0xff, 0xff, 0xff, 0xff);

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(-1.0d, -45d, 1.0d, 45d);
            _wingsNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/ADI-Wings.xaml", new Point(175d - 121d, 160d), new Size(204.025d, 29.333d), new Point(0d, 0d));
            Components.Add(_wingsNeedle);
            
            Components.Add(new GaugeImage("{helios}/Gauges/Common/Circular-Shading.xaml", new Rect(78d, 65d, 197d, 197d)));

            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/ADI-Innermost-Ring.xaml", new Rect(65d, 52d, 224d, 224d)));

            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/ADI-Inner-Ring.xaml", new Rect(30d, 23d, 287.451d, 313.572d)));
            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/ADI-Guides.xaml", new Rect(66d, 54d, 222d, 250d)));

            _bankNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/ADI-Arrow.xaml", center, new Size(17d, 110d), new Point(8.5d, 110d));
            Components.Add(_bankNeedle);

            _bankBarCalibration = new CalibrationPointCollectionDouble(-1d, -135d, 1d, 135d);
            _bankSteeringNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/ADI-GS-H.xaml", new Point(178d, 16d), new Size(44.186d, 262.436d), new Point(41.75d, 0d));
            _bankSteeringNeedle.HorizontalOffset = _bankBarCalibration.Interpolate(-1d);
            Components.Add(_bankSteeringNeedle);

            _pitchBarCalibration = new CalibrationPointCollectionDouble(-1d, -110d, 1d, 110d);
            _pitchSteeringNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/ADI-GS-V.xaml", new Point(42d, 166d), new Size(224.901d, 39.043d), new Point(0d, 38.5d));
            _pitchSteeringNeedle.VerticalOffset = _pitchBarCalibration.Interpolate(-1d);
            Components.Add(_pitchSteeringNeedle);

            _slipBallCalibration = new CalibrationPointCollectionDouble(-1d, -33d, 1d, 33d);
            _slipBallNeedle = new GaugeNeedle("{FA-18C}/Gauges/ADI/ADI-Slip-Ball.xaml", new Point(177d, 301d), new Size(10d, 10d), new Point(5d, 5d));
            Components.Add(_slipBallNeedle);

            _TurnMarker = new GaugeNeedle("{FA-18C}/Gauges/ADI/ADI-Turn-Marker.xaml", new Point(177d, 320d), new Size(14.4d, 12.5d), new Point(7.2d, 0d));
            Components.Add(_TurnMarker);


            _offFlagImage = new GaugeNeedle("{FA-18C}/Gauges/ADI/ADI-Off-Flag.xaml", new Point (310d, 75d), new Size( 31.052d, 122.769d), new Point(16d, 0d), -10d);
            _offFlagImage.IsHidden = false;
            Components.Add(_offFlagImage);

            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/ADI-Outer-Ring.xaml", new Rect(10d, 9d, 336d, 336d)));
            Components.Add(new GaugeImage("{FA-18C}/Gauges/ADI/ADI-Bezel.png", new Rect(0d, 0d, 350d, 350d)));

            _slipBall = new HeliosValue(this, new BindingValue(0d), "", "Slip Ball Offset", "Side slip indicator offset from the center of the tube.", "-1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _slipBall.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBall);

            _turnIndicator = new HeliosValue(this, new BindingValue(0d), "", "Turn Indicator Offset", "Turn indicator offset from the center of the gauge.", "-1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _turnIndicator.Execute += new HeliosActionHandler(turnIndicator_Execute);
            Actions.Add(_turnIndicator);

            _offFlag = new HeliosValue(this, new BindingValue(false), "", "Off Flag", "Indicates whether the off flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), "", "Pitch", "Current pitch of the aircraft.", "-90 to 90)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _pitchAdjustment = new HeliosValue(this, new BindingValue(0d), "", "Pitch adjustment offset", "Location of pitch reference wings.", "1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchAdjustment.Execute += new HeliosActionHandler(PitchAdjust_Execute);
            Actions.Add(_pitchAdjustment);

            _roll = new HeliosValue(this, new BindingValue(0d), "", "Bank", "Current bank of the aircraft.", "(-180 to +180)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

            _bankSteering = new HeliosValue(this, new BindingValue(1d), "", "Bank steering bar offset", "Location of bank steering bar.", "-1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _bankSteering.Execute += new HeliosActionHandler(BankSteering_Execute);
            Actions.Add(_bankSteering);

            _pitchSteering = new HeliosValue(this, new BindingValue(1d), "", "Pitch steering bar offset", "Location of pitch steering bar.", "1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchSteering.Execute += new HeliosActionHandler(PitchSteering_Execute);
            Actions.Add(_pitchSteering);

            _altLightValue = new HeliosValue(this, new BindingValue(false), "", "Alternate Lighting Source", "Boolean", "true if Alt Lighting is used", BindingValueUnits.Boolean);
            _altLightValue.Execute += new HeliosActionHandler(AltLightingUsed_Execute);
            Actions.Add(_altLightValue);

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
            _offFlagImage.Rotation = e.Value.BoolValue ? 10d : -10d;
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Yaw = e.Value.DoubleValue;
        }
        void PitchAdjust_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchAdjustment.SetValue(e.Value, e.BypassCascadingTriggers);
            _wingsNeedle.VerticalOffset = -_pitchAdjustCalibaration.Interpolate(e.Value.DoubleValue);
        }
        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _roll.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Roll = -e.Value.DoubleValue;
            _bankNeedle.Rotation = e.Value.DoubleValue;
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
        void AltLightingUsed_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.LightingAltEnabled = e.Value.BoolValue;
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
            _ball.Reset();
            base.EnableAlternateImageSet = false;
            _pitch.SetValue(new BindingValue(0d), true);
            _roll.SetValue(new BindingValue(0d), true);
            _pitchAdjustment.SetValue(new BindingValue(0d), true);
            _slipBall.SetValue(new BindingValue(0d), true);
            _turnIndicator.SetValue(new BindingValue(0d), true);
            _offFlag.SetValue(new BindingValue(false), true);
            _pitchSteering.SetValue(new BindingValue(-1d),true);
            _bankSteering.SetValue(new BindingValue(-1d), true);
        }
    }
}
