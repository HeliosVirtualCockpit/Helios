//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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
    /// <summary>
    /// interaction styles for rotary controls
    ///
    /// WARNING: these value names must never be changed, as they are serialized to Profile XML
    /// </summary>
    public enum RotaryClickType
    {
        [Description("Touch on right to repeatedly increment or left to decrement")]
        Touch,

        [Description("Swipe around the control to increment or decrement")]
        Swipe,

        [Description("Reverse Swipe around the control to decrement or increment")]
        ReverseSwipe,

        [Description("Drag a radial handle for precise control")]
        Radial
    }
    /// <summary>
    /// interaction styles for clickable rotary controls
    ///
    /// WARNING: these value names must never be changed, as they are serialized to Profile XML
    /// </summary>
    public enum RotaryClickAllowRotationType
    {
        [Description("Allow both clicked and unclicked knobs to rotate")]
        Both,

        [Description("Allow only Unclicked knob to rotate")]
        Unclicked,

        [Description("Allow only Clicked knob to rotate")]
        Clicked
    }

    /// <summary>
    /// interaction styles for linear controls
    ///
    /// WARNING: these value names must never be changed, as they are serialized to Profile XML
    /// </summary>
    public enum LinearClickType
    {
        [Description("Touch the location towards which the control should move")]
        Touch,

        [Description("Swipe over the control in the direction it should move")]
        Swipe,
    }
    public enum ClickControlType
    {
        [Description("Push Button")]
        PushButton,
        [Description("Toggle Switch")]
        Switch
    }
}
