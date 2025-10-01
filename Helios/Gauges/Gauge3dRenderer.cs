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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace GadrocsWorkshop.Helios.Gauges
{
    public class Gauge3dRenderer : FrameworkElement
    {
        private readonly Viewport3DVisual _viewport;
        private readonly VisualBrush _visualBrush;
        private readonly PerspectiveCamera _camera;
        private readonly AxisAngleRotation3D _rotA, _rotB, _rotC;
        private readonly GeometryModel3D _model;
        private readonly Model3DCollection _models;
        private MeshGeometry3D _mesh;
        private readonly MeshGeometry3D[] _meshs;
        private Point _location;
        private ImageSource _imageSource;
        private double _fieldOfView = 35d;
        private Color _lightingColor;
        private double _lightingBrightness = 1d;
        private readonly DirectionalLight _lighting;
        private readonly AmbientLight _ambientLight;
        private double _lightingX, _lightingY, _lightingZ;
        private Effects.ColorAdjustEffect _effect;
        private bool _effectsExclusion = false;
        private readonly bool _designTime = false;
        private long _renderCalls = 0;
        private long _totalRenderCallTime = 0;
        private long _meanRenderCallTime = 0;
        private long _hwmRenderCallTime = 0;
        private long _lwmRenderCallTime = 1000;
        private double _initialX = 0, _initialY = 0, _initialZ = 0;  // This is correct for the majority of the 3d objects.
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private bool _flipXY = false;

        public Gauge3dRenderer(MeshGeometry3D mesh) : this(new MeshGeometry3D[1] { mesh }) { }
        public Gauge3dRenderer(MeshGeometry3D[] meshs)
        {
            _models = new Model3DCollection();
            var group = new Model3DGroup();
            _meshs = meshs;
            _mesh = meshs[0];

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
             _ambientLight = new AmbientLight(ScaleBrightness(Colors.White,0.0));

            group.Children.Add(_ambientLight);
            group.Children.Add(_lighting);

            _rotA = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            _rotB = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            _rotC = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            Transform3DGroup transform = new Transform3DGroup();
            // The order of the transformations is important!
            transform.Children.Add(new RotateTransform3D(_rotB));
            transform.Children.Add(new RotateTransform3D(_rotA));
            transform.Children.Add(new RotateTransform3D(_rotC));
            _model = new GeometryModel3D
            {
                Geometry = _meshs[0],
                BackMaterial = new DiffuseMaterial(Brushes.DarkBlue),
                Transform = transform
            };
            _models.Add(_model);
            group.Children.Add(_model);

            if(_meshs.Length > 1)
            {
                for (int i = 1; i < _meshs.Length; i++)
                {
                    _models.Add(new GeometryModel3D
                    {
                        Geometry = meshs[i],
                        Material = new DiffuseMaterial(Brushes.Black),
                        BackMaterial = new DiffuseMaterial(Brushes.Black),
                        Transform = transform
                    });
                    group.Children.Add(_models[i]);
                }
            }
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
        public Color AmbientLightingColor
        {
            get => _ambientLight.Color;
            set
            {
                if (value != _ambientLight.Color)
                {
                    _ambientLight.Color = value;
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
        public double InitialX
        {
            get => _initialX;
            set
            {
                if(value != _initialX)
                {
                    _initialX = value;
                }
            }
        }
        public double InitialY
        {
            get => _initialY;
            set
            {
                if (value != _initialY)
                {
                    _initialY = value;
                }
            }
        }
        public double InitialZ
        {
            get => _initialZ;
            set
            {
                if (value != _initialZ)
                {
                    _initialZ = value;
                    _flipXY = _initialZ % 90 == 0 && _initialZ % 180 != 0 && _initialZ != 0;
                }
            }
        }
        /// <summary>
        /// Rotate programmatically and redraw.
        /// </summary>
        /// <remarks>The wrapping of the images is best performed with images which
        /// have an aspect ratio of width = 2 x height, and oriented with the north
        /// pole along the top or the image and the south pole along the bottom.  
        /// This might requires the 3D object to be rotated to a sensible initial
        /// position.  Reorientation of the sphere also causes the axes to be rotated 
        /// so changes are made in this class so that the users of the class have an 
        /// intuitive X, Y, Z set of axes to work with when the default rotations 
        /// are in use
        /// Because the axes of rotation can change due to initiatial orientation, the
        /// rotations are referred to as A, B & C</remarks>
        public void RotateX(double x)
        {
            if (!_flipXY)
            {
                _rotA.Angle = _initialX + x;
            } else
            {
                _rotB.Angle = _initialX + x;
            }
        }
        public void RotateY(double y)
        {
            if (!_flipXY)
            {
                _rotB.Angle = _initialY + y;
            } else
            {
                _rotA.Angle = _initialY + y;
            }
        }
        public void RotateZ(double z)
        {
            _rotC.Angle = _initialZ + z;
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
        public void UpdateModelGeometries(MeshGeometry3D[] meshs)
        {
            int i = 0;
            foreach(Model3D model in _models)
            {
                GeometryModel3D model3D = model as GeometryModel3D;
                model3D.Geometry = meshs[i++];
            }
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
