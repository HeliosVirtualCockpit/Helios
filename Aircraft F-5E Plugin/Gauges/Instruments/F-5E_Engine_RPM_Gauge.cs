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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.EngineRPM
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [HeliosControl("Helios.F5E.Instruments.EngineRPM", "Engine RPM Gauge", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class EngineRPM : BaseGauge
    {
        private readonly HeliosValue _engineRPMLeft;
        private readonly HeliosValue _engineRPMRight;
        private readonly GaugeNeedle _needleLeft;
        private readonly GaugeNeedle _needleRight;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly CalibrationPointCollectionDouble _calibrationPoints2;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public EngineRPM(): base("Engine RPM Gauge", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Engine_RPM_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needleLeft = new GaugeNeedle($"{_gaugeImagePath}F-5E_Engine_RPM_Needle_1.xaml", new Point(this.Height / 2, this.Width / 2), new Size(69, 163), new Point(34.5, 128.5), 0d);
            Components.Add(_needleLeft); 
            _needleRight = new GaugeNeedle($"{_gaugeImagePath}F-5E_Engine_RPM_Needle_2.xaml", new Point(98d, 95d), new Size(10, 57), new Point(5d , 31d), 0d);
            Components.Add(_needleRight);

            _engineRPMLeft = new HeliosValue(this, new BindingValue(0d), "", "Engine RPM Percentage", "Numeric", "(0 to 106)", BindingValueUnits.Numeric);
            _engineRPMLeft.Execute += new HeliosActionHandler(EngineRPM_Execute);
            Actions.Add(_engineRPMLeft);

            _engineRPMRight = new HeliosValue(this, new BindingValue(0d), "", "Engine RPM Units", "Numeric", "(0 to 10)", BindingValueUnits.Numeric);
            _engineRPMRight.Execute += new HeliosActionHandler(EngineRPM_Execute);
            Actions.Add(_engineRPMRight);


            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 106d, 286.2d);
            _calibrationPoints2 = new CalibrationPointCollectionDouble(0d, 0d, 10d, 360d);
        }
        void EngineRPM_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosValue hv)
            {
                switch (hv.Name)
                {
                    case "Engine RPM Percentage":
                        _needleLeft.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Engine RPM Units":
                        _needleRight.Rotation = _calibrationPoints2.Interpolate(e.Value.DoubleValue);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
