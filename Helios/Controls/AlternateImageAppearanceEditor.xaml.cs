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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for AlternateImageAppearanceEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.FA18C.ADI", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.ADI.V1", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.IFEI", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.Instruments.BAltimeter", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.Instruments", "Appearance")] // RadAlt
    [HeliosPropertyEditor("Helios.FA18C.Battery", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.Instruments.BrakePressure", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.Hydraulic", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.IAS", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.Instruments.O2Gauge", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.VVI", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.cabinPressure", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.UFC", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.MPCD.Left", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.MPCD.Right", "Appearance")]
    [HeliosPropertyEditor("FA18C.AMPCD", "Appearance")]
    //[HeliosPropertyEditor("Helios.FA18C.Instruments.BAltimeter", "Appearance")]
    //[HeliosPropertyEditor("Helios.FA18C.Instruments.BAltimeter", "Appearance")]

    public partial class AlternateImageAppearanceEditor : HeliosPropertyEditor
    {
        public AlternateImageAppearanceEditor()
        {
            InitializeComponent();
        }

    }
}
