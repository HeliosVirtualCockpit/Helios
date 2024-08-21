namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System.Windows;

    /// <summary>
    /// Interaction logic for GlassOpacityAppearanceEditor.xaml
    /// </summary>
    /// 
    [HeliosPropertyEditor("Helios.F15E.Instruments.HydraulicPressure.Utility", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.HydraulicPressure.PC1", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.HydraulicPressure.PC2", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.ADI.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.ADI.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Clock.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Clock.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.IAS.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.IAS.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.VVI.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.VVI.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.OxygenPressure.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.OxygenPressure.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.CabinPressure.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.CabinPressure.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.BAltimeter.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.BAltimeter.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.AoA.Gauge.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.AoA.Gauge.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Instruments.PitchRatio", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.MPD.PilotLeft", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.MPD.PilotCenter", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.MPD.PilotRight", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.MPD.ColorWsoLeft", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.MPD.ColorWsoRight", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.MPD.WsoLeft", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.MPD.WsoRight", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.FuelPanel.WSO", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.FuelPanel.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.EngineMonitorPanel", "Appearance")]
    [HeliosPropertyEditor("Helios.AV8B.Cockpit", "Appearance")]
    [HeliosPropertyEditor("Helios.AV8B.FlightInstruments", "Appearance")]
    [HeliosPropertyEditor("Helios.FA18C.IFEI", "Appearance")]
    [HeliosPropertyEditor("Helios.M2000C.Clock", "Appearance")]
    [HeliosPropertyEditor("Helios.M2000C.ADI.Backup", "Appearance")]
    [HeliosPropertyEditor("Helios.M2000C.Accelerometer", "Appearance")]
    [HeliosPropertyEditor("Helios.M2000C.Instruments.VVI", "Appearance")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.CPGLEFT", "Appearance")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.CPGRIGHT", "Appearance")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.PLTLEFT", "Appearance")]
    [HeliosPropertyEditor("Helios.AH64D.MFD.PLTRIGHT", "Appearance")]
    [HeliosPropertyEditor("Helios.AH64D.TEDAC", "Appearance")]
    [HeliosPropertyEditor("Helios.AH64D.EUFD.CPG", "Appearance")]
    [HeliosPropertyEditor("Helios.AH64D.EUFD.PILOT", "Appearance")]
    [HeliosPropertyEditor("Helios.AH64D.CMWS", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.RWR", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.SFD.Copilot", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.SFD.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.Center", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.CopilotLeft", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.CopilotRight", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.PilotLeft", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.MFD.PilotRight", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.CDU.Top.Copilot", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.CDU.Top.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.CDU.Copilot", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.CDU.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.CH47F.Chronometer", "Appearance")]

    public partial class GlassOpacityAppearanceEditor : HeliosPropertyEditor
    {
        public GlassOpacityAppearanceEditor()
        {
            InitializeComponent();
        }

        private void Opacity_GotFocus(object sender, RoutedEventArgs e)
        {
            // REVISIT nothing to do?
        }
    }
}
