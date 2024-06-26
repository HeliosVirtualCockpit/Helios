﻿//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Gauges.F_16.Nozzle
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F16.Nozzle", "Nozzle", "F-16", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class Nozzle : BaseGauge
    {
        private HeliosValue _nozzlePosition;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;

        public Nozzle()
            : base("Nozzle", new Size(360, 360))
        {
            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 100d, 280d);

            Components.Add(new GaugeImage("{F-16C}/Gauges/Nozzle/nozzle_faceplate.xaml", new Rect(30d, 30d, 300d, 300d)));

            _needle = new GaugeNeedle("{F-16C}/Gauges/Nozzle/nozzle_needle.xaml", new Point(180d, 180d), new Size(60d, 144d), new Point(30d, 114d), 40d);
            _needle.Rotation = _needleCalibration.Interpolate(0);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{F-16C}/Gauges/Common/f16_engine_bezel.png", new Rect(0d, 0d, 360d, 360d)));

            _nozzlePosition = new HeliosValue(this, new BindingValue(0d), "", "nozzle position", "Current afterburner nozzel position.", "Percent open (0-100)", BindingValueUnits.Numeric);
            _nozzlePosition.SetValue(new BindingValue(29.92), true);
            _nozzlePosition.Execute += new HeliosActionHandler(NozzlePosition_Execute);
            Actions.Add(_nozzlePosition);
        }

        void NozzlePosition_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
