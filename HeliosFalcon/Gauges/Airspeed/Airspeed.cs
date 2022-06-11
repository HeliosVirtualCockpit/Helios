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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.Airspeed
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;

	[HeliosControl("Helios.Falcon.Airspeed", "Falcon BMS Airspeed", "Falcon Simulator", typeof(GaugeRenderer))]
	public class Airspeed : BaseGauge
	{
		private FalconInterface _falconInterface;
		private CalibrationPointCollectionDouble _needleCalibration;
		private CalibrationPointCollectionDouble _machCalibration;
		private GaugeImage _faceplate;
		private GaugeNeedle _machRing;
		private GaugeNeedle _needle;

		private const string _faceplateOffImage = "{HeliosFalcon}/Gauges/Airspeed/asi_faceplate_off.xaml";
		private const string _faceplateDimImage = "{HeliosFalcon}/Gauges/Airspeed/asi_faceplate_dim.xaml";
		private const string _faceplateBrtImage = "{HeliosFalcon}/Gauges/Airspeed/asi_faceplate_brt.xaml";
		private const string _machRingOffImage = "{HeliosFalcon}/Gauges/Airspeed/asi_mach_ring_off.xaml";
		private const string _machRingDimImage = "{HeliosFalcon}/Gauges/Airspeed/asi_mach_ring_dim.xaml";
		private const string _machRingBrtImage = "{HeliosFalcon}/Gauges/Airspeed/asi_mach_ring_brt.xaml";
		private const string _needleOffImage = "{HeliosFalcon}/Gauges/Airspeed/asi_needle_off.xaml";
		private const string _needleDimImage = "{HeliosFalcon}/Gauges/Airspeed/asi_needle_dim.xaml";
		private const string _needleBrtImage = "{HeliosFalcon}/Gauges/Airspeed/asi_needle_brt.xaml";

		private double _backlight;
		private bool _inFlightLastValue = true;

		public Airspeed()
			: base("Airspeed", new Size(300, 300))
		{
			AddComponents();
		}

		#region Components

		private void AddComponents()
		{
			_faceplate =new GaugeImage(_faceplateOffImage, new Rect(0d, 0d, 300d, 300d));
			Components.Add(_faceplate);

			_machCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1.9d, 270d);
			_machCalibration.Add(new CalibrationPointDouble(0.5d, 60d));

			_machRing = new GaugeNeedle(_machRingOffImage, new Point(150d, 150d), new Size(188d, 188d), new Point(94d, 94d), -90d);
			Components.Add(_machRing);

			_needleCalibration = new CalibrationPointCollectionDouble(0d, -49.5d, 850d, 350d);
			_needleCalibration.Add(new CalibrationPointDouble(100d, 45d));
			_needleCalibration.Add(new CalibrationPointDouble(200d, 135d));
			_needleCalibration.Add(new CalibrationPointDouble(300d, 195d));
			_needleCalibration.Add(new CalibrationPointDouble(400d, 235d));
			_needleCalibration.Add(new CalibrationPointDouble(500d, 267d));

			_needle = new GaugeNeedle(_needleOffImage, new Point(150d, 150d), new Size(300d, 300d), new Point(150d, 150d), -90d);
			Components.Add(_needle);
		}

		#endregion Components

		#region Methods

		protected override void OnProfileChanged(HeliosProfile oldProfile)
		{
			base.OnProfileChanged(oldProfile);

			if (oldProfile != null)
			{
				oldProfile.ProfileStarted -= new EventHandler(Profile_ProfileStarted);
				oldProfile.ProfileTick -= new EventHandler(Profile_ProfileTick);
				oldProfile.ProfileStopped -= new EventHandler(Profile_ProfileStopped);
			}

			if (Profile != null)
			{
				Profile.ProfileStarted += new EventHandler(Profile_ProfileStarted);
				Profile.ProfileTick += new EventHandler(Profile_ProfileTick);
				Profile.ProfileStopped += new EventHandler(Profile_ProfileStopped);
			}
		}

		private void Profile_ProfileStarted(object sender, EventArgs e)
		{
			if (Parent.Profile.Interfaces.ContainsKey("Falcon"))
			{
				_falconInterface = Parent.Profile.Interfaces["Falcon"] as FalconInterface;
			}
		}

		private void Profile_ProfileTick(object sender, EventArgs e)
		{
			if (_falconInterface != null)
			{
				BindingValue runtimeFlying = GetValue("Runtime", "Flying");
				bool inFlight = runtimeFlying.BoolValue;

				if (inFlight)
				{
					ProcessBindingValues();
					ProcessAirspeedValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetAirspeed();
						_inFlightLastValue = false;
					}
				}
			}
		}

		private void Profile_ProfileStopped(object sender, EventArgs e)
		{
			_falconInterface = null;
		}

		private void ProcessBindingValues()
		{
			BindingValue backlight = GetValue("Lighting", "instrument backlight");
			Backlight = backlight.DoubleValue;

			BindingValue airspeed = GetValue("IAS", "indicated air speed");
			AirSpeed = airspeed.DoubleValue;

			BindingValue mach = GetValue("IAS", "mach");
			Mach = mach.DoubleValue;
		}

		private void ProcessAirspeedValues()
		{
			_needle.Rotation = _needleCalibration.Interpolate(AirSpeed < 52.5 ? 52.5 : AirSpeed);
			_machRing.Rotation = _needle.Rotation - _machCalibration.Interpolate(Mach);
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_faceplate.Image = _faceplateDimImage;
				_machRing.Image = _machRingDimImage;
				_needle.Image = _needleDimImage;
			}
			else if (Backlight == 2)
			{
				_faceplate.Image = _faceplateBrtImage;
				_machRing.Image = _machRingBrtImage;
				_needle.Image = _needleBrtImage;
			}
			else
			{
				_faceplate.Image = _faceplateOffImage;
				_machRing.Image = _machRingOffImage;
				_needle.Image = _needleOffImage;
			}

			Refresh();
		}

		public override void Reset()
		{
			ResetAirspeed();
		}

		private void ResetAirspeed()
		{
			Backlight = 0d;
			AirSpeed = 0d;
			Mach = 0d;

			ProcessAirspeedValues();
			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Properties

		private double AirSpeed { get; set; }
		private double Mach { get; set; }

		private double Backlight
		{
			get
			{
				return _backlight;
			}
			set
			{
				double oldValue = _backlight;
				_backlight = value;
				if (!_backlight.Equals(oldValue))
				{
					ProcessBacklightValues();
				}
			}
		}

		#endregion Properties
	}
}
