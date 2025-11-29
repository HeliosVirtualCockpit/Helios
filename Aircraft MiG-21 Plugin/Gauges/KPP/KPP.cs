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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.MiG21.KPP", "KPP", "MiG-21", typeof(GaugeRenderer))]
    public class KPP : BaseGauge
    {
        GaugeNeedle _pitchBallNeedle;
        GaugeNeedle _bankSteeringBarNeedle;
        GaugeNeedle _pitchSteeringBarNeedle;
        GaugeNeedle _aircraftSymbolNeedle;
        GaugeNeedle _courseDeviationNeedle;
        GaugeNeedle _glideslopeDeviationNeedle;
        GaugeNeedle _slipBallNeedle;

        GaugeImage _tFlagImage;
        GaugeImage _kFlagImage;

        HeliosValue _pitch;
        HeliosValue _bank;

        HeliosValue _bankSteeringOffset;
        HeliosValue _pitchSteeringOffet;

        HeliosValue _glideslopeDeviation;
        HeliosValue _courseDeviation;

        HeliosValue _tFlag;
        HeliosValue _kFlag;

        HeliosValue _sideSlip;

        CalibrationPointCollectionDouble _pitchCalibration;
        CalibrationPointCollectionDouble _bankSteeringCalibration;
        CalibrationPointCollectionDouble _courseDeviationCalibration;
        CalibrationPointCollectionDouble _glideslopeDeviationCalibration;
        CalibrationPointCollectionDouble _pitchSteeringCalibration;
        CalibrationPointCollectionDouble _slipCalibration;

        public KPP()
            : base("KPP", new Size(400, 420))
        {
            Point center = new Point(200, 196);

            _bankSteeringCalibration = new CalibrationPointCollectionDouble(-1d, -45d, 1d, 45d);

            _pitchCalibration = new CalibrationPointCollectionDouble(180d, -953d, -180d, 953d);

            _pitchSteeringCalibration = new CalibrationPointCollectionDouble(-1d, -100d, 1d, 100d);

            _courseDeviationCalibration = new CalibrationPointCollectionDouble(-1d, -55d, 1d, 55d);
            _glideslopeDeviationCalibration = new CalibrationPointCollectionDouble(1d, -55d, -1d, 55d);

            _slipCalibration = new CalibrationPointCollectionDouble(-1d, 74d, 1d, -74d);
            _slipCalibration.Add(new CalibrationPointDouble(0d, 0d));

            _pitchBallNeedle = new GaugeNeedle("{MiG-21}/Gauges/KPP/adi_pitch_ball.xaml", center, new Size(264, 2165), new Point(132, 1082.5));

            _pitchBallNeedle.Clip = new EllipseGeometry(center, 142, 142);
            Components.Add(_pitchBallNeedle);

            _bankSteeringBarNeedle = new GaugeNeedle("{MiG-21}/Gauges/KPP/adi_bank_steering_bar.xaml", new Point(200, 332), new Size(6, 186), new Point(4, 182));
            Components.Add(_bankSteeringBarNeedle);

            _pitchSteeringBarNeedle = new GaugeNeedle("{MiG-21}/Gauges/KPP/adi_pitch_steering_bar.xaml", center, new Size(271, 6), new Point(135.5, 3));
            Components.Add(_pitchSteeringBarNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/KPP/adi_inner_bezel.xaml", new Rect(0, 0, 400, 420)));

            _aircraftSymbolNeedle = new GaugeNeedle("{MiG-21}/Gauges/KPP/adi_aircraft_symbol.xaml", center, new Size(253, 37), new Point(126.5, 18.5));
            Components.Add(_aircraftSymbolNeedle);

            _courseDeviationNeedle = new GaugeNeedle("{MiG-21}/Gauges/KPP/adi_lateral_deviation_needle.xaml", new Point(199, 31), new Size(5, 23), new Point(2.5, 12.5));
            Components.Add(_courseDeviationNeedle);

            _glideslopeDeviationNeedle = new GaugeNeedle("{MiG-21}/Gauges/KPP/adi_airspeed_deviation_needle.xaml", new Point(29, 195), new Size(23, 5), new Point(12.5, 2.5));
            Components.Add(_glideslopeDeviationNeedle);

            _tFlagImage = new GaugeImage("{MiG-21}/Gauges/KPP/adi_t_flag.xaml", new Rect(54, 48, 54, 54));
            Components.Add(_tFlagImage);

            _kFlagImage = new GaugeImage("{MiG-21}/Gauges/KPP/adi_k_flag.xaml", new Rect(286, 47, 54, 54));
            Components.Add(_kFlagImage);

            _slipBallNeedle = new GaugeNeedle("{MiG-21}/Gauges/KPP/adi_slip_ball.xaml", new Point(199.5, 405), new Size(23, 23), new Point(11.5, 11.5));
            Components.Add(_slipBallNeedle);

            _tFlag = new HeliosValue(this, new BindingValue(false), "", "T Flag", "Indicates whether the T flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _tFlag.Execute += new HeliosActionHandler(TFlag_Execute);
            Actions.Add(_tFlag);

            _kFlag = new HeliosValue(this, new BindingValue(false), "", "K Flag", "Indicates whether the K flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _kFlag.Execute += new HeliosActionHandler(KFlag_Execute);
            Actions.Add(_kFlag);

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

            _glideslopeDeviation = new HeliosValue(this, new BindingValue(1d), "", "Aux Glideslop Deviation use NPP", "Aux glideslope deviation needle.", "(-1 to 1) -1 full up and 1 is full down.", BindingValueUnits.Numeric);
            _glideslopeDeviation.Execute += new HeliosActionHandler(GlideslopeDeviation_Execute);
            Actions.Add(_glideslopeDeviation);

            _courseDeviation = new HeliosValue(this, new BindingValue(1d), "", "Aux Course Deviation use NPP", "Aux course deviation needle.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _courseDeviation.Execute += new HeliosActionHandler(CourseDeviation_Execute);
            Actions.Add(_courseDeviation);

            _sideSlip = new HeliosValue(this, new BindingValue(1d), "", "Side Slip use DA200 slip", "Offset of side slip ball.", "(-1 to 1) -1 full left and 1 is full right.", BindingValueUnits.Numeric);
            _sideSlip.Execute += new HeliosActionHandler(SideSlip_Execute);
            Actions.Add(_sideSlip);
        }

        void SideSlip_Execute(object action, HeliosActionEventArgs e)
        {
            _sideSlip.SetValue(e.Value, e.BypassCascadingTriggers);
            _slipBallNeedle.HorizontalOffset = _slipCalibration.Interpolate(e.Value.DoubleValue);
        }

        void CourseDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _courseDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _courseDeviationNeedle.HorizontalOffset = _courseDeviationCalibration.Interpolate(e.Value.DoubleValue);
        }

        void GlideslopeDeviation_Execute(object action, HeliosActionEventArgs e)
        {
            _glideslopeDeviation.SetValue(e.Value, e.BypassCascadingTriggers);
            _glideslopeDeviationNeedle.VerticalOffset = _glideslopeDeviationCalibration.Interpolate(e.Value.DoubleValue);
        }

        void PitchSteering_Execute(object action, HeliosActionEventArgs e)
        {
            _pitchSteeringOffet.SetValue(e.Value, e.BypassCascadingTriggers);
            _pitchSteeringBarNeedle.VerticalOffset = _pitchSteeringCalibration.Interpolate(e.Value.DoubleValue);
        }

        void BankSteering_Execute(object action, HeliosActionEventArgs e)
        {
            _bankSteeringOffset.SetValue(e.Value, e.BypassCascadingTriggers);
            _bankSteeringBarNeedle.Rotation = _bankSteeringCalibration.Interpolate(e.Value.DoubleValue * 3);
        }

        void TFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _tFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _tFlagImage.IsHidden = !e.Value.BoolValue;
        }

        void KFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _kFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _kFlagImage.IsHidden = !e.Value.BoolValue;
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
