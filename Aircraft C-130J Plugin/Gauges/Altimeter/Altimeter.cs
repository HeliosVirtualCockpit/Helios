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

namespace GadrocsWorkshop.Helios.Gauges.C130J.Altimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.C130J;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.C130J.Altimeter", "Altimeter", "C-130J Hercules", typeof(GaugeRenderer),HeliosControlFlags.None)]
    public class Altimeter : CompositeBaseGauge
    {
        private HeliosValue _altitude;
        private HeliosValue _airPressure;
        private HeliosValue _ias;

        private GaugeNeedle _needle;
        private GaugeNeedle _iasTape;
        private CalibrationPointCollectionDouble _needleCalibration;
        private CalibrationPointCollectionDouble _iasCalibration;
        private GaugeDrumCounter _altimeter10KDrum;
        private GaugeDrumCounter _altimeter1KDrum;
        private GaugeDrumCounter _airPressureDrumLSD;
        private GaugeDrumCounter _airPressureDrumMSD;
        private GaugeDrumCounter _metricAirPressureDrumLSD;
        private GaugeDrumCounter _metricAirPressureDrumMSD;
        private double _prevAltitude = 0;

        public Altimeter()
            : base("Altimeter", new Size(300, 300))
        {
            SupportedInterfaces = new[] { typeof(C130JInterface) };

            _altimeter10KDrum = new GaugeDrumCounter("{C-130J}/Gauges/Altimeter/Herc-Altimeter-10K-Tape.xaml", new Point(96d, 75d - (11d * 18d)), "#", new Size(21d, 18d), new Size(21d, 18d), 10d, true);
            _altimeter10KDrum.Value = 0d;
            _altimeter10KDrum.Clip = new RectangleGeometry(new Rect(96d, 73d, 20d, 22d));
            Components.Add(_altimeter10KDrum);

            _altimeter1KDrum = new GaugeDrumCounter("{C-130J}/Gauges/Altimeter/Herc-Altimeter-1K-Tape.xaml", new Point(118d, -216d), "#", new Size(18.423d, 24d), new Size(18.423d, 24d), 10d, true);
            _altimeter1KDrum.Value = 0d;
            _altimeter1KDrum.Clip = new RectangleGeometry(new Rect(118d, 68d, 20d, 30d));
            Components.Add(_altimeter1KDrum);

            // InHg Aperture new Rect(228d, 150d, 52d, 26d)
            _airPressureDrumLSD = new GaugeDrumCounter("{C-130J}/Gauges/Altimeter/Herc-Altimeter-0to0-Tape.xaml", new Point(228d + 26d, 150d - 211d), "#%", new Size(13d, 18d), new Size(13d, 18d), 10d, true);
            _airPressureDrumLSD.Value = 92d;
            _airPressureDrumLSD.Clip = new RectangleGeometry(new Rect(228d + 26d, 150d, 26d, 26d));
            Components.Add(_airPressureDrumLSD);

            _airPressureDrumMSD = new GaugeDrumCounter("{C-130J}/Gauges/Altimeter/Herc-Altimeter-InHg-100s-Tape.xaml", new Point(228d, 150d - 193d), "#", new Size(26d, 18d), new Size(26d, 18d), 10d, true);
            _airPressureDrumMSD.Value = 29d - 22d;
            _airPressureDrumMSD.Clip = new RectangleGeometry(new Rect(228d, 150d, 26d, 26d));
            Components.Add(_airPressureDrumMSD);

            // MB Aperture new Rect(18d, 150d, 54d, 26d)
            _metricAirPressureDrumLSD = new GaugeDrumCounter("{C-130J}/Gauges/Altimeter/Herc-Altimeter-0to0-Tape.xaml", new Point(18d + 26d, 150d - 211d), "#%", new Size(13d, 18d), new Size(13d, 18d), 10d, true);
            _metricAirPressureDrumLSD.Value = 13d;
            _metricAirPressureDrumLSD.Clip = new RectangleGeometry(new Rect(18d + 26d, 150d, 26d, 26d));
            Components.Add(_metricAirPressureDrumLSD);

            _metricAirPressureDrumMSD = new GaugeDrumCounter("{C-130J}/Gauges/Altimeter/Herc-Altimeter-MB-100s-Tape.xaml", new Point(18d, 150d - 85d), "#", new Size(26d, 18d), new Size(26d, 18d), 4d, true);
            _metricAirPressureDrumMSD.Value = 10d - 7d;
            _metricAirPressureDrumMSD.Clip = new RectangleGeometry(new Rect(18d, 150d, 54d, 26d));
            Components.Add(_metricAirPressureDrumMSD);

            _iasTape = new GaugeNeedle("{C-130J}/Gauges/Altimeter/Herc-IAS-Tape.xaml", new Point(68d, 214d), new Size(1500d, 50d), new Point(311d - 82d, 0d));
            _iasTape.Clip = new RectangleGeometry(new Rect(68d, 214d, 164d, 42d));
            Components.Add(_iasTape);
            _iasCalibration = new CalibrationPointCollectionDouble(0d, -311d, 360d, -1203d) {
            new CalibrationPointDouble(60d,-331d),
            new CalibrationPointDouble(80d,-408d),
            new CalibrationPointDouble(100d,-485d),
            new CalibrationPointDouble(120d,-562d),
            new CalibrationPointDouble(140d,-639d),
            new CalibrationPointDouble(160d,-716d),
            new CalibrationPointDouble(180d,-793d),
            new CalibrationPointDouble(200d,-870d),
            new CalibrationPointDouble(220d,-947d),
            new CalibrationPointDouble(250d,-1024d),
            new CalibrationPointDouble(300d,-1101d),
            new CalibrationPointDouble(350d,-1178d),
            };

            Components.Add(new GaugeImage("{C-130J}/Gauges/Altimeter/Herc-Altimeter-Faceplate.xaml", new Rect(0d, 0d, 300d, 300d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1000d, 360d);
            _needle = new GaugeNeedle("{C-130J}/Gauges/Altimeter/Herc-Altimeter-Needle.xaml", new Point(150d, 105d), new Size(21.394d, 102.432d), new Point(10.697d, 69.590d));
            Components.Add(_needle);

            _airPressure = new HeliosValue(this, new BindingValue(0d), "Instruments", "air pressure inHg", "Current air pressure calibaration setting in inHg for the altimeter.", "", BindingValueUnits.InchesOfMercury);
            _airPressure.SetValue(new BindingValue(29.92), true);
            _airPressure.Execute += new HeliosActionHandler(AirPressure_Execute);
            Actions.Add(_airPressure);

            _altitude = new HeliosValue(this, new BindingValue(0d), "Instruments", "barometric altitude", "Current altitude of the aircraft.", "", BindingValueUnits.Feet);
            _altitude.Execute += new HeliosActionHandler(Altitude_Execute);
            Actions.Add(_altitude);

            _ias = new HeliosValue(this, new BindingValue(0d), "Instruments", "air speed", "Indicated Air Speed", "0 to 360 knots", BindingValueUnits.Knots);
            _ias.Execute += new HeliosActionHandler(IAS_Execute);
            Actions.Add(_ias);
            CreateInputBindings();
        }
        void CreateInputBindings()
        {
            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { "Instruments.air speed.changed", "Instruments.set.air speed" },
                { "Instruments.Barometric Altitude.changed", "Instruments.set.barometric altitude" },
                { "Instruments.Pressure.changed", "Instruments.set.air pressure inHg" },
            };

            foreach (string t in bindings.Keys)
            {
                AddDefaultInputBinding(
                    childName: "",
                    interfaceTriggerName: t,
                    deviceActionName: bindings[t]
                    );
            }
        }
        void Altitude_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue % 1000d);
            bool increasing = _prevAltitude < e.Value.DoubleValue;
            _prevAltitude = e.Value.DoubleValue;
            double drumAdjustment10K = 0d;
            if (e.Value.DoubleValue >= 0)
            {
                drumAdjustment10K = 10000d;
            }
            _altimeter10KDrum.Value = (e.Value.DoubleValue + drumAdjustment10K) / 10000d;

            // Setup the thousands drum to roll with the rest
            double thousands = (e.Value.DoubleValue / 100d) % 100d;
            if (thousands >= 99.1 && increasing)
            {
                _altimeter10KDrum.StartRoll = thousands % 1d;
            }
            else
            {
                _altimeter10KDrum.StartRoll = -1d;
            }
            _altimeter1KDrum.Value = thousands/10d;
        }
        void AirPressure_Execute(object action, HeliosActionEventArgs e)
        {
            const double InHgToMillibar = 33.8638866667;
            _airPressureDrumMSD.Value = Math.Truncate(e.Value.DoubleValue) - 22d;
            _airPressureDrumLSD.Value = Math.Round((e.Value.DoubleValue % 1d) * 100d,1);
            double mbarPressure = e.Value.DoubleValue * InHgToMillibar;
            _metricAirPressureDrumMSD.Value = Math.Truncate(mbarPressure / 100d) - 7d;
            _metricAirPressureDrumLSD.Value = mbarPressure % 100d;
        }
        void IAS_Execute(object action, HeliosActionEventArgs e)
        {
            _iasTape.HorizontalOffset = _iasCalibration.Interpolate(e.Value.DoubleValue) + 311d;
        }
    }
}
