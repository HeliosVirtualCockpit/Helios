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
	using System.Globalization;


	public class EHSIRenderer : GaugeComponent
	{
		private readonly Brush textWhite = new SolidColorBrush(Color.FromRgb(255, 255, 255));
		private readonly Brush textBlue = new SolidColorBrush(Color.FromRgb(50, 155, 255));
		private readonly Brush textYellow = new SolidColorBrush(Color.FromRgb(255, 255, 0));
		private readonly Brush textGreen = new SolidColorBrush(Color.FromRgb(0, 225, 0));

		private readonly Typeface _typeface = new Typeface("Arial Bold");
		private double _pixelsPerDip = 1.0;
		private const double _fontBaseSize = 21;
		private double _fontSmallScaleSize;
		private double _fontLargeScaleSize;
		private const double fontScale_X = 0.75;
		private const double fontScale_Y = 1.0;
		private const double _textBaseSmallMargin = 6;
		private const double _textBaseLargeMargin = 18;
		private const double _textBaseTopMargin = 4;
		private const double _textBaseBottomMargin = 30;
		private const double _textTopLeftLabelWidth = 1.70;
		private const double _textTopRightLabelWidth = 1.75;
		private double _textSmallMargin;
		private double _textLargeMargin;
		private double _textSmallRightPosition;
		private double _textLargeRightPosition;
		private double _textTopMargin;
		private double _textBottomMargin;


		public EHSIRenderer()
		{
			SetPixelsPerDip();
		}


		#region Methods

		void SetPixelsPerDip()
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
			FormattedText textLabelCRS = new FormattedText("CRS", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontSmallScaleSize, textWhite, _pixelsPerDip);
			FormattedText textValueCRS = new FormattedText(DesiredCourse.ToString("000"), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontSmallScaleSize, textYellow, _pixelsPerDip);

			DrawScaledText(drawingContext, textLabelCRS, _textSmallRightPosition, _textTopMargin, fontScale_X, fontScale_Y);
			DrawNormalText(drawingContext, textValueCRS, _textSmallRightPosition + (_fontSmallScaleSize * _textTopRightLabelWidth), _textTopMargin);

			FormattedText textLabelHDG = new FormattedText("HDG", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontSmallScaleSize * 0.97, textWhite, _pixelsPerDip);
			FormattedText textValueHDG = new FormattedText(DesiredHeading.ToString("000"), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontSmallScaleSize, textBlue, _pixelsPerDip);

			DrawScaledText(drawingContext, textLabelHDG, _textSmallRightPosition, _textTopMargin + _fontSmallScaleSize, fontScale_X, fontScale_Y);
			DrawNormalText(drawingContext, textValueHDG, _textSmallRightPosition + (_fontSmallScaleSize * _textTopRightLabelWidth), _textTopMargin + _fontSmallScaleSize);

			FormattedText textLabelDistanceTCN = new FormattedText("TCN", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontSmallScaleSize, textWhite, _pixelsPerDip);
			FormattedText textValueDistanceTCN = new FormattedText(BeaconDistance.ToString("0.0" + "Nm"), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontSmallScaleSize, textGreen, _pixelsPerDip);

			DrawScaledText(drawingContext, textLabelDistanceTCN, _textSmallMargin, _textTopMargin, fontScale_X, fontScale_Y);
			DrawNormalText(drawingContext, textValueDistanceTCN, _textSmallMargin + (_fontSmallScaleSize * _textTopLeftLabelWidth), _textTopMargin);

			if (TacanChannel > 0)
			{
				FormattedText textLabelChannelTCN = new FormattedText("CHL", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontSmallScaleSize, textWhite, _pixelsPerDip);
				FormattedText textValueChannelTCN = new FormattedText(TacanChannel.ToString("0") + GetTacanBand(TacanBand) + GetTacanMode(TacanMode), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontSmallScaleSize, textGreen, _pixelsPerDip);

				DrawScaledText(drawingContext, textLabelChannelTCN, _textSmallMargin, _textTopMargin + _fontSmallScaleSize, fontScale_X, fontScale_Y);
				DrawScaledText(drawingContext, textValueChannelTCN, _textSmallMargin + (_fontSmallScaleSize * _textTopLeftLabelWidth), _textTopMargin + _fontSmallScaleSize, fontScale_X, fontScale_Y);
			}

			FormattedText textLabelILS = new FormattedText(GetILSMode(NavMode), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontLargeScaleSize, textWhite, _pixelsPerDip);
			FormattedText textLabelNAV = new FormattedText(GetNAVMode(NavMode), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, _typeface, _fontLargeScaleSize, textWhite, _pixelsPerDip);

			DrawNormalText(drawingContext, textLabelILS, _textLargeMargin + _fontLargeScaleSize * 0.3, _textBottomMargin);
			DrawNormalText(drawingContext, textLabelNAV, _textLargeRightPosition, _textBottomMargin);
		}

		void DrawNormalText(DrawingContext drawingContext, FormattedText text, double pos_X, double pos_Y)
		{
			drawingContext.DrawText(text, new Point(pos_X, pos_Y));
		}

		void DrawScaledText(DrawingContext drawingContext, FormattedText text, double pos_X, double pos_Y, double scale_X, double scale_Y)
		{
			Transform fontScale = new ScaleTransform(scale_X, scale_Y, pos_X, pos_Y);
			drawingContext.PushTransform(fontScale);
			drawingContext.DrawText(text, new Point(pos_X, pos_Y));
			drawingContext.Pop();
		}

		#endregion Drawing


		#region Functions

		string GetTacanBand(double band)
		{
			if (band == 1)
			{
				return "X";
			}
			else if (band == 2)
			{
				return "Y";
			}
			else
			{
				return "";
			}
		}

		string GetTacanMode(double mode)
		{
			if (mode == 1)
			{
				return " T/R";
			}
			else if (mode == 2)
			{
				return " A/A";
			}
			else
			{
				return "";
			}
		}

		string GetILSMode(double navmode)
		{
			if (navmode == 0 || navmode == 3)
			{
				return "ILS";
			}
			else
			{
				return "";
			}
		}

		string GetNAVMode(double navmode)
		{
			if (navmode < 2)
			{
				return "TCN";
			}
			else
			{
				return "NAV";
			}
		}

		#endregion Functions


		#region OnRefresh

		protected override void OnRefresh(double xScale, double yScale)
		{
			double _fontScaleFactor = Math.Max(xScale, yScale) * Math.Sqrt(Math.Min(xScale / yScale, yScale / xScale));
			double _sizeScaleFactor = Math.Min(xScale, yScale);

			_fontSmallScaleSize = _fontBaseSize * _fontScaleFactor;
			_fontLargeScaleSize = _fontBaseSize * 1.2 * _fontScaleFactor;

			_textTopMargin = _textBaseTopMargin * _sizeScaleFactor;
			_textBottomMargin = ControlHeight - _textBaseBottomMargin * _sizeScaleFactor - _fontLargeScaleSize;

			_textSmallMargin = _textBaseSmallMargin * _sizeScaleFactor;
			_textSmallRightPosition = ControlWidth - _textSmallMargin - _fontSmallScaleSize * 3.5;

			_textLargeMargin = _textBaseLargeMargin * _sizeScaleFactor;
			_textLargeRightPosition = ControlWidth - _textLargeMargin - _fontLargeScaleSize * 2.3;
		}

		#endregion OnRefresh


		#region Properties

		public double ControlWidth { get; set; }
		public double ControlHeight { get; set; }

		public double NavMode { get; set; } = 2;

		public double BeaconDistance { get; set; }
		public double DesiredCourse { get; set; }
		public double DesiredHeading { get; set; }

		public double TacanChannel { get; set; }
		public double TacanBand { get; set; }
		public double TacanMode { get; set; }

		#endregion Properties

	}
}
