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

namespace GadrocsWorkshop.Helios.Gauges.C130J.NoseWheel
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

    [HeliosControl("Helios.C130J.NoseWheel", "Nose Wheel Gauge", "C-130J Hercules", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class AileronTrim : CompositeBaseGauge
    {
        private readonly HeliosValue _position;

        private readonly GaugeNeedle _positionNeedle;
        private readonly CalibrationPointCollectionDouble _wheelCalibration;

        public AileronTrim()
            : base("NoseWheel", new Size(300, 200))
        {
            SupportedInterfaces = new[] { typeof(C130JInterface) };

            Components.Add(new GaugeImage("{C-130J}/Gauges/NoseWheel/Herc-Nosewheel-scale.xaml", new Rect(17d, 18d, 265.720d, 98.491d)));

            _positionNeedle = new GaugeNeedle("{C-130J}/Gauges/NoseWheel/Herc-Nosewheel-Needle.xaml", new Point(150d, 158.119d), new Size(15.600d, 134.682d), new Point(8.301d, 134.177d)); // 
            Components.Add(_positionNeedle);

            Components.Add(new GaugeImage("{C-130J}/Gauges/NoseWheel/Herc-Nosewheel-Marks.xaml", new Rect(17d, 18d, 265.720d, 98.491d)));
            Components.Add(new GaugeImage("{C-130J}/Gauges/NoseWheel/Herc-Nosewheel-Bezel.xaml", new Rect(0d, 0d, 300d, 200d)));


            foreach (GaugeComponent gc in Components)
            {
                gc.EffectsExclusion = this.EffectsExclusion;
            }

            _position = new HeliosValue(this, BindingValue.Empty, "NoseWheel Gauge", "position", "Current rotation of the nose wheel.", "-90 to +90 degrees", BindingValueUnits.Degrees);
            _position.Execute += new HeliosActionHandler(Position_Execute);
            Actions.Add(_position);

            _wheelCalibration = new CalibrationPointCollectionDouble(-90d, -90d, 90d, 90d){
                new CalibrationPointDouble(0d,0d) };
            CreateInputBindings();
        }
        void CreateInputBindings()
        {

            Dictionary<string, string> bindings = new Dictionary<string, string>
            {
                { "Mech Interface.Nose Wheel Position Indicator.changed", "NoseWheel Gauge.set.position" },
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
            _positionNeedle.Rotation = _wheelCalibration.Interpolate(e.Value.DoubleValue);
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
