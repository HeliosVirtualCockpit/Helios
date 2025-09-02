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

namespace GadrocsWorkshop.Helios.Gauges
{
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    public class GaugeBall : Gauge3d, IRefreshableImage, IGauge3d
    {
        // private string _imageFile;
        private Point _location;
        private Size _size;

        private double _xScale = 1.0;
        private double _yScale = 1.0;
        
        // private Gauge3dSnapshot _gauge;

        public GaugeBall(string imageFile, Point location, Size size, Point center)
            : this(imageFile, location, size, 180d, 270d, 0d)
        {
        }

        public GaugeBall(string imageFile, Point location, Size size, double basePitch = 0, double baseRoll = 0, double baseYaw = 0, double FOV = 35d)
            : base(imageFile, location, size, basePitch, baseRoll, baseYaw, FOV)
        {
            // _imageFile = string.IsNullOrEmpty(imageFile) ? "{helios}/Gauges/Common/ChequerBoard.xaml" : imageFile;
            _location = location;
            _size = size;
        }

        #region Properties
        protected override MeshGeometry3D BuildMesh()
        {
            return BuildSphere(1.5d, 64, 32);
        }
        #endregion
        protected override void OnRender(DrawingContext drawingContext)
        {
            Snapshot.RedrawSnapshot(drawingContext);
        }

        protected override void OnRefresh(double xScale, double yScale)
        {
            if(_xScale != xScale || _yScale != yScale)
            {
                _xScale = xScale;
                _yScale = yScale;
                Snapshot.Width = Math.Max(1d, _size.Width * xScale);
                Snapshot.Height = Math.Max(1d, _size.Height * yScale);
            }
        }
        public override void ScaleChildren(double scaleX, double scaleY)
        {
            _location.X *= scaleX;
            _location.Y *= scaleY;
            Snapshot.Top = _location.Y;
            Snapshot.Left = _location.X;
            OnDisplayUpdate();
        }
    }
}
