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
	using GadrocsWorkshop.Helios.Gauges;
	using System;
	using System.Windows;
	using System.Windows.Media;
	using System.Collections.Generic;
	using System.Globalization;


	public class MapControlTextRenderer : GaugeComponent
	{
		private List<ITargetData> TargetList = new List<ITargetData>();

		private const double _controlBaseSize = 200;
		private double _controlScaleFactor = 1.0d;
		private double _pixelsPerDip = 1.0d;
		private const double _fontBaseSize = 6d;
		private double _fontScaleSize;
		private const double _textBaseMargin = 5d;
		private const double _textBaseTopMargin = 5d;
		private double _textMargin;
		private double _textTopMargin;
		private double _courseBaseTextOffset = 6d;
		private double _courseTextOffset;


		public MapControlTextRenderer()
		{
			SetPixelsPerDip();
		}


		#region Methods

		public void SetTargetData(List<ITargetData> targetList)
		{
			TargetList = targetList;
		}

		private void SetPixelsPerDip()
		{
			DisplayManager displayManager = new DisplayManager();

			if (displayManager.PixelsPerDip != 0d)
			{
				_pixelsPerDip = displayManager.PixelsPerDip;
			}
		}

		#endregion Methods


		#region Drawing

		protected override void OnRender(DrawingContext drawingContext)
		{
			Brush lineBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
			Pen linePen = new Pen(lineBrush, _fontScaleSize * 0.06d);
			Brush backgroundFillBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
			FormattedText textFormattedAircraft;
			FormattedText textFormattedTarget;
			FormattedText textFormattedCourse;

			double textAircraftPos;
			double textTargetPos;
			double courseTextPosX;
			double courseTextPosY;

			string textAircraft = "OWNSHIP: " + AircraftBearing.ToString("000") + "° " + AircraftDistance.ToString() + "Nm";
			string textTarget = "TARGET 01: " + TargetBearing.ToString("000") + "° " + TargetDistance.ToString() + "Nm";

			textFormattedAircraft = new FormattedText(textAircraft, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana Bold"), _fontScaleSize, Brushes.White, _pixelsPerDip);
			textFormattedTarget = new FormattedText(textTarget, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana Bold"), _fontScaleSize, Brushes.White, _pixelsPerDip);
	
			textAircraftPos = _textMargin;
			Rect textBoundsAircraft = new Rect(textAircraftPos, _textTopMargin, textFormattedAircraft.Width + (4 * _controlScaleFactor), textFormattedAircraft.Height + (2 * _controlScaleFactor));
			drawingContext.DrawRectangle(backgroundFillBrush, linePen, textBoundsAircraft);
			drawingContext.DrawText(textFormattedAircraft, new Point(textAircraftPos + (2 * _controlScaleFactor), _textTopMargin + (1 * _controlScaleFactor)));

			textTargetPos = MapControlWidth - (_textMargin + textFormattedTarget.Width + (4 * _controlScaleFactor));
			Rect textBoundsTarget = new Rect(textTargetPos, _textTopMargin, textFormattedTarget.Width + (4 * _controlScaleFactor), textFormattedTarget.Height + (2 * _controlScaleFactor));
			drawingContext.DrawRectangle(backgroundFillBrush, linePen, textBoundsTarget);
			drawingContext.DrawText(textFormattedTarget, new Point(textTargetPos + (2 * _controlScaleFactor), _textTopMargin + (1 * _controlScaleFactor)));

			if (TargetSelected)
			{
				for (int i = TargetList.Count - 1; i >= 0; i--)
				{
					string textCourse = (i + 1).ToString("00") + " " + TargetList[i].CourseBearing.ToString("000") + "° " + TargetList[i].CourseDistance.ToString() + "Nm";
					textFormattedCourse = new FormattedText(textCourse, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Arial Bold"), _fontScaleSize, Brushes.White, _pixelsPerDip);

					if (TargetList[i].TargetBearing > 90 && TargetList[i].TargetBearing < 270)
					{
						courseTextPosY = TargetList[i].TargetPosition_Y - (_courseTextOffset + textFormattedCourse.Height + (2 * _controlScaleFactor));
					}
					else
					{
						courseTextPosY = TargetList[i].TargetPosition_Y + _courseTextOffset;
					}

					if (TargetList[i].TargetDistance > 95 + 30 * Math.Abs(Math.Cos(TargetList[i].TargetBearing * Math.PI / 180)))
					{
						if (TargetList[i].TargetBearing > 180 && TargetList[i].TargetBearing < 360)
						{
							courseTextPosX = TargetList[i].TargetPosition_X;
						}
						else
						{
							courseTextPosX = TargetList[i].TargetPosition_X - (textFormattedCourse.Width + (4 * _controlScaleFactor));
						}
					}
					else
					{
						courseTextPosX = TargetList[i].TargetPosition_X - (textFormattedCourse.Width + (4 * _controlScaleFactor)) / 2;
					}

					Rect textBoundsCourse = new Rect(courseTextPosX, courseTextPosY, textFormattedCourse.Width + (4 * _controlScaleFactor), textFormattedCourse.Height + (2 * _controlScaleFactor));
					drawingContext.DrawRectangle(backgroundFillBrush, linePen, textBoundsCourse);
					drawingContext.DrawText(textFormattedCourse, new Point(courseTextPosX + (2 * _controlScaleFactor), courseTextPosY + (1 * _controlScaleFactor)));
				}
			}
		}

		#endregion Drawing


		#region OnRefresh

		protected override void OnRefresh(double xScale, double yScale)
		{
			_controlScaleFactor = Math.Min(MapControlWidth, MapControlHeight) / _controlBaseSize;
			_fontScaleSize = _fontBaseSize * _controlScaleFactor;
			_textMargin = _textBaseMargin * _controlScaleFactor;
			_textTopMargin = _textBaseTopMargin * _controlScaleFactor;
			_courseTextOffset = _courseBaseTextOffset * _controlScaleFactor;

			if (TargetList.Count > 0)
			{
				TargetBearing = TargetList[0].TargetBearing;
				TargetDistance = TargetList[0].TargetDistance;
			}
			else
			{
				TargetBearing = 0d;
				TargetDistance = 0d;
			}
		}

		#endregion OnRefresh


		#region Properties

		public double MapControlWidth { get; set; }
		public double MapControlHeight { get; set; }
		public double TargetBearing { get; set; }
		public double TargetDistance { get; set; }
		public double AircraftBearing { get; set; }
		public double AircraftDistance { get; set; }
		public bool TargetSelected { get; set; }

		#endregion Properties

	}
}
