﻿//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System.Windows;

    [HeliosControl("Helios.UH60L.Stabilator.Indicator", "Stabilator Position Instrument", "H-60", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class StabInstrument : H60.Instruments.StabInstrument
    {
        public StabInstrument(string name, Size size)
            : base(name, size)
        {
        }
    }
}
