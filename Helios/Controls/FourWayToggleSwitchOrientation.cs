//  Copyright 2025 Helios Contributors
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

using System.ComponentModel;

namespace GadrocsWorkshop.Helios.Controls
{
    public enum FourWayToggleSwitchOrientation
    {
        [Description("No Rotation")]
        None = 0,
        [Description("30° CW Rotation")]
        Rotation30 = 30,
        [Description("60° CW Rotation")]
        Rotation60 = 60,
        [Description("90° CW Rotation")]
        Rotation90 = 90,
        [Description("120° CW Rotation")]
        Rotation120 = 120,
        [Description("150° CW Rotation")]
        Rotation150 = 150,
        [Description("180° CW Rotation")]
        Rotation180 = 180,
        [Description("210° CW Rotation")]
        Rotation210 = 210,
        [Description("240° CW Rotation")]
        Rotation240 = 240,
        [Description("270° CW Rotation")]
        Rotation270 = 270,
        [Description("300° CW Rotation")]
        Rotation300 = 300,
        [Description("330° CW Rotation")]
        Rotation330 = 330,
    }
}
