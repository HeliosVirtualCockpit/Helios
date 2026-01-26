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
    /// 
    [HeliosPropertyEditor("Helios.Base.ViewportExtent", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.EUFD.CPG", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.EUFD.PILOT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.PLTRIGHT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.PLTLEFT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.CPGRIGHT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.CPGLEFT", "Behavior")]
    [HeliosPropertyEditor("Helios.AH64D.TEDAC", "Behavior")]
    [HeliosPropertyEditor("Helios.F15E.MPD.PilotLeft", "Behavior")]
    [HeliosPropertyEditor("Helios.F15E.MPD.PilotCenter", "Behavior")]
    [HeliosPropertyEditor("Helios.F15E.MPD.PilotRight", "Behavior")]
    [HeliosPropertyEditor("Helios.F15E.MPD.WsoLeft", "Behavior")]
    [HeliosPropertyEditor("Helios.F15E.MPD.ColorWsoLeft", "Behavior")]
    [HeliosPropertyEditor("Helios.F15E.MPD.WsoRight", "Behavior")]
    [HeliosPropertyEditor("Helios.F15E.MPD.ColorWsoRight", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.PilotLeft", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.PilotRight", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.CopilotLeft", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.CopilotRight", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.Center", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.CDU.Top.Pilot", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.CDU.Top.Copilot", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.CDU.Pilot", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.CDU.Copilot", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.SFD.Pilot", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.SFD.Copilot", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.RWR", "Behavior")]
    [HeliosPropertyEditor("Helios.CH47F.Chronometer", "Behavior")]
    [HeliosPropertyEditor("FA18C.AMPCD", "Behavior")]
    [HeliosPropertyEditor("Helios.FA18C.MPCD.Right", "Behavior")]
    [HeliosPropertyEditor("Helios.FA18C.MPCD.Left", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.CNI.PILOT", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.CNI.COPILOT", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.CNI.AUGCREW", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.CNBP", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.AMU.PILOT", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.AMU.COPILOT", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.HDD.Pilot.Left", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.HDD.Pilot.Right", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.HDD.Copilot.Left", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.HDD.Copilot.Right", "Behavior")]
    [HeliosPropertyEditor("Helios.C130J.ARC210Radio", "Behavior")]


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