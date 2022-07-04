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

    [HeliosControl("Helios.Base.LinearPotentiometerDetentsAnimated", "Linear Potentiometer with Detents (Animated)", "Potentiometers", typeof( KineographRenderer ) )]
    public class LinearPotentiometerDetentsAnimated : LinearPotentiometerAnimated
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //private double _initialValue = 0.0d;
        //private double _initialPosition = 0d;

        private bool _clickableVertical = false;
        private bool _clickableHorizontal = false;
        LinearClickType _clickType = LinearClickType.Swipe;


        private bool _mouseDown = false;
        private Point _mouseDownLocation;
        private double _swipeThreshold = 10d;
        private List<double> _detents = new List<double>();
        private bool _detentHit = false;
        private int _currentDetentPosition = 0;
        private HeliosTrigger _minValueTrigger;

        public LinearPotentiometerDetentsAnimated( )
            : base( "Linear Potentiometer with Detents (Animated)", new Size( 73, 240 ) )
        {
            _clickableVertical = true;
            ClickType = LinearClickType.Swipe;
            _detents.Sort();
            _minValueTrigger = new HeliosTrigger(this, "", "minimum value position", "released", "Fires when potentiometer moves out of MinValue position");
            this.Triggers.Add(_minValueTrigger);
        }

        #region Properties
        public List<double> DetentPositions
        {
            get => _detents;
            set
            {
                _detents = value;
                _detents.Sort();
                OnPropertyChanged("DetentPositions", value, _detents, true);
            }
        }
        public void AddDetent(double value)
        {
            if (!_detents.Contains(value))
            {
                _detents.Add(value);
                if (this.Triggers.Count < 1 + (_detents.Count - 2) * 2)
                {
                    this.Triggers.Add(new HeliosTrigger(this, "", $"detent { _detents.Count - 2  }", "holding", "Fires when potentiometer stopped at detent position", "true when detent encoutered, false when leaving a detent", BindingValueUnits.Boolean));
                    this.Triggers.Add(new HeliosTrigger(this, "", $"detent { _detents.Count - 2  }", "released", "Fires when potentiometer released from detent position", "+1 if increasing, -1 if decreasing", BindingValueUnits.Numeric));
                }
            }
            _detents.Sort();
            OnPropertyChanged("DetentPosition", -999d, value, true);
        }
        public void RemoveDetent(double value)
        {
            if (_detents.Contains(value))
            {
                _detents.Remove(value);
                if (this.Triggers.Count > 1 + (_detents.Count - 2) * 2)
                {
                    this.Triggers.RemoveAt(this.Triggers.Count - 1);
                    this.Triggers.RemoveAt(this.Triggers.Count - 1);
                }
            }
            OnPropertyChanged("DetentPosition", value, -999d, true);
        }

        #endregion

        #region Actions
        void SetValuePotentionmeter_Execute(object action, HeliosActionEventArgs e)
        {
            double maxImage = AnimationFrameCount - 1;
            Value = e.Value.DoubleValue;
            AnimationFrameNumber = Convert.ToInt32(Clamp(Math.Round(e.Value.DoubleValue * maxImage), 0, maxImage));
        }
        #endregion

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "MaxValue":
                    if (args.OldValue != args.NewValue)
                    {
                        _detents.Sort();
                        double scaleFactor = ((double)args.NewValue - MinValue) / ((double)args.OldValue - MinValue);
                        for (int i= 0;i < _detents.Count;i++)
                        {
                            _detents[i] = Math.Round((_detents[i] - MinValue) * scaleFactor + MinValue, 3);
                        }
                    }
                    break;
                case "MinValue":
                    if (args.OldValue != args.NewValue)
                    {
                        _detents.Sort();
                        double scaleFactor = (MaxValue - (double)args.NewValue) / (MaxValue - (double)args.OldValue);
                        for (int i = 0; i < _detents.Count; i++)
                        {
                            _detents[i] = Math.Round((_detents[i] - (double)args.OldValue)  * scaleFactor + (double)args.NewValue, 3);
                        }
                    }
                    break;
                default:
                    break;
            }
            base.OnPropertyChanged(args);
        }
        public override void Reset ( )
        {
            base.Reset();

            BeginTriggerBypass(true);
            Value = InitialValue;
            EndTriggerBypass(true);
        }
        public override void WriteXml ( XmlWriter writer )
        {
            base.WriteXml( writer );
            writer.WriteStartElement("DetentPositions");
            _detents.Sort();
            foreach (double position in _detents)
            {
                writer.WriteStartElement("DetentPosition");
                writer.WriteAttributeString("Position", position.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        public override void ReadXml ( XmlReader reader )
        {
            base.ReadXml( reader );
            if (!reader.IsEmptyElement)
            {
                _detents.Clear();
                reader.ReadStartElement("DetentPositions");
                int i = 1;
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    DetentPositions.Add(Double.Parse(reader.GetAttribute("Position"), CultureInfo.InvariantCulture));
                    this.Triggers.Add(new HeliosTrigger(this, "", $"detent { i }", "holding", "Fires when potentiometer stopped at detent position","true when detent encoutered, false when leaving a detent",BindingValueUnits.Boolean));
                    this.Triggers.Add(new HeliosTrigger(this, "", $"detent { i++ }", "released", "Fires when potentiometer released from detent position","+1 if increasing, -1 if decreasing",BindingValueUnits.Numeric));
                    reader.Read();
                }
                reader.ReadEndElement();
            }
            else
            {
                reader.Read();
            }
            _detents.Sort();
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
            _detentHit = false;
        }
        protected override void CalculateMovement(double pulses)
        {
            double dragProportion = pulses / this.Height * MaxValue * -1;
            // we want to find out if the drag takes us through a detent, and clamp if it will
            double currentValue = Value;
            double detentMinValue = MinValue;
            double detentMaxValue = MaxValue;
            if (!_detents.Contains(MinValue)) _detents.Add(MinValue);
            if (!_detents.Contains(MaxValue)) _detents.Add(MaxValue);
            _detents.Sort();
            if (!_detentHit && !BypassTriggers)
            {
                for (int i = 0; i < _detents.Count; i++)
                {
                    if (_detents[i] == currentValue)
                    {
                        int j = 0;
                        foreach (IBindingTrigger ibt in this.Triggers)
                        {
                            if (ibt.Name == $"detent {i}")
                            {
                                HeliosTrigger hTrigger = ibt as HeliosTrigger;
                                if (hTrigger.TriggerVerb == "released")
                                {
                                    hTrigger.FireTrigger(new BindingValue(dragProportion > 0 ? -1 : 1));
                                    j++;
                                }
                                if (hTrigger.TriggerVerb == "holding")
                                {
                                    hTrigger.FireTrigger(new BindingValue(false));
                                    j++;
                                }
                            }
                            if (j == 2) break;
                        }
                        break;
                    }
                }
            }
            if (dragProportion > 0)
            {
                double previousDetent = MaxValue;
                foreach (double detent in _detents)
                {
                    if (currentValue >= previousDetent && currentValue <= detent)
                    {
                        detentMaxValue = detent;
                        detentMinValue = previousDetent;
                        if (currentValue != detent) break;
                        if (currentValue == previousDetent) break;
                    }
                    previousDetent = detent;
                }
            }
            else
            {
                _detents.Reverse();
                double previousDetent = MinValue;
                foreach (double detent in _detents)
                {
                    if (currentValue >= detent && currentValue <= previousDetent)
                    {
                        detentMinValue = detent;
                        detentMaxValue = previousDetent;
                        if (currentValue != detent) break;
                        if (currentValue == previousDetent) break;
                    }
                    previousDetent = detent;
                }
                _detents.Sort();
            }       
            if(!_detentHit) Value = Math.Round(Math.Max(Math.Min(Value + dragProportion, detentMaxValue), detentMinValue), 3);
            if (Value == detentMinValue || Value == detentMaxValue) 
            {
                _detentHit = true;
                if (!BypassTriggers)
                {
                    for (int i = 0; i < _detents.Count; i++)
                    {
                        if (_detents[i] == Value)
                        {
                            foreach (IBindingTrigger ibt in this.Triggers)
                            {
                                if (ibt.Name == $"detent {i}" && ibt.TriggerVerb == "holding")
                                {
                                    HeliosTrigger hTrigger = ibt as HeliosTrigger;
                                    hTrigger.FireTrigger(new BindingValue(true));
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            } else
            {
                _detentHit = false;
            }
            if (currentValue == MinValue && Value != MinValue)
            {
                _minValueTrigger.FireTrigger(BindingValue.Empty);
            }
            Logger.Debug($"OldValue {currentValue} NewValue {Value} Min {detentMinValue} Max {detentMaxValue} ");
            AnimationFrameNumber = Convert.ToInt32(Clamp(Math.Round(Value * (AnimationFrameCount - 1)), 0, AnimationFrameCount - 1));
            if (_detents.Contains(MinValue)) _detents.Remove(MinValue);
            if (_detents.Contains(MaxValue)) _detents.Remove(MaxValue);

        }

        public int CurrentPosition
        {
            get => _currentDetentPosition;
            set => _currentDetentPosition = value;
        }
        private double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value ;
        }
    }
}
