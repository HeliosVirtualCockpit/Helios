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

namespace GadrocsWorkshop.Helios.Gauges.C130J.RudderTrim
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.C130J;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Xml.Linq;

    [HeliosControl("Helios.C130J.RudderTrim", "Rudder Trim Gauge", "C-130J Hercules", typeof(GaugeRenderer), HeliosControlFlags.None)]
    public class RudderTrim : CompositeBaseGauge
    {
        private HeliosValue _position;

        private GaugeNeedle _positionNeedle;

        private CalibrationPointCollectionDouble _needleCalibration;

        private bool _suppressScale = false;

        public RudderTrim()
            : base("RudderTrim", new Size(300, 300))
        {
            SupportedInterfaces = new[] { typeof(C130JInterface) };

            Components.Add(new GaugeImage("{C-130J}/Gauges/Trim/Herc-Rudder-Trim-Faceplate.xaml", new Rect(0d, 0d, 300d, 300d)));

            _positionNeedle = new GaugeNeedle("{C-130J}/Gauges/Trim/Herc-Rudder-Trim-Needle.xaml", new Point(150d, 150d), new Size(24.772d, 93.688d), new Point(12.391d, 81.302d));
            Components.Add(_positionNeedle);


            foreach (GaugeComponent gc in Components)
            {
                gc.EffectsExclusion = this.EffectsExclusion;
            }

            _position = new HeliosValue(this, BindingValue.Empty, "Rudder Trim Gauge", "position", "Current position of the rudder trim.", "-45 to +45 degrees", BindingValueUnits.Degrees);
            _position.Execute += new HeliosActionHandler(Position_Execute);
            Actions.Add(_position);

            _needleCalibration = new CalibrationPointCollectionDouble(-30d, -90d, 30d, 90d){
                new CalibrationPointDouble(0d,0d) };
            CreateInputBindings();
        }
        void CreateInputBindings()
        {

            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { "Instruments.Rudder Trim Needle.changed", "Rudder Trim Gauge.set.position" },
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

        void Position_Execute(object action, HeliosActionEventArgs e)
        {
            _positionNeedle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }

        /// <summary>
        /// Whether this control will have effects applied to is on rendering.
        /// </summary>
        public override bool EffectsExclusion
        {
            get => base.EffectsExclusion;
            set
            {
                if (!base.EffectsExclusion.Equals(value))
                {
                    base.EffectsExclusion = value;
                    OnPropertyChanged("EffectsExclusion", !value, value, true);
                }
            }
        }
        public override void ScaleChildren(double scaleX, double scaleY)
        {
            base.ScaleChildren(scaleX, scaleY);
        }
        protected override void PostUpdateRectangle(Rect previous, Rect current)
        {
            //_suppressScale = false;
            //if (!previous.Equals(new Rect(0, 0, 0, 0)) && !(previous.Width == current.Width && previous.Height == current.Height))
            //{
            //    _ball.ScaleChildren(current.Width / previous.Width, current.Height / previous.Height);
            //    _suppressScale = true;
            //}
        }
        public override void Reset()
        {
            base.Reset();
            _position.SetValue(new BindingValue(0d), true);
        }
    }
}
