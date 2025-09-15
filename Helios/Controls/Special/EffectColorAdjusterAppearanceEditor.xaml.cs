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



namespace GadrocsWorkshop.Helios.Controls.Special
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System;
    using System.Globalization;
    using System.Windows.Data;

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
            SliderHighlights.Value = 1.0;
            SliderMidtones.Value = 0.5;
            SliderShadows.Value = 1.0;
            ShaderName.ImageFilename = "ColorAdjust.psc";
        }
    }
    public class DoubleToStringN2Converter : IValueConverter
    {
        // double -> string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
                return d.ToString("N2", culture); // e.g. "1,234.57"
            return "0.00";
        }

        // string -> double
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && double.TryParse(s, NumberStyles.Any, culture, out double result))
                return result;

            return Binding.DoNothing; // prevents overwriting with invalid input
        }
    }
}

