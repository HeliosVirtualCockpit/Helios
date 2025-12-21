//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Controls.DH98Mosquito
{
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.ComponentModel;

    [HeliosControl("Helios.DH98Mosquito.LockingThreeWayToggleSwitch", "Locking Three Way Toggle Switch", "Toggle Switches (Multi-Way)", typeof(ThreeWayToggleSwitchRenderer), HeliosControlFlags.NotShownInUI)]
    public class LockingThreeWayToggleSwitch : Controls.ThreeWayToggleSwitchLocking
    {
        // Retained for Backwards Compatibility following move of this control to Helios.Controls
        public LockingThreeWayToggleSwitch() : base("Locking Three Way Toggle Switch", new System.Windows.Size(50, 100)) { }
        public LockingThreeWayToggleSwitch(string name, System.Windows.Size size) : base(name, size) { }

        #region Properties

        #endregion

        #region HeliosControl Implementation

        #endregion

        #region Actions
        #endregion
    }
}
