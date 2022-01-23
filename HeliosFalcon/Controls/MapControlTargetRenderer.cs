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
	using GadrocsWorkshop.Helios.Gauges;
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Media;


	public class MapControlTargetRenderer : GaugeComponent
	{
		private double _scaleFactor = 1.0d;
		private double _pixelsPerDip = 1.0d;
		private const double _fontBaseSize = 6d;
		private double _fontScaleSize;


		public MapControlTargetRenderer()
		{
			GetPixelsPerDip();
		}


		#region Actions

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

		protected override void OnRender(DrawingContext drawingContext)
		{
			Brush lineBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
			Pen linePen = new Pen(lineBrush, _fontScaleSize * 0.05d);
			Brush backgroundFillBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
			FormattedText _targetText;
			FormattedText _courseText;
			double textMargin = 10d;
			double textTopMargin = 10d;

			string target_text = "TGT: " + TargetDistance.ToString() + " Nm " + TargetBearing.ToString("000") + "°";
			string course_text = "CRS: " + CourseDistance.ToString() + " Nm " + CourseBearing.ToString("000") + "°";

			_targetText = new FormattedText(target_text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana Bold"), _fontScaleSize, Brushes.White, _pixelsPerDip);
			double targetTextPos = textMargin;
			Rect _targetTextBounds = new Rect(targetTextPos, textTopMargin, _targetText.Width + (4 * _scaleFactor), _targetText.Height + (2 * _scaleFactor));
			drawingContext.DrawRectangle(backgroundFillBrush, linePen, _targetTextBounds);

			_courseText = new FormattedText(course_text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana Bold"), _fontScaleSize, Brushes.White, _pixelsPerDip);
			double courseTextPos = MapControlWidth - (textMargin + _courseText.Width + (4 * _scaleFactor));
			Rect _courseTextBounds = new Rect(courseTextPos, textTopMargin, _courseText.Width + (4 * _scaleFactor), _courseText.Height + (2 * _scaleFactor));
			drawingContext.DrawRectangle(backgroundFillBrush, linePen, _courseTextBounds);

			drawingContext.DrawText(_targetText, new Point(targetTextPos + (2 * _scaleFactor), textTopMargin + (1 * _scaleFactor)));
			drawingContext.DrawText(_courseText, new Point(courseTextPos + (2 * _scaleFactor), textTopMargin + (1 * _scaleFactor)));
		}

		protected override void OnRefresh(double xScale, double yScale)
		{
			_scaleFactor = Math.Min(xScale, yScale);
			_fontScaleSize = _fontBaseSize * _scaleFactor;
		}

		#endregion Drawing


		#region Properties

		public double MapControlWidth { get; set; }
		public double MapControlHeight { get; set; }
		public double TargetBearing { get; set; }
		public double TargetDistance { get; set; }
		public double CourseBearing { get; set; }
		public double CourseDistance { get; set; }

		#endregion

	}
}
