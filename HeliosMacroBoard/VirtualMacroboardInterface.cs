// Copyright 2014 Craig Courtney
// Copyright 2022 Helios Contributors
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

using GadrocsWorkshop.Helios.ComponentModel;
using OpenMacroBoard.SDK;

namespace GadrocsWorkshop.Helios.Interfaces.HeliosMacroBoard
{
    [HeliosInterface("Helios.Base.VirtualMacroBoardInterface", "HeliosMacroBoard Virtual Board",
        typeof(MacroBoardInterfaceEditor),
        typeof(UniqueHeliosInterfaceFactory), AutoAdd = false)]
    public class VirtualMacroboardInterface : MacroBoardInterface
    {
        public VirtualMacroboardInterface() : base(MacroBoardModel.Virtual, "HeliosMacroBoard Virtual Board", 4, 8)
        {
        }

        #region Overrides

        protected override IMacroBoard OpenDevice() =>
            OpenMacroBoard.VirtualBoard.BoardFactory.SpawnVirtualBoard(new GridKeyPositionCollection(8, 4, 72, 25));

        #endregion
    }
}