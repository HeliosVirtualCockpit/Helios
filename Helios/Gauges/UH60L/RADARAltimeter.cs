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

    [HeliosControl("Helios.UH60L.Instruments", "RADAR Altimeter", "UH-60L", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class RAltimeter : BaseGauge
    {
        private HeliosValue _altitude;
        private GaugeNeedle _needle;
        private HeliosValue _loAltitude;
        private HeliosValue _hiAltitude;
        private HeliosValue _redIndicator;
        private HeliosValue _greenIndicator;
        private HeliosValue _offIndicator;
        private GaugeNeedle _loNeedle;
        private GaugeNeedle _hiNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeImage _giRed;
        private GaugeImage _giGreen;
        private GaugeImage _giOff;

        public RAltimeter()
            : base("RADAR Altimeter", new Size(420, 420))
        {
            //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{Helios}/Images/UH60L/RadAltIndicatorOff.xaml", new Rect(65d, 184d, 44d, 30d)));
            Components.Add(new GaugeImage("{Helios}/Images/UH60L/RadAltIndicatorOff.xaml", new Rect(133d, 47d, 44d, 30d)));
            //Components.Add(new GaugeImage("{FA-18C}/Images/Radar Altimeter Blank.png", new Rect(263d, 300d, 60d, 35d)));

            _giRed = new GaugeImage("{Helios}/Images/UH60L/RadAltIndicatorLo.xaml", new Rect(65d, 184d, 44d, 30d));
            Components.Add(_giRed);
            _redIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Red", "Red Indicator.", "", BindingValueUnits.Boolean);
            _redIndicator.Execute += new HeliosActionHandler(RedIndicator_Execute);
            Values.Add(_redIndicator);
            Actions.Add(_redIndicator);

            _giGreen = new GaugeImage("{Helios}/Images/UH60L/RadAltIndicatorHi.xaml", new Rect(133d, 47d, 44d, 30d));
            Components.Add(_giGreen);
            _greenIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Green", "Green Indicator.", "", BindingValueUnits.Boolean);
            _greenIndicator.Execute += new HeliosActionHandler(GreenIndicator_Execute);
            Values.Add(_greenIndicator);
            Actions.Add(_greenIndicator);

            _giOff = new GaugeImage("{Helios}/Images/UH60L/RadAltFlagOff.xaml", new Rect(240d, 290d, 80d, 44d));
            Components.Add(_giOff);
            _offIndicator = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Off", "Off Indicator.", "", BindingValueUnits.Boolean);
            _offIndicator.Execute += new HeliosActionHandler(OffIndicator_Execute);
            Values.Add(_offIndicator);
            Actions.Add(_offIndicator);

            Components.Add(new GaugeImage("{Helios}/Images/UH60L/RadAltFaceplate.xaml", new Rect(0d, 0d, 420d, 420d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 180d, 180d);
            _needleCalibration.Add(new CalibrationPointDouble(360d, 270d));
            _loNeedle = new GaugeNeedle("{Helios}/Images/UH60L/RadAltBugLo.xaml", new Point(210d, 210d), new Size(21d, 26d), new Point(23d, 210d), 180d);
            Components.Add(_loNeedle);
            _hiNeedle = new GaugeNeedle("{Helios}/Images/UH60L/RadAltBugHi.xaml", new Point(210d, 210d), new Size(21d, 26d), new Point(23d, 210d), 180d);
            Components.Add(_hiNeedle);
            _needle = new GaugeNeedle("{Helios}/Gauges/FA-18C/Altimeter/altimeter_needle.xaml", new Point(210d, 210d), new Size(16d, 220d), new Point(8d, 200d), 180d);
            Components.Add(_needle);

            //Components.Add(new GaugeImage("{Helios}/Gauges/FA-18C/Common/engine_bezel.png", new Rect(0d, 0d, 400d, 400d)));

            _altitude = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Needle", "Current RADAR altitude needle rotational position.", "", BindingValueUnits.Degrees);
            _altitude.Execute += new HeliosActionHandler(AltitudeExecute);
            Actions.Add(_altitude);
            _loAltitude = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter Low Marker", "Low altitude marker rotational position.", "", BindingValueUnits.Degrees);
            _loAltitude.Execute += new HeliosActionHandler(LoAltitudeExecute);
            Actions.Add(_loAltitude);
            _hiAltitude = new HeliosValue(this, new BindingValue(0d), "", "RADAR Altimeter High Marker", "High altitude marker rotational position.", "", BindingValueUnits.Degrees);
            _hiAltitude.Execute += new HeliosActionHandler(HiAltitudeExecute);
            Actions.Add(_hiAltitude);

        }

        void AltitudeExecute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void LoAltitudeExecute(object action, HeliosActionEventArgs e)
        {
            _loNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void HiAltitudeExecute(object action, HeliosActionEventArgs e)
        {
            _hiNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
        void RedIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giRed)].IsHidden = !e.Value.BoolValue;
        }
        void GreenIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giGreen)].IsHidden = !e.Value.BoolValue;
        }
        void OffIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giOff)].IsHidden = e.Value.BoolValue;
        }
    }
}
