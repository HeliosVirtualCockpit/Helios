//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.HeliosInformation
{
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System.Windows;

    /// <summary>
    /// Interaction logic for HeliosInformationInterfaceEditor.xaml
    /// </summary>
    public partial class HeliosInformationInterfaceEditor : HeliosInterfaceEditor
    {
        public HeliosInformationInterfaceEditor()
        {
            InitializeComponent();

            SetTextBlockText();
        }

        private void SetTextBlockText()
        {
            HeliosVersion.Text = "Helios Version: " + ConfigManager.HeliosVersion;
            HeliosPath.Text = "Helios Path: " + ConfigManager.DocumentPath;
            BMSFalconPath.Text = "BMS Falcon Path: " + ConfigManager.BMSFalconPath;
            ProfileName.Text = "Active Profile Name: the name of the profile being run by Control Center";

            BMSFalconPath.Visibility = ConfigManager.BMSFalconPath.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
