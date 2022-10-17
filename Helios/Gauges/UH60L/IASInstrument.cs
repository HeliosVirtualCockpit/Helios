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
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.UH60L.IAS.Indicator", "Indicated Air Speed Instrument", "UH-60L", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class IASInstrument : BaseGauge
    {
        private HeliosValue _iasPosition;
        private GaugeNeedle _needle;
        //        private GaugeNeedle _offFlagNeedle;
        //        private HeliosValue _offIndicator;
        private CalibrationPointCollectionDouble _needleCalibration;

        public IASInstrument() : this("IAS Instrument", new Size(420, 420)) { }
        public IASInstrument(string name, Size size)
            : base(name, size)
        {
            //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{Helios}/Images/UH60L/IAS.xaml", new Rect(0d, 4d, 420d, 420d)));
            // m/s to degrees  (Knots to m/s = 0.5144444)
            _needleCalibration = new CalibrationPointCollectionDouble(0d, 3d, 250d, 333d);
            _needleCalibration.Add(new CalibrationPointDouble(20d, 11d));
            _needleCalibration.Add(new CalibrationPointDouble(30d, 23d));
            _needleCalibration.Add(new CalibrationPointDouble(40d, 34d));
            _needleCalibration.Add(new CalibrationPointDouble(50d, 48d));
            _needleCalibration.Add(new CalibrationPointDouble(55d, 63d));
            _needleCalibration.Add(new CalibrationPointDouble(60d, 78d));
            _needleCalibration.Add(new CalibrationPointDouble(70d, 91.8d));
            _needleCalibration.Add(new CalibrationPointDouble(75d, 98.5d));
            _needleCalibration.Add(new CalibrationPointDouble(80d, 105d));
            _needleCalibration.Add(new CalibrationPointDouble(90d, 119d));
            _needleCalibration.Add(new CalibrationPointDouble(100d, 133d));
            _needleCalibration.Add(new CalibrationPointDouble(110d, 146d));
            _needleCalibration.Add(new CalibrationPointDouble(120d, 160d));
            _needleCalibration.Add(new CalibrationPointDouble(125d, 167d));
            _needleCalibration.Add(new CalibrationPointDouble(130d, 173.5d));
            _needleCalibration.Add(new CalibrationPointDouble(140d, 187d));
            _needleCalibration.Add(new CalibrationPointDouble(150d, 201d));
            _needleCalibration.Add(new CalibrationPointDouble(160d, 214d));
            _needleCalibration.Add(new CalibrationPointDouble(170d, 227d));
            _needleCalibration.Add(new CalibrationPointDouble(175d, 234d));
            _needleCalibration.Add(new CalibrationPointDouble(180d, 241d));
            _needleCalibration.Add(new CalibrationPointDouble(190d, 254d));
            _needleCalibration.Add(new CalibrationPointDouble(200d, 267d));
            _needleCalibration.Add(new CalibrationPointDouble(210d, 280d));
            _needleCalibration.Add(new CalibrationPointDouble(220d, 284.5d));
            _needleCalibration.Add(new CalibrationPointDouble(225d, 300d));
            _needleCalibration.Add(new CalibrationPointDouble(230d, 307d));
            _needleCalibration.Add(new CalibrationPointDouble(240d, 320d));
            _needle = new GaugeNeedle("{Helios}/Images/UH60L/IASNeedle.xaml", new Point(210d, 210d), new Size(48d, 230d), new Point(24d, 129d), 0d);
            Components.Add(_needle);
            _iasPosition = new HeliosValue(this, new BindingValue(0d), name, "Air Speed", "Current Indicated air speed.", "Number between 0 and 250", BindingValueUnits.Knots);
            _iasPosition.Execute += new HeliosActionHandler(iasExecute);
            Actions.Add(_iasPosition);

        }

        void iasExecute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
