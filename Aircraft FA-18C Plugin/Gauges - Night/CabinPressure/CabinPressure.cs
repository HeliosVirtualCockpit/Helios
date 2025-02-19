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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{ 
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Gauges.A_10.CabinPressure;
    using System;
    using System.Windows;

    [HeliosControl("Helios.FA18C.cabinPressure2", "Cabin Pressure - Night", "F/A-18C Gauges - Night", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class cabinPressure2 : CabinPressure
    {
        public cabinPressure2()
            : base()
        {
            Components.RemoveAt(Components.Count - 1);  // remove the bezel
            Components.Insert(1, new GaugeImage("{FA-18C}/Gauges/CabinPressure/N Cabin_Pressure_Faceplate.png", new Rect(32d, 38d, 300, 300)));  // add in a new faceplate
            Components.RemoveAt(0);  // remove the original faceplate
        }
    }
}
