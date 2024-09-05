// Copyright 2020 Helios Contributors
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System.Globalization;
using GadrocsWorkshop.Helios.UDPInterface;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Common
{
    public class DualNetworkValue : ScaledNetworkValue
    {
        private string _rawName;
        private HeliosValue _rawValue;

        public DualNetworkValue(BaseUDPInterface sourceInterface, string id, CalibrationPointCollectionDouble calibration, string device, string name, string description, string valueDescription, BindingValueUnit unit)
            : base(sourceInterface, id, device, name, description, valueDescription, unit, "%.4f", calibration, 0d, 0d, false)
        {
            // base calls its DoBuild, we add ours
            DoBuild();
        }

        public DualNetworkValue(BaseUDPInterface sourceInterface, string id, CalibrationPointCollectionDouble calibration, string device, string name, string description, string valueDescription, BindingValueUnit unit, string exportFormat)
            : base(sourceInterface, id, device, name, description, valueDescription, unit, exportFormat, calibration, 0d, 0d, false)
        {
            // base calls its DoBuild, we add ours
            DoBuild();
        }

        public DualNetworkValue(BaseUDPInterface sourceInterface, string id, double scale, string device, string name, string description, string valueDescription, BindingValueUnit unit)
            : base(sourceInterface, id, device, name, description, valueDescription, unit, "%.4f", null, 0d, scale, false)
        {
            // base calls its DoBuild, we add ours
            DoBuild();
        }

        public DualNetworkValue(BaseUDPInterface sourceInterface, string id, double scale, string device, string name, string description, string valueDescription, BindingValueUnit unit, double baseValue, string exportFormat)
            : base(sourceInterface, id, device, name, description, valueDescription, unit, exportFormat, null, baseValue, scale, false)
        {
            // base calls its DoBuild, we add ours
            DoBuild();
        }

        // deserialization constructor
        public DualNetworkValue(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
            : base(sourceInterface, context)
        {
            // no code
        }

        public override void BuildAfterDeserialization()
        {
            base.BuildAfterDeserialization();
            DoBuild();
        }

        private new void DoBuild()
        {
            _rawName = "Pure " + SerializedFunctionName;
            _rawValue = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, _rawName,
                "Pure value in DCS", "pure value", Unit);
            Values.Add(_rawValue);
            Triggers.Add(_rawValue);
        }

        public override void ProcessNetworkData(string id, string value)
        {
            // handle interpolation
            base.ProcessNetworkData(id, value);

            // also store the raw value
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double rawValue))
            {
				_rawValue.SetValue(new BindingValue(rawValue), false);
			}
        }

        public override void Reset()
        {
            base.Reset();
			_rawValue.SetValue(BindingValue.Empty, true);
		}
    }
}
