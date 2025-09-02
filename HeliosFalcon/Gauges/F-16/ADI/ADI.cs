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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.ADI
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;

	[HeliosControl("Helios.Falcon.ADI", "Falcon BMS ADI", "Falcon BMS F-16", typeof(GaugeRenderer))]
	public class ADI : BaseGauge
	{
		private FalconInterface _falconInterface;

		private GaugeImage _adiBezel;
		private GaugeImage _ballMask;
		private GaugeImage _adiGuides;
		private GaugeImage _adiFaceplate;
		private GaugeImage _auxFlag;
		private GaugeImage _offFlag;
		private GaugeImage _gsFlag;
		private GaugeImage _locFlag;

		private GaugeBall _ball;
		private GaugeNeedle _rollMarkers;
		private GaugeNeedle _slipBall;
		private GaugeNeedle _ilsHorizontalNeedleSolid;
		private GaugeNeedle _ilsHorizontalNeedleDashed;
		private GaugeNeedle _ilsVerticalNeedleSolid;
		private GaugeNeedle _ilsVerticalNeedleDashed;
		private GaugeNeedle _ilsPointer;

		private const string _ballOffImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ball_off.xaml";
		private const string _ballDimImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ball_dim.xaml";
		private const string _ballBrtImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ball_brt.xaml";

		private const string _bezelImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_bezel.png";
		private const string _ballMaskImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ball_mask.png";
		private const string _guidesImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_guides.xaml";
		private const string _ilsNeedleSolidImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ils_needle_solid.xaml";
		private const string _ilsNeedleDashedImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ils_needle_dashed.xaml";

		private const string _rollMarkersOffImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_roll_markers_off.xaml";
		private const string _rollMarkersDimImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_roll_markers_dim.xaml";
		private const string _rollMarkersBrtImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_roll_markers_brt.xaml";

		private const string _slipBallOffImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_slip_ball_off.xaml";
		private const string _slipBallDimImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_slip_ball_dim.xaml";
		private const string _slipBallBrtImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_slip_ball_brt.xaml";

		private const string _auxFlagImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_aux_flag.xaml";
		private const string _offFlagImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_off_flag.xaml";
		private const string _gsFlagImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_gs_flag.xaml";
		private const string _locFlagImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_loc_flag.xaml";

		private const string _faceplateOffImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_faceplate_off.xaml";
		private const string _faceplateDimImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_faceplate_dim.xaml";
		private const string _faceplateBrtImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_faceplate_brt.xaml";

		private const string _ilsPointerOffImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ils_pointer_off.xaml";
		private const string _ilsPointerDimImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ils_pointer_dim.xaml";
		private const string _ilsPointerBrtImage = "{HeliosFalcon}/Gauges/F-16/ADI/adi_ils_pointer_brt.xaml";

		private CalibrationPointCollectionDouble _pitchCalibration;
		private CalibrationPointCollectionDouble _ilsCalibration;
		private CalibrationPointCollectionDouble _slipBallCalibration;

		private double _backlight;
		private bool _inFlightLastValue = true;

		private bool _suppressScale = false;


        public ADI()
			: base("ADI", new Size(350, 350))
		{
			AddComponents();
		}

		#region Components

		private void AddComponents()
		{
			_pitchCalibration = new CalibrationPointCollectionDouble(-360d, -990d, 360d, 990d);
            _ball = new GaugeBall("{F-16C}/Gauges/ADI/Viper-ADI-Ball-2.xaml", new Point(50d, 42d), new Size(250d, 250d), 0d, -90d, 180d, 35d);
			_ball.Clip = new EllipseGeometry(new Point(175d, 165d), 110d, 110d);
			Components.Add(_ball);

			_ballMask = new GaugeImage(_ballMaskImage, new Rect(60d, 50d, 230d, 230d));
			Components.Add(_ballMask);

			_rollMarkers = new GaugeNeedle(_rollMarkersOffImage, new Point(175d, 165d), new Size(50d, 230d), new Point(25d, 115d));
			Components.Add(_rollMarkers);

			_ilsCalibration = new CalibrationPointCollectionDouble(-1d, -55d, 1d, 55d);

			_ilsHorizontalNeedleSolid = new GaugeNeedle(_ilsNeedleSolidImage, new Point(175d, 165d), new Size(190d, 6d), new Point(95d, 3d), 90d);
			_ilsHorizontalNeedleSolid.IsHidden = true;
			Components.Add(_ilsHorizontalNeedleSolid);

			_ilsHorizontalNeedleDashed = new GaugeNeedle(_ilsNeedleDashedImage, new Point(175d, 165d), new Size(190d, 6d), new Point(95d, 3d), 90d);
			_ilsHorizontalNeedleDashed.IsHidden = true;
			Components.Add(_ilsHorizontalNeedleDashed);

			_ilsVerticalNeedleSolid = new GaugeNeedle(_ilsNeedleSolidImage, new Point(175d, 165d), new Size(190d, 6d), new Point(95d, 3d));
			_ilsVerticalNeedleSolid.IsHidden = true;
			Components.Add(_ilsVerticalNeedleSolid);

			_ilsVerticalNeedleDashed = new GaugeNeedle(_ilsNeedleDashedImage, new Point(175d, 165d), new Size(190d, 6d), new Point(95d, 3d));
			_ilsVerticalNeedleDashed.IsHidden = true;
			Components.Add(_ilsVerticalNeedleDashed);

			_adiFaceplate = new GaugeImage(_faceplateOffImage, new Rect(0d, 0d, 350d, 350d));
			Components.Add(_adiFaceplate);

			_adiBezel = new GaugeImage(_bezelImage, new Rect(0d, 0d, 350d, 350d));
			Components.Add(_adiBezel);

			_auxFlag = new GaugeImage(_auxFlagImage, new Rect(0d, 0d, 350d, 350d));
			_auxFlag.IsHidden = true;
			Components.Add(_auxFlag);

			_offFlag = new GaugeImage(_offFlagImage, new Rect(0d, 0d, 350d, 350d));
			_offFlag.IsHidden = true;
			Components.Add(_offFlag);

			_gsFlag = new GaugeImage(_gsFlagImage, new Rect(0d, 0d, 350d, 350d));
			_gsFlag.IsHidden = true;
			Components.Add(_gsFlag);

			_locFlag = new GaugeImage(_locFlagImage, new Rect(0d, 0d, 350d, 350d));
			_locFlag.IsHidden = true;
			Components.Add(_locFlag);

			_ilsPointer = new GaugeNeedle(_ilsPointerOffImage, new Point(36d, 165d), new Size(22d, 22d), new Point(0d, 11.5d));
			Components.Add(_ilsPointer);

			_adiGuides = new GaugeImage(_guidesImage, new Rect(0d, 0d, 350d, 350d));
			Components.Add(_adiGuides);

			_slipBallCalibration = new CalibrationPointCollectionDouble(-1d, -30d, 1d, 30d);

			_slipBall = new GaugeNeedle(_slipBallOffImage, new Point(175d, 305d), new Size(10d, 10d), new Point(5d, 5d));
			Components.Add(_slipBall);
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
					ProcessADIValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetADI();
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

			BindingValue sideslip = GetValue("ADI", "sideslip angle");
			SideSlipAngle = sideslip.DoubleValue;

			BindingValue deviationHorizontal = GetValue("ADI", "ils horizontal");
			ILSDeviationHorizontal = -(deviationHorizontal.DoubleValue + 1d) * 40d;

			BindingValue deviationVertical = GetValue("ADI", "ils vertical");
			ILSDeviationVertical = -(deviationVertical.DoubleValue  + 1d) * 40d;

			BindingValue flagOFF = GetValue("ADI", "off flag");
			FlagOff = flagOFF.BoolValue;

			BindingValue flagAUX = GetValue("ADI", "aux flag");
			FlagAUX = flagAUX.BoolValue;

			BindingValue flagGS = GetValue("ADI", "gs flag");
			FlagGS = flagGS.BoolValue;

			BindingValue flagLOC = GetValue("ADI", "loc flag");
			FlagLOC = flagLOC.BoolValue;

			BindingValue navMode = GetValue("HSI", "nav mode");
			NavMode = navMode.DoubleValue;
		}

		private void ProcessADIValues()
		{
			bool ilsActiveVertical = !(ILSDeviationVertical <= -1 || ILSDeviationVertical >= 1);
			bool ilsActiveHorizontal = !(ILSDeviationHorizontal <= -1 || ILSDeviationHorizontal >= 1);

			if (NavMode == 1 || NavMode == 2 || FlagOff || FlagGS || FlagLOC || !(ilsActiveVertical || ilsActiveHorizontal))
			{
				_ilsVerticalNeedleSolid.IsHidden = true;
				_ilsVerticalNeedleDashed.IsHidden = true;
				_ilsHorizontalNeedleSolid.IsHidden = true;
				_ilsHorizontalNeedleDashed.IsHidden = true;

				_ilsPointer.VerticalOffset = 0d;
			}
			else
			{
				_ilsVerticalNeedleSolid.IsHidden = !ilsActiveVertical;
				_ilsVerticalNeedleDashed.IsHidden = ilsActiveVertical;
				_ilsHorizontalNeedleSolid.IsHidden = !ilsActiveHorizontal;
				_ilsHorizontalNeedleDashed.IsHidden = ilsActiveHorizontal;

				_ilsPointer.VerticalOffset = _ilsCalibration.Interpolate(ILSDeviationVertical);
				_ilsVerticalNeedleSolid.VerticalOffset = _ilsCalibration.Interpolate(ILSDeviationVertical + PitchAngle / 17);
				_ilsVerticalNeedleDashed.VerticalOffset = _ilsCalibration.Interpolate(ILSDeviationVertical + PitchAngle / 17);
				_ilsHorizontalNeedleSolid.VerticalOffset = _ilsCalibration.Interpolate(ILSDeviationHorizontal);
				_ilsHorizontalNeedleDashed.VerticalOffset = _ilsCalibration.Interpolate(ILSDeviationHorizontal);
			}

			_ball.Yaw = PitchAngle;
			_ball.Roll = -RollAngle;
			_rollMarkers.Rotation = -RollAngle;
			_slipBall.HorizontalOffset = _slipBallCalibration.Interpolate(SideSlipAngle);

			_offFlag.IsHidden = !FlagOff;
			_auxFlag.IsHidden = !FlagAUX;
			_gsFlag.IsHidden = !FlagGS;
			_locFlag.IsHidden = !FlagLOC;
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_adiFaceplate.Image = _faceplateDimImage;
				_rollMarkers.Image = _rollMarkersDimImage;
				_slipBall.Image = _slipBallDimImage;
				_ilsPointer.Image = _ilsPointerDimImage;
				_ball.Image = _ballDimImage;
			}
			else if (Backlight == 2)
			{
				_adiFaceplate.Image = _faceplateBrtImage;
				_rollMarkers.Image = _rollMarkersBrtImage;
				_slipBall.Image = _slipBallBrtImage;
				_ilsPointer.Image = _ilsPointerBrtImage;
				_ball.Image = _ballBrtImage;
			}
			else
			{
				_adiFaceplate.Image = _faceplateOffImage;
				_rollMarkers.Image = _rollMarkersOffImage;
				_slipBall.Image = _slipBallOffImage;
				_ilsPointer.Image = _ilsPointerOffImage;
				_ball.Image = _ballOffImage;
			}

			Refresh();
		}

		public override void Reset()
		{
			ResetADI();
		}

		private void ResetADI()
		{
			Backlight = 0d;

			PitchAngle = 0d;
			RollAngle = 0d;
			SideSlipAngle = 0d;
			ILSDeviationVertical = -1d;
			ILSDeviationHorizontal = -1d;
			NavMode = 2d;

			FlagOff = false;
			FlagAUX = false;
			FlagGS = false;
			FlagLOC = false;

			ProcessADIValues();
			ProcessBacklightValues();
		}
        public override void ScaleChildren(double scaleX, double scaleY)
        {
			if (!_suppressScale)
			{
                _ball.ScaleChildren(scaleX, scaleY);
				_suppressScale = false;
            }
            base.ScaleChildren(scaleX, scaleY);
        }
        protected override void PostUpdateRectangle(Rect previous, Rect current)
        {
            _suppressScale = false;
            if (!previous.Equals(new Rect(0, 0, 0, 0)) && !(previous.Width == current.Width && previous.Height == current.Height))
            {
                _ball.ScaleChildren(current.Width / previous.Width, current.Height / previous.Height);
				_suppressScale = true;
            }
        }

        private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Properties

		private double PitchAngle { get; set; }
		private double RollAngle { get; set; }
		private double SideSlipAngle { get; set; }
		private double ILSDeviationVertical { get; set; } = -1d;
		private double ILSDeviationHorizontal { get; set; } = -1d;
		private double NavMode { get; set; }

		private bool FlagOff { get; set; }
		private bool FlagAUX { get; set; }
		private bool FlagGS { get; set; }
		private bool FlagLOC { get; set; }

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
