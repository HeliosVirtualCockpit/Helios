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


    public class GaugeBall : GaugeComponent, IRefreshableImage
    {
        private string _imageFile;
        private Point _location;
        private Size _size;
        private double _basePitch;
        private double _baseRoll;
        private double _baseYaw;
        private double _pitch;
        private double _roll;
        private double _yaw;
        private double _fov;

        private double _xScale = 1.0;
        private double _yScale = 1.0;
        
        private GaugeBallSphere3DSnapshot _sphere3D;

        public GaugeBall(string imageFile, Point location, Size size, Point center)
            : this(imageFile, location, size, 0d, 0d, 0d)
        {
        }

        public GaugeBall(string imageFile, Point location, Size size, double basePitch = 0, double baseRoll = 0, double baseYaw = 0, double FOV = 35d)
        {
            _imageFile = string.IsNullOrEmpty(imageFile) ? "{helios}/Gauges/Common/ChequerBoard.xaml" : imageFile;
            _location = location;
            _size = size;
            _fov = FOV;

            _sphere3D = new GaugeBallSphere3DSnapshot
            {
                Width = _size.Width,
                Height = _size.Height,
                Top = _location.Y,
                Left = _location.X,
                SetTexture = ConfigManager.ImageManager.LoadImage(_imageFile, (int)_size.Width * 4, (int)_size.Height * 2),
                FieldOfView = _fov
            };
            BasePitch = basePitch;
            BaseRoll = baseRoll;
            BaseYaw = baseYaw;
            LightingColor = Colors.White;
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
                    _sphere3D.SetTexture = ConfigManager.ImageManager.LoadImage(value, (int)_size.Width * 4, (int)_size.Height * 2);
                    OnDisplayUpdate();
                }
            }
        }

        public double BasePitch
        {
            get
            {
                return _basePitch;
            }
            set
            {
                if (value != _basePitch)
                {
                    _basePitch = value;
                    _sphere3D.RotateX(_basePitch);
                    OnDisplayUpdate();
                }
            }
        }
        public double BaseRoll
        {
            get
            {
                return _baseRoll;
            }
            set
            {
                if (value != _baseRoll)
                {
                    _baseRoll = value;
                    _sphere3D.RotateZ(_baseRoll);
                    OnDisplayUpdate();
                }
            }
        }
        public double BaseYaw
        {
            get
            {
                return _baseYaw;
            }
            set
            {
                if (value != _baseYaw)
                {
                    _baseYaw = value;
                    _sphere3D.RotateY(_baseYaw);
                    OnDisplayUpdate();
                }
            }
        }
        public double Pitch
        {
            get
            {
                return _pitch;
            }
            set
            {
                if (value != _pitch)
                {
                    _pitch = value;
                    _sphere3D.RotateX(_pitch + _basePitch);
                    OnDisplayUpdate();
                }
            }
        }
        public double Roll
        {
            get
            {
                return _roll;
            }
            set
            {
                if (value != _roll)
                {
                    _roll = value;
                    _sphere3D.RotateZ(_roll + _baseRoll);
                    OnDisplayUpdate();
                }
            }
        }
        public double Yaw
        {
            get
            {
                return _yaw;
            }
            set
            {
                if (value != _yaw)
                {
                    _yaw = value;
                    _sphere3D.RotateY(_yaw + _baseYaw);
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
                    _sphere3D.FieldOfView = _fov;
                    OnDisplayUpdate();
                }
            }
        }
        public Color LightingColor
        {
            get => _sphere3D.LightingColor;
            set
            {
                if (value != _sphere3D.LightingColor)
                {
                    _sphere3D.LightingColor = value;
                    OnDisplayUpdate();
                }
            }
        }
        public Color LightingColorAlt
        {
            get => _sphere3D.LightingColorAlt;
            set
            {
                if (value != _sphere3D.LightingColorAlt)
                {
                    _sphere3D.LightingColorAlt = value;
                    OnDisplayUpdate();
                }
            }
        }
        public bool LightingAltEnabled
        {
            get => _sphere3D.LightingAltEnabled;
            set
            {
                if(value != _sphere3D.LightingAltEnabled)
                {
                    _sphere3D.LightingAltEnabled = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingX
        {
            get => _sphere3D.LightingX;
            set
            {
                if (value != _sphere3D.LightingX)
                {
                    _sphere3D.LightingX = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingY
        {
            get => _sphere3D.LightingY;
            set
            {
                if (value != _sphere3D.LightingY)
                {
                    _sphere3D.LightingY = value;
                    OnDisplayUpdate();
                }
            }
        }
        public double LightingZ
        {
            get => _sphere3D.LightingZ;
            set
            {
                if (value != _sphere3D.LightingZ)
                {
                    _sphere3D.LightingZ = value;
                    OnDisplayUpdate();
                }
            }
        }

        #endregion
        public void Reset()
        {
            Pitch = Roll = Yaw = 0d;
            LightingAltEnabled = false;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            _sphere3D.RedrawSnapshot(drawingContext);
        }

        protected override void OnRefresh(double xScale, double yScale)
        {
            if(_xScale != xScale || _yScale != yScale)
            {
                _xScale = xScale;
                _yScale = yScale;
                _sphere3D.Width = Math.Max(1d, _size.Width * xScale);
                _sphere3D.Height = Math.Max(1d, _size.Height * yScale);
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
        public void ScaleChildren(double scaleX, double scaleY)
        {
            _location.X *= scaleX;
            _location.Y *= scaleY;
            _sphere3D.Top = _location.Y;
            _sphere3D.Left = _location.X;
            OnDisplayUpdate();
        }

    }
}
