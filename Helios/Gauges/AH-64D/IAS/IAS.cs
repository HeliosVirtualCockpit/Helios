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

namespace GadrocsWorkshop.Helios.Gauges.AH64D
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AH64D.IAS", "Standby IAS Gauge", "AH-64D", typeof(GaugeRenderer))]
    public class IAS : BaseGauge
    {
        private HeliosValue _indicatedAirSpeed;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;
        
        public IAS()
            : base("Standby IAS Gauge", new Size(300, 300))
        {

            Components.Add(new GaugeImage("{Helios}/Images/AH-64D/IAS/ias_faceplate.xaml", new Rect(0d, 0d, 300, 300)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 250d, 360d);
            _needle = new GaugeNeedle("{Helios}/Gauges/AV-8B/Common/needle_a.xaml", new Point(150d, 150d), new Size(30, 128), new Point(15, 113), 0d);
            Components.Add(_needle);
            #region Unused Visuals
            //Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));
            //Components.Add(new GaugeImage("{Helios}/Gauges/AV-8B/IAS/needle_mask.xaml", new Rect(130d, 34d, 73d, 98d)));
            //Components.Add(new GaugeImage("{AV-8B}/Images/WQHD/Panel/crystal_reflection_round.png", new Rect(0d, 0d, 300d, 300d)));
            //GaugeImage _gauge = new GaugeImage("{AV-8B}/Images/WQHD/Panel/crystal_reflection_round.png", new Rect(0d, 0d, 300d, 300d));
            //_gauge.Opacity = 0.4;
            //Components.Add(_gauge);
            #endregion
            _indicatedAirSpeed = new HeliosValue(this, new BindingValue(0d), "Standby IAS", "Indicated Airspeed", "Current indicated airspeed of the aircraft.", "(0 - 250)", BindingValueUnits.Knots);
            _indicatedAirSpeed.Execute += new HeliosActionHandler(IndicatedAirSpeed_Execute);
            Actions.Add(_indicatedAirSpeed);
        }

        void IndicatedAirSpeed_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
