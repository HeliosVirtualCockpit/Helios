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

    /// <summary>
    /// Interaction logic for ImageAppearanceEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("HELIOS.M2000C.LANDING_GEAR_PANEL_V2", "Appearance")]
    [HeliosPropertyEditor("HELIOS.M2000C.UHF_RADIO", "Appearance")]
    [HeliosPropertyEditor("HELIOS.M2000C.VHF_RADIO", "Appearance")]
    [HeliosPropertyEditor("HELIOS.M2000C.IFF_MODE_SETTING", "Appearance")]
    [HeliosPropertyEditor("HELIOS.M2000C.RADAR_IFF_MODE_PANEL", "Appearance")]    
    [HeliosPropertyEditor("HELIOS.M2000C.LOW_ALTITUDE_SETTING", "Appearance")]
    public partial class ImageAssetLocationAppearanceEditor : HeliosPropertyEditor
    {
        public ImageAssetLocationAppearanceEditor()
        {
            InitializeComponent();
        }
    }
}
