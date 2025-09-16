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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.M2000C.Functions
{
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;
    using System.ServiceModel.Security;

    public class ADIBallRotation : DCSFunction
    {
        private static readonly ExportDataElement[] DataElementsTemplate = { new DCSDataElement("2050", null, true)};

        private HeliosValue _combinedRotation;
        private CalibrationPointCollectionDouble _scaleX, _scaleY, _scaleZ;

        public ADIBallRotation(BaseUDPInterface sourceInterface, CalibrationPointCollectionDouble scaleX, CalibrationPointCollectionDouble scaleY, CalibrationPointCollectionDouble scaleZ)
            : base(sourceInterface,
                   "Flight Instruments", "ADI Rotation", "Single value containing X, Y & Z movement of the aircraft.")
        {
            _scaleX = scaleX;
            _scaleY = scaleY;
            _scaleZ = scaleZ;
            DoBuild();
        }

        // deserialization constructor
        public ADIBallRotation(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _combinedRotation = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, SerializedFunctionName,
                SerializedDescription, "Single value containing X, Y & Z movement of the aircraft.", BindingValueUnits.Text);
            Values.Add(_combinedRotation);
            Triggers.Add(_combinedRotation);
        }

        protected override ExportDataElement[] DefaultDataElements => DataElementsTemplate;

        public override void ProcessNetworkData(string id, string value)
        {
            string[] parts;
            parts = Tokenizer.TokenizeAtLeast(value, 3, ';');
            double x = Parse(parts[0], _scaleX);
            double y = Parse(parts[1], _scaleY);
            double z = Parse(parts[2], _scaleZ);
            _combinedRotation.SetValue(new BindingValue($"{x};{y};{z}"), false);
        }

        private double Parse(string value, CalibrationPointCollectionDouble scale)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat,
                out double scaledValue))
            {
                return scaledValue;
            }

            if (scaledValue <= 1.0d)
            {
                scaledValue = scale.Interpolate(scaledValue);
            }
            else
            {
                scaledValue = 0d;
            }
            return scaledValue;
        }

        public override void Reset()
        {
            _combinedRotation.SetValue(BindingValue.Empty, true);
        }

    }
}
