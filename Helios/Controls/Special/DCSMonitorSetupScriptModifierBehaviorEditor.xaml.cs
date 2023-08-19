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
    /// Interaction logic for DCSMonitorScriptModifierBehaviorEditor.xaml
    /// </summary>
    /// 
    [HeliosPropertyEditor("Helios.Base.DCSMonitorScriptModifier", "Behavior")]
    public partial class DCSMonitorScriptModifierBehaviorEditor : HeliosPropertyEditor
    {
        public DCSMonitorScriptModifierBehaviorEditor()
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