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

namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using NLog.LayoutRenderers.Wrappers;
    using System;
    using System.Security.Cryptography;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AV8B.SAI", "SAI Gauge", "AV-8B Harrier", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class SAI : BaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _pitchAdjustment;
        private HeliosValue _warningFlag;
        private HeliosValue _altLightValue;


        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _pitchAdjustCalibaration;

        private GaugeCylinder _cylinder;
        private GaugeNeedle _wings;
        private GaugeNeedle _warningFlagNeedle;
        private GaugeNeedle _bankNeedle;

        private bool _suppressScale = false;

        public SAI()
            : base("Flight Instruments", new Size(350, 350))
        {
            Point center = new Point(174d, 163d);

            _pitchCalibration = new CalibrationPointCollectionDouble(-360d, -1066d, 360d, 1066d);
            _cylinder = new GaugeCylinder("{AV-8B}/Gauges/ADI/ADI-Tape.xaml", new Point(46d, 33d), new Size(260, 260));
            _cylinder.Clip = new EllipseGeometry(center, 130d, 130d);
            Components.Add(_cylinder);

            Components.Add(new GaugeImage("{AV-8B}/Gauges/ADI/ADI-Inner-Ring.xaml", new Rect(center.X - (280d * 0.5d), center.Y - (280d * 0.5d), 280d, 280d)));

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(-1d, -30d, 1d, 30d);
            _wings = new GaugeNeedle("{AV-8B}/Gauges/ADI/ADI-Wings.xaml", center, new Size(188d, 37d), new Point(94d, 3d));
            Components.Add(_wings);

            _bankNeedle = new GaugeNeedle("{AV-8B}/Gauges/ADI/ADI-Roll-Pointer.xaml", center, new Size(222d, 222d), new Point(111d, 111d));
            Components.Add(_bankNeedle);

            Components.Add(new GaugeImage("{helios}/Gauges/Common/Circular-Shading.xaml", new Rect(center.X - (222d * 0.5d), center.Y - (222d * 0.5d), 222d, 222d)));

            _warningFlagNeedle = new GaugeNeedle("{AV-8B}/Gauges/ADI/ADI-Off-Flag.xaml", new Point(29d, 226d), new Size(31d, 127d), new Point(0d, 127d));
            Components.Add(_warningFlagNeedle);

            Components.Add(new GaugeImage("{AV-8B}/Gauges/ADI/ADI-Outer-Ring.xaml", new Rect(0d, 0d, 350d, 350d)));

            GaugeImage _reflection = new GaugeImage("{AV-8B}/Images/WQHD/Panel/crystal_reflection_round.png", new Rect(44d, 33d, 260d, 260d));
            _reflection.Opacity = 0.3;
            Components.Add(_reflection);

            Components.Add(new GaugeImage("{AV-8B}/Gauges/ADI/ADI-Bezel.png", new Rect(0d, 0d, 350d, 350d)));
            
            Components.Add(new GaugeImage("{AV-8B}/Gauges/ADI/ADI-Pitch-Adjust-Scale.xaml", new Rect(235d, 230d, 75d, 75d)));

            _pitch = new HeliosValue(this, new BindingValue(0d), "Flight Instruments", "SAI Pitch", "Current pitch of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _roll = new HeliosValue(this, new BindingValue(0d), "Flight Instruments", "SAI Bank", "Current bank of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

            _pitchAdjustment = new HeliosValue(this, new BindingValue(0d), "Flight Instruments", "SAI Pitch adjustment offset", "Location of pitch reference wings.", "(-1 to 1) 1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchAdjustment.Execute += new HeliosActionHandler(PitchAdjust_Execute);
            Actions.Add(_pitchAdjustment);

            _warningFlag = new HeliosValue(this, new BindingValue(false), "Flight Instruments", "SAI Warning Flag", "Indicates whether the warning flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _warningFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_warningFlag);

            _altLightValue = new HeliosValue(this, new BindingValue(false), "", "Alternate Lighting Source", "Boolean", "true if Alt Lighting is used", BindingValueUnits.Boolean);
            _altLightValue.Execute += new HeliosActionHandler(AltLightingUsed_Execute);
            Actions.Add(_altLightValue);
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _cylinder.Yaw = -e.Value.DoubleValue;
        }

        void PitchAdjust_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchAdjustment.SetValue(e.Value, e.BypassCascadingTriggers);
            _wings.VerticalOffset = e.Value.DoubleValue;
        }

        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _roll.SetValue(e.Value, e.BypassCascadingTriggers);
            _cylinder.Roll = -e.Value.DoubleValue;
            _bankNeedle.Rotation = e.Value.DoubleValue;
        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _warningFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _warningFlagNeedle.Rotation = e.Value.BoolValue ? 0 : 20;
        }
        void AltLightingUsed_Execute(object action, HeliosActionEventArgs e)
        {
            _cylinder.LightingAltEnabled = e.Value.BoolValue;
        }

        public override void ScaleChildren(double scaleX, double scaleY)
        {
            if (!_suppressScale)
            {
                _cylinder.ScaleChildren(scaleX, scaleY);
                _suppressScale = false;
            }
            base.ScaleChildren(scaleX, scaleY);
        }
        protected override void PostUpdateRectangle(Rect previous, Rect current)
        {
            _suppressScale = false;
            if (!previous.Equals(new Rect(0, 0, 0, 0)) && !(previous.Width == current.Width && previous.Height == current.Height))
            {
                _cylinder.ScaleChildren(current.Width / previous.Width, current.Height / previous.Height);
                _suppressScale = true;
            }
        }
        public override void Reset()
        {
            base.Reset();
            _cylinder.Reset();
            _pitch.SetValue(new BindingValue(0d), true);
            _roll.SetValue(new BindingValue(0d), true);
            _pitchAdjustment.SetValue(new BindingValue(0d), true);
            //_offFlag.SetValue(new BindingValue(false), true);
        }

    }

}