namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System.Windows;

    /// <summary>
    /// Interaction logic for GlassOpacityAppearanceEditor.xaml
    /// </summary>
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
