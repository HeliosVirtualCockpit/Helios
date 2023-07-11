//  Copyright 2014 Craig Courtney
//  Copyright 2023 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.F15E.UFC
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.DCS.F15E;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.F15E.UFC.PILOT", "Up Front Controller (Pilot)", "F-15E Strike Eagle", typeof(BackgroundImageRenderer))]
    public class UFC_Pilot : UFC
    {
        private string _interfaceDevice = "";
        private HeliosPanel _frameBezelPanel;
        private const string Panel_Image = "UFC_Panel_";
        private const string ImageLocation = "{F-15E}/Images/UFC/";

        public UFC_Pilot()
            : base("UFC Panel (Pilot)", Cockpit.Pilot)
        {
            _frameBezelPanel = AddPanel("UFC Frame Pilot", new Point(Left, NativeSize.Height - 161), new Size(NativeSize.Width, 161), $"{ImageLocation}{Panel_Image}Lower.png", _interfaceDevice);
            _frameBezelPanel.Opacity = 1d;
            _frameBezelPanel.FillBackground = false;
            _frameBezelPanel.DrawBorder = false;
            InterfaceDevice = "HUD Control Panel";
            AddThreeWayToggle("HUD Symbology Reject Mode", 172, 695, new Size(37, 86), "HUD Symbology Reject Mode");
            AddThreeWayToggle("HUD DAY/AUTO/NIGHT Mode Selector", 289, 695, new Size(37, 86), "HUD DAY/AUTO/NIGHT Mode Selector");

            AddPot("HUD Brightness Control", new Point(51d, 684d), new Size(75d, 75d), "HUD Brightness Control", $"{ImageLocation}UFC_Knob_1a.png", 225d, 270d);
            AddPot("HUD Video Brightness Control", new Point(415d, 684d), new Size(75d, 75d), "HUD Video Brightness Control", $"{ImageLocation}UFC_Knob_1a.png", 225d, 270d);
            AddPot("HUD Contrast Control", new Point(500d, 684d), new Size(75d, 75d), "HUD Contrast Control", $"{ImageLocation}UFC_Knob_1a.png", 225d, 270d);
            string[] MasterModes = { "AA", "AG", "Nav", "Inst" };
            int j = 0;
            for (int i = 180; i <= 405; i += 75)
            {
                AddIndicatorPushButton($"{MasterModes[j]} Master Mode Selector", new Point(i, 793), new Size(47,30), MasterModes[j], "HUD Control Panel", $"{MasterModes[j]} Master Mode Selector",$"{MasterModes[j++]} Master Mode Indicator");
            }
        }
    }
}
