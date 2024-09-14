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

    /// <summary>
    /// Interaction logic for PotentiometerClickableAppearanceEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.PotentiometerClickable", "Appearance")]
    public partial class PotentiometerClickableAppearanceEditor : HeliosPropertyEditor
    {
        public PotentiometerClickableAppearanceEditor()
        {
            InitializeComponent();
        }

        private void InitialRotation_GotFocus(object sender, RoutedEventArgs e)
        {
            PotentiometerKnob pot = Control as PotentiometerKnob;
            if (pot != null)
            {
                pot.Value = pot.MinValue;
            }
        }

        private void RotationTravel_GotFocus(object sender, RoutedEventArgs e)
        {
            PotentiometerKnob pot = Control as PotentiometerKnob;
            if (pot != null)
            {
                pot.Value = pot.MaxValue;
            }
        }
    }
}
