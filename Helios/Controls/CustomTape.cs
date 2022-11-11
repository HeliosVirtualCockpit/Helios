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
	using System.Globalization;
	using System.Windows;
    using System.Windows.Media;
	using System.Xml;
	using System;

	[HeliosControl("Helios.Base.CustomTape", "Custom Tape", "Custom Controls", typeof(Gauges.GaugeRenderer))]

    public class CustomTape : Gauges.BaseGauge, Capabilities.IConfigurableImageLocation
    {
        private HeliosValue _offFlag;
        private HeliosValue _tapeOffset;
		private HeliosValue _tapeHOffset;
		private HeliosValue _tapeRotation;
		private Gauges.GaugeImage _OffFlag;
		private Gauges.GaugeImage _Foreground;
		private Gauges.GaugeImage _Background;
		private Gauges.CustomGaugeNeedle _Tape;
		private string _tapeImage = "{Helios}/Gauges/A-10/ADI/adi_backup_ball.xaml";
		private string _foregroundImage = "{Helios}/Gauges/A-10/ADI/adi_bezel.png";
		private string _backgroundImage = "{Helios}/Images/Panels/panel-bottom-right.png";
		private string _offFlagImage = "{Helios}/Gauges/A-10/ADI/adi_off_flag.xaml";
		private double _gaugeWidth = 200d;
		private double _gaugeHeight = 200d;
		private double _tape_PosX = 0d;
		private double _tape_PosY = 0d;
		private double _tape_Width = 200d;
		private double _tape_Height = 200d;
		private double _tape_CenterX = 100d;
		private double _tape_CenterY = 100d;
		private double _offFlag_PosX = 30d;
		private double _offFlag_PosY = 30d;
		private double _offFlag_Width = 50d;
		private double _offFlag_Height = 50d;
		private bool _offFlag_IsHidden = false;
		private int _initialHorizontal = 0;
		private int _minHorizontal = 0;
		private int _horizontalTravel = 0;
		private int _initialVertical = 0;
		private int _minVertical = 0;
		private int _verticalTravel = 0;
		private int _initialRotation = 0;
		private int _minRotation = 0;
		private int _rotationTravel = 0;
		private double _minInputHorizontal = 0d;
		private double _maxInputHorizontal = 1d;
		private double _minInputVertical = 0d;
		private double _maxInputVertical = 1d;
		private double _minInputRotation = 0d;
		private double _maxInputRotation = 1d;

		private bool _imageRefresh;
        private HeliosValue _tapeImageRefresh;

        public CustomTape()
            : base("CustomTape", new Size(200d, 200d))
        {
			_Background = new Gauges.GaugeImage(_backgroundImage, new Rect(0, 0, _gaugeWidth, _gaugeHeight));
			Components.Add(_Background);

			_Tape = new Gauges.CustomGaugeNeedle(_tapeImage, new Point(0, 0), new Size(200, 200), new Point(100, 100));
            _Tape.Clip = new RectangleGeometry(new Rect(1d, 1d,198d, 198d));
			Components.Add(_Tape);
			

            _OffFlag = new Gauges.GaugeImage(_offFlagImage, new Rect(1d, 1d,55d, 55d));
            _OffFlag.IsHidden = false;
            Components.Add(_OffFlag);

   
			_Foreground = new Gauges.GaugeImage(_foregroundImage, new Rect(0, 0, _gaugeWidth, _gaugeHeight));
			Components.Add(_Foreground);

            _offFlag = new HeliosValue(this, new BindingValue(false), "", "Off Flag", "Indicates whether the \"Off Flag\" is displayed.", "True if displayed.", BindingValueUnits.Boolean);
            _offFlag.Execute += new HeliosActionHandler(OffFlag_Execute);
            Actions.Add(_offFlag);

            _tapeOffset = new HeliosValue(this, new BindingValue(0d), "", "Tape Vertical Offset", "Values between configured Min and Max", "", BindingValueUnits.Numeric);
            _tapeOffset.Execute += new HeliosActionHandler(TapeOffset_Execute);
            Actions.Add(_tapeOffset);
            Values.Add(_tapeOffset);

			_tapeHOffset = new HeliosValue(this, new BindingValue(0d), "", "Tape Horizontal Offset", "Values between configured Min and Max", "", BindingValueUnits.Numeric);
			_tapeHOffset.Execute += new HeliosActionHandler(TapeHOffset_Execute);
			Actions.Add(_tapeHOffset);
			Values.Add(_tapeHOffset);

			_tapeRotation = new HeliosValue(this, new BindingValue(0d), "", "Tape Rotation", "Values between configured Min and Max", "", BindingValueUnits.Numeric);
			_tapeRotation.Execute += new HeliosActionHandler(TapeRotation_Execute);
			Actions.Add(_tapeRotation);
			Values.Add(_tapeRotation);

			_tapeImageRefresh = new HeliosValue(this, new BindingValue(false), "", "Tape Image Reload", "Indicates a reload of the Tape image", "set true to reload.", BindingValueUnits.Boolean);
			_tapeImageRefresh.Execute += new HeliosActionHandler(TapeImageRefresh_Execute);
			Actions.Add(_tapeImageRefresh);
			Values.Add(_tapeImageRefresh);
		}


        #region Properties

        public bool ImageRefresh
		{
			get { return _imageRefresh; }
			set
			{
				if(!_imageRefresh.Equals(value))
                {
					var oldValue = _imageRefresh;
					_imageRefresh = value;
					_Tape.ImageRefresh = _imageRefresh;
					OnPropertyChanged("ImageRefresh", oldValue, value, true);
				}
			}
		}

		public string TapeImage
		{
			get
			{
				return _tapeImage;
			}
			set
			{
				if ((_tapeImage == null && value != null)
					|| (_tapeImage != null && !_tapeImage.Equals(value)))
				{
					string oldValue = _tapeImage;
					_tapeImage = value;
					_Tape.Image = _tapeImage;
					OnPropertyChanged("TapeImage", oldValue, value, true);
					Refresh();
				}
			}
		}

		public string ForegroundImage
		{
			get
			{
				return _foregroundImage;
			}
			set
			{
				if ((_foregroundImage == null && value != null)
					|| (_foregroundImage != null && !_foregroundImage.Equals(value)))
				{
					string oldValue = _foregroundImage;
					_foregroundImage = value;
					_Foreground.Image = _foregroundImage;
					OnPropertyChanged("ForegroundImage", oldValue, value, true);
					Refresh();
				}
			}
		}

		public string BackgroundImage
		{
			get
			{
				return _backgroundImage;
			}
			set
			{
				if ((_backgroundImage == null && value != null)
					|| (_backgroundImage != null && !_backgroundImage.Equals(value)))
				{
					string oldValue = _backgroundImage;
					_backgroundImage = value;
					_Background.Image = _backgroundImage;
					OnPropertyChanged("BackgroundImage", oldValue, value, true);
					Refresh();
				}
			}
		}


		public string OffFlagImage
		{
			get
			{
				return _offFlagImage;
			}
			set
			{
				if ((_offFlagImage == null && value != null)
					|| (_offFlagImage != null && !_offFlagImage.Equals(value)))
				{
					string oldValue = _offFlagImage;
					_offFlagImage = value;
					_OffFlag.Image = _offFlagImage;
					OnPropertyChanged("OffFlagImage", oldValue, value, true);
					Refresh();
				}
			}
		}

        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void ReplaceImageNames(string oldName, string newName)
        {
            OffFlagImage = !string.IsNullOrEmpty(OffFlagImage) ? OffFlagImage : string.IsNullOrEmpty(oldName) ? newName + OffFlagImage : OffFlagImage.Replace(oldName, newName);
            BackgroundImage = !string.IsNullOrEmpty(BackgroundImage) ? BackgroundImage : string.IsNullOrEmpty(oldName) ? newName + BackgroundImage : BackgroundImage.Replace(oldName, newName);
            ForegroundImage = !string.IsNullOrEmpty(ForegroundImage) ? ForegroundImage : string.IsNullOrEmpty(oldName) ? newName + ForegroundImage : ForegroundImage.Replace(oldName, newName);
            TapeImage = !string.IsNullOrEmpty(TapeImage) ? TapeImage : string.IsNullOrEmpty(oldName) ? newName + TapeImage : TapeImage.Replace(oldName, newName);
        }

        public double Tape_PosX
		{
			get
			{
				return _tape_PosX;
			}
			set
			{
				if (!_tape_PosX.Equals(value))
				{
					double oldValue = _tape_PosX;
					_tape_PosX = value;
					_Tape.TapePosX = _tape_PosX;
					OnPropertyChanged("Tape_PosX", oldValue, value, true);
					Refresh();
				}
			}
		}

		public double Tape_PosY
		{
			get
			{
				return _tape_PosY;
			}
			set
			{
				if (!_tape_PosY.Equals(value))
				{
					double oldValue = _tape_PosY;
					_tape_PosY = value;
					_Tape.TapePosY = _tape_PosY;
					OnPropertyChanged("Tape_PosY", oldValue, value, true);
					Refresh();
				}
			}
		}


		public double Tape_Width
		{
			get
			{
				return _tape_Width;
			}
			set
			{
				if (!_tape_Width.Equals(value))
				{
					double oldValue = _tape_Width;
					_tape_Width = value;
					_Tape.Tape_Width = _tape_Width;
					OnPropertyChanged("Tape_Width", oldValue, value, true);
					Refresh();
				}
			}
		}

		public double Tape_Height
		{
			get
			{
				return _tape_Height;
			}
			set
			{
				if (!_tape_Height.Equals(value))
				{
					double oldValue = _tape_Height;
					_tape_Height = value;
					_Tape.Tape_Height = _tape_Height;
					OnPropertyChanged("Tape_Height", oldValue, value, true);
					Refresh();
				}
			}
		}


		public double Tape_CenterX
		{
			get
			{
				return _tape_CenterX;
			}
			set
			{
				if (!_tape_CenterX.Equals(value))
				{
					double oldValue = _tape_CenterX;
					_tape_CenterX = value;
					_Tape.Tape_CenterX = _tape_CenterX;
					OnPropertyChanged("Tape_CenterX", oldValue, value, true);
					Refresh();
				}
			}
		}

		public double Tape_CenterY
		{
			get
			{
				return _tape_CenterY;
			}
			set
			{
				if (!_tape_CenterY.Equals(value))
				{
					double oldValue = _tape_CenterY;
					_tape_CenterY = value;
					_Tape.Tape_CenterY = _tape_CenterY;
					OnPropertyChanged("Tape_CenterY", oldValue, value, true);
					Refresh();
				}
			}
		}


		public double OffFlag_PosX
		{
			get
			{
				return _offFlag_PosX;
			}
			set
			{
				if (!_offFlag_PosX.Equals(value))
				{
					double oldValue = _offFlag_PosX;
					_offFlag_PosX = value;
					_OffFlag.PosX = _offFlag_PosX;
					OnPropertyChanged("OffFlag_PosX", oldValue, value, true);
					Refresh();
				}
			}
		}

		public double OffFlag_PosY
		{
			get
			{
				return _offFlag_PosY;
			}
			set
			{
				if (!_offFlag_PosY.Equals(value))
				{
					double oldValue = _offFlag_PosY;
					_offFlag_PosY = value;
					_OffFlag.PosY = _offFlag_PosY;
					OnPropertyChanged("OffFlag_PosY", oldValue, value, true);
					Refresh();
				}
			}
		}


		public double OffFlag_Width
		{
			get
			{
				return _offFlag_Width;
			}
			set
			{
				if (!_offFlag_Width.Equals(value))
				{
					double oldValue = _offFlag_Width;
					_offFlag_Width = value;
					_OffFlag.Width = _offFlag_Width;
					OnPropertyChanged("OffFlag_Width", oldValue, value, true);
					Refresh();
				}
			}
		}

		public double OffFlag_Height
		{
			get
			{
				return _offFlag_Height;
			}
			set
			{
				if (!_offFlag_Height.Equals(value))
				{
					double oldValue = _offFlag_Height;
					_offFlag_Height = value;
					_OffFlag.Height = _offFlag_Height;
					OnPropertyChanged("OffFlag_Height", oldValue, value, true);
					Refresh();
				}
			}
		}

		public bool OffFlag_IsHidden
		{
			get
			{
				return _offFlag_IsHidden;
			}
			set
			{
				if (!_offFlag_IsHidden.Equals(value))
				{
					bool oldValue = _offFlag_IsHidden;
					_offFlag_IsHidden = value;
					_OffFlag.IsHidden = _offFlag_IsHidden;
					OnPropertyChanged("OffFlag_IsHidden", oldValue, value, true);
					Refresh();
				}
			}
		}


		public int InitialHorizontal
		{
			get
			{
				return _initialHorizontal;
			}
			set
			{
				if (!_initialHorizontal.Equals(value))
				{
					int oldValue = _initialHorizontal;
					_initialHorizontal = value;
					_Tape.HorizontalOffset = value;
					OnPropertyChanged("InitialHorizontal", oldValue, value, true);
					Refresh();
				}
			}
		}

		public int MinHorizontal
		{
			get
			{
				return _minHorizontal;
			}
			set
			{
				if (!_minHorizontal.Equals(value))
				{
					int oldValue = _minHorizontal;
					_minHorizontal = value;
					_Tape.HorizontalOffset = value;
					OnPropertyChanged("MinHorizontal", oldValue, value, true);
					Refresh();
				}
			}
		}

		public int HorizontalTravel
		{
			get
			{
				return _horizontalTravel;
			}
			set
			{
				if (!_horizontalTravel.Equals(value))
				{
					int oldValue = _horizontalTravel;
					_horizontalTravel = value;
					_Tape.HorizontalOffset = value;
					OnPropertyChanged("HorizontalTravel", oldValue, value, true);
					Refresh();
				}
			}
		}

		public int InitialVertical
		{
			get
			{
				return _initialVertical;
			}
			set
			{
				if (!_initialVertical.Equals(value))
				{
					int oldValue = _initialVertical;
					_initialVertical = value;
					_Tape.VerticalOffset = value;
					OnPropertyChanged("InitialVertical", oldValue, value, true);
					Refresh();
				}
			}
		}

		public int MinVertical
		{
			get
			{
				return _minVertical;
			}
			set
			{
				if (!_minVertical.Equals(value))
				{
					int oldValue = _minVertical;
					_minVertical = value;
					_Tape.VerticalOffset = value;
					OnPropertyChanged("MinVertical", oldValue, value, true);
					Refresh();
				}
			}
		}

		public int VerticalTravel
		{
			get
			{
				return _verticalTravel;
			}
			set
			{
				if (!_verticalTravel.Equals(value))
				{
					int oldValue = _verticalTravel;
					_verticalTravel = value;
					_Tape.VerticalOffset = value;
					OnPropertyChanged("VerticalTravel", oldValue, value, true);
					Refresh();
				}
			}
		}


		public int InitialRotation
		{
			get
			{
				return _initialRotation;
			}
			set
			{
				if (!_initialRotation.Equals(value))
				{
					int oldValue = _initialRotation;
					_initialRotation = value;
					_Tape.BaseRotation = value;
					OnPropertyChanged("InitialRotation", oldValue, value, true);
					Refresh();
				}
			}
		}

		public int MinRotation
		{
			get
			{
				return _minRotation;
			}
			set
			{
				if (!_minRotation.Equals(value))
				{
					int oldValue = _minRotation;
					_minRotation = value;
					_Tape.BaseRotation = value;
					OnPropertyChanged("MinRotation", oldValue, value, true);
					Refresh();
				}
			}
		}

		public int RotationTravel
		{
			get
			{
				return _rotationTravel;
			}
			set
			{
				if (!_rotationTravel.Equals(value))
				{
					int oldValue = _rotationTravel;
					_rotationTravel = value;
					_Tape.BaseRotation = value;
					OnPropertyChanged("RotationTravel", oldValue, value, true);
					Refresh();
				}
			}
		}



		public double MinInputHorizontal
		{
			get
			{
				return _minInputHorizontal;
			}
			set
			{
				if (!_minInputHorizontal.Equals(value))
				{
					double oldValue = _minInputHorizontal;
					if ((value != _maxInputHorizontal) & (value < _maxInputHorizontal))
					{
						_minInputHorizontal = value;
					}
					OnPropertyChanged("MinInputHorizontal", oldValue, _minInputHorizontal, true);
				}
			}
		}

		public double MaxInputHorizontal
		{
			get
			{
				return _maxInputHorizontal;
			}
			set
			{
				if (!_maxInputHorizontal.Equals(value))
				{
					double oldValue = _maxInputHorizontal;
					if ((value != _minInputHorizontal) & (value > _minInputHorizontal))
					{
						_maxInputHorizontal = value;
					}
					OnPropertyChanged("MaxInputHorizontal", oldValue, _maxInputHorizontal, true);
				}
			}
		}

		public double MinInputVertical
		{
			get
			{
				return _minInputVertical;
			}
			set
			{
				if (!_minInputVertical.Equals(value))
				{
					double oldValue = _minInputVertical;
					if ((value != _maxInputVertical) & (value < _maxInputVertical))
					{
						_minInputVertical = value;
					}

					OnPropertyChanged("MinInputVertical", oldValue, _minInputVertical, true);
				}
			}
		}

		public double MaxInputVertical
		{
			get
			{
				return _maxInputVertical;
			}
			set
			{
				if (!_maxInputVertical.Equals(value))
				{
					double oldValue = _maxInputVertical;
					if ((value != _minInputVertical) & (value > _minInputVertical))
					{
						_maxInputVertical = value;
					}
					OnPropertyChanged("MaxInputVertical", oldValue, _maxInputVertical, true);
				}
			}
		}

		public double MinInputRotation
		{ 
			get
			{
				return _minInputRotation;
			}
			set
			{
				if (!_minInputRotation.Equals(value))
				{
					double oldValue = _minInputRotation;
					if ((value != _maxInputRotation) & (value < _maxInputRotation))
					{
						_minInputRotation = value;
					}
					OnPropertyChanged("MinInputRotation", oldValue, _minInputRotation, true);
				}
			}
		}

		public double MaxInputRotation
{
			get
			{
				return _maxInputRotation;
			}
			set
			{
				if (!_maxInputRotation.Equals(value))
				{
					double oldValue = _maxInputRotation;
					if ((value != _minInputRotation) & (value > _minInputRotation))
					{
						_maxInputRotation = value;
					}
					OnPropertyChanged("MaxInputRotation", oldValue, _maxInputRotation, true);
				}
			}
		}


		public override void Reset()
		{
            base.Reset();

			BeginTriggerBypass(true);
			_Tape.VerticalOffset = InitialVertical;
			_Tape.HorizontalOffset = InitialHorizontal;
			_Tape.Rotation = 0;
			_Tape.BaseRotation = InitialRotation;
			Refresh();

			EndTriggerBypass(true);
		}






		#endregion


		#region Actions

		void TapeImageRefresh_Execute(object action, HeliosActionEventArgs e)
        {
			_tapeImageRefresh.SetValue(e.Value, e.BypassCascadingTriggers);
			_Tape.ImageRefresh = e.Value.BoolValue;
        }

		void OffFlag_Execute(object action, HeliosActionEventArgs e)
        {
            _offFlag.SetValue(e.Value, e.BypassCascadingTriggers);
            _OffFlag.IsHidden = !e.Value.BoolValue;
        }

        void TapeOffset_Execute(object action, HeliosActionEventArgs e)
        {
            _tapeOffset.SetValue(e.Value, e.BypassCascadingTriggers);
			double vValue = (_tapeOffset.Value.DoubleValue + ((_maxInputVertical - _minInputVertical) - _maxInputVertical)) / (_maxInputVertical - _minInputVertical); // convert to to 0-1
			_Tape.VerticalOffset = (_minVertical * (1 - vValue)) + (_verticalTravel * vValue); // lerp vertical 
		}

		void TapeHOffset_Execute(object action, HeliosActionEventArgs e)
		{
			_tapeHOffset.SetValue(e.Value, e.BypassCascadingTriggers);
			double hValue = (_tapeHOffset.Value.DoubleValue + ((_maxInputHorizontal - _minInputHorizontal) - _maxInputHorizontal)) / (_maxInputHorizontal - _minInputHorizontal);
			_Tape.HorizontalOffset = (_minHorizontal * (1 - hValue)) + (_horizontalTravel * hValue) ; // lerp horizontal
		}

		void TapeRotation_Execute(object action, HeliosActionEventArgs e)
		{
			_tapeRotation.SetValue(e.Value, e.BypassCascadingTriggers);
			double rValue = (_tapeRotation.Value.DoubleValue + ((_maxInputRotation - _minInputRotation) - _maxInputRotation)) / (_maxInputRotation - _minInputRotation); // convert to to 0-1
			_Tape.Rotation = (_minRotation * (1 - rValue)) + (_rotationTravel * rValue); // lerp rotation


		}

		#endregion

		public override void WriteXml(XmlWriter writer)
		{

			base.WriteXml(writer);

			writer.WriteElementString("TapeImage", TapeImage);
			writer.WriteElementString("OffFlagImage", BackgroundImage);
			writer.WriteElementString("ForegroundImage", ForegroundImage);
			writer.WriteElementString("BackgroundImage", BackgroundImage);
			writer.WriteElementString("Tape_PosX", Tape_PosX.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Tape_PosY", Tape_PosY.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Tape_Width", Tape_Width.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Tape_Height", Tape_Height.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Tape_CenterX", Tape_CenterX.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Tape_CenterY", Tape_CenterY.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("OffFlag_PosX", OffFlag_PosX.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("OffFlag_PosY", OffFlag_PosY.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("OffFlag_Width", OffFlag_Width.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("OffFlag_Height", OffFlag_Height.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("OffFlag_IsHidden", OffFlag_IsHidden.ToString(CultureInfo.InvariantCulture));			
			writer.WriteElementString("MinHorizontal", MinHorizontal.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("HorizontalTravel", HorizontalTravel.ToString(CultureInfo.InvariantCulture));			
			writer.WriteElementString("MinVertical", MinVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("VerticalTravel", VerticalTravel.ToString(CultureInfo.InvariantCulture));			
			writer.WriteElementString("MinRotation", MinRotation.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("RotationTravel", RotationTravel.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("InitialHorizontal", InitialHorizontal.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("InitialVertical", InitialVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("InitialRotation", InitialRotation.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MinInputHorizontal", MinInputHorizontal.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MaxInputHorizontal",MaxInputHorizontal.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MinInputVertical", MinInputVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MaxInputVertical", MaxInputVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MinInputRotation", MinInputRotation.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MaxInputRotation", MaxInputRotation.ToString(CultureInfo.InvariantCulture));

		}

		public override void ReadXml(XmlReader reader)
		{

			base.ReadXml(reader);

			TapeImage = reader.ReadElementString("TapeImage");
			BackgroundImage = reader.ReadElementString("OffFlagImage");
			ForegroundImage = reader.ReadElementString("ForegroundImage");
			BackgroundImage = reader.ReadElementString("BackgroundImage");
			Tape_PosX = double.Parse(reader.ReadElementString("Tape_PosX"), CultureInfo.InvariantCulture);
			Tape_PosY = double.Parse(reader.ReadElementString("Tape_PosY"), CultureInfo.InvariantCulture);
			Tape_Width = double.Parse(reader.ReadElementString("Tape_Width"), CultureInfo.InvariantCulture);
			Tape_Height = double.Parse(reader.ReadElementString("Tape_Height"), CultureInfo.InvariantCulture);
			Tape_CenterX = double.Parse(reader.ReadElementString("Tape_CenterX"), CultureInfo.InvariantCulture);
			Tape_CenterY = double.Parse(reader.ReadElementString("Tape_CenterY"), CultureInfo.InvariantCulture);
			OffFlag_PosX = double.Parse(reader.ReadElementString("OffFlag_PosX"), CultureInfo.InvariantCulture);
			OffFlag_PosY = double.Parse(reader.ReadElementString("OffFlag_PosY"), CultureInfo.InvariantCulture);
			OffFlag_Width = double.Parse(reader.ReadElementString("OffFlag_Width"), CultureInfo.InvariantCulture);
			OffFlag_Height = double.Parse(reader.ReadElementString("OffFlag_Height"), CultureInfo.InvariantCulture);
			OffFlag_IsHidden = bool.Parse(reader.ReadElementString("OffFlag_IsHidden"));			
			MinHorizontal = int.Parse(reader.ReadElementString("MinHorizontal"), CultureInfo.InvariantCulture);
			HorizontalTravel = int.Parse(reader.ReadElementString("HorizontalTravel"), CultureInfo.InvariantCulture);			
			MinVertical = int.Parse(reader.ReadElementString("MinVertical"), CultureInfo.InvariantCulture);
			VerticalTravel = int.Parse(reader.ReadElementString("VerticalTravel"), CultureInfo.InvariantCulture);			
			MinRotation = int.Parse(reader.ReadElementString("MinRotation"), CultureInfo.InvariantCulture);
			RotationTravel = int.Parse(reader.ReadElementString("RotationTravel"), CultureInfo.InvariantCulture);			
			InitialHorizontal = int.Parse(reader.ReadElementString("InitialHorizontal"), CultureInfo.InvariantCulture);
			InitialVertical = int.Parse(reader.ReadElementString("InitialVertical"), CultureInfo.InvariantCulture);
			InitialRotation = int.Parse(reader.ReadElementString("InitialRotation"), CultureInfo.InvariantCulture);
			MinInputHorizontal = double.Parse(reader.ReadElementString("MinInputHorizontal"), CultureInfo.InvariantCulture);
			MaxInputHorizontal = double.Parse(reader.ReadElementString("MaxInputHorizontal"), CultureInfo.InvariantCulture);
			MinInputVertical = double.Parse(reader.ReadElementString("MinInputVertical"), CultureInfo.InvariantCulture);
			MaxInputVertical = double.Parse(reader.ReadElementString("MaxInputVertical"), CultureInfo.InvariantCulture);
			MinInputRotation = double.Parse(reader.ReadElementString("MinInputRotation"), CultureInfo.InvariantCulture);
			MaxInputRotation = double.Parse(reader.ReadElementString("MaxInputRotation"), CultureInfo.InvariantCulture);


			BeginTriggerBypass(true);
			_Tape.VerticalOffset = InitialVertical;
			_Tape.HorizontalOffset = InitialHorizontal;
			_Tape.BaseRotation = InitialRotation;
			Refresh();
			EndTriggerBypass(true);
		}



	}
}
