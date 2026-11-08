//  Copyright 2014 Craig Courtney
//  Copyright 2026 Helios Contributors
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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using GadrocsWorkshop.Helios.Windows.Controls;

namespace GadrocsWorkshop.Helios.Controls
{
    /// <summary>
    /// which potentiometer of a DualPotentiometer, if any, is carried around by the other one
    ///
    /// WARNING: these value names must never be changed, as they are serialized to Profile XML
    /// </summary>
    public enum DualPotentiometerFollowMode
    {
        [Description("Pot 2 follows Pot 1")]
        Potentiometer2FollowsPotentiometer1,

        [Description("Pot 1 follows Pot 2")]
        Potentiometer1FollowsPotentiometer2,

        [Description("Independent")]
        None
    }

    /// <summary>
    /// Two concentric rotating images, each driven by its own input value in the same way
    /// a potentiometer is (min value, max value, initial rotation, rotation travel).
    ///
    /// Potentiometer 1 is shown to the user as "Pot 1 (Lower)" and Potentiometer 2 as "Pot 2 (Upper)".
    /// Either one can be carried around by the other while its own value turns it in either direction
    /// on top of that (for example the F-16 airspeed pointer and the Mach dial that rides with it).
    ///
    /// The mouse wheel turns Potentiometer 2 while the pointer is over its image, and Potentiometer 1
    /// anywhere else on the control.
    /// </summary>
    [HeliosControl("Helios.Base.DualPotentiometer", "Dual Potentiometer", "Potentiometers", typeof(DualPotentiometerRenderer))]
    public class DualPotentiometer : HeliosVisual, IConfigurableImageLocation, IRefreshableImage
    {
        private const string DefaultPotentiometer1Name = "pot 1";
        private const string DefaultPotentiometer2Name = "pot 2";

        private bool _mouseWheelAction = true;

        private string _potentiometer1Name = DefaultPotentiometer1Name;
        private string _potentiometer1Image = "{Helios}/Images/Knobs/knob1.png";
        private double _potentiometer1InitialValue;
        private double _potentiometer1StepValue = 0.1d;
        private double _potentiometer1MinValue;
        private double _potentiometer1MaxValue = 1d;
        private double _potentiometer1InitialRotation;
        private double _potentiometer1RotationTravel = 360d;
        private bool _potentiometer1CounterClockwise;
        private double _potentiometer1Scale = 1d;

        private string _potentiometer2Name = DefaultPotentiometer2Name;
        private string _potentiometer2Image = "{Helios}/Images/Knobs/knob1.png";
        private double _potentiometer2InitialValue;
        private double _potentiometer2StepValue = 0.1d;
        private double _potentiometer2MinValue;
        private double _potentiometer2MaxValue = 1d;
        private double _potentiometer2InitialRotation;
        private double _potentiometer2RotationTravel = 360d;
        private bool _potentiometer2CounterClockwise = true;
        private double _potentiometer2Scale = 0.5d;

        private DualPotentiometerFollowMode _followMode = DualPotentiometerFollowMode.Potentiometer2FollowsPotentiometer1;
        private bool _potentiometer2OnTop = true;

        private double _potentiometer1Rotation;
        private double _potentiometer2Rotation;

        private readonly HeliosValue _potentiometer1Value;
        private readonly HeliosValue _potentiometer2Value;

        public DualPotentiometer()
            : base("Dual Potentiometer", new Size(200, 200))
        {
            _potentiometer1Value = new HeliosValue(this, new BindingValue(0d), "", "potentiometer 1 value", "potentiometer 1 value",
                "Current value of pot 1 (the lower pot).", "", BindingValueUnits.Numeric);
            _potentiometer1Value.Execute += Potentiometer1Value_Execute;
            Values.Add(_potentiometer1Value);
            Actions.Add(_potentiometer1Value);
            Triggers.Add(_potentiometer1Value);

            _potentiometer2Value = new HeliosValue(this, new BindingValue(0d), "", "potentiometer 2 value", "potentiometer 2 value",
                "Current value of pot 2 (the upper pot).", "", BindingValueUnits.Numeric);
            _potentiometer2Value.Execute += Potentiometer2Value_Execute;
            Values.Add(_potentiometer2Value);
            Actions.Add(_potentiometer2Value);
            Triggers.Add(_potentiometer2Value);

            UpdateValueNames();

            UpdateRotations();
        }

        #region Potentiometer 1 (lower) properties

        /// <summary>
        /// Name of potentiometer 1 as it is shown in the bindings lists.  Changing it does not break existing bindings.
        /// </summary>
        public string Potentiometer1Name
        {
            get => _potentiometer1Name;
            set
            {
                string newValue = string.IsNullOrWhiteSpace(value) ? DefaultPotentiometer1Name : value.Trim();
                if (_potentiometer1Name.Equals(newValue))
                {
                    return;
                }
                string oldValue = _potentiometer1Name;
                _potentiometer1Name = newValue;
                UpdateValueNames();
                OnPropertyChanged(nameof(Potentiometer1Name), oldValue, newValue, true);
            }
        }

        public string Potentiometer1Image
        {
            get => _potentiometer1Image;
            set => SetImage(ref _potentiometer1Image, value, nameof(Potentiometer1Image));
        }

        public double Potentiometer1InitialValue
        {
            get => _potentiometer1InitialValue;
            set => SetConfig(ref _potentiometer1InitialValue, value, nameof(Potentiometer1InitialValue));
        }

        /// <summary>
        /// Amount the value of this pot changes for each click of the mouse wheel
        /// </summary>
        public double Potentiometer1StepValue
        {
            get => _potentiometer1StepValue;
            set => SetConfig(ref _potentiometer1StepValue, value, nameof(Potentiometer1StepValue));
        }

        public double Potentiometer1MinValue
        {
            get => _potentiometer1MinValue;
            set => SetConfig(ref _potentiometer1MinValue, value, nameof(Potentiometer1MinValue));
        }

        public double Potentiometer1MaxValue
        {
            get => _potentiometer1MaxValue;
            set => SetConfig(ref _potentiometer1MaxValue, value, nameof(Potentiometer1MaxValue));
        }

        /// <summary>
        /// Rotation in degrees of potentiometer 1 when both pots are at their minimum value
        /// </summary>
        public double Potentiometer1InitialRotation
        {
            get => _potentiometer1InitialRotation;
            set => SetConfig(ref _potentiometer1InitialRotation, value, nameof(Potentiometer1InitialRotation));
        }

        /// <summary>
        /// Degrees potentiometer 1 turns between its minimum and maximum value
        /// </summary>
        public double Potentiometer1RotationTravel
        {
            get => _potentiometer1RotationTravel;
            set => SetConfig(ref _potentiometer1RotationTravel, value, nameof(Potentiometer1RotationTravel));
        }

        public bool Potentiometer1CounterClockwise
        {
            get => _potentiometer1CounterClockwise;
            set => SetConfig(ref _potentiometer1CounterClockwise, value, nameof(Potentiometer1CounterClockwise));
        }

        /// <summary>
        /// Size of the potentiometer 1 image relative to the control (1 = fills the control)
        /// </summary>
        public double Potentiometer1Scale
        {
            get => _potentiometer1Scale;
            set
            {
                if (SetConfig(ref _potentiometer1Scale, value, nameof(Potentiometer1Scale)))
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// UI access to the current value of potentiometer 1; writes do not create Undo events
        /// </summary>
        public double Potentiometer1Value
        {
            get => _potentiometer1Value.Value.DoubleValue;
            set
            {
                double oldValue = _potentiometer1Value.Value.DoubleValue;
                if (oldValue.Equals(value))
                {
                    return;
                }
                _potentiometer1Value.SetValue(new BindingValue(value), BypassTriggers);
                OnPropertyChanged(nameof(Potentiometer1Value), oldValue, value, false);
                UpdateRotations();
            }
        }

        #endregion

        #region Potentiometer 2 (upper) properties

        /// <summary>
        /// Name of potentiometer 2 as it is shown in the bindings lists.  Changing it does not break existing bindings.
        /// </summary>
        public string Potentiometer2Name
        {
            get => _potentiometer2Name;
            set
            {
                string newValue = string.IsNullOrWhiteSpace(value) ? DefaultPotentiometer2Name : value.Trim();
                if (_potentiometer2Name.Equals(newValue))
                {
                    return;
                }
                string oldValue = _potentiometer2Name;
                _potentiometer2Name = newValue;
                UpdateValueNames();
                OnPropertyChanged(nameof(Potentiometer2Name), oldValue, newValue, true);
            }
        }

        public string Potentiometer2Image
        {
            get => _potentiometer2Image;
            set => SetImage(ref _potentiometer2Image, value, nameof(Potentiometer2Image));
        }

        public double Potentiometer2InitialValue
        {
            get => _potentiometer2InitialValue;
            set => SetConfig(ref _potentiometer2InitialValue, value, nameof(Potentiometer2InitialValue));
        }

        /// <summary>
        /// Amount the value of this pot changes for each click of the mouse wheel
        /// </summary>
        public double Potentiometer2StepValue
        {
            get => _potentiometer2StepValue;
            set => SetConfig(ref _potentiometer2StepValue, value, nameof(Potentiometer2StepValue));
        }

        public double Potentiometer2MinValue
        {
            get => _potentiometer2MinValue;
            set => SetConfig(ref _potentiometer2MinValue, value, nameof(Potentiometer2MinValue));
        }

        public double Potentiometer2MaxValue
        {
            get => _potentiometer2MaxValue;
            set => SetConfig(ref _potentiometer2MaxValue, value, nameof(Potentiometer2MaxValue));
        }

        /// <summary>
        /// Rotation in degrees of potentiometer 2 when both pots are at their minimum value
        /// </summary>
        public double Potentiometer2InitialRotation
        {
            get => _potentiometer2InitialRotation;
            set => SetConfig(ref _potentiometer2InitialRotation, value, nameof(Potentiometer2InitialRotation));
        }

        /// <summary>
        /// Degrees potentiometer 2 turns, relative to whatever is carrying it, between its minimum and maximum value
        /// </summary>
        public double Potentiometer2RotationTravel
        {
            get => _potentiometer2RotationTravel;
            set => SetConfig(ref _potentiometer2RotationTravel, value, nameof(Potentiometer2RotationTravel));
        }

        public bool Potentiometer2CounterClockwise
        {
            get => _potentiometer2CounterClockwise;
            set => SetConfig(ref _potentiometer2CounterClockwise, value, nameof(Potentiometer2CounterClockwise));
        }

        /// <summary>
        /// Size of the potentiometer 2 image relative to the control (1 = fills the control)
        /// </summary>
        public double Potentiometer2Scale
        {
            get => _potentiometer2Scale;
            set
            {
                if (SetConfig(ref _potentiometer2Scale, value, nameof(Potentiometer2Scale)))
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Selects which potentiometer is carried around by the other one.  The following potentiometer's own value
        /// turns it relative to the potentiometer it follows.
        /// </summary>
        public DualPotentiometerFollowMode FollowMode
        {
            get => _followMode;
            set => SetConfig(ref _followMode, value, nameof(FollowMode));
        }

        /// <summary>
        /// If true, potentiometer 2 is drawn over potentiometer 1, otherwise under it
        /// </summary>
        public bool Potentiometer2OnTop
        {
            get => _potentiometer2OnTop;
            set => SetConfig(ref _potentiometer2OnTop, value, nameof(Potentiometer2OnTop));
        }

        /// <summary>
        /// UI access to the current value of potentiometer 2; writes do not create Undo events
        /// </summary>
        public double Potentiometer2Value
        {
            get => _potentiometer2Value.Value.DoubleValue;
            set
            {
                double oldValue = _potentiometer2Value.Value.DoubleValue;
                if (oldValue.Equals(value))
                {
                    return;
                }
                _potentiometer2Value.SetValue(new BindingValue(value), BypassTriggers);
                OnPropertyChanged(nameof(Potentiometer2Value), oldValue, value, false);
                UpdateRotations();
            }
        }

        #endregion

        #region Rendered state

        /// <summary>
        /// Current rotation in degrees at which potentiometer 1 is drawn, including any rotation it inherits from potentiometer 2
        /// </summary>
        public double Potentiometer1Rotation => _potentiometer1Rotation;

        /// <summary>
        /// Current rotation in degrees at which potentiometer 2 is drawn, including any rotation it inherits from potentiometer 1
        /// </summary>
        public double Potentiometer2Rotation => _potentiometer2Rotation;

        #endregion

        #region Actions

        private void Potentiometer1Value_Execute(object action, HeliosActionEventArgs e)
        {
            _potentiometer1Value.SetValue(e.Value, e.BypassCascadingTriggers);
            UpdateRotations();
        }

        private void Potentiometer2Value_Execute(object action, HeliosActionEventArgs e)
        {
            _potentiometer2Value.SetValue(e.Value, e.BypassCascadingTriggers);
            UpdateRotations();
        }

        #endregion

        /// <summary>
        /// Degrees a potentiometer has turned away from its initial rotation for the given value
        /// </summary>
        private static double Deflection(double value, double min, double max, double travel, bool counterClockwise)
        {
            double span = max - min;
            if (span.Equals(0d) || double.IsNaN(value))
            {
                return 0d;
            }
            double fraction = Math.Max(0d, Math.Min(1d, (value - min) / span));
            return (counterClockwise ? -1d : 1d) * fraction * travel;
        }

        private void UpdateRotations()
        {
            double deflection1 = Deflection(_potentiometer1Value.Value.DoubleValue, _potentiometer1MinValue, _potentiometer1MaxValue,
                _potentiometer1RotationTravel, _potentiometer1CounterClockwise);
            double deflection2 = Deflection(_potentiometer2Value.Value.DoubleValue, _potentiometer2MinValue, _potentiometer2MaxValue,
                _potentiometer2RotationTravel, _potentiometer2CounterClockwise);

            double rotation1 = _potentiometer1InitialRotation + deflection1 +
                               (_followMode == DualPotentiometerFollowMode.Potentiometer1FollowsPotentiometer2 ? deflection2 : 0d);
            double rotation2 = _potentiometer2InitialRotation + deflection2 +
                               (_followMode == DualPotentiometerFollowMode.Potentiometer2FollowsPotentiometer1 ? deflection1 : 0d);

            if (!rotation1.Equals(_potentiometer1Rotation) || !rotation2.Equals(_potentiometer2Rotation))
            {
                _potentiometer1Rotation = rotation1;
                _potentiometer2Rotation = rotation2;
                OnDisplayUpdate();
            }
        }

        /// <summary>
        /// Applies the potentiometer names to the names shown for the bindable values.  The identifiers used
        /// to save bindings in the profile are fixed, so bindings survive a rename.
        /// </summary>
        private void UpdateValueNames()
        {
            _potentiometer1Value.Name = _potentiometer1Name + " value";
            _potentiometer1Value.RecalculateName();
            _potentiometer2Value.Name = _potentiometer2Name + " value";
            _potentiometer2Value.RecalculateName();
        }

        private bool SetConfig<T>(ref T field, T value, string propertyName)
        {
            if (Equals(field, value))
            {
                return false;
            }
            T oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value, true);
            UpdateRotations();
            OnDisplayUpdate();
            return true;
        }

        private void SetImage(ref string field, string value, string propertyName)
        {
            if (string.Equals(field, value))
            {
                return;
            }
            string oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value, true);
            Refresh();
        }

        #region IConfigurableImageLocation

        public void ReplaceImageNames(string oldName, string newName)
        {
            Potentiometer1Image = ReplaceImageName(Potentiometer1Image, oldName, newName);
            Potentiometer2Image = ReplaceImageName(Potentiometer2Image, oldName, newName);
        }

        private static string ReplaceImageName(string image, string oldName, string newName)
        {
            return string.IsNullOrEmpty(image) ? image : string.IsNullOrEmpty(oldName) ? newName + image : image.Replace(oldName, newName);
        }

        #endregion

        #region IRefreshableImage

        public override bool ConditionalImageRefresh(string imageName)
        {
            if ((Potentiometer1Image ?? "").ToLower().Replace("/", @"\") == imageName ||
                (Potentiometer2Image ?? "").ToLower().Replace("/", @"\") == imageName)
            {
                ImageRefresh = true;
                Refresh();
            }
            return ImageRefresh;
        }

        #endregion

        public override void Reset()
        {
            base.Reset();

            BeginTriggerBypass(true);
            Potentiometer1Value = Potentiometer1InitialValue;
            Potentiometer2Value = Potentiometer2InitialValue;
            EndTriggerBypass(true);
            UpdateRotations();
        }

        #region Mouse wheel

        /// <summary>
        /// If true, the mouse wheel turns Pot 2 while the pointer is over the Pot 2 image and Pot 1 anywhere else
        /// </summary>
        public bool MouseWheelAction
        {
            get => _mouseWheelAction;
            set => SetConfig(ref _mouseWheelAction, value, nameof(MouseWheelAction));
        }

        public override bool CanConsumeMouseWheel => _mouseWheelAction;

        public override void MouseWheel(int delta)
        {
            if (!_mouseWheelAction || delta == 0)
            {
                return;
            }

            // NOTE: ignoring mouse wheel speed because unit is unclear, same as the other rotary controls
            int pulses = delta < 1 ? -1 : 1;
            if (IsPointerOverPotentiometer2())
            {
                Potentiometer2Value = Stepped(Potentiometer2Value, pulses * _potentiometer2StepValue, _potentiometer2MinValue, _potentiometer2MaxValue);
            }
            else
            {
                Potentiometer1Value = Stepped(Potentiometer1Value, pulses * _potentiometer1StepValue, _potentiometer1MinValue, _potentiometer1MaxValue);
            }
        }

        private static double Stepped(double value, double change, double min, double max)
        {
            double low = Math.Min(min, max);
            double high = Math.Max(min, max);
            return Math.Round(Math.Max(low, Math.Min(high, value + change)), 5);
        }

        /// <summary>
        /// The mouse wheel notification does not include a location, so we ask WPF where the pointer is
        /// relative to the view that is displaying this control, and then test that against the
        /// area covered by the Pot 2 image.
        /// </summary>
        private bool IsPointerOverPotentiometer2()
        {
            DependencyObject element = Mouse.DirectlyOver as DependencyObject;
            while (element != null)
            {
                if (element is HeliosVisualView view && ReferenceEquals(view.Visual, this))
                {
                    return IsOverPotentiometer2(Mouse.GetPosition(view));
                }
                element = element is Visual || element is System.Windows.Media.Media3D.Visual3D
                    ? VisualTreeHelper.GetParent(element)
                    : null;
            }

            // pointer location is not available, so treat it as being outside of Pot 2
            return false;
        }

        /// <param name="location">location relative to the upper left corner of this control</param>
        /// <returns>true if the location is inside the ellipse covered by the Pot 2 image</returns>
        private bool IsOverPotentiometer2(Point location)
        {
            double radiusX = Width * _potentiometer2Scale / 2d;
            double radiusY = Height * _potentiometer2Scale / 2d;
            if (radiusX <= 0d || radiusY <= 0d)
            {
                return false;
            }
            double x = (location.X - Width / 2d) / radiusX;
            double y = (location.Y - Height / 2d) / radiusY;
            return x * x + y * y <= 1d;
        }

        #endregion

        // this control does not react to touch or mouse buttons

        public override void MouseDown(Point location)
        {
            // no code
        }

        public override void MouseDrag(Point location)
        {
            // no code
        }

        public override void MouseUp(Point location)
        {
            // no code
        }

        #region Persistence

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteStartElement("Potentiometer1");
            WritePotentiometer(writer, Potentiometer1Name, Potentiometer1Image, Potentiometer1InitialValue, Potentiometer1MinValue, Potentiometer1MaxValue,
                Potentiometer1InitialRotation, Potentiometer1RotationTravel, Potentiometer1CounterClockwise, Potentiometer1Scale, Potentiometer1StepValue);
            writer.WriteEndElement();
            writer.WriteStartElement("Potentiometer2");
            WritePotentiometer(writer, Potentiometer2Name, Potentiometer2Image, Potentiometer2InitialValue, Potentiometer2MinValue, Potentiometer2MaxValue,
                Potentiometer2InitialRotation, Potentiometer2RotationTravel, Potentiometer2CounterClockwise, Potentiometer2Scale, Potentiometer2StepValue);
            writer.WriteElementString("OnTop", Potentiometer2OnTop.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            writer.WriteElementString("FollowMode", FollowMode.ToString());
            writer.WriteElementString("MouseWheel", MouseWheelAction.ToString(CultureInfo.InvariantCulture));
        }

        private static void WritePotentiometer(XmlWriter writer, string name, string image, double initialValue, double minValue,
            double maxValue, double initialRotation, double rotationTravel, bool counterClockwise, double scale, double stepValue)
        {
            writer.WriteElementString("Name", name);
            writer.WriteElementString("Image", image ?? "");
            writer.WriteElementString("InitialValue", initialValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("MinValue", minValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("MaxValue", maxValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("InitialRotation", initialRotation.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("RotationTravel", rotationTravel.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("CounterClockwise", counterClockwise.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("Scale", scale.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("StepValue", stepValue.ToString(CultureInfo.InvariantCulture));
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            TypeConverter bc = TypeDescriptor.GetConverter(typeof(bool));

            reader.ReadStartElement("Potentiometer1");
            if (reader.Name.Equals("Name"))
            {
                Potentiometer1Name = reader.ReadElementString("Name");
            }
            Potentiometer1Image = reader.ReadElementString("Image");
            Potentiometer1InitialValue = ReadDouble(reader, "InitialValue");
            Potentiometer1MinValue = ReadDouble(reader, "MinValue");
            Potentiometer1MaxValue = ReadDouble(reader, "MaxValue");
            Potentiometer1InitialRotation = ReadDouble(reader, "InitialRotation");
            Potentiometer1RotationTravel = ReadDouble(reader, "RotationTravel");
            Potentiometer1CounterClockwise = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("CounterClockwise"));
            Potentiometer1Scale = ReadDouble(reader, "Scale");
            if (reader.Name.Equals("StepValue"))
            {
                Potentiometer1StepValue = ReadDouble(reader, "StepValue");
            }
            reader.ReadEndElement();

            reader.ReadStartElement("Potentiometer2");
            if (reader.Name.Equals("Name"))
            {
                Potentiometer2Name = reader.ReadElementString("Name");
            }
            Potentiometer2Image = reader.ReadElementString("Image");
            Potentiometer2InitialValue = ReadDouble(reader, "InitialValue");
            Potentiometer2MinValue = ReadDouble(reader, "MinValue");
            Potentiometer2MaxValue = ReadDouble(reader, "MaxValue");
            Potentiometer2InitialRotation = ReadDouble(reader, "InitialRotation");
            Potentiometer2RotationTravel = ReadDouble(reader, "RotationTravel");
            Potentiometer2CounterClockwise = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("CounterClockwise"));
            Potentiometer2Scale = ReadDouble(reader, "Scale");
            if (reader.Name.Equals("StepValue"))
            {
                Potentiometer2StepValue = ReadDouble(reader, "StepValue");
            }
            Potentiometer2OnTop = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("OnTop"));
            reader.ReadEndElement();
            if (reader.Name.Equals("FollowMode"))
            {
                FollowMode = (DualPotentiometerFollowMode)Enum.Parse(typeof(DualPotentiometerFollowMode), reader.ReadElementString("FollowMode"));
            }
            if (reader.Name.Equals("MouseWheel"))
            {
                MouseWheelAction = (bool)bc.ConvertFromInvariantString(reader.ReadElementString("MouseWheel"));
            }

            BeginTriggerBypass(true);
            Potentiometer1Value = Potentiometer1InitialValue;
            Potentiometer2Value = Potentiometer2InitialValue;
            EndTriggerBypass(true);
            UpdateRotations();
        }

        private static double ReadDouble(XmlReader reader, string elementName)
        {
            return double.Parse(reader.ReadElementString(elementName), CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
