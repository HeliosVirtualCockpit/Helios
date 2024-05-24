//  Copyright 2014 Craig Courtney
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

    [HeliosControl("Helios.F5E.Instruments.ADI_Standby", "Standby ADI", "F-5E Tiger II", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class ADIStandbyGauge : BaseGauge
    {
        private readonly HeliosValue _pitch;
        private readonly HeliosValue _roll;
        private readonly HeliosValue _pitchAdjustment;
        private readonly HeliosValue _offFlag;

        private readonly GaugeNeedle _offFlagNeedle;
        private readonly GaugeNeedle _ball;
        private readonly GaugeNeedle _bankNeedle;
        private readonly GaugeNeedle  _wingsNeedle;

        private readonly CalibrationPointCollectionDouble _pitchCalibration;
        private readonly CalibrationPointCollectionDouble _pitchAdjustCalibaration;

        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public ADIStandbyGauge() : this("Standby ADI",new Size(387.357d, 382.353d),"Instruments") { }
        public ADIStandbyGauge(string name, Size size, string device)
            : base(name, size)
        {
            Point center = new Point(188.675d, 183.42d);
            double pitchMax = -235.17935d * 1.5d / 2d;
            _pitchCalibration = new CalibrationPointCollectionDouble(-90d, pitchMax, 90d, pitchMax * -1) {
                new CalibrationPointDouble(-180d, pitchMax),
                new CalibrationPointDouble(180d, pitchMax * -1)
            };

            _ball = new GaugeNeedle($"{_gaugeImagePath}F-5E_ADI_Standby_Tape.xaml", center, new Size(235d, 944d * 1.5d), new Point(117d, 944d * 1.5d / 2d));
            _ball.Clip = new EllipseGeometry(center, 117d, 117d);
            Components.Add(_ball);

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(-30d, -40d, 30d, 40d);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_ADI_Standby_Inner_Ring.xaml", new Rect(size.Width - 353.550d, size.Height - 353.803d, 353.550d, 353.803d)));

            _bankNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_ADI_Standby_Bank_Pointer.xaml", center, new Size(143.712d, 228.844d), new Point(143.712d / 2d, 228.844d / 2d));
            Components.Add(_bankNeedle);

            _wingsNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_ADI_Standby_Wings.xaml", center, new Size(242.196d, 26.186d), new Point(82.039d, 4.522d));
            Components.Add(_wingsNeedle);

            _offFlagNeedle = new GaugeNeedle($"{_gaugeImagePath}F-5E_ADI_Standby_Off_Flag.xaml", new Point(15.0977d, 111.8505d + 189.3085d), new Size(102.416d, 189.309d), new Point(0d, 189d), -15d);
            Components.Add(_offFlagNeedle);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_ADI_Standby_Outer_Ring.xaml", new Rect(0d, 0d, 387.357d, 382.353d)));

            _offFlag = new HeliosValue(this, new BindingValue(false), $"", "Standby ADI Off Flag", "Number representing the off flag position.", "(0 to 1)", BindingValueUnits.Numeric);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), $"", "Standby ADI Aircraft Pitch Angle", "Current pitch of the aircraft in degrees.", "(-90 to +90)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _pitchAdjustment = new HeliosValue(this, new BindingValue(0d), $"", "Standby ADI Pitch adjustment offset", "Location of pitch reference wings.", "(-30 to 30)", BindingValueUnits.Degrees);
            _pitchAdjustment.Execute += new HeliosActionHandler(PitchAdjust_Execute);
            Actions.Add(_pitchAdjustment);

            _roll = new HeliosValue(this, new BindingValue(0d), $"", "Standby ADI Aircraft Bank Angle", "Current bank of the aircraft in degrees.", "(-180 to +180)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _offFlagNeedle.Rotation = e.Value.DoubleValue * 15d;
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
