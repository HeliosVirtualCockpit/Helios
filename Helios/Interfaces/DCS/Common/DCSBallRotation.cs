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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;
    using Newtonsoft.Json;


    public class DCSBallRotation : DCSFunction
    {
        private readonly ExportDataElement[] DataElementsTemplate;

        private HeliosValue _combinedRotation;

        [JsonProperty("calibration x")]
        private CalibrationPointCollectionDouble _scaleX;

        [JsonProperty("calibration y")]
        private CalibrationPointCollectionDouble _scaleY;

        [JsonProperty("calibration z")]
        private CalibrationPointCollectionDouble _scaleZ;

        private string _id;

        public DCSBallRotation(BaseUDPInterface sourceInterface, string id, string device, string name, string description, CalibrationPointCollectionDouble scaleX, CalibrationPointCollectionDouble scaleY, CalibrationPointCollectionDouble scaleZ)
            : base(sourceInterface, device, name, description)
        {
            _id = id;
            DataElementsTemplate = new ExportDataElement[] { new DCSDataElement(_id, null, true) };
            _scaleX = scaleX;
            _scaleY = scaleY;
            _scaleZ = scaleZ;
            DoBuild();
        }

        // deserialization constructor
        public DCSBallRotation(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
                SerializedDescription, "Single value containing X, Y & Z movement of the ADI ball.", BindingValueUnits.Text);
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
