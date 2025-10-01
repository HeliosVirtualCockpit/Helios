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

    public abstract class Gauge3d : GaugeComponent, IRefreshableImage
    {
        private string _imageFile;
        private Point _location;
        private Size _size;
        private double _initialAngleX;
        private double _initialAngleZ;
        private double _initialAngleY;
        private double _x;
        private double _z;
        private double _y;
        private Point3D _rotation3D;
        private double _fov;
        private bool _effectsExclusion = false;

        private double _xScale = 1.0;
        private double _yScale = 1.0;
        
        private Gauge3dRenderer _GaugeRenderer;

        public Gauge3d(string imageFile, Point location, Size size, double initialAngleX = 0, double initialAngleY = 0, double initialAngleZ = 0, double FOV = 35d)
        {
            _imageFile = string.IsNullOrEmpty(imageFile) ? "{helios}/Gauges/Common/ChequerBoard.xaml" : imageFile;
            _location = location;
            _size = size;
            _fov = FOV;
            _initialAngleX = initialAngleX;
            _initialAngleY = initialAngleY;
            _initialAngleZ = initialAngleZ;

            _GaugeRenderer = new Gauge3dRenderer(BuildMeshCollection())
            {
                Width = _size.Width,
                Height = _size.Height,
                Top = _location.Y,
                Left = _location.X,
                SetTexture = ConfigManager.ImageManager.LoadImage(_imageFile),
                FieldOfView = _fov,
                InitialX = _initialAngleX,
                InitialY = _initialAngleY,
                InitialZ = _initialAngleZ,
                LightingColor = Colors.White,
                EffectsExclusion = EffectsExclusion
            };
            OnDisplayUpdate();
        }

        #region Properties

        public string Image
        {
            get => _imageFile;
            set
            {
                if (value != _imageFile)
                {
                    _imageFile = value;
                    _GaugeRenderer.SetTexture = ConfigManager.ImageManager.LoadImage(value);
                    OnDisplayUpdate();
                }
            }
        }

        public double InitialAngleX
        {
            get
            {
                return _initialAngleX;
            }
            set
            {
                if (value != _initialAngleX)
                {
                    _initialAngleX = value;
                    _GaugeRenderer.InitialX = _initialAngleX;
                    _GaugeRenderer.RotateX(0d);
                    OnDisplayUpdate();
                }
            }
        }
        public double InitialAngleY
        {
            get
            {
                return _initialAngleY;
            }
            set
            {
                if (value != _initialAngleY)
                {
                    _initialAngleY = value;
                    _GaugeRenderer.InitialY = _initialAngleY;
                    _GaugeRenderer.RotateY(0d);
                    OnDisplayUpdate();
                }
            }
        }
        public double InitialAngleZ
        {
            get
            {
                return _initialAngleZ;
            }
            set
            {
                if (value != _initialAngleZ)
                {
                    _initialAngleZ = value;
                    _GaugeRenderer.InitialZ = _initialAngleZ;
                    _GaugeRenderer.RotateZ(0d);
                    OnDisplayUpdate();
                }
            }
        }
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                if (value != _x)
                {
                    _x = value;
                    _GaugeRenderer.RotateX(_x);
                    OnDisplayUpdate();
                }
            }
        }
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (value != _y)
                {
                    _y = value;
                    _GaugeRenderer.RotateY(_y);
                    OnDisplayUpdate();
                }
            }
        }
        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                if (value != _z)
                {
                    _z = value;
                    _GaugeRenderer.RotateZ(_z);
                    OnDisplayUpdate();
                }
            }
        }

        public Point3D Rotation3D
        {
            get => _rotation3D;
            set
            {
                if(_rotation3D != value)
                {
                    _rotation3D = value;
                    _GaugeRenderer.Rotation3D(new Point3D(_rotation3D.X, _rotation3D.Y, _rotation3D.Z));
                    OnDisplayUpdate();
                }
            }
        }
        public double FieldOfView
        {
            get => _fov;
            set
            {
                if (_fov != value)
                {
                    _fov = value;
                    _GaugeRenderer.FieldOfView = _fov;
                    OnDisplayUpdate();
                }
            }
        }
        public Color LightingColor
        {
            get => _GaugeRenderer.LightingColor;
            set
            {
                if (value != _GaugeRenderer.LightingColor)
                {
                    _GaugeRenderer.LightingColor = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingBrightness
        {
            get => _GaugeRenderer.LightingBrightness;
            set
            {
                if (value != _GaugeRenderer.LightingBrightness)
                {
                    _GaugeRenderer.LightingBrightness = value;
                    OnDisplayUpdate();
                }
            }

        }
        public double LightingX
        {
            get => _GaugeRenderer.LightingX;
            set
            {
                if (value != _GaugeRenderer.LightingX)
                {
                    _GaugeRenderer.LightingX = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingY
        {
            get => _GaugeRenderer.LightingY;
            set
            {
                if (value != _GaugeRenderer.LightingY)
                {
                    _GaugeRenderer.LightingY = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingZ
        {
            get => _GaugeRenderer.LightingZ;
            set
            {
                if (value != _GaugeRenderer.LightingZ)
                {
                    _GaugeRenderer.LightingZ = value;
                    OnDisplayUpdate();
                }
            }
        }

        public Gauge3dRenderer Snapshot
        {
            get => _GaugeRenderer;
            set
            {
                if (!value.Equals(_GaugeRenderer))
                {
                    _GaugeRenderer = value;
                }
            }
        }
        #endregion
        protected void UpdateMesh()
        {
            _GaugeRenderer.UpdateModelGeometries(BuildMeshCollection());
        }
        public virtual void Reset()
        {
            X = _initialAngleX;
            Y = _initialAngleY;
            Z = _initialAngleZ;
        }
        public override bool EffectsExclusion
        {
            get => _effectsExclusion;
            set
            {
                if (!_effectsExclusion.Equals(value))
                {
                    _effectsExclusion = value;
                    Snapshot.EffectsExclusion = value;

                }
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            _GaugeRenderer.RedrawSnapshot(drawingContext);
        }
        protected override void OnRefresh(double xScale, double yScale)
        {
            if(_xScale != xScale || _yScale != yScale)
            {
                _xScale = xScale;
                _yScale = yScale;
                _GaugeRenderer.Width = Math.Max(1d, _size.Width * xScale);
                _GaugeRenderer.Height = Math.Max(1d, _size.Height * yScale);
            }
        }
        public bool ConditionalImageRefresh(string imageName)
        {
            if ((Image ?? "").ToLower().Replace("/", @"\") == imageName || Image.ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                //Refresh();
            }
            return ImageRefresh;
        }
        public virtual void ScaleChildren(double scaleX, double scaleY)
        {
            _location.X *= scaleX;
            _location.Y *= scaleY;
            _GaugeRenderer.Top = _location.Y;
            _GaugeRenderer.Left = _location.X;
            OnDisplayUpdate();
        }
        protected abstract MeshGeometry3D[] BuildMeshCollection();
        protected static MeshGeometry3D BuildSphere(double radius, int slices, int stacks)
        {
            var mesh = new MeshGeometry3D();

            for (int stack = 0; stack <= stacks; stack++)
            {
                double phi = Math.PI * stack / stacks;
                double y = radius * Math.Cos(phi);
                double r = radius * Math.Sin(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    double theta = 2 * Math.PI * slice / slices + Math.PI;
                    double x = r * Math.Sin(theta);
                    double z = r * Math.Cos(theta);

                    mesh.Positions.Add(new Point3D(radius * x,
                                                   radius * y,
                                                   radius * z));
                    mesh.Normals.Add(new Vector3D(x, y, z));
                    mesh.TextureCoordinates.Add(new Point((double)slice / slices, (double)stack / stacks));
                }
            }

            for (int stack = 0; stack < stacks; stack++)
            {
                for (int slice = 0; slice < slices; slice++)
                {
                    int first = stack * (slices + 1) + slice;
                    int second = first + slices + 1;

                    mesh.TriangleIndices.Add(first);
                    mesh.TriangleIndices.Add(second);
                    mesh.TriangleIndices.Add(first + 1);

                    mesh.TriangleIndices.Add(second);
                    mesh.TriangleIndices.Add(second + 1);
                    mesh.TriangleIndices.Add(first + 1);
                }
            }
            return mesh;
        }
        protected static MeshGeometry3D BuildCylinder(double radius, double height, int slices)
        {
            var mesh = new MeshGeometry3D();

            double halfH = height / 2.0;

            // Generate side vertices
            for (int i = 0; i <= slices; i++)
            {
                double theta = 2 * Math.PI * i / slices - (Math.PI * 0.5);
                double x = radius * Math.Cos(theta);
                double z = radius * Math.Sin(theta);

                // Bottom vertex
                mesh.Positions.Add(new Point3D(x, -halfH, z));
                mesh.TextureCoordinates.Add(new Point((double)i / slices, 0));

                // Top vertex
                mesh.Positions.Add(new Point3D(x, halfH, z));
                mesh.TextureCoordinates.Add(new Point((double)i / slices, 1));
            }

            // Create triangles for side
            for (int i = 0; i < slices; i++)
            {
                int bottom1 = i * 2;
                int top1 = bottom1 + 1;
                int bottom2 = bottom1 + 2;
                int top2 = bottom2 + 1;

                mesh.TriangleIndices.Add(bottom1);
                mesh.TriangleIndices.Add(top1);
                mesh.TriangleIndices.Add(bottom2);

                mesh.TriangleIndices.Add(top1);
                mesh.TriangleIndices.Add(top2);
                mesh.TriangleIndices.Add(bottom2);
            }
            return mesh;
        }
        protected static MeshGeometry3D TopCap(double radius, double height, int slices)
        {
            double halfH = height / 2.0;
            Point3D center = new Point3D(0, 0, 0);

            // --- TOP CAP ---
            var topMesh = new MeshGeometry3D();
            int topCenterIndex = topMesh.Positions.Count;
            topMesh.Positions.Add(new Point3D(center.X, center.Y + halfH, center.Z));
            topMesh.Normals.Add(new Vector3D(0, 1, 0));
            topMesh.TextureCoordinates.Add(new Point(0.5, 0.5));

            for (int i = 0; i <= slices; i++)
            {
                double theta = 2 * Math.PI * i / slices;
                double x = Math.Cos(theta);
                double z = Math.Sin(theta);

                topMesh.Positions.Add(new Point3D(center.X + radius * x, center.Y + halfH, center.Z + radius * z));
                topMesh.Normals.Add(new Vector3D(0, 1, 0));
                topMesh.TextureCoordinates.Add(new Point((x + 1) / 2, (z + 1) / 2));
            }

            for (int i = 0; i < slices; i++)
            {
                topMesh.TriangleIndices.Add(topCenterIndex);
                topMesh.TriangleIndices.Add(topCenterIndex + i + 1);
                topMesh.TriangleIndices.Add(topCenterIndex + i + 2);
            }

            return topMesh;
        }
        protected static MeshGeometry3D BottomCap(double radius, double height, int slices)
        {
            double halfH = height / 2.0;
            Point3D center = new Point3D(0, 0, 0);

            // --- BOTTOM CAP ---
            var bottomMesh = new MeshGeometry3D();
            int bottomCenterIndex = bottomMesh.Positions.Count;
            bottomMesh.Positions.Add(new Point3D(center.X, center.Y - halfH, center.Z));
            bottomMesh.Normals.Add(new Vector3D(0, -1, 0));
            bottomMesh.TextureCoordinates.Add(new Point(0.5, 0.5));

            for (int i = 0; i <= slices; i++)
            {
                double theta = 2 * Math.PI * i / slices;
                double x = Math.Cos(theta);
                double z = Math.Sin(theta);

                bottomMesh.Positions.Add(new Point3D(center.X + radius * x, center.Y - halfH, center.Z + radius * z));
                bottomMesh.Normals.Add(new Vector3D(0, -1, 0));
                bottomMesh.TextureCoordinates.Add(new Point((x + 1) / 2, (z + 1) / 2));
            }

            for (int i = 0; i < slices; i++)
            {
                bottomMesh.TriangleIndices.Add(bottomCenterIndex);
                bottomMesh.TriangleIndices.Add(bottomCenterIndex + i + 2);
                bottomMesh.TriangleIndices.Add(bottomCenterIndex + i + 1);
            }
            return bottomMesh;
        }
    }
}
