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

    [HeliosControl("Helios.Base.CustomGaugeBall", "Custom Gauge Ball", "Custom Controls", typeof(GaugeRenderer), HeliosControlFlags.None)]

    public class CustomGaugeBall : BaseGauge
    {

        private Size _size;
        private double _fov;
        private double _baseX, _baseY, _baseZ;
        private GaugeBall _ball;
        private string _imageName;

        private HeliosValue _rotXValue, _rotYValue, _rotZValue, _altLightValue;

        public CustomGaugeBall()
            : base("Custom Gauge Ball", new Size(300, 300))
        {
            _size = base.NativeSize;
            _imageName = "{F-16C}/Gauges/ADI/Viper-ADI-Ball.xaml";
            _baseX = 0d;
            _baseY = 270d;
            _baseZ = 180d;
            _fov = 35d;
            _ball = new GaugeBall(_imageName, new Point(0d, 0d), _size, _baseX, _baseY, _baseZ, _fov);
            Components.Add(_ball);
            LightingColor = Colors.White;
            LightingColorAlt = Colors.Green;
            _rotXValue = new HeliosValue(this, new BindingValue(0d), "", "X Rotation", "Rotation in the X-Axis.", "(0 - 360)", BindingValueUnits.Degrees);
            _rotXValue.Execute += new HeliosActionHandler(X_Execute);
            Actions.Add(_rotXValue);
            _rotYValue = new HeliosValue(this, new BindingValue(0d), "", "Y Rotation", "Rotation in the Y-AYis.", "(0 - 360)", BindingValueUnits.Degrees);
            _rotYValue.Execute += new HeliosActionHandler(Y_Execute);
            Actions.Add(_rotYValue);
            _rotZValue = new HeliosValue(this, new BindingValue(0d), "", "Z Rotation", "Rotation in the Z-Axis.", "(0 - 360)", BindingValueUnits.Degrees);
            _rotZValue.Execute += new HeliosActionHandler(Z_Execute);
            Actions.Add(_rotZValue);
            _altLightValue = new HeliosValue(this, new BindingValue(false), "", "Enable Alternate Lighting Source", "Boolean", "true if Alt Lighting is used", BindingValueUnits.Boolean);
            _altLightValue.Execute += new HeliosActionHandler(AltLightingUsed_Execute);
            Actions.Add(_altLightValue);


        }

        #region Properties

        public string Image
        {
            get => _imageName;
            set
            {
                if (value != _imageName)
                {
                    _imageName = value;
                    _ball.Image = _imageName;
                }
            }
        }
        public double BasePitch
        {
            get => _baseX;
            set
            {
                if (value != _baseX)
                {
                    _baseX = value;
                    _ball.BasePitch = _baseX;
                }
            }
        }
        public double BaseYaw
        {
            get => _baseY;
            set
            {
                if (value != _baseY)
                {
                    _baseY = value;
                    _ball.BaseYaw = _baseY;
                }
            }
        }
        public double BaseRoll
        {
            get => _baseZ;
            set
            {
                if (value != _baseZ)
                {
                    _baseZ = value;
                    _ball.BaseRoll = _baseZ;
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
                    _fov = value;
                    _ball.FieldOfView = _fov;
                }
            }
        }
        public Color LightingColor
        {
            get => _ball.LightingColor;
            set
            {
                if (value != _ball.LightingColor)
                {
                    _ball.LightingColor = value;
                    OnDisplayUpdate();
                }
            }
        }
        public Color LightingColorAlt
        {
            get => _ball.LightingColorAlt;
            set
            {
                if (value != _ball.LightingColorAlt)
                {
                    _ball.LightingColorAlt = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingX
        {
            get => _ball.LightingX;
            set
            {
                if (value != _ball.LightingX)
                {
                    _ball.LightingX = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingY
        {
            get => _ball.LightingY;
            set
            {
                if (value != _ball.LightingY)
                {
                    _ball.LightingY = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingZ
        {
            get => _ball.LightingZ;
            set
            {
                if (value != _ball.LightingZ)
                {
                    _ball.LightingZ = value;
                    OnDisplayUpdate();
                }
            }
        }
        void X_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.Pitch = e.Value.DoubleValue;
        }
        void Y_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.Yaw = e.Value.DoubleValue;
        }
        void Z_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.Roll = e.Value.DoubleValue;
        }
        void AltLightingUsed_Execute(object action, HeliosActionEventArgs e)
        {
            _ball.LightingAltEnabled = e.Value.BoolValue;
        }
        public override void Reset()
        {
            base.Reset();
            _rotXValue.SetValue(new BindingValue(0d), true);
            _rotYValue.SetValue(new BindingValue(0d), true);
            _rotZValue.SetValue(new BindingValue(0d), true);
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
            writer.WriteElementString("AltColor", colorConverter.ConvertToInvariantString(LightingColorAlt));
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
                    LightingColorAlt = (Color)colorConverter.ConvertFromInvariantString(reader.ReadElementString("AltColor"));
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
        }
        #endregion de/serialize 
    }
    // helper for intellisense in XAML
    public class DesignTimeCustomGaugeBall : DesignTimeControl<CustomGaugeBall> { }
}
