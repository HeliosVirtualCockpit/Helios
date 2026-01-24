//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for TextualDisplayEnablerAppearancePropertyEditor.xaml
    /// </summary>
    /// <remarks >This is used to set a boolean to enable textual displays which are usually
    /// used instead of viewports.  The text displays when enabled will cover holes in the 
    /// panels which allow the viewport to show through, and replace them with text boxes
    /// which are populated from the UDP port (usually)</remarks>

    [HeliosPropertyEditor("HELIOS.M2000C.FUEL_BURN_BINGO_PANEL", "Appearance")]
    [HeliosPropertyEditor("HELIOS.M2000C.RADIO_REPEATER_DISPLAY", "Appearance")]
    [HeliosPropertyEditor("HELIOS.M2000C.PPA_PANEL", "Appearance")]
    [HeliosPropertyEditor("HELIOS.M2000C.PCA_PANEL", "Appearance")]
    [HeliosPropertyEditor("HELIOS.M2000C.PCN_PANEL", "Appearance")]
    [HeliosPropertyEditor("Helios.A10C.ARC210Radio", "Appearance")]
    [HeliosPropertyEditor("Helios.C130J.ARC210Radio", "Appearance")]
    public partial class TextualDisplayEnablerAppearancePropertyEditor : HeliosPropertyEditor
    {
        public TextualDisplayEnablerAppearancePropertyEditor()
        {
            InitializeComponent();
        }
    }
}
