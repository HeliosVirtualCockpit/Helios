﻿//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Gauges.F15E.Instruments.CabinPressure
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.F15E.Instruments.CabinPressure.Pilot", "Cabin Pressure (Pilot)", "F-15E Strike Eagle", typeof(GaugeRenderer),HeliosControlFlags.None)]
    public class CabinPressurePilot : CabinPressure
    {
        public CabinPressurePilot()
            : base("Cabin Pressure (Pilot)", new Size(364, 376))
        {
        }
    }
}
