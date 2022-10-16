//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributers
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

    public class Chronometer : DCSFunctionPair
    {
        private ExportDataElement[] DataElementsTemplate = new ExportDataElement[2];

        private HeliosValue _timeHHmm;
        private HeliosValue _timeSS;
        private HeliosValue _timeMode;
        public enum Flyer {Pilot,Copilot };

        public Chronometer(BaseUDPInterface sourceInterface, string id1, string id2, Flyer cockpit)
            : base(sourceInterface,
                  $"Chronometer ({cockpit})", "Time", "Time in hours, minutes and delimited seconds",
                  $"Chronometer ({cockpit})", "Mode", "Chronometer mode setting.")
        {
            DataElementsTemplate[0] = new DCSDataElement(id1, null, true);
            DataElementsTemplate[1] = new DCSDataElement(id2, null, true);
            DoBuild();
        }

        // deserialization constructor
        public Chronometer(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            _timeHHmm = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, $"{SerializedFunctionName} hh:mm",
                SerializedDescription, "Text HH:MM.", BindingValueUnits.Text);
            Values.Add(_timeHHmm);
            Triggers.Add(_timeHHmm);

            _timeSS = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, $"{SerializedFunctionName} ss",
                SerializedDescription, "Text ss", BindingValueUnits.Text);
            Values.Add(_timeSS);
            Triggers.Add(_timeSS);

            _timeMode = new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName2, $"{SerializedFunctionName2} Mode",
                SerializedDescription2, "Text Mode", BindingValueUnits.Text);
            Values.Add(_timeMode);
            Triggers.Add(_timeMode);
        }

        protected override ExportDataElement[] DefaultDataElements => DataElementsTemplate;

        public override void ProcessNetworkData(string id, string value)
        {
            string[] parts;
            switch (id)
            {
                case "2096":
                case "2098":
                    parts = Tokenizer.TokenizeAtLeast(value, 2, ' ');
                    _timeHHmm.SetValue(new BindingValue(parts[0]), false);
                    _timeSS.SetValue(new BindingValue(parts[1]), false);
                    break;
                case "2097":
                case "2099":
                    _timeMode.SetValue(new BindingValue(value), false);
                    break;

            }
        }

        public override void Reset()
        {
            _timeHHmm.SetValue(BindingValue.Empty, true);
            _timeSS.SetValue(BindingValue.Empty, true);
            _timeMode.SetValue(BindingValue.Empty, true);
        }

    }
}
