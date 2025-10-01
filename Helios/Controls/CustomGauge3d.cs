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
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using GadrocsWorkshop.Helios.Gauges;
     using GadrocsWorkshop.Helios.Util;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Xml;

    public abstract class CustomGauge3d : BaseGauge
    {

        private Size _size;
        private double _fov;
        private double _initialX, _initialY, _initialZ;
        private bool _calibrationNeeded = false;
        private double _minInputX, _minInputY, _minInputZ;
        private double _maxInputX, _maxInputY, _maxInputZ;
        private double _minOutputX, _minOutputY, _minOutputZ;
        private double _maxOutputX, _maxOutputY, _maxOutputZ;
        private CalibrationPointCollectionDouble _xScale, _yScale, _zScale;
        private double _ballDirection = 1d;

        private HeliosValue _rotXValue, _rotYValue, _rotZValue, _rotRotationValue, _lightBrightnessValue;

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
            _rotRotationValue = new HeliosValue(this, new BindingValue("0;0;0"), "", "All Rotations", "Rotation of X, Y & Z Axes in degrees.", "Text String containing x°;y°;z° values", BindingValueUnits.Text);
            _rotRotationValue.Execute += new HeliosActionHandler(Rotation_Execute);
            Actions.Add(_rotRotationValue);
            _lightBrightnessValue = new HeliosValue(this, new BindingValue(false), "", "Brightness of default lighting source", "Number", "0 to 1", BindingValueUnits.Numeric);
            _lightBrightnessValue.Execute += new HeliosActionHandler(LightingBrightness_Execute);
            Actions.Add(_lightBrightnessValue);
        }
        protected abstract IGauge3d gauge { get; }

        public abstract string Image { get; set; }

        #region Properties

        public double InitialAngleX
        {
            get => _initialX;
            set
            {
                _ballDirection = gauge is GaugeBall ? 1d : -1d;
                double oldValue = _initialX;
                if (value != _initialX)
                {
                    _initialX = value;
                    if (gauge != null)
                    {
                        gauge.InitialAngleX = _initialX;
                    }
                    OnPropertyChanged("InitialAngleX", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double InitialAngleY
        {
            get => _initialY;
            set
            {
                double oldValue = _initialY;

                if (value != _initialY)
                {
                    _initialY = value;
                    if (gauge != null)
                    {
                        gauge.InitialAngleY = _initialY;
                    }
                    OnPropertyChanged("InitialAngleY", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double InitialAngleZ
        {
            get => _initialZ;
            set
            {
                double oldValue = _initialZ;
                if (value != _initialZ)
                {
                    _initialZ = value;
                    if(gauge != null)
                    {
                        gauge.InitialAngleZ = _initialZ;
                    }
                    OnPropertyChanged("InitialAngleZ", oldValue, value, true);
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
        public bool CalibrationNeeded
        {
            get => _calibrationNeeded;
            set
            {
                if (value != _calibrationNeeded)
                {
                    bool oldValue = _calibrationNeeded;
                    _calibrationNeeded = value;
                    OnPropertyChanged("CalibrationNeeded", oldValue, value, true);
                    Refresh();
                }
            }
        }
        private CalibrationPointCollectionDouble ReScaleX()
        {
            return ReScale(_minInputX, _minOutputX, _maxInputX, _maxOutputX);
        }
        private CalibrationPointCollectionDouble ReScaleY()
        {
            return ReScale(_minInputY, _minOutputY, _maxInputY, _maxOutputY);
        }
        private CalibrationPointCollectionDouble ReScaleZ()
        {
            return ReScale(_minInputZ, _minOutputZ, _maxInputZ, _maxOutputZ);
        }
        private CalibrationPointCollectionDouble ReScale(double in1, double out1, double in2, double out2)
        {
            if (in1 == in2 || out1 == out2)
            {
                return null;
            }
            return new CalibrationPointCollectionDouble(in1, out1, in2, out2);
        }
        public double MinInputX
        {
            get => _minInputX;
            set
            {
                if (value != _minInputX)
                {
                    double oldValue = _minInputX;
                    _minInputX = value;
                    _xScale = ReScaleX();
                }
            }
        }
        public double MinInputY
        {
            get => _minInputY;
            set
            {
                if (value != _minInputY)
                {
                    double oldValue = _minInputY;
                    _minInputY = value;
                    _yScale = ReScaleY();
                }
            }
        }
        public double MinInputZ
        {
            get => _minInputZ;
            set
            {
                if (value != _minInputZ)
                {
                    double oldValue = _minInputZ;
                    _minInputZ = value;
                    _zScale = ReScaleZ();
                }
            }
        }
        public double MaxInputX
        {
            get => _maxInputX;
            set
            {
                if (value != _maxInputX)
                {
                    double oldValue = _maxInputX;
                    _maxInputX = value;
                    _xScale = ReScaleX();
                }
            }
        }
        public double MaxInputY
        {
            get => _maxInputY;
            set
            {
                if (value != _maxInputY)
                {
                    double oldValue = _maxInputY;
                    _maxInputY = value;
                    _yScale = ReScaleY();
                }
            }
        }
        public double MaxInputZ
        {
            get => _maxInputZ;
            set
            {
                if (value != _maxInputZ)
                {
                    double oldValue = _maxInputZ;
                    _maxInputZ = value;
                    _zScale = ReScaleZ();
                }
            }
        }
        public double MinOutputX
        {
            get => _minOutputX;
            set
            {
                if (value != _minOutputX)
                {
                    double oldValue = _minOutputX;
                    _minOutputX = value;
                    _xScale = ReScaleX();
                }
            }
        }
        public double MinOutputY
        {
            get => _minOutputY;
            set
            {
                if (value != _minOutputY)
                {
                    double oldValue = _minOutputY;
                    _minOutputY = value;
                    _yScale = ReScaleY();
                }
            }
        }
        public double MinOutputZ
        {
            get => _minOutputZ;
            set
            {
                if (value != _minOutputZ)
                {
                    double oldValue = _minOutputZ;
                    _minOutputZ = value;
                    _zScale = ReScaleZ();
                }
            }
        }
        public double MaxOutputX
        {
            get => _maxOutputX;
            set
            {
                if (value != _maxOutputX)
                {
                    double oldValue = _maxOutputX;
                    _maxOutputX = value;
                    _xScale = ReScaleX();
                }
            }
        }
        public double MaxOutputY
        {
            get => _maxOutputY;
            set
            {
                if (value != _maxOutputY)
                {
                    double oldValue = _maxOutputY;
                    _maxOutputY = value;
                    _yScale = ReScaleY();
                }
            }
        }
        public double MaxOutputZ
        {
            get => _maxOutputZ;
            set
            {
                if (value != _maxOutputZ)
                {
                    double oldValue = _maxOutputZ;
                    _maxOutputZ = value;
                    _zScale = ReScaleZ();
                }
            }
        }
        #endregion Properties

        void X_Execute(object action, HeliosActionEventArgs e)
        {
             gauge.X = _xScale == null ? _ballDirection * e.Value.DoubleValue : _ballDirection * _xScale.Interpolate(e.Value.DoubleValue);
        }
        void Y_Execute(object action, HeliosActionEventArgs e)
        {
            gauge.Y = _yScale == null ? -e.Value.DoubleValue : -_yScale.Interpolate(e.Value.DoubleValue);
        }
        void Z_Execute(object action, HeliosActionEventArgs e)
        {
            gauge.Z = _zScale == null ? -e.Value.DoubleValue : -_zScale.Interpolate(e.Value.DoubleValue);
        }
        void Rotation_Execute(object action, HeliosActionEventArgs e)
        {
            _rotRotationValue.SetValue(e.Value, e.BypassCascadingTriggers);
            string[] parts;
            parts = Tokenizer.TokenizeAtLeast(e.Value.StringValue, 3, ';');
            double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double x);
            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double y);
            double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double z);
            (gauge as GaugeBall).Rotation3D = new Point3D(x, -y, -z);
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
        #endregion Actions
        #region de/serialize
        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            base.WriteXml(writer);
            writer.WriteElementString("Image", Image.ToString(CultureInfo.InvariantCulture));
            writer.WriteStartElement("Properties3D");
            writer.WriteElementString("InitialAngleX", _initialX.ToString("N0", CultureInfo.InvariantCulture));
            writer.WriteElementString("InitialAngleY", _initialY.ToString("N0", CultureInfo.InvariantCulture));
            writer.WriteElementString("InitialAngleZ", _initialZ.ToString("N0", CultureInfo.InvariantCulture));
            if (_xScale != null || _yScale != null || _zScale != null)
            {
                writer.WriteStartElement("AxesCalibrations");
                if (_xScale != null)
                {
                    writer.WriteStartElement("XCalibration");
                    writer.WriteElementString("MinInputX", _minInputX.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MaxInputX", _maxInputX.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MinOutputX", _minOutputX.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MaxOutputX", _maxOutputX.ToString(CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                if (_yScale != null)
                {
                    writer.WriteStartElement("YCalibration");
                    writer.WriteElementString("MinInputY", _minInputY.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MaxInputY", _maxInputY.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MinOutputY", _minOutputY.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MaxOutputY", _maxOutputY.ToString(CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                if (_zScale != null)
                {
                    writer.WriteStartElement("ZCalibration");
                    writer.WriteElementString("MinInputZ", _minInputZ.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MaxInputZ", _maxInputZ.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MinOutputZ", _minOutputZ.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("MaxOutputZ", _maxOutputZ.ToString(CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
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
            Image = reader.ReadElementString("Image");
            if (reader.Name.Equals("Properties3D"))
            {
                reader.ReadStartElement("Properties3D");
                InitialAngleX = double.Parse(reader.ReadElementString("InitialAngleX"), CultureInfo.InvariantCulture);
                InitialAngleY = double.Parse(reader.ReadElementString("InitialAngleY"), CultureInfo.InvariantCulture);
                InitialAngleZ = double.Parse(reader.ReadElementString("InitialAngleZ"), CultureInfo.InvariantCulture);
                if (reader.Name.Equals("AxesCalibrations"))
                {
                    reader.ReadStartElement("AxesCalibrations");
                    _calibrationNeeded = true;
                    if (reader.Name.Equals("XCalibration"))
                    {
                        reader.ReadStartElement("XCalibration");
                        _minInputX = double.Parse(reader.ReadElementString("MinInputX"), CultureInfo.InvariantCulture);
                        _maxInputX = double.Parse(reader.ReadElementString("MaxInputX"), CultureInfo.InvariantCulture);
                        _minOutputX = double.Parse(reader.ReadElementString("MinOutputX"), CultureInfo.InvariantCulture);
                        _maxOutputX = double.Parse(reader.ReadElementString("MaxOutputX"), CultureInfo.InvariantCulture);
                        reader.ReadEndElement();
                        _xScale = ReScaleX();
                    }
                    if (reader.Name.Equals("YCalibration"))
                    {
                        reader.ReadStartElement("YCalibration");
                        _minInputY = double.Parse(reader.ReadElementString("MinInputY"), CultureInfo.InvariantCulture);
                        _maxInputY = double.Parse(reader.ReadElementString("MaxInputY"), CultureInfo.InvariantCulture);
                        _minOutputY = double.Parse(reader.ReadElementString("MinOutputY"), CultureInfo.InvariantCulture);
                        _maxOutputY = double.Parse(reader.ReadElementString("MaxOutputY"), CultureInfo.InvariantCulture);
                        reader.ReadEndElement();
                        _yScale = ReScaleY();
                    }
                    if (reader.Name.Equals("ZCalibration"))
                    {
                        reader.ReadStartElement("ZCalibration");
                        _minInputZ = double.Parse(reader.ReadElementString("MinInputZ"), CultureInfo.InvariantCulture);
                        _maxInputZ = double.Parse(reader.ReadElementString("MaxInputZ"), CultureInfo.InvariantCulture);
                        _minOutputZ = double.Parse(reader.ReadElementString("MinOutputZ"), CultureInfo.InvariantCulture);
                        _maxOutputZ = double.Parse(reader.ReadElementString("MaxOutputZ"), CultureInfo.InvariantCulture);
                        reader.ReadEndElement();
                        _zScale = ReScaleZ();
                    }
                    reader.ReadEndElement();
                }
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
