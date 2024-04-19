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

namespace GadrocsWorkshop.Helios.Gauges.F_16.OilPressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F16.OilPressure", "Oil Pressure", "F-16", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class OilPressure : BaseGauge
    {
        private HeliosValue _oilPressure;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;

        public OilPressure()
            : base("Oil Pressure", new Size(360, 360))
        {
            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 100d, 320d);

            Components.Add(new GaugeImage("{F-16C}/Gauges/OilPressure/oil_faceplate.xaml", new Rect(30d, 30d, 300d, 300d)));

            _needle = new GaugeNeedle("{F-16C}/Gauges/OilPressure/oil_needle.xaml", new Point(180d, 180d), new Size(60d, 144d), new Point(30d, 114d), 110d);
            _needle.Rotation = _needleCalibration.Interpolate(0);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{F-16C}/Gauges/Common/f16_engine_bezel.png", new Rect(0d, 0d, 360d, 360d)));

            _oilPressure = new HeliosValue(this, new BindingValue(0d), "", "oil pressure", "Current oil pressure in the engine.", "Percent (0-100)", BindingValueUnits.Numeric);
            _oilPressure.Execute += new HeliosActionHandler(OilPressure_Execute);
            Actions.Add(_oilPressure);
        }

        void OilPressure_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}