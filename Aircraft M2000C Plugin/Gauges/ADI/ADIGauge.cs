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
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.M2000C.ADI.Gauge", "ADI Gauge", "M-2000C", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class ADIGauge : BaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _yaw;
        private HeliosValue _pitchAdjustment;
        private HeliosValue _offFlag;

        private GaugeNeedle _offFlagNeedle;
        private GaugeBall _ball;
        private GaugeNeedle _bankNeedle;
        private GaugeNeedle _wingsNeedle;

        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _pitchAdjustCalibaration;

        public ADIGauge(string name, Size size, string device)
            : base(name, size)
        {
            Point center = new Point(200d, 200d);

            _pitchCalibration = new CalibrationPointCollectionDouble(-90d, -461d, 90d, 461d);
            _ball = new GaugeBall("{M2000C}/Gauges/ADI/ADI_Ball.xaml", new Point(-50,-50), new Size(500d, 500d), 180d, 0d, 0d);
            Components.Add(_ball);

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(-1.0d, -60d, 1.0d, 60d);
            _wingsNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Wings.xaml", new Point(50d, 194d), new Size(300d, 55d), new Point(0d, 0d));
            Components.Add(_wingsNeedle);

            _bankNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Pointer.xaml", center, new Size(10d, 39d), new Point(5d, -111d));
            Components.Add(_bankNeedle);

            Components.Add(new GaugeImage("{M2000C}/Gauges/ADI/ADI_Bezel.xaml", new Rect(0d, 0d, 400d, 400d)));

            _offFlagNeedle = new GaugeNeedle("{M2000C}/Gauges/ADI/ADI_Off_Flag.xaml", new Point(-14d, 315d), new Size(66d, 265d), new Point(5d, 260d), 0d);
            Components.Add(_offFlagNeedle);

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

            _yaw = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "ADI Heading", "Current heading of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _yaw.Execute += new HeliosActionHandler(Yaw_Execute);
            Actions.Add(_yaw);

        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = e.Value.BoolValue ? 15d : 0d;
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Pitch = -e.Value.DoubleValue;
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
        void Yaw_Execute(object action, HeliosActionEventArgs e)
        {
            _yaw.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Yaw = e.Value.DoubleValue;
        }
    }
}
