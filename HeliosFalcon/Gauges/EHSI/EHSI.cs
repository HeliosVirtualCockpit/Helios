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

namespace GadrocsWorkshop.Helios.Controls
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;


	[HeliosControl("Helios.Falcon.EHSI", "Falcon EHSI", "Falcon Simulator", typeof(Gauges.GaugeRenderer))]

	public class EHSI : Gauges.BaseGauge
	{
		private FalconInterface _falconInterface;

		private Gauges.GaugeImage _BackgroundBaseImage;
		private Gauges.GaugeImage _BackgroundMarkerImage;
		private Gauges.GaugeImage _AircraftImage;
		private Gauges.CustomGaugeNeedle _CurrentHeadingScale;
		private Gauges.CustomGaugeNeedle _DesiredHeadingMarker;
		private Gauges.CustomGaugeNeedle _ILSIndicator;
		private Gauges.CustomGaugeNeedle _ILSToFlagIndicator;
		private Gauges.CustomGaugeNeedle _ILSFromFlagIndicator;
		private Gauges.CustomGaugeNeedle _BeaconBearingNeedle;
		private Gauges.CustomGaugeNeedle _OuterMarkerNeedle;
		private Gauges.CustomGaugeNeedle _MiddleMarkerNeedle;
		private Gauges.CustomGaugeNeedle _DesiredCourseNeedle;
		private Gauges.CustomGaugeNeedle _CourseDeviationNeedle;
		private EHSIRenderer _TextData;

		private Rect _imageSize = new Rect(0, 0, 400, 400);
		private Size _needleSize = new Size(400, 400);
		private Rect _needleClip = new Rect(0, 0, 400, 400);
		private Point _needleLocation = new Point(0, 0);
		private Point _needleCenter = new Point(200, 200);

		private const string _imageBackgroundBaseImage = "{HeliosFalcon}/Images/EHSI/Background Base Image.png";
		private const string _imageBackgroundMarkerImage = "{HeliosFalcon}/Images/EHSI/Background Marker Image.png";
		private const string _imageAircraftImage = "{HeliosFalcon}/Images/EHSI/Aircraft Image.png";
		private const string _imageCurrentHeadingScale = "{HeliosFalcon}/Images/EHSI/Current Heading Scale.png";
		private const string _imageDesiredHeadingMarker = "{HeliosFalcon}/Images/EHSI/Desired Heading Marker.png";
		private const string _imageILSIndicator = "{HeliosFalcon}/Images/EHSI/ILS Indicator.png";
		private const string _imageILSToFlagIndicator = "{HeliosFalcon}/Images/EHSI/ILS To Flag Indicator.png";
		private const string _imageILSFromFlagIndicator = "{HeliosFalcon}/Images/EHSI/ILS From Flag Indicator.png";
		private const string _imageBeaconBearingNeedle = "{HeliosFalcon}/Images/EHSI/Beacon Bearing Needle.png";
		private const string _imageOuterMarkerNeedle = "{HeliosFalcon}/Images/EHSI/Outer Marker Needle.png";
		private const string _imageMiddleMarkerNeedle = "{HeliosFalcon}/Images/EHSI/Middle Marker Needle.png";
		private const string _imageDesiredCourseNeedle = "{HeliosFalcon}/Images/EHSI/Desired Course Needle.png";
		private const string _imageCourseDeviationNeedle = "{HeliosFalcon}/Images/EHSI/Course Deviation Needle.png";

		private double _deviationScaleFactor;
		private bool _inFlightLastValue = true;


		public EHSI()
			: base("EHSI", new Size(400, 400))
		{
			AddComponents();
			ControlStaticResize();
			Resized += new EventHandler(OnControl_Resized);
		}


		#region Components

		void AddComponents()
		{
			_BackgroundBaseImage = new Gauges.GaugeImage(_imageBackgroundBaseImage, _imageSize);
			_BackgroundBaseImage.Clip = new RectangleGeometry(_needleClip);
			_BackgroundBaseImage.IsHidden = false;
			Components.Add(_BackgroundBaseImage);

			_BackgroundMarkerImage = new Gauges.GaugeImage(_imageBackgroundMarkerImage, _imageSize);
			_BackgroundMarkerImage.Clip = new RectangleGeometry(_needleClip);
			_BackgroundMarkerImage.IsHidden = false;
			Components.Add(_BackgroundMarkerImage);

			_CurrentHeadingScale = new Gauges.CustomGaugeNeedle(_imageCurrentHeadingScale, _needleLocation, _needleSize, _needleCenter);
			_CurrentHeadingScale.Clip = new RectangleGeometry(_needleClip);
			_CurrentHeadingScale.IsHidden = false;
			Components.Add(_CurrentHeadingScale);

			_TextData = new EHSIRenderer();
			_TextData.Clip = new RectangleGeometry(_needleClip);
			_TextData.IsHidden = false;
			Components.Add(_TextData);

			_DesiredHeadingMarker = new Gauges.CustomGaugeNeedle(_imageDesiredHeadingMarker, _needleLocation, _needleSize, _needleCenter);
			_DesiredHeadingMarker.Clip = new RectangleGeometry(_needleClip);
			_DesiredHeadingMarker.IsHidden = false;
			Components.Add(_DesiredHeadingMarker);

			_ILSIndicator = new Gauges.CustomGaugeNeedle(_imageILSIndicator, _needleLocation, _needleSize, _needleCenter);
			_ILSIndicator.Clip = new RectangleGeometry(_needleClip);
			_ILSIndicator.IsHidden = true;
			Components.Add(_ILSIndicator);

			_ILSToFlagIndicator = new Gauges.CustomGaugeNeedle(_imageILSToFlagIndicator, _needleLocation, _needleSize, _needleCenter);
			_ILSToFlagIndicator.Clip = new RectangleGeometry(_needleClip);
			_ILSToFlagIndicator.IsHidden = true;
			Components.Add(_ILSToFlagIndicator);

			_ILSFromFlagIndicator = new Gauges.CustomGaugeNeedle(_imageILSFromFlagIndicator, _needleLocation, _needleSize, _needleCenter);
			_ILSFromFlagIndicator.Clip = new RectangleGeometry(_needleClip);
			_ILSFromFlagIndicator.IsHidden = true;
			Components.Add(_ILSFromFlagIndicator);

			_AircraftImage = new Gauges.GaugeImage(_imageAircraftImage, _imageSize);
			_AircraftImage.Clip = new RectangleGeometry(_needleClip);
			_AircraftImage.IsHidden = false;
			Components.Add(_AircraftImage);

			_BeaconBearingNeedle = new Gauges.CustomGaugeNeedle(_imageBeaconBearingNeedle, _needleLocation, _needleSize, _needleCenter);
			_BeaconBearingNeedle.Clip = new RectangleGeometry(_needleClip);
			_BeaconBearingNeedle.IsHidden = false;
			Components.Add(_BeaconBearingNeedle);

			_OuterMarkerNeedle = new Gauges.CustomGaugeNeedle(_imageOuterMarkerNeedle, _needleLocation, _needleSize, _needleCenter);
			_OuterMarkerNeedle.Clip = new RectangleGeometry(_needleClip);
			_OuterMarkerNeedle.IsHidden = true;
			Components.Add(_OuterMarkerNeedle);

			_MiddleMarkerNeedle = new Gauges.CustomGaugeNeedle(_imageMiddleMarkerNeedle, _needleLocation, _needleSize, _needleCenter);
			_MiddleMarkerNeedle.Clip = new RectangleGeometry(_needleClip);
			_MiddleMarkerNeedle.IsHidden = true;
			Components.Add(_MiddleMarkerNeedle);

			_DesiredCourseNeedle = new Gauges.CustomGaugeNeedle(_imageDesiredCourseNeedle, _needleLocation, _needleSize, _needleCenter);
			_DesiredCourseNeedle.Clip = new RectangleGeometry(_needleClip);
			_DesiredCourseNeedle.IsHidden = false;
			Components.Add(_DesiredCourseNeedle);

			_CourseDeviationNeedle = new Gauges.CustomGaugeNeedle(_imageCourseDeviationNeedle, _needleLocation, _needleSize, _needleCenter);
			_CourseDeviationNeedle.Clip = new RectangleGeometry(_needleClip);
			_CourseDeviationNeedle.IsHidden = false;
			Components.Add(_CourseDeviationNeedle);
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

		void Profile_ProfileStarted(object sender, EventArgs e)
		{
			if (Parent.Profile.Interfaces.ContainsKey("Falcon"))
			{
				_falconInterface = Parent.Profile.Interfaces["Falcon"] as FalconInterface;
			}
		}

		void Profile_ProfileTick(object sender, EventArgs e)
		{
			if (_falconInterface != null)
			{
				BindingValue runtimeFlying = GetValue("Runtime", "Flying");
				bool inFlight = runtimeFlying.BoolValue;

				if (inFlight)
				{
					GetDataValues();
					ProcessDataValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetDataValues();
						_inFlightLastValue = false;
					}
				}
			}
		}

		void Profile_ProfileStopped(object sender, EventArgs e)
		{
			_falconInterface = null;
		}

		void GetDataValues()
		{
			BindingValue hsiOffFlag = GetValue("HSI", "off flag");
			HSIOffFlag = hsiOffFlag.BoolValue;

			BindingValue ilsToFlag = GetValue("HSI", "to flag");
			ILSToFlag = ilsToFlag.BoolValue;

			BindingValue ilsFromFlag = GetValue("HSI", "from flag");
			ILSFromFlag = ilsFromFlag.BoolValue;

			BindingValue ilsWarningFlag = GetValue("HSI", "ils warning flag");
			ILSWarningFlag = ilsWarningFlag.BoolValue;

			BindingValue desiredCourseCalculated = GetValue("HSI", "desired course calculated");
			DesiredCourseCalculated = desiredCourseCalculated.DoubleValue;

			BindingValue desiredHeadingCalculated = GetValue("HSI", "desired heading calculated");
			DesiredHeadingCalculated = desiredHeadingCalculated.DoubleValue;

			BindingValue bearingToBeaconCalculated = GetValue("HSI", "bearing to beacon calculated");
			BearingToBeaconCalculated = bearingToBeaconCalculated.DoubleValue;

			BindingValue beaconBearing = GetValue("HSI", "bearing to beacon");
			BeaconBearing = beaconBearing.DoubleValue;

			BindingValue desiredCourse = GetValue("HSI", "desired course");
			DesiredCourse = desiredCourse.DoubleValue;

			BindingValue currentHeading = GetValue("HSI", "current heading");
			CurrentHeading = currentHeading.DoubleValue;

			BindingValue beaconDistance = GetValue("HSI", "distance to beacon");
			BeaconDistance = beaconDistance.DoubleValue;

			BindingValue desiredHeading = GetValue("HSI", "desired heading");
			DesiredHeading = desiredHeading.DoubleValue;

			BindingValue courseDeviation = GetValue("HSI", "course deviation");
			CourseDeviation = courseDeviation.DoubleValue;

			BindingValue navMode = GetValue("HSI", "nav mode");
			_TextData.NavMode = navMode.DoubleValue;

			BindingValue tacanBand = GetValue("Tacan", "ufc tacan band");
			_TextData.TacanBand = tacanBand.DoubleValue;

			BindingValue tacanMode = GetValue("Tacan", "ufc tacan mode");
			_TextData.TacanMode = tacanMode.DoubleValue;

			BindingValue tacanChannel = GetValue("Tacan", "ufc tacan chan");
			_TextData.TacanChannel = tacanChannel.DoubleValue;

			BindingValue outerMarker = GetValue("HSI", "Outer marker indicator");
			OuterMarker = outerMarker.BoolValue;

			BindingValue middleMarker = GetValue("HSI", "Middle marker indicator");
			MiddleMarker = middleMarker.BoolValue;

			_TextData.BeaconDistance = BeaconDistance;
			_TextData.DesiredCourse = DesiredCourse;
			_TextData.DesiredHeading = DesiredHeading;
		}

		void ProcessDataValues()
		{
			bool flash2Hz = DateTime.Now.Millisecond % 500 < 250;
			bool flash4Hz = DateTime.Now.Millisecond % 250 < 125;

			_CurrentHeadingScale.Rotation = 360 - CurrentHeading;

			_DesiredHeadingMarker.Rotation = DesiredHeadingCalculated;

			_DesiredCourseNeedle.Rotation = DesiredCourseCalculated;
			_CourseDeviationNeedle.Rotation = DesiredCourseCalculated;
			_ILSIndicator.Rotation = DesiredCourseCalculated;
			_ILSToFlagIndicator.Rotation = DesiredCourseCalculated;
			_ILSFromFlagIndicator.Rotation = DesiredCourseCalculated;

			_BeaconBearingNeedle.Rotation = BearingToBeaconCalculated;
			_OuterMarkerNeedle.Rotation = BearingToBeaconCalculated;
			_MiddleMarkerNeedle.Rotation = BearingToBeaconCalculated;

			_CourseDeviationNeedle.HorizontalOffset = CourseDeviationScaleValue(CourseDeviation);

			_ILSIndicator.IsHidden = !ILSWarningFlag;
			_ILSToFlagIndicator.IsHidden = !ILSToFlag;
			_ILSFromFlagIndicator.IsHidden = !ILSFromFlag;
			
			_OuterMarkerNeedle.IsHidden = !(OuterMarker && flash2Hz);
			_MiddleMarkerNeedle.IsHidden = !(MiddleMarker && flash4Hz);
		}

		public override void Reset()
		{
			ResetDataValues();
		}

		void ResetDataValues()
		{
			HSIOffFlag = true;
			ILSToFlag = false;
			ILSFromFlag = false;
			ILSWarningFlag = false;
			OuterMarker = false;
			MiddleMarker = false;

			DesiredCourseCalculated = 0;
			DesiredHeadingCalculated = 0;
			BearingToBeaconCalculated = 0;
			BeaconBearing = 0;
			DesiredCourse = 0;
			CurrentHeading = 0;
			BeaconDistance = 0;
			DesiredHeading = 0;
			CourseDeviation = 0;

			_TextData.NavMode = 2;
			_TextData.BeaconDistance = 0;
			_TextData.DesiredCourse = 0;
			_TextData.DesiredHeading = 0;
			_TextData.TacanChannel = 0;
			_TextData.TacanBand = 0;
			_TextData.TacanMode = 0;
		}

		#endregion Methods


		#region Functions

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		double CourseDeviationScaleValue(double deviation)
		{
			if (deviation >= 1)
			{
				deviation = 1;
			}
			else if (deviation <= -1)
			{
				deviation = -1;
			}

			return - (deviation * _deviationScaleFactor * _needleSize.Width / 4.7);
		}

		#endregion Functions


		#region Scaling

		void OnControl_Resized(object sender, EventArgs e)
		{
			ControlStaticResize();
		}

		void ControlStaticResize()
		{
			double _squareWidth;
			double _squareHeight;
			double _squarePosX;
			double _squarePosY;

			double ratioHeightToWidth = Height / Width;
			double ratioWidthToHeight = Width / Height;

			if (Height >= Width)
			{
				_deviationScaleFactor = 1;

				_squareWidth = _needleSize.Width;
				_squareHeight = _needleSize.Height * ratioWidthToHeight;
				_squarePosX = 0;
				_squarePosY = _needleSize.Height * (1 - ratioWidthToHeight) / 2;
			}
			else
			{
				_deviationScaleFactor = ratioHeightToWidth;

				_squareWidth = _needleSize.Width * ratioHeightToWidth;
				_squareHeight = _needleSize.Height;
				_squarePosX = _needleSize.Width * (1 - ratioHeightToWidth) / 2;
				_squarePosY = 0;
			}

			_BackgroundMarkerImage.Width = _squareWidth;
			_BackgroundMarkerImage.Height = _squareHeight;
			_BackgroundMarkerImage.PosX = _squarePosX;
			_BackgroundMarkerImage.PosY = _squarePosY;

			_CurrentHeadingScale.Tape_Width = _squareWidth;
			_CurrentHeadingScale.Tape_Height = _squareHeight;
			_CurrentHeadingScale.TapePosX = _squarePosX;
			_CurrentHeadingScale.TapePosY = _squarePosY;

			_DesiredHeadingMarker.Tape_Width = _squareWidth;
			_DesiredHeadingMarker.Tape_Height = _squareHeight;
			_DesiredHeadingMarker.TapePosX = _squarePosX;
			_DesiredHeadingMarker.TapePosY = _squarePosY;

			_ILSIndicator.Tape_Width = _squareWidth;
			_ILSIndicator.Tape_Height = _squareHeight;
			_ILSIndicator.TapePosX = _squarePosX;
			_ILSIndicator.TapePosY = _squarePosY;

			_ILSToFlagIndicator.Tape_Width = _squareWidth;
			_ILSToFlagIndicator.Tape_Height = _squareHeight;
			_ILSToFlagIndicator.TapePosX = _squarePosX;
			_ILSToFlagIndicator.TapePosY = _squarePosY;

			_ILSFromFlagIndicator.Tape_Width = _squareWidth;
			_ILSFromFlagIndicator.Tape_Height = _squareHeight;
			_ILSFromFlagIndicator.TapePosX = _squarePosX;
			_ILSFromFlagIndicator.TapePosY = _squarePosY;

			_AircraftImage.Width = _squareWidth;
			_AircraftImage.Height = _squareHeight;
			_AircraftImage.PosX = _squarePosX;
			_AircraftImage.PosY = _squarePosY;

			_BeaconBearingNeedle.Tape_Width = _squareWidth;
			_BeaconBearingNeedle.Tape_Height = _squareHeight;
			_BeaconBearingNeedle.TapePosX = _squarePosX;
			_BeaconBearingNeedle.TapePosY = _squarePosY;

			_OuterMarkerNeedle.Tape_Width = _squareWidth;
			_OuterMarkerNeedle.Tape_Height = _squareHeight;
			_OuterMarkerNeedle.TapePosX = _squarePosX;
			_OuterMarkerNeedle.TapePosY = _squarePosY;

			_MiddleMarkerNeedle.Tape_Width = _squareWidth;
			_MiddleMarkerNeedle.Tape_Height = _squareHeight;
			_MiddleMarkerNeedle.TapePosX = _squarePosX;
			_MiddleMarkerNeedle.TapePosY = _squarePosY;

			_DesiredCourseNeedle.Tape_Width = _squareWidth;
			_DesiredCourseNeedle.Tape_Height = _squareHeight;
			_DesiredCourseNeedle.TapePosX = _squarePosX;
			_DesiredCourseNeedle.TapePosY = _squarePosY;

			_CourseDeviationNeedle.Tape_Width = _squareWidth;
			_CourseDeviationNeedle.Tape_Height = _squareHeight;
			_CourseDeviationNeedle.TapePosX = _squarePosX;
			_CourseDeviationNeedle.TapePosY = _squarePosY;

			_TextData.ControlWidth = Width;
			_TextData.ControlHeight = Height;

			Refresh();
		}

		#endregion Scaling


		#region Properties

		private bool HSIOffFlag { get; set; }
		private bool ILSToFlag { get; set; }
		private bool ILSFromFlag { get; set; }
		private bool ILSWarningFlag { get; set; }
		private bool OuterMarker { get; set; }
		private bool MiddleMarker { get; set; }

		private double DesiredCourseCalculated { get; set; }
		private double DesiredHeadingCalculated { get; set; }
		private double BearingToBeaconCalculated { get; set; }
		private double BeaconBearing { get; set; }
		private double DesiredCourse { get; set; }
		private double CurrentHeading { get; set; }
		private double BeaconDistance { get; set; }
		private double DesiredHeading { get; set; }
		private double CourseDeviation { get; set; }

		#endregion Properties

	}
}
