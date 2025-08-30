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
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;


    /// <summary>
    /// Interaction logic for CustomGauge3D3LightingAppearanceEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.CustomGaugeBall", "Appearance")]
    [HeliosPropertyEditor("Helios.Base.CustomGaugeCylinder", "Appearance")]
    
    public partial class CustomGauge3D3LightingAppearanceEditor : HeliosPropertyEditor
    {
        public CustomGauge3D3LightingAppearanceEditor()
        {
            InitializeComponent();
        }

        private void MinPosition_GotFocus(object sender, RoutedEventArgs e)
        {
            CustomGauge pot = Control as CustomGauge;
            if (pot != null)
            {
                pot.Value = pot.MinValue;
            }
        }

        private void MaxPosition_GotFocus(object sender, RoutedEventArgs e)
        {
            CustomGauge pot = Control as CustomGauge;
            if (pot != null)
            {
                pot.Value = pot.MaxValue;
            }
        }
    }
}
