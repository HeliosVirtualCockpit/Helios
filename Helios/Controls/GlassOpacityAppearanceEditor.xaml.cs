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
    [HeliosPropertyEditor("Helios.F15E.Clock.Pilot", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.Clock.WSO", "Appearance")]
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
    [HeliosPropertyEditor("Helios.F15E.FuelPanel", "Appearance")]
    [HeliosPropertyEditor("Helios.F15E.EngineMonitorPanel", "Appearance")]
    [HeliosPropertyEditor("Helios.AV8B.Cockpit", "Appearance")]
    [HeliosPropertyEditor("Helios.AV8B.FlightInstruments", "Appearance")]
    
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
