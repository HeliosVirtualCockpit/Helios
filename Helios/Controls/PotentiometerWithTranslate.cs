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
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Xml;

    // XXX this is something else entirely.  this should be replaced with AffineTransformImage descendants along with an input-only linear pot
    [HeliosControl("Helios.Base.PotentiometerWithTranslate", "Linear Potentiometer", "Potentiometers", typeof( MetricRenderer ) )]
    public class PotentiometerWithTranslate : Metric
    {
        private double _valueRotation = 0.0d;
        private double _valueTranslationX = 0.0d;
        private double _valueTranslationY = 0.0d;

        private double _initialValueRotation = 0.0d;
        private double _stepValueRotation = 0.1d;
        private double _minValueRotation = 0d;
        private double _maxValueRotation = 1d;

        private double _initialValueTranslationX = 0.0d;
        private double _stepValueTranslationX = 0.1d;
        private double _minValueTranslationX = 0d;
        private double _maxValueTranslationX = 0d;

        private double _initialValueTranslationY = 0.0d;
        private double _stepValueTranslationY = 0.1d;
        private double _minValueTranslationY = 0d;
        private double _maxValueTranslationY = 0d;

        private double _initialRotation = 0d;
        private double _rotationTravel = 360d;

        private double _initialTranslationX = 0.0d;
        private double _translationTravelX = 1.0d;

        private double _initialTranslationY = 0.0d;
        private double _translationTravelY = 1.0d;

        private bool _invertedHorizontal = false;
        private bool _invertedVertical = false;

        private HeliosValue _pottranValueRotation;
        private HeliosValue _pottranValueTranslationX;
        private HeliosValue _pottranValueTranslationY;

        public PotentiometerWithTranslate( )
            : base( "Linear Potentiometer", new Size( 100, 100 ) )
        {
            MetricImage = "{Helios}/Images/Custom/translate.png";

            _pottranValueRotation = new HeliosValue( this, new BindingValue( 0d ), "", "value rotation", "Current rotation value of the metric.", "", BindingValueUnits.Numeric );
            _pottranValueRotation.Execute += new HeliosActionHandler( SetValueRotation_Execute );
            Values.Add( _pottranValueRotation );
            Actions.Add( _pottranValueRotation );
            Triggers.Add( _pottranValueRotation );

            _pottranValueTranslationX = new HeliosValue( this, new BindingValue( 0d ), "", "value Translation X", "Current Translation X value of the metric.", "", BindingValueUnits.Numeric );
            _pottranValueTranslationX.Execute += new HeliosActionHandler( SetValueTranslationX_Execute );
            Values.Add( _pottranValueTranslationX );
            Actions.Add( _pottranValueTranslationX );
            Triggers.Add( _pottranValueTranslationX );

            _pottranValueTranslationY = new HeliosValue( this, new BindingValue( 0d ), "", "value Translation Y", "Current Translation Y value of the metric.", "", BindingValueUnits.Numeric );
            _pottranValueTranslationY.Execute += new HeliosActionHandler( SetValueTranslationY_Execute );
            Values.Add( _pottranValueTranslationY );
            Actions.Add( _pottranValueTranslationY );
            Triggers.Add( _pottranValueTranslationY );

        }

        #region Properties

        public double InitialValueRotation
        {
            get
            {
                return _initialValueRotation;
            }
            set
            {
                if ( !_initialValueRotation.Equals( value ) )
                {
                    double oldValue = _initialValueRotation;
                    _initialValueRotation = value;
                    OnPropertyChanged( "InitialValueRotation", oldValue, value, true );
                }
            }
        }

        public double MinValueRotation
        {
            get
            {
                return _minValueRotation;
            }
            set
            {
                if ( !_minValueRotation.Equals( value ) )
                {
                    double oldValue = _minValueRotation;
                    _minValueRotation = value;
                    OnPropertyChanged( "MinValueRotation", oldValue, value, true );
                    SetRotation( );
                }
            }
        }

        public double MaxValueRotation
        {
            get
            {
                return _maxValueRotation;
            }
            set
            {
                if ( !_maxValueRotation.Equals( value ) )
                {
                    double oldValue = _maxValueRotation;
                    _maxValueRotation = value;
                    OnPropertyChanged( "MaxValueRotation", oldValue, value, true );
                    SetRotation( );
                }
            }
        }

        public double StepValueRotation
        {
            get
            {
                return _stepValueRotation;
            }
            set
            {
                if ( !_stepValueRotation.Equals( value ) )
                {
                    double oldValue = _stepValueRotation;
                    _stepValueRotation = value;
                    OnPropertyChanged( "StepValueRotation", oldValue, value, true );
                }
            }
        }

        public double ValueRotation
        {
            get
            {
                return _valueRotation;
            }
            set
            {
                if ( !_valueRotation.Equals( value ) )
                {
                    double oldValue = _valueRotation;
                    _valueRotation = value;
                    _pottranValueRotation.SetValue( new BindingValue( _valueRotation ), BypassTriggers );
                    OnPropertyChanged( "ValueRotation", oldValue, value, false );
                    SetRotation( );
                }
            }
        }




        public double InitialValueTranslationX
        {
            get
            {
                return _initialValueTranslationX;
            }
            set
            {
                if ( !_initialValueTranslationX.Equals( value ) )
                {
                    double oldValue = _initialValueTranslationX;
                    _initialValueTranslationX = value;
                    OnPropertyChanged( "InitialValueTranslationX", oldValue, value, true );
                }
            }
        }

        public double MinValueTranslationX
        {
            get
            {
                return _minValueTranslationX;
            }
            set
            {
                if ( !_minValueTranslationX.Equals( value ) )
                {
                    double oldValue = _minValueTranslationX;
                    _minValueTranslationX = value;
                    OnPropertyChanged( "MinValueTranslationX", oldValue, value, true );
                    SetTranslationX( );
                }
            }
        }

        public double MaxValueTranslationX
        {
            get
            {
                return _maxValueTranslationX;
            }
            set
            {
                if ( !_maxValueTranslationX.Equals( value ) )
                {
                    double oldValue = _maxValueTranslationX;
                    _maxValueTranslationX = value;
                    OnPropertyChanged( "MaxValueTranslationX", oldValue, value, true );
                    SetTranslationX( );
                }
            }
        }

        public double StepValueTranslationX
        {
            get
            {
                return _stepValueTranslationX;
            }
            set
            {
                if ( !_stepValueTranslationX.Equals( value ) )
                {
                    double oldValue = _stepValueTranslationX;
                    _stepValueTranslationX = value;
                    OnPropertyChanged( "StepValueTranslationX", oldValue, value, true );
                }
            }
        }

        public double ValueTranslationX
        {
            get
            {
                return _valueTranslationX;
            }
            set
            {
                if ( !_valueTranslationX.Equals( value ) )
                {
                    double oldValue = _valueTranslationX;
                    _valueTranslationX = value;
                    _pottranValueTranslationX.SetValue( new BindingValue( _valueTranslationX ), BypassTriggers );
                    OnPropertyChanged( "ValueTranslationX", oldValue, value, false );
                    SetTranslationX( );
                }
            }
        }


        public double InitialValueTranslationY
        {
            get
            {
                return _initialValueTranslationY;
            }
            set
            {
                if ( !_initialValueTranslationY.Equals( value ) )
                {
                    double oldValue = _initialValueTranslationY;
                    _initialValueTranslationY = value;
                    OnPropertyChanged( "InitialValueTranslationY", oldValue, value, true );
                }
            }
        }

        public double MinValueTranslationY
        {
            get
            {
                return _minValueTranslationY;
            }
            set
            {
                if ( !_minValueTranslationY.Equals( value ) )
                {
                    double oldValue = _minValueTranslationY;
                    _minValueTranslationY = value;
                    OnPropertyChanged( "MinValueTranslationY", oldValue, value, true );
                    SetTranslationY( );
                }
            }
        }

        public double MaxValueTranslationY
        {
            get
            {
                return _maxValueTranslationY;
            }
            set
            {
                if ( !_maxValueTranslationY.Equals( value ) )
                {
                    double oldValue = _maxValueTranslationY;
                    _maxValueTranslationY = value;
                    OnPropertyChanged( "MaxValueTranslationY", oldValue, value, true );
                    SetTranslationY( );
                }
            }
        }

        public double StepValueTranslationY
        {
            get
            {
                return _stepValueTranslationY;
            }
            set
            {
                if ( !_stepValueTranslationY.Equals( value ) )
                {
                    double oldValue = _stepValueTranslationY;
                    _stepValueTranslationY = value;
                    OnPropertyChanged( "StepValueTranslationY", oldValue, value, true );
                }
            }
        }

        public double ValueTranslationY
        {
            get
            {
                return _valueTranslationY;
            }
            set
            {
                if ( !_valueTranslationY.Equals( value ) )
                {
                    double oldValue = _valueTranslationY;
                    _valueTranslationY = value;
                    _pottranValueTranslationY.SetValue( new BindingValue( _valueTranslationY ), BypassTriggers );
                    OnPropertyChanged( "ValueTranslationY", oldValue, value, false );
                    SetTranslationY( );
                }
            }
        }




        public double InitialRotation
        {
            get
            {
                return _initialRotation;
            }
            set
            {
                if ( !_initialRotation.Equals( value ) )
                {
                    double oldValue = _initialRotation;
                    _initialRotation = value;
                    OnPropertyChanged( "InitialRotation", oldValue, value, true );
                    SetRotation( );
                }
            }
        }

        public double InitialTranslationX
        {
            get
            {
                return _initialTranslationX;
            }
            set
            {
                if ( !_initialTranslationX.Equals( value ) )
                {
                    double oldValue = _initialTranslationX;
                    _initialTranslationX = value;
                    OnPropertyChanged( "InitialTranslationX", oldValue, value, true );
                    SetTranslationX( );
                }
            }
        }

        public double InitialTranslationY
        {
            get
            {
                return _initialTranslationY;
            }
            set
            {
                if ( !_initialTranslationY.Equals( value ) )
                {
                    double oldValue = _initialTranslationY;
                    _initialTranslationY = value;
                    OnPropertyChanged( "InitialTranslationY", oldValue, value, true );
                    SetTranslationY( );
                }
            }
        }

        public double RotationTravel
        {
            get
            {
                return _rotationTravel;
            }
            set
            {
                if ( !_rotationTravel.Equals( value ) )
                {
                    double oldValue = _rotationTravel;
                    _rotationTravel = value;
                    OnPropertyChanged( "RotationTravel", oldValue, value, true );
                    SetRotation( );
                }
            }
        }

        public double TranslationTravelX
        {
            get
            {
                return _translationTravelX;
            }
            set
            {
                if ( !_translationTravelX.Equals( value ) )
                {
                    double oldValue = _translationTravelX;
                    _translationTravelX = value;
                    OnPropertyChanged( "TranslationTravelX", oldValue, value, true );
                    SetTranslationX( );
                }
            }
        }

        public double TranslationTravelY
        {
            get
            {
                return _translationTravelY;
            }
            set
            {
                if ( !_translationTravelY.Equals( value ) )
                {
                    double oldValue = _translationTravelY;
                    _translationTravelY = value;
                    OnPropertyChanged( "TranslationTravelY", oldValue, value, true );
                    SetTranslationY( );
                }
            }
        }

        public bool InvertedHorizontal
        {
            get  => _invertedHorizontal;
            set
            {
                if(_invertedHorizontal != value)
                {
                    _invertedHorizontal = value;
                }
            }
        }

        public bool InvertedVertical
        {
            get => _invertedVertical;
            set
            {
                if (_invertedVertical != value)
                {
                    _invertedVertical = value;
                }
           }
        }



        #endregion

        #region Actions

        void SetValueRotation_Execute ( object action, HeliosActionEventArgs e )
        {
            try
            {
                BeginTriggerBypass( e.BypassCascadingTriggers );
                ValueRotation = e.Value.DoubleValue;
                EndTriggerBypass( e.BypassCascadingTriggers );
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

        void SetValueTranslationX_Execute ( object action, HeliosActionEventArgs e )
        {
            try
            {
                BeginTriggerBypass(e.BypassCascadingTriggers);
                ValueTranslationX = e.Value.DoubleValue; ;
                EndTriggerBypass( e.BypassCascadingTriggers );
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

        void SetValueTranslationY_Execute ( object action, HeliosActionEventArgs e )
        {
            try
            {
                BeginTriggerBypass( e.BypassCascadingTriggers );
                ValueTranslationY = e.Value.DoubleValue;
                EndTriggerBypass( e.BypassCascadingTriggers );
            }
            catch
            {
                // No-op if the parse fails we won't set the position.
            }
        }

        #endregion

        private void SetRotation ( )
        {
            MetricRotation = InitialRotation + (((ValueRotation - MinValueRotation) / (MaxValueRotation - MinValueRotation)) * RotationTravel);
        }

        private void SetTranslationX ( )
        {
            //MetricTranslation = InitialTranslation + (((ValueTranslation - MinValueTranslation) / (MaxValueTranslation - MinValueTranslation)) * TranslationTravel);

            if ( ValueTranslationX >= 0 )
            {
                MetricTranslationX = MaxValueTranslationX * (InvertedHorizontal ? (1 - ValueTranslationX) : ValueTranslationX); ;
            }
            else
            {
                MetricTranslationX = MinValueTranslationX * (InvertedHorizontal ? (1 - ValueTranslationX) : ValueTranslationX); ;
            }
        }

        private void SetTranslationY ( )
        {
            //MetricTranslation = InitialTranslation + (((ValueTranslation - MinValueTranslation) / (MaxValueTranslation - MinValueTranslation)) * TranslationTravel);

            if ( ValueTranslationY >= 0 )
            {
                MetricTranslationY = MaxValueTranslationY * (InvertedVertical ? (1 - ValueTranslationY) : ValueTranslationY);
            }
            else
            {
                MetricTranslationY = MinValueTranslationY * (InvertedVertical ? (1 - ValueTranslationY) : ValueTranslationY);
            }
        }

        protected override Point GetCurrentTranslation ( )
        {
            return new Point( MetricTranslationX, MetricTranslationY );
        }

        protected override void Pulse ( PulseType type, bool increment )
        {
            if ( (increment && !InvertedHorizontal && !InvertedVertical) || (!increment && (InvertedHorizontal || InvertedVertical)) )
            {
                if ( type == PulseType.Vertical )
                {
                    ValueTranslationY = Math.Min( ValueTranslationY + _stepValueTranslationY, 1 );

                }
                else if ( type == PulseType.Horizontal )
                {
                    ValueTranslationX = Math.Min( ValueTranslationX + _stepValueTranslationX, 1 );
                }
            }
            else
            {
                if ( type == PulseType.Vertical )
                {
                    ValueTranslationY = Math.Max( ValueTranslationY - _stepValueTranslationY, 0 );

                }
                else if ( type == PulseType.Horizontal )
                {
                    ValueTranslationX = Math.Max( ValueTranslationX - _stepValueTranslationX, 0 );
                }
            }
        }

        public override void Reset ( )
        {
            base.Reset();

            BeginTriggerBypass( true );
            ValueRotation = InitialValueRotation;
            ValueTranslationX = InitialValueTranslationX;
            ValueTranslationY = InitialValueTranslationY;
            EndTriggerBypass( true );
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

            this.MaxValueTranslationX = this.MaxValueTranslationX * scaleX;
            this.MinValueTranslationX = this.MinValueTranslationX * scaleX;
            this.MaxValueTranslationY = this.MaxValueTranslationY * scaleY;
            this.MinValueTranslationY = this.MinValueTranslationY * scaleY;
        }

        protected override double CalculateNumberSteps ( PulseType type, double increment )
        {
            double maxValueTranslation = type == PulseType.Horizontal ? MaxValueTranslationX : MaxValueTranslationY;
            double minValueTranslation = type == PulseType.Horizontal ? MinValueTranslationX : MinValueTranslationY;
            double stepValueTranslation = type == PulseType.Horizontal ? StepValueTranslationX : StepValueTranslationY;

            double number = increment / ((maxValueTranslation - minValueTranslation) * stepValueTranslation);

            return number;
        }

        public override void WriteXml ( XmlWriter writer )
        {
            base.WriteXml( writer );
            writer.WriteElementString( "MetricImage", MetricImage );
            writer.WriteElementString( "InitialValueRotation", InitialValueRotation.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "StepValueRotation", StepValueRotation.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "MaxValueRotation", MaxValueRotation.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "MinValueRotation", MinValueRotation.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "InitialRotation", InitialRotation.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "RotationTravel", RotationTravel.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "InitialValueTranslationX", InitialValueTranslationX.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "StepValueTranslationX", StepValueTranslationX.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "MaxValueTranslationX", MaxValueTranslationX.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "MinValueTranslationX", MinValueTranslationX.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "InitialTranslationX", InitialTranslationX.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "TranslationTravelX", TranslationTravelX.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "InitialValueTranslationY", InitialValueTranslationY.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "StepValueTranslationY", StepValueTranslationY.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "MaxValueTranslationY", MaxValueTranslationY.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "MinValueTranslationY", MinValueTranslationY.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "InitialTranslationY", InitialTranslationY.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "TranslationTravelY", TranslationTravelY.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "ClickableVertical", ClickableVertical.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "ClickableHorizontal", ClickableHorizontal.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "InvertedHorizontal", InvertedHorizontal.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "InvertedVertical", InvertedVertical.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "DragOneOnOne", DragOneForOne.ToString( CultureInfo.InvariantCulture ) );

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
            MetricImage = reader.ReadElementString( "MetricImage" );
            InitialValueRotation = double.Parse( reader.ReadElementString( "InitialValueRotation" ), CultureInfo.InvariantCulture );
            StepValueRotation = double.Parse( reader.ReadElementString( "StepValueRotation" ), CultureInfo.InvariantCulture );
            MaxValueRotation = double.Parse( reader.ReadElementString( "MaxValueRotation" ), CultureInfo.InvariantCulture );
            MinValueRotation = double.Parse( reader.ReadElementString( "MinValueRotation" ), CultureInfo.InvariantCulture );
            InitialRotation = double.Parse( reader.ReadElementString( "InitialRotation" ), CultureInfo.InvariantCulture );
            RotationTravel = double.Parse( reader.ReadElementString( "RotationTravel" ), CultureInfo.InvariantCulture );
            InitialValueTranslationX = double.Parse( reader.ReadElementString( "InitialValueTranslationX" ), CultureInfo.InvariantCulture );
            StepValueTranslationX = double.Parse( reader.ReadElementString( "StepValueTranslationX" ), CultureInfo.InvariantCulture );
            MaxValueTranslationX = double.Parse( reader.ReadElementString( "MaxValueTranslationX" ), CultureInfo.InvariantCulture );
            MinValueTranslationX = double.Parse( reader.ReadElementString( "MinValueTranslationX" ), CultureInfo.InvariantCulture );
            InitialTranslationX = double.Parse( reader.ReadElementString( "InitialTranslationX" ), CultureInfo.InvariantCulture );
            TranslationTravelX = double.Parse( reader.ReadElementString( "TranslationTravelX" ), CultureInfo.InvariantCulture );
            InitialValueTranslationY = double.Parse( reader.ReadElementString( "InitialValueTranslationY" ), CultureInfo.InvariantCulture );
            StepValueTranslationY = double.Parse( reader.ReadElementString( "StepValueTranslationY" ), CultureInfo.InvariantCulture );
            MaxValueTranslationY = double.Parse( reader.ReadElementString( "MaxValueTranslationY" ), CultureInfo.InvariantCulture );
            MinValueTranslationY = double.Parse( reader.ReadElementString( "MinValueTranslationY" ), CultureInfo.InvariantCulture );
            InitialTranslationY = double.Parse( reader.ReadElementString( "InitialTranslationY" ), CultureInfo.InvariantCulture );
            TranslationTravelY = double.Parse( reader.ReadElementString( "TranslationTravelY" ), CultureInfo.InvariantCulture );
            try
            {
                ClickableVertical = bool.Parse( reader.ReadElementString( "ClickableVertical" ) );
                ClickableHorizontal = bool.Parse( reader.ReadElementString( "ClickableHorizontal" ) );
                InvertedHorizontal = bool.Parse( reader.ReadElementString( "InvertedHorizontal" ) );
                InvertedVertical = bool.Parse( reader.ReadElementString( "InvertedVertical" ) );
                DragOneForOne = bool.Parse( reader.ReadElementString( "DragOneOnOne" ) );
            }
            catch
            {

            }
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
            ValueRotation = InitialValueRotation;
            ValueTranslationX = InitialValueTranslationX;
            ValueTranslationY = InitialValueTranslationY;
            SetRotation( );
            EndTriggerBypass( true );
        }
    }
}
