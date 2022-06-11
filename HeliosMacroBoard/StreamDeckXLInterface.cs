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
using StreamDeckSharp;

namespace GadrocsWorkshop.Helios.Interfaces.HeliosMacroBoard
{
    [HeliosInterface("Helios.Base.StreamDeckXLInterface", "Stream Deck XL", typeof(MacroBoardInterfaceEditor),
        typeof(UniqueHeliosInterfaceFactory), AutoAdd = false)]
    // ReSharper disable once InconsistentNaming
    public class StreamDeckXLInterface : MacroBoardInterface
    {
        public StreamDeckXLInterface() : base(MacroBoardModel.StreamDeckXL, "Stream Deck XL", 4, 8)
        {
        }

        #region Overrides

        protected override IMacroBoard OpenDevice() =>
            // REVISIT this needs to target a specific device so that we can support multiple devices
            StreamDeck.OpenDevice();

        #endregion
    }
}