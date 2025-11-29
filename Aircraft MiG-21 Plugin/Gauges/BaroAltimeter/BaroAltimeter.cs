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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.BaroAltimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG21.BarometricAltimeter", "Barometric Altimeter", "MiG-21", typeof(GaugeRenderer))]
    public class BarometricAltimeter : BaseGauge
    {
        private GaugeNeedle _qfeCard;
        private GaugeNeedle _shortNeedle;
        private GaugeNeedle _longNeedle;
        private GaugeNeedle _mTriangleNeedle;
        private GaugeNeedle _kmTriangleNeedle;

        private HeliosValue _altitudeLong;
        private HeliosValue _altitudeShort;
        private HeliosValue _qfePressure;
        private HeliosValue _mTriangle;
        private HeliosValue _kmTriangle;

        private CalibrationPointCollectionDouble _qfeCalibration;
        private CalibrationPointCollectionDouble _longneedleCalibration;
        private CalibrationPointCollectionDouble _shortneedleCalibration;
        private CalibrationPointCollectionDouble _kmTriangleneedleCalibration;

        public BarometricAltimeter()
            : base("Barometric Altimeter", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _qfeCalibration = new CalibrationPointCollectionDouble(-1d, -135.5d, 1d, 45.5d);
            _qfeCalibration.Add(new CalibrationPointDouble(0.6223d, 30d));  
            _qfeCalibration.Add(new CalibrationPointDouble(0.3111d, 15d));  
            _qfeCalibration.Add(new CalibrationPointDouble(-0.0090d, 0d));  
            _qfeCalibration.Add(new CalibrationPointDouble(-0.1166d, -15d)); 
            _qfeCalibration.Add(new CalibrationPointDouble(-0.2246, -30d)); 
            _qfeCalibration.Add(new CalibrationPointDouble(-0.3362, -45d)); 
            _qfeCalibration.Add(new CalibrationPointDouble(-0.4501, -60d)); 
            _qfeCalibration.Add(new CalibrationPointDouble(-0.5577, -75d)); 
            _qfeCalibration.Add(new CalibrationPointDouble(-0.6715, -90d)); 
            _qfeCalibration.Add(new CalibrationPointDouble(-0.7793, -105d)); 
            _qfeCalibration.Add(new CalibrationPointDouble(-0.8899, -120d)); 

            _longneedleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1000d, 360d);
            _longneedleCalibration.Add(new CalibrationPointDouble(200d, 74d));
            _longneedleCalibration.Add(new CalibrationPointDouble(400d, 146d));
            _longneedleCalibration.Add(new CalibrationPointDouble(600d, 220d));
            _longneedleCalibration.Add(new CalibrationPointDouble(800d, 293d));

            _shortneedleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 30000d, 340d);
            _kmTriangleneedleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 30000d, 319d);

            _qfeCard = new GaugeNeedle("{MiG-21}/Gauges/BaroAltimeter/baro_alt_qfe_card.xaml", center, new Size(264, 264), new Point(132, 132), 150d);
            Components.Add(_qfeCard);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/BaroAltimeter/baro_alt_faceplate.xaml", new Rect(0, 0, 340, 340)));
            Components.Add(new GaugeImage("{MiG-21}/Gauges/BaroAltimeter/baro_alt_innerfaceplate.xaml", new Rect(90, 90, 160, 160)));

            _mTriangleNeedle = new GaugeNeedle("{MiG-21}/Gauges/BaroAltimeter/baro_alt_triangle_m.xaml", center, new Size(40, 222), new Point(20, 152));
            Components.Add(_mTriangleNeedle);

            _kmTriangleNeedle = new GaugeNeedle("{MiG-21}/Gauges/BaroAltimeter/baro_alt_triangle_m.xaml", center, new Size(21, 110), new Point(10.5, 79));
            Components.Add(_kmTriangleNeedle);

            _shortNeedle = new GaugeNeedle("{MiG-21}/Gauges/BaroAltimeter/baro_alt_short_needle.xaml", center, new Size(21, 110), new Point(10.5, 79));
            Components.Add(_shortNeedle);

            _longNeedle = new GaugeNeedle("{MiG-21}/Gauges/BaroAltimeter/baro_alt_long_needle.xaml", center, new Size(40, 209), new Point(20, 139));
            Components.Add(_longNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));

            _altitudeLong = new HeliosValue(this, BindingValue.Empty, "", "Altitude M", "Current barometric altitude M", "", BindingValueUnits.Meters);
            _altitudeLong.Execute += AltitudeLong_Execute;
            Actions.Add(_altitudeLong);

            _altitudeShort = new HeliosValue(this, BindingValue.Empty, "", "Altitude KM", "Current barometric altitude KM", "", BindingValueUnits.Meters);
            _altitudeShort.Execute += AltitudeShort_Execute;
            Actions.Add(_altitudeShort);

            _qfePressure = new HeliosValue(this, BindingValue.Empty, "", "QFE Pressure", "Use Altimeter pressure knob axis as input", "Use Altimeter pressure knob axis as input", BindingValueUnits.Numeric);
            _qfePressure.Execute += Pressure_Execute;
            Actions.Add(_qfePressure);

            _mTriangle = new HeliosValue(this, BindingValue.Empty, "", "Triangle  M", "Current Triangle altitude M", "", BindingValueUnits.Meters);
            _mTriangle.Execute += TriangleM_Execute;
            Actions.Add(_mTriangle);

            _kmTriangle = new HeliosValue(this, BindingValue.Empty, "", "Triangle  KM", "Current Triangle altitude KM", "", BindingValueUnits.Meters);
            _kmTriangle.Execute += TriangleKM_Execute;
            Actions.Add(_kmTriangle);
        }

        private void Pressure_Execute(object action, HeliosActionEventArgs e)
        {
            _qfePressure.SetValue(e.Value, e.BypassCascadingTriggers);
            _qfeCard.Rotation = _qfeCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void AltitudeLong_Execute(object action, HeliosActionEventArgs e)
        {
            _altitudeLong.SetValue(e.Value, e.BypassCascadingTriggers);
            _longNeedle.Rotation = _longneedleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void AltitudeShort_Execute(object action, HeliosActionEventArgs e)
        {
            _altitudeShort.SetValue(e.Value, e.BypassCascadingTriggers);
            _shortNeedle.Rotation = _shortneedleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void TriangleM_Execute(object action, HeliosActionEventArgs e)
        {
            _mTriangle.SetValue(e.Value, e.BypassCascadingTriggers);
            _mTriangleNeedle.Rotation = _longneedleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void TriangleKM_Execute(object action, HeliosActionEventArgs e)
        {
            _kmTriangle.SetValue(e.Value, e.BypassCascadingTriggers);
            _kmTriangleNeedle.Rotation = _kmTriangleneedleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
