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

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Xml;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// This invisible control is used to communicate set-up and communicate with a
    /// ShaderEffect that is used for altering the colour of most visuals & gauges  
    /// </summary>
    [HeliosControl("Helios.Base.Effects.ColorAdjuster", "Color Adjustment Effect", "Special Controls", typeof(ImageDecorationRenderer), HeliosControlFlags.None)]
    public class EffectColorAdjuster : ImageDecorationBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _imageFile = "{helios}/Images/General/ColourAdjuster.xaml";
        private string _shaderName = "{helios}/Resources/ColorAdjust.psc";
        private double _greenFactor = 1.0d, _redFactor = 1.0d, _blueFactor = 1.0d;
        private Effects.ColorAdjustEffect _effect;

        private HeliosValue _redFactorValue, _greenFactorValue, _blueFactorValue;

        public EffectColorAdjuster()
            : base("Color Adjustment Effect")
        {
            DesignTimeOnly = true;
            Image = _imageFile;
            Alignment = ImageAlignment.Stretched;
            Width = 128;
            Height = 128;
            AddEffect();
            _redFactorValue = new HeliosValue(this, new BindingValue(0d), "", "Red Factor for Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _redFactorValue.Execute += new HeliosActionHandler(RedFactor_Execute);
            Actions.Add(_redFactorValue);
            _greenFactorValue = new HeliosValue(this, new BindingValue(0d), "", "Green Factor for Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _greenFactorValue.Execute += new HeliosActionHandler(GreenFactor_Execute);
            Actions.Add(_greenFactorValue);
            _blueFactorValue = new HeliosValue(this, new BindingValue(0d), "", "Blue Factor for Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _blueFactorValue.Execute += new HeliosActionHandler(BlueFactor_Execute);
            Actions.Add(_blueFactorValue);
        }
        private void AddEffect()
        {
            _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;

            if (_effect == null) {
                _effect = new Effects.ColorAdjustEffect
                {
                    GreenFactor = _greenFactor,
                    RedFactor = _redFactor,
                    BlueFactor = _blueFactor
                };
                ConfigManager.ProfileManager.CurrentEffect = _effect;
            }
        }
        public double RedFactor
        {
            get => _redFactor;
            set
            {
                if (!value.Equals(_redFactor))
                {
                    if (_effect != null)
                    {
                        _redFactor = value;
                        _effect.RedFactor = _redFactor;
                    }
                }
            }
        }
        public double GreenFactor
        {
            get => _greenFactor;
            set
            {
                if (!value.Equals(_greenFactor))
                {
                    if (_effect != null)
                    {
                        _greenFactor = value;
                        _effect.GreenFactor = _greenFactor;
                    }
                }
            }
        }
        public double BlueFactor
        {
            get => _blueFactor;
            set
            {
                if (!value.Equals(_blueFactor))
                {
                    if (_effect != null)
                    {
                        _blueFactor = value;
                        _effect.BlueFactor = _blueFactor;
                    }
                }
            }
        }
        public string ShaderName
        {
            get => _shaderName;
            set
            {
                if (!value.Equals(_shaderName))
                {
                    _shaderName = value;
                }
            }
        }

        #region Actions
        void RedFactor_Execute(object action, HeliosActionEventArgs e)
        {
            RedFactor = e.Value.DoubleValue;
        }
        void GreenFactor_Execute(object action, HeliosActionEventArgs e)
        {
            GreenFactor = e.Value.DoubleValue;
        }
        void BlueFactor_Execute(object action, HeliosActionEventArgs e)
        {
            BlueFactor = e.Value.DoubleValue;
        }
        #endregion Actions
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
            if (reader.Name.Equals("Adjustments"))
            {
                reader.ReadStartElement("Adjustments");
                RedFactor = Double.Parse(reader.ReadElementString("RedFactor"));
                GreenFactor = Double.Parse(reader.ReadElementString("GreenFactor"));
                BlueFactor = Double.Parse(reader.ReadElementString("BlueFactor"));
                ShaderName = reader.ReadElementString("ShaderName");
                reader.ReadEndElement();
            }
            AddEffect();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteStartElement("Adjustments");
            writer.WriteElementString("RedFactor", RedFactor.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("GreenFactor", GreenFactor.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("BlueFactor", BlueFactor.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ShaderName", ShaderName);
            writer.WriteEndElement();
        }

        #endregion
    }
}