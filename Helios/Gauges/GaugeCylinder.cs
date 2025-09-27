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
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    public class GaugeCylinder : Gauge3d, IGauge3d
    {
        private Point _location;
        private Size _size;

        private double _xScale = 1.0;
        private double _yScale = 1.0;

        private double _cylinderHeight = 2.0;
        private double _radius = 1.5d;
 
        public GaugeCylinder(string imageFile, Point location, Size size, Point center)
            : this(imageFile, location, size, 0d, 0d, 0d)
        {
        }

        public GaugeCylinder(string imageFile, Point location, Size size, double initialAngleX = 0, double initialAngleY = 0, double initialAngleZ = 0, double FOV = 70d)
            : base(imageFile, location, size, initialAngleX, initialAngleZ, initialAngleY, FOV)
        {
            _location = location;
            _size = size;

        }

        #region Properties
        public double CylinderRadius
        {
            get => _radius;
            set
            {
                if (value != _radius)
                {
                    _radius = value;
                    UpdateMesh();
                    OnDisplayUpdate();
                }
            }
        }
        public double CylinderHeight
        {
            get => _cylinderHeight;
            set
            {
                if (value != _cylinderHeight)
                {
                    _cylinderHeight = value;
                    UpdateMesh();
                    OnDisplayUpdate();
                }
            }
        }

        #endregion
        protected override MeshGeometry3D[] BuildMeshCollection()
        {
            MeshGeometry3D[] meshs = new MeshGeometry3D[3];
            meshs[0] = BuildCylinder(_radius, _cylinderHeight, 32);
            meshs[1] = TopCap(_radius, _cylinderHeight, 32);
            meshs[2] = BottomCap(_radius, _cylinderHeight, 32);
            return meshs;
        }
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
        public override void ScaleChildren(double xScale, double yScale)
        {
            _location.X *= xScale;
            _location.Y *= yScale;
            Snapshot.Top = _location.Y;
            Snapshot.Left = _location.X;
            OnDisplayUpdate();
        }
    }
}
