//  Copyright 2014 Craig Courtney
//  Copyright 2020 Ammo Goettsch
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

using System.Windows.Input;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using GadrocsWorkshop.Helios.Windows;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;

    /// <summary>
    /// Appearance editor for DCS Monitor Script Modifier Background Image
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.Effects.ColorAdjuster", "Appearance")]
    public partial class EffectColorAdjusterAppearanceEditor : HeliosPropertyEditor
    {
        public EffectColorAdjusterAppearanceEditor()
        {
            InitializeComponent();
        }

        private void Reset_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SliderR.Value = 1.0;
            SliderG.Value = 1.0;
            SliderB.Value = 1.0;
            SliderBrt.Value = 0;
            SliderCont.Value = 1.0;
            SliderGamma.Value = 1.0;
        }

        private void SliderR_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            ValueR.Content = SliderR.Value.ToString("N2");
        }
        private void SliderG_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            ValueG.Content = SliderG.Value.ToString("N2");
        }
        private void SliderB_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            ValueB.Content = SliderB.Value.ToString("N2");
        }
        private void SliderBrt_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            ValueBrt.Content = SliderBrt.Value.ToString("N2");
        }
        private void SliderCont_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            ValueContrast.Content = SliderCont.Value.ToString("N2");
        }
        private void SliderGamma_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            ValueGamma.Content = SliderGamma.Value.ToString("N2");
        }
    }
}
