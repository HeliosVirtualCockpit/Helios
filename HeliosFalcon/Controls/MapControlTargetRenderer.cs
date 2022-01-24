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
		private const double _textBaseMargin = 5d;
		private const double _textBaseTopMargin = 5d;
		private double _textMargin;
		private double _textTopMargin;


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
			FormattedText textFormattedAircraft;
			FormattedText textFormattedTarget;
						
			string textAircraft = "OWNSHIP: " + AircraftDistance.ToString() + " Nm " + AircraftBearing.ToString("000") + "°";
			string textTarget = "TARGET: " + TargetDistance.ToString() + " Nm " + TargetBearing.ToString("000") + "°";

			textFormattedAircraft = new FormattedText(textAircraft, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana Bold"), _fontScaleSize, Brushes.White, _pixelsPerDip);
			double textAircraftPos = _textMargin;
			Rect textBoundsAircraft = new Rect(textAircraftPos, _textTopMargin, textFormattedAircraft.Width + (4 * _scaleFactor), textFormattedAircraft.Height + (2 * _scaleFactor));
			drawingContext.DrawRectangle(backgroundFillBrush, linePen, textBoundsAircraft);

			textFormattedTarget = new FormattedText(textTarget, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana Bold"), _fontScaleSize, Brushes.White, _pixelsPerDip);
			double textTargetPos = MapControlWidth - (_textMargin + textFormattedTarget.Width + (4 * _scaleFactor));
			Rect textBoundsTarget = new Rect(textTargetPos, _textTopMargin, textFormattedTarget.Width + (4 * _scaleFactor), textFormattedTarget.Height + (2 * _scaleFactor));
			drawingContext.DrawRectangle(backgroundFillBrush, linePen, textBoundsTarget);

			drawingContext.DrawText(textFormattedAircraft, new Point(textAircraftPos + (2 * _scaleFactor), _textTopMargin + (1 * _scaleFactor)));
			drawingContext.DrawText(textFormattedTarget, new Point(textTargetPos + (2 * _scaleFactor), _textTopMargin + (1 * _scaleFactor)));
		}

		protected override void OnRefresh(double xScale, double yScale)
		{
			_scaleFactor = Math.Min(xScale, yScale);
			_fontScaleSize = _fontBaseSize * _scaleFactor;
			_textMargin = _textBaseMargin * _scaleFactor;
			_textTopMargin = _textBaseTopMargin * _scaleFactor;
		}

		#endregion Drawing


		#region Properties

		public double MapControlWidth { get; set; }
		public double MapControlHeight { get; set; }
		public double TargetBearing { get; set; }
		public double TargetDistance { get; set; }
		public double AircraftBearing { get; set; }
		public double AircraftDistance { get; set; }

		#endregion

	}
}
