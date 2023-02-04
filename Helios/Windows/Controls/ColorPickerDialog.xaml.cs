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

namespace GadrocsWorkshop.Helios.Windows.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Documents;
	using System.Windows.Navigation;
	using System.Windows.Shapes;
	using System.Windows.Data;
	using System.Windows.Media;
	using System.Windows.Media.Animation;
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for ColorPickerDialog.xaml
    /// </summary>

    public partial class ColorPickerDialog : Window
    {


        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        private void okButtonClicked(object sender, RoutedEventArgs e)
        {

            OKButton.IsEnabled = false;
            m_color = cPicker.SelectedColor;
            DialogResult = true;
            Hide();

        }


        private void cancelButtonClicked(object sender, RoutedEventArgs e)
        {

            OKButton.IsEnabled = false;
            DialogResult = false;

        }

        private void onSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {

            if (e.NewValue != m_color)
            {

                OKButton.IsEnabled = true;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {

            OKButton.IsEnabled = false;
            base.OnClosing(e);
        }


        private Color m_color = new Color();
        private Color startingColor = new Color();

        public Color SelectedColor
        {
            get
            {
                return m_color;
            }

        }
        
        public Color StartingColor
        {
            get
            {
                return startingColor;
            }
            set
            {
                cPicker.SelectedColor = value;
                OKButton.IsEnabled = false;
                
            }

        }

        private void getButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ConfigManager.SettingsManager.IsSettingAvailable("ProfileEditor", "StoredColor"))
            {
                TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(System.Windows.Media.Color));
                StartingColor = (System.Windows.Media.Color)colorConverter.ConvertFromString(ConfigManager.SettingsManager.LoadSetting("ProfileEditor", "StoredColor", "#FFFFFFFF"));
                OKButton.IsEnabled = true;
            }
        }

        private void setButtonClicked(object sender, RoutedEventArgs e)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(System.Windows.Media.Color));
            ConfigManager.SettingsManager.SaveSetting("ProfileEditor", "StoredColor", colorConverter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, cPicker.SelectedColor));
        }
    }
}