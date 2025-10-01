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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.UH60L;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Xml.Linq;

    public class VSI : CompositeBaseGauge
    {
        private readonly HeliosValue _pitch, _roll, _rotationValue;
        private readonly HeliosValue _slipBall, _turn;
        private readonly HeliosValue _gsHorizontal, _gsVertical;
        private readonly HeliosValue _collectiveIndicatorValue, _trackErrorIndicatorValue, _gsIndicatorValue;
        private readonly HeliosValue _gaIndicatorValue, _dhIndicatorValue, _mbIndicatorValue;

        private readonly HeliosValue _attFlag, _cmdFlag, _gsFlag, _navFlag;

        private readonly GaugeNeedle _attFlagNeedle, _cmdFlagNeedle, _gsFlagNeedle, _navFlagNeedle;

        private readonly GaugeBall _ball;
        private readonly GaugeNeedle _rollNeedle;
        private readonly GaugeNeedle _slipBallNeedle, _turnNeedle;
        private readonly GaugeNeedle _gsHorizontalNeedle, _gsVerticalNeedle;
        private readonly GaugeNeedle _glideSlopeIndicatorNeedle, _trackErrorNeedle, _collectiveIndicatorNeedle;

        private readonly GaugeImage _gaIndicatorLit, _dhIndicatorLit, _mbIndicatorLit;

        private readonly CalibrationPointCollectionDouble _ilsHCalibration, _ilsVCalibration;
        private readonly CalibrationPointCollectionDouble _slipBallCalibration, _turnCalibration;
        private readonly CalibrationPointCollectionDouble _trackErrorScale, _collectiveScale, _gsScale;
        private readonly FLYER _flyer;
        private readonly Dictionary<string, string> _bindingDictionary = new Dictionary<string, string>();

        private bool _suppressScale = false;

        public VSI(FLYER flyer, Size size)
            : base($"VSI ({flyer})", size)
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.UH60L.UH60LInterface), typeof(Interfaces.DCS.Soft.SoftInterface) };
            SupportedSoftInterfaceNames = new[] { "DCS H-60 (UH-60L Blackhawk)", "DCS H-60 (MH-60R Seahawk)" };

            _flyer = flyer;
            _trackErrorScale = new CalibrationPointCollectionDouble(-1d, -110d, 1d, 110d);
            _collectiveScale = new CalibrationPointCollectionDouble(-1d, 200d, 1d, 200d) {
                new CalibrationPointDouble(-0.5, 110),
                new CalibrationPointDouble(0d, 0d),
                new CalibrationPointDouble(0.5d, -110)
            };
            _gsScale = new CalibrationPointCollectionDouble(-1d, -92d, 1d, 92d);
            _ilsHCalibration = new CalibrationPointCollectionDouble(-1d, 210d, 1d, 210d) {
                new CalibrationPointDouble(-0.5d, -120),
                new CalibrationPointDouble(0d, 0d),
                new CalibrationPointDouble(0.5d, 120)
            };
            _ilsVCalibration = new CalibrationPointCollectionDouble(-1d, 133d, 1d, 133d) {
                new CalibrationPointDouble(-0.5d, -38),
                new CalibrationPointDouble(0d, 0d),
                new CalibrationPointDouble(0.5d, 100)
            };
            _slipBallCalibration = new CalibrationPointCollectionDouble(-1d, -60d, 1d, 60d);
            _turnCalibration = new CalibrationPointCollectionDouble(-1d, -48d, 1d, 48d);


            _ball = new GaugeBall("{UH-60L}/Gauges/VSI/VSI-Ball-Horizontal.xaml", new Point(148d, 148d), new Size(300d, 300d), 0d, 0d, -90d, 40d);
            Components.Add(_ball);
            _ball.Y = 0.0001d;
            _ball.Z = 0.0001d;
            _ball.LightingBrightness = 1.0d;

            Components.Add(new GaugeImage("{helios}/Gauges/Common/Circular-Shading.xaml", new Rect(156d, 156d, 280d, 280d)));
            Components.Add(new GaugeImage("{UH-60L}/Gauges/VSI/VSI-Gauge-Surround.xaml", new Rect(0d, 0d, 596d, 604d)));

            Components.Add(new GaugeImage("{UH-60L}/Gauges/VSI/VSI-Gauge-Furniture.xaml", new Rect(54d, 133d, 457d, 323d)));

            _rollNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-Roll-Pointer.xaml", new Point(295d, 295d), new Size(11.500d, 22.500d), new Point(5.525d, 146d));
            Components.Add(_rollNeedle);

            _gsHorizontalNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-GS-H.xaml", new Point(295d, 295d), new Size(27.000d, 336.000d), new Point(22d, 221d));
            _gsHorizontalNeedle.HorizontalOffset = _ilsHCalibration.Interpolate(0d);
            Components.Add(_gsHorizontalNeedle);

            _gsVerticalNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-GS-V.xaml", new Point(295d, 295d), new Size(355.000d, 3.000d), new Point(236d, -1d));
            _gsVerticalNeedle.VerticalOffset = _ilsVCalibration.Interpolate(0d);
            Components.Add(_gsVerticalNeedle);

            _trackErrorNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-Track-Error-Pointer.xaml", new Point(296d, 452d), new Size(13.500d, 21.500d), new Point(6.525d, 0d));
            Components.Add(_trackErrorNeedle);

            _glideSlopeIndicatorNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-Glide-Slope-Pointer.xaml", new Point(473d, 296d), new Size(21.500d, 13.500d), new Point(0d, 6.525d));
            Components.Add(_glideSlopeIndicatorNeedle);

            _collectiveIndicatorNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-Collective-Indicator.xaml", new Point(75d, 295d), new Size(50.562d, 10.501d), new Point(0d, 2.25d));
            Components.Add(_collectiveIndicatorNeedle);

            _gsFlagNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-GS-Flag.xaml", new Point(451d, 314d), new Size(59.279d, 102.276d), new Point(0d, 0d));
            Components.Add(_gsFlagNeedle);

            _navFlagNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-NAV-Flag.xaml", new Point(123d, 410d), new Size(54d, 23d), new Point(0d, 0d));
            Components.Add(_navFlagNeedle);

            _attFlagNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-ATT-Flag.xaml", new Point(400d, 128d), new Size(79.009d, 44.003d), new Point(0, 0));
            Components.Add(_attFlagNeedle);

            _cmdFlagNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-CMD-Flag.xaml", new Point(106d, 128d), new Size(79.009d, 44.003d), new Point(0d, 0d));
            Components.Add(_cmdFlagNeedle);

            _slipBallNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-Slip-Ball.xaml", new Point(295d, 495d), new Size(16d, 16d), new Point(8d, 8d));
            Components.Add(_slipBallNeedle);

            Components.Add(new GaugeImage("{UH-60L}/Gauges/VSI/VSI-Bezel-Outside.xaml", new Rect(0d, 0d, 596d, 604d)));

            _turnNeedle = new GaugeNeedle("{UH-60L}/Gauges/VSI/VSI-Turn-Pointer.xaml", new Point(295d, 518d), new Size(23.500d, 13.500d), new Point(11.525d, 0d));
            Components.Add(_turnNeedle);

            Components.Add(new GaugeImage("{UH-60L}/Gauges/VSI/VSI-GA-Unlit.xaml", new Rect(157.180d - 38.782d, 66.766d - 26.559d, 38.782d, 26.559d), 1d, 0d));
            Components.Add(new GaugeImage("{UH-60L}/Gauges/VSI/VSI-DH-Unlit.xaml", new Rect(265.194d - 38.782d, 66.766d - 26.559d, 38.782d, 26.559d), 1d, 0d));
            Components.Add(new GaugeImage("{UH-60L}/Gauges/VSI/VSI-MB-Unlit.xaml", new Rect(373.207d - 38.782d, 66.766d - 26.559d, 38.782d, 26.559d), 1d, 0d));

            _gaIndicatorLit = new GaugeImage("{UH-60L}/Gauges/VSI/VSI-GA-Lit.xaml", new Rect(157.180 - 38.782d, 66.766d - 26.559d, 38.782d, 26.559d), 1d, 0d);
            _gaIndicatorLit.IsHidden = true;
            Components.Add(_gaIndicatorLit);
            _dhIndicatorLit = new GaugeImage("{UH-60L}/Gauges/VSI/VSI-DH-Lit.xaml", new Rect(265.194d - 38.782d, 66.766d - 26.559d, 38.782d, 26.559d), 1d, 0d);
            _dhIndicatorLit.IsHidden = true;
            Components.Add(_dhIndicatorLit);
            _mbIndicatorLit = new GaugeImage("{UH-60L}/Gauges/VSI/VSI-MB-Lit.xaml", new Rect(373.207d - 38.782d, 66.766d - 26.559d, 38.782d, 26.559d), 1d, 0d);
            _mbIndicatorLit.IsHidden = true;
            Components.Add(_mbIndicatorLit);

            // dummy knobs because the UH-60L V2 mod does not implement them. 
            Components.Add(new GaugeImage("{UH-60L}/Gauges/VSI/VSI-Knob.xaml", new Rect(25, 475, 101.025d, 101.036d), 1d, 90d));
            Components.Add(new GaugeImage("{UH-60L}/Gauges/VSI/VSI-Knob.xaml", new Rect(460, 475, 101.025d, 101.036d), 1d, 0d));

            foreach (GaugeComponent gc in Components)
            {
                gc.EffectsExclusion = this.EffectsExclusion;
            }
            _pitch = new HeliosValue(this, new BindingValue(0d), "", "pitch", "Current pitch of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _roll = new HeliosValue(this, new BindingValue(0d), "", "roll", "Current roll of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Roll_Execute);
            Actions.Add(_roll);

            _rotationValue = new HeliosValue(this, new BindingValue(""), "", "ball rotation", "X/Y/Z angle changes for the ADI ball.", "Text containing three numbers x;y;z", BindingValueUnits.Text);
            _rotationValue.Execute += new HeliosActionHandler(Rotation_Execute);
            Actions.Add(_rotationValue);

            _slipBall = new HeliosValue(this, new BindingValue(0d), "", "side slip", "Side slip indicator offset.", "-1 to 1", BindingValueUnits.Numeric);
            _slipBall.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBall);

            _turn = new HeliosValue(this, new BindingValue(0d), "", "turn rate", "turn rate indicator offset.", "-1 to 1", BindingValueUnits.Numeric);
            _turn.Execute += new HeliosActionHandler(Turn_Execute);
            Actions.Add(_turn);

            _gsHorizontal = new HeliosValue(this, new BindingValue(1d), "", "GS horizontal deviation", "Current deviation from glide slope.", "-1 to 1", BindingValueUnits.Numeric);
            _gsHorizontal.Execute += new HeliosActionHandler(GSHorizontal_Execute);
            Actions.Add(_gsHorizontal);

            _gsVertical = new HeliosValue(this, new BindingValue(1d), "", "GS vertical deviation", "Current deviation from glide slope.", "-1 to 1", BindingValueUnits.Numeric);
            _gsVertical.Execute += new HeliosActionHandler(GSVertical_Execute);
            Actions.Add(_gsVertical);

            _collectiveIndicatorValue = new HeliosValue(this, new BindingValue(1d), "", "Collective Indicator", "Number.", "-1 to 1", BindingValueUnits.Numeric);
            _collectiveIndicatorValue.Execute += new HeliosActionHandler(CollectiveIndicator_Execute);
            Actions.Add(_collectiveIndicatorValue);

            _trackErrorIndicatorValue = new HeliosValue(this, new BindingValue(1d), "", "Track Error Indicator", "Number.", "-1 to 1", BindingValueUnits.Numeric);
            _trackErrorIndicatorValue.Execute += new HeliosActionHandler(TrackErrorIndicator_Execute);
            Actions.Add(_trackErrorIndicatorValue);

            _gsIndicatorValue = new HeliosValue(this, new BindingValue(1d), "", "Glide Slope Indicator", "Number.", "-1 to 1", BindingValueUnits.Numeric);
            _gsIndicatorValue.Execute += new HeliosActionHandler(GSIndicator_Execute);
            Actions.Add(_gsIndicatorValue);

            _attFlag = new HeliosValue(this, new BindingValue(0d), "", "ATT flag", "Indicates whether the ATT flag is displayed.", "0 to 1", BindingValueUnits.Numeric);
            _attFlag.Execute += new HeliosActionHandler(AttFlag_Execute);
            Actions.Add(_attFlag);

            _cmdFlag = new HeliosValue(this, new BindingValue(0d), "", "CMD flag", "Indicates whether the CMD flag is displayed.", "0 to 1", BindingValueUnits.Numeric);
            _cmdFlag.Execute += new HeliosActionHandler(CmdFlag_Execute);
            Actions.Add(_cmdFlag);

            _gsFlag = new HeliosValue(this, new BindingValue(0d), "", "GS flag", "Indicates whether the GS flag is displayed.", "0 to 1", BindingValueUnits.Numeric);
            _gsFlag.Execute += new HeliosActionHandler(GsFlag_Execute);
            Actions.Add(_gsFlag);

            _navFlag = new HeliosValue(this, new BindingValue(0d), "", "NAV flag", "Indicates whether the NAV flag is displayed.", "0 to 1", BindingValueUnits.Numeric);
            _navFlag.Execute += new HeliosActionHandler(NavFlag_Execute);
            Actions.Add(_navFlag);

            _gaIndicatorValue = new HeliosValue(this, new BindingValue(false), "", "GA Indicator", "Indicates whether the GA Indicator is displayed.", "0 to 1", BindingValueUnits.Boolean);
            _gaIndicatorValue.Execute += new HeliosActionHandler(GAIndicator_Execute);
            Actions.Add(_gaIndicatorValue);

            _dhIndicatorValue = new HeliosValue(this, new BindingValue(false), "", "DH Indicator", "Indicates whether the GA Indicator is displayed.", "0 to 1", BindingValueUnits.Boolean);
            _dhIndicatorValue.Execute += new HeliosActionHandler(DHIndicator_Execute);
            Actions.Add(_dhIndicatorValue);

            _mbIndicatorValue = new HeliosValue(this, new BindingValue(false), "", "MB Indicator", "Indicates whether the GA Indicator is displayed.", "0 to 1", BindingValueUnits.Boolean);
            _mbIndicatorValue.Execute += new HeliosActionHandler(MBIndicator_Execute);
            Actions.Add(_mbIndicatorValue);

            _bindingDictionary.Add("ball rotation", "_ball rotation");
            _bindingDictionary.Add("side slip", "_VSI_SLIP_IND");
            _bindingDictionary.Add("turn rate", "_VSI_TURN_RATE_IND");
            _bindingDictionary.Add("GS horizontal deviation", "_VSI_ROLL_CMD_BAR");
            _bindingDictionary.Add("GS vertical deviation", "_VSI_PITCH_CMD_BAR");
            _bindingDictionary.Add("Collective Indicator", "_VSI_COLLECTIVE_CMD_BAR");
            _bindingDictionary.Add("Track Error Indicator", "_VSI_TRACK_ERROR_IND");
            _bindingDictionary.Add("Glide Slope Indicator", "_VSI_GLIDE_SLOPE_IND");
            _bindingDictionary.Add("ATT flag", "_VSI_ATT_FLAG");
            _bindingDictionary.Add("CMD flag", "_VSI_CMD_FLAG");
            _bindingDictionary.Add("GS flag", "_VSI_GS_FLAG");
            _bindingDictionary.Add("NAV flag", "_VSI_NAV_FLAG");
            _bindingDictionary.Add("GA Indicator", "VSILtGA");
            _bindingDictionary.Add("DH Indicator", "_APN209_LOLIGHT");
            _bindingDictionary.Add("MB Indicator", "VSILtMB");

            BuildBindings();
        }
        private void BuildBindings() {
            

            foreach (IBindingAction action in Actions)
            {
                if (!(action.Name.Contains("hidden") || action.Name.Contains("pitch") || action.Name.Contains("roll")))
                {
                    AddDefaultInputBinding(
                        childName: "",
                        interfaceTriggerName: $"VSI ({_flyer}).{action.Name}.changed",
                        deviceActionName: action.ActionID);
                    
                    string flyer;
                    if (_bindingDictionary.ContainsKey(action.Name))
                    {
                        flyer = _bindingDictionary[action.Name].StartsWith("_") ? _flyer.ToString().ToUpperInvariant() : _flyer.ToString().ToLowerInvariant();
                        AddDefaultInputBinding(
                            childName: "",
                            interfaceTriggerName: $"{_flyer.ToString().ToUpperInvariant()} VSI.{flyer}{_bindingDictionary[action.Name]}.changed",
                            deviceActionName: action.ActionID);

                    }
                }
            }

        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.X = e.Value.DoubleValue;
        }

        void Roll_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.Z = -e.Value.DoubleValue;
            _rollNeedle.Rotation = e.Value.DoubleValue;
        }
        void Rotation_Execute(object action, HeliosActionEventArgs e)
        {
            _rotationValue.SetValue(e.Value, e.BypassCascadingTriggers);
            string[] parts;
            parts = Tokenizer.TokenizeAtLeast(e.Value.StringValue, 3, ';');
            double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double x);
            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double y);
            double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double z);
            _ball.Rotation3D = new Point3D(x, y, z);
            _rollNeedle.Rotation = -z;
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
        void GSVertical_Execute(object action, HeliosActionEventArgs e)
        {
            _gsVertical.SetValue(e.Value, e.BypassCascadingTriggers);
            _gsVerticalNeedle.VerticalOffset = _ilsVCalibration.Interpolate(e.Value.DoubleValue);
        }

        void GSHorizontal_Execute(object action, HeliosActionEventArgs e)
        {
            _gsHorizontal.SetValue(e.Value, e.BypassCascadingTriggers);
            _gsHorizontalNeedle.HorizontalOffset = _ilsHCalibration.Interpolate(e.Value.DoubleValue);
        }
        void CollectiveIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _collectiveIndicatorValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _collectiveIndicatorNeedle.VerticalOffset = _collectiveScale.Interpolate(e.Value.DoubleValue);
        }
        void TrackErrorIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _trackErrorIndicatorValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _trackErrorNeedle.HorizontalOffset = _trackErrorScale.Interpolate(e.Value.DoubleValue);
        }
        void GSIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _gsIndicatorValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _glideSlopeIndicatorNeedle.VerticalOffset = _gsScale.Interpolate(e.Value.DoubleValue);
        }

        void AttFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _attFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _attFlagNeedle.VerticalOffset = -e.Value.DoubleValue * 90;
        }

        void GsFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _gsFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _gsFlagNeedle.VerticalOffset = e.Value.DoubleValue * 100;
            _gsFlagNeedle.HorizontalOffset = e.Value.DoubleValue * 50;
        }

        void NavFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _navFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _navFlagNeedle.VerticalOffset = e.Value.DoubleValue * 90;
            _navFlagNeedle.HorizontalOffset = -e.Value.DoubleValue * 90;
        }

        void CmdFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _cmdFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _cmdFlagNeedle.VerticalOffset = -e.Value.DoubleValue * 90;
        }

        void GAIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _gaIndicatorValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _gaIndicatorLit.IsHidden = !e.Value.BoolValue;
        }
        void DHIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _dhIndicatorValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _dhIndicatorLit.IsHidden = !e.Value.BoolValue;
        }
        void MBIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _mbIndicatorValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _mbIndicatorLit.IsHidden = !e.Value.BoolValue;
        }

        /// <summary>
        /// Whether this control will have effects applied to is on rendering.
        /// </summary>
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
            _pitch.SetValue(new BindingValue(0d), true);
            _roll.SetValue(new BindingValue(0d), true);
            _rotationValue.SetValue(new BindingValue("0;0;0"), true);
            _slipBall.SetValue(new BindingValue(_slipBallCalibration.Interpolate(0d)), true);
            _turn.SetValue(new BindingValue(_turnCalibration.Interpolate(0d)), true);
            _gsHorizontal.SetValue(new BindingValue(_ilsHCalibration.Interpolate(-1d)), true);
            _gsVertical.SetValue(new BindingValue(_ilsVCalibration.Interpolate(-1d)), true);

            _attFlag.SetValue(new BindingValue(0d), true);
            _cmdFlag.SetValue(new BindingValue(0d), true);
            _gsFlag.SetValue(new BindingValue(0d), true);
            _navFlag.SetValue(new BindingValue(0d), true);
        }
    }
}
