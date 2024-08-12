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

namespace GadrocsWorkshop.Helios.Gauges.CH47F.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.CH47F.RadAlt.Instrument", "RADAR Altimeter Instrument", "CH-47F Chinook", typeof(GaugeRenderer),HeliosControlFlags.None)]
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
        private bool _lowIndicatorState;
        private bool _highIndicatorState;   
        private RadAlt _radAltGauge;
        private bool _instrumentOn = false;

        public RadAltInstrument(string name, Size size, RadAlt radAltGauge)
            : base(name, size)
        {
            _radAltGauge = radAltGauge; 
            //  The first three images are the default images which appear behind the indicators.
            Components.Add(new GaugeImage("{UH-60L}/Images/RadAltIndicatorOff.xaml", new Rect(65d, 184d, 44d, 30d)));
            Components.Add(new GaugeImage("{UH-60L}/Images/RadAltIndicatorOff.xaml", new Rect(133d, 47d, 44d, 30d)));

            _giLowIndicator = new GaugeImage("{UH-60L}/Images/RadAltIndicatorLo.xaml", new Rect(65d, 184d, 44d, 30d));
            Components.Add(_giLowIndicator);
            _lowIndicator = new HeliosValue(this, new BindingValue(0d), name, "Low flag", "Low Altitude Indicator.", "", BindingValueUnits.Boolean);
            _lowIndicator.Execute += new HeliosActionHandler(LowIndicator_Execute);
            Values.Add(_lowIndicator);
            Actions.Add(_lowIndicator);

            _giHighIndicator = new GaugeImage("{UH-60L}/Images/RadAltIndicatorHi.xaml", new Rect(133d, 47d, 44d, 30d));
            Components.Add(_giHighIndicator);
            _HighIndicator = new HeliosValue(this, new BindingValue(0d), name, "High flag", "High Altitude Indicator.", "", BindingValueUnits.Boolean);
            _HighIndicator.Execute += new HeliosActionHandler(HighIndicator_Execute);
            Values.Add(_HighIndicator);
            Actions.Add(_HighIndicator);

            _giOffIndicator = new GaugeImage("{UH-60L}/Images/RadAltFlagOff.xaml", new Rect(238d, 290d, 74d, 42d));
            Components.Add(_giOffIndicator);
            _offIndicator = new HeliosValue(this, new BindingValue(0d), name, "Off flag", "Indicator to show instrument is off.", "", BindingValueUnits.Boolean);
            _offIndicator.Execute += new HeliosActionHandler(OffIndicator_Execute);
            Values.Add(_offIndicator);
            Actions.Add(_offIndicator);

            Components.Add(new GaugeImage("{UH-60L}/Images/RadAltFaceplate.xaml", new Rect(0d, 0d, 420d, 420d)));

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 180d, 180d);
            _needleCalibration.Add(new CalibrationPointDouble(360d, 270d));
            _loNeedle = new GaugeNeedle("{UH-60L}/Images/RadAltBugLo.xaml", new Point(210d, 210d), new Size(21d, 26d), new Point(23d, 210d), 183d);
            Components.Add(_loNeedle);
            _hiNeedle = new GaugeNeedle("{UH-60L}/Images/RadAltBugHi.xaml", new Point(210d, 210d), new Size(21d, 26d), new Point(23d, 210d), 183d);
            Components.Add(_hiNeedle);
            _needle = new GaugeNeedle("{UH-60L}/Images/RadAltNeedle.xaml", new Point(210d, 210d), new Size(40d, 193d), new Point(20d, 20d), 0d);
            Components.Add(_needle);

            _altitude = new HeliosValue(this, new BindingValue(0d), name, "Altitude Needle", "Current RADAR altitude needle rotational position.", "", BindingValueUnits.Degrees);
            _altitude.Execute += new HeliosActionHandler(AltitudeExecute);
            Actions.Add(_altitude);
            _loAltitude = new HeliosValue(this, new BindingValue(0d), name, "Low Altitude Bug Marker", "Low altitude marker rotational position.", "", BindingValueUnits.Degrees);
            _loAltitude.Execute += new HeliosActionHandler(LoAltitudeExecute);
            Actions.Add(_loAltitude);
            _hiAltitude = new HeliosValue(this, new BindingValue(0d), name, "High Altitude Bug Marker", "High altitude marker rotational position.", "", BindingValueUnits.Degrees);
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
        void LowIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _lowIndicatorState = e.Value.BoolValue;
            Components[Components.IndexOf(_giLowIndicator)].IsHidden = !_lowIndicatorState;
        }
        void HighIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            _highIndicatorState = e.Value.BoolValue;
            Components[Components.IndexOf(_giHighIndicator)].IsHidden = !_highIndicatorState;
        }
        void OffIndicator_Execute(object action, HeliosActionEventArgs e)
        {
            Components[Components.IndexOf(_giOffIndicator)].IsHidden = e.Value.BoolValue;
            _instrumentOn = !e.Value.BoolValue;
            if (!_instrumentOn)
            {
                Components[Components.IndexOf(_giLowIndicator)].IsHidden = !_lowIndicatorState;
                Components[Components.IndexOf(_giHighIndicator)].IsHidden = !_highIndicatorState;
            }
            _radAltGauge.TextVisibility = e.Value.BoolValue;

        }
    }
}
