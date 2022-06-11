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
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AV8B.SADI", "Backup ADI", "AV-8B Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class BackupADI : BaseGauge
    {
        private HeliosValue _pitch;
        private HeliosValue _roll;
        private HeliosValue _pitchAdjustment;
        private HeliosValue _warningFlag;

        private CalibrationPointCollectionDouble _pitchCalibration;
        private CalibrationPointCollectionDouble _pitchAdjustCalibaration;

        private GaugeNeedle _ball;
        private GaugeNeedle _wings;
        private GaugeNeedle _warningFlagNeedle;
        private GaugeNeedle _bankNeedle;

        public BackupADI()
            : base("Flight Instruments", new Size(350, 350))
        {
            Point center = new Point(174d, 163d);

            _pitchCalibration = new CalibrationPointCollectionDouble(-360d, -1066d, 360d, 1066d);
            _ball = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_backup_ball.xaml", center, new Size(225d, 1350d), new Point(112.5d, 677d));
            _ball.Clip = new EllipseGeometry(center, 113d, 113d);
            Components.Add(_ball);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_backup_inner_ring.xaml", new Rect(0d, 0d, 350d, 350d)));

            _pitchAdjustCalibaration = new CalibrationPointCollectionDouble(-1d, -30d, 1d, 30d);
            _wings = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_backup_wings.xaml", center, new Size(188d, 37d), new Point(94d, 3d));
            Components.Add(_wings);

            _bankNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_bank_pointer.xaml", center, new Size(11d, 221d), new Point(5.5d, 110.5d));
            Components.Add(_bankNeedle);

            _warningFlagNeedle = new GaugeNeedle("{Helios}/Gauges/A-10/ADI/adi_backup_warning_flag.xaml", new Point(29d, 226d), new Size(31d, 127d), new Point(0d, 127d));
            Components.Add(_warningFlagNeedle);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_backup_outer_ring.xaml", new Rect(0d, 0d, 350d, 350d)));


            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/ADI/adi_bezel.png", new Rect(0d, 0d, 350d, 350d)));

            _pitch = new HeliosValue(this, new BindingValue(0d), "Flight Instruments", "Pitch", "Current ptich of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _roll = new HeliosValue(this, new BindingValue(0d), "Flight Instruments", "Bank", "Current bank of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _roll.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_roll);

            _pitchAdjustment = new HeliosValue(this, new BindingValue(0d), "Flight Instruments", "Pitch adjustment offset", "Location of pitch reference wings.", "(0 to 1) 0 full up and 1 is full down.", BindingValueUnits.Numeric);
            _pitchAdjustment.Execute += new HeliosActionHandler(PitchAdjust_Execute);
            Actions.Add(_pitchAdjustment);

            _warningFlag = new HeliosValue(this, new BindingValue(false), "Flight Instruments", "ADI Warning Flag", "Indicates whether the warning flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _warningFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_warningFlag);
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.VerticalOffset = _pitchCalibration.Interpolate(e.Value.DoubleValue);
        }

        void PitchAdjust_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchAdjustment.SetValue(e.Value, e.BypassCascadingTriggers);
            _wings.VerticalOffset = -_pitchAdjustCalibaration.Interpolate(e.Value.DoubleValue);
        }

        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _roll.SetValue(e.Value, e.BypassCascadingTriggers);
            _ball.Rotation = -e.Value.DoubleValue;
            _bankNeedle.Rotation = e.Value.DoubleValue;
        }

        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _warningFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _warningFlagNeedle.Rotation = e.Value.BoolValue ? 0 : 20;
        }
    }

}
