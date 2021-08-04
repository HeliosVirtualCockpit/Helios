//  Copyright 2014 Craig Courtney
//  Copyright 2021 Helios Contributors
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

#pragma warning disable IDE0054 // Use compound assignment

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;


	/// <summary>
	/// Interaction logic for MapControlsBaseMapsEditor.xaml
	/// </summary>

	[HeliosPropertyEditor("Helios.Falcon.MapControl", "Base Maps")]
	[HeliosPropertyEditor("Helios.Falcon.MapViewer", "Base Maps")]

	public partial class MapControlsBaseMapEditor : HeliosPropertyEditor
	{
		public MapControlsBaseMapEditor()
		{
			InitializeComponent();
		}
	}
}
