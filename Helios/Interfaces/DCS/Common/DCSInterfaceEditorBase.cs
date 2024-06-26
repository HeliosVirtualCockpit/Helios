﻿// Copyright 2021 Helios Contributors
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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using GadrocsWorkshop.Helios.Windows;
using GadrocsWorkshop.Helios.Windows.Controls;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    /// TODO: implement a content container into which specific options can be added.
    public class DCSInterfaceEditorBase : HeliosInterfaceEditor
    {
        protected DCSPhantomMonitorFixConfig _phantomFix;

        /// <summary>
        /// backing field for property ConfigureCommand, contains
        /// handler for Configure button
        /// </summary>
        private ICommand _configureCommand;

        public static readonly RoutedUICommand AddDoFileCommand =
            new RoutedUICommand("Adds a dofile(...) to a DCS config.", "AddDoFile", typeof(DCSInterfaceEditor));

        public static readonly RoutedUICommand RemoveDoFileCommand =
            new RoutedUICommand("Removes a dofile(...) to a DCS config.", "RemoveDoFile", typeof(DCSInterfaceEditor));

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(DCSExportConfiguration), typeof(DCSInterfaceEditor),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PhantomFixProperty =
            DependencyProperty.Register("PhantomFix", typeof(DCSPhantomMonitorFixConfig), typeof(DCSInterfaceEditor),
                new PropertyMetadata(null));

        public static readonly DependencyProperty VehicleImpersonationProperty =
            DependencyProperty.Register("VehicleImpersonation", typeof(DCSVehicleImpersonation),
                typeof(DCSInterfaceEditor), new PropertyMetadata(null));

        protected static void RegisterCommands(Type ownerType)
        {
            CommandManager.RegisterClassCommandBinding(ownerType,
                new CommandBinding(AddDoFileCommand, AddDoFile_Executed));
            CommandManager.RegisterClassCommandBinding(ownerType,
                new CommandBinding(RemoveDoFileCommand, RemoveDoFile_Executed));
        }

        public DCSInterfaceEditorBase()
        {
            DataContext = this;
        }

        /// <summary>
        /// handler for Configure button
        /// </summary>
        public ICommand ConfigureCommand
        {
            get
            {
                _configureCommand = _configureCommand ?? new RelayCommand(parameter =>
                {
                    Configuration.Install(new InstallationDialogs(this));
                });
                return _configureCommand;
            }
        }

        /// <summary>
        /// used by UI binding paths.
        /// </summary>
        public DCSExportConfiguration Configuration => (DCSExportConfiguration) GetValue(ConfigurationProperty);

        /// <summary>
        /// used by UI binding paths
        /// </summary>
        public DCSPhantomMonitorFixConfig PhantomFix => (DCSPhantomMonitorFixConfig) GetValue(PhantomFixProperty);

        /// <summary>
        /// used by UI binding paths
        /// </summary>
        public DCSVehicleImpersonation VehicleImpersonation =>
            (DCSVehicleImpersonation) GetValue(VehicleImpersonationProperty);

        /// <summary>
        /// Called immediately after construction when our factory installs the Interface property, not
        /// executed at run time (Profile Editor only)
        /// </summary>
        protected override void OnInterfaceChanged(HeliosInterface oldInterface, HeliosInterface newInterface)
        {
            base.OnInterfaceChanged(oldInterface, newInterface);
            DCSExportConfiguration configuration;
            DCSVehicleImpersonation vehicleImpersonation;
            if (newInterface is DCSInterface dcsInterface)
            {
                // create or connect to configuration objects
                _phantomFix = new DCSPhantomMonitorFixConfig(dcsInterface);
                configuration = dcsInterface.Configuration;
                vehicleImpersonation = dcsInterface.VehicleImpersonation;
            }
            else
            {
                // provoke crash on attempt to use 
                _phantomFix = null;
                configuration = null;
                vehicleImpersonation = null;
            }

            // need to rebind everything on the form
            SetValue((DependencyProperty)ConfigurationProperty, configuration);
            SetValue((DependencyProperty)PhantomFixProperty, _phantomFix);
            SetValue((DependencyProperty)VehicleImpersonationProperty, vehicleImpersonation);
        }

        private static void AddDoFile_Executed(object target, ExecutedRoutedEventArgs e)
        {
            DCSInterfaceEditor editor = target as DCSInterfaceEditor;
            TextBox newDoFile = (TextBox)e.Parameter;
            string file = newDoFile.Text;
            if (editor != null && !string.IsNullOrWhiteSpace(file) && !editor.Configuration.DoFiles.Contains(file))
            {
                editor.Configuration.DoFiles.Add(file);
                newDoFile.Text = "";
            }
        }

        private static void RemoveDoFile_Executed(object target, ExecutedRoutedEventArgs e)
        {
            DCSInterfaceEditor editor = target as DCSInterfaceEditor;
            string file = e.Parameter as string;
            if (editor != null && !string.IsNullOrWhiteSpace(file) && editor.Configuration.DoFiles.Contains(file))
            {
                editor.Configuration.DoFiles.Remove(file);
            }
        }
    }
}