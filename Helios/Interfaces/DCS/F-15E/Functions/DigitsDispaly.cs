//  Copyright 2014 Craig Courtney
//  Copyright 2020 Ammo Goettsch
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.F15E.Functions
{
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;

    public class DigitsDisplay : DCSFunction
    {
        private HeliosValue _digitDisplay;

        public DigitsDisplay(BaseUDPInterface sourceInterface, string deviceId, string id, string device, string name, string description)
            : base(sourceInterface, device, name, description)
        {
            DefaultDataElements = new ExportDataElement[] { new DCSDataElement(id, null, false) };
            DoBuild();
        }

        // deserialization constructor
        public DigitsDisplay(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public override void BuildAfterDeserialization()
        {
            DoBuild();
        }

        private void DoBuild()
        {
            _digitDisplay = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName,
                SerializedFunctionName, SerializedDescription, "", BindingValueUnits.Numeric);
            Values.Add(_digitDisplay);
            Triggers.Add(_digitDisplay);
        }

        protected override ExportDataElement[] DefaultDataElements { get; }

        public override void ProcessNetworkData(string id, string value)
        {
            double tenThousands; ;
            double thousands;
            double hundreds;
            string[] parts;
            switch (id)
            {
                case "2010":
                case "2013":
                    parts = Tokenizer.TokenizeAtLeast(value, 3, ';');
                    tenThousands = ClampedParse(parts[0], 10000d);
                    thousands = ClampedParse(parts[1], 1000d);
                    hundreds = Parse(parts[2], 100d);
                    break;
                default:
                    parts = Tokenizer.TokenizeAtLeast(value, 2, ';');
                    tenThousands = 0;
                    thousands = ClampedParse(parts[0], 1000d);
                    hundreds = Parse(parts[1], 100d);
                    break;
            }
            double displayValue = tenThousands + thousands + hundreds;
            _digitDisplay.SetValue(new BindingValue(displayValue), false);
        }
        private double Parse(string value, double scale)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat,
                out double scaledValue))
            {
                return scaledValue;
            }

            if (scaledValue < 1.0d)
            {
                scaledValue *= scale * 10d;
            }
            else
            {
                scaledValue = 0d;
            }
            return scaledValue;
        }

        private double ClampedParse(string value, double scale)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat,
                out double scaledValue))
            {
                return scaledValue;
            }

            if (scaledValue < 1.0d)
            {
                scaledValue = Math.Truncate(scaledValue * 10d) * scale;
            }
            else
            {
                scaledValue = 0d;
            }
            return scaledValue;
        }
        private double ClampedParse(string value, double scale, double offset, double mult)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat,
                out double scaledValue))
            {
                return scaledValue;
            }

            if (scaledValue < 1.0d)
            {
                scaledValue = (Math.Truncate(scaledValue * mult) + offset) * scale;
            }
            else
            {
                scaledValue = 0d;
            }
            return scaledValue;
        }

        public override void Reset()
        {
            _digitDisplay.SetValue(BindingValue.Empty, true);
        }
    }
}
