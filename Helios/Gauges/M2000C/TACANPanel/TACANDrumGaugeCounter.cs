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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.TACAN
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    public class TACANDrumGaugeCounter : GaugeDrumCounter
    {

        private double _drumDigits = 10d;
 
        public TACANDrumGaugeCounter(string imageFile, Point location, string format, Size digitSize)
            : this(imageFile, location, format, digitSize, digitSize)
        {
        }

        public TACANDrumGaugeCounter(string imageFile, Point location, string format, Size digitSize, Size digitRenderSize, double drumDigits = 10d) : base(imageFile, location, format, digitSize, digitRenderSize)
        {
            _drumDigits = drumDigits;
         }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_drumDigits > 10d)
            {
                double xOffset = 0;

                drawingContext.PushTransform(new TranslateTransform(ScaledLocation.X , ScaledLocation.Y));
                double digitValue = Value;
                double renderValue = digitValue;

                renderValue += 1d;
                drawingContext.PushTransform(new TranslateTransform(xOffset, -(renderValue * DigitRenderSize.Height)));
                drawingContext.DrawImage(ImageSource, ImageRect);
                drawingContext.Pop();
                drawingContext.Pop();
            }
            else base.OnRender(drawingContext);
        }
    }
}
