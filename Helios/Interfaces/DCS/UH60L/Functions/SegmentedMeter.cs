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

    public class SegmentedMeter : DCSFunction
    {
        private ExportDataElement[] DataElementsTemplate = new ExportDataElement[1];

        private HeliosValue _startValue, _endValue, _startSegment, _endSegment;
 
        public SegmentedMeter(BaseUDPInterface sourceInterface, string id, double maxsegments, string device, string name, string description)
            : base(sourceInterface,device, name, description)
        {
            DataElementsTemplate[0] = new DCSDataElement(id, null, true);
            DoBuild();
        }

        // deserialization constructor
        public SegmentedMeter(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _startValue = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, $"{SerializedFunctionName} Start Value",
                SerializedDescription, "Value is changed when the meter starting value changes.", BindingValueUnits.Numeric);
            Values.Add(_startValue);
            Triggers.Add(_startValue);

            _endValue = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, $"{SerializedFunctionName} End Value",
                SerializedDescription, "Value is changed when the meter finishing value changes.", BindingValueUnits.Numeric);
            Values.Add(_endValue);
            Triggers.Add(_endValue);

            _startSegment = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, $"{SerializedFunctionName} Start Segment",
                SerializedDescription, "Value is changed when the meter starting segment changes.", BindingValueUnits.Numeric);
            Values.Add(_startSegment);
            Triggers.Add(_startSegment);

            _endSegment = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, $"{SerializedFunctionName} End Segment",
                SerializedDescription, "Value is changed when the meter finishing segment changes.", BindingValueUnits.Numeric);
            Values.Add(_endSegment);
            Triggers.Add(_endSegment);


        }

        protected override ExportDataElement[] DefaultDataElements => DataElementsTemplate;

        public override void ProcessNetworkData(string id, string value)
        {
            string[] parts;
            parts = Tokenizer.TokenizeAtLeast(value, 4, ';');
            double startValue = Parse(parts[0], 1d);
            _startValue.SetValue(new BindingValue(startValue), false);
            double endValue = Parse(parts[2], 1d);
            _endValue.SetValue(new BindingValue(endValue), false);
            double startSegment = Parse(parts[1], 1d);
            _startSegment.SetValue(new BindingValue(startSegment), false);
            double endSegment = Parse(parts[3], 1d);
            _endSegment.SetValue(new BindingValue(endSegment), false);

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
        public override void Reset()
        {
            _startValue.SetValue(BindingValue.Empty, true);
            _endValue.SetValue(BindingValue.Empty, true);
            _startSegment.SetValue(BindingValue.Empty, true);
            _endSegment.SetValue(BindingValue.Empty, true);
        }

    }
}
