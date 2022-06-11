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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.FuelFlow
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;

	[HeliosControl("Helios.Falcon.FuelFLow", "Falcon BMS Fuel Flow", "Falcon Simulator", typeof(GaugeRenderer))]
	public class FuelFlow : BaseGauge
	{
		private FalconInterface _falconInterface;

		private GaugeDrumCounter _drum;
		private GaugeImage _labels;

		private const string _drumTapeOffImage = "{HeliosFalcon}/Gauges/Common/drum_tape_off.xaml";
		private const string _drumTapeDimImage = "{HeliosFalcon}/Gauges/Common/drum_tape_dim.xaml";
		private const string _drumTapeBrtImage = "{HeliosFalcon}/Gauges/Common/drum_tape_brt.xaml";
		private const string _labelsOffImage = "{HeliosFalcon}/Gauges/FuelFlow/fuelflow_labels_off.xaml";
		private const string _labelsDimImage = "{HeliosFalcon}/Gauges/FuelFlow/fuelflow_labels_dim.xaml";
		private const string _labelsBrtImage = "{HeliosFalcon}/Gauges/FuelFlow/fuelflow_labels_brt.xaml";
		private const string _faceplateImage = "{HeliosFalcon}/Gauges/FuelFlow/fuelflow_bezel.png";

		private double _backlight;
		private bool _inFlightLastValue = true;

		public FuelFlow()
			: base("Fuel Flow", new Size(220, 204))
		{
			AddComponents();
		}

		#region Components

		private void AddComponents()
		{
			Rect drumRect = new Rect(29d, 60d, 162d, 80d);

			Components.Add(new GaugeRectangle(Colors.Black, drumRect));

			_drum = new GaugeDrumCounter(_drumTapeOffImage, new Point(30d, 79d), "##%00", new Size(10d, 15d), new Size(32d, 48d))
			{
				Clip = new RectangleGeometry(drumRect)
			};
			Components.Add(_drum);

			Components.Add(new GaugeImage(_faceplateImage, new Rect(0d, 0d, 220d, 204d)));

			_labels = new GaugeImage(_labelsOffImage, new Rect(0d, 0d, 220d, 204d));
			Components.Add(_labels);
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
					ProcessFuelFlowValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetFuelFlow();
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

			BindingValue fuelflow = GetValue("Engine", "fuel flow");
			FFlow = fuelflow.DoubleValue;
		}

		private void ProcessFuelFlowValues()
		{
			_drum.Value = FFlow;
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_drum.Image = _drumTapeDimImage;
				_labels.Image = _labelsDimImage;
			}
			else if (Backlight == 2)
			{
				_drum.Image = _drumTapeBrtImage;
				_labels.Image = _labelsBrtImage;
			}
			else
			{
				_drum.Image = _drumTapeOffImage;
				_labels.Image = _labelsOffImage;
			}

			Refresh();
		}

		public override void Reset()
		{
			ResetFuelFlow();
		}

		private void ResetFuelFlow()
		{
			Backlight = 0d;
			FFlow = 0d;

			ProcessFuelFlowValues();
			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Properties

		private double FFlow { get; set; }

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
