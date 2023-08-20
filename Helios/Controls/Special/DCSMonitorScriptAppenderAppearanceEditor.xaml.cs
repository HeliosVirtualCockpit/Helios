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

using System.Windows.Input;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using GadrocsWorkshop.Helios.Windows;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;

    /// <summary>
    /// Appearance editor for DCS Monitor Script Modifier Background Image
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.DCSMonitorScriptAppender", "Appearance")]
    public partial class DCSMonitorScriptAppenderAppearanceEditor : HeliosPropertyEditor
    {
        public DCSMonitorScriptAppenderAppearanceEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// backing field for property ResetCommand, contains
        /// command handlers for resetting the background image path
        /// </summary>
        private ICommand _resetCommand;

        /// <summary>
        /// command handlers for resetting the background image path
        /// </summary>
        public ICommand ResetCommand
        {
            get
            {
            _resetCommand = _resetCommand ?? new RelayCommand(
                parameter =>
                {
                    if (!(Control is IConfigurableBackgroundImage configurable))
                    {
                        return;
                    }
                    configurable.BackgroundImage = configurable.DefaultBackgroundImage;
                },
                parameter =>
                {
                    if (!(Control is IConfigurableBackgroundImage configurable))
                    {
                        return false;
                    }
                    return configurable.BackgroundImage != configurable.DefaultBackgroundImage;
                });
                return _resetCommand;
            }
        }
    }
}
