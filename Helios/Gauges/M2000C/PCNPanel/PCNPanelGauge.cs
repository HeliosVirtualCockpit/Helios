using System;

using System.Windows;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls;

namespace GadrocsWorkshop.Helios.Gauges.M2000C.PCNPanel
{
    [HeliosControl("HELIOS.M2000C.PCN_GAUGE", "PCN Panel Gauge", "M-2000C Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]

    internal class PCNPanelGauge : BaseGauge
    {
        private GaugeImage _latitudeNorthImage;
        private GaugeImage _latitudeSouthImage;
        private GaugeImage _longitudeEastImage;
        private GaugeImage _longitudeWestImage;

        private GaugeImage _leftPlusImage;
        private GaugeImage _leftMinusImage;
        private GaugeImage _rightPlusImage;
        private GaugeImage _rightMinusImage;

        private HeliosValue _latitudeNorthIndicator;
        private HeliosValue _latitudeSouthIndicator;
        private HeliosValue _longitudeEastIndicator;
        private HeliosValue _longitudeWestIndicator;

        private HeliosValue _leftPlusIndicator;
        private HeliosValue _leftMinusIndicator;
        private HeliosValue _rightPlusIndicator;
        private HeliosValue _rightMinusIndicator;


        internal PCNPanelGauge(M2000C_PCNPanel panel, string name, Size size)
            : base(name, size)
        {
            _latitudeNorthIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "North Indicator", "North Indicator on the PCN display", "True if displayed.", BindingValueUnits.Boolean);
            _latitudeNorthIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            panel.Actions.Add(_latitudeNorthIndicator);
            _latitudeSouthIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "South Indicator", "South Indicator on the PCN display", "True if displayed.", BindingValueUnits.Boolean);
            _latitudeSouthIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            panel.Actions.Add(_latitudeSouthIndicator);
            _longitudeEastIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "East Indicator", "East Indicator on the PCN display", "True if displayed.", BindingValueUnits.Boolean);
            _longitudeEastIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            panel.Actions.Add(_longitudeEastIndicator);
            _longitudeWestIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "West Indicator", "West Indicator on the PCN display", "True if displayed.", BindingValueUnits.Boolean);
            _longitudeWestIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            panel.Actions.Add(_longitudeWestIndicator);

            _leftPlusIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "Left Plus Indicator", "Plus Indicator on the PCN Left Upper display", "True if displayed.", BindingValueUnits.Boolean);
            _leftPlusIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            panel.Actions.Add(_leftPlusIndicator);
            _leftMinusIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "Left Minus Indicator", "Minus Indicator on the PCN Left Upper display", "True if displayed.", BindingValueUnits.Boolean);
            _leftMinusIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            panel.Actions.Add(_leftMinusIndicator);
            _rightPlusIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "Right Plus Indicator", "Plus Indicator on the PCN Right Upper display", "True if displayed.", BindingValueUnits.Boolean);
            _rightPlusIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            panel.Actions.Add(_rightPlusIndicator);
            _rightMinusIndicator = new HeliosValue(this, new BindingValue(false), "PCN Panel", "Right Minus Indicator", "Minus Indicator on the PCN Right Upper display", "True if displayed.", BindingValueUnits.Boolean);
            _rightMinusIndicator.Execute += new HeliosActionHandler(Flag_Execute);
            panel.Actions.Add(_rightMinusIndicator);

            Components.Add(new GaugeImage("{M2000C}/Images/PCNPanel/PCNScreenBackground.png", new Rect(53, 4, 585, 74)));
            Components.Add(new GaugeImage("{M2000C}/Images/PCNPanel/PCNScreenBackground.png", new Rect(53, 86, 585, 74)));

            _latitudeNorthImage = new GaugeImage("{Helios}/Gauges/M2000C/PCNPanel/PCN_North.xaml", new Rect(58, 23, 16, 16));
           _latitudeNorthImage.IsHidden = false;
            Components.Add(_latitudeNorthImage);
           _latitudeSouthImage = new GaugeImage("{Helios}/Gauges/M2000C/PCNPanel/PCN_South.xaml", new Rect(58, 52, 16, 16));
           _latitudeSouthImage.IsHidden = false;
            Components.Add(_latitudeSouthImage);
           _longitudeEastImage = new GaugeImage("{Helios}/Gauges/M2000C/PCNPanel/PCN_East.xaml", new Rect(343, 23, 16, 16));
           _longitudeEastImage.IsHidden = false;
            Components.Add(_longitudeEastImage);
           _longitudeWestImage = new GaugeImage("{Helios}/Gauges/M2000C/PCNPanel/PCN_West.xaml", new Rect(343, 52, 16, 16));
           _longitudeWestImage.IsHidden = false;
            Components.Add(_longitudeWestImage);

            _leftPlusImage = new GaugeImage("{Helios}/Gauges/M2000C/PCNPanel/PCN_Plus.xaml", new Rect(78, 23, 16, 16));
            _leftPlusImage.IsHidden = false;
            Components.Add(_leftPlusImage);
            _leftMinusImage = new GaugeImage("{Helios}/Gauges/M2000C/PCNPanel/PCN_Minus.xaml", new Rect(80, 60, 14, 12));
            _leftMinusImage.IsHidden = false;
            Components.Add(_leftMinusImage);
            _rightPlusImage = new GaugeImage("{Helios}/Gauges/M2000C/PCNPanel/PCN_Plus.xaml", new Rect(364, 23, 16, 16));
            _rightPlusImage.IsHidden = false;
            Components.Add(_rightPlusImage);
            _rightMinusImage = new GaugeImage("{Helios}/Gauges/M2000C/PCNPanel/PCN_Minus.xaml", new Rect(366, 60, 14, 12));
            _rightMinusImage.IsHidden = false;
            Components.Add(_rightMinusImage);

        }

        void Flag_Execute(object action, HeliosActionEventArgs e)
        {
            HeliosValue hAction = (HeliosValue)action;
            Boolean hActionVal = !e.Value.BoolValue;
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
                case "Left Plus Indicator":
                    _leftPlusIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
                    _leftPlusImage.IsHidden = hActionVal;
                    break;
                case "Left Minus Indicator":
                    _leftMinusIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
                    _leftMinusImage.IsHidden = hActionVal;
                    break;
                case "Right Plus Indicator":
                    _rightPlusIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
                    _rightPlusImage.IsHidden = hActionVal;
                    break;
                case "Right Minus Indicator":
                    _rightMinusIndicator.SetValue(e.Value, e.BypassCascadingTriggers);
                    _rightMinusImage.IsHidden = hActionVal;
                    break;
                default:
                    break;
            }
        }
    }
}
