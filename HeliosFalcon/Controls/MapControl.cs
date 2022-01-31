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
		private HeliosValue _overviewPanelVisible;
		private HeliosValue _overviewClearTarget;
		private HeliosValue _overviewRangeRingsVisible;

		private Gauges.GaugeImage _MapBackground;
		private Gauges.GaugeImage _MapNoDataBackground;
		private Gauges.GaugeImage _OverviewBackground;
		private Gauges.GaugeImage _OverviewBullseye;
		private Gauges.GaugeImage _OverviewTarget;
		private Gauges.GaugeImage _MapNoDataForeground;
		private Gauges.CustomGaugeNeedle _Map;
		private Gauges.CustomGaugeNeedle _MapBullseye;
		private Gauges.CustomGaugeNeedle _MapRangeRings;
		private Gauges.CustomGaugeNeedle _MapAircraft;
		private Gauges.CustomGaugeNeedle _OverviewRangeRings;
		private Gauges.CustomGaugeNeedle _OverviewAircraft;
		private Gauges.CustomGaugeNeedle _OverviewAircraftRemote;
		private MapControlMapRenderer _MapOverlays;
		private MapControlLineRenderer _OverviewTargetLines;
		private MapControlTextRenderer _OverviewTextData;

		private Rect _imageSize = new Rect(0d, 0d, 200d, 200d);
		private Size _needleSize = new Size(200d, 200d);
		private Rect _needleClip = new Rect(0d, 0d, 200d, 200d);
		private Point _needleLocation = new Point(0d, 0d);
		private Point _needleCenter = new Point(100d, 100d);

		private const string _mapBackgroundImage = "{HeliosFalcon}/Images/MapControl/Map Background.png";
		private const string _mapBullseyeImage64 = "{HeliosFalcon}/Images/MapControl/Map Bullseye 64.png";
		private const string _mapBullseyeImage128 = "{HeliosFalcon}/Images/MapControl/Map Bullseye 128.png";
		private const string _mapRangeRingsImage = "{HeliosFalcon}/Images/MapControl/Map Range Rings.png";
		private const string _mapAircraftImage = "{HeliosFalcon}/Images/MapControl/Map Aircraft.png";
		private const string _mapNoDataBackgroundImage = "{HeliosFalcon}/Images/MapControl/MapNoData Background.png";
		private const string _mapNoDataForegroundImage = "{HeliosFalcon}/Images/MapControl/MapNoData Foreground.png";

		private const string _overviewBullseyeImage = "{HeliosFalcon}/Images/MapControl/Overview Bullseye.png";
		private const string _overviewBackgroundImage = "{HeliosFalcon}/Images/MapControl/Overview Background.png";
		private const string _overviewTargetImage = "{HeliosFalcon}/Images/MapControl/Overview Target.png";
		private const string _overviewRangeRingsImage = "{HeliosFalcon}/Images/MapControl/Overview Range Rings.png";
		private const string _overviewAircraftImage = "{HeliosFalcon}/Images/MapControl/Overview Aircraft.png";
		private const string _overviewAircraftRemoteImage = "{HeliosFalcon}/Images/MapControl/Overview Aircraft Remote.png";

		private const double _mapBaseScale = 2.2d;
		private const double _mapSizeFeet64 = 3358700;   // 1024 km x 3279.98 ft/km (BMS conversion value)
		private const double _mapSizeFeet128 = 6717400;  // 2048 km x 3279.98 ft/km (BMS conversion value)
		private const double _mapFeetPerNauticalMile = 6076;
		private const double _targetBullseyeScale = 1.200d * 1.075d;

		private double _mapSizeFeet = 3358700;
		private double _mapScaleMultiplier = 1d;  // 1d = 60Nm, 2d = 30Nm, 4d = 15Nm
		private double _mapSizeMultiplier = 1d;   // 1d = 64 Segment, 2d = 128 Segment
		private double _mapModifiedScale;
		private double _mapRotationAngle;
		private double _mapVerticalValue;
		private double _mapHorizontalValue;
		private double _bullseyeVerticalValue;
		private double _bullseyeHorizontalValue;
		private double _widthCenterPosition;
		private double _heightCenterPosition;
		private double _ratioHeightToWidth;
		private double _ratioWidthToHeight;
		private double _rangeScale;
		private double _targetVerticalOffset;
		private double _targetHorizontalOffset;
		private double _aircraftBearing;
		private double _aircraftDistance;
		private int _rangeInitialHorizontal;
		private int _mapInitialHorizontal;
		private int _rangeInitialVertical;
		private int _mapInitialVertical;
		private int _targetTextRefreshCount;
		private bool _mapRotation_Enabled = false;
		private bool _mapImageChanged = false;
		private bool _profileFirstStart = true;
		private bool _navPointsInitialized = false;
		private bool _refreshPending = false;
		private bool _overviewPanelEnabled = false;
		private bool _overviewRangeRingsEnabled = true;
		private bool _targetSelected = false;
		private string _lastTheater;


		public MapControl()
			: base("MapControl", new Size(200d, 200d))
		{
			_MapBackground = new Gauges.GaugeImage(_mapBackgroundImage, _imageSize);
			Components.Add(_MapBackground);

			_Map = new Gauges.CustomGaugeNeedle(_mapBaseImages[7, 1], _needleLocation, _needleSize, _needleCenter);
			_Map.Clip = new RectangleGeometry(_needleClip);
			_Map.ImageRefresh = true;
			Components.Add(_Map);

			_MapBullseye = new Gauges.CustomGaugeNeedle(_mapBullseyeImage64, _needleLocation, _needleSize, _needleCenter);
			_MapBullseye.Clip = new RectangleGeometry(_needleClip);
			_MapBullseye.IsHidden = true;
			Components.Add(_MapBullseye);

			_MapOverlays = new MapControlMapRenderer(_needleLocation, _needleSize, _needleCenter);
			_MapOverlays.Clip = new RectangleGeometry(_needleClip);
			Components.Add(_MapOverlays);
			
			_MapRangeRings = new Gauges.CustomGaugeNeedle(_mapRangeRingsImage, _needleLocation, _needleSize, _needleCenter);
			_MapRangeRings.Clip = new RectangleGeometry(_needleClip);
			Components.Add(_MapRangeRings);

			_MapAircraft = new Gauges.CustomGaugeNeedle(_mapAircraftImage, _needleLocation, _needleSize, _needleCenter);
			_MapAircraft.Clip = new RectangleGeometry(_needleClip);
			Components.Add(_MapAircraft);

			_OverviewBackground = new Gauges.GaugeImage(_overviewBackgroundImage, _imageSize);
			_OverviewBackground.IsHidden = true;
			Components.Add(_OverviewBackground);

			_OverviewBullseye = new Gauges.GaugeImage(_overviewBullseyeImage, _imageSize);
			_OverviewBullseye.IsHidden = true;
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

			_OverviewTarget = new Gauges.GaugeImage(_overviewTargetImage, _imageSize);
			_OverviewTarget.IsHidden = true;
			Components.Add(_OverviewTarget);

			_OverviewTextData = new MapControlTextRenderer();
			_OverviewTextData.Clip = new RectangleGeometry(_needleClip);
			_OverviewTextData.IsHidden = true;
			Components.Add(_OverviewTextData);

			_MapNoDataBackground = new Gauges.GaugeImage(_mapNoDataBackgroundImage, _imageSize);
			_MapNoDataBackground.IsHidden = true;
			Components.Add(_MapNoDataBackground);

			_MapNoDataForeground = new Gauges.GaugeImage(_mapNoDataForegroundImage, _imageSize);
			_MapNoDataForeground.IsHidden = true;
			Components.Add(_MapNoDataForeground);

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

			_overviewPanelVisible = new HeliosValue(this, new BindingValue(false), "", "Target Selection Visible", "Sets visibility of the target selection.", "Set true to show target selection.", BindingValueUnits.Boolean);
			_overviewPanelVisible.Execute += new HeliosActionHandler(OverviewPanelVisible_Execute);
			Actions.Add(_overviewPanelVisible);
			Values.Add(_overviewPanelVisible);

			_overviewClearTarget = new HeliosValue(this, new BindingValue(false), "", "Target Selection Clear", "Clears the selected target.", "Set true to clear the selected target.", BindingValueUnits.Boolean);
			_overviewClearTarget.Execute += new HeliosActionHandler(OverviewClearTarget_Execute);
			Actions.Add(_overviewClearTarget);
			Values.Add(_overviewClearTarget);

			_overviewRangeRingsVisible = new HeliosValue(this, new BindingValue(true), "", "Target Selection Range Rings Visible", "Sets visibility of the target selection range rings.", "Set true to show target selection range rings.", BindingValueUnits.Boolean);
			_overviewRangeRingsVisible.Execute += new HeliosActionHandler(OverviewRangeRingsVisible_Execute);
			Actions.Add(_overviewRangeRingsVisible);
			Values.Add(_overviewRangeRingsVisible);

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
				else
				{
					_navPointsInitialized = false;
					ShowNoDataPanel();
					ResetTargetOverview();
				}

				ProcessOwnshipValues();
				ProcessTargetValues(false);
				ProcessAircraftValues();

				if (_refreshPending)
				{
					Refresh();
					_refreshPending = false;
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
			double target_vertical_value;
			double target_horizontal_value;
			double xValue;
			double yValue;
			double distance;
			double bearing;

			if (_targetSelected)
			{
				target_vertical_value = MapVerticalValue - BullseyeVerticalValue + _targetVerticalOffset;
				target_horizontal_value = MapHorizontalValue - BullseyeHorizontalValue + _targetHorizontalOffset;

				_MapOverlays.OwnshipVerticalValue = MapVerticalValue;
				_MapOverlays.OwnshipHorizontalValue = MapHorizontalValue;

				_MapOverlays.TargetVerticalValue = target_vertical_value;
				_MapOverlays.TargetHorizontalValue = target_horizontal_value;

				if (_targetTextRefreshCount == 10 || targetTextRefresh)
				{
					xValue = (target_horizontal_value - MapHorizontalValue) / _mapFeetPerNauticalMile;
					yValue = (target_vertical_value - MapVerticalValue) / _mapFeetPerNauticalMile;

					distance = GetHypotenuse(xValue, yValue);
					bearing = GetBearing(xValue, yValue);

					_MapOverlays.CourseDistance = distance;
					_MapOverlays.CourseBearing = bearing;
					_OverviewTextData.CourseDistance = distance;
					_OverviewTextData.CourseBearing = bearing;

					_targetTextRefreshCount = 0;
				}
				else
				{
					_targetTextRefreshCount++;
				}
			}
		}

		private void ProcessAircraftValues()
		{
			double xValue;
			double yValue;
			double xScaleValue;
			double yScaleValue;

			if (_overviewPanelEnabled)
			{
				xValue = BullseyeHorizontalValue / _mapFeetPerNauticalMile;
				yValue = BullseyeVerticalValue / _mapFeetPerNauticalMile;

				xScaleValue = xValue / _targetBullseyeScale;
				yScaleValue = - yValue / _targetBullseyeScale;

				if (Height >= Width)
				{
					_OverviewRangeRings.TapePosX = xScaleValue;
					_OverviewRangeRings.TapePosY = yScaleValue * _ratioWidthToHeight + _OverviewBullseye.PosY;

					_OverviewRangeRings.Tape_CenterX = (xScaleValue + _needleCenter.X);
					_OverviewRangeRings.Tape_CenterY = (yScaleValue + _needleCenter.Y) * _ratioWidthToHeight + _OverviewBullseye.PosY;

					_OverviewTargetLines.AircraftPositionX = xScaleValue * Width / _needleSize.Width + _widthCenterPosition;
					_OverviewTargetLines.AircraftPositionY = yScaleValue * Width / _needleSize.Width + _heightCenterPosition;
				}
				else
				{
					_OverviewRangeRings.TapePosX = xScaleValue * _ratioHeightToWidth + _OverviewBullseye.PosX;
					_OverviewRangeRings.TapePosY = yScaleValue;

					_OverviewRangeRings.Tape_CenterX = (xScaleValue + _needleCenter.X) * _ratioHeightToWidth + _OverviewBullseye.PosX;
					_OverviewRangeRings.Tape_CenterY = (yScaleValue + _needleCenter.Y);

					_OverviewTargetLines.AircraftPositionX = xScaleValue * Height / _needleSize.Height + _widthCenterPosition;
					_OverviewTargetLines.AircraftPositionY = yScaleValue * Height / _needleSize.Height + _heightCenterPosition;
				}

				_OverviewAircraft.TapePosX = _OverviewRangeRings.TapePosX;
				_OverviewAircraft.TapePosY = _OverviewRangeRings.TapePosY;

				_OverviewAircraft.Tape_CenterX = _OverviewRangeRings.Tape_CenterX;
				_OverviewAircraft.Tape_CenterY = _OverviewRangeRings.Tape_CenterY;

				AircraftDistance = GetHypotenuse(xValue, yValue);
				AircraftBearing = GetBearing(xValue, yValue);

				_OverviewTextData.AircraftDistance = AircraftDistance;
				_OverviewTextData.AircraftBearing = AircraftBearing;

				_OverviewRangeRings.Rotation = MapRotationAngle;
				_OverviewAircraft.Rotation = MapRotationAngle;
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
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		public override void Reset()
		{
			BeginTriggerBypass(true);

			_MapBullseye.IsHidden = true;
			_MapOverlays.ThreatsVisible = false;
			_MapOverlays.WaypointsVisible = false;
			_mapRotation_Enabled = false;
			MapRotationAngle = 0d;
			MapScaleChange(2d);
			ResetTargetOverview();

			EndTriggerBypass(true);
		}

		void ShowNoDataPanel()
		{
			_MapNoDataForeground.IsHidden = false;
			_MapNoDataBackground.IsHidden = false;
		}

		void HideNoDataPanel()
		{
			_MapNoDataForeground.IsHidden = true;
			_MapNoDataBackground.IsHidden = true;
		}

		void BullseyeVisible_Execute(object action, HeliosActionEventArgs e)
		{
			_bullseyeVisible.SetValue(e.Value, e.BypassCascadingTriggers);
			_MapBullseye.IsHidden = !_bullseyeVisible.Value.BoolValue;
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

		void OverviewPanelVisible_Execute(object action, HeliosActionEventArgs e)
		{
			_overviewPanelVisible.SetValue(e.Value, e.BypassCascadingTriggers);
			_overviewPanelEnabled = _overviewPanelVisible.Value.BoolValue;

			if (_overviewPanelEnabled)
			{
				ProcessAircraftValues();

				_OverviewBullseye.IsHidden = false;
				_OverviewBackground.IsHidden = false;
				_OverviewTextData.IsHidden = false;

				if (_targetSelected)
				{
					_OverviewTarget.IsHidden = false;
					_OverviewTargetLines.IsHidden = false;
				}

				if (AircraftDistance <= 120)
				{
					_OverviewAircraft.IsHidden = false;

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
					_OverviewAircraftRemote.IsHidden = false;
				}
			}
			else
			{
				_OverviewBullseye.IsHidden = true;
				_OverviewBackground.IsHidden = true;
				_OverviewTarget.IsHidden = true;

				_OverviewTextData.IsHidden = true;
				_OverviewTargetLines.IsHidden = true;
				
				_OverviewRangeRings.IsHidden = true;
				_OverviewAircraft.IsHidden = true;
				_OverviewAircraftRemote.IsHidden = true;

				Refresh();
			}
		}

		void OverviewClearTarget_Execute(object action, HeliosActionEventArgs e)
		{
			_overviewClearTarget.SetValue(e.Value, e.BypassCascadingTriggers);
			bool overviewClearTarget = _overviewClearTarget.Value.BoolValue;

			if (overviewClearTarget)
			{
				ClearSelectedTarget();
			}
		}

		void OverviewRangeRingsVisible_Execute(object action, HeliosActionEventArgs e)
		{
			_overviewRangeRingsVisible.SetValue(e.Value, e.BypassCascadingTriggers);
			_overviewRangeRingsEnabled = _overviewRangeRingsVisible.Value.BoolValue;
		}

		public override void MouseDown(Point location)
		{
			double target_posX;
			double target_posY;
			double distance_posX;
			double distance_posY;
			double bearing;
			double distance;

			if (!_OverviewBullseye.IsHidden)
			{
				target_posX = (location.X - _widthCenterPosition) * 200 / Width;
				target_posY = (location.Y - _heightCenterPosition) * 200 / Height;

				if (Height >= Width)
				{
					distance_posX = target_posX * _targetBullseyeScale;
					distance_posY = - target_posY * _targetBullseyeScale * _ratioHeightToWidth;
				}
				else
				{
					distance_posX = target_posX * _targetBullseyeScale * _ratioWidthToHeight;
					distance_posY = - target_posY * _targetBullseyeScale;
				}

				bearing = GetBearing(distance_posX, distance_posY);
				distance = GetHypotenuse(distance_posX, distance_posY);

				if (distance <= 125)
				{
					if (_OverviewTarget.IsHidden || !TargetClearOnTouch)
					{
						if (Height >= Width)
						{
							_OverviewTarget.PosX = target_posX;
							_OverviewTarget.PosY = target_posY + _OverviewBullseye.PosY;
						}
						else
						{
							_OverviewTarget.PosX = target_posX + _OverviewBullseye.PosX;
							_OverviewTarget.PosY = target_posY;
						}

						_targetSelected = true;
						_OverviewTarget.IsHidden = false;

						_OverviewTextData.TargetSelected = true;
						_OverviewTextData.TargetBearing = bearing;
						_OverviewTextData.TargetDistance = distance;
						_OverviewTextData.TargetPositionX = location.X;
						_OverviewTextData.TargetPositionY = location.Y;

						_OverviewTargetLines.IsHidden = false;
						_OverviewTargetLines.TargetPositionX = location.X;
						_OverviewTargetLines.TargetPositionY = location.Y;

						_MapOverlays.TargetVisible = true;
						_targetHorizontalOffset = distance_posX * _mapFeetPerNauticalMile;
						_targetVerticalOffset = distance_posY * _mapFeetPerNauticalMile;

						ProcessTargetValues(true);
						Refresh();
					}
					else
					{
						ClearSelectedTarget();
					}
				}
			}
		}

		void ClearSelectedTarget()
		{
			_targetSelected = false;
			_OverviewTarget.IsHidden = true;

			_OverviewTextData.TargetSelected = false;
			_OverviewTextData.TargetBearing = 0;
			_OverviewTextData.TargetDistance = 0;

			_OverviewTargetLines.IsHidden = true;

			_MapOverlays.TargetVisible = false;
			_targetHorizontalOffset = 0;
			_targetVerticalOffset = 0;

			ProcessTargetValues(true);
			Refresh();
		}

		void ResetTargetOverview()
		{
			_OverviewBullseye.IsHidden = true;
			_OverviewBackground.IsHidden = true;

			_targetSelected = false;
			_overviewPanelEnabled = false;
			_OverviewTarget.IsHidden = true;

			_OverviewTextData.IsHidden = true;
			_OverviewTextData.TargetSelected = false;
			_OverviewTextData.TargetBearing = 0;
			_OverviewTextData.TargetDistance = 0;

			_OverviewTargetLines.IsHidden = true;
			
			_MapOverlays.TargetVisible = false;
			_targetHorizontalOffset = 0;
			_targetVerticalOffset = 0;

			_overviewRangeRingsEnabled = true;
			_OverviewRangeRings.IsHidden = true;
			_OverviewAircraft.IsHidden = true;
			_OverviewAircraftRemote.IsHidden = true;
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
				_MapBullseye.Image = _mapBullseyeImage64;
			}
			else if (_mapSizeMultiplier == 2d)
			{
				_mapSizeFeet = _mapSizeFeet128;
				_MapBullseye.Image = _mapBullseyeImage128;
			}
			else
			{
				_mapSizeFeet = _mapSizeFeet64;
				_MapBullseye.Image = _mapBullseyeImage64;
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

			_widthCenterPosition = Width / 2;
			_heightCenterPosition = Height / 2;

			_ratioHeightToWidth = Height / Width;
			_ratioWidthToHeight = Width / Height;

			if (Height >= Width)
			{
				mapShortestSize = Width;
				_rangeScale = _mapBaseScale * _ratioWidthToHeight;
				rangeWidth = _needleSize.Width * _mapBaseScale;
				rangeHeight = _needleSize.Height * _rangeScale;

				rangeInitialHorizontal = (_needleSize.Width - rangeWidth) / 2d;
				rangeInitialVertical = (_needleSize.Height - rangeHeight) / 2d;

				squareWidth = _needleSize.Width;
				squareHeight = _needleSize.Height * _ratioWidthToHeight;
				squarePosX = 0d;
				squarePosY = _needleSize.Height * (1 - _ratioWidthToHeight) / 2d;

				_OverviewRangeRings.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y * _ratioWidthToHeight);
				_OverviewAircraft.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y * _ratioWidthToHeight);
				_OverviewTargetLines.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X, _needleCenter.Y * _ratioWidthToHeight);
			}
			else
			{
				mapShortestSize = Height;
				_rangeScale = _mapBaseScale * _ratioHeightToWidth;
				rangeWidth = _needleSize.Width * _rangeScale;
				rangeHeight = _needleSize.Height * _mapBaseScale;

				rangeInitialHorizontal = (_needleSize.Width - rangeWidth) / 2d;
				rangeInitialVertical = (_needleSize.Height - rangeHeight) / 2d;

				squareWidth = _needleSize.Width * _ratioHeightToWidth;
				squareHeight = _needleSize.Height;
				squarePosX = _needleSize.Width * (1 - _ratioHeightToWidth) / 2d;
				squarePosY = 0d;

				_OverviewRangeRings.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X * _ratioHeightToWidth, _needleCenter.Y);
				_OverviewAircraft.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X * _ratioHeightToWidth, _needleCenter.Y);
				_OverviewTargetLines.Clip = new EllipseGeometry(_needleCenter, _needleCenter.X * _ratioHeightToWidth, _needleCenter.Y);
			}

			_MapOverlays.MapShortestSize = mapShortestSize;

			_MapRangeRings.Tape_Width = rangeWidth;
			_MapAircraft.Tape_Width = rangeWidth;
			_rangeInitialHorizontal = Convert.ToInt32(rangeInitialHorizontal);

			_MapRangeRings.Tape_Height = rangeHeight;
			_MapAircraft.Tape_Height = rangeHeight;
			_rangeInitialVertical = Convert.ToInt32(rangeInitialVertical);

			_OverviewBullseye.Width = squareWidth;
			_OverviewBullseye.Height = squareHeight;
			_OverviewBullseye.PosX = squarePosX;
			_OverviewBullseye.PosY = squarePosY;

			_OverviewTarget.Width = squareWidth;
			_OverviewTarget.Height = squareHeight;
			_OverviewTarget.PosX = squarePosX;
			_OverviewTarget.PosY = squarePosY;

			_OverviewRangeRings.Tape_Width = squareWidth;
			_OverviewRangeRings.Tape_Height = squareHeight;
			_OverviewRangeRings.TapePosX = squarePosX;
			_OverviewRangeRings.TapePosY = squarePosY;

			_OverviewAircraft.Tape_Width = squareWidth;
			_OverviewAircraft.Tape_Height = squareHeight;
			_OverviewAircraft.TapePosX = squarePosX;
			_OverviewAircraft.TapePosY = squarePosY;

			_OverviewAircraftRemote.Tape_Width = squareWidth;
			_OverviewAircraftRemote.Tape_Height = squareHeight;
			_OverviewAircraftRemote.TapePosX = squarePosX;
			_OverviewAircraftRemote.TapePosY = squarePosY;

			_MapNoDataForeground.Width = squareWidth;
			_MapNoDataForeground.Height = squareHeight;
			_MapNoDataForeground.PosX = squarePosX;
			_MapNoDataForeground.PosY = squarePosY;

			_OverviewTextData.MapControlWidth = Width;
			_OverviewTextData.MapControlHeight = Height;
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

				mapWidth = _needleSize.Width * _ratioHeightToWidth * mapScale;
				mapInitialHorizontal = (_needleSize.Width - mapWidth) / 2d;
				mapHeight = _needleSize.Height * mapScale;
				mapInitialVertical = (_needleSize.Height - mapHeight) / 2d;
			}
			else
			{
				mapScale = _rangeScale * _mapScaleMultiplier * _mapSizeMultiplier;

				mapHeight = _needleSize.Height * _ratioWidthToHeight * mapScale;
				mapInitialVertical = (_needleSize.Height - mapHeight) / 2d;
				mapWidth = _needleSize.Width * mapScale;
				mapInitialHorizontal = (_needleSize.Width - mapWidth) / 2d;
			}

			_mapModifiedScale = mapScale;

			_MapOverlays.MapWidth = mapWidth;
			_MapOverlays.MapHeight = mapHeight;
			_MapOverlays.MapSizeFeet = _mapSizeFeet;
			_MapOverlays.MapScaleMultiplier = _mapScaleMultiplier;

			_Map.Tape_Width = mapWidth;
			_MapOverlays.Tape_Width = mapWidth;
			_MapBullseye.Tape_Width = mapWidth;
			_mapInitialHorizontal = Convert.ToInt32(mapInitialHorizontal);

			_Map.Tape_Height = mapHeight;
			_MapOverlays.Tape_Height = mapHeight;
			_MapBullseye.Tape_Height = mapHeight;
			_mapInitialVertical = Convert.ToInt32(mapInitialVertical);

			_MapRangeRings.HorizontalOffset = _rangeInitialHorizontal;
			_MapRangeRings.VerticalOffset = _rangeInitialVertical;
			_MapAircraft.HorizontalOffset = _rangeInitialHorizontal;
			_MapAircraft.VerticalOffset = _rangeInitialVertical;

			ProcessTargetValues(true);

			Refresh();

			if (mapResized)
			{
				_Map.HorizontalOffset = _mapInitialHorizontal;
				_Map.VerticalOffset = _mapInitialVertical;
				_MapOverlays.HorizontalOffset = _mapInitialHorizontal;
				_MapOverlays.VerticalOffset = _mapInitialVertical;
				_MapBullseye.HorizontalOffset = _mapInitialHorizontal;
				_MapBullseye.VerticalOffset = _mapInitialVertical;
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
				_MapBullseye.VerticalOffset = _mapInitialVertical + (bullseyeVerticalValue / _mapSizeFeet * _mapModifiedScale * 200);
			}
			else
			{
				_MapBullseye.VerticalOffset = _mapInitialVertical + (bullseyeVerticalValue / _mapSizeFeet * _mapBaseScale * _mapScaleMultiplier * _mapSizeMultiplier * 200);
			}
		}

		void BullseyeHorizontalOffset_Calculate(double bullseyeHorizontalValue)
		{
			if (Height >= Width)
			{
				_MapBullseye.HorizontalOffset = _mapInitialHorizontal - (bullseyeHorizontalValue / _mapSizeFeet * _mapBaseScale * _mapScaleMultiplier * _mapSizeMultiplier * 200);
			}
			else
			{
				_MapBullseye.HorizontalOffset = _mapInitialHorizontal - (bullseyeHorizontalValue / _mapSizeFeet * _mapModifiedScale * 200);
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
				_MapBullseye.Rotation = -angle;
				_MapRangeRings.Rotation = -angle;
				_MapAircraft.Rotation = 0d;
			}
			else
			{
				_Map.Rotation = 0d;
				_MapOverlays.Rotation = 0d;
				_MapBullseye.Rotation = 0d;
				_MapRangeRings.Rotation = 0d;
				_MapAircraft.Rotation = angle;
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

		public double AircraftBearing
		{
			get
			{
				return _aircraftBearing;
			}
			set
			{
				double oldValue = _aircraftBearing;
				_aircraftBearing = value;
				if (!_aircraftBearing.Equals(oldValue))
				{
					Refresh();
				}
			}
		}

		public double AircraftDistance
		{
			get
			{
				return _aircraftDistance;
			}
			set
			{
				double oldValue = _aircraftDistance;
				_aircraftDistance = value;
				if (!_aircraftDistance.Equals(oldValue))
				{
					Refresh();
				}
			}
		}

		#endregion

	}
}
