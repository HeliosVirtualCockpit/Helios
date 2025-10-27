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

namespace GadrocsWorkshop.Helios.Gauges.Mi_8.ADI
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using System;
	using System.Windows;
	using System.Windows.Media;

	[HeliosControl("Helios.Mi-8.ADI", "ADI", "Mi-8", typeof(GaugeRenderer))]
	public class ADI : BaseGauge
	{
		GaugeNeedle _pitchBallNeedle;
		GaugeNeedle _aircraftSymbolNeedle;
		GaugeNeedle _altDeviationNeedle;
		GaugeNeedle _lateralDeviationNeedle;
		GaugeNeedle _airspeedDeviationNeedle;
		GaugeNeedle _slipBallNeedle;

		GaugeImage _dirFlagImage;

		HeliosValue _pitch;
		HeliosValue _bank;
		HeliosValue _airspeedDeviation;
		HeliosValue _lateralDeviation;
		HeliosValue _altitudeDeviation;
		HeliosValue _dirFlag;
		HeliosValue _sideSlip;

		CalibrationPointCollectionDouble _pitchCalibration;
		CalibrationPointCollectionDouble _deviationCalibration;
		CalibrationPointCollectionDouble _slipCalibration;

		public ADI()
			: base("ADI", new Size(400, 400))
		{
			Point center = new Point(200, 176);

			_pitchCalibration = new CalibrationPointCollectionDouble(-180d, -1083d, 180d, 1083d);

			_deviationCalibration = new CalibrationPointCollectionDouble(-1d, -55d, 1d, 55d);
			_slipCalibration = new CalibrationPointCollectionDouble(-1d, -40d, 1d, 40d);

			_pitchBallNeedle = new GaugeNeedle("{Mi-8}/Gauges/ADI/adi_pitch_ball.xaml", center, new Size(264, 2165), new Point(132, 1082.5));
			_pitchBallNeedle.Clip = new EllipseGeometry(center, 132, 132);
			Components.Add(_pitchBallNeedle);



			Components.Add(new GaugeImage("{Mi-8}/Gauges/ADI/adi_bank_steering_bar.xaml", new Rect(195, 30, 8, 300)));

			_aircraftSymbolNeedle = new GaugeNeedle("{Mi-8}/Gauges/ADI/adi_aircraft_symbol.xaml", center, new Size(253, 50), new Point(126.5, 18.5));
			Components.Add(_aircraftSymbolNeedle);



			_dirFlagImage = new GaugeImage("{Mi-8}/Gauges/ADI/adi_dir_flag.xaml", new Rect(92, 74, 85, 30));
			Components.Add(_dirFlagImage);

			Components.Add(new GaugeImage("{Mi-8}/Gauges/ADI/adi_inner_bezel.xaml", new Rect(0, 0, 400, 400)));





			_altDeviationNeedle = new GaugeNeedle("{Mi-8}/Gauges/ADI/adi_alt_deviation_needle.xaml", new Point(371, 176), new Size(28, 12), new Point(14, 6));
			Components.Add(_altDeviationNeedle);

			_lateralDeviationNeedle = new GaugeNeedle("{Mi-8}/Gauges/ADI/adi_lateral_deviation_needle.xaml", new Point(200, 30), new Size(5, 29), new Point(2.5, 20));
			Components.Add(_lateralDeviationNeedle);

			_airspeedDeviationNeedle = new GaugeNeedle("{Mi-8}/Gauges/ADI/adi_airspeed_deviation_needle.xaml", new Point(27, 176), new Size(29, 5), new Point(16, 3.5));
			Components.Add(_airspeedDeviationNeedle);

			Components.Add(new GaugeImage("{Mi-8}/Gauges/ADI/adi_bubble_bezel.xaml", new Rect(0, 0, 400, 400)));

			_slipBallNeedle = new GaugeNeedle("{Mi-8}/Gauges/ADI/adi_slip_ball.xaml", new Point(200, 363.5), new Size(23, 23), new Point(11.5, 11.5));
			Components.Add(_slipBallNeedle);

			Components.Add(new GaugeImage("{Mi-8}/Gauges/ADI/adi_outer_bezel.xaml", new Rect(0, 0, 400, 400)));



			_dirFlag = new HeliosValue(this, new BindingValue(false), "", "Malfunction Flag", "Indicates whether the malfunction flag is displayed.", "True if displayed.", BindingValueUnits.Boolean);
			_dirFlag.Execute += new HeliosActionHandler(DirFlag_Execute);
			Actions.Add(_dirFlag);

			_pitch = new HeliosValue(this, new BindingValue(0d), "", "Pitch", "Current ptich of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
			_pitch.Execute += new HeliosActionHandler(Pitch_Execute);
			Actions.Add(_pitch);

			_bank = new HeliosValue(this, new BindingValue(0d), "", "Bank", "Current bank of the aircraft.", "(0 - 360)", BindingValueUnits.Degrees);
			_bank.Execute += new HeliosActionHandler(Bank_Execute);
			Actions.Add(_bank);

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
