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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.UH60L.Functions
{
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;

    public class Altimeter : DCSFunctionPair
    {
        //private static readonly ExportDataElement[] DataElementsTemplate = { new DCSDataElement("2051", null, true), new DCSDataElement("2052", null, true) };
        private ExportDataElement[] DataElementsTemplate = new ExportDataElement[2];

        private HeliosValue _altitude;
        private HeliosValue _pressure;
        public enum Cockpit {Pilot,Copilot };

        public Altimeter(BaseUDPInterface sourceInterface, Cockpit cockpit)
            : base(sourceInterface,
                  $"Altimeter ({cockpit})", "Altitude", "Barometric altitude above sea level of the aircraft.",
                  $"Altimeter ({cockpit})", "Pressure", "Manually set barometric altitude.")
        {
            if(cockpit == Cockpit.Pilot)
            {
                DataElementsTemplate[0] = new DCSDataElement("2051", null, true);
                DataElementsTemplate[1] = new DCSDataElement("2052", null, true);
            } else
            {
                DataElementsTemplate[0] = new DCSDataElement("2053", null, true);
                DataElementsTemplate[1] = new DCSDataElement("2054", null, true);
            }
            DoBuild();
        }

        // deserialization constructor
        public Altimeter(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _altitude = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, "Value is adjusted per altimeter pressure setting.", BindingValueUnits.Feet);
            Values.Add(_altitude);
            Triggers.Add(_altitude);

            _pressure = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName2, SerializedFunctionName2,
                SerializedDescription2, "", BindingValueUnits.InchesOfMercury);
            Values.Add(_pressure);
            Triggers.Add(_pressure);
        }

        protected override ExportDataElement[] DefaultDataElements => DataElementsTemplate;

        public override void ProcessNetworkData(string id, string value)
        {
            string[] parts;
            switch (id)
            {
                case "2051":
                case "2053":
                    parts = Tokenizer.TokenizeAtLeast(value, 3, ';');
                    double tenThousands = ClampedParse(parts[0], 10000d);
                    double thousands = ClampedParse(parts[1], 1000d);
                    double hundreds = Parse(parts[2], 100d);

                    double altitude = tenThousands + thousands + hundreds;
                    _altitude.SetValue(new BindingValue(altitude), false);
                    break;
                case "2052":
                case "2054":
                    parts = Tokenizer.TokenizeAtLeast(value, 3, ';');
                    double tensOne = ClampedParse(parts[0], 1d, 26d, 5d);
                    double tenths = ClampedParse(parts[1], .1d);
                    double hundredths = Parse(parts[2], .01d);

                    double pressure = tensOne + tenths + hundredths;
                    _pressure.SetValue(new BindingValue(pressure), false);
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
            _altitude.SetValue(BindingValue.Empty, true);
            _pressure.SetValue(BindingValue.Empty, true);
        }

    }
}
