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
    using System.Xml;
    using GadrocsWorkshop.Helios.Controls.Capabilities;

    [HeliosControl("Helios.Base.CustomGaugeCylinder", "Custom Gauge Cylinder", "Custom Controls", typeof(GaugeRenderer), HeliosControlFlags.None)]

    public class CustomGaugeCylinder : CustomGauge3d
    {

        private Size _size;
        private GaugeCylinder _cylinder;
        private string _imageName;

        private bool _suppressScale = false;

        public CustomGaugeCylinder()
            : base("Custom Gauge Cylinder", new Size(300, 300))
        {
            _size = base.NativeSize;
            _imageName = "{F-15E}/Gauges/Instruments/ADI-Tape.xaml";
            InitialAngleX = 0d;
            InitialAngleY = 90d;
            InitialAngleZ = 90d;
            FieldOfView = 35d;
            _cylinder = new GaugeCylinder(_imageName, new Point(0d, 0d), _size, InitialAngleX, InitialAngleY, InitialAngleZ, FieldOfView);
            CylinderRadius = 0.9d;
            CylinderHeight = 0.8d;
            _cylinder.X = _cylinder.Y = _cylinder.Z = 0.00001d;
            _cylinder.X = _cylinder.Y = _cylinder.Z = 0.0d;
            Components.Add(_cylinder);


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
                    _cylinder.Image = _imageName;
                }
            }
        }
        protected override IGauge3d gauge {
            get => _cylinder;

        }
        public double CylinderRadius
        {
            get => _cylinder.CylinderRadius;
            set
            {
                if (value != _cylinder.CylinderRadius)
                {
                    double oldValue = _cylinder.CylinderRadius;
                    _cylinder.CylinderRadius = value;
                    OnPropertyChanged("CylinderRadius", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public double CylinderHeight
        {
            get => _cylinder.CylinderHeight;
            set
            {
                if (value != _cylinder.CylinderHeight)
                {
                    double oldValue = _cylinder.CylinderHeight;
                    _cylinder.CylinderHeight = value;
                    OnPropertyChanged("CylinderHeight", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public override void Reset()
        {
            base.Reset();
            _cylinder.Reset();
        }
        public override void ScaleChildren(double scaleX, double scaleY)
        {
            if (!_suppressScale)
            {
                _cylinder.ScaleChildren(scaleX, scaleY);
                _suppressScale = false;
            }
            base.ScaleChildren(scaleX, scaleY);
        }
        protected override void PostUpdateRectangle(Rect previous, Rect current)
        {
            _suppressScale = false;
            if (!previous.Equals(new Rect(0, 0, 0, 0)) && !(previous.Width == current.Width && previous.Height == current.Height))
            {
                _cylinder.ScaleChildren(current.Width / previous.Width, current.Height / previous.Height);
                _suppressScale = true;
            }
        }
        #endregion
        #region de/serialize
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteStartElement("CylinderProperties");
            writer.WriteElementString("Radius", CylinderRadius.ToString("N3", CultureInfo.InvariantCulture));
            writer.WriteElementString("Height", CylinderHeight.ToString("N3", CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        public override void ReadXml(XmlReader reader)
        {

            base.ReadXml(reader);
            if (reader.Name.Equals("CylinderProperties"))
            {
                reader.ReadStartElement("CylinderProperties");
                CylinderRadius = double.Parse(reader.ReadElementString("Radius"), CultureInfo.InvariantCulture);
                CylinderHeight = double.Parse(reader.ReadElementString("Height"), CultureInfo.InvariantCulture);
                reader.ReadEndElement();
            }
        }
        #endregion de/serialize 

    }
    // helper for intellisense in XAML
    public class DesignTimeCustomGaugeCylinder : DesignTimeControl<CustomGaugeCylinder> { }
}
