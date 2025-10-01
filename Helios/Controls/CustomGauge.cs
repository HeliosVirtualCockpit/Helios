//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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

using System.Globalization;
using System.Windows;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.Controls
{
    [HeliosControl("Helios.Base.CustomGauge", "Custom Gauge", "Custom Controls", typeof(CustomGaugeRenderer))]
    public class CustomGauge : CustomNeedle, IRefreshableImage
    {
        private double _needleScale = 0.5d;
        private double _needleHeight = 1d;
        private double _needlePivotX = 0.5d;
        private double _needlePivotY = 0.5d;
        private double _needlePosX = 0.475d;
        private double _needlePosY = 0.05d;
        private double _initialValue = 0d;
        private double _stepValue = 0.1d;
        private double _minValue = 0d;
        private double _maxValue = 1d;
        private string _bgplateImage = "{Helios}/Gauges/KA-50/RadarAltimeter/radar_alt_faceplate.xaml";
        private double _initialRotation;
        private double _rotationTravel = 360d;

        private readonly HeliosValue _heliosValue;

        public CustomGauge()
            : base("CustomGauge", new Size(100, 100))
        {
            KnobImage = "{Helios}/Gauges/KA-50/RadarAltimeter/radar_alt_needle.xaml";
            _heliosValue = new HeliosValue(this, new BindingValue(0d), "", "value", "Current value of the CustomGauge.",
                "", BindingValueUnits.Numeric);
            _heliosValue.Execute += SetValue_Execute;
            Values.Add(_heliosValue);
            Actions.Add(_heliosValue);
            //Triggers.Add(_potValue);
        }

        public override bool HitTest(Point location) =>
            // our extent can be greater than our background rectangle due to rotated needle images
            new Rect(0, 0, Width, Height).Contains(location);

        #region Properties

        public string BgPlateImage
        {
            get => _bgplateImage;
            set
            {
                if ((_bgplateImage == null && value != null)
                    || (_bgplateImage != null && !_bgplateImage.Equals(value)))
                {
                    string oldValue = _bgplateImage;
                    _bgplateImage = value;
                    OnPropertyChanged("BGPlateImage", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public double InitialValue
        {
            get => _initialValue;
            set
            {
                if (!_initialValue.Equals(value))
                {
                    double oldValue = _initialValue;
                    _initialValue = value;
                    OnPropertyChanged("InitialValue", oldValue, value, true);
                }
            }
        }

        public double NeedleScale
        {
            get => _needleScale;
            set
            {
                if (!_needleScale.Equals(value))
                {
                    double oldValue = _needleScale;
                    _needleScale = value;
                    OnPropertyChanged("Needle_Scale", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double NeedleHeight
        {
            get => _needleHeight;
            set
            {
                if (!_needleHeight.Equals(value))
                {
                    double oldValue = _needleHeight;
                    _needleHeight = value;
                    OnPropertyChanged("Needle_Height", oldValue, value, true);
                    Refresh();
                }
            }
        }


        public double NeedlePivotX
        {
            get => _needlePivotX;
            set
            {
                if (!_needlePivotX.Equals(value))
                {
                    double oldValue = _needlePivotX;
                    _needlePivotX = value;
                    OnPropertyChanged("Needle_PivotX", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public double NeedlePivotY
        {
            get => _needlePivotY;
            set
            {
                if (!_needlePivotY.Equals(value))
                {
                    double oldValue = _needlePivotY;
                    _needlePivotY = value;
                    OnPropertyChanged("Needle_PivotY", oldValue, value, true);
                    Refresh();
                }
            }
        }


        public double NeedlePosX
        {
            get => _needlePosX;
            set
            {
                if (!_needlePosX.Equals(value))
                {
                    double oldValue = _needlePosX;
                    _needlePosX = value;
                    OnPropertyChanged("Needle_PosX", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public double NeedlePosY
        {
            get => _needlePosY;
            set
            {
                if (!_needlePosY.Equals(value))
                {
                    double oldValue = _needlePosY;
                    _needlePosY = value;
                    OnPropertyChanged("Needle_PosY", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public double MinValue
        {
            get => _minValue;
            set
            {
                if (!_minValue.Equals(value))
                {
                    double oldValue = _minValue;
                    _minValue = value;
                    OnPropertyChanged("MinValue", oldValue, value, true);
                    SetRotation();
                }
            }
        }

        public double MaxValue
        {
            get => _maxValue;
            set
            {
                if (!_maxValue.Equals(value))
                {
                    double oldValue = _maxValue;
                    _maxValue = value;
                    OnPropertyChanged("MaxValue", oldValue, value, true);
                    SetRotation();
                }
            }
        }

        public double StepValue
        {
            get => _stepValue;
            set
            {
                if (!_stepValue.Equals(value))
                {
                    double oldValue = _stepValue;
                    _stepValue = value;
                    OnPropertyChanged("StepValue", oldValue, value, true);
                }
            }
        }

        public double Value
        {
            get => _heliosValue.Value.DoubleValue;
            set
            {
                if (!_heliosValue.Value.DoubleValue.Equals(value))
                {
                    double oldValue = _heliosValue.Value.DoubleValue;
                    _heliosValue.SetValue(new BindingValue(value), BypassTriggers);
                    OnPropertyChanged("Value", oldValue, value, false);
                    SetRotation();
                }
            }
        }

        public double InitialRotation
        {
            get => _initialRotation;
            set
            {
                if (!_initialRotation.Equals(value))
                {
                    double oldValue = _initialRotation;
                    _initialRotation = value;
                    OnPropertyChanged("InitialRotation", oldValue, value, true);
                    SetRotation();
                }
            }
        }

        public double RotationTravel
        {
            get => _rotationTravel;
            set
            {
                if (!_rotationTravel.Equals(value))
                {
                    double oldValue = _rotationTravel;
                    _rotationTravel = value;
                    OnPropertyChanged("RotationTravel", oldValue, value, true);
                    SetRotation();
                }
            }
        }

        #endregion

        #region Actions

        private void SetValue_Execute(object action, HeliosActionEventArgs e)
        {
            try
            {
                _heliosValue.SetValue(e.Value, e.BypassCascadingTriggers);
                SetRotation();
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

        #endregion

        private void SetRotation()
        {
            _knobRotation = (InitialRotation + (((Value - MinValue) / (MaxValue - MinValue)) * RotationTravel)) % 360d;
            OnDisplayUpdate();
        }

        public override void Reset()
        {
            base.Reset();
            _heliosValue.SetValue(new BindingValue(InitialValue), true);
            SetRotation();
        }
        public override bool ConditionalImageRefresh(string imageName)
        {
            if ((KnobImage ?? "").ToLower().Replace("/", @"\") == imageName || BgPlateImage.ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                Refresh();
            }
            return ImageRefresh;
        }
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("BGPlateImage", BgPlateImage);
            writer.WriteElementString("KnobImage", KnobImage);
            writer.WriteElementString("Needle_Scale", NeedleScale.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Needle_PosX", NeedlePosX.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Needle_PosY", NeedlePosY.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Needle_PivotX", NeedlePivotX.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Needle_PivotY", NeedlePivotY.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("InitialValue", InitialValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("StepValue", StepValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("MaxValue", MaxValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("MinValue", MinValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("InitialRotation", InitialRotation.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("RotationTravel", RotationTravel.ToString(CultureInfo.InvariantCulture));
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            BgPlateImage = reader.ReadElementString("BGPlateImage");
            KnobImage = reader.ReadElementString("KnobImage");
            NeedleScale = double.Parse(reader.ReadElementString("Needle_Scale"), CultureInfo.InvariantCulture);
            NeedlePosX = double.Parse(reader.ReadElementString("Needle_PosX"), CultureInfo.InvariantCulture);
            NeedlePosY = double.Parse(reader.ReadElementString("Needle_PosY"), CultureInfo.InvariantCulture);
            NeedlePivotX = double.Parse(reader.ReadElementString("Needle_PivotX"), CultureInfo.InvariantCulture);
            NeedlePivotY = double.Parse(reader.ReadElementString("Needle_PivotY"), CultureInfo.InvariantCulture);
            InitialValue = double.Parse(reader.ReadElementString("InitialValue"), CultureInfo.InvariantCulture);
            StepValue = double.Parse(reader.ReadElementString("StepValue"), CultureInfo.InvariantCulture);
            MaxValue = double.Parse(reader.ReadElementString("MaxValue"), CultureInfo.InvariantCulture);
            MinValue = double.Parse(reader.ReadElementString("MinValue"), CultureInfo.InvariantCulture);
            InitialRotation = double.Parse(reader.ReadElementString("InitialRotation"), CultureInfo.InvariantCulture);
            RotationTravel = double.Parse(reader.ReadElementString("RotationTravel"), CultureInfo.InvariantCulture);

            Reset();
        }
    }

    // helper for intellisense in XAML
    public class DesignTimeCustomGauge: DesignTimeControl<CustomGauge> {}
}