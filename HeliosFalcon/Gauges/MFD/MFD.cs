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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.MFD
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;

	[HeliosControl("Helios.Falcon.MFD", "Falcon BMS MFD", "Falcon Simulator", typeof(GaugeRenderer))]
	public class MFD : BaseGauge
	{
		private FalconInterface _falconInterface;
		private GaugeImage _backplate;
		private GaugeImage _faceplate;
		private GaugeImage _buttonOSB01;
		private GaugeImage _buttonOSB02;
		private GaugeImage _buttonOSB03;
		private GaugeImage _buttonOSB04;
		private GaugeImage _buttonOSB05;
		private GaugeImage _buttonOSB06;
		private GaugeImage _buttonOSB07;
		private GaugeImage _buttonOSB08;
		private GaugeImage _buttonOSB09;
		private GaugeImage _buttonOSB10;
		private GaugeImage _buttonOSB11;
		private GaugeImage _buttonOSB12;
		private GaugeImage _buttonOSB13;
		private GaugeImage _buttonOSB14;
		private GaugeImage _buttonOSB15;
		private GaugeImage _buttonOSB16;
		private GaugeImage _buttonOSB17;
		private GaugeImage _buttonOSB18;
		private GaugeImage _buttonOSB19;
		private GaugeImage _buttonOSB20;
		private GaugeImage _rockerGAIN;
		private GaugeImage _rockerSYM;
		private GaugeImage _rockerBRT;
		private GaugeImage _rockerCON;

		private HeliosTrigger _buttonOSB01Trigger;
		private HeliosTrigger _buttonOSB02Trigger;
		private HeliosTrigger _buttonOSB03Trigger;
		private HeliosTrigger _buttonOSB04Trigger;
		private HeliosTrigger _buttonOSB05Trigger;
		private HeliosTrigger _buttonOSB06Trigger;
		private HeliosTrigger _buttonOSB07Trigger;
		private HeliosTrigger _buttonOSB08Trigger;
		private HeliosTrigger _buttonOSB09Trigger;
		private HeliosTrigger _buttonOSB10Trigger;
		private HeliosTrigger _buttonOSB11Trigger;
		private HeliosTrigger _buttonOSB12Trigger;
		private HeliosTrigger _buttonOSB13Trigger;
		private HeliosTrigger _buttonOSB14Trigger;
		private HeliosTrigger _buttonOSB15Trigger;
		private HeliosTrigger _buttonOSB16Trigger;
		private HeliosTrigger _buttonOSB17Trigger;
		private HeliosTrigger _buttonOSB18Trigger;
		private HeliosTrigger _buttonOSB19Trigger;
		private HeliosTrigger _buttonOSB20Trigger;
		private HeliosTrigger _rockerGAINUpTrigger;
		private HeliosTrigger _rockerGAINDownTrigger;
		private HeliosTrigger _rockerSYMUpTrigger;
		private HeliosTrigger _rockerSYMDownTrigger;
		private HeliosTrigger _rockerBRTUpTrigger;
		private HeliosTrigger _rockerBRTDownTrigger;
		private HeliosTrigger _rockerCONUpTrigger;
		private HeliosTrigger _rockerCONDownTrigger;

		Rect _rectButtonOSB01 = new Rect(100, 8, 51, 51);
		Rect _rectButtonOSB02 = new Rect(156, 8, 51, 51);
		Rect _rectButtonOSB03 = new Rect(212, 8, 51, 51);
		Rect _rectButtonOSB04 = new Rect(268, 8, 51, 51);
		Rect _rectButtonOSB05 = new Rect(324, 8, 51, 51);
		Rect _rectButtonOSB06 = new Rect(416, 100, 51, 51);
		Rect _rectButtonOSB07 = new Rect(416, 156, 51, 51);
		Rect _rectButtonOSB08 = new Rect(416, 212, 51, 51);
		Rect _rectButtonOSB09 = new Rect(416, 268, 51, 51);
		Rect _rectButtonOSB10 = new Rect(416, 324, 51, 51);
		Rect _rectButtonOSB11 = new Rect(324, 416, 51, 51);
		Rect _rectButtonOSB12 = new Rect(268, 416, 51, 51);
		Rect _rectButtonOSB13 = new Rect(212, 416, 51, 51);
		Rect _rectButtonOSB14 = new Rect(156, 416, 51, 51);
		Rect _rectButtonOSB15 = new Rect(100, 416, 51, 51);
		Rect _rectButtonOSB16 = new Rect(8, 324, 51, 51);
		Rect _rectButtonOSB17 = new Rect(8, 268, 51, 51);
		Rect _rectButtonOSB18 = new Rect(8, 212, 51, 51);
		Rect _rectButtonOSB19 = new Rect(8, 156, 51, 51);
		Rect _rectButtonOSB20 = new Rect(8, 100, 51, 51);
		Rect _rectRockerGAINUp = new Rect(11, 23, 45, 35);
		Rect _rectRockerGAINCenter = new Rect(11, 23, 45, 75);
		Rect _rectRockerGAINDown = new Rect(11, 63, 45, 35);
		Rect _rectRockerSYMUp = new Rect(419, 23, 45, 35);
		Rect _rectRockerSYMCenter = new Rect(419, 23, 45, 75);
		Rect _rectRockerSYMDown = new Rect(419, 63, 45, 35);
		Rect _rectRockerBRTUp = new Rect(11, 375, 45, 35);
		Rect _rectRockerBRTCenter = new Rect(11, 375, 45, 75);
		Rect _rectRockerBRTDown = new Rect(11, 415, 45, 35);
		Rect _rectRockerCONUp = new Rect(419, 375, 45, 35);
		Rect _rectRockerCONCenter = new Rect(419, 375, 45, 75);
		Rect _rectRockerCONDown = new Rect(419, 415, 45, 35);

		private const string _backplateOffImage = "{HeliosFalcon}/Gauges/MFD/mfd_backplate_off.xaml";
		private const string _backplateDimImage = "{HeliosFalcon}/Gauges/MFD/mfd_backplate_dim.xaml";
		private const string _backplateBrtImage = "{HeliosFalcon}/Gauges/MFD/mfd_backplate_brt.xaml";
		private const string _faceplateImage = "{HeliosFalcon}/Gauges/MFD/mfd_faceplate.png";
		private const string _buttonOffImage = "{HeliosFalcon}/Gauges/MFD/mfd_button_off.png";
		private const string _buttonLitImage = "{HeliosFalcon}/Gauges/MFD/mfd_button_lit.png";
		private const string _buttonPressImage = "{HeliosFalcon}/Gauges/MFD/mfd_button_press.png";
		private const string _rockerGAINUpImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_gain_up.png";
		private const string _rockerGAINCenterImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_gain_center.png";
		private const string _rockerGAINDownImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_gain_down.png";
		private const string _rockerSYMUpImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_sym_up.png";
		private const string _rockerSYMCenterImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_sym_center.png";
		private const string _rockerSYMDownImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_sym_down.png";
		private const string _rockerBRTUpImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_brt_up.png";
		private const string _rockerBRTCenterImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_brt_center.png";
		private const string _rockerBRTDownImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_brt_down.png";
		private const string _rockerCONUpImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_con_up.png";
		private const string _rockerCONCenterImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_con_center.png";
		private const string _rockerCONDownImage = "{HeliosFalcon}/Gauges/MFD/mfd_rocker_con_down.png";

		private static Rect _rectBase = new Rect(0d, 0d, 475d, 475d);
		private double _backlight;
		private bool _inFlightLastValue = true;

		public MFD()
			: base("MFD", new Size(_rectBase.Width, _rectBase.Height))
		{
			AddComponents();
			AddActions();
		}

		#region Components

		private void AddComponents()
		{
			_backplate = new GaugeImage(_backplateOffImage, _rectBase);
			Components.Add(_backplate);

			_faceplate = new GaugeImage(_faceplateImage, _rectBase);
			Components.Add(_faceplate);

			_buttonOSB01 = new GaugeImage(_buttonOffImage, _rectButtonOSB01);
			Components.Add(_buttonOSB01);

			_buttonOSB02 = new GaugeImage(_buttonOffImage, _rectButtonOSB02);
			Components.Add(_buttonOSB02);

			_buttonOSB03 = new GaugeImage(_buttonOffImage, _rectButtonOSB03);
			Components.Add(_buttonOSB03);

			_buttonOSB04 = new GaugeImage(_buttonOffImage, _rectButtonOSB04);
			Components.Add(_buttonOSB04);

			_buttonOSB05 = new GaugeImage(_buttonOffImage, _rectButtonOSB05);
			Components.Add(_buttonOSB05);

			_buttonOSB06 = new GaugeImage(_buttonOffImage, _rectButtonOSB06);
			Components.Add(_buttonOSB06);

			_buttonOSB07 = new GaugeImage(_buttonOffImage, _rectButtonOSB07);
			Components.Add(_buttonOSB07);

			_buttonOSB08 = new GaugeImage(_buttonOffImage, _rectButtonOSB08);
			Components.Add(_buttonOSB08);

			_buttonOSB09 = new GaugeImage(_buttonOffImage, _rectButtonOSB09);
			Components.Add(_buttonOSB09);

			_buttonOSB10 = new GaugeImage(_buttonOffImage, _rectButtonOSB10);
			Components.Add(_buttonOSB10);

			_buttonOSB11 = new GaugeImage(_buttonOffImage, _rectButtonOSB11);
			Components.Add(_buttonOSB11);

			_buttonOSB12 = new GaugeImage(_buttonOffImage, _rectButtonOSB12);
			Components.Add(_buttonOSB12);

			_buttonOSB13 = new GaugeImage(_buttonOffImage, _rectButtonOSB13);
			Components.Add(_buttonOSB13);

			_buttonOSB14 = new GaugeImage(_buttonOffImage, _rectButtonOSB14);
			Components.Add(_buttonOSB14);

			_buttonOSB15 = new GaugeImage(_buttonOffImage, _rectButtonOSB15);
			Components.Add(_buttonOSB15);

			_buttonOSB16 = new GaugeImage(_buttonOffImage, _rectButtonOSB16);
			Components.Add(_buttonOSB16);

			_buttonOSB17 = new GaugeImage(_buttonOffImage, _rectButtonOSB17);
			Components.Add(_buttonOSB17);

			_buttonOSB18 = new GaugeImage(_buttonOffImage, _rectButtonOSB18);
			Components.Add(_buttonOSB18);

			_buttonOSB19 = new GaugeImage(_buttonOffImage, _rectButtonOSB19);
			Components.Add(_buttonOSB19);

			_buttonOSB20 = new GaugeImage(_buttonOffImage, _rectButtonOSB20);
			Components.Add(_buttonOSB20);

			_rockerGAIN = new GaugeImage(_rockerGAINCenterImage, _rectRockerGAINCenter);
			Components.Add(_rockerGAIN);

			_rockerSYM = new GaugeImage(_rockerSYMCenterImage, _rectRockerSYMCenter);
			Components.Add(_rockerSYM);

			_rockerBRT = new GaugeImage(_rockerBRTCenterImage, _rectRockerBRTCenter);
			Components.Add(_rockerBRT);

			_rockerCON = new GaugeImage(_rockerCONCenterImage, _rectRockerCONCenter);
			Components.Add(_rockerCON);
		}

		#endregion Components

		#region Actions

		private void AddActions()
		{
			_buttonOSB01Trigger = new HeliosTrigger(this, "", "", "Button OSB-1 Pushed", "Fired when button OSB-1 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB01Trigger);

			_buttonOSB02Trigger = new HeliosTrigger(this, "", "", "Button OSB-2 Pushed", "Fired when button OSB-2 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB02Trigger);

			_buttonOSB03Trigger = new HeliosTrigger(this, "", "", "Button OSB-3 Pushed", "Fired when button OSB-3 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB03Trigger);

			_buttonOSB04Trigger = new HeliosTrigger(this, "", "", "Button OSB-4 Pushed", "Fired when button OSB-4 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB04Trigger);

			_buttonOSB05Trigger = new HeliosTrigger(this, "", "", "Button OSB-5 Pushed", "Fired when button OSB-5 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB05Trigger);

			_buttonOSB06Trigger = new HeliosTrigger(this, "", "", "Button OSB-6 Pushed", "Fired when button OSB-6 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB06Trigger);

			_buttonOSB07Trigger = new HeliosTrigger(this, "", "", "Button OSB-7 Pushed", "Fired when button OSB-7 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB07Trigger);

			_buttonOSB08Trigger = new HeliosTrigger(this, "", "", "Button OSB-8 Pushed", "Fired when button OSB-8 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB08Trigger);

			_buttonOSB09Trigger = new HeliosTrigger(this, "", "", "Button OSB-9 Pushed", "Fired when button OSB-9 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB09Trigger);

			_buttonOSB10Trigger = new HeliosTrigger(this, "", "", "Button OSB-10 Pushed", "Fired when button OSB-10 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB10Trigger);

			_buttonOSB11Trigger = new HeliosTrigger(this, "", "", "Button OSB-11 Pushed", "Fired when button OSB-11 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB11Trigger);

			_buttonOSB12Trigger = new HeliosTrigger(this, "", "", "Button OSB-12 Pushed", "Fired when button OSB-12 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB12Trigger);

			_buttonOSB13Trigger = new HeliosTrigger(this, "", "", "Button OSB-13 Pushed", "Fired when button OSB-13 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB13Trigger);

			_buttonOSB14Trigger = new HeliosTrigger(this, "", "", "Button OSB-14 Pushed", "Fired when button OSB-14 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB14Trigger);

			_buttonOSB15Trigger = new HeliosTrigger(this, "", "", "Button OSB-15 Pushed", "Fired when button OSB-15 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB15Trigger);

			_buttonOSB16Trigger = new HeliosTrigger(this, "", "", "Button OSB-16 Pushed", "Fired when button OSB-16 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB16Trigger);

			_buttonOSB17Trigger = new HeliosTrigger(this, "", "", "Button OSB-17 Pushed", "Fired when button OSB-17 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB17Trigger);

			_buttonOSB18Trigger = new HeliosTrigger(this, "", "", "Button OSB-18 Pushed", "Fired when button OSB-18 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB18Trigger);

			_buttonOSB19Trigger = new HeliosTrigger(this, "", "", "Button OSB-19 Pushed", "Fired when button OSB-19 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB19Trigger);

			_buttonOSB20Trigger = new HeliosTrigger(this, "", "", "Button OSB-20 Pushed", "Fired when button OSB-20 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonOSB20Trigger);

			_rockerGAINUpTrigger = new HeliosTrigger(this, "", "", "Rocker GAIN-UP Pushed", "Fired when rocker GAIN-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerGAINUpTrigger);

			_rockerGAINDownTrigger = new HeliosTrigger(this, "", "", "Rocker GAIN-DOWN Pushed", "Fired when rocker GAIN-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerGAINDownTrigger);

			_rockerBRTUpTrigger = new HeliosTrigger(this, "", "", "Rocker BRT-UP Pushed", "Fired when rocker BRT-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerBRTUpTrigger);

			_rockerBRTDownTrigger = new HeliosTrigger(this, "", "", "Rocker BRT-DOWN Pushed", "Fired when rocker BRT-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerBRTDownTrigger);

			_rockerSYMUpTrigger = new HeliosTrigger(this, "", "", "Rocker SYM-UP Pushed", "Fired when rocker SYM-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerSYMUpTrigger);

			_rockerSYMDownTrigger = new HeliosTrigger(this, "", "", "Rocker SYM=DOWN Pushed", "Fired when rocker SYM=DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerSYMDownTrigger);

			_rockerCONUpTrigger = new HeliosTrigger(this, "", "", "Rocker CON-UP Pushed", "Fired when rocker CON-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerCONUpTrigger);

			_rockerCONDownTrigger = new HeliosTrigger(this, "", "", "Rocker CON=DOWN Pushed", "Fired when rocker CON=DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerCONDownTrigger);
		}

		#endregion Actions

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
					 _inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetMFD();
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
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_backplate.Image = _backplateDimImage;
				SetButtonUpImage();
			}
			else if (Backlight == 2)
			{
				_backplate.Image = _backplateBrtImage;
				SetButtonUpImage();
			}
			else
			{
				_backplate.Image = _backplateOffImage;
				SetButtonUpImage();
			}

			Refresh();
		}

		private void SetButtonUpImage()
		{
			_buttonOSB01.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB02.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB03.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB04.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB05.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB06.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB07.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB08.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB09.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB10.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB11.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB12.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB13.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB14.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB15.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB16.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB17.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB18.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB19.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
			_buttonOSB20.Image = Backlight == 0 ? _buttonOffImage : _buttonLitImage;
		}

		public override void Reset()
		{
			ResetMFD();
		}

		private void ResetMFD()
		{
			Backlight = 0d;

			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Mouse Actions

		public override void MouseDown(Point location)
		{
			Point point = new Point(location.X * _rectBase.Width / Width, location.Y * _rectBase.Height / Height);

			if (_rectButtonOSB01.Contains(point))
			{
				_buttonOSB01.Image = _buttonPressImage;
				_buttonOSB01Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB02.Contains(point))
			{
				_buttonOSB02.Image = _buttonPressImage;
				_buttonOSB02Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB03.Contains(point))
			{
				_buttonOSB03.Image = _buttonPressImage;
				_buttonOSB03Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB04.Contains(point))
			{
				_buttonOSB04.Image = _buttonPressImage;
				_buttonOSB04Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB05.Contains(point))
			{
				_buttonOSB05.Image = _buttonPressImage;
				_buttonOSB05Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB06.Contains(point))
			{
				_buttonOSB06.Image = _buttonPressImage;
				_buttonOSB06Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB07.Contains(point))
			{
				_buttonOSB07.Image = _buttonPressImage;
				_buttonOSB07Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB08.Contains(point))
			{
				_buttonOSB08.Image = _buttonPressImage;
				_buttonOSB08Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB09.Contains(point))
			{
				_buttonOSB09.Image = _buttonPressImage;
				_buttonOSB09Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB10.Contains(point))
			{
				_buttonOSB10.Image = _buttonPressImage;
				_buttonOSB10Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB11.Contains(point))
			{
				_buttonOSB11.Image = _buttonPressImage;
				_buttonOSB11Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB12.Contains(point))
			{
				_buttonOSB12.Image = _buttonPressImage;
				_buttonOSB12Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB13.Contains(point))
			{
				_buttonOSB13.Image = _buttonPressImage;
				_buttonOSB13Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB14.Contains(point))
			{
				_buttonOSB14.Image = _buttonPressImage;
				_buttonOSB14Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB15.Contains(point))
			{
				_buttonOSB15.Image = _buttonPressImage;
				_buttonOSB15Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB16.Contains(point))
			{
				_buttonOSB16.Image = _buttonPressImage;
				_buttonOSB16Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB17.Contains(point))
			{
				_buttonOSB17.Image = _buttonPressImage;
				_buttonOSB17Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB18.Contains(point))
			{
				_buttonOSB18.Image = _buttonPressImage;
				_buttonOSB18Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB19.Contains(point))
			{
				_buttonOSB19.Image = _buttonPressImage;
				_buttonOSB19Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectButtonOSB20.Contains(point))
			{
				_buttonOSB20.Image = _buttonPressImage;
				_buttonOSB20Trigger.FireTrigger(new BindingValue(true));
			}
			if (_rectRockerGAINUp.Contains(point))
			{
				_rockerGAIN.Image = _rockerGAINUpImage;
				_rockerGAINUpTrigger.FireTrigger(new BindingValue(true));
			}
			if (_rectRockerGAINDown.Contains(point))
			{
				_rockerGAIN.Image = _rockerGAINDownImage;
				_rockerGAINDownTrigger.FireTrigger(new BindingValue(true));
			}
			if (_rectRockerSYMUp.Contains(point))
			{
				_rockerSYM.Image = _rockerSYMUpImage;
				_rockerSYMUpTrigger.FireTrigger(new BindingValue(true));
			}
			if (_rectRockerSYMDown.Contains(point))
			{
				_rockerSYM.Image = _rockerSYMDownImage;
				_rockerSYMDownTrigger.FireTrigger(new BindingValue(true));
			}
			if (_rectRockerBRTUp.Contains(point))
			{
				_rockerBRT.Image = _rockerBRTUpImage;
				_rockerBRTUpTrigger.FireTrigger(new BindingValue(true));
			}
			if (_rectRockerBRTDown.Contains(point))
			{
				_rockerBRT.Image = _rockerBRTDownImage;
				_rockerBRTDownTrigger.FireTrigger(new BindingValue(true));
			}
			if (_rectRockerCONUp.Contains(point))
			{
				_rockerCON.Image = _rockerCONUpImage;
				_rockerCONUpTrigger.FireTrigger(new BindingValue(true));
			}
			if (_rectRockerCONDown.Contains(point))
			{
				_rockerCON.Image = _rockerCONDownImage;
				_rockerCONDownTrigger.FireTrigger(new BindingValue(true));
			}

			Refresh();
		}

		public override void MouseUp(Point location)
		{
			SetButtonUpImage();

			_rockerGAIN.Image = _rockerGAINCenterImage;
			_rockerSYM.Image = _rockerSYMCenterImage;
			_rockerBRT.Image = _rockerBRTCenterImage;
			_rockerCON.Image = _rockerCONCenterImage;

			Refresh();
		}

		#endregion Mouse Actions

		#region Properties

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
