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
	using System.Globalization;
	using System.Windows;
    using System.Windows.Media;
	using System.Xml;

	[HeliosControl("Helios.Base.CustomDrum", "Custom Drum", "Custom Controls", typeof(Gauges.GaugeRenderer))]

    public class CustomDrum : Gauges.BaseGauge, IConfigurableImageLocation
    {

        private HeliosValue _drumOffset;



		private Gauges.CustomGaugeNeedle _Drum;
		private string _drumImage = "{Helios}/Gauges/A-10/Common/drum_tape.xaml";
		private double _drum_PosX = 0d;
		private double _drum_PosY = 0d;
		private double _drum_Width = 50d;
		private double _drum_Height = 1000d;
		private int _initialVertical = -490;
		private int _minVertical = -910;
		private int _verticalTravel = 10;
		private double _minInputVertical = 0d;
		private double _maxInputVertical = 1d;

		public CustomDrum() : this("CustomDrum", new Size(50d, 100d)) { }
        public CustomDrum(string name, Size size)
            : base(name, size)
        {
	

			_Drum = new Gauges.CustomGaugeNeedle(_drumImage, new Point(0, 0), new Size(size.Width, size.Height*10), new Point(0, 0));
            _Drum.Clip = new RectangleGeometry(new Rect(0d, 0d,size.Width, size.Height));
            Components.Add(_Drum);

            _drumOffset = new HeliosValue(this, new BindingValue(0d), "", "Drum tape offset", "Value between configured Min and Max", "", BindingValueUnits.Numeric);
            _drumOffset.Execute += new HeliosActionHandler(DrumOffset_Execute);
            Actions.Add(_drumOffset);
            Values.Add(_drumOffset);

		}

		#region Properties

		public string DrumImage
		{
			get
			{
				return _drumImage;
			}
			set
			{
				if ((_drumImage == null && value != null)
					|| (_drumImage != null && !_drumImage.Equals(value)))
				{
					string oldValue = _drumImage;
					_drumImage = value;
					_Drum.Image = _drumImage;
					OnPropertyChanged("DrumImage", oldValue, value, true);
					Refresh();
				}
			}
		}

		public double Drum_PosX
		{
			get
			{
				return _drum_PosX;
			}
			set
			{
				if (!_drum_PosX.Equals(value))
				{
					double oldValue = _drum_PosX;
					_drum_PosX = value;
					_Drum.TapePosX = _drum_PosX;
					OnPropertyChanged("Drum_PosX", oldValue, value, true);
					Refresh();
				}
			}
		}

		public double Drum_PosY
		{
			get
			{
				return _drum_PosY;
			}
			set
			{
				if (!_drum_PosY.Equals(value))
				{
					double oldValue = _drum_PosY;
					_drum_PosY = value;
					_Drum.TapePosY = _drum_PosY;
					OnPropertyChanged("Drum_PosY", oldValue, value, true);
					Refresh();
				}
			}
		}


		public double Drum_Width
		{
			get
			{
				return _drum_Width;
			}
			set
			{
				if (!_drum_Width.Equals(value))
				{
					double oldValue = _drum_Width;
					_drum_Width = value;
					_Drum.Tape_Width = _drum_Width;
					OnPropertyChanged("Drum_Width", oldValue, value, true);
					Refresh();
				}
			}
		}

		public double Drum_Height
		{
			get
			{
				return _drum_Height;
			}
			set
			{
				if (!_drum_Height.Equals(value))
				{
					double oldValue = _drum_Height;
					_drum_Height = value;
					_Drum.Tape_Height = _drum_Height;
					OnPropertyChanged("Drum_Height", oldValue, value, true);
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
					_Drum.VerticalOffset = value;
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
					_Drum.VerticalOffset = value;
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
					_Drum.VerticalOffset = value;
					OnPropertyChanged("VerticalTravel", oldValue, value, true);
					Refresh();
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

        public override void Reset()
        {
            base.Reset();
			BeginTriggerBypass(true);
			try
			{
				_Drum.VerticalOffset = InitialVertical;
				Refresh();
			}
			finally
			{
				EndTriggerBypass(true);
			}

		}

        #endregion

        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void ReplaceImageNames(string oldName, string newName)
        {
            DrumImage = !string.IsNullOrEmpty(DrumImage) ? DrumImage : string.IsNullOrEmpty(oldName) ? newName + DrumImage : DrumImage.Replace(oldName, newName);
        }

        #region Actions


        void DrumOffset_Execute(object action, HeliosActionEventArgs e)
        {
            _drumOffset.SetValue(e.Value, e.BypassCascadingTriggers);
			double vValue = (_drumOffset.Value.DoubleValue + ((_maxInputVertical - _minInputVertical) - _maxInputVertical)) / (_maxInputVertical - _minInputVertical); // convert to to 0-1
			_Drum.VerticalOffset = (_minVertical * (1 - vValue)) + (_verticalTravel * vValue); // lerp vertical 
		}

		#endregion

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);

			writer.WriteElementString("DrumImage", DrumImage);
			writer.WriteElementString("Drum_PosX", Drum_PosX.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Drum_PosY", Drum_PosY.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Drum_Width", Drum_Width.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Drum_Height", Drum_Height.ToString(CultureInfo.InvariantCulture));		
			writer.WriteElementString("MinVertical", MinVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("VerticalTravel", VerticalTravel.ToString(CultureInfo.InvariantCulture));			
			writer.WriteElementString("InitialVertical", InitialVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MinInputVertical", MinInputVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MaxInputVertical", MaxInputVertical.ToString(CultureInfo.InvariantCulture));

		}

		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);

			DrumImage = reader.ReadElementString("DrumImage");
			Drum_PosX = double.Parse(reader.ReadElementString("Drum_PosX"), CultureInfo.InvariantCulture);
			Drum_PosY = double.Parse(reader.ReadElementString("Drum_PosY"), CultureInfo.InvariantCulture);
			Drum_Width = double.Parse(reader.ReadElementString("Drum_Width"), CultureInfo.InvariantCulture);
			Drum_Height = double.Parse(reader.ReadElementString("Drum_Height"), CultureInfo.InvariantCulture);			
			MinVertical = int.Parse(reader.ReadElementString("MinVertical"), CultureInfo.InvariantCulture);
			VerticalTravel = int.Parse(reader.ReadElementString("VerticalTravel"), CultureInfo.InvariantCulture);			
			InitialVertical = int.Parse(reader.ReadElementString("InitialVertical"), CultureInfo.InvariantCulture);
			MinInputVertical = double.Parse(reader.ReadElementString("MinInputVertical"), CultureInfo.InvariantCulture);
			MaxInputVertical = double.Parse(reader.ReadElementString("MaxInputVertical"), CultureInfo.InvariantCulture);

			BeginTriggerBypass(true);
				try
				{
					_Drum.VerticalOffset = InitialVertical;
					Refresh();
				}
				finally
				{
					EndTriggerBypass(true);
				}
		}
	}
}
