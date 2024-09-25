//  Copyright 2014 Craig Courtney
//  Copyright 2020 Ammo Goettsch
//  Copyright 2020 Helios Contributors
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

using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace GadrocsWorkshop.Helios.Controls
{
    using System;
    using System.Windows;

    /// <summary>
    /// base class for most rotary controls sharing the same interaction styles
    /// </summary>
    public abstract class Rotary : HeliosVisual, IRotaryBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private bool _mouseDown;

        private RotaryClickType _clickType = RotaryClickType.Swipe;

        private bool _mouseWheelAction = true;

        /// <summary>
        /// current mouse/touch interaction, or null
        /// </summary>
        private IRotaryInteraction _interaction;

        /// <summary>
        /// relative sensitivity from -1 to 1, with 0 being default
        /// </summary>
        private double _sensitivity;

        /// <summary>
        /// backing field for property VisualizeInteraction, contains
        /// true if interaction style is allowed to draw additional visual
        /// representations of interaction
        /// </summary>
        private bool _visualizeInteraction;

        /// <summary>
        /// backing field for property Routable, contains
        /// true if this control can be attached to a Control Router by selecting it and then
        /// providing input into the Control Router
        /// </summary>
        private bool _routable = true;

        protected Rotary(string name, Size defaultSize)
            : base(name, defaultSize)
        {
            // no code
        }

        #region Properties

        public RotaryClickType ClickType
        {
            get => _clickType;
            set
            {
                if (_clickType.Equals(value))
                {
                    return;
                }

                RotaryClickType oldValue = _clickType;
                _clickType = value;
                OnPropertyChanged("ClickType", oldValue, value, true);
                if (value == RotaryClickType.Radial)
                {
                    VisualizeInteraction = true;
                }
            }
        }

        public virtual bool IsPushed
        {
            get => false;
            set {}
        }

        /// <summary>
        /// this is the relative sensitivity for multiple interaction styles, but it is called
        /// "Sensitivity" to remain compatible with config, legacy classes, and the UI
        /// </summary>
        public double Sensitivity
        {
            get => _sensitivity;
            set
            {
                if (_sensitivity.Equals(value))
                {
                    return;
                }

                double oldValue = _sensitivity;
                _sensitivity = value;
                OnPropertyChanged("Sensitivity", oldValue, value, true);
            }
        }

        public bool MouseWheelAction
        {
            get => _mouseWheelAction;
            set
            {
                if (_mouseWheelAction.Equals(value))
                {
                    return;
                }

                bool oldValue = _mouseWheelAction;
                _mouseWheelAction = value;
                OnPropertyChanged("MouseWheelAction", oldValue, value, true);
            }
        }

        /// <summary>
        /// true if interaction style is allowed to draw additional visual
        /// representations of interaction
        /// </summary>
        public bool VisualizeInteraction
        {
            get => _visualizeInteraction;
            set
            {
                if (_visualizeInteraction == value)
                {
                    return;
                }
                bool oldValue = _visualizeInteraction;
                _visualizeInteraction = value;
                OnPropertyChanged("VisualizeInteraction", oldValue, value, true);
            }
        }

        /// <summary>
        /// true if this control can be attached to a Control Router by selecting it and then
        /// providing input into the Control Router
        /// </summary>
        public bool Routable
        {
            get => _routable;
            set
            {
                if (_routable == value)
                {
                    return;
                }
                bool oldValue = _routable;
                _routable = value;
                OnPropertyChanged("Routable", oldValue, value, true);
            }
        }

        internal bool VisualizeDragging => VisualizeInteraction && (_interaction?.VisualizeDragging ?? false);

        // REVISIT: we don't currently advertise this visualization through IRotaryInteraction, because only one type supports it
        internal Point DragPoint => (_interaction as RadialRotaryInteraction)?.DragPoint ?? GenerateCenterPoint();

        public override bool CanConsumeMouseWheel => _mouseWheelAction;

        #endregion

        private Point GenerateCenterPoint() => new Point(DisplayRectangle.Width / 2, DisplayRectangle.Height / 2);

        public override void MouseDown(Point location)
        {
            _mouseDown = true;
            switch (_clickType)
            {
                case RotaryClickType.Touch:
                {
                    TouchRotaryInteraction touchInteraction = 
                        new TouchRotaryInteraction(ControlAngle, GenerateCenterPoint(), location, Sensitivity);
                    Pulse(touchInteraction.Pulses);
                    _interaction = touchInteraction;
                    if (Parent?.Profile != null)
                    {
                        Parent.Profile.ProfileTick += Profile_ProfileTick;
                    }
                    break;
                }
                case RotaryClickType.Swipe:
                    _interaction =
                        new SwipeRotaryInteraction(ControlAngle, GenerateCenterPoint(), location, Sensitivity);
                    break;
                case RotaryClickType.Radial:
                    _interaction =
                        new RadialRotaryInteraction(ControlAngle, GenerateCenterPoint(), location, Sensitivity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Routable)
            {
                Profile?.OnRoutableControlSelected(this);
            }
        }

        public override void MouseWheel(int delta)
        {
            if (_mouseWheelAction)
            {
                // NOTE: ignoring mouse wheel speed because unit is unclear
                Pulse(delta < 1 ? -1 : 1);
            }
        }

        private void Profile_ProfileTick(object sender, EventArgs e)
        {
            _interaction?.Update(this, GenerateCenterPoint());
        }

        public override void MouseDrag(Point location)
        {
            if (!_mouseDown)
            {
                return;
            }

            if (_interaction?.Update(this, location) ?? false)
            {
                // control already updated
                return;
            }

            if (VisualizeDragging)
            {
                // update the visualization
                OnDisplayUpdate();
            }
        }

        public override void MouseUp(Point location)
        {
            bool dirty = VisualizeDragging;

            _mouseDown = false;

            if (_clickType == RotaryClickType.Touch)
            {
                if (Parent?.Profile != null)
                {
                    Parent.Profile.ProfileTick -= Profile_ProfileTick;
                }
            }

            _interaction = null;

            // clean up drag visualization
            if (dirty)
            {
                Refresh();
            }
        }

        // helper to read XML for optional configuration that is shared across descendants
        protected virtual void ReadOptionalXml(XmlReader reader)
        {
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));
            while (reader.NodeType == XmlNodeType.Element && reader.Name != "Children")
            {
                switch (reader.Name)
                {
                    case "ClickType":
                        reader.ReadStartElement("ClickType");
                        ClickType = (RotaryClickType)Enum.Parse(typeof(RotaryClickType), reader.ReadElementString("Type"));
                        if (reader.Name == "Sensitivity")
                        {
                            Sensitivity = double.Parse(reader.ReadElementString("Sensitivity"), CultureInfo.InvariantCulture);
                        }
                        reader.ReadEndElement();
                        break;
                    case "MouseWheelAction":
                    case "MouseWheel":
                        MouseWheelAction = (bool)bc.ConvertFromInvariantString(reader.ReadElementString());
                        break;
                    case "Routable":
                        Routable = (bool)bc.ConvertFromInvariantString(reader.ReadElementString());
                        break;
                    case "VisualizeInteraction":
                        VisualizeInteraction = (bool)bc.ConvertFromInvariantString(reader.ReadElementString());
                        break;
                    default:
                        // ignore unsupported settings
                        string elementName = reader.Name;
                        string discard = reader.ReadInnerXml();
                        Logger.Warn($"Ignored unsupported {GetType().Name} setting '{elementName}' with value '{discard}'");
                        break;
                }
            }
        }

        // helper to writer XML for optional configuration that is shared across descendants
        protected virtual void WriteOptionalXml(XmlWriter writer)
        {
            if (ClickType != RotaryClickType.Swipe || Sensitivity != 0.0)
            {
                writer.WriteStartElement("ClickType");
                writer.WriteElementString("Type", ClickType.ToString());
                if (ClickType != RotaryClickType.Touch)
                {
                    writer.WriteElementString("Sensitivity", Sensitivity.ToString(CultureInfo.InvariantCulture));
                }
                writer.WriteEndElement();
            }
            if (!MouseWheelAction)
            {
                writer.WriteElementString("MouseWheel", MouseWheelAction.ToString(CultureInfo.InvariantCulture));
            }
            if (VisualizeInteraction || ClickType == RotaryClickType.Radial)
            {
                writer.WriteElementString("VisualizeInteraction", VisualizeInteraction.ToString(CultureInfo.InvariantCulture));
            }
            if (!Routable)
            {
                writer.WriteElementString("Routable", Routable.ToString(CultureInfo.InvariantCulture));
            }
        }

        #region IPulsedControl

        public abstract void Pulse(int pulses);

        #endregion

        #region IRotaryControl

        public abstract double ControlAngle { get; set; }

        #endregion
    }
}