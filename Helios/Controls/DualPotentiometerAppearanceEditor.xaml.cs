//  Copyright 2014 Craig Courtney
//  Copyright 2026 Helios Contributors
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
    /// Interaction logic for DualPotentiometerAppearanceEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.DualPotentiometer", "Appearance")]
    public partial class DualPotentiometerAppearanceEditor : HeliosPropertyEditor
    {
        public DualPotentiometerAppearanceEditor()
        {
            InitializeComponent();
        }

        // while a rotation slider is being adjusted, move the pots to the position that slider
        // describes, so the effect can be seen in the editor

        private void Potentiometer1InitialRotation_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Control is DualPotentiometer pots)
            {
                pots.Potentiometer1Value = pots.Potentiometer1MinValue;
                pots.Potentiometer2Value = pots.Potentiometer2MinValue;
            }
        }

        private void Potentiometer1RotationTravel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Control is DualPotentiometer pots)
            {
                pots.Potentiometer1Value = pots.Potentiometer1MaxValue;
                pots.Potentiometer2Value = pots.Potentiometer2MinValue;
            }
        }

        private void Potentiometer2InitialRotation_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Control is DualPotentiometer pots)
            {
                pots.Potentiometer1Value = pots.Potentiometer1MinValue;
                pots.Potentiometer2Value = pots.Potentiometer2MinValue;
            }
        }

        private void Potentiometer2RotationTravel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Control is DualPotentiometer pots)
            {
                pots.Potentiometer1Value = pots.Potentiometer1MinValue;
                pots.Potentiometer2Value = pots.Potentiometer2MaxValue;
            }
        }
    }
}
