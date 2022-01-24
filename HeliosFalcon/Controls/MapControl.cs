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
	using System.Windows;
	using System.Windows.Media;
	using System;
	using System.Collections.Generic;
	using System.Linq;


	[HeliosControl("Helios.Falcon.MapControl", "Map Control", "Falcon Simulator", typeof(Gauges.GaugeRenderer))]

	public class MapControl : MapControls
	{
		private FalconInterface _falconInterface;

		private HeliosValue _mapRotationEnable;
		private HeliosValue _mapScaleChange;
		private HeliosValue _bullseyeVisible;
		private HeliosValue _threatsVisible;
		private HeliosValue _waypointsVisible;
		private HeliosValue _targetSelectVisible;

		private Gauges.GaugeImage _Background;
		private Gauges.GaugeImage _Foreground;
		private Gauges.GaugeImage _TargetBullseyeBackground;
		private Gauges.GaugeImage _TargetBullseye;
		private Gauges.GaugeImage _TargetPosition;
		private Gauges.GaugeImage _MapNoData;
		private Gauges.CustomGaugeNeedle _Map;
		private Gauges.CustomGaugeNeedle _Bullseye;
		private Gauges.CustomGaugeNeedle _RangeRings;
		private Gauges.CustomGaugeNeedle _Aircraft;
		private MapControlRenderer _MapOverlays;
		private MapControlTargetRenderer _TargetOverlays;

		private Rect _imageSize = new Rect(0d, 0d, 200d, 200d);
		private Size _needleSize = new Size(200d, 200d);
		private Rect _needleClip = new Rect(1d, 1d, 198d, 198d);
		private Point _needleLocation = new Point(0d, 0d);
		private Point _needleCenter = new Point(100d, 100d);

		private const string _mapBullseyeImage64 = "{HeliosFalcon}/Images/MapControl/Map Bullseye 64.png";
		private const string _mapBullseyeImage128 = "{HeliosFalcon}/Images/MapControl/Map Bullseye 128.png";
		private const string _mapRangeRingsImage = "{HeliosFalcon}/Images/MapControl/Map Range Rings.png";
		private const string _mapAircraftImage = "{HeliosFalcon}/Images/MapControl/Map Aircraft.png";
		private const string _mapNoDataImage = "{HeliosFalcon}/Images/MapControl/Map No Data.png";
		private const string _targetBullseyeImage = "{HeliosFalcon}/Images/MapControl/Target Bullseye.png";
		private const string _targetPositionImage = "{HeliosFalcon}/Images/MapControl/Target Position.png";
		private const string _targetBackgroundImage = "{HeliosFalcon}/Images/MapControl/Target Background.png";
		private const string _backgroundImage = "{HeliosFalcon}/Images/MapControl/Background 01.png";
		private const string _foregroundImage = "{HeliosFalcon}/Images/MapControl/Foreground.png";

		private const double _mapBaseScale = 2.2d;
		private const double _mapSizeFeet64 = 3358700;   // 1024 km x 3279.98 ft/km (BMS conversion value)
		private const double _mapSizeFeet128 = 6717400;  // 2048 km x 3279.98 ft/km (BMS conversion value)
		private const double _mapFeetPerNauticalMile = 6076;

		private double _mapSizeFeet = 3358700;
		private double _mapScaleMultiplier = 1d;  // 1d = 60Nm, 2d = 30Nm, 4d = 15Nm
		private double _mapSizeMultiplier = 1d;   // 1d = 64 Segment, 2d = 128 Segment
		private double _mapModifiedScale = 0d;
		private double _mapRotationAngle = 0d;
		private double _mapVerticalValue = 0d;
		private double _mapHorizontalValue = 0d;
		private double _bullseyeVerticalValue = 0d;
		private double _bullseyeHorizontalValue = 0d;
		private double _rangeScale = 0d;
		private double _targetBullseyeScale = 1.200d * 1.075d;
		private double _targetVerticalOffset;
		private double _targetHorizontalOffset;
		private int _rangeInitialHorizontal = 0;
		private int _mapInitialHorizontal = 0;
		private int _rangeInitialVertical = 0;
		private int _mapInitialVertical = 0;
		private int _targetTextRefreshCount = 0;
		private bool _mapRotation_Enabled = false;
		private bool _mapImageChanged = false;
		private bool _profileFirstStart = true;
		private bool _navPointsInitialized = false;
		private bool _refreshPending = false;
		private bool _targetSelected;
		private string _lastTheater;


		public MapControl()
			: base("MapControl", new Size(200d, 200d))
		{
			_Background = new Gauges.GaugeImage(_backgroundImage, _imageSize);
			Components.Add(_Background);

			_Map = new Gauges.CustomGaugeNeedle(_mapBaseImages[7, 1], _needleLocation, _needleSize, _needleCenter);
			_Map.Clip = new RectangleGeometry(_needleClip);
			_Map.ImageRefresh = true;
			Components.Add(_Map);

			_Bullseye = new Gauges.CustomGaugeNeedle(_mapBullseyeImage64, _needleLocation, _needleSize, _needleCenter);
			_Bullseye.Clip = new RectangleGeometry(_needleClip);
			_Bullseye.IsHidden = true;
			Components.Add(_Bullseye);

			_MapOverlays = new MapControlRenderer(_needleLocation, _needleSize, _needleCenter);
			_MapOverlays.Clip = new RectangleGeometry(_needleClip);
			Components.Add(_MapOverlays);
			
			_RangeRings = new Gauges.CustomGaugeNeedle(_mapRangeRingsImage, _needleLocation, _needleSize, _needleCenter);
			_RangeRings.Clip = new RectangleGeometry(_needleClip);
			Components.Add(_RangeRings);

			_Aircraft = new Gauges.CustomGaugeNeedle(_mapAircraftImage, _needleLocation, _needleSize, _needleCenter);
			_Aircraft.Clip = new RectangleGeometry(_needleClip);
			Components.Add(_Aircraft);

			_TargetBullseyeBackground = new Gauges.GaugeImage(_targetBackgroundImage, _imageSize);
			_TargetBullseyeBackground.IsHidden = true;
			Components.Add(_TargetBullseyeBackground);

			_TargetBullseye = new Gauges.GaugeImage(_targetBullseyeImage, _imageSize);
			_TargetBullseye.IsHidden = true;
			Components.Add(_TargetBullseye);

			_TargetPosition = new Gauges.GaugeImage(_targetPositionImage, _imageSize);
			_TargetPosition.IsHidden = true;
			Components.Add(_TargetPosition);

			_TargetOverlays = new MapControlTargetRenderer();
			_TargetOverlays.Clip = new RectangleGeometry(_needleClip);
			_TargetOverlays.IsHidden = true;
			Components.Add(_TargetOverlays);

			_Foreground = new Gauges.GaugeImage(_foregroundImage, _imageSize);
			_Foreground.IsHidden = true;
			Components.Add(_Foreground);

			_MapNoData = new Gauges.GaugeImage(_mapNoDataImage, _imageSize);
			_MapNoData.IsHidden = true;
			Components.Add(_MapNoData);

			_mapRotationEnable = new HeliosValue(this, new BindingValue(false), "", "Map North Up vs Heading Up", "Sets North Up or Heading Up map orientation.", "Set true for Heading Up orientation.", BindingValueUnits.Boolean);
			_mapRotationEnable.Execute += new HeliosActionHandler(MapRotationEnable_Execute);
			Actions.Add(_mapRotationEnable);
			Values.Add(_mapRotationEnable);

			_mapScaleChange = new HeliosValue(this, new BindingValue(0d), "", "Map Scale", "Sets the scale of the map.", "1 = 60Nm, 2 = 30Nm, 3 = 15Nm, Default = 2", BindingValueUnits.Numeric);
			_mapScaleChange.Execute += new HeliosActionHandler(MapScaleChange_Execute);
			Actions.Add(_mapScaleChange);
			Values.Add(_mapScaleChange);

			_bullseyeVisible = new HeliosValue(this, new BindingValue(false), "", "Bullseye Visible", "Sets visibility of the bullseye.", "Set true to show bullseye.", BindingValueUnits.Boolean);
			_bullseyeVisible.Execute += new HeliosActionHandler(BullseyeVisible_Execute);
			Actions.Add(_bullseyeVisible);
			Values.Add(_bullseyeVisible);

			_threatsVisible = new HeliosValue(this, new BindingValue(false), "", "Pre-Planned Threats Visible", "Sets visibility of the pre-planned threats.", "Set true to show pre-planned threats.", BindingValueUnits.Boolean);
			_threatsVisible.Execute += new HeliosActionHandler(ThreatsVisible_Execute);
			Actions.Add(_threatsVisible);
			Values.Add(_threatsVisible);

			_waypointsVisible = new HeliosValue(this, new BindingValue(false), "", "Waypoints Visible", "Sets visibility of the waypoints.", "Set true to show waypoints.", BindingValueUnits.Boolean);
			_waypointsVisible.Execute += new HeliosActionHandler(WaypointsVisible_Execute);
			Actions.Add(_waypointsVisible);
			Values.Add(_waypointsVisible);

			_targetSelectVisible = new HeliosValue(this, new BindingValue(false), "", "Target Selection Visible", "Sets visibility of the target selection.", "Set true to show target selection.", BindingValueUnits.Boolean);
			_targetSelectVisible.Execute += new HeliosActionHandler(TargetSelectVisible_Execute);
			Actions.Add(_targetSelectVisible);
			Values.Add(_targetSelectVisible);

			MapControlStaticResize();
			MapControlDynamicResize(true);
			Resized += new EventHandler(OnMapControl_Resized);
		}


		#region Actions

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

			if (_profileFirstStart)
			{
				_profileFirstStart = false;
				ShowNoDataPanel();
				MapScaleChange(2d);
			}
		}

		void Profile_ProfileTick(object sender, EventArgs e)
		{
			if (_falconInterface != null)
			{
				BindingValue runtimeFlying = GetValue("Runtime", "Flying");
				bool inFlight = runtimeFlying.BoolValue;

				string theater = _falconInterface.CurrentTheater;

				if (inFlight)
				{
					if (!string.IsNullOrEmpty(theater) && theater != _lastTheater)
					{
						_lastTheater = theater;
						TheaterMapSelect(theater);
					}

					if (!_navPointsInitialized)
					{
						List<string> navPoints = _falconInterface.NavPoints;

						if (navPoints != null && navPoints.Any())
						{
							_MapOverlays.ProcessNavPointValues(navPoints);
							_navPointsInitialized = true;
							Refresh();
						}
					}

					HideNoDataPanel();
				}

				ProcessOwnshipValues();
				ProcessTargetValues(false);

				if (_refreshPending)
				{
					Refresh();
					_refreshPending = false;
				}

				if (!inFlight)
				{
					_navPointsInitialized = false;
					ShowNoDataPanel();
					ResetTargetValues();
				}
			}
		}

		void Profile_ProfileStopped(object sender, EventArgs e)
		{
			_falconInterface = null;
		}

		void ProcessOwnshipValues()
		{
			BindingValue mapRotationAngle = GetValue("HSI", "current heading");
			MapRotationAngle = mapRotationAngle.DoubleValue;

			BindingValue mapVerticalValue = GetValue("Ownship", "x");
			MapVerticalValue = mapVerticalValue.DoubleValue;

			BindingValue mapHorizontalValue = GetValue("Ownship", "y");
			MapHorizontalValue = mapHorizontalValue.DoubleValue;

			BindingValue bullseyeVerticalValue = GetValue("Ownship", "deltaX from bulls");
			BullseyeVerticalValue = bullseyeVerticalValue.DoubleValue;

			BindingValue bullseyeHorizontalValue = GetValue("Ownship", "deltaY from bulls");
			BullseyeHorizontalValue = bullseyeHorizontalValue.DoubleValue;

			if (_mapImageChanged)
			{
				_mapImageChanged = false;
				CalculateOffsets();
			}
		}

		private void ProcessTargetValues(bool targetTextRefresh)
		{
			if (_targetSelected)
			{
				double target_vertical_value = MapVerticalValue - BullseyeVerticalValue + _targetVerticalOffset;
				double target_horizontal_value = MapHorizontalValue - BullseyeHorizontalValue + _targetHorizontalOffset;

				_MapOverlays.OwnshipVerticalValue = MapVerticalValue;
				_MapOverlays.OwnshipHorizontalValue = MapHorizontalValue;

				_MapOverlays.TargetVerticalValue = target_vertical_value;
				_MapOverlays.TargetHorizontalValue = target_horizontal_value;

				if (_targetTextRefreshCount == 10 || targetTextRefresh)
				{
					double xValue = (target_horizontal_value - MapHorizontalValue) / _mapFeetPerNauticalMile;
					double yValue = (target_vertical_value - MapVerticalValue) / _mapFeetPerNauticalMile;

					double distance = GetHypotenuse(xValue, yValue);
					double bearing = GetBearing(xValue, yValue);

					_TargetOverlays.CourseDistance = distance;
					_TargetOverlays.CourseBearing = bearing;

					_MapOverlays.CourseDistance = distance;
					_MapOverlays.CourseBearing = bearing;

					_targetTextRefreshCount = 0;
				}
				else
				{
					_targetTextRefreshCount++;
				}
			}
			else
			{
				_TargetOverlays.CourseDistance = 0;
				_TargetOverlays.CourseBearing = 0;
			}
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		public override void Reset()
		{
			BeginTriggerBypass(true);

			_Bullseye.IsHidden = true;
			_MapOverlays.ThreatsVisible = false;
			_MapOverlays.WaypointsVisible = false;
			_mapRotation_Enabled = false;
			MapRotationAngle = 0d;
			MapScaleChange(2d);
			ResetTargetValues();

			EndTriggerBypass(true);
		}

		void ShowNoDataPanel()
		{
			_MapNoData.IsHidden = false;
			_Foreground.IsHidden = false;
		}

		void HideNoDataPanel()
		{
			_MapNoData.IsHidden = true;
			_Foreground.IsHidden = true;
		}

		void BullseyeVisible_Execute(object action, HeliosActionEventArgs e)
		{
			_bullseyeVisible.SetValue(e.Value, e.BypassCascadingTriggers);
			_Bullseye.IsHidden = !_bullseyeVisible.Value.BoolValue;
		}

		void ThreatsVisible_Execute(object action, HeliosActionEventArgs e)
		{
			_threatsVisible.SetValue(e.Value, e.BypassCascadingTriggers);
			bool value = _threatsVisible.Value.BoolValue;

			_MapOverlays.ThreatsVisible = value;
			_refreshPending = true;
		}

		void WaypointsVisible_Execute(object action, HeliosActionEventArgs e)
		{
			_waypointsVisible.SetValue(e.Value, e.BypassCascadingTriggers);
			bool value = _waypointsVisible.Value.BoolValue;

			_MapOverlays.WaypointsVisible = value;
			_refreshPending = true;
		}

		void TargetSelectVisible_Execute(object action, HeliosActionEventArgs e)
		{
			_targetSelectVisible.SetValue(e.Value, e.BypassCascadingTriggers);
			bool value = _targetSelectVisible.Value.BoolValue;

			_TargetBullseye.IsHidden = !value;
			_TargetBullseyeBackground.IsHidden = !value;
			_TargetOverlays.IsHidden = !value;

			if (_targetSelected)
			{
				_TargetPosition.IsHidden = !value;
			}
			else
			{
				_TargetPosition.IsHidden = true;
			}

			if (_TargetOverlays.IsHidden)
			{
				Refresh();
			}
		}

		public override void MouseDown(Point location)
		{
			double target_posX;
			double target_posY;
			double distance_posX;
			double distance_posY;
			double distance;

			if (!_TargetBullseye.IsHidden)
			{
				target_posX = (location.X - Width / 2) * 200 / Width;
				target_posY = (location.Y - Height / 2) * 200 / Height;

				if (Height >= Width)
				{
					distance_posX = target_posX * _targetBullseyeScale;
					distance_posY = - target_posY * _targetBullseyeScale * Height / Width;
				}
				else
				{
					distance_posX = target_posX * _targetBullseyeScale * Width / Height;
					distance_posY = - target_posY * _targetBullseyeScale;
				}

				distance = GetHypotenuse(distance_posX, distance_posY);

				if (distance <= 125)
				{
					if (_TargetPosition.IsHidden)
					{
						if (Height >= Width)
						{
							_TargetPosition.PosX = target_posX;
							_TargetPosition.PosY = target_posY + _TargetBullseye.PosY;
						}
						else
						{
							_TargetPosition.PosX = target_posX + _TargetBullseye.PosX;
							_TargetPosition.PosY = target_posY;
						}

						_TargetPosition.IsHidden = false;
						_TargetOverlays.TargetBearing = GetBearing(distance_posX, distance_posY);
						_TargetOverlays.TargetDistance = distance;
						_targetHorizontalOffset = distance_posX * _mapFeetPerNauticalMile;
						_targetVerticalOffset = distance_posY * _mapFeetPerNauticalMile;
						_targetSelected = true;
						_MapOverlays.TargetVisible = true;
					}
					else
					{
						_TargetPosition.IsHidden = true;
						_TargetOverlays.TargetBearing = 0;
						_TargetOverlays.TargetDistance = 0;
						_targetHorizontalOffset = 0;
						_targetVerticalOffset = 0;
						_targetSelected = false;
						_MapOverlays.TargetVisible = false;
					}

					ProcessTargetValues(true);
					Refresh();
				}
			}
		}

		void ResetTargetValues()
		{
			_targetSelected = false;
			_TargetBullseyeBackground.IsHidden = true;
			_TargetBullseye.IsHidden = true;
			_TargetPosition.IsHidden = true;
			_TargetOverlays.IsHidden = true;
			_TargetOverlays.TargetBearing = 0;
			_TargetOverlays.TargetDistance = 0;
			_targetHorizontalOffset = 0;
			_targetVerticalOffset = 0;
			_MapOverlays.TargetVisible = false;
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

		#endregion Actions


		#region Map Selection

		void TheaterMapSelect(string theater)
		{
			double mapNumber = 0d;

			mapNumber = GetTheaterMapNumber(_mapBaseImages, theater);

			if (mapNumber == 0d)
			{
				mapNumber = GetTheaterMapNumber(_mapUserImages, theater);
			}

			if (mapNumber > 0d)
			{
				MapImageSelect(mapNumber);
			}
		}

		double GetTheaterMapNumber(string[,] mapImages, string theater)
		{
			double mapNumber = 0d;

			for (int i = 0; i < mapImages.GetLength(0); i++)
			{
				List<string> theaters = mapImages[i, 3].Split(',').Select(p => p.Trim()).ToList<string>();
				if (theaters.Contains(theater, StringComparer.OrdinalIgnoreCase))
				{
					mapNumber = Convert.ToDouble(mapImages[i, 0]);
				}
			}

			return mapNumber;
		}

		void MapImageSelect(double mapNumber)
		{
			if (mapNumber > 100d && mapNumber < 200d)
			{
				MapImageAssign(_mapBaseImages, mapNumber);

			}
			else if (mapNumber > 200d && mapNumber < 300d)
			{
				MapImageAssign(_mapUserImages, mapNumber);
			}

			if (_mapImageChanged)
			{
				MapImageChanged();
			}
		}

		void MapImageChanged()
		{
			if (_mapSizeMultiplier == 1d)
			{
				_mapSizeFeet = _mapSizeFeet64;
				_Bullseye.Image = _mapBullseyeImage64;
			}
			else if (_mapSizeMultiplier == 2d)
			{
				_mapSizeFeet = _mapSizeFeet128;
				_Bullseye.Image = _mapBullseyeImage128;
			}
			else
			{
				_mapSizeFeet = _mapSizeFeet64;
				_Bullseye.Image = _mapBullseyeImage64;
			}

			MapControlDynamicResize(true);
			CalculateOffsets();
		}

		void MapImageAssign(string[,] mapImages, double mapNumber)
		{
			for (int i = 0; i < mapImages.GetLength(0); i++)
			{
				if (mapNumber == Convert.ToDouble(mapImages[i, 0]))
				{
					if (_Map.Image != mapImages[i, 1])
					{
						_Map.Image = mapImages[i, 1];
						_mapSizeMultiplier = Convert.ToDouble(mapImages[i, 2]);
						_mapImageChanged = true;
					}
				}
			}
		}

		#endregion


		#region Map Scaling

		void OnMapControl_Resized(object sender, EventArgs e)
		{
			MapControlStaticResize();
			MapControlDynamicResize(true);
		}

		void MapControlStaticResize()
		{
			double mapShortestSize = 0d;
			double rangeWidth = 0d;
			double rangeHeight = 0d;
			double rangeInitialHorizontal = 0d;
			double rangeInitialVertical = 0d;
			double squareWidth = 0d;
			double squareHeight = 0d;
			double squarePosX = 0d;
			double squarePosY = 0d;

			if (Height >= Width)
			{
				mapShortestSize = Width;
				_rangeScale = Width / Height * _mapBaseScale;
				rangeWidth = 200d * _mapBaseScale;
				rangeHeight = 200d * _rangeScale;

				rangeInitialHorizontal = (200d - rangeWidth) / 2d;
				rangeInitialVertical = (200d - rangeHeight) / 2d;

				squareWidth = 200d;
				squareHeight = Width / Height * 200;
				squarePosX = 0d;
				squarePosY = (200d - Width / Height * 200d) / 2d;
			}
			else
			{
				mapShortestSize = Height;
				_rangeScale = Height / Width * _mapBaseScale;
				rangeWidth = 200d * _rangeScale;
				rangeHeight = 200d * _mapBaseScale;

				rangeInitialHorizontal = (200d - rangeWidth) / 2d;
				rangeInitialVertical = (200d - rangeHeight) / 2d;

				squareWidth = Height / Width * 200;
				squareHeight = 200d;
				squarePosX = (200d - Height / Width * 200d) / 2d;
				squarePosY = 0d;
			}

			_MapOverlays.MapShortestSize = mapShortestSize;

			_RangeRings.Tape_Width = rangeWidth;
			_Aircraft.Tape_Width = rangeWidth;
			_rangeInitialHorizontal = Convert.ToInt32(rangeInitialHorizontal);

			_RangeRings.Tape_Height = rangeHeight;
			_Aircraft.Tape_Height = rangeHeight;
			_rangeInitialVertical = Convert.ToInt32(rangeInitialVertical);

			_TargetBullseye.Width = squareWidth;
			_TargetBullseye.Height = squareHeight;
			_TargetBullseye.PosX = squarePosX;
			_TargetBullseye.PosY = squarePosY;

			_TargetPosition.Width = squareWidth;
			_TargetPosition.Height = squareHeight;
			_TargetPosition.PosX = squarePosX;
			_TargetPosition.PosY = squarePosY;

			_MapNoData.Width = squareWidth;
			_MapNoData.Height = squareHeight;
			_MapNoData.PosX = squarePosX;
			_MapNoData.PosY = squarePosY;

			_TargetOverlays.MapControlWidth = Width;
			_TargetOverlays.MapControlHeight = Height;
		}

		void MapControlDynamicResize(bool mapResized)
		{
			double mapScale = 0d;
			double mapWidth = 0d;
			double mapHeight = 0d;
			double mapInitialHorizontal = 0d;
			double mapInitialVertical = 0d;

			if (Height >= Width)
			{
				mapScale = _rangeScale * _mapScaleMultiplier * _mapSizeMultiplier;

				mapWidth = Height / Width * 200d * mapScale;
				mapInitialHorizontal = (200d - mapWidth) / 2d;
				mapHeight = 200d * mapScale;
				mapInitialVertical = (200d - mapHeight) / 2d;
			}
			else
			{
				mapScale = _rangeScale * _mapScaleMultiplier * _mapSizeMultiplier;

				mapHeight = Width / Height * 200d * mapScale;
				mapInitialVertical = (200d - mapHeight) / 2d;
				mapWidth = 200d * mapScale;
				mapInitialHorizontal = (200d - mapWidth) / 2d;
			}

			_mapModifiedScale = mapScale;

			_MapOverlays.MapWidth = mapWidth;
			_MapOverlays.MapHeight = mapHeight;
			_MapOverlays.MapSizeFeet = _mapSizeFeet;
			_MapOverlays.MapScaleMultiplier = _mapScaleMultiplier;

			_Map.Tape_Width = mapWidth;
			_MapOverlays.Tape_Width = mapWidth;
			_Bullseye.Tape_Width = mapWidth;
			_mapInitialHorizontal = Convert.ToInt32(mapInitialHorizontal);

			_Map.Tape_Height = mapHeight;
			_MapOverlays.Tape_Height = mapHeight;
			_Bullseye.Tape_Height = mapHeight;
			_mapInitialVertical = Convert.ToInt32(mapInitialVertical);

			_RangeRings.HorizontalOffset = _rangeInitialHorizontal;
			_RangeRings.VerticalOffset = _rangeInitialVertical;
			_Aircraft.HorizontalOffset = _rangeInitialHorizontal;
			_Aircraft.VerticalOffset = _rangeInitialVertical;

			ProcessTargetValues(true);

			Refresh();

			if (mapResized)
			{
				_Map.HorizontalOffset = _mapInitialHorizontal;
				_Map.VerticalOffset = _mapInitialVertical;
				_MapOverlays.HorizontalOffset = _mapInitialHorizontal;
				_MapOverlays.VerticalOffset = _mapInitialVertical;
				_Bullseye.HorizontalOffset = _mapInitialHorizontal;
				_Bullseye.VerticalOffset = _mapInitialVertical;
			}
		}

		void MapVerticalOffset_Calculate(double vValue)
		{
			double mapVerticalValue = vValue - _mapSizeFeet / 2;

			if (Height >= Width)
			{
				_Map.VerticalOffset = _mapInitialVertical + (mapVerticalValue / _mapSizeFeet * _mapModifiedScale * 200);
				_MapOverlays.VerticalOffset = _Map.VerticalOffset;
			}
			else
			{
				_Map.VerticalOffset = _mapInitialVertical + (mapVerticalValue / _mapSizeFeet * _mapBaseScale * _mapScaleMultiplier * _mapSizeMultiplier * 200);
				_MapOverlays.VerticalOffset = _Map.VerticalOffset;
			}
		}

		void MapHorizontalOffset_Calculate(double hValue)
		{
			double mapHorizontalValue = hValue - _mapSizeFeet / 2;

			if (Height >= Width)
			{
				_Map.HorizontalOffset = _mapInitialHorizontal - (mapHorizontalValue / _mapSizeFeet * _mapBaseScale * _mapScaleMultiplier * _mapSizeMultiplier * 200);
				_MapOverlays.HorizontalOffset = _Map.HorizontalOffset;
			}
			else
			{
				_Map.HorizontalOffset = _mapInitialHorizontal - (mapHorizontalValue / _mapSizeFeet * _mapModifiedScale * 200);
				_MapOverlays.HorizontalOffset = _Map.HorizontalOffset;
			}
		}

		void BullseyeVerticalOffset_Calculate(double bullseyeVerticalValue)
		{
			if (Height >= Width)
			{
				_Bullseye.VerticalOffset = _mapInitialVertical + (bullseyeVerticalValue / _mapSizeFeet * _mapModifiedScale * 200);
			}
			else
			{
				_Bullseye.VerticalOffset = _mapInitialVertical + (bullseyeVerticalValue / _mapSizeFeet * _mapBaseScale * _mapScaleMultiplier * _mapSizeMultiplier * 200);
			}
		}

		void BullseyeHorizontalOffset_Calculate(double bullseyeHorizontalValue)
		{
			if (Height >= Width)
			{
				_Bullseye.HorizontalOffset = _mapInitialHorizontal - (bullseyeHorizontalValue / _mapSizeFeet * _mapBaseScale * _mapScaleMultiplier * _mapSizeMultiplier * 200);
			}
			else
			{
				_Bullseye.HorizontalOffset = _mapInitialHorizontal - (bullseyeHorizontalValue / _mapSizeFeet * _mapModifiedScale * 200);
			}
		}

		void MapRotationEnable_Execute(object action, HeliosActionEventArgs e)
		{
			_mapRotationEnable.SetValue(e.Value, e.BypassCascadingTriggers);
			_mapRotation_Enabled = _mapRotationEnable.Value.BoolValue;
			MapRotationAngle_Calculate(MapRotationAngle);
		}

		void MapRotationAngle_Calculate(double angle)
		{
			if (_mapRotation_Enabled)
			{
				_Map.Rotation = -angle;
				_MapOverlays.Rotation = -angle;
				_Bullseye.Rotation = -angle;
				_RangeRings.Rotation = -angle;
				_Aircraft.Rotation = 0d;
			}
			else
			{
				_Map.Rotation = 0d;
				_MapOverlays.Rotation = 0d;
				_Bullseye.Rotation = 0d;
				_RangeRings.Rotation = 0d;
				_Aircraft.Rotation = angle;
			}
		}

		void MapScaleChange_Execute(object action, HeliosActionEventArgs e)
		{
			_mapScaleChange.SetValue(e.Value, e.BypassCascadingTriggers);
			double value = _mapScaleChange.Value.DoubleValue;
			MapScaleChange(value);
		}

		void MapScaleChange(double value)
		{ 
			if (value == 1d)
			{
				_mapScaleMultiplier = 1d;
			}
			else if (value == 2d)
			{
				_mapScaleMultiplier = 2d;
			}
			else if (value == 3d)
			{
				_mapScaleMultiplier = 4d;
			}
			else
			{
				_mapScaleMultiplier = 2d;
			}

			MapControlDynamicResize(false);
			CalculateOffsets();
		}

		void CalculateOffsets()
		{
			MapRotationAngle_Calculate(MapRotationAngle);
			MapVerticalOffset_Calculate(MapVerticalValue);
			MapHorizontalOffset_Calculate(MapHorizontalValue);
			BullseyeVerticalOffset_Calculate(BullseyeVerticalValue);
			BullseyeHorizontalOffset_Calculate(BullseyeHorizontalValue);
		}

		#endregion


		#region Properties

		private double MapRotationAngle
		{
			get
			{
				return _mapRotationAngle;
			}
			set
			{
				if ((_mapRotationAngle == 0d && value != 0)
					|| (_mapRotationAngle != 0d && !_mapRotationAngle.Equals(value)))
				{
					double oldValue = _mapRotationAngle;
					_mapRotationAngle = value;
					OnPropertyChanged("MapRotationAngle", oldValue, value, true);
					{
						MapRotationAngle_Calculate(_mapRotationAngle);
						_refreshPending = false;
					}
				}
			}
		}

		private double MapVerticalValue
		{
			get
			{
				return _mapVerticalValue;
			}
			set
			{
				if ((_mapVerticalValue == 0d && value != 0)
					|| (_mapVerticalValue != 0d && !_mapVerticalValue.Equals(value)))
				{
					double oldValue = _mapVerticalValue;
					_mapVerticalValue = value;
					OnPropertyChanged("MapVerticalValue", oldValue, value, true);
					{
						MapVerticalOffset_Calculate(_mapVerticalValue);
						_refreshPending = false;
					}
				}
			}
		}

		private double MapHorizontalValue
		{
			get
			{
				return _mapHorizontalValue;
			}
			set
			{
				if ((_mapHorizontalValue == 0d && value != 0)
					|| (_mapHorizontalValue != 0d && !_mapHorizontalValue.Equals(value)))
				{
					double oldValue = _mapHorizontalValue;
					_mapHorizontalValue = value;
					OnPropertyChanged("MapHorizontalValue", oldValue, value, true);
					{
						MapHorizontalOffset_Calculate(_mapHorizontalValue);
						_refreshPending = false;
					}
				}
			}
		}

		private double BullseyeVerticalValue
		{
			get
			{
				return _bullseyeVerticalValue;
			}
			set
			{
				if ((_bullseyeVerticalValue == 0d && value != 0)
					|| (_bullseyeVerticalValue != 0d && !_bullseyeVerticalValue.Equals(value)))
				{
					double oldValue = _bullseyeVerticalValue;
					_bullseyeVerticalValue = value;
					OnPropertyChanged("BullseyeVerticalValue", oldValue, value, true);
					{
						BullseyeVerticalOffset_Calculate(_bullseyeVerticalValue);
						_refreshPending = false;
					}
				}
			}
		}

		private double BullseyeHorizontalValue
		{
			get
			{
				return _bullseyeHorizontalValue;
			}
			set
			{
				if ((_bullseyeHorizontalValue == 0d && value != 0)
					|| (_bullseyeHorizontalValue != 0d && !_bullseyeHorizontalValue.Equals(value)))
				{
					double oldValue = _bullseyeHorizontalValue;
					_bullseyeHorizontalValue = value;
					OnPropertyChanged("BullseyeHorizontalValue", oldValue, value, true);
					{
						BullseyeHorizontalOffset_Calculate(_bullseyeHorizontalValue);
						_refreshPending = false;
					}
				}
			}
		}

		#endregion

	}
}
