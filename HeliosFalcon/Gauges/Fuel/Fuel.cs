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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.Fuel
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;

	[HeliosControl("Helios.Falcon.Fuel", "Falcon BMS Fuel", "Falcon Simulator", typeof(GaugeRenderer))]
	public class Fuel : BaseGauge
	{
		private FalconInterface _falconInterface;

		private CalibrationPointCollectionDouble _needleCalibration;
		private GaugeImage _backplate;
		private GaugeImage _faceplate;
		private GaugeDrumCounter _fuelDrum;
		private GaugeNeedle _needleAFT;
		private GaugeNeedle _needleFWD;

		private const string _backplateImage = "{HeliosFalcon}/Gauges/Common/gauge_backplate.xaml";
		private const string _faceplateOffImage = "{HeliosFalcon}/Gauges/Fuel/fuel_faceplate_off.xaml";
		private const string _faceplateDimImage = "{HeliosFalcon}/Gauges/Fuel/fuel_faceplate_dim.xaml";
		private const string _faceplateBrtImage = "{HeliosFalcon}/Gauges/Fuel/fuel_faceplate_brt.xaml";
		private const string _fuelDrumOffImage = "{HeliosFalcon}/Gauges/Common/drum_tape_off.xaml";
		private const string _fuelDrumDimImage = "{HeliosFalcon}/Gauges/Common/drum_tape_dim.xaml";
		private const string _fuelDrumBrtImage = "{HeliosFalcon}/Gauges/Common/drum_tape_brt.xaml";
		private const string _needleAftOffImage = "{HeliosFalcon}/Gauges/Fuel/fuel_needle_aft_off.xaml";
		private const string _needleAftDimImage = "{HeliosFalcon}/Gauges/Fuel/fuel_needle_aft_dim.xaml";
		private const string _needleAftBrtImage = "{HeliosFalcon}/Gauges/Fuel/fuel_needle_aft_brt.xaml";
		private const string _needleFwdOffImage = "{HeliosFalcon}/Gauges/Fuel/fuel_needle_fwd_off.xaml";
		private const string _needleFwdDimImage = "{HeliosFalcon}/Gauges/Fuel/fuel_needle_fwd_dim.xaml";
		private const string _needleFwdBrtImage = "{HeliosFalcon}/Gauges/Fuel/fuel_needle_fwd_brt.xaml";

		private double _backlight;
		private bool _inFlightLastValue = true;

		public Fuel()
			: base("Fuel", new Size(300, 300))
		{
			AddComponents();
		}

		#region Components

		private void AddComponents()
		{
			_backplate = new GaugeImage(_backplateImage, new Rect(0d, 0d, 300d, 300d));
			Components.Add(_backplate);

			_faceplate = new GaugeImage(_faceplateOffImage, new Rect(0d, 0d, 300d, 300d));
			Components.Add(_faceplate);

			_fuelDrum = new GaugeDrumCounter(_fuelDrumOffImage, new Point(100d, 216d), "####%", new Size(10d, 15d), new Size(20d, 28d));
			_fuelDrum.Clip = new RectangleGeometry(new Rect(100d, 218d, 100d, 24d));
			Components.Add(_fuelDrum);

			_needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 42000d, 252d);

			_needleAFT = new GaugeNeedle(_needleAftOffImage, new Point(150d, 150d), new Size(120d, 200d), new Point(60d, 150d), 235d);
			Components.Add(_needleAFT);
			
			_needleFWD = new GaugeNeedle(_needleFwdOffImage, new Point(150d, 150d), new Size(120d, 200d), new Point(60d, 150d), 235d);
			Components.Add(_needleFWD);
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
					ProcessFuelValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetFuel();
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

			BindingValue fuelAFT = GetValue("Fuel", "aft fuel");
			FuelAFT = fuelAFT.DoubleValue;

			BindingValue fuelFWD = GetValue("Fuel", "fwd fuel");
			FuelFWD = fuelFWD.DoubleValue;

			BindingValue fuelTotal = GetValue("Fuel", "total fuel");
			FuelTotal = fuelTotal.DoubleValue;
		}

		private void ProcessFuelValues()
		{
			_needleAFT.Rotation = _needleCalibration.Interpolate(FuelAFT);
			_needleFWD.Rotation = _needleCalibration.Interpolate(FuelFWD);

			_fuelDrum.Value = FuelTotal;
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_fuelDrum.Image = _fuelDrumDimImage;
				_faceplate.Image = _faceplateDimImage;
				_needleAFT.Image = _needleAftDimImage;
				_needleFWD.Image = _needleFwdDimImage;
			}
			else if (Backlight == 2)
			{
				_fuelDrum.Image = _fuelDrumBrtImage;
				_faceplate.Image = _faceplateBrtImage;
				_needleAFT.Image = _needleAftBrtImage;
				_needleFWD.Image = _needleFwdBrtImage;
			}
			else
			{
				_fuelDrum.Image = _fuelDrumOffImage;
				_faceplate.Image = _faceplateOffImage;
				_needleAFT.Image = _needleAftOffImage;
				_needleFWD.Image = _needleFwdOffImage;
			}

			Refresh();
		}

		public override void Reset() 
		{
			ResetFuel();
		}

		private void ResetFuel()
		{
			Backlight = 0d;
			FuelAFT = 0d;
			FuelFWD = 0d;
			FuelTotal = 0d;

			ProcessFuelValues();
			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Properties

		private double FuelAFT { get; set; }
		private double FuelFWD { get; set; }
		private double FuelTotal { get; set; }

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
