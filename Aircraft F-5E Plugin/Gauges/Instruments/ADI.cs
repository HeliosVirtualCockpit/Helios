﻿//  Copyright 2014 Craig Courtney
//  Copyright 2024 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F5E.Instruments.ADI", "ADI", "F-5E", typeof(GaugeRenderer),HeliosControlFlags.None)]
    public class ADIGauge : BaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _pitchAdjustment;
        private HeliosValue _offFlag;

        private GaugeNeedle _offFlagNeedle;
        private GaugeNeedle _ball;
        private GaugeNeedle _bankNeedle;
        private GaugeNeedle _wingsNeedle;

        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _pitchAdjustCalibaration;

        public ADIGauge() : this("ADI",new Size(300,300),"Instruments") { }
        public ADIGauge(string name, Size size, string device)
            : base(name, size)
        {
            Point center = new Point(size.Width / 2d, size.Height / 2d);

            _pitchCalibration = new CalibrationPointCollectionDouble(-90d, -461d, 90d, 461d);
            _ball = new GaugeNeedle("{F-5E}/Gauges/Instruments/F-5E_ADI_Tape.xaml", center, new Size(235d, 944d), new Point(117d, 944d / 2d));
            _ball.Clip = new EllipseGeometry(center, 117d, 117d);
            Components.Add(_ball);

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(-1.0d, -30d, 1.0d, 30d);


            _bankNeedle = new GaugeNeedle("{F-5E}/Gauges/Instruments/F-5E_ADI_Bank_Pointer.xaml", center, new Size(16d, 105d), new Point(8d, 0d));
            Components.Add(_bankNeedle);

            Components.Add(new GaugeImage("{F-5E}/Gauges/Instruments/F-5E_ADI_Inner_Ring.xaml", new Rect(25d, 25d, 250d, 250d)));

            Components.Add(new GaugeImage("{F-5E}/Gauges/Instruments/F-5E_ADI_Wings.xaml", new Rect(27d, 150d, 246d, 34d)));

            Components.Add(new GaugeImage("{F-5E}/Gauges/Instruments/F-5E_ADI_Outer_Ring.xaml", new Rect(0d, 0d, 300d, 300d)));

            _offFlagNeedle = new GaugeNeedle("{F-5E}/Gauges/Instruments/F-5E_ADI_Off_Flag.xaml", new Point(0d, 150d), new Size(150d, 93d), new Point(0d, 0d), 0d);
            Components.Add(_offFlagNeedle);



            _offFlag = new HeliosValue(this, new BindingValue(false), $"{device}_{name}", "ADI Off Flag", "Indicates the position of the off flag.", "1.0 if displayed.", BindingValueUnits.Numeric);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "ADI Aircraft Pitch Angle", "Current pitch of the aircraft in degrees.", "(-90 to +90)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _pitchAdjustment = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "ADI Pitch adjustment offset", "Location of pitch reference wings.", "(-1 to 1) 1 full up and -1 is full down.", BindingValueUnits.Numeric);
            _pitchAdjustment.Execute += new HeliosActionHandler(PitchAdjust_Execute);
            Actions.Add(_pitchAdjustment);

            _roll = new HeliosValue(this, new BindingValue(0d), $"{device}_{name}", "ADI Aircraft Bank Angle", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = (1-e.Value.DoubleValue) * 15d;
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
            _ball.Rotation = e.Value.DoubleValue;
            _bankNeedle.Rotation = e.Value.DoubleValue;
        }
    }
}
