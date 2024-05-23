//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.AoA
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F5E.Instruments.AoA", "Angle of Attack Indicator", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class AoAGauge : BaseGauge
    {
        private readonly HeliosValue _aoA;
        private readonly HeliosValue _offFlag;
        private readonly GaugeNeedle _needle;
        private readonly GaugeImage _offFlagImage;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public AoAGauge(): base("Angle of Attack Indicator", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            //scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_AoA_Off_Flag_Background.xaml", new Rect(40d, 107d, 80d, 80d)));

            _offFlagImage = new GaugeImage($"{_gaugeImagePath}F-5E_AoA_Off_Flag_Static.xaml", new Rect(66d, 101d, 56.727, 56.727));
            Components.Add(_offFlagImage);

            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_AoA_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needle = new GaugeNeedle($"{_gaugeImagePath}F-5E_AoA_Needle.xaml", new Point(this.Height/2, this.Width/2), new Size(28d, 148d), new Point(14d, 134d), 225d);
            Components.Add(_needle);


            _aoA = new HeliosValue(this, new BindingValue(0d), "", "Angle of Attack Indicator", "Degrees", "(0 to 30)", BindingValueUnits.Degrees);
            _aoA.Execute += new HeliosActionHandler(AoA_Execute);
            Actions.Add(_aoA);

            _offFlag = new HeliosValue(this, new BindingValue(0d), "", "Angle of Attack Off Flag", "Numeric", "(0 to 1)", BindingValueUnits.Numeric);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 30d, -270d);
        }
        void AoA_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }
        void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlagImage.PosX = 66 - (16 * e.Value.DoubleValue);
            _offFlagImage.PosY = 101 + (16 * e.Value.DoubleValue);
            _offFlagImage.Refresh(this.Width / NativeSize.Width, this.Height / NativeSize.Height);
        }

    }
}
