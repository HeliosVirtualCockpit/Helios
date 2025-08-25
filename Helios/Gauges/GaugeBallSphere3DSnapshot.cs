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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
namespace GadrocsWorkshop.Helios.Gauges
{
    public class GaugeBallSphere3DSnapshot : FrameworkElement
    {
        private readonly DrawingVisual _visual;
        private Viewport3D _viewport;
        private PerspectiveCamera _camera;
        private readonly AxisAngleRotation3D _rotX, _rotY, _rotZ;
        private readonly GeometryModel3D _sphereModel;
        private Point _location;
        private ImageSource _imageSource;
        private double _fieldOfView = 35d;
        private Vector3D _lightingDirection;
        private Color _lightingColor;
        private DirectionalLight _lighting;
        private double _lightingX, _lightingY, _lightingZ;
        public GaugeBallSphere3DSnapshot()
        {
            _visual = new DrawingVisual();
            AddVisualChild(_visual);

            // Build 3D viewport
            _viewport = new Viewport3D();

            _camera = new PerspectiveCamera(
                new Point3D(0, 0, 5),
                new Vector3D(0, 0, -5),
                new Vector3D(0, 1, 0),
                _fieldOfView);
            _viewport.Camera = _camera;

            _lightingColor = Colors.White;
            _lightingX = -1d;
            _lightingY = -1d;
            _lightingZ = -2d;
            _lightingDirection = new Vector3D(_lightingX, _lightingY, _lightingZ);
            _lighting  = new DirectionalLight(_lightingColor, _lightingDirection);

            var group = new Model3DGroup();
            group.Children.Add(_lighting);

            _rotX = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            _rotY = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            _rotZ = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            var transform = new Transform3DGroup();
            // The order of the transformations is important!
            transform.Children.Add(new RotateTransform3D(_rotY));
            transform.Children.Add(new RotateTransform3D(_rotX));
            transform.Children.Add(new RotateTransform3D(_rotZ));
            _sphereModel = new GeometryModel3D
            {
                Geometry = BuildSphere(1.5, 64, 32),
                BackMaterial = new DiffuseMaterial(Brushes.DarkBlue),
                Transform = transform
            };
            group.Children.Add(_sphereModel);

            _viewport.Children.Add(new ModelVisual3D { Content = group });
        }
        public ImageSource SetTexture
        {
            set
            {
                if(value != _imageSource)
                {
                    _imageSource = value;
                    _sphereModel.Material = new DiffuseMaterial(new ImageBrush(value));
                }
            }   
        }
        public Viewport3D GetViewport
        {
            get { return _viewport; }
        }
        public double Left
        {
            get => _location.X;
            set => _location.X = value;
        }
        public double Top
        {
            get => _location.Y;
            set => _location.Y = value;
        }
        public double FieldOfView
        {
            get => _fieldOfView;
            set
            {
                if (value != _fieldOfView)
                {
                    _fieldOfView = value;
                    _camera = new PerspectiveCamera(
                        new Point3D(0, 0, 5),
                        new Vector3D(0, 0, -5),
                        new Vector3D(0, 1, 0),
                        _fieldOfView);
                    _viewport.Camera = _camera;
                }
            }
        }
        public Color LightingColor
        {
            get => _lightingColor;
            set
            {
                if (value != _lightingColor)
                {
                    _lightingColor = value;
                }
            }
        }
        public double LightingX
        {
            get => _lightingX;
            set
            {
                if (value != _lightingX)
                {
                    _lightingX = value;
                }
            }
        }
        public double LightingY
        {
            get => _lightingY;
            set
            {
                if (value != _lightingY)
                {
                    _lightingY = value;
                }
            }
        }
        public double LightingZ
        {
            get => _lightingZ;
            set
            {
                if (value != _lightingZ)
                {
                    _lightingZ = value;
                }
            }
        }
        /// <summary>
        /// Rotate programmatically and redraw.
        /// </summary>
        public void RotateX(double x)
        {
            _rotX.Angle = x;
        }
        public void RotateY(double y)
        {
            _rotY.Angle = y;
        }
        public void RotateZ(double z)
        {
            _rotZ.Angle = z;
        }

        /// <summary>
        /// Takes a snapshot of the Viewport3D and draws it into the DrawingContext.
        /// </summary>
        public void RedrawSnapshot(DrawingContext dc)
        {
            int w = (int)Width;
            int h = (int)Height;

            if (w <= 0 || h <= 0) return;

            var rtb = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
            _viewport.Measure(new Size(w, h));
            _viewport.Arrange(new Rect(0, 0, w, h));
            rtb.Render(_viewport);

            dc.DrawImage(rtb, new Rect(_location.X, _location.Y, w, h));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _visual;

        private MeshGeometry3D BuildSphere(double radius, int slices, int stacks)
        {
            var mesh = new MeshGeometry3D();

            for (int stack = 0; stack <= stacks; stack++)
            {
                double phi = Math.PI * stack / stacks;
                double y = radius * Math.Cos(phi);
                double r = radius * Math.Sin(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    double theta = 2 * Math.PI * slice / slices;
                    double x = r * Math.Sin(theta);
                    double z = r * Math.Cos(theta);

                    mesh.Positions.Add(new Point3D(x, y, z));

                    double u = (double)slice / slices;
                    //double v = 1.0 - (double)stack / stacks;
                    double v = (double)stack / stacks;
                    mesh.TextureCoordinates.Add(new Point(u, v));
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
    }
}
