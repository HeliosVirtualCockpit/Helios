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


	[HeliosControl("Helios.Falcon.MapViewer", "Map Viewer", "Falcon Simulator", typeof(Gauges.GaugeRenderer))]

	public class MapViewer : MapControls
	{
		private FalconInterface _falconInterface;

		private Gauges.GaugeImage _MapBackground;
		private Gauges.CustomGaugeNeedle _Map;
		private Gauges.CustomGaugeNeedle _MapZoomIn;
		private MapViewerRenderer _MapOverlay;

		private Rect _imageSize = new Rect(0d, 0d, 200d, 200d);
		private Size _needleSize = new Size(200d, 200d);
		private Rect _needleClip = new Rect(1d, 1d, 198d, 198d);
		private Point _needleLocation = new Point(0d, 0d);
		private Point _needleCenter = new Point(100d, 100d);

		private const string _mapBackgroundImage = "{HeliosFalcon}/Images/MapControl/MapViewer Background.png";
		private string _lastTheater;
		private bool _navPointsInitialized = false;

		private const double _mapSizeFeet64 = 3358700;   // 1024 km x 3279.98 ft/km (BMS conversion value)
		private const double _mapSizeFeet128 = 6717400;  // 2048 km x 3279.98 ft/km (BMS conversion value)

		private double _mapSizeFeet = 3358700;
		private double _mapSizeMultiplier = 1d;   // 1d = 64 Segment, 2d = 128 Segment
		private double _mapSize = 200d;
		private double _mapMultiplier = 4d;
		private double _baseMapWidth = 0d;
		private double _baseMapHeight = 0d;
		private double _mapWidthScale = 0d;
		private double _mapHeightScale = 0d;
		private double _xMinValue = 0d;
		private double _xMaxValue = 0d;
		private double _yMinValue = 0d;
		private double _yMaxValue = 0d;

		
		public MapViewer()
			: base("MapViewer", new Size(200d, 200d))
		{
			AddComponents();
			BaseMapResize();
			Resized += new EventHandler(OnMapControl_Resized);
		}


		#region Components

		void AddComponents()
		{
			_MapBackground = new Gauges.GaugeImage(_mapBackgroundImage, _imageSize);
			Components.Add(_MapBackground);

			_Map = new Gauges.CustomGaugeNeedle(_mapBaseImages[7, 1], _needleLocation, _needleSize, _needleCenter);
			_Map.Clip = new RectangleGeometry(_needleClip);
			_Map.ImageRefresh = true;
			Components.Add(_Map);

			_MapZoomIn = new Gauges.CustomGaugeNeedle(_mapBaseImages[7, 1], _needleLocation, _needleSize, _needleCenter);
			_MapZoomIn.Clip = new RectangleGeometry(_needleClip);
			_MapZoomIn.ImageRefresh = true;
			_MapZoomIn.IsHidden = true;
			Components.Add(_MapZoomIn);

			_MapOverlay = new MapViewerRenderer(_needleLocation, _needleSize);
			_MapOverlay.Clip = new RectangleGeometry(_needleClip);
			_MapOverlay.IsHidden = true;
			Components.Add(_MapOverlay);
		}

		#endregion Components


		#region Methods

		public override void MouseDown(Point location)
		{
			if (_MapZoomIn.IsHidden)
			{
				if (location.X >= _xMinValue && location.X <= _xMaxValue && location.Y >= _yMinValue && location.Y <= _yMaxValue)
				{
					_Map.IsHidden = true;
					_MapZoomIn.IsHidden = false;

					MapZoomInResize(location.X, location.Y);
				}
				else
				{
					this.IsHidden = true;
				}
			}
			else
			{
				_Map.IsHidden = false;
				_MapZoomIn.IsHidden = true;

				MapOverlayResize();
			}
		}

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

				string theater = _falconInterface.CurrentTheater;

				if (!string.IsNullOrEmpty(theater) && theater != _lastTheater)
				{
					_lastTheater = theater;
					TheaterMapSelect(theater);
				}

				if (inFlight)
				{
					_MapOverlay.IsHidden = false;

					if (!_navPointsInitialized)
					{
						List<string> navPoints = _falconInterface.NavPoints;

						if (navPoints != null && navPoints.Any())
						{
							_MapOverlay.ProcessNavPointValues(navPoints);
							_navPointsInitialized = true;
							Refresh();
						}
					}
				}

				if (!inFlight)
				{
					_MapOverlay.IsHidden = true;
					_navPointsInitialized = false;
					Refresh();
				}
			}
		}

		void Profile_ProfileStopped(object sender, EventArgs e)
		{
			_falconInterface = null;
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods


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
						_MapZoomIn.Image = mapImages[i, 1];

						_mapSizeMultiplier = Convert.ToDouble(mapImages[i, 2]);

						if (_mapSizeMultiplier == 1d)
						{
							_mapSizeFeet = _mapSizeFeet64;
							_MapOverlay.MapSizeMultiplier = 1d;
						}
						else if (_mapSizeMultiplier == 2d)
						{
							_mapSizeFeet = _mapSizeFeet128;
							_MapOverlay.MapSizeMultiplier = 2d;
						}
						else
						{
							_mapSizeFeet = _mapSizeFeet64;
							_MapOverlay.MapSizeMultiplier = 1d;
						}

						_Map.IsHidden = false;
						_MapZoomIn.IsHidden = true;

						BaseMapResize();
					}
				}
			}
		}

		#endregion Map Selection


		#region Scaling

		void OnMapControl_Resized(object sender, EventArgs e)
		{
			BaseMapResize();
		}

		void BaseMapResize()
		{
			double mapOffsetHorizontal;
			double mapOffsetVertical;
			double mapWidthZoomIn;
			double mapHeightZoomIn;
			double mapShortestSize = 0d;

			double xMinValue;
			double xMaxValue;
			double yMinValue;
			double yMaxValue;

			if (Height >= Width)
			{
				_baseMapWidth = _mapSize;
				_baseMapHeight = _mapSize * Width / Height;
				mapOffsetHorizontal = 0d;
				mapOffsetVertical = (_mapSize - _baseMapHeight) / 2d;

				xMinValue = 0d;
				xMaxValue = _baseMapWidth;
				yMinValue = mapOffsetVertical;
				yMaxValue = mapOffsetVertical + _baseMapHeight;

				mapWidthZoomIn = _baseMapWidth * _mapMultiplier;
				mapHeightZoomIn = _baseMapHeight * _mapMultiplier;
				mapShortestSize = Width;
			}
			else
			{
				_baseMapHeight = _mapSize;
				_baseMapWidth = _mapSize * Height / Width;
				mapOffsetVertical = 0d;
				mapOffsetHorizontal = (_mapSize - _baseMapWidth) / 2d;

				xMinValue = mapOffsetHorizontal;
				xMaxValue = mapOffsetHorizontal + _baseMapWidth;
				yMinValue = 0;
				yMaxValue = _baseMapHeight;

				mapWidthZoomIn = _baseMapWidth * _mapMultiplier;
				mapHeightZoomIn = _baseMapHeight * _mapMultiplier;
				mapShortestSize = Height;
			}

			_mapWidthScale = Width / _mapSize;
			_mapHeightScale = Height / _mapSize;

			_xMinValue = xMinValue * _mapWidthScale;
			_xMaxValue = xMaxValue * _mapWidthScale;
			_yMinValue = yMinValue * _mapHeightScale;
			_yMaxValue = yMaxValue * _mapHeightScale;

			_Map.Tape_Width = _baseMapWidth;
			_Map.Tape_Height = _baseMapHeight;
			_Map.HorizontalOffset = mapOffsetHorizontal;
			_Map.VerticalOffset = mapOffsetVertical;

			_MapZoomIn.Tape_Width = mapWidthZoomIn;
			_MapZoomIn.Tape_Height = mapHeightZoomIn;
			_MapOverlay.MapShortestSize = mapShortestSize;

			MapOverlayResize();
		}

		void MapOverlayResize()
		{
			_MapOverlay.MapSizeFeet = _mapSizeFeet;
			_MapOverlay.MapScaleMultiplier = 1d;

			_MapOverlay.MapWidth = _Map.Tape_Width;
			_MapOverlay.MapHeight = _Map.Tape_Height;
			_MapOverlay.Tape_Width = _MapZoomIn.Tape_Width;
			_MapOverlay.Tape_Height = _MapZoomIn.Tape_Height;
			_MapOverlay.HorizontalOffset = _Map.HorizontalOffset;
			_MapOverlay.VerticalOffset = _Map.VerticalOffset;

			Refresh();
		}

		void MapZoomInResize(double xPos, double yPos)
		{
			double xMapPos;
			double yMapPos;

			xMapPos = (_Map.HorizontalOffset - xPos / _mapWidthScale) * _mapMultiplier;
			yMapPos = (_Map.VerticalOffset - yPos / _mapHeightScale) * _mapMultiplier;

			_MapOverlay.MapSizeFeet = _mapSizeFeet;
			_MapOverlay.MapScaleMultiplier = _mapMultiplier;

			_MapOverlay.MapWidth = _baseMapWidth * _mapMultiplier;
			_MapOverlay.MapHeight = _baseMapHeight * _mapMultiplier;
			_MapOverlay.Tape_Width = _baseMapWidth * _mapMultiplier;
			_MapOverlay.Tape_Height = _baseMapHeight * _mapMultiplier;

			_MapOverlay.HorizontalOffset = xMapPos + _mapSize / 2d;
			_MapOverlay.VerticalOffset = yMapPos + _mapSize / 2d;

			_MapZoomIn.HorizontalOffset = xMapPos + _mapSize / 2d;
			_MapZoomIn.VerticalOffset = yMapPos + _mapSize / 2d;

			Refresh();
		}

		#endregion Scaling

	}
}
