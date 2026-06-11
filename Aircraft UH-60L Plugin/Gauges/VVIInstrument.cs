//  Copyright 2018 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.UH60L.VVI.Indicator", "Vertical Velocity Indicator Instrument", "UH-60L Blackhawk", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class VVIInstrument : BaseGauge
    {
        private HeliosValue _VVIPosition;
        private GaugeNeedle _needle;
        //        private GaugeNeedle _offFlagNeedle;
        //        private HeliosValue _offIndicator;
        //        private CalibrationPointCollectionDouble _needleCalibration;

        public VVIInstrument() : this("VVI Instrument", new Size(300, 300)) { }
        public VVIInstrument(string name, Size size)
            : base(name, size)
        {
            //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{UH-60L}/Images/VVI.xaml", new Rect(0d, 0d, 300d, 300d)));
            // m/s to degrees  (Knots to m/s = 0.5144444)
            //_needleCalibration = new CalibrationPointCollectionDouble(0d, 3d, 250d, 333d);
            //_needleCalibration.Add(new CalibrationPointDouble(20d, 11d));
            //_needleCalibration.Add(new CalibrationPointDouble(30d, 23d));
            //_needleCalibration.Add(new CalibrationPointDouble(40d, 34d));
            //_needleCalibration.Add(new CalibrationPointDouble(50d, 48d));
            //_needleCalibration.Add(new CalibrationPointDouble(55d, 63d));
            //_needleCalibration.Add(new CalibrationPointDouble(60d, 78d));
            //_needleCalibration.Add(new CalibrationPointDouble(70d, 91.8d));
            //_needleCalibration.Add(new CalibrationPointDouble(75d, 98.5d));
            //_needleCalibration.Add(new CalibrationPointDouble(80d, 105d));
            //_needleCalibration.Add(new CalibrationPointDouble(90d, 119d));
            //_needleCalibration.Add(new CalibrationPointDouble(100d, 133d));
            //_needleCalibration.Add(new CalibrationPointDouble(110d, 146d));
            //_needleCalibration.Add(new CalibrationPointDouble(120d, 160d));
            //_needleCalibration.Add(new CalibrationPointDouble(125d, 167d));
            //_needleCalibration.Add(new CalibrationPointDouble(130d, 173.5d));
            //_needleCalibration.Add(new CalibrationPointDouble(140d, 187d));
            //_needleCalibration.Add(new CalibrationPointDouble(150d, 201d));
            //_needleCalibration.Add(new CalibrationPointDouble(160d, 214d));
            //_needleCalibration.Add(new CalibrationPointDouble(170d, 227d));
            //_needleCalibration.Add(new CalibrationPointDouble(175d, 234d));
            //_needleCalibration.Add(new CalibrationPointDouble(180d, 241d));
            //_needleCalibration.Add(new CalibrationPointDouble(190d, 254d));
            //_needleCalibration.Add(new CalibrationPointDouble(200d, 267d));
            //_needleCalibration.Add(new CalibrationPointDouble(210d, 280d));
            //_needleCalibration.Add(new CalibrationPointDouble(220d, 284.5d));
            //_needleCalibration.Add(new CalibrationPointDouble(225d, 300d));
            //_needleCalibration.Add(new CalibrationPointDouble(230d, 307d));
            //_needleCalibration.Add(new CalibrationPointDouble(240d, 320d));
            _needle = new GaugeNeedle("{F-15E}/Gauges/Instruments/NeedleA.xaml", new Point(150d, 150d), new Size(34d, 214d), new Point(17d, 130d), -90d);
            Components.Add(_needle);
            _VVIPosition = new HeliosValue(this, new BindingValue(0d), name, "Vertical Velocity Needle", "Vertical Velocity.", "Rotation -170 to +170", BindingValueUnits.Degrees);
            _VVIPosition.Execute += new HeliosActionHandler(VVIExecute);
            Actions.Add(_VVIPosition);

        }

        void VVIExecute(object action, HeliosActionEventArgs e)
        {
            // _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
            _needle.Rotation = e.Value.DoubleValue;
        }
    }
}
