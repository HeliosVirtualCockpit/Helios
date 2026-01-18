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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.C130J.Functions
{
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;

    public class Altimeter : DCSFunctionPair
    {
        private static ExportDataElement[] DataElementsTemplate;

        private HeliosValue _altitude;
        private HeliosValue _pressure;
        private string _altitudeArg = "2051";
        private string _pressureArg = "2059";
        private double _prevThousands = 0d;
        private double _prevTenThousands = 0d;
        private double _prevHundreds = 0d;
        private bool _initialised = false;
        public Altimeter(BaseUDPInterface sourceInterface, string device = "Altimeter", string altitudeArg = "2051", string pressureArg = "2059")
            : base(sourceInterface,
                  device, "Barometric Altitude", "Altimeter barometric altitude above sea level of the aircraft.",
                  device, "Pressure", "Altimeter manually set barometric altitude.")
        {
            _altitudeArg = altitudeArg;
            _pressureArg = pressureArg;

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
            _altitudeArg = SerializedDataElements[0].ID;
            _pressureArg = SerializedDataElements[1].ID;
            DoBuild();
        }

        private void DoBuild()
        {
            DataElementsTemplate = new ExportDataElement[] { new DCSDataElement(_altitudeArg, null, true), new DCSDataElement(_pressureArg, null, true) };

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
            double pressure;
            if (id.Equals(_altitudeArg))
            {
                parts = Tokenizer.TokenizeAtLeast(value, 3, ';');

                double tenThousands;
                double thousands;
                double hundreds = Math.Truncate(Parse(parts[2], 100d));

                double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double value10K);
                double upValue10K = (Math.Round(value10K, 1) -0.1d) * 100000d;
                double dnValue10K = (Math.Truncate(value10K * 10d) -1d) * 10000d;

                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double value1K);
                double upValue1K = Math.Round(value1K, 1);
                upValue1K = upValue1K == 1d ? 0d : upValue1K * 10000d;
                double dnValue1K = Math.Truncate(value1K * 10d) / 10d;
                dnValue1K = dnValue1K == 1d ? 0d : dnValue1K * 10000d;

                if (!_initialised)
                {
                    _initialised = _prevThousands == dnValue1K && _prevTenThousands == upValue10K;
                    _prevThousands = thousands = hundreds == 0d ? upValue1K : dnValue1K;
                    _prevTenThousands = tenThousands = upValue10K;
                }
                else if (_prevHundreds < 100d && hundreds > 900d){
                    // descending
                    _prevThousands = thousands = dnValue1K;
                    _prevTenThousands = tenThousands = thousands == 9000d ? dnValue10K : upValue10K;
                }
                else if (_prevHundreds > 900d && hundreds < 100d)
                {
                    // ascending
                    _prevThousands = thousands = upValue1K;
                    _prevTenThousands = tenThousands = upValue10K;
                }
                else 
                {
                    tenThousands = _prevTenThousands;
                    thousands = _prevThousands;
                }

                double altitude = tenThousands + thousands + hundreds;
                _altitude.SetValue(new BindingValue(altitude), false);
                _prevHundreds = hundreds;
            }
            else if (id.Equals(_pressureArg))
            {
                parts = Tokenizer.TokenizeAtLeast(value, 3, ';');
                double ones = ClampedParse(parts[0], 1d);
                double tens = ones == 0d ? 30d : 20d;
                double tenths = ClampedParse(parts[1], .1d);
                double hundredths = Parse(parts[2], .01d);
                pressure = tens + ones + tenths + hundredths;
                _pressure.SetValue(new BindingValue(pressure), false);
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
                //scaledValue = Math.Round(scaledValue, 1);
                scaledValue = Math.Truncate(scaledValue * 10d) * scale;
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
            _initialised = false;
            _prevTenThousands = 0d;
            _prevThousands = 0d;
        }

    }
}
