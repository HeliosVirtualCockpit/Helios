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

namespace GadrocsWorkshop.Helios.Gauges.C130J.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.C130J;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Xml.Linq;

    [HeliosControl("Helios.C130J.ADI", "ADI", "C-130J Hercules", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class ADI : CompositeBaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _rotationValue;
        private HeliosValue _wingsValue;

        private HeliosValue _offFlag;

        private GaugeImage _offFlagImage;

        private GaugeBall _ball;
        private GaugeNeedle _rollNeedle;
        private GaugeNeedle _wingsNeedle;

        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _wingsCalibration;

        private bool _suppressScale = false;

        public ADI()
            : base("ADI", new Size(350, 350))
        {
            SupportedInterfaces = new[] { typeof(C130JInterface) };

            _ball = new GaugeBall("{C-130J}/Gauges/ADI/Herc-ADI-Ball-L.xaml", new Point(25d, 25d), new Size(300d, 300d), 0d, 0d, -90d, 50d);
            Components.Add(_ball);
            _ball.Y = 0.0001d;
            _ball.Z = 0.0001d;
            _ball.LightingBrightness = 1.0d;

            Components.Add(new GaugeImage("{helios}/Gauges/Common/Circular-Shading.xaml", new Rect(65d, 65d, 220d, 220d)));
            Components.Add(new GaugeImage("{C-130J}/Gauges/ADI/Herc-Bezel.xaml", new Rect(0d, 0d, 350d, 350d)));

            _rollNeedle = new GaugeNeedle("{C-130J}/Gauges/ADI/Herc-Roll-Marker.xaml", new Point(175d, 175d), new Size(230d, 230d), new Point(115d, 115d));
            Components.Add(_rollNeedle);

            _wingsNeedle = new GaugeNeedle("{C-130J}/Gauges/ADI/Herc-Horizon.xaml", new Point(175d, 175d), new Size(225.550d, 30.747d), new Point(147.910d, 2.833d));
            Components.Add(_wingsNeedle);

            _offFlagImage = new GaugeImage("{C-130J}/Gauges/ADI/Herc-Flag.xaml", new Rect(261.9067d, 57.5725d, 41.672d, 125.206d), 1, 0);
            _offFlagImage.IsHidden = true;
            Components.Add(_offFlagImage);

            Components.Add(new GaugeImage("{C-130J}/Gauges/ADI/Herc-Mask.xaml", new Rect(0d, 0d, 350d, 350d)));


            foreach (GaugeComponent gc in Components)
            {
                gc.EffectsExclusion = this.EffectsExclusion;
            }

            _pitch = new HeliosValue(this, BindingValue.Empty, "ADI", "pitch", "Current pitch of the aircraft.", "-90 to +90 degrees", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _roll = new HeliosValue(this, BindingValue.Empty, "ADI", "roll", "Current roll of the aircraft.", "-180 to +180 degrees", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Roll_Execute);
            Actions.Add(_roll);

            _rotationValue = new HeliosValue(this, BindingValue.Empty, "ADI", "ball rotation", "X/Y/Z angle changes for the ADI ball.", "Text containing three numbers x;y;z", BindingValueUnits.Text);
            _rotationValue.Execute += new HeliosActionHandler(Rotation_Execute);
            Actions.Add(_rotationValue);

            _wingsValue = new HeliosValue(this, BindingValue.Empty, "ADI", "Wing position", "Position of the wing indicator.", "-10 to +15 degrees", BindingValueUnits.Degrees);
            _wingsValue.Execute += new HeliosActionHandler(Wings_Execute);
            Actions.Add(_wingsValue);


            _offFlag = new HeliosValue(this, new BindingValue(false), "ADI", "off flag", "Indicates whether the off flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _pitchCalibration = new CalibrationPointCollectionDouble(-125d, -180d, 125d, 180d){
                new CalibrationPointDouble(0d,0d) };
            _wingsCalibration = new CalibrationPointCollectionDouble(-10d, 40d, 15d, -60d){
                new CalibrationPointDouble(0d,0d)
            };
            CreateInputBindings();
        }
        void CreateInputBindings()
        {

            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { "Instruments.ADI Horizon.changed", "ADI.set.Wing position" },
                { "Instruments.ADI Off Flag.changed", "ADI.set.off flag" },
                { "Instruments.ADI Pitch.changed", "ADI.set.pitch" },
                { "Instruments.ADI Roll.changed", "ADI.set.roll" }
            };

            foreach (string t in bindings.Keys)
            {
                AddDefaultInputBinding(
                    childName: "",
                    interfaceTriggerName: t,
                    deviceActionName: bindings[t]
                    );
            }
        }
        void Wings_Execute(object action, HeliosActionEventArgs e)
        {
            _wingsValue.SetValue(e.Value, e.BypassCascadingTriggers);
            _wingsNeedle.VerticalOffset = _wingsCalibration.Interpolate(e.Value.DoubleValue);
        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagImage.IsHidden = e.Value.BoolValue;
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.X = _pitchCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Roll_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.Z = e.Value.DoubleValue;
            _rollNeedle.Rotation = -e.Value.DoubleValue;
        }
        void Rotation_Execute(object action, HeliosActionEventArgs e)
        {
            _rotationValue.SetValue(e.Value, e.BypassCascadingTriggers);
            string[] parts;
            parts = Tokenizer.TokenizeAtLeast(e.Value.StringValue, 3, ';');
            double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double x);
            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double y);
            double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double z);
            _ball.Rotation3D = new Point3D(_pitchCalibration.Interpolate(x), y, -z);
            _rollNeedle.Rotation = z;
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
            _wingsValue.SetValue(new BindingValue(0d), true);
            _offFlag.SetValue(new BindingValue(true), true);
        }
    }
}
