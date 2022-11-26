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

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;

namespace GadrocsWorkshop.Helios.Windows.Controls
{
    /// <summary>
    /// A picker for a Lua file, with optional initial guess where to get it
    /// </summary>
    public partial class FolderPicker : UserControl
    {
        public FolderPicker()
        {
            InitializeComponent();
        }

        #region Properties

        /// <summary>
        /// initial folder or null
        /// </summary>
        public string FolderGuess
        {
            get => (string)GetValue(FolderGuessProperty);
            set => SetValue(FolderGuessProperty, value);
        }

        public static readonly DependencyProperty FolderGuessProperty =
            DependencyProperty.Register("FolderGuess", typeof(string), typeof(FolderPicker),
                new PropertyMetadata(null));

        /// <summary>
        /// picker result full path
        /// </summary>
        public string SelectedFolderPath
        {
            get => (string)GetValue(SelectedFolderPathProperty);
            set => SetValue(SelectedFolderPathProperty, value);
        }

        public static readonly DependencyProperty SelectedFolderPathProperty =
            DependencyProperty.Register("SelectedFolderPath", typeof(string), typeof(FolderPicker),
                new UIPropertyMetadata(""));

        /// <summary>
        /// text to show in the picker text field
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FolderPicker), new PropertyMetadata(null));

        /// <summary>
        /// the human-readable short description of the type of file being selected
        /// </summary>
        public string FolderDescription
        {
            get => (string)GetValue(FolderDescriptionProperty);
            set => SetValue(FolderDescriptionProperty, value);
        }

        public static readonly DependencyProperty FolderDescriptionProperty =
            DependencyProperty.Register("FolderDescription", typeof(string), typeof(FolderPicker),
                new PropertyMetadata("Folder"));

        /// <summary>
        /// extension to enforce on files being selected, not including the dot
        /// </summary>
        public string FileExtension
        {
            get => (string)GetValue(FileExtensionProperty);
            set => SetValue(FileExtensionProperty, value);
        }

        public static readonly DependencyProperty FileExtensionProperty =
            DependencyProperty.Register("FileExtension", typeof(string), typeof(FolderPicker),
                new PropertyMetadata("folder"));

        #endregion

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFolderPath = null;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // initialize to no file selected, in case we are run again and pick the same file
            // we want to trigger change events again, since there is no guarantee the file contents
            // are still the same
            SelectedFolderPath = null;
            string guess = FolderGuess.StartsWith("{") ? null : FolderGuess.ToString();

            CommonOpenFileDialog openFolderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                EnsurePathExists = true,
                Multiselect = false,
                InitialDirectory = guess ?? ConfigManager.ImagePath,
                ShowPlacesList = true
            };
            string path = ConfigManager.ImageManager.MakeImagePathAbsolute(SelectedFolderPath);

            if (string.IsNullOrEmpty(path) || !path.StartsWith(ConfigManager.ImagePath))
            {
                openFolderDialog.InitialDirectory = ConfigManager.ImagePath;
            }
            else
            {
                //openFolderDialog.FileName = path;
            }
            if (openFolderDialog.ShowDialog(Window.GetWindow(this)) == CommonFileDialogResult.Ok)
            {
                // write the selected file, which will usually trigger some bindings
                SelectedFolderPath = ConfigManager.ImageManager.MakeImagePathRelative(openFolderDialog.FileName)+"/";
            }
        }
    }
}