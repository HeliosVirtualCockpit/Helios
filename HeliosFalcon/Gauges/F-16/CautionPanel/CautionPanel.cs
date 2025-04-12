// Copyright 2014 Craig Courtney
//  Copyright 2025 Helios Contributors
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



namespace GadrocsWorkshop.Helios.Gauges.Falcon.CautionPanel
{
	using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;

	[HeliosControl("Helios.Falcon.CautionPanel", "Falcon BMS Caution Panel", "Falcon BMS F-16", typeof(GaugeRenderer))]
	class CautionPanel : BaseGauge
    {
		private FalconInterface _falconInterface;

		private GaugeImage _caution01;
		private GaugeImage _caution02;
		private GaugeImage _caution03;
		private GaugeImage _caution04;
		private GaugeImage _caution05;
		private GaugeImage _caution06;
		private GaugeImage _caution07;
		private GaugeImage _caution08;
		private GaugeImage _caution09;
		private GaugeImage _caution10;
		private GaugeImage _caution11;
		private GaugeImage _caution12;
		private GaugeImage _caution13;
		private GaugeImage _caution14;
		private GaugeImage _caution15;
		private GaugeImage _caution16;
		private GaugeImage _caution17;
		private GaugeImage _caution18;
		private GaugeImage _caution19;
		private GaugeImage _caution20;
		private GaugeImage _caution21;
		private GaugeImage _caution22;
		private GaugeImage _caution23;
		private GaugeImage _caution24;
		private GaugeImage _caution25;
		private GaugeImage _caution26;
		private GaugeImage _caution27;
		private GaugeImage _caution28;
		private GaugeImage _caution29;
		private GaugeImage _caution30;
		private GaugeImage _caution31;
		private GaugeImage _caution32;

        private const string _backplateImage = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution_panel.png";
		private const string _caution01Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution01.png";
		private const string _caution02Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution02.png";
		private const string _caution03Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution03.png";
		private const string _caution04Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution04.png";
		private const string _caution05Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution05.png";
		private const string _caution06Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution06.png";
		private const string _caution07Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution07.png";
		private const string _caution08Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution08.png";
		private const string _caution09Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution09.png";
		private const string _caution10Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution10.png";
		private const string _caution11Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution11.png";
		private const string _caution12Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution12.png";
		private const string _caution13Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution13.png";
		private const string _caution14Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution14.png";
		private const string _caution15Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution15.png";
		private const string _caution16Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution16.png";
		private const string _caution17Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution17.png";
		private const string _caution18Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution18.png";
		private const string _caution19Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution19.png";
		private const string _caution20Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution20.png";
		private const string _caution21Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution21.png";
		private const string _caution22Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution22.png";
		private const string _caution23Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution23.png";
		private const string _caution24Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution24.png";
		private const string _caution25Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution25.png";
		private const string _caution26Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution26.png";
		private const string _caution27Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution27.png";
		private const string _caution28Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution28.png";
		private const string _caution29Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution29.png";
		private const string _caution30Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution30.png";
		private const string _caution31Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution31.png";
		private const string _caution32Image = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution32.png";

		private static Rect _rectBase = new Rect(0d, 0d, 200, 156);

		private static Rect _rectCaution01 = new Rect(8d, 6d, 46d,18d);
		private static Rect _rectCaution02 = new Rect(54d, 6d, 46d, 18d);
		private static Rect _rectCaution03 = new Rect(100d, 6d, 46d, 18d);
		private static Rect _rectCaution04 = new Rect(146d, 6d, 46d, 18d);
		private static Rect _rectCaution05 = new Rect(8d, 24d, 46d, 18d);
		private static Rect _rectCaution06 = new Rect(54d, 24d, 46d, 18d);
		private static Rect _rectCaution07 = new Rect(100d, 24d, 46d, 18d);
		private static Rect _rectCaution08 = new Rect(146d, 24d, 46d, 18d);
		private static Rect _rectCaution09 = new Rect(8d, 42d, 46d, 18d);
		private static Rect _rectCaution10 = new Rect(54d, 42d, 46d, 18d);
		private static Rect _rectCaution11 = new Rect(100d, 42d, 46d, 18d);
		private static Rect _rectCaution12 = new Rect(146d, 42d, 46d, 18d);
		private static Rect _rectCaution13 = new Rect(8d, 60d, 46d, 18d);
		private static Rect _rectCaution14 = new Rect(54d, 60d, 46d, 18d);
		private static Rect _rectCaution15 = new Rect(100d, 60d, 46d, 18d);
		private static Rect _rectCaution16 = new Rect(146d, 60d, 46d, 18d);
		private static Rect _rectCaution17 = new Rect(8d, 78d, 46d, 18d);
		private static Rect _rectCaution18 = new Rect(54d, 78d, 46d, 18d);
		private static Rect _rectCaution19 = new Rect(100d, 78d, 46d, 18d);
		private static Rect _rectCaution20 = new Rect(146d, 78d, 46d, 18d);
		private static Rect _rectCaution21 = new Rect(8d, 96d, 46d, 18d);
		private static Rect _rectCaution22 = new Rect(54d, 96d, 46d, 18d);
		private static Rect _rectCaution23 = new Rect(100d, 96d, 46d, 18d);
		private static Rect _rectCaution24 = new Rect(146d, 96d, 46d, 18d);
		private static Rect _rectCaution25 = new Rect(8d, 114d, 46d, 18d);
		private static Rect _rectCaution26 = new Rect(54d, 114d, 46d, 18d);
		private static Rect _rectCaution27 = new Rect(100d, 114d, 46d, 18d);
		private static Rect _rectCaution28 = new Rect(146d, 114d, 46d, 18d);
		private static Rect _rectCaution29 = new Rect(8d, 132d, 46d, 18d);
		private static Rect _rectCaution30 = new Rect(54d, 132d, 46d, 18d);
		private static Rect _rectCaution31 = new Rect(100d, 132d, 46d, 18d);
		private static Rect _rectCaution32 = new Rect(146d, 132d, 46d, 18d);

        private bool _inFlightLastValue;

        public CautionPanel()
			: base("CautionPanel", new Size(_rectBase.Width,_rectBase.Height))
        {
			AddComponents();
        }

        #region Components
		private void AddComponents()
        {
			Components.Add(new GaugeImage(_backplateImage, _rectBase));

			_caution01 = new GaugeImage(_caution01Image, _rectCaution01)
			{
				IsHidden = true
			};
			Components.Add(_caution01);

			_caution02 = new GaugeImage(_caution02Image, _rectCaution02)
			{
				IsHidden = true
			};
			Components.Add(_caution02);
			
			_caution03 = new GaugeImage(_caution03Image, _rectCaution03)
			{
				IsHidden = true
			};
			Components.Add(_caution03);
			
			_caution04 = new GaugeImage(_caution04Image, _rectCaution04)
			{
				IsHidden = true
			};
			Components.Add(_caution04);
			
			_caution05 = new GaugeImage(_caution05Image, _rectCaution05)
			{
				IsHidden = true
			};
			Components.Add(_caution05);
			
			_caution06 = new GaugeImage(_caution06Image, _rectCaution06)
			{
				IsHidden = true
			};
			Components.Add(_caution06);
			
			_caution07 = new GaugeImage(_caution07Image, _rectCaution07)
			{
				IsHidden = true
			};
			Components.Add(_caution07);
			
			_caution08 = new GaugeImage(_caution08Image, _rectCaution08)
			{
				IsHidden = true
			};
			Components.Add(_caution08);

            _caution09 = new GaugeImage(_caution09Image, _rectCaution09)
            { 
				IsHidden = true
			};
			Components.Add(_caution09);

			_caution10 = new GaugeImage(_caution10Image, _rectCaution10)
			{
				IsHidden = true
			};
			Components.Add(_caution10);

			_caution11 = new GaugeImage(_caution11Image, _rectCaution11)
			{
				IsHidden = true
			};
			Components.Add(_caution11);

			_caution12 = new GaugeImage(_caution12Image, _rectCaution12)
			{
				IsHidden = true
			};
			Components.Add(_caution12);

			_caution13 = new GaugeImage(_caution13Image, _rectCaution13)
			{
				IsHidden = true
			};
			Components.Add(_caution13);

			_caution14 = new GaugeImage(_caution14Image, _rectCaution14) 
			{ 
				IsHidden = true 
			};
			Components.Add(_caution14);

			_caution15 = new GaugeImage(_caution15Image, _rectCaution15)
			{
				IsHidden = true
			};
			Components.Add(_caution15);

			_caution16 = new GaugeImage(_caution16Image, _rectCaution16)
			{
				IsHidden = true
			};
			Components.Add(_caution16);

			_caution17 = new GaugeImage(_caution17Image, _rectCaution17)
			{
				IsHidden = true
			};
			Components.Add(_caution17);

			_caution18 = new GaugeImage(_caution18Image, _rectCaution18)
			{
				IsHidden = true
			};
			Components.Add(_caution18);

			_caution19 = new GaugeImage(_caution19Image, _rectCaution19)
			{
				IsHidden = true
			};
			Components.Add(_caution19);

			_caution20 = new GaugeImage(_caution20Image, _rectCaution20)
			{
				IsHidden = true
			};
			Components.Add(_caution20);

			_caution21 = new GaugeImage(_caution21Image, _rectCaution21)
			{
				IsHidden = true
			};
			Components.Add(_caution21);

			_caution22 = new GaugeImage(_caution22Image, _rectCaution22)
			{
				IsHidden = true
			};
			Components.Add(_caution22);

			_caution23 = new GaugeImage(_caution23Image, _rectCaution23)
			{
				IsHidden = true
			};
			Components.Add(_caution23);

			_caution24 = new GaugeImage(_caution24Image, _rectCaution24)
			{
				IsHidden = true
			};
			Components.Add(_caution24);

			_caution25 = new GaugeImage(_caution25Image, _rectCaution25)
			{
				IsHidden = true
			};
			Components.Add(_caution25);

			_caution26 = new GaugeImage(_caution26Image, _rectCaution26)
			{
				IsHidden = true
			};
			Components.Add(_caution26);

			_caution27 = new GaugeImage(_caution27Image, _rectCaution27)
			{
				IsHidden = true
			};
			Components.Add(_caution27);

			_caution28 = new GaugeImage(_caution28Image, _rectCaution28)
			{
				IsHidden = true
			};
			Components.Add(_caution28);

			_caution29 = new GaugeImage(_caution29Image, _rectCaution29)
			{
				IsHidden = true
			};
			Components.Add(_caution29);

			_caution30 = new GaugeImage(_caution30Image, _rectCaution30)
			{
				IsHidden = true
			};
			Components.Add(_caution30);

			_caution31 = new GaugeImage(_caution31Image, _rectCaution31)
			{
				IsHidden = true
			};
			Components.Add(_caution31);

			_caution32 = new GaugeImage(_caution32Image, _rectCaution32)
			{
				IsHidden = true
			};
			Components.Add(_caution32);
        }

        #endregion

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
						ResetCautionPanel();
						_inFlightLastValue = false;
					}
				}
			}
		}

		private void Profile_ProfileStopped(object sender, EventArgs e)
		{
			_falconInterface = null;
		}


		public override void Reset()
		{
			ResetCautionPanel();
		}

		private void ResetCautionPanel()
		{
			ProcessBindingValues();

			
		}

		private void ProcessBindingValues()
		{
			// If the Getvalue is true then make the control not hidden
			_caution01.IsHidden = GetValue("Caution", "flight control system indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution02.IsHidden = GetValue("Caution", "engine fault indticator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution03.IsHidden = GetValue("Caution", "avionics indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution04.IsHidden = GetValue("Caution", "seat arm indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution05.IsHidden = GetValue("Caution", "electric bus fail indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution06.IsHidden = GetValue("Caution", "second engine compressor indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution07.IsHidden = GetValue("Caution", "equip hot indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution08.IsHidden = GetValue("Caution", "nws fail indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution09.IsHidden = GetValue("Caution", "probe heat indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution10.IsHidden = GetValue("Caution", "fuel oil hot indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution11.IsHidden = GetValue("Caution", "radar altimeter indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution12.IsHidden = GetValue("Caution", "anti skid indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution13.IsHidden = GetValue("Caution", "cadc indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution14.IsHidden = GetValue("Caution", "Inlet Icing indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution15.IsHidden = GetValue("Caution", "iff indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution16.IsHidden = GetValue("Caution", "hook indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution17.IsHidden = GetValue("Caution", "stores config indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution18.IsHidden = GetValue("Caution", "overheat indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			
			_caution20.IsHidden = GetValue("Caution", "oxygen low indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution21.IsHidden = GetValue("Caution", "atf not engaged").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			
			_caution23.IsHidden = GetValue("Caution", "ecm indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution24.IsHidden = GetValue("Caution", "cabin pressure indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution25.IsHidden = GetValue("Caution", "forward fuel low indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution26.IsHidden = GetValue("Caution", "backup fuel control indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			
			_caution29.IsHidden = GetValue("Caution", "aft fuel low indicator").BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;

			// Only will illuminate if MAL/IND is pressed, otherwise there are no lightbits for these indicators
			_caution19.IsHidden = GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution22.IsHidden = GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution27.IsHidden = GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution28.IsHidden = GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution30.IsHidden = GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution31.IsHidden = GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
			_caution32.IsHidden = GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
		}

        private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

        #endregion Methods

    }
}
