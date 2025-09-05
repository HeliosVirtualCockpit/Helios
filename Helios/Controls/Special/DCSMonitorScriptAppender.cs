// Copyright 2020 Ammo Goettsch
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls.Capabilities;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// Special viewport extent with an associated editor, so the user can introduce lua at the end of monitorsetup files without
    /// triggering the warning to reconfigure monitors in the DCS Monitor Setup interface
    /// </summary>
    [HeliosControl("Helios.Base.DCSMonitorScriptAppender", "DCS Monitor Setup Script Appender", "Special Controls", typeof(DCSMonitorScriptAppenderRenderer),HeliosControlFlags.NotShownInUI)]
    public class DCSMonitorScriptAppender : ViewportExtentBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const string DEFAULT_TEXT = "DCS Monitor Script Appender";
        private string _dcsMonitorSetupAdditionalLua = "";
        private string _imageFile = "";

        public DCSMonitorScriptAppender()
            : base()
        {
            ViewportName = DEFAULT_TEXT;
            RequiresPatches = false;
            Width = 130;
            FillBackground = false;
            BackgroundColor = Color.FromArgb(0, 0, 0, 0);
            FontColor = Colors.IndianRed;
            Format.VerticalAlignment = TextVerticalAlignment.Top;
            Format.HorizontalAlignment = TextHorizontalAlignment.Center;
            Format.FontSize = 14;
            Text = DEFAULT_TEXT;
        }
        public string DCSMonitorSetupAdditionalLua {
            get => _dcsMonitorSetupAdditionalLua;
            set
            {
                if (_dcsMonitorSetupAdditionalLua == value)
                {
                    return;
                }
                string oldValue = _dcsMonitorSetupAdditionalLua;
                _dcsMonitorSetupAdditionalLua = value;
                OnPropertyChanged(nameof(DCSMonitorSetupAdditionalLua), oldValue, value, true);
            }
        }

        public string Image
        {
            get
            {
                return _imageFile;
            }
            set
            {
                if ((_imageFile == null && value != null)
                    || (_imageFile != null && !_imageFile.Equals(value)))
                {
                    string oldValue = _imageFile;
                    _imageFile = value;

                    ImageSource image = ConfigManager.ImageManager.LoadImage(_imageFile);
                    if (image != null)
                    {
                        Width = image.Width / 2;
                        Height = image.Height / 2;
                    }
                    else
                    {
                        Width = 150d;
                        Height = 32d;
                    }
                    OnPropertyChanged("Image", oldValue, value, true);
                    Refresh();
                }
            }
        }

        #region Overrides

        public override bool HitTest(Point location) =>
            // only design time
            ConfigManager.Application.ShowDesignTimeControls;
        public override bool EffectsExclusion
        {
            get => true;
            set { }
        }
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            bool childrenProcessing = false;
            while (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "DCSMonitorSetupAdditionalLua":
                        DCSMonitorSetupAdditionalLua = reader.ReadElementContentAsString();
                        break;
                    case "Image":
                        Image = reader.ReadElementContentAsString();
                        break;
                    case "Children":
                        if (!reader.IsEmptyElement)
                        {
                            reader.ReadStartElement("Children");
                            childrenProcessing = true;
                        } else
                        {
                            return;
                        }
                        break;
                    default:
                        // ignore unsupported settings
                        string elementName = reader.Name;
                        string discard = reader.ReadInnerXml();
                        Logger.Warn(
                            $"Ignored unsupported {GetType().Name} setting '{elementName}' with value '{discard}'");
                        break;
                }
            }
            if (childrenProcessing)
            {
                reader.ReadEndElement();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (DCSMonitorSetupAdditionalLua != "")
            {
                writer.WriteStartElement("Children");
                writer.WriteElementString("DCSMonitorSetupAdditionalLua", DCSMonitorSetupAdditionalLua);
                writer.WriteElementString("Image", Image);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}