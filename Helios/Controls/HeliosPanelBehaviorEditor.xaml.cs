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

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System.Windows;

    /// <summary>
    /// Panel behavior for pass through interaction with underlying controls
    /// </summary>
    [HeliosPropertyEditor("Helios.Panel", "Behavior")]
    [HeliosPropertyEditor("Helios.Panel.Timer", "Behavior")]

    public partial class HeliosPanelBehaviorEditor : HeliosPropertyEditor
    {
        public HeliosPanelBehaviorEditor()
        {
            InitializeComponent();
        }      

        private void Chk1_Clicked(object sender, RoutedEventArgs e)
        {
            Chk1.IsChecked = true;
            Chk2.IsChecked = false;
            Chk3.IsChecked = false;
        }

        private void Chk2_Clicked(object sender, RoutedEventArgs e)
        {
            Chk1.IsChecked = false;
            Chk2.IsChecked = true;
            Chk3.IsChecked = false;
        }

        private void Chk3_Clicked(object sender, RoutedEventArgs e)
        {
            Chk1.IsChecked = false;
            Chk2.IsChecked = false;
            Chk3.IsChecked = true;
        }
    }
}
