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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.Altimeter
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;

	[HeliosControl("Helios.Falcon.Altimeter", "Falcon BMS Altimeter", "Falcon Simulator", typeof(GaugeRenderer))]
	public class Altimeter : BaseGauge
	{
		private FalconInterface _falconInterface;

		private CalibrationPointCollectionDouble _needleCalibration;
		private GaugeDrumCounter _tensDrum;
		private GaugeDrumCounter _mainDrum;
		private GaugeDrumCounter _pressureDrum;
		private GaugeImage _faceplate;
		private GaugeNeedle _needle;

		private const string _tensDrumOffImage = "{HeliosFalcon}/Gauges/Altimeter/alt_drum_tape_off.xaml";
		private const string _tensDrumDimImage = "{HeliosFalcon}/Gauges/Altimeter/alt_drum_tape_dim.xaml";
		private const string _tensDrumBrtImage = "{HeliosFalcon}/Gauges/Altimeter/alt_drum_tape_brt.xaml";
		private const string _mainDrumOffImage = "{HeliosFalcon}/Gauges/Common/drum_tape_off.xaml";
		private const string _mainDrumDimImage = "{HeliosFalcon}/Gauges/Common/drum_tape_dim.xaml";
		private const string _mainDrumBrtImage = "{HeliosFalcon}/Gauges/Common/drum_tape_brt.xaml";
		private const string _pressureDrumOffImage = "{HeliosFalcon}/Gauges/Common/drum_tape_off.xaml";
		private const string _pressureDrumDimImage = "{HeliosFalcon}/Gauges/Common/drum_tape_dim.xaml";
		private const string _pressureDrumBrtImage = "{HeliosFalcon}/Gauges/Common/drum_tape_brt.xaml";
		private const string _faceplateOffImage = "{HeliosFalcon}/Gauges/Altimeter/altimeter_faceplate_off.xaml";
		private const string _faceplateDimImage = "{HeliosFalcon}/Gauges/Altimeter/altimeter_faceplate_dim.xaml";
		private const string _faceplateBrtImage = "{HeliosFalcon}/Gauges/Altimeter/altimeter_faceplate_brt.xaml";
		private const string _needleOffImage = "{HeliosFalcon}/Gauges/Altimeter/altimeter_needle_off.xaml";
		private const string _needleDimImage = "{HeliosFalcon}/Gauges/Altimeter/altimeter_needle_dim.xaml";
		private const string _needleBrtImage = "{HeliosFalcon}/Gauges/Altimeter/altimeter_needle_brt.xaml";

		private double _backlight;
		private bool _inFlightLastValue = true;

		public Altimeter()
			: base("Altimeter", new Size(300, 300))
		{
			AddComponents();
		}

		#region Components

		private void AddComponents()
		{
			_tensDrum = new GaugeDrumCounter(_tensDrumOffImage, new Point(39d, 126d), "##", new Size(10d, 15d), new Size(30d, 45d));
			_tensDrum.Clip = new RectangleGeometry(new Rect(39d, 106d, 30d, 81d));
			Components.Add(_tensDrum);

			_mainDrum = new GaugeDrumCounter(_mainDrumOffImage, new Point(69d, 126d), "#%00", new Size(10d, 15d), new Size(30d, 45d));
			_mainDrum.Clip = new RectangleGeometry(new Rect(69d, 106d, 150d, 81d));
			Components.Add(_mainDrum);

			_pressureDrum = new GaugeDrumCounter(_pressureDrumOffImage, new Point(182d, 195d), "###%", new Size(10d, 15d), new Size(15d, 20d));
			_pressureDrum.Clip = new RectangleGeometry(new Rect(182d, 195d, 60d, 20d));
			Components.Add(_pressureDrum);

			_faceplate = new GaugeImage(_faceplateOffImage, new Rect(0d, 0d, 300d, 300d));
			Components.Add(_faceplate);

			_needleCalibration = new CalibrationPointCollectionDouble(-1000d, -360d, 1000d, 360d);

			_needle = new GaugeNeedle(_needleOffImage, new Point(150d, 150d), new Size(16d, 257d), new Point(8d, 138.5d));
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
					ProcessAltimeterValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetAltimeter();
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

			BindingValue altitude = GetValue("Altimeter", "indicated altitude");
			Altitude = altitude.DoubleValue;

			BindingValue pressure = GetValue("Altimeter", "barimetric pressure");
			Pressure = pressure.DoubleValue > 0 ? pressure.DoubleValue : 2992d;
		}

		private void ProcessAltimeterValues()
		{
			 _needle.Rotation = _needleCalibration.Interpolate(Altitude % 1000d);
			_tensDrum.Value = (Altitude < 0 ? 99999 - Math.Abs(Altitude) : Altitude) / 1000d;
			_mainDrum.Value = Altitude < 0 ? 99999 - Math.Abs(Altitude) : Altitude;
			_pressureDrum.Value = Pressure;
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_tensDrum.Image = _tensDrumDimImage;
				_mainDrum.Image = _mainDrumDimImage;
				_pressureDrum.Image = _pressureDrumDimImage;
				_faceplate.Image = _faceplateDimImage;
				_needle.Image = _needleDimImage;
			}
			else if (Backlight == 2)
			{
				_tensDrum.Image = _tensDrumBrtImage;
				_mainDrum.Image = _mainDrumBrtImage;
				_pressureDrum.Image = _pressureDrumBrtImage;
				_faceplate.Image = _faceplateBrtImage;
				_needle.Image = _needleBrtImage;
			}
			else
			{
				_tensDrum.Image = _tensDrumOffImage;
				_mainDrum.Image = _mainDrumOffImage;
				_pressureDrum.Image = _pressureDrumOffImage;
				_faceplate.Image = _faceplateOffImage;
				_needle.Image = _needleOffImage;
			}

			Refresh();
		}

		public override void Reset() 
		{
			ResetAltimeter();
		}

		private void ResetAltimeter()
		{
			Backlight = 0d;
			Altitude = 0d;
			Pressure = 2992d;

			ProcessAltimeterValues();
			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Properties

		private double Altitude { get; set; }
		private double Pressure { get; set; }

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
