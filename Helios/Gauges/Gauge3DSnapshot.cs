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
using System.Diagnostics.Contracts;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
namespace GadrocsWorkshop.Helios.Gauges
{
    public class Gauge3dSnapshot : FrameworkElement
    {
        private readonly DrawingVisual _visual;
        private Viewport3D _viewport;
        private PerspectiveCamera _camera;
        private readonly AxisAngleRotation3D _rotX, _rotY, _rotZ;
        private readonly GeometryModel3D _model;
        private MeshGeometry3D _mesh;
        private Point _location;
        private ImageSource _imageSource;
        private double _fieldOfView = 35d;
        private Color _lightingColor, _lightingColorAlt;
        private double _lightingBrightness = 1d, _lightingAltBrightness = 1d;
        private bool _lightingAltEnabled = false;
        private DirectionalLight _lighting;
        private double _lightingX, _lightingY, _lightingZ;
        public Gauge3dSnapshot(MeshGeometry3D mesh)
        {
            _mesh = mesh;
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
            _lightingColorAlt = Colors.Green;
            _lightingAltEnabled = false;

            _lightingX = -1d;
            _lightingY = -1d;
            _lightingZ = -2d;
            _lighting  = new DirectionalLight(_lightingColor, new Vector3D(_lightingX, _lightingY, _lightingZ));

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
            _model = new GeometryModel3D
            {
                Geometry = _mesh,
                BackMaterial = new DiffuseMaterial(Brushes.DarkBlue),
                Transform = transform
            };
            group.Children.Add(_model);

            _viewport.Children.Add(new ModelVisual3D { Content = group });
        }
        public ImageSource SetTexture
        {
            set
            {
                if(value != _imageSource)
                {
                    _imageSource = value;
                    _model.Material = new DiffuseMaterial(new ImageBrush(value));
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
                    _camera.FieldOfView = _fieldOfView;
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
                    if (!_lightingAltEnabled)
                    {
                        _lighting.Color = ScaleBrightness(_lightingColor, _lightingBrightness);
                    }
                }
            }
        }
        public double LightingBrightness
        {
            get => _lightingBrightness;
            set
            {
                if (value != _lightingBrightness)
                {
                    _lightingBrightness = value;
                    if (!_lightingAltEnabled)
                    {
                        _lighting.Color = ScaleBrightness(_lightingColor, _lightingBrightness);
                    }
                }
            }
        }
        public Color LightingColorAlt
        {
            get => _lightingColorAlt;
            set
            {
                if (value != _lightingColorAlt)
                {
                    _lightingColorAlt = value;
                    if (_lightingAltEnabled)
                    {
                        _lighting.Color = ScaleBrightness(_lightingColorAlt, _lightingAltBrightness);
                    }
                }
            }
        }
        public double LightingAltBrightness
        {
            get => _lightingAltBrightness;
            set
            {
                if (value != _lightingAltBrightness)
                {
                    _lightingAltBrightness = value;
                    if (_lightingAltEnabled)
                    {
                        _lighting.Color = ScaleBrightness(_lightingColorAlt, _lightingAltBrightness);
                    }
                }
            }
        }
        public bool LightingAltEnabled { 
            get => _lightingAltEnabled; 
            set
            {
                if (value != _lightingAltEnabled)
                {
                    _lightingAltEnabled = value;
                    if (!_lightingAltEnabled)
                    {
                        _lighting.Color = ScaleBrightness(_lightingColor, _lightingBrightness);
                    } else
                    {
                        _lighting.Color = ScaleBrightness(_lightingColorAlt, _lightingAltBrightness);
                    }
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
                    _lighting.Direction = new Vector3D(_lightingX, _lightingY, _lightingZ);
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
                    _lighting.Direction = new Vector3D(_lightingX, _lightingY, _lightingZ);
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
                    _lighting.Direction = new Vector3D(_lightingX, _lightingY, _lightingZ);
                }
            }
        }
      public MeshGeometry3D Mesh
        {
            set
            {
                if (!value.Equals(_mesh))
                {
                    _mesh = value;
                    _model.Geometry = _mesh;
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
        public static Color ScaleBrightness(Color baseColor, double factor)
        {
            byte r = (byte)Math.Min(255, baseColor.R * factor);
            byte g = (byte)Math.Min(255, baseColor.G * factor);
            byte b = (byte)Math.Min(255, baseColor.B * factor);

            return Color.FromArgb(baseColor.A, r, g, b);
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
    }
}
