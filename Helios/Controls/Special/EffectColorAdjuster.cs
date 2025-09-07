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
        private double _brightness, _contrast, _gamma;
        private Effects.ColorAdjustEffect _effect;
        private bool _enabled = true;

        private HeliosValue _redFactorValue, _greenFactorValue, _blueFactorValue;
        private HeliosValue _brightnessValue, _contrastValue, _gammaValue;
        private HeliosValue _enabledValue;

        public EffectColorAdjuster()
            : base("Color Adjustment Effect")
        {
            DesignTimeOnly = true;
            IsUnique = true;   // we only want one of these in the profile.
            Image = _imageFile;
            Alignment = ImageAlignment.Stretched;
            Width = 128;
            Height = 128;
            AddEffect();

            _redFactorValue = new HeliosValue(this, new BindingValue(0d), "", "Red Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _redFactorValue.Execute += new HeliosActionHandler(RedFactor_Execute);
            Actions.Add(_redFactorValue);

            _greenFactorValue = new HeliosValue(this, new BindingValue(0d), "", "Green Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _greenFactorValue.Execute += new HeliosActionHandler(GreenFactor_Execute);
            Actions.Add(_greenFactorValue);

            _blueFactorValue = new HeliosValue(this, new BindingValue(0d), "", "Blue Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _blueFactorValue.Execute += new HeliosActionHandler(BlueFactor_Execute);
            Actions.Add(_blueFactorValue);

            _brightnessValue = new HeliosValue(this, new BindingValue(0d), "", "Brightness Adjustment", "Number to brighten image", "-1 to +1", BindingValueUnits.Numeric);
            _brightnessValue.Execute += new HeliosActionHandler(Brightness_Execute);
            Actions.Add(_brightnessValue);

            _contrastValue = new HeliosValue(this, new BindingValue(0d), "", "Contrast Adjustment", "Number to adjust contrast", "0 to 2.  1 = normal", BindingValueUnits.Numeric);
            _contrastValue.Execute += new HeliosActionHandler(Contrast_Execute);
            Actions.Add(_contrastValue);

            _gammaValue = new HeliosValue(this, new BindingValue(0d), "", "Gamma", "Number to alter the gamma", "1.0 to 2.2", BindingValueUnits.Numeric);
            _gammaValue.Execute += new HeliosActionHandler(Gamma_Execute);
            Actions.Add(_gammaValue);

            _enabledValue = new HeliosValue(this, new BindingValue(0d), "", "Enabled", "Boolean to enable the effect", "true for effect to be applied", BindingValueUnits.Boolean);
            _enabledValue.Execute += new HeliosActionHandler(Enabled_Execute);
            Actions.Add(_enabledValue);
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
        public void DeleteEffect()
        {
            ConfigManager.ProfileManager.CurrentEffect = null;
            _effect = null;
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
        public double Brightness
        {
            get => _brightness;
            set
            {
                if (!value.Equals(_brightness))
                {
                    if (_effect != null)
                    {
                        _brightness = value;
                        _effect.Brightness = _brightness;
                    }
                }
            }
        }
        public double Contrast
        {
            get => _contrast;
            set
            {
                if (!value.Equals(_contrast))
                {
                    if (_effect != null)
                    {
                        _contrast = value;
                        _effect.Contrast = _contrast;
                    }
                }
            }
        }
        public double Gamma
        {
            get => _gamma;
            set
            {
                if (!value.Equals(_gamma))
                {
                    if (_effect != null)
                    {
                        _gamma = value;
                        _effect.Gamma = _gamma;
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
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    _effect.Enabled = _enabled;
                    foreach (HeliosVisual hv in Profile.WalkVisuals())
                    {
                        hv.RenderWithoutImageReload();
                    }
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
        void Brightness_Execute(object action, HeliosActionEventArgs e)
        {
            Brightness = e.Value.DoubleValue;
        }
        void Contrast_Execute(object action, HeliosActionEventArgs e)
        {
            Contrast = e.Value.DoubleValue;
        }
        void Gamma_Execute(object action, HeliosActionEventArgs e)
        {
            Gamma = e.Value.DoubleValue;
        }
        void Enabled_Execute(object action, HeliosActionEventArgs e)
        {
            Enabled = e.Value.BoolValue;
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
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));
            base.ReadXml(reader);
            if (reader.Name.Equals("Effects"))
            {
                reader.ReadStartElement("Effects");
                Enabled = (bool)boolConverter.ConvertFromInvariantString(reader.ReadElementString("EffectsEnabled"));
                RedFactor = Double.Parse(reader.ReadElementString("RedFactor"));
                GreenFactor = Double.Parse(reader.ReadElementString("GreenFactor"));
                BlueFactor = Double.Parse(reader.ReadElementString("BlueFactor"));
                Brightness = Double.Parse(reader.ReadElementString("Brightness"));
                Contrast = Double.Parse(reader.ReadElementString("Contrast"));
                Gamma = Double.Parse(reader.ReadElementString("Gamma"));
                ShaderName = reader.ReadElementString("ShaderName");
                reader.ReadEndElement();
            }
            AddEffect();
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));
            base.WriteXml(writer);
            writer.WriteStartElement("Effects");
            writer.WriteElementString("EffectsEnabled", Enabled.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("RedFactor", RedFactor.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("GreenFactor", GreenFactor.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("BlueFactor", BlueFactor.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("Brightness", Brightness.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("Contrast", Contrast.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("Gamma", Gamma.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("ShaderName", ShaderName);
            writer.WriteEndElement();
        }
        #endregion
    }
}