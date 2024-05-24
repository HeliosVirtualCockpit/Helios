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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.NozzleOpen
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F5E.Instruments.NozzleOpen", "Nozzle Open Indicator", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class NozzleOpenGauge : BaseGauge
    {
        private readonly HeliosValue _NozzleOpen;
        private readonly GaugeNeedle _needle;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public NozzleOpenGauge(): base("Nozzle Open Indicator", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Nozzle_Open_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needle = new GaugeNeedle($"{_gaugeImagePath}F-5E_Nozzle_Open_Needle.xaml", new Point(this.Height/2, this.Width/2), new Size(69, 163), new Point(34.5, 128.5), 67d);
            Components.Add(_needle);
            _NozzleOpen = new HeliosValue(this, new BindingValue(0d), "", "Nozzle Open", "Percent", "(0 to 100)", BindingValueUnits.Numeric);
            _NozzleOpen.Execute += new HeliosActionHandler(NozzleOpen_Execute);
            Actions.Add(_NozzleOpen);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 100d, 226d)
            {
            };
        }
        void NozzleOpen_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }

    }
}
