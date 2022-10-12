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

    [HeliosControl("Helios.UH60L.Instruments", "RADAR Altimeter Instrument", "UH-60L", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class RadAltInstrument : BaseGauge
    {
        private HeliosValue _altitude;
        private GaugeNeedle _needle;
        private HeliosValue _loAltitude;
        private HeliosValue _hiAltitude;
        private HeliosValue _lowIndicator;
        private HeliosValue _HighIndicator;
        private HeliosValue _offIndicator;
        private GaugeNeedle _loNeedle;
        private GaugeNeedle _hiNeedle;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeImage _giLowIndicator;
        private GaugeImage _giHighIndicator;
        private GaugeImage _giOffIndicator;

        public RadAltInstrument(string name, Size size)
            : base(name, size)
        {
            //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{Helios}/Images/UH60L/RadAltIndicatorOff.xaml", new Rect(65d, 184d, 44d, 30d)));
            Components.Add(new GaugeImage("{Helios}/Images/UH60L/RadAltIndicatorOff.xaml", new Rect(133d, 47d, 44d, 30d)));
            //Components.Add(new GaugeImage("{FA-18C}/Images/Radar Altimeter Blank.png", new Rect(263d, 300d, 60d, 35d)));

            _giLowIndicator = new GaugeImage("{Helios}/Images/UH60L/RadAltIndicatorLo.xaml", new Rect(65d, 184d, 44d, 30d));
            Components.Add(_giLowIndicator);
            _lowIndicator = new HeliosValue(this, new BindingValue(0d), "", "Low flag", "Low Altitude Indicator.", "", BindingValueUnits.Boolean);
            _lowIndicator.Execute += new HeliosActionHandler(RedIndicator_Execute);
            Values.Add(_lowIndicator);
            Actions.Add(_lowIndicator);

            _giHighIndicator = new GaugeImage("{Helios}/Images/UH60L/RadAltIndicatorHi.xaml", new Rect(133d, 47d, 44d, 30d));
            Components.Add(_giHighIndicator);
            _HighIndicator = new HeliosValue(this, new BindingValue(0d), "", "High flag", "High Altitude Indicator.", "", BindingValueUnits.Boolean);
            _HighIndicator.Execute += new HeliosActionHandler(GreenIndicator_Execute);
            Values.Add(_HighIndicator);
            Actions.Add(_HighIndicator);

            _giOffIndicator = new GaugeImage("{Helios}/Images/UH60L/RadAltFlagOff.xaml", new Rect(240d, 290d, 80d, 44d));
            Components.Add(_giOffIndicator);
            _offIndicator = new HeliosValue(this, new BindingValue(0d), "", "Off flag", "Indicator to show instrument is off.", "", BindingValueUnits.Boolean);
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

            _altitude = new HeliosValue(this, new BindingValue(0d), "", "Altitude Needle", "Current RADAR altitude needle rotational position.", "", BindingValueUnits.Degrees);
            _altitude.Execute += new HeliosActionHandler(AltitudeExecute);
            Actions.Add(_altitude);
            _loAltitude = new HeliosValue(this, new BindingValue(0d), "", "Low Altitude Bug Marker", "Low altitude marker rotational position.", "", BindingValueUnits.Degrees);
            _loAltitude.Execute += new HeliosActionHandler(LoAltitudeExecute);
            Actions.Add(_loAltitude);
            _hiAltitude = new HeliosValue(this, new BindingValue(0d), "", "High Altitude Bug Marker", "High altitude marker rotational position.", "", BindingValueUnits.Degrees);
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
            Components[Components.IndexOf(_giLowIndicator)].IsHidden = !e.Value.BoolValue;
        }
        void GreenIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giHighIndicator)].IsHidden = !e.Value.BoolValue;
        }
        void OffIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giOffIndicator)].IsHidden = e.Value.BoolValue;
        }
    }
}
