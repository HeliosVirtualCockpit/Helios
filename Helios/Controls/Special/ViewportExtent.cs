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

using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls.Capabilities;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// viewport extent with an associated editor, so the user can control the name of the viewport being represented
    /// </summary>
    [HeliosControl("Helios.Base.ViewportExtent", "Simulator Viewport", "Special Controls", typeof(ViewportExtentRenderer), HeliosControlFlags.NotRotatable | HeliosControlFlags.None)]
    public class ViewportExtent : ViewportExtentBase
    {
        // no code
    }

    /// <summary>
    /// a generic viewport extent that is recognized as a viewport by various simulator-specific tools
    ///
    /// does not include any knowledge about what viewports are in various simulators, just that it is a named screen rectangle
    /// </summary>
    public class ViewportExtentBase : TextDecoration, IViewportExtent, IDataErrorInfo
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const string DEFAULT_NAME = "Simulator Viewport";
        private const string DEFAULT_TEXT = "Label";
        private const string DEFAULT_VIEWPORT_NAME = "";
        private string _viewportName = DEFAULT_VIEWPORT_NAME;

        /// <summary>
        /// backing field for property RequiresPatches, contains
        /// true if this viewport requires some sort of external patch support, depending on the Simulator used
        /// </summary>
        private bool _requiresPatches;

        public ViewportExtentBase()
            : base(DEFAULT_NAME)
        {
            FillBackground = true;
            BackgroundColor = Color.FromArgb(128, 128, 0, 0);
            FontColor = Color.FromArgb(255, 255, 255, 255);
            Format.VerticalAlignment = TextVerticalAlignment.Center;
        }

        #region Overrides
        public override bool EffectsExclusion
        {
            get => true;
            set { }
        }
        public override bool HitTest(Point location) =>
            // only design time
            ConfigManager.Application.ShowDesignTimeControls;

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            System.ComponentModel.TypeConverter bc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(bool));
            while (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "ViewportName":
                        ViewportName = reader.ReadElementContentAsString();
                        break;
                    case "RequiresPatches":
                        RequiresPatches =
                            (bool) bc.ConvertFromInvariantString(reader.ReadElementString("RequiresPatches"));
                        break;
                    case "Children":
                        // leave this for our caller
                        return;
                    default:
                        // ignore unsupported settings
                        string elementName = reader.Name;
                        string discard = reader.ReadInnerXml();
                        Logger.Warn(
                            $"Ignored unsupported {GetType().Name} setting '{elementName}' with value '{discard}'");
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            System.ComponentModel.TypeConverter bc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(bool));
            if (ViewportName != DEFAULT_VIEWPORT_NAME)
            {
                writer.WriteElementString("ViewportName", ViewportName);
            }

            if (RequiresPatches)
            {
                writer.WriteElementString("RequiresPatches", bc.ConvertToInvariantString(RequiresPatches));
            }
        }

        #endregion

        #region IViewportExtent

        /// <summary>
        /// true if this viewport requires some sort of external patch support, depending on the Simulator used
        /// </summary>
        public bool RequiresPatches
        {
            get => _requiresPatches;
            set
            {
                if (_requiresPatches == value)
                {
                    return;
                }

                bool oldValue = _requiresPatches;
                _requiresPatches = value;
                OnPropertyChanged(nameof(RequiresPatches), oldValue, value, true);
            }
        }

        public string ViewportName
        {
            get => _viewportName;
            set
            {
                if (_viewportName != null && _viewportName == value)
                {
                    return;
                }

                string oldValue = _viewportName;
                _viewportName = value;
                OnPropertyChanged(nameof(ViewportName), oldValue, value, true);

                // now also update the Name of the control if defaulted
                if ((Name == DEFAULT_NAME) || (Name == oldValue))
                {
                    // XXX this fails to update the UI, put a diag on it and find out why
                    Name = value;
                }

                if ((Text == DEFAULT_TEXT) || (Text == oldValue))
                {
                    Text = value;
                }
            }
        }

        string IDataErrorInfo.Error => throw new System.NotImplementedException();

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                return Validate(columnName);
            }

        }
        private string Validate(string propertyName)
        {
            // Return error message if there is error on else return empty or null string
            string validationMessage = string.Empty;
            switch (propertyName)
            {
                case "ViewportName": // property name
                    if(!Regex.Match(this.ViewportName, "^[a-zA-Z][_a-zA-Z0-9]{0,64}$", RegexOptions.CultureInvariant | RegexOptions.Compiled).Success)
                    {
                        validationMessage = "Invalid Character in Viewport Name";
                    }
                    break;
            }

            return validationMessage;
        }
        #endregion
    }
}