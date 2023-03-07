//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.UH60L.Chronometer
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System.Windows;

    [HeliosControl("Helios.UH60L.Chronometer.Pilot", "Chronometer Display (Pilot)", "H-60", typeof(BackgroundImageRenderer), HeliosControlFlags.NotShownInUI)]
    public class ChronometerDisplayPilot : H60.Chronometer.ChronometerDisplayPilot
    {
        public ChronometerDisplayPilot()
            : base()
        {
        }
    }
}
