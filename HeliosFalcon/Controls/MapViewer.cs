//  Copyright 2014 Craig Courtney
//  Copyright 2021 Helios Contributors
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


	[HeliosControl("Helios.Falcon.MapViewer", "Map Viewer", "Falcon Simulator", typeof(Gauges.GaugeRenderer))]

	public class MapViewer : MapControls
	{
		private FalconInterface _falconInterface;

		private Gauges.GaugeImage _Background;
		private Gauges.CustomGaugeNeedle _Map;
		private Gauges.CustomGaugeNeedle _MapZoomIn;

		private Rect _imageSize = new Rect(0d, 0d, 200d, 200d);
		private Size _needleSize = new Size(200d, 200d);
		private Rect _needleClip = new Rect(1d, 1d, 198d, 198d);
		private Point _needleLocation = new Point(0d, 0d);
		private Point _needleCenter = new Point(100d, 100d);

		private const string _backgroundImage = "{HeliosFalcon}/Images/MapControl/Background 02.png";
		private string _lastTheater;

		private double _mapSize = 200d;
		private double _mapMultiplier = 4d;
		private double _mapWidthScale = 0d;
		private double _mapHeightScale = 0d;
		private double _mapOffsetHorizontal = 0d;
		private double _mapOffsetVertical = 0d;
		private double _xMinValue = 0d;
		private double _xMaxValue = 0d;
		private double _yMinValue = 0d;
		private double _yMaxValue = 0d;


		public MapViewer()
			: base("MapViewer", new Size(200d, 200d))
		{
			_Background = new Gauges.GaugeImage(_backgroundImage, _imageSize);
			Components.Add(_Background);

			_Map = new Gauges.CustomGaugeNeedle(_mapBaseImages[7, 1], _needleLocation, _needleSize, _needleCenter);
			_Map.Clip = new RectangleGeometry(_needleClip);
			_Map.ImageRefresh = true;
			Components.Add(_Map);

			_MapZoomIn = new Gauges.CustomGaugeNeedle(_mapBaseImages[7, 1], _needleLocation, _needleSize, _needleCenter);
			_MapZoomIn.Clip = new RectangleGeometry(_needleClip);
			_MapZoomIn.ImageRefresh = true;
			_MapZoomIn.IsHidden = true;
			Components.Add(_MapZoomIn);
			
			MapResize();
			Resized += new EventHandler(OnMapControl_Resized);
		}


		#region Actions

		public override void MouseDown(Point location)
		{
			if (_MapZoomIn.IsHidden)
			{
				if (location.X >= _xMinValue && location.X <= _xMaxValue && location.Y >= _yMinValue && location.Y <= _yMaxValue)
				{
					_MapZoomIn.IsHidden = false;
					_Map.IsHidden = true;

					SetMapZoomInOffsets(location.X, location.Y);
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
				string theater = _falconInterface.CurrentTheater;

				if (!string.IsNullOrEmpty(theater) && theater != _lastTheater)
				{
					_lastTheater = theater;
					TheaterMapSelect(theater);
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
					}

					if (_MapZoomIn.Image != mapImages[i, 1])
					{
						_MapZoomIn.Image = mapImages[i, 1];
					}

					Refresh();
				}
			}
		}

		#endregion


		#region Map Scaling

		void OnMapControl_Resized(object sender, EventArgs e)
		{
			MapResize();
		}

		void MapResize()
		{
			double mapWidth;
			double mapHeight;
			double mapOffsetHorizontal;
			double mapOffsetVertical;
			double mapWidthZoomIn;
			double mapHeightZoomIn;

			double xMinValue;
			double xMaxValue;
			double yMinValue;
			double yMaxValue;

			if (Height >= Width)
			{
				mapWidth = _mapSize;
				mapOffsetHorizontal = 0d;
				mapHeight = _mapSize * Width / Height;
				mapOffsetVertical = (_mapSize - mapHeight) / 2d;

				xMinValue = 0d;
				xMaxValue = mapWidth;
				yMinValue = mapOffsetVertical;
				yMaxValue = mapOffsetVertical + mapHeight;

				mapWidthZoomIn = _mapSize * _mapMultiplier;
				mapHeightZoomIn = _mapSize * Width / Height * _mapMultiplier;
			}
			else
			{
				mapWidth = _mapSize * Height / Width;
				mapOffsetHorizontal = (_mapSize - mapWidth) / 2d;
				mapHeight = _mapSize;
				mapOffsetVertical = 0d;

				xMinValue = mapOffsetHorizontal;
				xMaxValue = mapOffsetHorizontal + mapWidth;
				yMinValue = 0;
				yMaxValue = mapHeight;

				mapWidthZoomIn = _mapSize * Height / Width * _mapMultiplier;
				mapHeightZoomIn = _mapSize * _mapMultiplier;
			}

			_mapWidthScale = Width / _mapSize;
			_mapHeightScale = Height / _mapSize;

			_xMinValue = xMinValue * _mapWidthScale;
			_xMaxValue = xMaxValue * _mapWidthScale;
			_yMinValue = yMinValue * _mapHeightScale;
			_yMaxValue = yMaxValue * _mapHeightScale;

			_Map.Tape_Width = mapWidth;
			_Map.Tape_Height = mapHeight;

			_mapOffsetHorizontal = mapOffsetHorizontal;
			_mapOffsetVertical = mapOffsetVertical;

			_Map.HorizontalOffset = mapOffsetHorizontal;
			_Map.VerticalOffset = mapOffsetVertical;

			_MapZoomIn.Tape_Width = mapWidthZoomIn;
			_MapZoomIn.Tape_Height = mapHeightZoomIn;

			Refresh();
		}

		void SetMapZoomInOffsets(double xPos, double yPos)
		{
			double xMapPos;
			double yMapPos;

			xMapPos = (_mapOffsetHorizontal - xPos / _mapWidthScale) * _mapMultiplier;
			yMapPos = (_mapOffsetVertical - yPos / _mapHeightScale) * _mapMultiplier;

			_MapZoomIn.HorizontalOffset = xMapPos + _mapSize / 2d;
			_MapZoomIn.VerticalOffset = yMapPos + _mapSize / 2d;

			Refresh();
		}

		#endregion

	}
}
