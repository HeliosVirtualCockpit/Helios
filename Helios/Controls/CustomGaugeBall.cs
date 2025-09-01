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
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using GadrocsWorkshop.Helios.Gauges;
     using GadrocsWorkshop.Helios.Util;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.Base.CustomGaugeBall", "Custom Gauge Ball", "Custom Controls", typeof(GaugeRenderer), HeliosControlFlags.None)]

    public class CustomGaugeBall : CustomGauge3d
    {

        private Size _size;
        private GaugeBall _sphere;
        private string _imageName;

        private bool _suppressScale = false;

        public CustomGaugeBall()
            : base("Custom Gauge Ball", new Size(300, 300))
        {
            _size = base.NativeSize;
            _imageName = "{F-16C}/Gauges/ADI/Viper-ADI-Ball.xaml";
            BasePitch = 0d;
            BaseYaw = 270d;
            BaseRoll = 180d;
            FieldOfView = 35d;
            _sphere = new GaugeBall(_imageName, new Point(0d, 0d), _size, BasePitch, BaseYaw, BaseRoll, FieldOfView);
            Components.Add(_sphere);

            LightingColor = Colors.White;
            LightingColorAlt = Colors.Green;
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
                    _sphere.Image = _imageName;
                }
            }
        }
        public override void Reset()
        {
            base.Reset();
            _sphere.Reset();
        }
        public override void ScaleChildren(double scaleX, double scaleY)
        {
            if (!_suppressScale)
            {
                _sphere.ScaleChildren(scaleX, scaleY);
                _suppressScale = false;
            }
            base.ScaleChildren(scaleX, scaleY);
        }
        protected override void PostUpdateRectangle(Rect previous, Rect current)
        {
            _suppressScale = false;
            if (!previous.Equals(new Rect(0, 0, 0, 0)) && !(previous.Width == current.Width && previous.Height == current.Height))
            {
                _sphere.ScaleChildren(current.Width / previous.Width, current.Height / previous.Height);
                _suppressScale = true;
            }
        }
        protected override IGauge3d gauge
        {
            get => _sphere;

        }
        #endregion
        #region de/serialize
        public override void WriteXml(XmlWriter writer)
        {

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
        }
        #endregion de/serialize 
    }
    // helper for intellisense in XAML
    public class DesignTimeCustomGaugeBall : DesignTimeControl<CustomGaugeBall> { }
}
