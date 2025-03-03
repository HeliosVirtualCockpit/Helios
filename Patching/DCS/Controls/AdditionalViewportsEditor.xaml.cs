// Copyright 2020 Ammo Goettsch
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System.Windows;
using GadrocsWorkshop.Helios.Patching.DCS.ViewModel;
using GadrocsWorkshop.Helios.Windows;
using GadrocsWorkshop.Helios.Windows.Controls;

namespace GadrocsWorkshop.Helios.Patching.DCS.Controls
{
    /// <summary>
    /// This interface editor manages a collection of DCS installation locations and allows installation of viewport patches
    /// into those locations.
    /// It also translates from DCS-specific installation location to generic patching interfaces to be shared with other
    /// instances of patching things
    /// </summary>
    public partial class AdditionalViewportsEditor : HeliosInterfaceEditor
    {
        private readonly InstallationDialogs _installationDialogs;
        private AdditionalViewports _interface;

        public AdditionalViewportsEditor()
        {
            InitializeComponent();
            _installationDialogs = new InstallationDialogs(this);
        }

        /// <summary>
        /// Called immediately after construction when our factory installs the Interface property, not
        /// executed at run time (Profile Editor only)
        /// </summary>
        protected override void OnInterfaceChanged(HeliosInterface oldInterface, HeliosInterface newInterface)
        {
            base.OnInterfaceChanged(oldInterface, newInterface);
            if (newInterface is AdditionalViewports viewportsInterface)
            {
                _interface = viewportsInterface;
                DataContext = new AdditionalViewportsViewModel<DCSPatchInstallation>(_interface.ViewportPatches, _interface.CommunityPatches );
            }
            else
            {
                // deinit; provoke crash on attempt to use 
                _interface = null;
                DataContext = null;
            }
        }

        #region Commands

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            _interface?.ViewportPatches.Install(_installationDialogs);
            _interface?.CommunityPatches.Install(_installationDialogs);
            _interface?.InvalidateStatusReport();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            _interface?.ViewportPatches.Patching.Uninstall(_installationDialogs);
            _interface?.CommunityPatches.Patching.Uninstall(_installationDialogs);
            _interface?.InvalidateStatusReport();
        }

        #endregion
    }
}