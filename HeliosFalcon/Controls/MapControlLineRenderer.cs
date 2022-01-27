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
	using System.Windows;
	using System.Windows.Media;


	public class MapControlLineRenderer : GaugeComponent
	{
		private double _scaleFactor = 1.0d;
		private const double _targetBaseLineWidth = 1.3d;
		private double _targetLineWidth;


		public MapControlLineRenderer() {}


		#region Drawing

		protected override void OnRender(DrawingContext drawingContext)
		{
			Brush _lineBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
			Pen _linePen = new Pen(_lineBrush, _targetLineWidth) { DashStyle = DashStyles.Dash };

			drawingContext.DrawLine(_linePen, new Point(AircraftPositionX, AircraftPositionY), new Point(TargetPositionX, TargetPositionY));
		}

		protected override void OnRefresh(double xScale, double yScale)
		{
			_scaleFactor = Math.Min(xScale, yScale);
			_targetLineWidth = _targetBaseLineWidth * _scaleFactor;
		}

		#endregion Drawing


		#region Properties

		public double TargetPositionX { get; set; }
		public double TargetPositionY { get; set; }
		public double AircraftPositionX { get; set; }
		public double AircraftPositionY { get; set; }

		#endregion

	}
}
