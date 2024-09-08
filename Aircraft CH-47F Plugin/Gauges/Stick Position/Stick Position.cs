//  Copyright 2014 Craig Courtney
//  Copyright 2021 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.CH47F.StickPosition
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.CH47F.StickPosition", "Stick Position Indicator", "CH-47F Chinook", typeof(GaugeRenderer),HeliosControlFlags.None)]
    public class StickPosition : BaseGauge
    {
        private HeliosValue _stickPosition;
        private GaugeNeedle _indicator;
        private CalibrationPointCollectionDouble _indicatorCalibration;

        public StickPosition()
            : base("StickPosition", new Size(86, 904))
        {
            Components.Add(new GaugeImage("{CH-47F}/Gauges/Stick Position/Images/Stick Position Indicator Background.xaml", new Rect(0d, 0d, 86, 904)));

            _indicatorCalibration = new CalibrationPointCollectionDouble(-8d, 0d, 8d, 800d){};
            _indicator = new GaugeNeedle("{CH-47F}/Gauges/Stick Position/Images/Stick Position Indicator Red Marker.xaml", new Point(34d, -850d), new Size(18, 900), new Point(0, 0), 0d);
            _indicator.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 86, 904) };
            Components.Add(_indicator);

            _stickPosition = new HeliosValue(this, new BindingValue(-8d), "", "Longitudinal Stick Position", "Current position of the stick.", "-8 to +8", BindingValueUnits.Degrees);
            _stickPosition.Execute += new HeliosActionHandler(StickMove_Execute);
            Actions.Add(_stickPosition);
            Components.Add(new GaugeImage("{CH-47F}/Gauges/Stick Position/Images/Stick Position Indicator Scale.xaml", new Rect(0d, 0d, 86, 904)));

        }

        void StickMove_Execute(object action, HeliosActionEventArgs e)
        {
            _indicator.VerticalOffset = _indicatorCalibration.Interpolate(e.Value.DoubleValue);
        }

    }
}
