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

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Windows;
using System.Windows;

namespace GadrocsWorkshop.Helios.Patching.DCS.ViewModel
{
    /// <summary>
    /// view model for the configuration and application of DCS viewport patches
    ///
    /// this will grow if the additional viewports interface ever does something else
    /// </summary>
    public class AdditionalViewportsViewModel<T> : DependencyObject where T : NotificationObject
    {
        public AdditionalViewportsViewModel(DCSPatchInstallation viewportData, DCSPatchInstallation communityData)
        {
            Data = viewportData;
            CommunityData = communityData;
        }

        protected static void GenerateHeliosUndoForProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // pretend this dependency object is a NotificationObject in the tree under our data object and
            // generate an Undo record that will set our property back if called
            AdditionalViewportsViewModel<DCSPatchInstallation> sourceObject = (AdditionalViewportsViewModel<DCSPatchInstallation>)d;
            /// ToDo:  Determine which of the two source objects actually changed
            sourceObject.Data.OnPropertyChanged(
                e.Property.Name,
                new PropertyNotificationEventArgs(sourceObject, e.Property.Name, e.OldValue, e.NewValue));
            sourceObject.CommunityData.OnPropertyChanged(
                e.Property.Name,
                new PropertyNotificationEventArgs(sourceObject, e.Property.Name, e.OldValue, e.NewValue));
        }

        #region Properties

        public DCSPatchInstallation Data { get; }
        public DCSPatchInstallation CommunityData { get; }

        #endregion        
    }
}