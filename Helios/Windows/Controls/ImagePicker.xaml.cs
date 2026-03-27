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
    using Microsoft.Win32;
    using System;
    using System.Windows;
    using System.Windows.Controls;
 
    /// <summary>
    /// Interaction logic for ImagePicker.xaml
    /// </summary>
    public partial class ImagePicker : UserControl
    {
        public ImagePicker()
        {
            InitializeComponent();
        }

        #region Properties

        public string ImageFilename
        {
            get { return (string)GetValue(ImageFilenameProperty); }
            set { SetValue(ImageFilenameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageFilename.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageFilenameProperty =
            DependencyProperty.Register("ImageFilename", typeof(string), typeof(ImagePicker), new UIPropertyMetadata(""));

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,
                Multiselect = false,
                ValidateNames = true,
                Filter = "Images Files (*.gif, *.jpg, *.jpe, *.png, *.bmp, *.dib, *.tif, *.wmf, *.pcx, *.tga, *.xaml)|*.gif;*.jpg;*.jpe;*.png;*.bmp;*.dib;*.tif;*.wmf;*.pcx;*.tga;*.xaml",
                Title = "Select Image"
            };

            ofd.CustomPlaces.Add(new FileDialogCustomPlace(ConfigManager.ImagePath));
            
            string path = ConfigManager.ImageManager.MakeImagePathAbsolute(ImageFilename);

            if (string.IsNullOrEmpty(path) || !path.StartsWith(ConfigManager.ImagePath, StringComparison.InvariantCultureIgnoreCase))
            {
                ofd.InitialDirectory = ConfigManager.ImagePath;
            }
            else
            {
                ofd.InitialDirectory = System.IO.Path.GetDirectoryName(path);
                ofd.FileName = path;
            }

            bool? result = ofd.ShowDialog(Window.GetWindow(this));

            if (result == true)
            {
                ImageFilename = ConfigManager.ImageManager.MakeImagePathRelative(ofd.FileName);
            }
        }

    }
}
