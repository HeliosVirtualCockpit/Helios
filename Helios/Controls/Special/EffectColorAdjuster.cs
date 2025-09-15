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
using GadrocsWorkshop.Helios.Effects;
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
    [HeliosControl("Helios.Base.Effects.ColorAdjuster", "Color Adjustment Effect", "Special Controls", typeof(EffectColorAdjusterRenderer), HeliosControlFlags.None)]
    public class EffectColorAdjuster : ImageDecorationBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _imageFile = "{helios}/Images/General/ColourAdjuster.xaml";
        private string _shaderName = "ColorAdjust.psc";
        private double _greenAdjust = 1.0d, _redAdjust = 1.0d, _blueAdjust = 1.0d;
        private double _brightness = 0d, _contrast = 1.0d, _gamma = 1.0d;
        private double _midtoneBalance = 0.5d, _highlightStrength = 1.0d, _shadowStrength = 1.0d;
        private Effects.ColorAdjustEffect _effect;
        private bool _enabled = true;
        private bool _advancedFeatures = false;
        private static readonly string _actionState = " (NOT IMPLEMENTED)";
        private static readonly string _shaderLocation = "pack://application:,,,/Helios;component/Resources/";

        private HeliosValue _redAdjustValue, _greenAdjustValue, _blueAdjustValue;
        private HeliosValue _brightnessValue, _contrastValue, _gammaValue;
        private HeliosValue _midtoneBalanceValue, _highlightStrengthValue, _shadowStrengthValue;
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

            _redAdjustValue = new HeliosValue(this, new BindingValue(0d), "", "Red Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _redAdjustValue.Execute += new HeliosActionHandler(RedAdjust_Execute);
            Actions.Add(_redAdjustValue);

            _greenAdjustValue = new HeliosValue(this, new BindingValue(0d), "", "Green Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _greenAdjustValue.Execute += new HeliosActionHandler(GreenAdjust_Execute);
            Actions.Add(_greenAdjustValue);

            _blueAdjustValue = new HeliosValue(this, new BindingValue(0d), "", "Blue Color Adjustment", "Number to be used as multiplier", "0 to 2", BindingValueUnits.Numeric);
            _blueAdjustValue.Execute += new HeliosActionHandler(BlueAdjust_Execute);
            Actions.Add(_blueAdjustValue);

            _brightnessValue = new HeliosValue(this, new BindingValue(0d), "", "Brightness Adjustment", "Number to brighten image" + _actionState, "-1 to +1", BindingValueUnits.Numeric);
            _brightnessValue.Execute += new HeliosActionHandler(Brightness_Execute);
            Actions.Add(_brightnessValue);

            _contrastValue = new HeliosValue(this, new BindingValue(0d), "", "Contrast Adjustment", "Number to adjust contrast" + _actionState, "0 to 2.  1 = normal", BindingValueUnits.Numeric);
            _contrastValue.Execute += new HeliosActionHandler(Contrast_Execute);
            Actions.Add(_contrastValue);

            _gammaValue = new HeliosValue(this, new BindingValue(0d), "", "Gamma", "Number to alter the gamma" + _actionState, "1.0 to 2.2", BindingValueUnits.Numeric);
            _gammaValue.Execute += new HeliosActionHandler(Gamma_Execute);
            Actions.Add(_gammaValue);

            _highlightStrengthValue = new HeliosValue(this, new BindingValue(0d), "", "Highlight Strength", "How much to boost the highlights" + _actionState, "1.0 to 2.0", BindingValueUnits.Numeric);
            _highlightStrengthValue.Execute += new HeliosActionHandler(HighlightStrength_Execute);
            Actions.Add(_highlightStrengthValue);

            _midtoneBalanceValue = new HeliosValue(this, new BindingValue(0d), "", "Midtone Balance", "Midpoint position of the adjustment curve" + _actionState, "0.0 to 1.0", BindingValueUnits.Numeric);
            _midtoneBalanceValue.Execute += new HeliosActionHandler(MidtoneBalance_Execute);
            Actions.Add(_midtoneBalanceValue);

            _shadowStrengthValue = new HeliosValue(this, new BindingValue(0d), "", "Shadow Strength", "How much to boost the shadows" + _actionState, "1.0 to 2.0", BindingValueUnits.Numeric);
            _shadowStrengthValue.Execute += new HeliosActionHandler(ShadowStrength_Execute);
            Actions.Add(_shadowStrengthValue);

            _enabledValue = new HeliosValue(this, new BindingValue(0d), "", "Enabled", "Boolean to enable the effect" + _actionState, "true for effect to be applied", BindingValueUnits.Boolean);
            _enabledValue.Execute += new HeliosActionHandler(Enabled_Execute);
            Actions.Add(_enabledValue);
        }
        private void AddEffect()
        {
//            AdvancedFeatures = true;
            _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;
            if (_effect == null) {
                _effect = new Effects.ColorAdjustEffect
                {
                    GreenAdjust = _greenAdjust,
                    RedAdjust = _redAdjust,
                    BlueAdjust = _blueAdjust,
                    Brightness = _brightness,
                    Contrast = _contrast,
                    Gamma = _gamma,
                    ShadowStrength = _shadowStrength,
                    MidtoneBalance = _midtoneBalance,
                    HighlightStrength = _highlightStrength,
                    Enabled = _enabled,
                    EnableCurve = true,
                    ShaderUri = $"{_shaderLocation}{_shaderName}"
                };
                ConfigManager.ProfileManager.CurrentEffect = _effect;
            }
        }
        public void DeleteEffect()
        {
            ConfigManager.ProfileManager.CurrentEffect = null;
            _effect = null;
        }
        private bool CheckEffect()
        {
            if(_effect == null)
            {
                AddEffect();
                foreach (HeliosVisual hv in Profile.WalkVisuals())
                {
                    hv.RenderWithoutImageReload();
                }
                return (_effect != null);
            } else
            {
                if(ConfigManager.ProfileManager.CurrentEffect == null)
                {
                    ConfigManager.ProfileManager.CurrentEffect = _effect;
                }
            }
                return true;
        }
        #region Properties
        public double RedAdjust
        {
            get => _redAdjust;
            set
            {
                if (!value.Equals(_redAdjust))
                {
                    if (CheckEffect())
                    {
                        _redAdjust = value;
                        _effect.RedAdjust = _redAdjust;
                    }
                }
            }
        }
        public double GreenAdjust
        {
            get => _greenAdjust;
            set
            {
                if (!value.Equals(_greenAdjust))
                {
                    if (CheckEffect())
                    {
                        _greenAdjust = value;
                        _effect.GreenAdjust = _greenAdjust;
                    }
                }
            }
        }
        public double BlueAdjust
        {
            get => _blueAdjust;
            set
            {
                if (!value.Equals(_blueAdjust))
                {
                    if (CheckEffect())
                    {
                        _blueAdjust = value;
                        _effect.BlueAdjust = _blueAdjust;
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
                    if (CheckEffect())
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
                    if (CheckEffect())
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
                    if (CheckEffect())
                    {
                        _gamma = value;
                        _effect.Gamma = _gamma;
                    }
                }
            }
        }
        public double HighlightStrength
        {
            get => _highlightStrength;
            set
            {
                if (!value.Equals(_highlightStrength))
                {
                    if (CheckEffect())
                    {
                        _highlightStrength = value;
                        _effect.HighlightStrength = _highlightStrength;
                    }
                }
            }
        }
        public double MidtoneBalance
        {
            get => _midtoneBalance;
            set
            {
                if (!value.Equals(_midtoneBalance))
                {
                    if (CheckEffect())
                    {
                        _midtoneBalance = value;
                        _effect.MidtoneBalance = _midtoneBalance;
                    }
                }
            }
        }
        public double ShadowStrength
        {
            get => _shadowStrength;
            set
            {
                if (!value.Equals(_shadowStrength))
                {
                    if (CheckEffect())
                    {
                        _shadowStrength = value;
                        _effect.ShadowStrength = _shadowStrength;
                    }
                }
            }
        }
        public bool AdvancedFeatures
        {
            get => _advancedFeatures;
            set
            {
                if (!value.Equals(_advancedFeatures))
                {
                    if (CheckEffect())
                    {
                        _advancedFeatures = value;
                        _effect.EnableCurve = _advancedFeatures;
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
                if (CheckEffect())
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
        }
#endregion Properties
        #region Actions
        void RedAdjust_Execute(object action, HeliosActionEventArgs e)
        {
            RedAdjust = e.Value.DoubleValue;
        }
        void GreenAdjust_Execute(object action, HeliosActionEventArgs e)
        {
            GreenAdjust = e.Value.DoubleValue;
        }
        void BlueAdjust_Execute(object action, HeliosActionEventArgs e)
        {
            BlueAdjust = e.Value.DoubleValue;
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
        void HighlightStrength_Execute(object action, HeliosActionEventArgs e)
        {
            HighlightStrength = e.Value.DoubleValue;
        }
        void MidtoneBalance_Execute(object action, HeliosActionEventArgs e)
        {
            MidtoneBalance = e.Value.DoubleValue;
        }
        void ShadowStrength_Execute(object action, HeliosActionEventArgs e)
        {
           ShadowStrength = e.Value.DoubleValue;
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
                RedAdjust = Double.Parse(reader.ReadElementString("RedAdjust"));
                GreenAdjust = Double.Parse(reader.ReadElementString("GreenAdjust"));
                BlueAdjust = Double.Parse(reader.ReadElementString("BlueAdjust"));
                Brightness = Double.Parse(reader.ReadElementString("Brightness"));
                Contrast = Double.Parse(reader.ReadElementString("Contrast"));
                Gamma = Double.Parse(reader.ReadElementString("Gamma"));
                HighlightStrength = Double.Parse(reader.ReadElementString("HighlightStrength"));
                MidtoneBalance = Double.Parse(reader.ReadElementString("MidtoneBalance"));
                ShadowStrength = Double.Parse(reader.ReadElementString("ShadowStrength"));
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
            writer.WriteElementString("RedAdjust", RedAdjust.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("GreenAdjust", GreenAdjust.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("BlueAdjust", BlueAdjust.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("Brightness", Brightness.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("Contrast", Contrast.ToString("N", CultureInfo.InvariantCulture));
            writer.WriteElementString("Gamma", Gamma.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("HighlightStrength", HighlightStrength.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("MidtoneBalance", MidtoneBalance.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("ShadowStrength", ShadowStrength.ToString("N2", CultureInfo.InvariantCulture));
            writer.WriteElementString("ShaderName", ShaderName);
            writer.WriteEndElement();
        }
        #endregion
    }
}