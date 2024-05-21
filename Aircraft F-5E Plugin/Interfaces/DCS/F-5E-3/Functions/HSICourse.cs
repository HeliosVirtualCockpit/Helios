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

    public class HSICourse : DCSFunction
    {
        private static readonly ExportDataElement[] DataElementsTemplate = { new DCSDataElement("2030", null, true) };

        private HeliosValue _direction;

        public HSICourse(BaseUDPInterface sourceInterface)
            : base(sourceInterface, "HSI", "Course Value", "Course.")
        {
            DoBuild();
        }

        // deserialization constructor
        public HSICourse(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _direction = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, "", BindingValueUnits.Degrees);
            Values.Add(_direction);
            Triggers.Add(_direction);
        }

        protected override ExportDataElement[] DefaultDataElements => DataElementsTemplate;

        public override void ProcessNetworkData(string id, string value)
        {
            string[] parts = Tokenizer.TokenizeAtLeast(value, 2, ';');

            double hundreds = ClampedParse(parts[0], 360d);
            double ones = Parse(parts[1], 1d);

            double direction = hundreds + ones;
            _direction.SetValue(new BindingValue(direction), false);
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
                    scaledValue = Math.Truncate(scaledValue * scale);
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
            _direction.SetValue(BindingValue.Empty, true);
        }
    }
}
