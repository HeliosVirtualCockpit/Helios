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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.F5E.Functions
{
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;

    public class HSIMiles : DCSFunction
    {
        private static readonly ExportDataElement[] DataElementsTemplate = { new DCSDataElement("2029", null, true) };

        private HeliosValue _distance;

        public HSIMiles(BaseUDPInterface sourceInterface)
            : base(sourceInterface, "HSI", "Distance to beacon", "Range in nautical miles to the currently selected steerpoint or TACAN station.")
        {
            DoBuild();
        }

        // deserialization constructor
        public HSIMiles(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _distance = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, "", BindingValueUnits.NauticalMiles);
            Values.Add(_distance);
            Triggers.Add(_distance);
        }

        protected override ExportDataElement[] DefaultDataElements => DataElementsTemplate;

        public override void ProcessNetworkData(string id, string value)
        {
            string[] parts = Tokenizer.TokenizeAtLeast(value, 3, ';');

            double hundreds = ClampedParse(parts[0], 100d);
            double tens = ClampedParse(parts[1], 10d);
            double ones = Parse(parts[2], 1d);

            double distance = hundreds + tens + ones;
            _distance.SetValue(new BindingValue(distance), false);
        }

        private double Parse(string value, double scale)
        {
            double scaledValue = 0d;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out scaledValue))
            {
                if (scaledValue < 1.0d)
                {
                    scaledValue *= scale * 10d;
                }
                else
                {
                    scaledValue = 0d;
                }
            }
            return scaledValue;
        }

        private double ClampedParse(string value, double scale)
        {
            double scaledValue = 0d;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out scaledValue))
            {
                if (scaledValue < 1.0d)
                {
                    scaledValue = Math.Truncate(scaledValue * 10d) * scale;
                }
                else
                {
                    scaledValue = 0d;
                }
            }
            return scaledValue;
        }


        public override void Reset()
        {
            _distance.SetValue(BindingValue.Empty, true);
        }
    }
}
