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

namespace GadrocsWorkshop.Helios.Gauges.MiG21.RSBNDistance
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.MiG-21.RSBNDistance", "RSBN Distance", "MiG-21", typeof(GaugeRenderer))]
    public class RSBNDistance : BaseGauge
    {
        private HeliosValue _ones;
        private HeliosValue _tens;
        private HeliosValue _hundreds;
        private GaugeDrumCounter _hundredsDrum;
        private GaugeDrumCounter _tensDrum;
        private GaugeDrumCounter _onesDrum;

        public RSBNDistance()
            : base("RSBN Distance", new Size(262, 152))
        {
            Components.Add(new GaugeImage("{MiG-21}/Gauges/RSBNDistance/RSBNDistance.png", new Rect(0d, 0d, 262d, 152d)));
  
            //======================================================================
            //need to create a new overload for GaugeDrumCounter to go the other way
            //but dont want to touch Gadrocs code until at least mine is ok and he's 
            //happy with it. Can live with the wrong rotation, still correct display.
            //=======================================================================

            _onesDrum = new GaugeDrumCounter("{MiG-21}/Gauges/Common/drum_tape.xaml", new Point(180d, 21.5d), "%", new Size(10d, 15d), new Size(35d, 55d));
            _onesDrum.Clip = new RectangleGeometry(new Rect(180d, 21.5d, 35d, 55d));
            Components.Add(_onesDrum);

            _tensDrum = new GaugeDrumCounter("{MiG-21}/Gauges/Common/drum_tape.xaml", new Point(111.5d, 21.5d), "#", new Size(10d, 15d), new Size(35d, 55d));
            _tensDrum.Clip = new RectangleGeometry(new Rect(111.5d, 21.5d, 35d, 55d));
            Components.Add(_tensDrum);

            _hundredsDrum = new GaugeDrumCounter("{MiG-21}/Gauges/Common/drum_tape.xaml", new Point(45.5d, 21.5d), "#", new Size(10d, 15d), new Size(35d, 55d));
            _hundredsDrum.Clip = new RectangleGeometry(new Rect(45.5d, 21.5d, 35d, 55d));
            Components.Add(_hundredsDrum);

            _ones = new HeliosValue(this, new BindingValue(0d), "", "Single Meters", "RSBN Distance", "Distance singles", BindingValueUnits.Numeric);
            _ones.Execute += new HeliosActionHandler(Drum_One_Execute);
            Actions.Add(_ones);

            _tens = new HeliosValue(this, new BindingValue(0d), "", "Tens Meters", "RSBN Distance", "Distance Tens", BindingValueUnits.Numeric);
            _tens.Execute += new HeliosActionHandler(Drum_Tens_Execute);
            Actions.Add(_tens);

            _hundreds = new HeliosValue(this, new BindingValue(0d), "", "Hundreds Meters", "RSBN Distance", "Distance Hundreds", BindingValueUnits.Numeric);
            _hundreds.Execute += new HeliosActionHandler(Drum_Hundreds_Execute);
            Actions.Add(_hundreds);
        }

        void Drum_One_Execute(object action, HeliosActionEventArgs e)
        {
            _onesDrum.Value = e.Value.DoubleValue;
        }

        void Drum_Tens_Execute(object action, HeliosActionEventArgs e)
        {
            _tensDrum.Value = e.Value.DoubleValue;
        }

        void Drum_Hundreds_Execute(object action, HeliosActionEventArgs e)
        {
            _hundredsDrum.Value = e.Value.DoubleValue;
        }

    }
}
