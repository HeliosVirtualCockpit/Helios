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
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls;
using GadrocsWorkshop.Helios.Controls.Capabilities;

namespace GadrocsWorkshop.Helios.Gauges
{
    public abstract class CompositeVisualWithBackgroundImage
        : CompositeVisual, IConfigurableBackgroundImage
    {
        /// <summary>
        /// backing field for property BackgroundImage, contains
        /// the Helios image path for the background/bezel image shown below all child controls
        /// </summary>
        private string _backgroundImage;

        /// <summary>
        /// to be initialized by descendant classes
        /// </summary>
        public abstract string DefaultBackgroundImage { get; }

        /// <summary>
        /// true if the current background image is not the same as the default provided for the class
        /// </summary>
        protected bool BackgroundImageIsCustomized => BackgroundImage != DefaultBackgroundImage;

        /// <summary>
        /// the Helios image path for the background/bezel image shown below all child controls
        /// </summary>
        public string BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                if (_backgroundImage != null && _backgroundImage == value) return;
                string oldValue = _backgroundImage;
                _backgroundImage = value;
                OnPropertyChanged("BackgroundImage", oldValue, value, true);
                OnBackgroundImageChange();
                Refresh();
            }
        }

        /// <summary>
        /// after UI configuration change or deserialization, for example used to
        /// adjust any additional background images to hide them
        /// if a custom background was configured
        /// </summary>
        protected virtual void OnBackgroundImageChange()
        {
            // no code in base
        }

        protected CompositeVisualWithBackgroundImage(string name, Size nativeSize) : base(name, nativeSize)
        {
            // use default image unless changed during ReadXML
            // ReSharper disable once VirtualMemberCallInConstructor we don't have a deferred construction call from CreateControl
            _backgroundImage = DefaultBackgroundImage;
        }

        /// <summary>
        /// must be called by any overrides of this function
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (BackgroundImageIsCustomized)
            {
                writer.WriteElementString("BackgroundImage", BackgroundImage);
            }
        }

        /// <summary>
        /// must be called by any overrides of this function
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            if (reader.Name != "BackgroundImage")
            {
                return;
            }

            _backgroundImage = reader.ReadElementString("BackgroundImage");
            OnBackgroundImageChange();
        }
        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("ImageAssetLocation"))
            {
                UpdateImageAssets(args);
            }
            base.OnPropertyChanged(args);
        }
        /// <summary>
        /// Used to update image names in children of this control
        /// </summary>
        /// <param name="PropertyChangedArgs"></param>
        protected void UpdateImageAssets(PropertyNotificationEventArgs args)
        {
            foreach (IConfigurableImageLocation child in Children)
            {
                child.ReplaceImageNames(args.OldValue.ToString(), args.NewValue.ToString());
            }
        }

        protected virtual string UpdateImageName(string imageName, string oldValue, string newValue)
            {
                if (string.IsNullOrEmpty(imageName))
                {
                    return imageName;
                }
                else
                {
                    if (string.IsNullOrEmpty(oldValue))
                    {
                        return newValue + imageName;
                    }
                    else
                    {
                        return imageName.Replace(oldValue, newValue);
                    }
                }
            }
    }
}