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

namespace GadrocsWorkshop.Helios.Gauges.AV8B
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;

    [HeliosControl("Helios.AV8B.trimPositionRudder", "AV-8B Rudder Trim Position", "AV-8B Harrier", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class trimPositionRudder: trimPosition
    {
        public trimPositionRudder()
            : base(new GaugeImage("{AV-8B}/Gauges/Trim Position/Trim_rudder_faceplate.xaml", new Rect(0d, 0d, 300d, 300d)), "Rudder Trim Position", new Size(300d, 300d)) { }

    }
}
