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

namespace GadrocsWorkshop.Helios.Gauges
{
    using GadrocsWorkshop.Helios;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;

    public class AltImageGauge : CompositeBaseGauge
    {
        private HeliosValue _alternateImages;
        private string _alternateImageSetFolderName = "";
        private bool _enableAlternateImageSet = false;
        private bool _enableAction = true;

        protected AltImageGauge(string name, Size nativeSize, string alternateImageSetFolderName = "Alt", bool enableAction = true)
            : base(name, nativeSize)
        {
            _enableAction = enableAction;
            _alternateImageSetFolderName = alternateImageSetFolderName;
            if (enableAction)
            {
                _alternateImages = new HeliosValue(this, new BindingValue(false), "", "Enable Alternate Image Set", "Indicates whether the alternate image set is to be used", "True or False", BindingValueUnits.Boolean);
                _alternateImages.Execute += new HeliosActionHandler(EnableAltImages_Execute);
                Actions.Add(_alternateImages);
            }
        }

        void EnableAltImages_Execute(object sender, HeliosActionEventArgs e)
        {
            EnableAlternateImageSet = e.Value.BoolValue;
            _alternateImages.SetValue(e.Value, e.BypassCascadingTriggers);
        }

        /// <summary>
        /// Replaces images in the Component collection with identically named images from a 
        /// subfolder (and vice versa)
        /// </summary>
        /// <param name="enableAltImages">Whether the images will come from subfolder, or returned to their original images</param>
        private void ReplaceComponentImages(bool enableAltImages)
        {
            foreach (GaugeComponent gc in Components)
            {
                string imageSubfolder = enableAltImages ? $"/{_alternateImageSetFolderName}" : ""; 
                if (gc is GaugeImage gi)
                {
                    string dir = System.IO.Path.GetDirectoryName(gi.Image);
                    if (new DirectoryInfo(dir).Name == _alternateImageSetFolderName)
                    {
                        dir = Path.GetDirectoryName(dir);
                    }

                    gi.Image = $"{dir}{imageSubfolder}/{System.IO.Path.GetFileName(gi.Image)}";
                    gi.ImageRefresh = true;
                    Refresh();
                    continue;
                }
                if (gc is GaugeNeedle gn)
                {
                    string dir = System.IO.Path.GetDirectoryName(gn.Image);
                    if (new DirectoryInfo(dir).Name == _alternateImageSetFolderName)
                    {
                        dir = Path.GetDirectoryName(dir);
                    }

                    gn.Image = $"{dir}{imageSubfolder}/{System.IO.Path.GetFileName(gn.Image)}";
                    gn.ImageRefresh = true;
                    Refresh();
                    continue;
                }
                if (gc is GaugeDrumCounter gdc)
                {
                    string dir = System.IO.Path.GetDirectoryName(gdc.Image);
                    if (new DirectoryInfo(dir).Name == _alternateImageSetFolderName)
                    {
                        dir = Path.GetDirectoryName(dir);
                    }

                    gdc.Image = $"{dir}{imageSubfolder}/{System.IO.Path.GetFileName(gdc.Image)}";
                    gdc.ImageRefresh = true;
                    Refresh();
                    continue;
                }
            }
        }

        #region Properties
        public string AlternateImageSetFolderName {
            get => _alternateImageSetFolderName;
            set
            {
                string newValue = value;
                string oldValue = _alternateImageSetFolderName;

                if (newValue != oldValue)
                {
                    _alternateImageSetFolderName = newValue;    
                    OnPropertyChanged("AlternateImageSetFolderName", oldValue, newValue, true);
                }
            }
        }

        virtual public bool EnableAlternateImageSet
        {
            get => _enableAlternateImageSet;

            set
            {
                bool newValue = value;
                bool oldValue = _enableAlternateImageSet;

                if (newValue != oldValue)
                {
                    _enableAlternateImageSet = newValue;
                    ReplaceComponentImages(newValue);
                    OnPropertyChanged("EnableAlternateImageSet", oldValue, newValue, true);
                }
            }
        }
        #endregion

        public override void MouseDown(Point location)
        {
            // No-Op
        }

        public override void MouseDrag(Point location)
        {
            // No-Op
        }

        public override void MouseUp(Point location)
        {
            // No-Op
        }

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
                EnableAlternateImageSet = enableAlternateImageSet;
            }
            else
            {
                EnableAlternateImageSet = false;
            }
        }
    }
}
