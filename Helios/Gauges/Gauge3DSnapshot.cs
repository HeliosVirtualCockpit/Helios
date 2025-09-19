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

using GadrocsWorkshop.Helios.Interfaces.Vendor.Functions;
using GadrocsWorkshop.Helios.Windows.ViewModel;
using NLog;
using System;
using System.Diagnostics.Contracts;
using System.Drawing.Imaging;
using System.Reflection;
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
        private Viewport3DVisual _viewport;
        private VisualBrush _visualBrush;
        private PerspectiveCamera _camera;
        private readonly AxisAngleRotation3D _rotX, _rotY, _rotZ;
        private readonly GeometryModel3D _model;
        private MeshGeometry3D _mesh;
        private Point _location;
        private ImageSource _imageSource;
        private double _fieldOfView = 35d;
        private Color _lightingColor;
        private double _lightingBrightness = 1d;
        private DirectionalLight _lighting;
        private double _lightingX, _lightingY, _lightingZ;
        private Effects.ColorAdjustEffect _effect;
        private bool _effectsExclusion = false;
        private bool _designTime = false, _designTimeChecked = false;
        private long _renderCalls = 0;
        private long _totalRenderCallTime = 0;
        private long _meanRenderCallTime = 0;
        private long _hwmRenderCallTime = 0;
        private long _lwmRenderCallTime = 1000;
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Gauge3dSnapshot(MeshGeometry3D mesh)
        {
            _mesh = mesh;

            // Build 3D viewport
            _viewport = new Viewport3DVisual();

            _camera = new PerspectiveCamera(
                new Point3D(0, 0, 3),
                new Vector3D(0, 0, -1),
                new Vector3D(0, 1, 0),
                _fieldOfView);

            _viewport.Camera = _camera;

            _lightingColor = Colors.White;

            _lightingX = -1d;
            _lightingY = -1d;
            _lightingZ = -2d;
            _lighting  = new DirectionalLight(_lightingColor, new Vector3D(_lightingX, _lightingY, _lightingZ));

            var group = new Model3DGroup();
            group.Children.Add(_lighting);

            _rotX = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            _rotY = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            _rotZ = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            Transform3DGroup transform = new Transform3DGroup();
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

            _visualBrush = new VisualBrush
            {
                Visual = _viewport,
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(0, 0, 1, 1)
            };

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
        public Viewport3DVisual GetViewport
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
                    _lighting.Color = ScaleBrightness(_lightingColor, _lightingBrightness);

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
                    _lighting.Color = ScaleBrightness(_lightingColor, _lightingBrightness);
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
        public bool EffectsExclusion
        {
            get => _effectsExclusion;
            set
            {
                if (!_effectsExclusion.Equals(value))
                {
                    _effectsExclusion = value;
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
        public void Rotation3D(Point3D point3D)
        {
            RotateX(point3D.X);
            RotateY(point3D.Y);
            RotateZ(point3D.Z);
        }
        public static Color ScaleBrightness(Color baseColor, double factor)
        {
            byte r = (byte)Math.Min(255, baseColor.R * factor);
            byte g = (byte)Math.Min(255, baseColor.G * factor);
            byte b = (byte)Math.Min(255, baseColor.B * factor);

            return Color.FromArgb(baseColor.A, r, g, b);
        }
        /// <summary>
        /// Keeps the Viewport3D in a brush and then uses it to create a temporary rectangle which can then 
        /// have a ShaderEffect applied to it, and then finally get drawn again as a rectangle.
        /// </summary>
        public void RedrawSnapshot(DrawingContext dc)
        {
            int w = (int)Width;
            int h = (int)Height;

            if (w <= 0 || h <= 0) return;
            _viewport.Viewport = new Rect(0, 0, w, h);
            _visualBrush.Viewbox = new Rect(0, 0, w, h);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            _renderCalls++;

            DrawingVisual tempVisual = new DrawingVisual();
            DrawingContext tdc = tempVisual.RenderOpen();
            tdc.DrawRectangle(_visualBrush, null, new Rect(_location.X, _location.Y, w, h));
            tdc.Close();
            if (!_designTime)
            {
                // Attempt to cache the ShaderEffect if we're in Control Center
                if (_effect == null && ConfigManager.ProfileManager.CurrentEffect != null)
                {
                    _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;
                }
            }
            else
            {
                _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;
            }
            if (_effect != null && !EffectsExclusion && _effect.Enabled)
            {
                tempVisual.Effect = _effect;
            }

            dc.DrawRectangle(new VisualBrush(tempVisual), null, new Rect(_location.X, _location.Y, w, h));

            watch.Stop();
            _totalRenderCallTime += watch.ElapsedTicks;
            _meanRenderCallTime = _totalRenderCallTime / _renderCalls;
            _hwmRenderCallTime = _hwmRenderCallTime < watch.ElapsedTicks ? watch.ElapsedTicks : _hwmRenderCallTime;
            _lwmRenderCallTime = _lwmRenderCallTime > watch.ElapsedTicks ? watch.ElapsedTicks : _lwmRenderCallTime;

            if (_renderCalls == 5000)
            {
                Logger.Debug($"Total Render Calls: {_renderCalls}, Mean Render Time: {_meanRenderCallTime} Ticks, Shortest Render Time: {_lwmRenderCallTime} Ticks, Longest Render Time: {_hwmRenderCallTime} Ticks");
                _renderCalls = _totalRenderCallTime = _hwmRenderCallTime = _meanRenderCallTime = 0;
                _lwmRenderCallTime = 1000;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }
        protected override int VisualChildrenCount => 1;
        // protected override Visual GetVisualChild(int index) => _visual;
    }
}
