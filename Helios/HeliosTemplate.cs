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

namespace GadrocsWorkshop.Helios
{
    using System.IO;
    using System.Xml;

    public class HeliosTemplate : NotificationObject
    {
        private bool _user = false;
        private string _templateControl = "";
        private string _name = "";
        private string _category = "User Templates";
        private string _typeIdentifier = "";

        public HeliosTemplate(bool userTemplate)
        {
            _user = userTemplate;
        }

        public HeliosTemplate(HeliosVisual control)
        {
            _user = true;
            _typeIdentifier = control.TypeIdentifier;
            _name = control.Name;
            StringWriter templateWriter = new StringWriter();
            XmlWriter xmlWriter = new XmlTextWriter(templateWriter);
            xmlWriter.WriteStartElement("TemplateValues");
            control.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
            templateWriter.Close();
            _templateControl = templateWriter.ToString();
        }

        #region Properties

        public bool IsUserTemplate
        {
            get
            {
                return _user;
            }
            set
            {
                if (!_user.Equals(value))
                {
                    bool oldValue = _user;
                    _user = value;
                    OnPropertyChanged("IsUserTemplate", oldValue, value, true);
                }
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if ((_name == null && value != null)
                    || (_name != null && !_name.Equals(value)))
                {
                    string oldValue = _name;
                    _name = value;
                    OnPropertyChanged("Name", oldValue, value, true);
                }
            }
        }

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                if ((_category == null && value != null)
                    || (_category != null && !_category.Equals(value)))
                {
                    string oldValue = _category;
                    _category = value;
                    OnPropertyChanged("Category", oldValue, value, true);
                }
            }
        }

        #endregion

        public HeliosVisual CreateInstance()
        {
            HeliosVisual control = ConfigManager.ModuleManager.CreateControl(_typeIdentifier);

            if (control != null)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                settings.CloseInput = true;

                StringReader templateReader = new StringReader(_templateControl);
                XmlReader xmlReader = XmlReader.Create(templateReader, settings);
                xmlReader.ReadStartElement("TemplateValues");
                control.ReadXml(xmlReader);
                if (xmlReader.Name.Equals("Children"))
                {
                    HeliosSerializer deserializer = new HeliosSerializer();
                    foreach (string progress in deserializer.DeserializeControls(control.Children, xmlReader))
                    {
                        //yield return progress;
                    }
                } else
                {
                    xmlReader.ReadEndElement();
                }
                xmlReader.Close();
                templateReader.Close();

                control.Name = Name;
            }

            return control;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("ControlTemplate");
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Category", Category);
            writer.WriteElementString("TypeIdentifier", _typeIdentifier);

            writer.WriteStartElement("Template");
            writer.WriteRaw(_templateControl);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("ControlTemplate");
            Name = reader.ReadElementString("Name");
            Category = reader.ReadElementString("Category");
            _typeIdentifier = reader.ReadElementString("TypeIdentifier");

            _templateControl = reader.ReadInnerXml();

            reader.ReadEndElement();
        }
    }
}
