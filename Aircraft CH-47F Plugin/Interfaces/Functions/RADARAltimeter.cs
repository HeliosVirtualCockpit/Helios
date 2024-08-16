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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.CH47F.Functions
{
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;

    public class RADARAltimeter : DCSFunction
    {
        private ExportDataElement[] DataElementsTemplate = new ExportDataElement[1];

        private HeliosValue _digitalAltitude;

        public RADARAltimeter(BaseUDPInterface sourceInterface, string id, string deviceName, string elementName, string elementDescription)
            : base(sourceInterface, deviceName, elementName, elementDescription)
        {
            DataElementsTemplate[0] = new DCSDataElement(id, null, true);
            DoBuild();
        }

        // deserialization constructor
        public RADARAltimeter(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _digitalAltitude = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, "Value in feet.", BindingValueUnits.Feet);
            Values.Add(_digitalAltitude);
            Triggers.Add(_digitalAltitude);

        }

        protected override ExportDataElement[] DefaultDataElements => DataElementsTemplate;

        public override void ProcessNetworkData(string id, string value)
        {
            string[] parts;
            switch (id)
            {
                case "2055":
                case "2056":
                    parts = Tokenizer.TokenizeAtLeast(value, 4, ';');
                    double thousands = ClampedParse(parts[0], 1000d);
                    double hundreds = ClampedParse(parts[1], 100d);
                    double tens = ClampedParse(parts[2], 10d);
                    double units = Parse(parts[3], 1d);

                    double altitude = thousands + hundreds + tens + units;
                    _digitalAltitude.SetValue(new BindingValue(altitude), false);
                    break;
            }
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
            _digitalAltitude.SetValue(BindingValue.Empty, true);
        }

    }
}
