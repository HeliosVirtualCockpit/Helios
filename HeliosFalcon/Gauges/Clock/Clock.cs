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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.Clock
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;

	[HeliosControl("Helios.Falcon.Clock", "Falcon BMS Clock", "Falcon Simulator", typeof(GaugeRenderer))]
	public class Clock : BaseGauge
	{
		private FalconInterface _falconInterface;
		private GaugeNeedle _needleHours;
		private GaugeNeedle _needleMinutes;
		private GaugeNeedle _needleSeconds;
		private GaugeImage _faceplate;
 
		private const string _faceplateOffImage = "{HeliosFalcon}/Gauges/Clock/clock_faceplate_off.xaml";
		private const string _faceplateDimImage = "{HeliosFalcon}/Gauges/Clock/clock_faceplate_dim.xaml";
		private const string _faceplateBrtImage = "{HeliosFalcon}/Gauges/Clock/clock_faceplate_brt.xaml";
		private const string _needleHoursOffImage = "{HeliosFalcon}/Gauges/Clock/hour_hand_off.xaml";
		private const string _needleHoursDimImage = "{HeliosFalcon}/Gauges/Clock/hour_hand_dim.xaml";
		private const string _needleHoursBrtImage = "{HeliosFalcon}/Gauges/Clock/hour_hand_brt.xaml";
		private const string _needleMinutesOffImage = "{HeliosFalcon}/Gauges/Clock/minute_hand_off.xaml";
		private const string _needleMinutesDimImage = "{HeliosFalcon}/Gauges/Clock/minute_hand_dim.xaml";
		private const string _needleMinutesBrtImage = "{HeliosFalcon}/Gauges/Clock/minute_hand_brt.xaml";
		private const string _needleSecondsOffImage = "{HeliosFalcon}/Gauges/Clock/second_hand_off.xaml";
		private const string _needleSecondsDimImage = "{HeliosFalcon}/Gauges/Clock/second_hand_dim.xaml";
		private const string _needleSecondsBrtImage = "{HeliosFalcon}/Gauges/Clock/second_hand_brt.xaml";

		private double _backlight;
		private bool _inFlightLastValue = true;

		public Clock()
			: base("Clock", new Size(300, 300))
		{
			AddComponents();
		}

		#region Components

		private void AddComponents()
		{
			_faceplate = new GaugeImage(_faceplateOffImage, new Rect(0d, 0d, 300d, 300d));
			Components.Add(_faceplate);

			_needleHours = new GaugeNeedle(_needleHoursOffImage, new Point(150d, 150d), new Size(40d, 300d), new Point(20d, 150d), 0d);
			Components.Add(_needleHours);

			_needleMinutes = new GaugeNeedle(_needleMinutesOffImage, new Point(150d, 150d), new Size(40d, 300d), new Point(20d, 150d), 0d);
			Components.Add(_needleMinutes);

			_needleSeconds = new GaugeNeedle(_needleSecondsOffImage, new Point(150d, 150d), new Size(40d, 300d), new Point(20d, 150d), 0d);
			Components.Add(_needleSeconds);
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
					ProcessClockValues();
					 _inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetClock();
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

			BindingValue timeValue = GetValue("Time", "Time");
			TimeInSeconds = timeValue.DoubleValue;
		}

		private void ProcessClockValues()
		{
			TimeSpan timespan = TimeSpan.FromSeconds(TimeInSeconds);

			_needleHours.Rotation = timespan.Hours * 360 / 12; ;
			_needleMinutes.Rotation = timespan.Minutes * 360 / 60; ;
			_needleSeconds.Rotation = timespan.Seconds * 360 / 60; ;
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_faceplate.Image = _faceplateDimImage;
				_needleHours.Image = _needleHoursDimImage;
				_needleMinutes.Image = _needleMinutesDimImage;
				_needleSeconds.Image = _needleSecondsDimImage;
			}
			else if (Backlight == 2)
			{
				_faceplate.Image = _faceplateBrtImage;
				_needleHours.Image = _needleHoursBrtImage;
				_needleMinutes.Image = _needleMinutesBrtImage;
				_needleSeconds.Image = _needleSecondsBrtImage;
			}
			else
			{
				_faceplate.Image = _faceplateOffImage;
				_needleHours.Image = _needleHoursOffImage;
				_needleMinutes.Image = _needleMinutesOffImage;
				_needleSeconds.Image = _needleSecondsOffImage;
			}

			Refresh();
		}

		public override void Reset()
		{
			ResetClock();
		}

		private void ResetClock()
		{
			Backlight = 0d;

			_needleHours.Rotation = 0;
			_needleMinutes.Rotation = 0;
			_needleSeconds.Rotation = 0;

			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Properties

		private double TimeInSeconds { get; set; }

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
