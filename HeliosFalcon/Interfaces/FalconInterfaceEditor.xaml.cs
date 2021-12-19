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

using System;
using System.IO;
using System.Windows;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Windows.Controls;
using Microsoft.Win32;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon
{
    /// <summary>
    /// Interaction logic for FalconInterfaceEditor.xaml
    /// </summary>
    public partial class FalconInterfaceEditor : HeliosInterfaceEditor
    {
        public FalconInterfaceEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DereferenceLinks = true;
            ofd.Multiselect = false;
            ofd.ValidateNames = true;
            ofd.Filter = "Key Files (*.key)|*.key";
            ofd.Title = "Select Key File";

            if (Directory.Exists(((FalconInterface)Interface).FalconPath))
            {
                if (((FalconInterface)Interface).FalconType == FalconTypes.BMS)
                {
                    ofd.InitialDirectory = ((FalconInterface)Interface).FalconPath + "\\User\\Config";
                }
                else
                {
                    ofd.InitialDirectory = ((FalconInterface)Interface).FalconPath;
                }
            }
            else
            {
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            Nullable<bool> result = ofd.ShowDialog(Window.GetWindow(this));

            if (result == true)
            {
                ((FalconInterface)Interface).KeyFileName = ConfigManager.ImageManager.MakeImagePathRelative(ofd.FileName);
            }

        }

        private void Chk90_Clicked(object sender, RoutedEventArgs e)
        {
            Chk90.IsChecked = true;
            Chk60.IsChecked = false;
            Chk30.IsChecked = false;
        }

        private void Chk60_Clicked(object sender, RoutedEventArgs e)
        {
            Chk90.IsChecked = false;
            Chk60.IsChecked = true;
            Chk30.IsChecked = false;
        }

        private void Chk30_Clicked(object sender, RoutedEventArgs e)
        {
            Chk90.IsChecked = false;
            Chk60.IsChecked = false;
            Chk30.IsChecked = true;
        }
    }

    public class DesignTimeFalconInterface : DesignTimeInterface<FalconInterface>
    {
        // no code
    }
}
