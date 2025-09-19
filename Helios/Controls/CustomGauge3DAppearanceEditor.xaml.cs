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
    /// Interaction logic for CustomGauge3DAppearanceEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.CustomGaugeBall", "Appearance")]
    [HeliosPropertyEditor("Helios.Base.CustomGaugeCylinder", "Appearance")]

    public partial class CustomGauge3DAppearanceEditor : HeliosPropertyEditor
    {
        public CustomGauge3DAppearanceEditor()
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
        private void HeliosTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
    public class DoubleToStringConverter : IValueConverter
    {
        // Slider.Value (double) -> TextBox.Text (string)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
                return d.ToString("N3", culture);
            return "0";
        }

        // TextBox.Text (string) -> Slider.Value (double)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && double.TryParse(s, NumberStyles.Any, culture, out double result))
                return result;
            return Binding.DoNothing; // prevents overwriting with 0
        }
    }
    public class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string s && double.TryParse(s, NumberStyles.Any, cultureInfo, out _))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Input must be a number");
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
    public class DoubleToStringN0Converter : IValueConverter
    {
        // double -> string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
                return d.ToString("N0", culture); // e.g. "1,234.57"
            return "0";
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
