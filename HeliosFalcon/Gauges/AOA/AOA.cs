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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.AOA
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;

	[HeliosControl("Helios.Falcon.AOA", "Falcon BMS AOA", "Falcon Simulator", typeof(GaugeRenderer))]
	public class AOA : BaseGauge
	{
		private FalconInterface _falconInterface;
		private GaugeImage _offFlagImage;
		private GaugeImage _faceplate;
		private GaugeNeedle _aoaTape;
		private CalibrationPointCollectionDouble _tapeCalibration;



		private const string _backplateImage = "{HeliosFalcon}/Gauges/Common/aoa_vvi_bezel.png";
		private const string _flagImage = "{HeliosFalcon}/Gauges/AOA/aoa_off_flag.xaml";
		private const string _faceplateOffImage = "{HeliosFalcon}/Gauges/AOA/aoa_faceplate_off.xaml";
		private const string _faceplateDimImage = "{HeliosFalcon}/Gauges/AOA/aoa_faceplate_dim.xaml";
		private const string _faceplateBrtImage = "{HeliosFalcon}/Gauges/AOA/aoa_faceplate_brt.xaml";
		private const string _tapeOffImage = "{HeliosFalcon}/Gauges/AOA/aoa_tape_off.xaml";
		private const string _tapeDimImage = "{HeliosFalcon}/Gauges/AOA/aoa_tape_dim.xaml";
		private const string _tapeBrtImage = "{HeliosFalcon}/Gauges/AOA/aoa_tape_brt.xaml";

		private double _backlight;
		private bool _inFlightLastValue = true;

		public AOA()
			: base("AOA", new Size(220, 452))
		{
			AddComponents();
		}

		#region Components

		private void AddComponents()
		{
			_tapeCalibration = new CalibrationPointCollectionDouble(-30d, -600d, 30d, 600d);

			_aoaTape = new GaugeNeedle(_tapeOffImage, new Point(110, 226), new Size(130, 1482), new Point(65, 741))
			{
				Clip = new RectangleGeometry(new Rect(55d, 86d, 130d, 280d))
			};
			Components.Add(_aoaTape);

			_offFlagImage = new GaugeImage(_flagImage, new Rect(55d, 84d, 110d, 282d))
			{
				IsHidden = true
			};
			Components.Add(_offFlagImage);

			_faceplate = new GaugeImage(_faceplateOffImage, new Rect(0, 0, 220, 452));
			Components.Add(_faceplate);

			Components.Add(new GaugeImage(_backplateImage, new Rect(0, 0, 220, 452)));
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
					ProcessAOAValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetAOA();
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

			BindingValue aoa = GetValue("AOA", "angle of attack");
			AngleOfAttack = aoa.DoubleValue;

			BindingValue off = GetValue("AOA", "off flag");
			OffFlag = off.BoolValue;
		}

		private void ProcessAOAValues()
		{
			_aoaTape.VerticalOffset = _tapeCalibration.Interpolate(AngleOfAttack);
			_offFlagImage.IsHidden = !OffFlag;
			
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_faceplate.Image = _faceplateDimImage;
				_aoaTape.Image = _tapeDimImage;
			}
			else if (Backlight == 2)
			{
				_faceplate.Image = _faceplateBrtImage;
				_aoaTape.Image = _tapeBrtImage;
			}
			else
			{
				_faceplate.Image = _faceplateOffImage;
				_aoaTape.Image = _tapeOffImage;
			} 

			Refresh();
		}

		public override void Reset()
		{
			ResetAOA();
		}

		private void ResetAOA()
		{
			Backlight = 0d;
			AngleOfAttack = 0d;
			OffFlag = false;

			ProcessAOAValues();
			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods



		#region Properties

		private double AngleOfAttack { get; set; }
		
		private bool OffFlag { get; set; }

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
