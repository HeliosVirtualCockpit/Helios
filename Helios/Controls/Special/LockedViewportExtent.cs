// Copyright 2021 Ammo Goettsch
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// a version of ViewportExtent that can't be edited, so its viewport name can be set in a template and never changed
    /// by the UI
    /// </summary>
    [HeliosControl("Helios.Base.LockedViewportExtent", "Simulator Viewport", "Special Controls", typeof(ViewportExtentRenderer), HeliosControlFlags.NotShownInUI)]
    public class LockedViewportExtent : ViewportExtentBase
    {
        // no code, just a version of the class without an associated behavior editor
    }
}