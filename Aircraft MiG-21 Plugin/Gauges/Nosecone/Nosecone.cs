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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.Nosecone
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.MiG-21.Nosecone", "Nosecone", "MiG-21", typeof(GaugeRenderer))]
    public class Nosecone : BaseGauge
    {
        private GaugeNeedle _currentNeedle;
        private GaugeNeedle _manualNeedle;

        private CalibrationPointCollectionDouble _needleCalibration;

        private HeliosValue _currentPosition;
        private HeliosValue _manualPosition;

        public Nosecone()
            : base("Nosecone", new Size(340, 340))
        {
            Point center = new Point(170, 170);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 1d, 300d);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Nosecone/nosecone_faceplate.xaml", new Rect(0, 0, 340, 340)));

            _currentNeedle = new GaugeNeedle("{MiG-21}/Gauges/Common/generic_needle.xaml", center, new Size(32, 185), new Point(16, 127), 180d);
            Components.Add(_currentNeedle);

            _manualNeedle = new GaugeNeedle("{MiG-21}/Gauges/Nosecone/nosecone_outer_needle.xaml", center, new Size(32, 230), new Point(16, 132), 180d);
            Components.Add(_manualNeedle);

            Components.Add(new GaugeImage("{MiG-21}/Gauges/Common/generic_bezel.xaml", new Rect(0, 0, 340, 340)));


            _currentPosition = new HeliosValue(this, BindingValue.Empty, "", "Nosecone", "Current Position", "", BindingValueUnits.Numeric);
            _currentPosition.Execute += CurrentPosition_Execute;
            Actions.Add(_currentPosition);

            _manualPosition = new HeliosValue(this, BindingValue.Empty, "", "Nosecone Manual", "Use Manual Knob controller output", "", BindingValueUnits.Numeric);
            _manualPosition.Execute += ManualPosition_Execute;
            Actions.Add(_manualPosition);
        }

        private void CurrentPosition_Execute(object action, HeliosActionEventArgs e)
        {
            _currentPosition.SetValue(e.Value, e.BypassCascadingTriggers);
            _currentNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        private void ManualPosition_Execute(object action, HeliosActionEventArgs e)
        {
            _manualPosition.SetValue(e.Value, e.BypassCascadingTriggers);
            _manualNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
