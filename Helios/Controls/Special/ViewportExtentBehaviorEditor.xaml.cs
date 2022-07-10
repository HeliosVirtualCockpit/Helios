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

using System.Windows;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Windows.Controls;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// Interaction logic for ViewportExtentBehaviorEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.ViewportExtent", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.EUFD.CPG", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.EUFD.PILOT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.PLTRIGHT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.PLTLEFT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.CPGRIGHT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.CPGLEFT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.TEDAC", "Behavior")]
    public partial class ViewportExtentBehaviorEditor : HeliosPropertyEditor
    {
        public ViewportExtentBehaviorEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FontChooserDialog dialog = new FontChooserDialog();
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }
}