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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{ 
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Gauges.A_10.CabinPressure;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;

    [HeliosControl("Helios.FA18C.cabinPressure", "Cabin Pressure", "F/A-18C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class cabinPressure : CabinPressure
    {
        private HeliosValue _alternateImages;
        private string _altImageLocation = "";

        public cabinPressure()
            : base()
        {
            SupportedInterfaces = new[] { typeof(Interfaces.DCS.FA18C.FA18CInterface) };
            CreateInputBindings();

            _alternateImages = new HeliosValue(this, new BindingValue(false), "", "Enable Alternate Image Set", "Indicates whether the alternate image set is to be used", "True or False", BindingValueUnits.Boolean);
            _alternateImages.Execute += new HeliosActionHandler(EnableAltImages_Execute);
            Actions.Add(_alternateImages);


            Components.Clear();  // remove the gauge components
            Components.Add(new GaugeImage("{FA-18C}/Gauges/CabinPressure/Cabin_Pressure_Faceplate.png", new Rect(32d, 38d, 300, 300)));  // add in a new faceplate
            Components.Add(new GaugeNeedle("{FA-18C}/Gauges/CabinPressure/cabin_pressure_needle.xaml", new Point(182d, 188d), new Size(53d, 158d), new Point(26.5d, 26.5d), 0d));
        }
        void CreateInputBindings()
        {
            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "Cockpit Lights.MODE Switch.changed",
                deviceActionName: "set.Enable Alternate Image Set",
                deviceTriggerName: "",
                triggerBindingValue: new BindingValue("return TriggerValue<3"),
                triggerBindingSource: BindingValueSources.LuaScript
                );

            AddDefaultInputBinding(
                childName: "",
                interfaceTriggerName: "System Gauges.Cabin Altitude.changed",
                deviceActionName: "set.cabin pressure"
                );
        }
        void SetImages(string altImageLocation)
        {
            foreach(GaugeComponent gc in Components)
            {
                if(gc is GaugeImage gi)
                {
                    string dir = System.IO.Path.GetDirectoryName(gi.Image);
                    if (new DirectoryInfo(dir).Name == "Alt") {
                        dir = Path.GetDirectoryName(dir);
                    }

                    gi.Image = $"{dir}{altImageLocation}/{System.IO.Path.GetFileName(gi.Image)}";
                    gi.ImageRefresh = true;
                    Refresh();
                    continue;
                }
                if (gc is GaugeNeedle gn)
                {
                    string dir = System.IO.Path.GetDirectoryName(gn.Image);
                    if (new DirectoryInfo(dir).Name == "Alt")
                    {
                        dir = Path.GetDirectoryName(dir);
                    }

                    gn.Image = $"{dir}{altImageLocation}/{System.IO.Path.GetFileName(gn.Image)}";
                    gn.ImageRefresh = true;
                    Refresh();
                    continue;
                }
                if (gc is GaugeDrumCounter gdc)
                {
                    string dir = System.IO.Path.GetDirectoryName(gdc.Image);
                    if (new DirectoryInfo(dir).Name == "Alt")
                    {
                        dir = Path.GetDirectoryName(dir);
                    }

                    gdc.Image = $"{dir}{altImageLocation}/{System.IO.Path.GetFileName(gdc.Image)}";
                    gdc.ImageRefresh = true;
                    Refresh();
                    continue;
                }
            }
        }

        void EnableAltImages_Execute(object sender, HeliosActionEventArgs e)
        {
            SetImages(e.Value.BoolValue ? "/Alt" : "");
            _altImageLocation = e.Value.BoolValue ? "/Alt" : "";
            _alternateImages.SetValue(e.Value, e.BypassCascadingTriggers);
        }
        #region Properties
        public bool EnableAlternateImageSet
        {
            get
            {
                return _altImageLocation == "" ? false : true;
            }
            set
            {
                bool newValue = value;

                bool oldValue = _altImageLocation == "" ? false : true;

                if (newValue != oldValue)
                {
                    _altImageLocation = newValue ? "/Alt" : "";
                    // notify change after change is made
                    OnPropertyChanged("EnableAlternateImageSet", oldValue, newValue, true);
                    SetImages(_altImageLocation);
                }
            }
        }
        #endregion
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            if (EnableAlternateImageSet) writer.WriteElementString("EnableAlternateImageSet", EnableAlternateImageSet.ToString(CultureInfo.InvariantCulture));
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            if (reader.NodeType != System.Xml.XmlNodeType.EndElement && reader.Name == "EnableAlternateImageSet")
            {
                bool enableAlternateImageSet = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("EnableAlternateImageSet"));
                _altImageLocation = enableAlternateImageSet ? "/Alt" : "";
                SetImages(_altImageLocation);
            }
            else
            {
                EnableAlternateImageSet = false;
            }
        }

    }
}
