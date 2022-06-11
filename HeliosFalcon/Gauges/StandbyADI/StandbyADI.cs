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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.StandbyADI
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;

	[HeliosControl("Helios.Falcon.StandbyADI", "Falcon BMS Standby ADI", "Falcon Simulator", typeof(GaugeRenderer))]
	public class StandbyADI : BaseGauge
	{
		private FalconInterface _falconInterface;

		private GaugeImage _faceplate;
		private GaugeImage _offFlag;

		private GaugeNeedle _ball;
		private GaugeNeedle _ballMask;
		private GaugeNeedle _ring;

		private const string _ballOffImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_ball_off.xaml";
		private const string _ballDimImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_ball_dim.xaml";
		private const string _ballBrtImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_ball_brt.xaml";

		private const string _ringOffImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_ring_off.xaml";
		private const string _ringDimImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_ring_dim.xaml";
		private const string _ringBrtImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_ring_brt.xaml";

		private const string _faceplateImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_faceplate.png";
		private const string _ballMaskImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_ball_mask.png";
		private const string _offFlagImage = "{HeliosFalcon}/Gauges/StandbyADI/adi_off_flag.png";

		private CalibrationPointCollectionDouble _pitchCalibration;

		private double _backlight;
		private bool _inFlightLastValue = true;

		public StandbyADI()
			: base("StandbyADI", new Size(310, 300))
		{
			AddComponents();
		}

		#region Components

		private void AddComponents()
		{
			_pitchCalibration = new CalibrationPointCollectionDouble(-360d, -864d, 360d, 864d);

			_ball = new GaugeNeedle(_ballOffImage, new Point(155d, 145d), new Size(192d, 1152d), new Point(96d, 576d));
			_ball.Clip = new EllipseGeometry(new Point(155d, 145d), 96d, 96d);
			Components.Add(_ball);

			_ballMask = new GaugeNeedle(_ballMaskImage, new Point(155d, 145d), new Size(210d, 210d), new Point(105d, 105d));
			_ballMask.Clip = new EllipseGeometry(new Point(155d, 145d), 105d, 105d);
			Components.Add(_ballMask);

			_ring = new GaugeNeedle(_ringOffImage, new Point(155d, 145d), new Size(310d, 300d), new Point(155d, 145d));
			_ring.Clip = new EllipseGeometry(new Point(155d, 145d), 125d, 125d);
			Components.Add(_ring);

			_offFlag = new GaugeImage(_offFlagImage, new Rect(40d, 70d, 115d, 180d));
			_offFlag.IsHidden = true;
			Components.Add(_offFlag);

			_faceplate = new GaugeImage(_faceplateImage, new Rect(0d, 0d, 310d, 300d));
			Components.Add(_faceplate);
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
					ProcessStandbyADIValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetStandbyADI();
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

			BindingValue pitch = GetValue("ADI", "pitch");
			PitchAngle = pitch.DoubleValue * 180 / Math.PI;

			BindingValue roll = GetValue("ADI", "roll");
			RollAngle = roll.DoubleValue * 180 / Math.PI;

			BindingValue flagOFF = GetValue("Backup ADI", "off flag");
			FlagOff = flagOFF.BoolValue;
		}

		private void ProcessStandbyADIValues()
		{
			_ball.VerticalOffset = _pitchCalibration.Interpolate(PitchAngle);
			_ball.Rotation = -RollAngle;
			_ballMask.Rotation = -RollAngle;
			_ring.Rotation = -RollAngle;

			_offFlag.IsHidden = !FlagOff;
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_ring.Image = _ringDimImage;
				_ball.Image = _ballDimImage;
			}
			else if (Backlight == 2)
			{
				_ring.Image = _ringBrtImage;
				_ball.Image = _ballBrtImage;
			}
			else
			{
				_ring.Image = _ringOffImage;
				_ball.Image = _ballOffImage;
			}

			Refresh();
		}

		public override void Reset()
		{
			ResetStandbyADI();
		}

		private void ResetStandbyADI()
		{
			Backlight = 0d;

			PitchAngle = 0d;
			RollAngle = 0d;

			FlagOff = false;

			ProcessStandbyADIValues();
			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Properties

		private double PitchAngle { get; set; }
		private double RollAngle { get; set; }

		private bool FlagOff { get; set; }

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
