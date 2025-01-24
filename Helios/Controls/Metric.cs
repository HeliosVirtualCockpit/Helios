namespace GadrocsWorkshop.Helios.Controls
{
    using System;
    using System.Windows;

    public abstract class Metric : HeliosVisual
    {
        private string _metricImage;
        private double _rotation;
        private double _translationX;
        private double _translationY;

        private double _repeatDelay = 750d;
        private double _repeatRate = 200d;
        private int _lastRepeat = int.MinValue;
        private int _lastPulse = int.MinValue;
        private bool _repeating = false;
        private bool _increment = false;

        PulseType _pulseType = PulseType.None;

        private const double SWIPE_SENSITIVY_BASE = 20;
        private const double SWIPE_SENSITIVY_MODIFIER = 10;

        private bool _mouseDown = false;
        private Point _mouseDownLocation;

        protected LinearClickTypeDirectional _clickType = LinearClickTypeDirectional.Swipe;
        private CalibrationPointCollectionDouble _swipeCalibration;
        private double _swipeThreshold = 10;
        private double _sensitivity = 0d;

        private bool _clickableVertical = false;
        private bool _clickableHorizontal = false;

        private bool _dragOneForOne = false;

        protected Metric ( string name, Size defaultSize )
            : base( name, defaultSize )
        {
            _swipeCalibration = new CalibrationPointCollectionDouble( -1d, 2d, 1d, 0.5d );
            _swipeCalibration.Add( new CalibrationPointDouble( 0.0d, 1d ) );
        }

        #region Properties

        public LinearClickTypeDirectional ClickType
        {
            get
            {
                return _clickType;
            }
            set
            {
                if ( !_clickType.Equals( value ) )
                {
                    LinearClickTypeDirectional oldValue = _clickType;
                    _clickType = value;
                    OnPropertyChanged( "ClickType", oldValue, value, true );
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
                if ( !_sensitivity.Equals( value ) )
                {
                    double oldValue = _sensitivity;
                    _sensitivity = value;
                    _swipeThreshold = SWIPE_SENSITIVY_BASE + (_sensitivity * SWIPE_SENSITIVY_MODIFIER * -1);
                    OnPropertyChanged( "Sensitivity", oldValue, value, true );
                }
            }
        }

        public string MetricImage
        {
            get
            {
                return _metricImage;
            }
            set
            {
                if ( (_metricImage == null && value != null)
                    || (_metricImage != null && !_metricImage.Equals( value )) )
                {
                    string oldValue = _metricImage;
                    _metricImage = value;
                    OnPropertyChanged( "MetricImage", oldValue, value, true );
                    Refresh( );
                }
            }
        }

        public double MetricRotation
        {
            get
            {
                return _rotation;
            }
            protected set
            {
                if ( !_rotation.Equals( value ) )
                {
                    double oldValue = _rotation;
                    _rotation = value % 360d;
                    OnPropertyChanged( "MetricRotation", oldValue, value, false );
                    OnDisplayUpdate( );
                }
            }
        }

        public double MetricTranslationX
        {
            get
            {
                return _translationX;
            }
            protected set
            {
                if ( !_translationX.Equals( value ) )
                {
                    double oldValue = _translationX;
                    //_translation = value % 360d;
                    _translationX = value;
                    OnPropertyChanged( "MetricTranslationX", oldValue, value, false );
                    OnDisplayUpdate( );
                }
            }
        }

        public double MetricTranslationY
        {
            get
            {
                return _translationY;
            }
            protected set
            {
                if ( !_translationY.Equals( value ) )
                {
                    double oldValue = _translationY;
                    //_translation = value % 360d;
                    _translationY = value;
                    OnPropertyChanged( "MetricTranslationY", oldValue, value, false );
                    OnDisplayUpdate( );
                }
            }
        }

        public double RepeatDelay
        {
            get
            {
                return _repeatDelay;
            }
            set
            {
                if ( !_repeatDelay.Equals( value ) )
                {
                    double oldValue = _repeatDelay;
                    _repeatDelay = value;
                    OnPropertyChanged( "RepeatDelay", oldValue, value, true );
                }
            }
        }

        public double RepeatRate
        {
            get
            {
                return _repeatRate;
            }
            set
            {
                if ( !_repeatRate.Equals( value ) )
                {
                    double oldValue = _repeatRate;
                    _repeatRate = value;
                    OnPropertyChanged( "RepeatRate", oldValue, value, true );
                }
            }
        }

        public bool ClickableVertical
        {
            get
            {
                return _clickableVertical;
            }
            set
            {
                _clickableVertical = value;
            }
        }

        public bool ClickableHorizontal
        {
            get
            {
                return _clickableHorizontal;
            }
            set
            {
                _clickableHorizontal = value;
            }
        }

        public bool DragOneForOne
        {
            get
            {
                return _dragOneForOne;
            }
            set
            {
                _dragOneForOne = value;
            }
        }

        #endregion

        protected abstract Point GetCurrentTranslation ( );

        protected abstract void Pulse ( PulseType type, bool increment );

        private Vector VectorFromCenter ( Point devicePosition )
        {
            return devicePosition - new Point( DisplayRectangle.Width / 2, DisplayRectangle.Height / 2 );
        }

        private double GetAngle ( Point startPoint, Point endPoint )
        {
            return Vector.AngleBetween( VectorFromCenter( startPoint ), VectorFromCenter( endPoint ) );
        }

        public override void MouseDown ( Point location )
        {
            if ( _clickType == LinearClickTypeDirectional.Touch )
            {
                if ( _clickableVertical )
                {
                    _increment = (location.Y > (this.GetCurrentTranslation( ).Y + Height / 2d));
                    _pulseType = PulseType.Vertical;

                    Pulse( _pulseType, _increment );
                }
                else if ( _clickableHorizontal )
                {
                    _increment = (location.X > (this.GetCurrentTranslation( ).X + Width / 2d));
                    _pulseType = PulseType.Horizontal;
                    Pulse( _pulseType, _increment );
                }

                //_repeating = false;
                ////_repeatRate = 200d;
                //_repeatRate = 0d;
                //_lastRepeat = Environment.TickCount & Int32.MaxValue;

                //if ( Parent != null && Parent.Profile != null )
                //{
                //    Parent.Profile.ProfileTick += new EventHandler( Profile_ProfileTick );
                //}
            }
            else if ( _clickType == LinearClickTypeDirectional.Swipe )
            {
                _mouseDown = true;
                _mouseDownLocation = location;
            }
        }

        void Profile_ProfileTick ( object sender, EventArgs e )
        {
            int currentTick = Environment.TickCount & Int32.MaxValue;

            if ( _repeating && (currentTick < _lastPulse || (currentTick - _lastPulse > _repeatRate)) )
            {
                Pulse( _pulseType, _increment );
                _lastPulse = currentTick;
            }

            if ( currentTick < _lastRepeat || (currentTick - _lastRepeat > _repeatDelay) )
            {
                if ( _repeating && _repeatRate > 33 )
                {
                    _repeatRate = _repeatRate / 2;
                    if ( _repeatRate < 33 ) _repeatRate = 33;
                }
                //Pulse( _pulseType, _increment );
                _lastPulse = currentTick;
                _lastRepeat = currentTick;
                _repeating = true;
            }
        }

        public override void MouseDrag ( Point location )
        {
            if ( _mouseDown && _clickType == LinearClickTypeDirectional.Swipe )
            {
                if ( DragOneForOne )
                {
                    double increment = 0;
                    PulseType type = PulseType.None;

                    if ( _clickableVertical )
                    {
                        increment = location.Y - _mouseDownLocation.Y;
                        type = PulseType.Vertical;

                    }
                    else if ( _clickableHorizontal )
                    {
                        increment = location.X - _mouseDownLocation.X;
                        type = PulseType.Horizontal;
                    }

                    var numberSteps = CalculateNumberSteps( type, increment );
                    for ( int i = 0; i < Math.Round( Math.Abs( numberSteps ) ); i++ )
                    {
                        Pulse( type, increment > 0 );
                    }
                    _mouseDownLocation = location;
                }
                else
                {
                    if ( _clickableVertical )
                    {
                        var increment = location.Y - _mouseDownLocation.Y;
                        if ( (increment > 0 && increment > _swipeThreshold) || (increment < 0 && (increment * -1) > _swipeThreshold) )
                        {
                            Pulse( PulseType.Vertical, increment > 0 );
                            _mouseDownLocation = location;
                        }
                    }
                    else if ( _clickableHorizontal )
                    {
                        var increment = location.X - _mouseDownLocation.X;
                        if ( (increment > 0 && increment > _swipeThreshold) || (increment < 0 && (increment * -1) > _swipeThreshold) )
                        {
                            Pulse( PulseType.Horizontal, increment > 0 );
                            _mouseDownLocation = location;
                        }
                    }
                }
            }
        }

        public override void MouseUp ( Point location )
        {
            _mouseDown = false;
            if ( Parent != null && Parent.Profile != null )
            {
                Parent.Profile.ProfileTick -= new EventHandler( Profile_ProfileTick );
            }
        }

        protected abstract double CalculateNumberSteps ( PulseType type, double increment );
    }
}
