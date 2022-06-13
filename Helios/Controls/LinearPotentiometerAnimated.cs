//  Copyright 2014 Craig Courtney
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
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Collections.Generic;
    using System.Xml;
    using System.Drawing;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Size = System.Windows.Size;
    using Point = System.Windows.Point;

    [HeliosControl("Helios.Base.LinearPotentiometerAnimated", "Linear Potentiometer (Animated)", "Potentiometers", typeof( KineographRenderer ) )]
    public class LinearPotentiometerAnimated : HeliosVisual, IKineographControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private double _initialValue = 0.0d;
        private double _stepValue = 0.05d;
        private double _minValue = 0d;
        private double _maxValue = 1d;
        private double _initialPosition = 0d;

        private bool _invertedHorizontal = false;
        private bool _invertedVertical = false;

        private bool _clickableVertical = false;
        private bool _clickableHorizontal = false;
        LinearClickType _clickType = LinearClickType.Swipe;

        private HeliosValue _linearPotentiometer;

        private bool _mouseDown = false;
        private Point _mouseDownLocation;
        private double _swipeThreshold = 10d;
        private double _sensitivity = 0d;
        private const double SWIPE_SENSITIVY_BASE = 20d;
        private const double SWIPE_SENSITIVY_MODIFIER = 10d;

        private List<ImageSource> _animationFrames = new List<ImageSource>();
        private String _animationFrameImageNamePattern;
        private int _animationFrameNumber = 0;
        private int _animationFrameCount = 0;
        private Bitmap _animationFrameBitmap =null;
        private bool _animationIsPng = false;

        public LinearPotentiometerAnimated( )
            : base( "Linear Potentiometer (Animated)", new Size( 73, 240 ) )
        {
            AnimationFrameImageNamePattern ="{Helios}/Images/AH-64D/Power/Lever_0.png";
            AnimationFrameNumber = AnimationFrameCount-1;
            _clickableVertical = true;
            ClickType = LinearClickType.Swipe;

            _linearPotentiometer = new HeliosValue(this, new BindingValue(0d), "", "value", "Current value of potentiometer", "0 to 1", BindingValueUnits.Numeric);
            _linearPotentiometer.SetValue(new BindingValue(0), true);
            _linearPotentiometer.Execute += new HeliosActionHandler(SetValuePotentionmeter_Execute);
            Values.Add(_linearPotentiometer);
            Actions.Add(_linearPotentiometer);
            Triggers.Add(_linearPotentiometer);
        }

        #region Properties
        public double InitialValue
        {
            get
            {
                return _initialValue;
            }
            set
            {
                if (!_initialValue.Equals(value))
                {
                    double oldValue = _initialValue;
                    _initialValue = value;
                    OnPropertyChanged("InitialValue", oldValue, value, true);
                }
            }
        }

        public double MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                if (!_minValue.Equals(value))
                {
                    double oldValue = _minValue;
                    _minValue = value;
                    OnPropertyChanged("MinValue", oldValue, value, true);
                }
            }
        }

        public double MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if (!_maxValue.Equals(value))
                {
                    double oldValue = _maxValue;
                    _maxValue = value;
                    OnPropertyChanged("MaxValue", oldValue, value, true);
                }
            }
        }

        public double StepValue
        {
            get
            {
                return _stepValue;
            }
            set
            {
                if (!_stepValue.Equals(value))
                {
                    double oldValue = _stepValue;
                    _stepValue = value;
                    OnPropertyChanged("StepValue", oldValue, value, true);
                }
            }
        }
        public bool AnimationIsPng
        {
            get => _animationIsPng;
            set => _animationIsPng = value; 
        }

        /// <summary>
        /// UI access to current value, backed by Helios value of the potentiometer
        ///
        /// writes to this property do not create Undo events
        /// </summary>
        public double Value
        {
            get
            {
                return _linearPotentiometer.Value.DoubleValue;
            }
            set
            {
                if (_linearPotentiometer.Value.DoubleValue.Equals(value))
                {
                    return;
                }

                double oldValue = _linearPotentiometer.Value.DoubleValue;
                _linearPotentiometer.SetValue(new BindingValue(value), BypassTriggers);
                OnPropertyChanged("Value", oldValue, value, false);
            }
        }

        public double InitialPosition
        {
            get
            {
                return _initialPosition;
            }
            set
            {
                if (!_initialPosition.Equals(value))
                {
                    double oldValue = _initialPosition;
                    _initialPosition = value;
                    OnPropertyChanged("InitialPosition", oldValue, value, true);
                }
            }
        }

        public bool InvertedHorizontal
        {
            get
            {
                return _invertedHorizontal;
            }
            set
            {
                this._invertedHorizontal = value;
            }
        }

        public bool InvertedVertical
        {
            get
            {
                return _invertedVertical;
            }
            set
            {
                this._invertedVertical = value;
            }
        }
        public LinearClickType ClickType
        {
            get
            {
                return _clickType;
            }
            set
            {
                if (!_clickType.Equals(value))
                {
                    LinearClickType oldValue = _clickType;
                    _clickType = value;
                    OnPropertyChanged("ClickType", oldValue, value, true);
                }
            }
        }

        public double Sensitivity
        {
            get
            {
                return _sensitivity;
            }
            set
            {
                if (!_sensitivity.Equals(value))
                {
                    double oldValue = _sensitivity;
                    _sensitivity = value;
                    _swipeThreshold = SWIPE_SENSITIVY_BASE + (_sensitivity * SWIPE_SENSITIVY_MODIFIER * -1);
                    OnPropertyChanged("Sensitivity", oldValue, value, true);
                }
            }
        }
        public String AnimationFrameImageNamePattern
        {
            get => _animationFrameImageNamePattern;
            set
            {
                if ((_animationFrameImageNamePattern == null && value != null)
                    || (_animationFrameImageNamePattern != null && !_animationFrameImageNamePattern.Equals(value)))
                {
                    String oldValue = _animationFrameImageNamePattern;
                    _animationFrameImageNamePattern = value;
                    OnPropertyChanged("AnimationFrameImageNamePattern", oldValue, value, true);
                    Refresh();
                }
            }
        }
        public int AnimationFrameNumber
        {
            get => _animationFrameNumber;
            set
            {
                if (_animationFrameNumber != value)
                {
                    int oldValue = _animationFrameNumber;
                    _animationFrameNumber = value;
                    OnPropertyChanged("AnimationFrameNumber", oldValue, value, true);
                    Refresh();
                    // we test for transparency in HitTest so we need a bitmap
                    // from the image source
                    if (AnimationFrames.Count > 0) {
                        _animationFrameBitmap = BitmapImage2Bitmap(AnimationFrames[AnimationFrameNumber]);
                    }
                }
            }
        }
        public int AnimationFrameCount
        {
            get => _animationFrames.Count;
            set
            {
                int oldValue = _animationFrameCount;
                _animationFrameCount = value;
                OnPropertyChanged("AnimationFrameCount", oldValue, value, true);
            }
        }
        public List<ImageSource> AnimationFrames
        {
            get => _animationFrames;
            set => _animationFrames = value;
        }

        #endregion

        #region Actions
        void SetValuePotentionmeter_Execute(object action, HeliosActionEventArgs e)
        {
            double maxImage = AnimationFrameCount - 1;
            //_linearPotentiometer.SetValue(new BindingValue(Clamp(Math.Round(e.Value.DoubleValue * maxImage), 0, maxImage)), true);
            //AnimationFrameNumber = Convert.ToInt32(_linearPotentiometer.Value.DoubleValue);
            Value = e.Value.DoubleValue;
            AnimationFrameNumber = Convert.ToInt32(Clamp(Math.Round(e.Value.DoubleValue * maxImage), 0, maxImage));
        }

        #endregion

        protected void CalculateMovement(double pulses)
        {
            double dragProportion = pulses / this.Height * MaxValue * -1;
            Value = Math.Round(Math.Max(Math.Min(Value + dragProportion, MaxValue), MinValue), 3);
            AnimationFrameNumber = Convert.ToInt32(Clamp(Math.Round(Value * (AnimationFrameCount - 1)), 0, AnimationFrameCount - 1));
        }

        public override void Reset ( )
        {
            base.Reset();

            BeginTriggerBypass(true);
            Value = InitialValue;
            EndTriggerBypass(true);
        }

        public override void ScaleChildren ( double scaleX, double scaleY )
        {
            base.ScaleChildren( scaleX, scaleY );

            if ( this.Left < 0 )
            {
                this.Left = this.Left * scaleX;
            }
            if ( this.Top < 0 )
            {
                this.Top = this.Top * scaleY;
            }
        }

         public override void WriteXml ( XmlWriter writer )
        {
            base.WriteXml( writer );
            writer.WriteElementString("AnimationFrameImageNamePattern", AnimationFrameImageNamePattern);
            writer.WriteElementString("InitialValue", InitialValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("StepValue", StepValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("MaxValue", MaxValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("MinValue", MinValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("InitialPosition", InitialPosition.ToString(CultureInfo.InvariantCulture));

            writer.WriteStartElement( "ClickType" );
            writer.WriteElementString( "Type", ClickType.ToString( ) );
            if ( ClickType == LinearClickType.Swipe )
            {
                writer.WriteElementString( "Sensitivity", Sensitivity.ToString( CultureInfo.InvariantCulture ) );
            }
            writer.WriteEndElement( );
        }

        public override void ReadXml ( XmlReader reader )
        {
            base.ReadXml( reader );
            AnimationFrameImageNamePattern = reader.ReadElementString("AnimationFrameImageNamePattern");
            InitialValue = double.Parse(reader.ReadElementString("InitialValue"), CultureInfo.InvariantCulture);
            StepValue = double.Parse(reader.ReadElementString("StepValue"), CultureInfo.InvariantCulture);
            MaxValue = double.Parse(reader.ReadElementString("MaxValue"), CultureInfo.InvariantCulture);
            MinValue = double.Parse(reader.ReadElementString("MinValue"), CultureInfo.InvariantCulture);
            InitialPosition = double.Parse(reader.ReadElementString("InitialPosition"), CultureInfo.InvariantCulture);

            if ( reader.Name.Equals( "ClickType" ) )
            {
                reader.ReadStartElement( "ClickType" );
                ClickType = (LinearClickType)Enum.Parse( typeof( LinearClickType ), reader.ReadElementString( "Type" ) );
                if (reader.Name == "Sensitivity")
                {
                    Sensitivity = double.Parse( reader.ReadElementString( "Sensitivity" ), CultureInfo.InvariantCulture );
                }
                reader.ReadEndElement( );
            }
            else
            {
                ClickType = LinearClickType.Swipe;
                Sensitivity = 0d;
            }
            BeginTriggerBypass( true );
            Value = InitialValue;
            EndTriggerBypass( true );
        }
        public override void MouseDown(Point location)
        {
            if (_clickType == LinearClickType.Swipe)
            {
                _mouseDown = true;
                _mouseDownLocation = location;
            }
        }

        public override void MouseDrag(Point location)
        {
            if (_mouseDown && _clickType == LinearClickType.Swipe)
            {
                if (_clickableVertical)
                {
                    double increment = location.Y - _mouseDownLocation.Y;
                    if ((increment > 0 && increment > _swipeThreshold) || (increment < 0 && (increment * -1) > _swipeThreshold))
                    {
                        CalculateMovement(increment);
                        _mouseDownLocation = location;
                    }
                }
                else if (_clickableHorizontal)
                {
                    double increment = location.X - _mouseDownLocation.X;
                    if ((increment > 0 && increment > _swipeThreshold) || (increment < 0 && (increment * -1) > _swipeThreshold))
                    {
                        CalculateMovement(increment);
                        _mouseDownLocation = location;
                    }
                }
            }
        }
        public override void MouseUp(Point location)
        {
            _mouseDown = false;
        }
        public override bool HitTest(Point location)
        {
            // The bitmap is unscaled so we adjust the location to be tested
            if (_animationFrameBitmap != null)
            {
                return IsTransparent(AdjustLocation(location, _animationFrameBitmap.Size, new Size(this.Width, this.Height)), _animationFrameBitmap);
            } else
            {
                return true;
            }
        }
        private bool IsTransparent(Point location, Bitmap bitmap)
        {
            if(_animationFrameBitmap != null)
            {
                switch (_animationFrameBitmap.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                        return _animationFrameBitmap.GetPixel(Convert.ToInt32(location.X), Convert.ToInt32(location.Y)).A != 0;
                    default:
                        return true;
                }
            }
            else
            {
                return true;
            }
        }
        private Point AdjustLocation(Point location, System.Drawing.Size bitmapSize, Size visualSize)
        {
            Point testPoint = new Point();
            testPoint.X = Math.Round(Clamp(location.X * bitmapSize.Width / visualSize.Width, 0, bitmapSize.Width - 1));
            testPoint.Y = Math.Round(Clamp(location.Y * bitmapSize.Height / visualSize.Height, 0, bitmapSize.Height - 1));
            return testPoint;
        }
        private double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value ;
        }
        private Bitmap BitmapImage2Bitmap(ImageSource imageSource)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                /// TODO: expand to include other formats               
                BitmapEncoder enc;
                if (_animationIsPng)
                {
                    enc = new PngBitmapEncoder();
                }
                else
                {
                    ///default to a BMP encoder
                    enc = new BmpBitmapEncoder();
                }
                try
                {
                    enc.Frames.Add(BitmapFrame.Create(imageSource as BitmapImage));
                    enc.Save(outStream);
                    if (_animationFrameBitmap != null)
                    {
                        _animationFrameBitmap.Dispose();
                    }
                    Bitmap bm = new Bitmap(outStream);
                    outStream.Dispose();
                    return bm;
                }
                catch {
                    Logger.Warn($"{Name} Unable to convert new animation frame to bitmap for use in transparency testing.");
                    return null;
                }
                
            }
        }
    }
}
