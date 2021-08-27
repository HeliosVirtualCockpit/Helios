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
	using GadrocsWorkshop.Helios.Gauges;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Media;


	public class MapViewerRenderer : GaugeComponent
	{
		private Point _location;
		private Size _size;
		private double _horizontalOffset;
		private double _verticalOffset;
		private double _xScale = 1.0d;
		private double _yScale = 1.0d;
		private double _mapWidth;
		private double _mapHeight;
		private double _mapSizeFeet;
		private double _pixelsPerDip = 1.0d;

		private CultureInfo _provider = new CultureInfo("en-US");

		private double[,] _navPoints_WP = new double[24, 2];
		private double[,] _navPoints_PO = new double[19, 2];
		private double[,] _navPoints_PT = new double[15, 3];
		private string[] _navNames_PT = new string[15];

		private double[,] _mapPoints_WP = new double[24, 2];
		private double[,] _mapPoints_PO = new double[19, 2];
		private double[,] _mapPoints_PT = new double[15, 3];

		private ImageSource[] _waypointImages = new ImageSource[24];
		private ImageSource[] _pospointImages = new ImageSource[19];
		private Rect[] _waypointRect = new Rect[24];
		private Rect[] _pospointRect = new Rect[19];

		private const double _minThreatCircleRadius = 30000d;
		private const double _mapBaseUnit = 1000d;
		private const double _mapScaleModifier = 1.2d;
		private double _mapScaleUnit;
		private const double _navpointBaseScale = 60d;
		private double _navpointSize;
		private double _navpointLineWidth;

		private Pen _linePen;
		private Brush _lineBrush;
		private Brush _pointFillBrush;
		private Brush _backgroundFillBrush;
		private Brush _transparentFillBrush;

		private FormattedText _formattedText;
		private const double _fontBaseSize = 30d;
		private double _fontScaleSize;


		public MapViewerRenderer(Point location, Size size)
		{
			_location = location;
			_size = size;

			GetPixelsPerDip();
			InitializeImageArrays();
		}


		#region Actions

		void InitializeImageArrays()
		{
			string imagePath;

			for (int i = 0; i < _waypointImages.GetLength(0); i++)
			{
				imagePath = "{HeliosFalcon}/Images/Navpoints/Waypoint_" + (i + 1).ToString("D2") + ".png";

				_waypointImages[i] = ConfigManager.ImageManager.LoadImage(imagePath, 120, 120);
			}

			for (int i = 0; i < _pospointImages.GetLength(0); i++)
			{
				imagePath = "{HeliosFalcon}/Images/Navpoints/Pospoint_" + (i + 81).ToString("D2") + ".png";

				_pospointImages[i] = ConfigManager.ImageManager.LoadImage(imagePath, 120, 120);
			}
		}

		internal void ProcessNavPointValues(List<string> navPoints)
		{
			int lenWP = _navPoints_WP.GetLength(0);
			int lenPO = _navPoints_PO.GetLength(0);
			int lenPT = _navPoints_PT.GetLength(0);

			int posWP = 0;
			int posPO = 0;
			int posPT = 0;

			Array.Clear(_navPoints_WP, 0, _navPoints_WP.Length);
			Array.Clear(_navPoints_PO, 0, _navPoints_PO.Length);
			Array.Clear(_navPoints_PT, 0, _navPoints_PT.Length);
			Array.Clear(_navNames_PT, 0, _navNames_PT.Length);

			foreach (string navLine in navPoints)
			{
				string[] navLineValues = navLine.Split(',');

				if (navLineValues.Length >= 4)
				{
					switch (navLineValues[1])
					{
						case "WP": // (WAYPOINT)
							{
								if (posWP < lenWP)
								{
									_navPoints_WP[posWP, 0] = NavPointToDouble(navLineValues[3]);
									_navPoints_WP[posWP, 1] = NavPointToDouble(navLineValues[2]);
									posWP++;
								}
								break;
							}
						case "PO": // (POSPOINT)
							{
								string[] navLineIndexValues = navLineValues[0].Split(':');

								if (navLineIndexValues.GetLength(0) == 2)
								{
									posPO = Convert.ToInt32(navLineIndexValues[1]) - 81;

									if (posPO >= 0 && posPO < lenPO)
									{
										_navPoints_PO[posPO, 0] = NavPointToDouble(navLineValues[3]);
										_navPoints_PO[posPO, 1] = NavPointToDouble(navLineValues[2]);
									}
								}
								break;
							}
						case "PT": // (PREPLANNEDTHREAT)
							{
								if (posPT < lenPT)
								{
									_navPoints_PT[posPT, 0] = NavPointToDouble(navLineValues[3]);
									_navPoints_PT[posPT, 1] = NavPointToDouble(navLineValues[2]);

									if (navLineValues.Length >= 7)
									{
										_navPoints_PT[posPT, 2] = NavPointToDouble(navLineValues[6]);
										_navNames_PT[posPT] = NavPointToName(navLineValues[5]);
									}
									posPT++;
								}
								break;
							}
					}
				}
			}
		}

		double NavPointToDouble(string navValue)
		{
			try
			{
				return decimal.ToDouble(decimal.Parse(navValue, NumberStyles.Float, _provider));
			}
			catch
			{
				return 0d;
			}
		}

		string NavPointToName(string navName)
		{
			string[] navNameValues = navName.Split(':');
			if (navNameValues.Length >= 2)
			{
				return navNameValues[1].Trim('"');
			}
			else
			{
				return "";
			}
		}

		void GetPixelsPerDip()
		{
			DisplayManager displayManager = new DisplayManager();

			if (displayManager.PixelsPerDip != 0d)
			{
				_pixelsPerDip = displayManager.PixelsPerDip;
			}
		}

		#endregion Actions


		#region Drawing

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			drawingContext.PushTransform(new TranslateTransform((_location.X + HorizontalOffset) * _xScale, (_location.Y + VerticalOffset) * _yScale));

			DrawThreatCircles(drawingContext);
			DrawThreatNames(drawingContext);
			DrawWaypointLines(drawingContext);
			DrawWaypointImages(drawingContext);
			DrawPospointImages(drawingContext);

			drawingContext.Pop();
		}

		void DrawThreatCircles(DrawingContext drawingContext)
		{
			_lineBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
			_linePen = new Pen(_lineBrush, _mapScaleUnit * 4d);
			_pointFillBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
			_transparentFillBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0)) {Opacity = 0.15d};

			for (int i = 0; i < _mapPoints_PT.GetLength(0); i++)
			{
				if (_mapPoints_PT[i, 0] > 0d && _mapPoints_PT[i, 1] > 0d)
				{
					if (_mapPoints_PT[i, 2] > 0d)
					{
						drawingContext.DrawEllipse(_transparentFillBrush, _linePen, new Point(_mapPoints_PT[i, 0], _mapPoints_PT[i, 1]), _mapPoints_PT[i, 2], _mapPoints_PT[i, 2]);
					}

					drawingContext.DrawEllipse(_pointFillBrush, null, new Point(_mapPoints_PT[i, 0], _mapPoints_PT[i, 1]), _mapScaleUnit * 12d, _mapScaleUnit * 12d);
				}
			}
		}

		void DrawThreatNames(DrawingContext drawingContext)
		{
			double sizeOffset = _fontScaleSize * 0.1d;
			double xPosOffset = _mapScaleUnit * 20d;
			double yPosOffset = _mapScaleUnit * 15d + _fontScaleSize * 1.1d;

			_lineBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
			_linePen = new Pen(_lineBrush, _fontScaleSize * 0.11d);
			_backgroundFillBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

			for (int i = 0; i < _mapPoints_PT.GetLength(0); i++)
			{
				if (_mapPoints_PT[i, 0] > 0d && _mapPoints_PT[i, 1] > 0d && !string.IsNullOrEmpty(_navNames_PT[i]))
				{
					double xPos = _mapPoints_PT[i, 0] + xPosOffset;
					double yPos = _mapPoints_PT[i, 1] - yPosOffset;

					_formattedText = new FormattedText(_navNames_PT[i], CultureInfo.GetCultureInfo("en-us"),
						FlowDirection.LeftToRight, new Typeface("Lucida Console Regular"), _fontScaleSize, Brushes.Black, _pixelsPerDip);

					Rect _textBounds = new Rect(xPos - sizeOffset, yPos - sizeOffset, _formattedText.Width + sizeOffset * 2.5d, _formattedText.Height + sizeOffset);

					drawingContext.DrawRectangle(_backgroundFillBrush, _linePen, _textBounds);

					drawingContext.DrawText(_formattedText, new Point(xPos, yPos));
				}
			}
		}

		void DrawWaypointLines(DrawingContext drawingContext)
		{
			_lineBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
			_linePen = new Pen(_lineBrush, _navpointLineWidth) {DashStyle = DashStyles.Dash};

			for (int i = 0; i < _mapPoints_WP.GetLength(0) - 1; i++)
			{
				if (_mapPoints_WP[i, 0] == 0d || _mapPoints_WP[i + 1, 0] == 0d)
					break;

				drawingContext.DrawLine(_linePen, new Point(_mapPoints_WP[i, 0], _mapPoints_WP[i, 1]), new Point(_mapPoints_WP[i + 1, 0], _mapPoints_WP[i + 1, 1]));
			}
		}

		void DrawWaypointImages(DrawingContext drawingContext)
		{
			for (int i = 0; i < _navPoints_WP.GetLength(0); i++)
			{
				if (_navPoints_WP[i, 0] > 0d && _navPoints_WP[i, 1] > 0d)
				{
					drawingContext.DrawImage(_waypointImages[i], _waypointRect[i]);
				}
				else
				{
					break;
				}
			}
		}

		void DrawPospointImages(DrawingContext drawingContext)
		{
			for (int i = 0; i < _navPoints_PO.GetLength(0); i++)
			{
				if (_navPoints_PO[i, 0] > 0d && _navPoints_PO[i, 1] > 0d)
				{
					drawingContext.DrawImage(_pospointImages[i], _pospointRect[i]);
				}
			}
		}

		protected override void OnRefresh(double xScale, double yScale)
		{
			_xScale = xScale;
			_yScale = yScale;

			_mapScaleUnit = FeetToMapUnits_ScaleUnit(_mapBaseUnit, _xScale, _yScale);
			_fontScaleSize = _fontBaseSize * _mapScaleUnit * MapSizeMultiplier;
			_navpointSize = MapShortestSize / _navpointBaseScale * MapScaleMultiplier;

			if (MapScaleMultiplier == 1d)
			{
				_fontScaleSize = _fontScaleSize * _mapScaleModifier;
				_navpointSize = _navpointSize * _mapScaleModifier;
			}

			_navpointLineWidth = _navpointSize / 10d;

			double navpointOffset = _navpointSize / 2;

			for (int i = 0; i < _navPoints_WP.GetLength(0); i++)
			{
				_mapPoints_WP[i, 0] = FeetToMapUnits_X(_navPoints_WP[i, 0], _xScale);
				_mapPoints_WP[i, 1] = FeetToMapUnits_Y(_navPoints_WP[i, 1], _yScale);
			}

			for (int i = 0; i < _navPoints_PO.GetLength(0); i++)
			{
				_mapPoints_PO[i, 0] = FeetToMapUnits_X(_navPoints_PO[i, 0], _xScale);
				_mapPoints_PO[i, 1] = FeetToMapUnits_Y(_navPoints_PO[i, 1], _yScale);
			}

			for (int i = 0; i < _navPoints_PT.GetLength(0); i++)
			{
				_mapPoints_PT[i, 0] = FeetToMapUnits_X(_navPoints_PT[i, 0], _xScale);
				_mapPoints_PT[i, 1] = FeetToMapUnits_Y(_navPoints_PT[i, 1], _yScale);
				_mapPoints_PT[i, 2] = FeetToMapUnits_CircleRadius(_navPoints_PT[i, 2], _xScale, _yScale);
			}

			for (int i = 0; i < _waypointRect.GetLength(0); i++)
			{
				_waypointRect[i] = new Rect(_mapPoints_WP[i, 0] - navpointOffset, _mapPoints_WP[i, 1] - navpointOffset, _navpointSize, _navpointSize);
			}

			for (int i = 0; i < _pospointRect.GetLength(0); i++)
			{
				_pospointRect[i] = new Rect(_mapPoints_PO[i, 0] - navpointOffset, _mapPoints_PO[i, 1] - navpointOffset, _navpointSize, _navpointSize);
			}
		}

		double FeetToMapUnits_X(double xPosFeet, double xScale)
		{
			if (xPosFeet > 0d)
			{
				return xPosFeet / MapSizeFeet * MapWidth * xScale;
			}
			else
			{
				return 0d;
			}
		}

		double FeetToMapUnits_Y(double yPosFeet, double yScale)
		{
			if (yPosFeet > 0d)
			{
				return (MapSizeFeet - yPosFeet) / MapSizeFeet * MapHeight * yScale;
			}
			else
			{
				return 0d;
			}
		}

		double FeetToMapUnits_CircleRadius(double radiusFeet, double xScale, double yScale)
		{
			if (radiusFeet < _minThreatCircleRadius)
			{
				return 0d;
			}
			else if (MapHeight >= MapWidth)
			{
				return radiusFeet / MapSizeFeet * MapWidth * xScale;
			}
			else
			{
				return radiusFeet / MapSizeFeet * MapHeight * yScale;
			}
		}

		double FeetToMapUnits_ScaleUnit(double unit, double xScale, double yScale)
		{
			if (MapHeight >= MapWidth)
			{
				return unit / MapSizeFeet * MapWidth * xScale;
			}
			else
			{
				return unit / MapSizeFeet * MapHeight * yScale;
			}
		}

		#endregion Drawing


		#region Properties

		public double MapShortestSize { get; set; }

		public double MapScaleMultiplier { get; set; }

		public double MapSizeMultiplier { get; set; } = 1d;

		public double MapWidth
		{
			get
			{
				return _mapWidth;
			}
			set
			{
				double newValue = Math.Round(value, 1);
				if (!_mapWidth.Equals(newValue))
				{
					_mapWidth = value;
				}
			}
		}

		public double MapHeight
		{
			get
			{
				return _mapHeight;
			}
			set
			{
				double newValue = Math.Round(value, 1);
				if (!_mapHeight.Equals(newValue))
				{
					_mapHeight = value;
				}
			}
		}

		public double MapSizeFeet
		{
			get
			{
				return _mapSizeFeet;
			}
			set
			{
				double newValue = Math.Round(value, 1);
				if (!_mapSizeFeet.Equals(newValue))
				{
					_mapSizeFeet = value;
				}
			}
		}

		public double Tape_Width
		{
			get
			{
				return _size.Width;
			}
			set
			{
				_size.Width = value;
			}
		}

		public double Tape_Height
		{
			get
			{
				return _size.Height;
			}
			set
			{
				_size.Height = value;
			}
		}

		public double TapePosX
		{
			get
			{
				return _location.X;
			}
			set
			{
				_location.X = value;
			}
		}

		public double TapePosY
		{
			get
			{
				return _location.Y;
			}
			set
			{
				_location.Y = value;
			}
		}

		public double HorizontalOffset
		{
			get
			{
				return _horizontalOffset;
			}
			set
			{
				double newValue = Math.Round(value, 1);
				if (!_horizontalOffset.Equals(newValue))
				{
					_horizontalOffset = value;
					OnDisplayUpdate();
				}
			}
		}

		public double VerticalOffset
		{
			get
			{
				return _verticalOffset;
			}
			set
			{
				double newValue = Math.Round(value, 1);
				if (!_verticalOffset.Equals(newValue))
				{
					_verticalOffset = value;
					OnDisplayUpdate();
				}
			}
		}

		#endregion

	}
}
