using System;

using System.Windows;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls;

namespace GadrocsWorkshop.Helios.Gauges.M2000C.PCNPanel
{
    [HeliosControl("HELIOS.M2000C.PCN_GAUGE", "PCN Panel Gauge", "M-2000C Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]

    public class PCNPanelGauge : BaseGauge
    {
        private GaugeImage _latitudeNorthImage;
        private GaugeImage _latitudeSouthImage;
        private GaugeImage _longitudeEastImage;
        private GaugeImage _longitudeWestImage;
        private HeliosValue _latitudeNorthIndicator;
        private HeliosValue _latitudeSouthIndicator;
        private HeliosValue _longitudeEastIndicator;
        private HeliosValue _longitudeWestIndicator;

        public PCNPanelGauge(string name, Size size)
            : base(name, size)
        {
            _latitudeNorthIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "North Indicator", "North Indicator on the PCN display", "True if displayed.", BindingValueUnits.Boolean);
            _latitudeNorthIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_latitudeNorthIndicator);
            _latitudeSouthIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "South Indicator", "South Indicator on the PCN display", "True if displayed.", BindingValueUnits.Boolean);
            _latitudeSouthIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_latitudeSouthIndicator);
            _longitudeEastIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "East Indicator", "East Indicator on the PCN display", "True if displayed.", BindingValueUnits.Boolean);
            _longitudeEastIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_longitudeEastIndicator);
            _longitudeWestIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "West Indicator", "West Indicator on the PCN display", "True if displayed.", BindingValueUnits.Boolean);
            _longitudeWestIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_longitudeWestIndicator);

            _latitudeNorthImage = new GaugeImage("{M2000C}/Images/PCNPanel/eff-on.png", new Rect(58, 13, 20, 30));
           _latitudeNorthImage.IsHidden = false;
            Components.Add(_latitudeNorthImage);
           _latitudeSouthImage = new GaugeImage("{M2000C}/Images/PCNPanel/eff-on.png", new Rect(58, 42, 20, 30));
           _latitudeSouthImage.IsHidden = false;
            Components.Add(_latitudeSouthImage);
           _longitudeEastImage = new GaugeImage("{M2000C}/Images/PCNPanel/eff-on.png", new Rect(353, 13, 20, 30));
           _longitudeEastImage.IsHidden = false;
            Components.Add(_longitudeEastImage);
           _longitudeWestImage = new GaugeImage("{M2000C}/Images/PCNPanel/eff-on.png", new Rect(353, 42, 20, 30));
           _longitudeWestImage.IsHidden = false;
            Components.Add(_longitudeWestImage);
        }

        void Flag_Execute(object action, HeliosActionEventArgs e)
        {
            HeliosValue hAction = (HeliosValue)action;
            Boolean hActionVal = !(e.Value.DoubleValue > 0d ? true : false);
            switch (hAction.Name)
            {
                case "North Indicator":
                    _latitudeNorthIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
                    _latitudeNorthImage.IsHidden = hActionVal;
                    break;
                case "South Indicator":
                    _latitudeSouthIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
                    _latitudeSouthImage.IsHidden = hActionVal;
                    break;
                case "East Indicator":
                    _longitudeEastIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
                    _longitudeEastImage.IsHidden = hActionVal;
                    break;
                case "West Indicator":
                    _longitudeWestIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
                    _longitudeWestImage.IsHidden = hActionVal;
                    break;
                default:
                    break;
            }
        }
    }
}
