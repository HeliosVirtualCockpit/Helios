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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.PitchTrim
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F5E.Instruments.PitchTrim", "Pitch Trim Indicator", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class PitchTrimGauge : BaseGauge
    {
        private readonly HeliosValue _pitchTrim;
        private readonly GaugeNeedle _needle;
        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public PitchTrimGauge(): base("Pitch Trim Indicator", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Pitch_Trim_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));
            _needle = new GaugeNeedle($"{_gaugeImagePath}F-5E_Pitch_Trim_Needle.xaml", new Point(this.Height/2, this.Width/2), new Size(58, 157), new Point(29, 128), 196d);
            Components.Add(_needle);
            _pitchTrim = new HeliosValue(this, new BindingValue(0d), "", "Pitch Trim", "Degrees", "(-1 to 10)", BindingValueUnits.Degrees);
            _pitchTrim.Execute += new HeliosActionHandler(PitchTrim_Execute);
            Actions.Add(_pitchTrim);

            _calibrationPoints = new CalibrationPointCollectionDouble(-1d, -180d / 11, 10d, 180d / 11d * 10d)
            {
                new CalibrationPointDouble(0d, 0d)
            };
        }
        void PitchTrim_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
        }

    }
}
