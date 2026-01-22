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

namespace GadrocsWorkshop.Helios.Gauges.C130J.HDD.Slip
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml.Linq;

    [HeliosControl("Helios.C130J.HDD.Slip", "Slip", "C-130J Hercules", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class Slip : CompositeBaseGauge
    {
        private GaugeNeedle _slipBall;
        private HeliosValue _slipBallValue;

        private bool _suppressScale = false;
        private readonly string _interfaceDevice = "";

            
        public Slip()
            : base("Slip", new Size(111, 14))
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.C130J.C130JInterface) };
            _interfaceDevice = "Slip Gauge";

            Components.Add(new GaugeImage("{C-130J}/Gauges/HDD/Herc-Slip-Tube.xaml", new Rect(0, 0, 111, 14)));

            _slipBall = new GaugeNeedle("{C-130J}/Gauges/HDD/Herc-Slip-Ball.xaml", new Point(55.5d, 7d), new Size(12.50d, 12.50d), new Point(6.25d, 6.25d));
            _slipBall.HorizontalOffset = 0d;
            Components.Add(_slipBall);
            _slipBallValue = new HeliosValue(this, BindingValue.Empty, _interfaceDevice, "slip ball", "Current slip of the aircraft.", "Number between -1 to +1", BindingValueUnits.Numeric);
            _slipBallValue.Execute += new HeliosActionHandler(SlipBall_Execute);
            Actions.Add(_slipBallValue);

            Components.Add(new GaugeImage("{C-130J}/Gauges/HDD/Herc-Slip-Markers.xaml", new Rect(0, 0, 111, 14)));

            foreach (GaugeComponent gc in Components)
            {
                gc.EffectsExclusion = this.EffectsExclusion;
            }
        }
        private void SlipBall_Execute(object action, HeliosActionEventArgs e)
        {
            _slipBall.HorizontalOffset = -e.Value.DoubleValue * 45d;
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
        public override void Reset()
        {
            base.Reset();
            _slipBall.HorizontalOffset = 0d;
            _slipBallValue.SetValue(new BindingValue(0d), true);
        }
    }
}
