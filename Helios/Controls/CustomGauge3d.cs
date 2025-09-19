//  Copyright 2025 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Controls

{
    using GadrocsWorkshop.Helios.ComponentModel;
     using GadrocsWorkshop.Helios.Util;
    using GadrocsWorkshop.Helios.Gauges;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using GadrocsWorkshop.Helios.Controls.Capabilities;

    public abstract class CustomGauge3d : BaseGauge
    {

        private Size _size;
        private double _fov;
        private double _baseX, _baseY, _baseZ;

        private HeliosValue _rotXValue, _rotYValue, _rotZValue, _lightBrightnessValue;

        private bool _suppressScale = false;

        public CustomGauge3d(string name, Size size)
            : base(name, new Size(300, 300))
        {
            _size = base.NativeSize;

            _rotXValue = new HeliosValue(this, new BindingValue(0d), "", "X Rotation", "Rotation in the X-Axis.", "(0 - 360)", BindingValueUnits.Degrees);
            _rotXValue.Execute += new HeliosActionHandler(X_Execute);
            Actions.Add(_rotXValue);
            _rotYValue = new HeliosValue(this, new BindingValue(0d), "", "Y Rotation", "Rotation in the Y-AYis.", "(0 - 360)", BindingValueUnits.Degrees);
            _rotYValue.Execute += new HeliosActionHandler(Y_Execute);
            Actions.Add(_rotYValue);
            _rotZValue = new HeliosValue(this, new BindingValue(0d), "", "Z Rotation", "Rotation in the Z-Axis.", "(0 - 360)", BindingValueUnits.Degrees);
            _rotZValue.Execute += new HeliosActionHandler(Z_Execute);
            Actions.Add(_rotZValue);
            _lightBrightnessValue = new HeliosValue(this, new BindingValue(false), "", "Brightness of default lighting source", "Number", "0 to 1", BindingValueUnits.Numeric);
            _lightBrightnessValue.Execute += new HeliosActionHandler(LightingBrightness_Execute);
            Actions.Add(_lightBrightnessValue);
        }

        #region Properties

        public double BasePitch
        {
            get => _baseX;
            set
            {
                double oldValue = _baseX;
                if (value != _baseX)
                {
                    _baseX = value;
                    if (gauge != null)
                    {
                        gauge.BasePitch = _baseX;
                    }
                    OnPropertyChanged("BasePitch", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double BaseYaw
        {
            get => _baseY;
            set
            {
                double oldValue = _baseY;

                if (value != _baseY)
                {
                    _baseY = value;
                    if (gauge != null)
                    {
                        gauge.BaseYaw = _baseY;
                    }
                    OnPropertyChanged("BaseYaw", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double BaseRoll
        {
            get => _baseZ;
            set
            {
                double oldValue = _baseZ;
                if (value != _baseZ)
                {
                    _baseZ = value;
                    if(gauge != null)
                    {
                        gauge.BaseRoll = _baseZ;
                    }
                    OnPropertyChanged("BaseRoll", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double FieldOfView
        {
            get => _fov;
            set
            {
                if (value != _fov)
                {
                    double oldValue = _fov;
                    _fov = value;
                    if(gauge != null)
                    {
                        gauge.FieldOfView = _fov;
                    }
                    OnPropertyChanged("FieldOfView", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public Color LightingColor
        {
            get => gauge.LightingColor;
            set
            {
                if (value != LightingColor && gauge != null)
                {
                    Color oldValue = gauge.LightingColor;
                    gauge.LightingColor = value;
                    OnPropertyChanged("LightingColor", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double LightingBrightness
        {
            get => gauge.LightingBrightness;
            set
            {
                if (value != LightingBrightness && gauge != null)
                {
                    double oldValue = gauge.LightingBrightness;
                    gauge.LightingBrightness = value;
                    OnPropertyChanged("LightingBrightness", oldValue, value, true);
                    Refresh();
                }
            }
        }

        public double LightingX
        {
            get => gauge.LightingX;
            set
            {

                if (gauge != null && value != gauge.LightingX)
                {
                    double oldValue = gauge.LightingX;
                    gauge.LightingX = value;
                    OnPropertyChanged("LightingX", oldValue, value, true);
                    Refresh();

                }
            }
        }
        public double LightingY
        {
            get => gauge.LightingY;
            set
            {
                if (gauge != null && value != gauge.LightingY)
                {
                    double oldValue = gauge.LightingY;
                    gauge.LightingY = value;
                    OnPropertyChanged("LightingY", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double LightingZ
        {
            get => gauge.LightingZ;
            set
            {
                if (gauge != null && value != gauge.LightingZ)
                {
                    double oldValue = gauge.LightingZ;
                    gauge.LightingZ = value;
                    OnPropertyChanged("LightingZ", oldValue, value, true);
                    Refresh();
                }
            }
        }
        protected abstract IGauge3d gauge { get;}

        #endregion Properties
        void X_Execute(object action, HeliosActionEventArgs e)
        {
            gauge.Pitch = e.Value.DoubleValue;
        }
        void Y_Execute(object action, HeliosActionEventArgs e)
        {
            gauge.Yaw = e.Value.DoubleValue;
        }
        void Z_Execute(object action, HeliosActionEventArgs e)
        {
            gauge.Roll = e.Value.DoubleValue;
        }
        void LightingBrightness_Execute(object action, HeliosActionEventArgs e)
        {
            gauge.LightingBrightness = e.Value.DoubleValue;
        }

        #region actions 
        public override void Reset()
        {
            base.Reset();
            _rotXValue.SetValue(new BindingValue(0d), true);
            _rotYValue.SetValue(new BindingValue(0d), true);
            _rotZValue.SetValue(new BindingValue(0d), true);
        }
        public override void ScaleChildren(double scaleX, double scaleY)
        {
            if (!_suppressScale)
            {
                if (gauge != null) { 
                    gauge.ScaleChildren(scaleX, scaleY);
                }
                _suppressScale = false;
            }
            base.ScaleChildren(scaleX, scaleY);
        }
        protected override void PostUpdateRectangle(Rect previous, Rect current)
        {
            _suppressScale = false;
            if (!previous.Equals(new Rect(0, 0, 0, 0)) && !(previous.Width == current.Width && previous.Height == current.Height))
            {
                if (gauge != null)
                {
                    gauge.ScaleChildren(current.Width / previous.Width, current.Height / previous.Height);
                }
                _suppressScale = true;
            }
        }
        #endregion
        #region de/serialize
        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            base.WriteXml(writer);
            writer.WriteStartElement("Properties3D");
            writer.WriteElementString("BasePitch", _baseX.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("BaseRoll", _baseY.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("BaseYaw", _baseZ.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("FieldOfView", _fov.ToString(CultureInfo.InvariantCulture));
            writer.WriteStartElement("Lighting");
            writer.WriteElementString("X", LightingX.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Y", LightingY.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Z", LightingZ.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Color", colorConverter.ConvertToInvariantString(LightingColor));
            writer.WriteElementString("ColorBrightness", LightingBrightness.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            base.ReadXml(reader);
            if (reader.Name.Equals("Properties3D"))
            {
                reader.ReadStartElement("Properties3D");
                BasePitch = double.Parse(reader.ReadElementString("BasePitch"), CultureInfo.InvariantCulture);
                BaseRoll = double.Parse(reader.ReadElementString("BaseRoll"), CultureInfo.InvariantCulture);
                BaseYaw = double.Parse(reader.ReadElementString("BaseYaw"), CultureInfo.InvariantCulture);
                FieldOfView = double.Parse(reader.ReadElementString("FieldOfView"), CultureInfo.InvariantCulture);
                if (reader.Name.Equals("Lighting"))
                {
                    reader.ReadStartElement("Lighting");
                    LightingX = double.Parse(reader.ReadElementString("X"), CultureInfo.InvariantCulture);
                    LightingY = double.Parse(reader.ReadElementString("Y"), CultureInfo.InvariantCulture);
                    LightingZ = double.Parse(reader.ReadElementString("Z"), CultureInfo.InvariantCulture);
                    LightingColor = (Color)colorConverter.ConvertFromInvariantString(reader.ReadElementString("Color"));
                    LightingBrightness = double.Parse(reader.ReadElementString("ColorBrightness"), CultureInfo.InvariantCulture);
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
        }
        #endregion de/serialize 

    }
    // helper for intellisense in XAML
    public class DesignTimeCustomGauge3d : DesignTimeControl<CustomGauge3d> { }
}
