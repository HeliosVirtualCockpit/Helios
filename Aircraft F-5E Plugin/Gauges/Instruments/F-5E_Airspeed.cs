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

namespace GadrocsWorkshop.Helios.Gauges.F5E.Instruments.Airspeed
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    [HeliosControl("Helios.F5E.Instruments.Airspeed", "Airspeed Gauge", "F-5E Tiger II", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class Airspeed : BaseGauge
    {
        private readonly HeliosValue _airspeedIndicated;
        private readonly HeliosValue _airspeedMach;
        private readonly HeliosValue _airspeedMax;
        private readonly HeliosValue _airspeedSet;

        private readonly GaugeNeedle _needleIndicated;
        private readonly GaugeNeedle _needleMach;
        private readonly GaugeNeedle _needleMax;
        private readonly GaugeNeedle _needleSet;

        private double _airspeedValue = 0;
        private double _airspeedMachValue = 0;

        private readonly CalibrationPointCollectionDouble _calibrationPoints;
        private readonly string _gaugeImagePath = "{F-5E}/Gauges/Instruments/Images/";

        public Airspeed(): base("Airspeed Gauge", new Size(300d,300d))
        {
            double scalingFactor = this.Height / NativeSize.Height;
            scalingFactor = 1d;
            Components.Add(new GaugeImage($"{_gaugeImagePath}F-5E_Airspeed_FacePlate.xaml", new Rect(0d, 0d, this.Height, this.Width)));

            _needleMach = new GaugeNeedle($"{_gaugeImagePath}F-5E_Airspeed_Mach_Dial.xaml", new Point(this.Height / 2, this.Width / 2), new Size(193d, 193d), new Point(96.5d, 96.5d), 0d);
            Components.Add(_needleMach);

            _needleSet = new GaugeNeedle($"{_gaugeImagePath}F-5E_Airspeed_Set_Bug.xaml", new Point(this.Height / 2, this.Width / 2), new Size(23.406d, 150.676d), new Point(12.7d, 150d), 0d);
            Components.Add(_needleSet);

            _needleMax = new GaugeNeedle($"{_gaugeImagePath}F-5E_Airspeed_Max_Bug.xaml", new Point(this.Height / 2, this.Width / 2), new Size(20, 109.500), new Point(10, 109), 0d);
            Components.Add(_needleMax);

            _needleIndicated = new GaugeNeedle($"{_gaugeImagePath}F-5E_Airspeed_Outer_Dial.xaml", new Point(this.Height / 2, this.Width / 2), new Size(192, 222), new Point(96.5d, 127), 0d);
            Components.Add(_needleIndicated);

            _airspeedIndicated = new HeliosValue(this, new BindingValue(0d), "", "Indicated Airspeed Rotation", "Numeric", "(0 to 1)", BindingValueUnits.Numeric);
            _airspeedIndicated.Execute += new HeliosActionHandler(Airspeed_Execute);
            Actions.Add(_airspeedIndicated);

            _airspeedMach = new HeliosValue(this, new BindingValue(0d), "", "Airspeed Mach Rotation", "Numeric", "(0 to 1)", BindingValueUnits. Numeric);
            _airspeedMach.Execute += new HeliosActionHandler(Airspeed_Execute);
            Actions.Add(_airspeedMach);

            _airspeedSet = new HeliosValue(this, new BindingValue(0d), "", "Airspeed Set Bug Rotation", "Numric", "(0 to 1)", BindingValueUnits.Numeric);
            _airspeedSet.Execute += new HeliosActionHandler(Airspeed_Execute);
            Actions.Add(_airspeedSet);

            _airspeedMax = new HeliosValue(this, new BindingValue(0d), "", "Airspeed Max Bug Rotation", "Numric", "(0 to 1)", BindingValueUnits.Numeric);
            _airspeedMax.Execute += new HeliosActionHandler(Airspeed_Execute);
            Actions.Add(_airspeedMax);

            _calibrationPoints = new CalibrationPointCollectionDouble(0d, 0d, 1d, 346.5d)
            {
            };
        }
        void Airspeed_Execute(object action, HeliosActionEventArgs e)
        {
            if (action is HeliosValue hv)
            {
                switch (hv.Name)
                {
                    case "Indicated Airspeed Rotation":
                        _airspeedValue = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        _needleIndicated.Rotation = _airspeedValue;
                        _needleMach.Rotation = _airspeedMachValue + _airspeedValue;
                        break;
                    case "Airspeed Mach Rotation":
                        _airspeedMachValue = e.Value.DoubleValue * 360d;
                        _needleMach.Rotation = _airspeedMachValue + _airspeedValue;
                        break;
                    case "Airspeed Set Bug Rotation":
                        _needleSet.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    case "Airspeed Max Bug Rotation":
                        _needleMax.Rotation = _calibrationPoints.Interpolate(e.Value.DoubleValue);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
