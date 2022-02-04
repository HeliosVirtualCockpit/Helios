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
	using static GadrocsWorkshop.Helios.Controls.MapControls;
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Windows;
	using System.Windows.Media;
	using System.Collections.Generic;


	[HeliosControl("Helios.Falcon.MapControlBullseye", "Bullseye Control", "Falcon Simulator", typeof(Gauges.GaugeRenderer))]

	public class MapControlBullseye : Gauges.BaseGauge
	{
		private FalconInterface _falconInterface;

		private List<ITargetData> TargetDataList = new List<ITargetData>();
		private List<Gauges.GaugeImage> OverviewTargetList = new List<Gauges.GaugeImage>();

		private HeliosValue _overviewClearTargets;
		private HeliosValue _overviewRangeRingsVisible;

		private Gauges.GaugeImage _OverviewBackground;
		private Gauges.GaugeImage _OverviewBullseye;
		private Gauges.CustomGaugeNeedle _OverviewRangeRings;
		private Gauges.CustomGaugeNeedle _OverviewAircraft;
		private Gauges.CustomGaugeNeedle _OverviewAircraftRemote;
		private MapControlLineRenderer _OverviewTargetLines;
		private MapControlTextRenderer _OverviewTextData;

		private Rect _imageSize = new Rect(0d, 0d, 200d, 200d);
		private Size _needleSize = new Size(200d, 200d);
		private Rect _needleClip = new Rect(0d, 0d, 200d, 200d);
		private Point _needleLocation = new Point(0d, 0d);
		private Point _needleCenter = new Point(100d, 100d);

		private const string _overviewBullseyeImage = "{HeliosFalcon}/Images/MapControl/Overview Bullseye.png";
		private const string _overviewBackgroundImage = "{HeliosFalcon}/Images/MapControl/Overview Background.png";
		private const string _overviewTargetImage = "{HeliosFalcon}/Images/MapControl/Overview Target.png";
		private const string _overviewRangeRingsImage = "{HeliosFalcon}/Images/MapControl/Overview Range Rings.png";
		private const string _overviewAircraftImage = "{HeliosFalcon}/Images/MapControl/Overview Aircraft.png";
		private const string _overviewAircraftRemoteImage = "{HeliosFalcon}/Images/MapControl/Overview Aircraft Remote.png";

		private const double _mapFeetPerNauticalMile = 6076;
		private const double _targetBullseyeScale = 1.200d * 1.075d;

		private double _scaleFactor;
		private double _squareWidth = 0d;
		private double _squareHeight = 0d;
		private double _squarePosX = 0d;
		private double _squarePosY = 0d;
		private double _widthCenterPosition;
		private double _heightCenterPosition;
		private double _ratioHeightToWidth;
		private double _ratioWidthToHeight;

		private bool _overviewRangeRingsEnabled = true;
		private bool _targetSelected = false;
		private bool _inFlightLastValue = true;
		private bool _inhibitMouseAction = false;


		public MapControlBullseye()
			: base("BullseyeControl", new Size(200d, 200d))
		{
			AddComponents();
			AddActions();
			MapControlStaticResize();
			Resized += new EventHandler(OnMapControl_Resized);
		}


		#region Components

		void AddComponents()
		{
			_OverviewBackground = new Gauges.GaugeImage(_overviewBackgroundImage, _imageSize);
			_OverviewBackground.IsHidden = false;
			Components.Add(_OverviewBackground);

			_OverviewBullseye = new Gauges.GaugeImage(_overviewBullseyeImage, _imageSize);
			_OverviewBullseye.IsHidden = false;
			Components.Add(_OverviewBullseye);

			_OverviewRangeRings = new Gauges.CustomGaugeNeedle(_overviewRangeRingsImage, _needleLocation, _needleSize, _needleCenter);
			_OverviewRangeRings.IsHidden = true;
			_OverviewRangeRings.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y);
			Components.Add(_OverviewRangeRings);

			_OverviewTargetLines = new MapControlLineRenderer();
			_OverviewTargetLines.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y);
			_OverviewTargetLines.IsHidden = true;
			Components.Add(_OverviewTargetLines);

			_OverviewAircraft = new Gauges.CustomGaugeNeedle(_overviewAircraftImage, _needleLocation, _needleSize, _needleCenter);
			_OverviewAircraft.IsHidden = true;
			_OverviewAircraft.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y);
			Components.Add(_OverviewAircraft);

			_OverviewAircraftRemote = new Gauges.CustomGaugeNeedle(_overviewAircraftRemoteImage, _needleLocation, _needleSize, _needleCenter);
			_OverviewAircraftRemote.IsHidden = true;
			Components.Add(_OverviewAircraftRemote);

			_OverviewTextData = new MapControlTextRenderer();
			_OverviewTextData.Clip = new RectangleGeometry(_needleClip);
			_OverviewTextData.IsHidden = false;
			Components.Add(_OverviewTextData);
		}

		#endregion Components


		#region Actions

		void AddActions()
		{
			_overviewClearTargets = new HeliosValue(this, new BindingValue(false), "", "Target Selection Clear", "Clears the selected targets.", "Set true to clear the selected targets.", BindingValueUnits.Boolean);
			_overviewClearTargets.Execute += new HeliosActionHandler(OverviewClearTargets_Execute);
			Actions.Add(_overviewClearTargets);
			Values.Add(_overviewClearTargets);

			_overviewRangeRingsVisible = new HeliosValue(this, new BindingValue(true), "", "Target Selection Range Rings Visible", "Sets visibility of the target selection range rings.", "Set true to show target selection range rings.", BindingValueUnits.Boolean);
			_overviewRangeRingsVisible.Execute += new HeliosActionHandler(OverviewRangeRingsVisible_Execute);
			Actions.Add(_overviewRangeRingsVisible);
			Values.Add(_overviewRangeRingsVisible);
		}

		void OverviewClearTargets_Execute(object action, HeliosActionEventArgs e)
		{
			_overviewClearTargets.SetValue(e.Value, e.BypassCascadingTriggers);
			bool overviewClearTargets = _overviewClearTargets.Value.BoolValue;

			if (overviewClearTargets)
			{
				OverviewClearTargets();
			}
		}

		void OverviewRangeRingsVisible_Execute(object action, HeliosActionEventArgs e)
		{
			_overviewRangeRingsVisible.SetValue(e.Value, e.BypassCascadingTriggers);
			_overviewRangeRingsEnabled = _overviewRangeRingsVisible.Value.BoolValue;

			if (_overviewRangeRingsEnabled)
			{
				_OverviewRangeRings.IsHidden = false;
			}
			else
			{
				_OverviewRangeRings.IsHidden = true;
			}
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

		void Profile_ProfileStarted(object sender, EventArgs e)
		{
			if (Parent.Profile.Interfaces.ContainsKey("Falcon"))
			{
				_falconInterface = Parent.Profile.Interfaces["Falcon"] as FalconInterface;
			}

			ResetTargetOverview();
		}

		void Profile_ProfileTick(object sender, EventArgs e)
		{
			if (_falconInterface != null)
			{
				BindingValue runtimeFlying = GetValue("Runtime", "Flying");
				bool inFlight = runtimeFlying.BoolValue;

				if (inFlight)
				{
					ProcessOwnshipValues();
					ProcessAircraftValues();
					ProcessTargetValues();
					_inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetTargetOverview();
						_inFlightLastValue = false;
					}
				}
			}
		}

		void Profile_ProfileStopped(object sender, EventArgs e)
		{
			_falconInterface = null;

			ResetTargetOverview();
		}

		void ProcessOwnshipValues()
		{
			BindingValue ownshipRotationAngle = GetValue("HSI", "current heading");
			OwnshipRotationAngle = ownshipRotationAngle.DoubleValue;

			BindingValue ownshipHorizontalValue = GetValue("Ownship", "y");
			OwnshipHorizontalValue = ownshipHorizontalValue.DoubleValue;

			BindingValue ownshipVerticalValue = GetValue("Ownship", "x");
			OwnshipVerticalValue = ownshipVerticalValue.DoubleValue;

			BindingValue bullseyeHorizontalValue = GetValue("Ownship", "deltaY from bulls");
			BullseyeHorizontalValue = bullseyeHorizontalValue.DoubleValue;

			BindingValue bullseyeVerticalValue = GetValue("Ownship", "deltaX from bulls");
			BullseyeVerticalValue = bullseyeVerticalValue.DoubleValue;
		}

		private void ProcessAircraftValues()
		{
			double xValue;
			double yValue;
			double xScaleValue;
			double yScaleValue;

			xValue = BullseyeHorizontalValue / _mapFeetPerNauticalMile;
			yValue = BullseyeVerticalValue / _mapFeetPerNauticalMile;

			xScaleValue = xValue / _targetBullseyeScale;
			yScaleValue = -yValue / _targetBullseyeScale;

			if (Height >= Width)
			{
				_OverviewRangeRings.TapePosX = xScaleValue;
				_OverviewRangeRings.TapePosY = yScaleValue * _ratioWidthToHeight + _OverviewBullseye.PosY;

				_OverviewRangeRings.Tape_CenterX = (xScaleValue + _needleCenter.X);
				_OverviewRangeRings.Tape_CenterY = (yScaleValue + _needleCenter.Y) * _ratioWidthToHeight + _OverviewBullseye.PosY;

				_OverviewTargetLines.AircraftPosition_X = xScaleValue * Width / _needleSize.Width + _widthCenterPosition;
				_OverviewTargetLines.AircraftPosition_Y = yScaleValue * Width / _needleSize.Width + _heightCenterPosition;
			}
			else
			{
				_OverviewRangeRings.TapePosX = xScaleValue * _ratioHeightToWidth + _OverviewBullseye.PosX;
				_OverviewRangeRings.TapePosY = yScaleValue;

				_OverviewRangeRings.Tape_CenterX = (xScaleValue + _needleCenter.X) * _ratioHeightToWidth + _OverviewBullseye.PosX;
				_OverviewRangeRings.Tape_CenterY = (yScaleValue + _needleCenter.Y);

				_OverviewTargetLines.AircraftPosition_X = xScaleValue * Height / _needleSize.Height + _widthCenterPosition;
				_OverviewTargetLines.AircraftPosition_Y = yScaleValue * Height / _needleSize.Height + _heightCenterPosition;
			}

			_OverviewAircraft.TapePosX = _OverviewRangeRings.TapePosX;
			_OverviewAircraft.TapePosY = _OverviewRangeRings.TapePosY;

			_OverviewAircraft.Tape_CenterX = _OverviewRangeRings.Tape_CenterX;
			_OverviewAircraft.Tape_CenterY = _OverviewRangeRings.Tape_CenterY;

			AircraftDistance = GetHypotenuse(xValue, yValue);
			AircraftBearing = GetBearing(xValue, yValue);

			_OverviewTextData.AircraftDistance = AircraftDistance;
			_OverviewTextData.AircraftBearing = AircraftBearing;

			_OverviewRangeRings.Rotation = OwnshipRotationAngle;
			_OverviewAircraft.Rotation = OwnshipRotationAngle;
			_OverviewAircraftRemote.Rotation = AircraftBearing;

			if (AircraftDistance <= 120)
			{
				_OverviewAircraft.IsHidden = false;
				_OverviewAircraftRemote.IsHidden = true;

				if (_overviewRangeRingsEnabled)
				{
					_OverviewRangeRings.IsHidden = false;
				}
				else
				{
					_OverviewRangeRings.IsHidden = true;
				}
			}
			else
			{
				_OverviewAircraft.IsHidden = true;
				_OverviewAircraftRemote.IsHidden = false;
				_OverviewRangeRings.IsHidden = true;
			}
		}

		private void ProcessTargetValues()
		{
			double xValue;
			double yValue;
			double distance;
			double bearing;

			if (_targetSelected)
			{
				for (int i = 0; i < TargetDataList.Count; i++)
				{
					TargetDataList[i].MapTargetHorizontalValue = OwnshipHorizontalValue + TargetDataList[i].TargetHorizontalValue - BullseyeHorizontalValue;
					TargetDataList[i].MapTargetVerticalValue = OwnshipVerticalValue + TargetDataList[i].TargetVerticalValue - BullseyeVerticalValue;

					xValue = (TargetDataList[i].TargetHorizontalValue - BullseyeHorizontalValue) / _mapFeetPerNauticalMile;
					yValue = (TargetDataList[i].TargetVerticalValue - BullseyeVerticalValue) / _mapFeetPerNauticalMile;

					distance = GetHypotenuse(xValue, yValue);
					bearing = GetBearing(xValue, yValue);

					TargetDataList[i].CourseDistance = distance;
					TargetDataList[i].CourseBearing = bearing;
				}
			}
		}

		public override void MouseDown(Point location)
		{
			int target_Num;
			double target_posX;
			double target_posY;
			double distance_posX;
			double distance_posY;
			double bearing;
			double distance;

			if (_inhibitMouseAction)
			{
				return;
			}

			_inhibitMouseAction = true;
			
			target_Num = GetTargetAtLocation(location.X, location.Y);

			if (target_Num >= 0)
			{
				OverviewRemoveTarget(target_Num);
				_inhibitMouseAction = false;

				return;
			}

			target_posX = (location.X - _widthCenterPosition) * 200 / Width;
			target_posY = (location.Y - _heightCenterPosition) * 200 / Height;

			if (Height >= Width)
			{
				distance_posX = target_posX * _targetBullseyeScale;
				distance_posY = -target_posY * _targetBullseyeScale * _ratioHeightToWidth;
			}
			else
			{
				distance_posX = target_posX * _targetBullseyeScale * _ratioWidthToHeight;
				distance_posY = -target_posY * _targetBullseyeScale;
			}

			bearing = GetBearing(distance_posX, distance_posY);
			distance = GetHypotenuse(distance_posX, distance_posY);

			if (distance <= 125)
			{
				OverviewTargetList.Insert(0, new Gauges.GaugeImage(_overviewTargetImage, _imageSize));
				OverviewTargetList[0].IsHidden = true;
				OverviewTargetList[0].Width = _squareWidth;
				OverviewTargetList[0].Height = _squareHeight;

				if (Height >= Width)
				{
					OverviewTargetList[0].PosX = target_posX;
					OverviewTargetList[0].PosY = target_posY + _OverviewBullseye.PosY;
				}
				else
				{
					OverviewTargetList[0].PosX = target_posX + _OverviewBullseye.PosX;
					OverviewTargetList[0].PosY = target_posY;
				}

				_targetSelected = true;
				OverviewTargetList[0].IsHidden = false;

				Components.Insert(Components.IndexOf(_OverviewAircraftRemote) + 1, OverviewTargetList[0]);

				TargetDataList.Insert(0, new TargetData
				{
					TargetBearing = bearing,
					TargetDistance = distance,
					TargetPosition_X = location.X,
					TargetPosition_Y = location.Y,
					TargetHorizontalValue = distance_posX * _mapFeetPerNauticalMile,
					TargetVerticalValue = distance_posY * _mapFeetPerNauticalMile
				});

				_OverviewTextData.TargetSelected = true;
				_OverviewTextData.SetTargetData(TargetDataList);

				_OverviewTargetLines.IsHidden = false;
				_OverviewTargetLines.SetTargetData(TargetDataList);

				ProcessTargetValues();
				Refresh();
			}

			_inhibitMouseAction = false;
		}

		int GetTargetAtLocation(double location_X, double location_Y)
		{
			double radius_Max = 8d * _scaleFactor;
			double radius_Min = radius_Max;
			double radius_Location;
			double diff_X;
			double diff_Y;

			int target_Num = -1;

			for (int i = 0; i < TargetDataList.Count; i++)
			{
				diff_X = TargetDataList[i].TargetPosition_X - location_X;
				diff_Y = TargetDataList[i].TargetPosition_Y - location_Y;

				radius_Location = GetHypotenuse(diff_X, diff_Y);

				if (radius_Location <= radius_Min)
				{
					radius_Min = radius_Location;
					target_Num = i;
				}
			}

			return target_Num;
		}

		void OverviewRemoveTarget(int index)
		{
			if (OverviewTargetList.Count > index)
			{
				Components.RemoveAt(Components.IndexOf(_OverviewAircraftRemote) + index + 1);

				OverviewTargetList.RemoveAt(index);
			}

			if (TargetDataList.Count > index)
			{
				TargetDataList.RemoveAt(index);
			}

			if (OverviewTargetList.Count == 0)
			{
				_targetSelected = false;

				_OverviewTextData.TargetSelected = false;
				_OverviewTextData.TargetBearing = 0;
				_OverviewTextData.TargetDistance = 0;

				_OverviewTargetLines.IsHidden = true;
			}

			ProcessTargetValues();
			Refresh();
		}

		void OverviewClearTargets()
		{
			while (OverviewTargetList.Count > 0)
			{
				Components.RemoveAt(Components.IndexOf(_OverviewAircraftRemote) + OverviewTargetList.Count);

				OverviewTargetList.RemoveAt(OverviewTargetList.Count - 1);
			}

			while (TargetDataList.Count > 0)
			{
				TargetDataList.RemoveAt(TargetDataList.Count - 1);
			}

			_targetSelected = false;

			_OverviewTextData.TargetSelected = false;
			_OverviewTextData.TargetBearing = 0;
			_OverviewTextData.TargetDistance = 0;

			_OverviewTargetLines.IsHidden = true;

			ProcessTargetValues();
			Refresh();
		}

		public override void Reset()
		{
			ResetTargetOverview();
		}

		void ResetTargetOverview()
		{
			_targetSelected = false;

			OverviewClearTargets();

			OwnshipRotationAngle = 0d;
			OwnshipHorizontalValue = 0d;
			OwnshipVerticalValue = 0d;

			BullseyeHorizontalValue = 0d;
			BullseyeVerticalValue = 0d;

			_OverviewTextData.TargetSelected = false;
			_OverviewTextData.TargetBearing = 0;
			_OverviewTextData.TargetDistance = 0;

			_OverviewTargetLines.IsHidden = true;

			_overviewRangeRingsEnabled = true;

			ProcessAircraftValues();
			ProcessTargetValues();
		}

		#endregion Methods


		#region Functions

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		private double GetHypotenuse(double xValue, double yValue)
		{
			return Math.Round(Math.Sqrt((xValue * xValue + yValue * yValue)));
		}

		private double GetBearing(double xValue, double yValue)
		{
			double bearing = Math.Round(Math.Atan2(xValue, yValue) * 180 / Math.PI);

			if (bearing < 0)
			{
				bearing = 360 + bearing;
			}

			return bearing;
		}

		#endregion Functions


		#region Scaling

		void OnMapControl_Resized(object sender, EventArgs e)
		{
			MapControlStaticResize();
		}

		void MapControlStaticResize()
		{
			_widthCenterPosition = Width / 2;
			_heightCenterPosition = Height / 2;

			_ratioHeightToWidth = Height / Width;
			_ratioWidthToHeight = Width / Height;

			if (Height >= Width)
			{
				_scaleFactor = Width / _needleSize.Width;

				_squareWidth = _needleSize.Width;
				_squareHeight = _needleSize.Height * _ratioWidthToHeight;
				_squarePosX = 0d;
				_squarePosY = _needleSize.Height * (1 - _ratioWidthToHeight) / 2d;

				_OverviewRangeRings.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y * _ratioWidthToHeight);
				_OverviewAircraft.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y * _ratioWidthToHeight);
				_OverviewTargetLines.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y * _ratioWidthToHeight);
			}
			else
			{
				_scaleFactor = Height / _needleSize.Height;

				_squareWidth = _needleSize.Width * _ratioHeightToWidth;
				_squareHeight = _needleSize.Height;
				_squarePosX = _needleSize.Width * (1 - _ratioHeightToWidth) / 2d;
				_squarePosY = 0d;

				_OverviewRangeRings.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X * _ratioHeightToWidth, _needleCenter.Y);
				_OverviewAircraft.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X * _ratioHeightToWidth, _needleCenter.Y);
				_OverviewTargetLines.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X * _ratioHeightToWidth, _needleCenter.Y);
			}

			_OverviewBullseye.Width = _squareWidth;
			_OverviewBullseye.Height = _squareHeight;
			_OverviewBullseye.PosX = _squarePosX;
			_OverviewBullseye.PosY = _squarePosY;

			_OverviewRangeRings.Tape_Width = _squareWidth;
			_OverviewRangeRings.Tape_Height = _squareHeight;
			_OverviewRangeRings.TapePosX = _squarePosX;
			_OverviewRangeRings.TapePosY = _squarePosY;

			_OverviewAircraft.Tape_Width = _squareWidth;
			_OverviewAircraft.Tape_Height = _squareHeight;
			_OverviewAircraft.TapePosX = _squarePosX;
			_OverviewAircraft.TapePosY = _squarePosY;

			_OverviewAircraftRemote.Tape_Width = _squareWidth;
			_OverviewAircraftRemote.Tape_Height = _squareHeight;
			_OverviewAircraftRemote.TapePosX = _squarePosX;
			_OverviewAircraftRemote.TapePosY = _squarePosY;

			_OverviewTextData.MapControlWidth = Width;
			_OverviewTextData.MapControlHeight = Height;

			Refresh();
		}

		#endregion Scaling


		#region Properties

		public class TargetData : ITargetData
		{
			public double TargetBearing { get; set; }
			public double TargetDistance { get; set; }
			public double TargetPosition_X { get; set; }
			public double TargetPosition_Y { get; set; }
			public double TargetHorizontalValue { get; set; }
			public double TargetVerticalValue { get; set; }
			public double MapTargetHorizontalValue { get; set; }
			public double MapTargetVerticalValue { get; set; }
			public double CourseBearing { get; set; }
			public double CourseDistance { get; set; }
		}

		public double AircraftBearing { get; set; }
		public double AircraftDistance { get; set; }
		private double OwnshipRotationAngle { get; set; }
		private double OwnshipHorizontalValue { get; set; }
		private double OwnshipVerticalValue { get; set; }
		private double BullseyeHorizontalValue { get; set; }
		private double BullseyeVerticalValue { get; set; }

		#endregion Properties

	}
}
