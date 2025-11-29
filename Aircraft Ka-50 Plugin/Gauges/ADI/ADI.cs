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

namespace GadrocsWorkshop.Helios.Gauges.KA_50.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.KA50.ADI", "ADI", "Ka-50 Black Shark", typeof(GaugeRenderer))]
    public class ADI : BaseGauge
    {
        GaugeNeedle _pitchBallNeedle;
        GaugeNeedle _bankSteeringBarNeedle;
        GaugeNeedle _pitchSteeringBarNeedle;
        GaugeNeedle _aircraftSymbolNeedle;
        GaugeNeedle _altDeviationNeedle;
        GaugeNeedle _lateralDeviationNeedle;
        GaugeNeedle _airspeedDeviationNeedle;
        GaugeNeedle _slipBallNeedle;

        GaugeImage _fiFlagImage;
        GaugeImage _dirFlagImage;

        HeliosValue _pitch;
        HeliosValue _bank;
        HeliosValue _bankSteeringOffset;
        HeliosValue _pitchSteeringOffet;
        HeliosValue _airspeedDeviation;
        HeliosValue _lateralDeviation;
        HeliosValue _altitudeDeviation;
        HeliosValue _fiFlag;
        HeliosValue _dirFlag;
        HeliosValue _sideSlip;

        CalibrationPointCollectionDouble _pitchCalibration;
        CalibrationPointCollectionDouble _bankSteeringCalibration;
        CalibrationPointCollectionDouble _deviationCalibration;
        CalibrationPointCollectionDouble _pitchSteeringCalibration;
        CalibrationPointCollectionDouble _slipCalibration;

        public ADI()
            : base("ADI", new Size(400, 400))
        {
            Point center = new Point(200, 176);

            _pitchCalibration = new CalibrationPointCollectionDouble(-180d, -1083d, 180d, 1083d);
            _bankSteeringCalibration = new CalibrationPointCollectionDouble(-1d, -45d, 1d, 45d);
            _pitchSteeringCalibration = new CalibrationPointCollectionDouble(-1d, -100d, 1d, 100d);
            _deviationCalibration = new CalibrationPointCollectionDouble(-1d, -55d, 1d, 55d);
            _slipCalibration = new CalibrationPointCollectionDouble(-1d, -40d, 1d, 40d);

            _pitchBallNeedle = new GaugeNeedle("{Ka-50}/Gauges/ADI/adi_pitch_ball.xaml", center, new Size(264, 2165), new Point(132, 1082.5));
            _pitchBallNeedle.Clip = new EllipseGeometry(center, 132, 132);
            Components.Add(_pitchBallNeedle);

            _bankSteeringBarNeedle = new GaugeNeedle("{Ka-50}/Gauges/ADI/adi_bank_steering_bar.xaml", new Point(200, 312), new Size(8, 206), new Point(4, 202));
            Components.Add(_bankSteeringBarNeedle);

            _pitchSteeringBarNeedle = new GaugeNeedle("{Ka-50}/Gauges/ADI/adi_pitch_steering_bar.xaml", center, new Size(271, 8), new Point(135.5, 4));
            Components.Add(_pitchSteeringBarNeedle);

            _aircraftSymbolNeedle = new GaugeNeedle("{Ka-50}/Gauges/ADI/adi_aircraft_symbol.xaml", center, new Size(253, 37), new Point(126.5, 18.5));
            Components.Add(_aircraftSymbolNeedle);

            _fiFlagImage = new GaugeImage("{Ka-50}/Gauges/ADI/adi_fi_flag.xaml", new Rect(85, 225, 62, 44));
            Components.Add(_fiFlagImage);

            _dirFlagImage = new GaugeImage("{Ka-50}/Gauges/ADI/adi_dir_flag.xaml", new Rect(92, 74, 62, 32));
            Components.Add(_dirFlagImage);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/ADI/adi_inner_bezel.xaml", new Rect(0, 0, 400, 400)));

            _altDeviationNeedle = new GaugeNeedle("{Ka-50}/Gauges/ADI/adi_alt_deviation_needle.xaml", new Point(371, 176), new Size(28, 12), new Point(14, 6));
            Components.Add(_altDeviationNeedle);

            _lateralDeviationNeedle = new GaugeNeedle("{Ka-50}/Gauges/ADI/adi_lateral_deviation_needle.xaml", new Point(200, 30), new Size(5, 29), new Point(2.5, 20));
            Components.Add(_lateralDeviationNeedle);

            _airspeedDeviationNeedle = new GaugeNeedle("{Ka-50}/Gauges/ADI/adi_airspeed_deviation_needle.xaml", new Point(27, 176), new Size(29, 5), new Point(16, 3.5));
            Components.Add(_airspeedDeviationNeedle);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/ADI/adi_bubble_bezel.xaml", new Rect(0, 0, 400, 400)));

            _slipBallNeedle = new GaugeNeedle("{Ka-50}/Gauges/ADI/adi_slip_ball.xaml", new Point(200, 363.5), new Size(23, 23), new Point(11.5, 11.5));
            Components.Add(_slipBallNeedle);

            Components.Add(new GaugeImage("{Ka-50}/Gauges/ADI/adi_outer_bezel.xaml", new Rect(0, 0, 400, 400)));

            _fiFlag = new HeliosValue(this, new BindingValue(false), "", "Steering Flag", "Indicates whether the steering (DIR) flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _fiFlag.Execute += new HeliosActionHandler(FiFlag_Execute);
            Actions.Add(_fiFlag);

            _dirFlag = new HeliosValue(this, new BindingValue(false), "", "Malfunction Flag", "Indicates whether the malfunction (FI) flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _dirFlag.Execute += new HeliosActionHandler(DirFlag_Execute);
            Actions.Add(_dirFlag);

            _pitch = new HeliosValue(this, new BindingValue(0d), "", "Pitch", "Current ptich of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _pitch.Execute += new HeliosActionHandler(Pitch_Execute);
            Actions.Add(_pitch);

            _bank = new HeliosValue(this, new BindingValue(0d), "", "Bank", "Current bank of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
            _bank.Execute += new HeliosActionHandler(Bank_Execute);
            Actions.Add(_bank);

            _bankSteeringOffset = new HeliosValue(this, new BindingValue(1d), "", "Bank steering bar offset", "Location of bank steering bar.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _bankSteeringOffset.Execute += new HeliosActionHandler(BankSteering_Execute);
            Actions.Add(_bankSteeringOffset);

            _pitchSteeringOffet = new HeliosValue(this, new BindingValue(1d), "", "Pitch steering bar offset", "Location of bank steering bar.", "(-1 to 1) -1 full up and 1 is full down.", BindingValueUnits.Numeric);
            _pitchSteeringOffet.Execute += new HeliosActionHandler(PitchSteering_Execute);
            Actions.Add(_pitchSteeringOffet);

            _airspeedDeviation = new HeliosValue(this, new BindingValue(1d), "", "Airspeed Deviation", "Offset of airspeed deviation needle.", "(-1 to 1) -1 full up and 1 is full down.", BindingValueUnits.Numeric);
            _airspeedDeviation.Execute += new HeliosActionHandler(AirspeedDeviation_Execute);
            Actions.Add(_airspeedDeviation);

            _altitudeDeviation = new HeliosValue(this, new BindingValue(1d), "", "Altitude Deviation", "Offset of airspeed deviation needle.", "(-1 to 1) -1 full up and 1 is full down.", BindingValueUnits.Numeric);
            _altitudeDeviation.Execute += new HeliosActionHandler(AltitudeDeviation_Execute);
            Actions.Add(_altitudeDeviation);

            _lateralDeviation = new HeliosValue(this, new BindingValue(1d), "", "Lateral Deviation", "Offset of lateral deviation needle.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _lateralDeviation.Execute += new HeliosActionHandler(LateralDeviation_Execute);
            Actions.Add(_lateralDeviation);

            _sideSlip = new HeliosValue(this, new BindingValue(1d), "", "Side Slip", "Offset of side slip ball.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _sideSlip.Execute += new HeliosActionHandler(SideSlip_Execute);
            Actions.Add(_sideSlip);
        }

        void SideSlip_Execute(object action, HeliosActionEventArgs e)
        {
            _sideSlip.SetValue(e.Value, e.BypassCascadingTriggers);
            _slipBallNeedle.HorizontalOffset = _slipCalibration.Interpolate(e.Value.DoubleValue);
        }

        void LateralDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _lateralDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _lateralDeviationNeedle.HorizontalOffset = _deviationCalibration.Interpolate(e.Value.DoubleValue);
        }

        void AltitudeDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _altitudeDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _altDeviationNeedle.VerticalOffset = _deviationCalibration.Interpolate(e.Value.DoubleValue);
        }

        void AirspeedDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _airspeedDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _airspeedDeviationNeedle.VerticalOffset = _deviationCalibration.Interpolate(e.Value.DoubleValue);
        }

        void PitchSteering_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchSteeringOffet.SetValue(e.Value, e.BypassCascadingTriggers);
            _pitchSteeringBarNeedle.VerticalOffset = _pitchSteeringCalibration.Interpolate(e.Value.DoubleValue);
        }

        void BankSteering_Execute(object action, HeliosActionEventArgs e)
        {
            _bankSteeringOffset.SetValue(e.Value, e.BypassCascadingTriggers);
            _bankSteeringBarNeedle.Rotation = _bankSteeringCalibration.Interpolate(e.Value.DoubleValue);
        }

        void FiFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _fiFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _fiFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void DirFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _dirFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _dirFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void Pitch_Execute(object action, HeliosActionEventArgs e)
        {
            _pitch.SetValue(e.Value, e.BypassCascadingTriggers);
            _pitchBallNeedle.VerticalOffset = _pitchCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Bank_Execute(object action, HeliosActionEventArgs e)
        {
            _bank.SetValue(e.Value, e.BypassCascadingTriggers);
            _aircraftSymbolNeedle.Rotation = e.Value.DoubleValue;
        }
    }
}
