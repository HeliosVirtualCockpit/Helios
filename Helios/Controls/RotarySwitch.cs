//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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
    using ComponentModel;
    using Capabilities;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.Base.RotarySwitch", "Rotary - Knob 2", "Rotary Switches", typeof(RotarySwitchRenderer))]
    public class RotarySwitch : RotaryKnob, IRotarySwitch
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private int _currentPosition;
        private int _defaultPosition;

        private bool _drawLines = true;
        private double _lineThickness = 2d;
        private Color _lineColor = Colors.White;
        private double _lineLength = 0.9d;

        private bool _drawLabels = true;
        private double _labelDistance = 1d;
        private double _maxLabelWidth = 40d;
        private double _maxLabelHeight = 0d;
        private Color _labelColor = Colors.White;
        private readonly HeliosValue _positionValue;
        private readonly HeliosValue _positionNameValue;
        private readonly HeliosValue _incrementValue;
        private readonly HeliosValue _decrementValue;

        private bool _isContinuous;

        // array ordered by position angles in degrees, so we can dereference to their position index
        private PositionIndexEntry[] _positionIndex;

        // comparison function to binary search in sorted array
        private static readonly PositionSortComparer PositionIndexComparer = new PositionSortComparer();

        public RotarySwitch()
            : base("Rotary Switch", new Size(100, 100))
        {
            KnobImage = "{Helios}/Images/Knobs/knob2.png";
            LabelFormat.PropertyChanged += LabelFormat_PropertyChanged;

            _positionValue = new HeliosValue(this, new BindingValue(1), "", "position", "Current position of the switch.", "", BindingValueUnits.Numeric);
            _positionValue.Execute += SetPositionAction_Execute;
            Values.Add(_positionValue);
            Actions.Add(_positionValue);
            Triggers.Add(_positionValue);

            _incrementValue = new HeliosValue(this, new BindingValue(1), "", "increment", "Increment current position of the switch.", "Set true to increment position.", BindingValueUnits.Boolean);
            _incrementValue.Execute += IncrementPositionAction_Execute;
            Actions.Add(_incrementValue);

            _decrementValue = new HeliosValue(this, new BindingValue(1), "", "decrement", "Decrement current position of the switch.", "Set true to decrement position.", BindingValueUnits.Boolean);
            _decrementValue.Execute += DecrementPositionAction_Execute;
            Actions.Add(_decrementValue);

            _positionNameValue = new HeliosValue(this, new BindingValue("0"), "", "position name", "Name of the current position of the switch.", "", BindingValueUnits.Text);
            Values.Add(_positionNameValue);
            Triggers.Add(_positionNameValue);

            Positions.CollectionChanged += Positions_CollectionChanged;
            Positions.PositionChanged += PositionChanged;
            Positions.Add(new RotarySwitchPosition(this, 1, "0", 0d));
            Positions.Add(new RotarySwitchPosition(this, 2, "1", 90d));
            _currentPosition = 1;
            _defaultPosition = 1;
        }

        #region Properties

        public bool DrawLabels
        {
            get => _drawLabels;
            set
            {
                if (_drawLabels.Equals(value))
                {
                    return;
                }

                bool oldValue = _drawLabels;
                _drawLabels = value;
                OnPropertyChanged("DrawLabels", oldValue, value, true);
                Refresh();
            }
        }

        public double LabelDistance
        {
            get => _labelDistance;
            set
            {
                if (_labelDistance.Equals(value))
                {
                    return;
                }

                double oldValue = _labelDistance;
                _labelDistance = value;
                OnPropertyChanged("LabelDistance", oldValue, value, true);
                Refresh();
            }
        }

        public double MaxLabelHeight
        {
            get => _maxLabelHeight;
            set
            {
                if (_maxLabelHeight.Equals(value))
                {
                    return;
                }

                double oldValue = _maxLabelHeight;
                _maxLabelHeight = value;
                OnPropertyChanged("MaxLabelHeight", oldValue, value, true);
                Refresh();
            }
        }

        public double MaxLabelWidth
        {
            get => _maxLabelWidth;
            set
            {
                if (_maxLabelWidth.Equals(value))
                {
                    return;
                }

                double oldValue = _maxLabelWidth;
                _maxLabelWidth = value;
                OnPropertyChanged("MaxLabelWidth", oldValue, value, true);
                Refresh();
            }
        }

        public bool DrawLines
        {
            get => _drawLines;
            set
            {
                if (_drawLines.Equals(value))
                {
                    return;
                }

                bool oldValue = _drawLines;
                _drawLines = value;
                OnPropertyChanged("DrawLines", oldValue, value, true);
                Refresh();
            }
        }

        public bool IsContinuous
        {
            get => _isContinuous;
            set
            {
                if (_isContinuous.Equals(value))
                {
                    return;
                }

                bool oldValue = _isContinuous;
                _isContinuous = value;
                OnPropertyChanged("IsContinuous", oldValue, value, true);
                Refresh();
            }
        }

        public Color LabelColor
        {
            get => _labelColor;
            set
            {
                if (_labelColor.Equals(value))
                {
                    return;
                }

                Color oldValue = _labelColor;
                _labelColor = value;
                OnPropertyChanged("LabelColor", oldValue, value, true);
                Refresh();
            }
        }

        public TextFormat LabelFormat { get; } = new TextFormat();

        public double LineThickness
        {
            get => _lineThickness;
            set
            {
                if (_lineThickness.Equals(value))
                {
                    return;
                }

                double oldValue = _lineThickness;
                _lineThickness = value;
                OnPropertyChanged("LineThickness", oldValue, value, true);
                Refresh();
            }
        }

        public Color LineColor
        {
            get => _lineColor;
            set
            {
                if (_lineColor.Equals(value))
                {
                    return;
                }

                Color oldValue = _lineColor;
                _lineColor = value;
                OnPropertyChanged("LineColor", oldValue, value, true);
                Refresh();
            }
        }

        public double LineLength
        {
            get => _lineLength;
            set
            {
                if (_lineLength.Equals(value))
                {
                    return;
                }

                double oldValue = _lineLength;
                _lineLength = value;
                OnPropertyChanged("LineLength", oldValue, value, true);
                Refresh();
            }
        }

        // XXX this is horrible:  during deserialization, this creates n^2 updates as each insert to the collection renumbers everything
        // XXX for a large profile, this is on the order of 2.5 seconds wasted
        public RotarySwitchPositionCollection Positions { get; } = new RotarySwitchPositionCollection();

        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                if (_currentPosition.Equals(value) || value <= 0 || value > Positions.Count)
                {
                    return;
                }

                int oldValue = _currentPosition;
                double oldRotation = KnobRotation;

                _currentPosition = value;
                KnobRotation = Positions[value-1].Rotation;

                _positionValue.SetValue(new BindingValue((double)_currentPosition), BypassTriggers);
                _positionNameValue.SetValue(new BindingValue(Positions[_currentPosition-1].Name), BypassTriggers);

                if (!BypassTriggers)
                {
                    if (oldValue > 0 && oldValue <= Positions.Count)
                    {
                        Positions[oldValue-1].ExitTrigger.FireTrigger(BindingValue.Empty);
                    }
                    Positions[_currentPosition-1].EnterTriggger.FireTrigger(BindingValue.Empty);
                }

                OnPropertyChanged("CurrentPosition", oldValue, value, false);
                OnPropertyChanged("Rotation", oldRotation, KnobRotation, false);
                OnDisplayUpdate();
            }
        }

        public int DefaultPosition
        {
            get => _defaultPosition;
            set
            {
                if (_defaultPosition.Equals(value) || value <= 0 || value > Positions.Count)
                {
                    return;
                }

                int oldValue = _defaultPosition;
                _defaultPosition = value;
                OnPropertyChanged("DefaultPosition", oldValue, value, true);
            }
        }
        public virtual PushButtonType ButtonType { get; set; }
        public virtual bool ClickConfigurable { get => false; }
        #endregion

        void LabelFormat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e is PropertyNotificationEventArgs origArgs)
            {
                OnPropertyChanged("LabelFormat", origArgs);
            }
            Refresh();
        }

        void Positions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (RotarySwitchPosition position in e.OldItems)
                {
                    Triggers.Remove(position.EnterTriggger);
                    Triggers.Remove(position.ExitTrigger);
                }

                if (Positions.Count == 0)
                {
                    _currentPosition = 0;
                }
                else if (_currentPosition > Positions.Count)
                {
                    _currentPosition = Positions.Count;
                }
            }

            if (e.NewItems != null)
            {
                foreach (RotarySwitchPosition position in e.NewItems)
                {
                    Triggers.Add(position.EnterTriggger);
                    Triggers.Add(position.ExitTrigger);
                }
            }

            // Need to do it twice to prevent collisions.  This is
            // just an easy way to do it instead of reordering everything
            // in the loops above.
            int i = 1000000;
            foreach (RotarySwitchPosition position in Positions)
            {
                position.Index = i++;
            }

            i = 1;
            foreach (RotarySwitchPosition position in Positions)
            {
                position.Index = i++;
            }
            UpdateValueHelp();
            UpdatePositionIndex();
        }

        private void PositionChanged(object sender, RotarySwitchPositionChangeArgs e)
        {
            PropertyNotificationEventArgs args = new PropertyNotificationEventArgs(e.Position, e.PropertyName, e.OldValue, e.NewValue, true);
            if (e.PropertyName.Equals("Rotation"))
            {
                KnobRotation = Positions[CurrentPosition - 1].Rotation;
                UpdatePositionIndex();
            }
            OnPropertyChanged("Positions", args);
            UpdateValueHelp();
            Refresh();
        }

        private void UpdateValueHelp()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(" (");
            for (int i = 0; i < Positions.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }
                RotarySwitchPosition position = Positions[i];
                sb.Append(i + 1);
                if (position.Name != null && position.Name.Length > 0)
                {
                    sb.Append("=");
                    sb.Append(position.Name);
                }
            }
            sb.Append(")");
            _positionValue.ValueDescription = sb.ToString();
        }

        private class PositionIndexEntry
        {
            public double Rotation { get; set; }
            public int Index { get; set; }
        }

        private class PositionSortComparer : IComparer<PositionIndexEntry>
        {
            public int Compare(PositionIndexEntry left, PositionIndexEntry right) => (int)(left.Rotation - right.Rotation);
        }

        private void UpdatePositionIndex()
        {
            _positionIndex = Positions.OrderBy(p => p.Rotation).Select(p => new PositionIndexEntry { Rotation = p.Rotation, Index = p.Index }).ToArray();
        }

        public override void MouseDown(Point location)
        {
            if (NonClickableZones != null)
            {
                foreach (NonClickableZone zone in NonClickableZones)
                {
                    if (zone.AllPositions && zone.isClickInZone(location))
                    {
                        zone.ChildVisual.MouseDown(new System.Windows.Point(
                            location.X - (zone.ChildVisual.Left - this.Left),
                            location.Y - (zone.ChildVisual.Top - this.Top)));
                        return; //we get out to let the ChildVisual using the click
                    }
                }
            }
            base.MouseDown(location);
        }

        public override void MouseUp(Point location)
        {
            if (NonClickableZones != null)
            {
                foreach (NonClickableZone zone in NonClickableZones)
                {
                    if (zone.AllPositions && zone.isClickInZone(location))
                    {
                        zone.ChildVisual.MouseUp(new System.Windows.Point(location.X - (zone.ChildVisual.Left - this.Left), location.Y - (zone.ChildVisual.Top - this.Top)));
                        return; //we get out to let the ChildVisual using the click
                    }
                }
            }
            base.MouseUp(location);
        }

        #region IPulsedControl

        public override void Pulse(int pulses)
        {
            if (Positions.Count == 0)
            {
                // there are no positions so we cannot move
                return;
            }

            // WARNING: Positions is a zero-based array, but _currentPosition is 1-based
            int newPosition = _currentPosition + pulses;

            if (IsContinuous)
            {
                // wrap around if we have to
                newPosition = 1 + ((newPosition - 1) % Positions.Count);
                if (newPosition < 1)
                {
                    // don't use negative remainder
                    newPosition += Positions.Count;
                }
                CurrentPosition = newPosition;
                return;
            }

            // explicitly check boundaries
            if (newPosition > Positions.Count)
            {
                CurrentPosition = Positions.Count;
            } 
            else if (newPosition < 1)
            {
                CurrentPosition = 1;
            }
            else
            {
                CurrentPosition = newPosition;
            }

            // move the virtual control to this snap location also
            _controlAngle = KnobRotation;
        }

        #endregion

        #region IRotarySwitch

        public int MinPosition => 1;

        public int MaxPosition => Positions.Count;

        #endregion

        #region IRotaryControl

        // the angle that our control would be at if we were allowed to stop everywhere, required so that IRotaryControl will operate correctly
        // with incremental changes
        private double _controlAngle;

        public override double ControlAngle
        {
            get => _controlAngle;
            set
            {
                if (_positionIndex.Length < 1)
                {
                    // no positions, nothing will work
                    return;
                }

                if (IsContinuous)
                {
                    _controlAngle = value % 360d;
                    if (_controlAngle < 0d)
                    {
                        // don't use negative remainder
                        _controlAngle += 360d;
                    }
                }
                else
                {
                    // clamp
                    _controlAngle = Math.Min(Math.Max(value, _positionIndex[0].Rotation), _positionIndex[_positionIndex.Length - 1].Rotation);
                }

                // see where requested position falls in the sorted sequence of knob positions
                Logger.Debug("setting rotary switch based on input angle {Angle}", _controlAngle);
                int searchResult = Array.BinarySearch(_positionIndex, new PositionIndexEntry { Rotation = _controlAngle }, PositionIndexComparer);
                if (searchResult >= 0)
                {
                    // direct hit
                    CurrentPosition = _positionIndex[searchResult].Index;
                    return;
                }

                // find closest two positons
                int nextLarger = ~searchResult;
                int lowIndex;
                int highIndex;
                double lowOffset;
                double highOffset;
                if (nextLarger == _positionIndex.Length)
                {
                    // larger than all values
                    lowIndex = _positionIndex.Length - 1;
                    highIndex = 0;
                    lowOffset = 0d;
                    highOffset = 360d;
                }
                else if (nextLarger == 0)
                {
                    // smaller than all values
                    lowIndex = _positionIndex.Length - 1;
                    highIndex = 0;
                    lowOffset = -360d;
                    highOffset = 0d;
                }
                else
                {
                    // somewhere in middle
                    lowIndex = nextLarger - 1;
                    highIndex = nextLarger;
                    lowOffset = 0d;
                    highOffset = 0d;
                }

                // snap to closest position
                double lowValue = _positionIndex[lowIndex].Rotation + lowOffset;
                double highValue = _positionIndex[highIndex].Rotation + highOffset;
                int closestIndex = Math.Abs(_controlAngle - lowValue) < Math.Abs(_controlAngle - highValue) ? lowIndex : highIndex;
                CurrentPosition = _positionIndex[closestIndex].Index;
            }
        }

        #endregion

        #region Actions

        void SetPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            BeginTriggerBypass(e.BypassCascadingTriggers);
            if (int.TryParse(e.Value.StringValue, out int index))
            {
                // WARNING: rotary switch positions are 1-based
                if (index > 0 && index <= Positions.Count)
                {
                    CurrentPosition = index;
                }
            }
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        void IncrementPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            int newPosition = CurrentPosition + 1;

            BeginTriggerBypass(e.BypassCascadingTriggers);
            // WARNING: rotary switch positions are 1-based
            if (newPosition <= Positions.Count)
            {
                CurrentPosition = newPosition;
            }
            else if (IsContinuous)
            {
                CurrentPosition = 1;
            }
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        void DecrementPositionAction_Execute(object action, HeliosActionEventArgs e)
        {
            int newPosition = CurrentPosition - 1;

            BeginTriggerBypass(e.BypassCascadingTriggers);
            // WARNING: rotary switch positions are 1-based
            if (newPosition >= 1)
            {
                CurrentPosition = newPosition;
            }
            else if (IsContinuous)
            {
                CurrentPosition = Positions.Count;
            }
            EndTriggerBypass(e.BypassCascadingTriggers);
        }

        #endregion

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            CurrentPosition = DefaultPosition;
            EndTriggerBypass(true);
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            base.WriteXml(writer);
            writer.WriteElementString("KnobImage", KnobImage);
            writer.WriteStartElement("Positions");
            foreach (RotarySwitchPosition position in Positions)
            {
                writer.WriteStartElement("Position");
                writer.WriteAttributeString("Name", position.Name);
                writer.WriteAttributeString("Rotation", position.Rotation.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteElementString("DefaultPosition", DefaultPosition.ToString(CultureInfo.InvariantCulture));
            if (DrawLines)
            {
                writer.WriteStartElement("Lines");
                writer.WriteElementString("Thickness", LineThickness.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Length", LineLength.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Color", colorConverter.ConvertToInvariantString(LineColor));
                writer.WriteEndElement();
            }
            if (DrawLabels)
            {
                writer.WriteStartElement("Labels");
                writer.WriteElementString("Color", colorConverter.ConvertToInvariantString(LabelColor));
                writer.WriteElementString("MaxWidth", MaxLabelWidth.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("MaxHeight", MaxLabelHeight.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Distance", LabelDistance.ToString(CultureInfo.InvariantCulture));
                LabelFormat.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (IsContinuous)
            {
                writer.WriteElementString("Continuous", true.ToString(CultureInfo.InvariantCulture));
            }
            WriteOptionalXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            base.ReadXml(reader);
            KnobImage = reader.ReadElementString("KnobImage");
            if (!reader.IsEmptyElement)
            {
                Positions.Clear();
                reader.ReadStartElement("Positions");
                int i = 1;
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    Positions.Add(new RotarySwitchPosition(this, i++, reader.GetAttribute("Name"), Double.Parse(reader.GetAttribute("Rotation"), CultureInfo.InvariantCulture)));
                    reader.Read();
                }
                reader.ReadEndElement();
            }
            else
            {
                reader.Read();
            }
            DefaultPosition = int.Parse(reader.ReadElementString("DefaultPosition"), CultureInfo.InvariantCulture);

            if (reader.Name.Equals("Lines"))
            {
                DrawLines = true;
                reader.ReadStartElement("Lines");
                LineThickness = double.Parse(reader.ReadElementString("Thickness"), CultureInfo.InvariantCulture);
                LineLength = double.Parse(reader.ReadElementString("Length"), CultureInfo.InvariantCulture);
                LineColor = (Color)colorConverter.ConvertFromInvariantString(reader.ReadElementString("Color"));
                reader.ReadEndElement();
            }
            else
            {
                DrawLines = false;
            }

            if (reader.Name.Equals("Labels"))
            {
                DrawLabels = true;
                reader.ReadStartElement("Labels");
                LabelColor = (Color)colorConverter.ConvertFromInvariantString(reader.ReadElementString("Color"));
                MaxLabelWidth = double.Parse(reader.ReadElementString("MaxWidth"), CultureInfo.InvariantCulture);
                MaxLabelHeight = double.Parse(reader.ReadElementString("MaxHeight"), CultureInfo.InvariantCulture);
                LabelDistance = double.Parse(reader.ReadElementString("Distance"), CultureInfo.InvariantCulture);
                LabelFormat.ReadXml(reader);
                reader.ReadEndElement();
            }
            else
            {
                DrawLabels = false;
            }

            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            if (reader.Name.Equals("Continuous"))
            {
                IsContinuous = (bool)bc.ConvertFromInvariantString(reader.ReadElementString());
            }
            ReadOptionalXml(reader);

            BeginTriggerBypass(true);
            CurrentPosition = DefaultPosition;
            EndTriggerBypass(true);
        }

        #region Overrides of HeliosVisual

        public override void ScaleChildren(double scaleX, double scaleY)
        {
            if (GlobalOptions.HasScaleAllText)
            {
                LabelFormat.FontSize *= Math.Max(scaleX, scaleY);
            }
            base.ScaleChildren(scaleX, scaleY);
        }

        #endregion
    }
}