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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.A10C.Functions
{
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Util;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using NLog;
    using GadrocsWorkshop.Helios.Gauges.A10C;
    using System.Linq;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;
    using System.Runtime.ConstrainedExecution;
    using System.Net;
    using System.Windows.Documents;

    public class ARC210Display : DCSFunction
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly ExportDataElement[] DataElementsTemplate = { new DCSDataElement("2422", null, true) };

        private HeliosValueCollection _displayValues = new HeliosValueCollection();
        private Dictionary<string, string[]> valueDictionary = new Dictionary<string, string[]>() ; 

        public ARC210Display(BaseUDPInterface sourceInterface)
            : base(sourceInterface, "ARC-210", "Display", "The values contained on the display panel.")
        {
            DoBuild();
        }

        // deserialization constructor
        public ARC210Display(BaseUDPInterface sourceInterface, System.Runtime.Serialization.StreamingContext context)
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
            valueDictionary.Add("freq_label_mhz", new string[] { "Frequency Display", "Currently tuned frequency of the ARC-210 radio." });  // this value also holds "dot_mark" and "freq_label_khz"
            valueDictionary.Add("xmit_recv_label", new string[] { "XMIT/RECV Label", "Transmit or Receive label on middle of the display." });
            valueDictionary.Add("prev_manual_freq", new string[] { "Display of Previous Manual Frequency", "Previous frequency value." });
            valueDictionary.Add("modulation_label", new string[] { "Modulation Mode", "Modulation value which appears above the main frequency value on the display." });
            valueDictionary.Add("ky_submode_label", new string[] { "KY Submode", "Value of the label which displays the sub mode on the display." }); 
            valueDictionary.Add("ky_label", new string[] { "KY label", "Value of the label which displays on the display." });  // LOS\nCOMSEC\nPARAMETRS\nUPDATED

            valueDictionary.Add("comsec_mode", new string[] { "Communications Security Mode", "KY status in the middle of the display." });
            valueDictionary.Add("comsec_submode", new string[] { "Communications Security Submode", "KY sub-status in the middle of the display." });
            valueDictionary.Add("txt_RT", new string[] { "RT Label", "Receive / Transmit status from the top right of the display." });
            valueDictionary.Add("active_channel", new string[] { "Active Channel Number", "Preset channel number on the display." });
            valueDictionary.Add("CT", new string[] { "CT", "CT label on the display." });
            valueDictionary.Add("satcom_top_channel_label", new string[] { "Sat comm channel label", "Satellite Communication channel label on the display." });  //DAMA
            //valueDictionary.Add("satcom_top_button_label", new string[] { "Sat comm button label", "Satellite Communication button label on the display." }); //LOGOUT
            //valueDictionary.Add("comsec_satcom_mode", new string[] { "Comm security sat comms mode", "Satellite Communication mode on the display." }); // ANDVT VOICE
            valueDictionary.Add("satcom_channel_type_label", new string[] { "Sat comm channel type", "Satellite Communication channel type on the display." }); //IDLE
            valueDictionary.Add("satcom_activated_status", new string[] { "Sat comm activated status", "Satellite Communication \"Active\" message on the display." }); //ACTIVE
            valueDictionary.Add("satcom_activated_time_remain", new string[] { "Sat comm activated time remaining", "Satellite Communication time remaing on the display." }); // 15:00:00
            valueDictionary.Add("satcom_connection_status", new string[] { "Sat comm connection status", "Satellite Communication connection status on the display." }); // LOGGED IN-\nCONNECTING

            valueDictionary.Add("comsec_satcom_delay", new string[] { "Comm security sat comm delay", "Satellite Communication delay value on the display." }); // 5
            valueDictionary.Add("active_eccm_channel_type", new string[] { "Active ECCM channel type", "ECCM channel type on the display." }); // not seen anything in this variable
            valueDictionary.Add("active_eccm_channel_index", new string[] { "Active ECCM channel index", "ECCM channel index on the display." });  // 1
            valueDictionary.Add("top_line_label", new string[] { "Upper FSK Label", "Label for Upper FSK on the display." });
            valueDictionary.Add("mid_line_label", new string[] { "Middle FSK Label", "Label for Middle FSK on the display." });
            valueDictionary.Add("bot_line_label", new string[] { "Lower FSK Label", "Label for Lower FSK on the display." });
            valueDictionary.Add("WOD_segment", new string[] { "WOD Segment Display", "Central dual digit display." });

            foreach (string valueVariable in valueDictionary.Keys)
            {
                AddValue(new HeliosValue(SourceInterface, BindingValue.Empty, SerializedDeviceName, valueDictionary[valueVariable][0],
                    valueDictionary[valueVariable][1], "", BindingValueUnits.Text));
            } 
        }

        protected override ExportDataElement[] DefaultDataElements => DataElementsTemplate;

        public override void ProcessNetworkData(string id, string value)
        {
            List<string> changedValueList = valueDictionary.Keys.ToList();
            HeliosValue hv;
            string mHz = "";
            string dotMark = "";
            string kHz = "";
            Regex rx = new Regex(@"(?<=-----------------------------------------\n)((?'variable'[^\n]+)\n(?'value'[^\n]*))\n(?=------|\})" +
                @"|(?<=-----------------------------------------\n)((?'variable'[^\n]+\n[^\n]+)\n(?'value'[^\n]*\n[^\n]*))\n(?=------|\})" + 
                @"|(?<=-----------------------------------------\n)((?'variable'[^\n]+)\n(?'value'[A-Z0-9-[\n]]*\n[A-Z0-9-[\n]]*\n[A-Z0-9-[\n]]*\n[A-Z0-9-[\n]]*))\n(?=------|\})", RegexOptions.Compiled | RegexOptions.Multiline);
            MatchCollection valueMatches = rx.Matches(decodeIndication(value));
            if (valueMatches.Count > 0)
            {
                if (valueMatches.Count == 3)
                {
                    if (valueMatches[0].Groups["variable"].Value == "POWER UP LOADER")
                    {
                        changedValueList.Remove("ky_label");
                        hv = _displayValues["ARC-210.KY label"] as HeliosValue;
                        hv?.SetValue(new BindingValue($"{valueMatches[0].Groups["value"].Value}\n\n\n\n\n\n\n"), false);
                        changedValueList.Remove("mid_line_label");
                        hv = _displayValues["ARC-210.Middle FSK Label"] as HeliosValue;
                        hv?.SetValue(new BindingValue(valueMatches[1].Groups["value"].Value), false);
                        changedValueList.Remove("bot_line_label");
                        hv = _displayValues["ARC-210.Lower FSK Label"] as HeliosValue;
                        hv?.SetValue(new BindingValue(valueMatches[2].Groups["value"].Value), false);

                        ClearUnusedValues(changedValueList);
                        return;
                    } else if(valueMatches[0].Groups["variable"].Value == "txt_RT" &&
                        valueMatches[1].Groups["variable"].Value == valueMatches[1].Groups["value"].Value &&
                        (valueMatches[2].Groups["variable"].Value == valueMatches[2].Groups["value"].Value ||
                        valueMatches[2].Groups["variable"].Value.Substring(0,1) == "{"))
                    {
                        changedValueList.Remove(valueMatches[0].Groups["variable"].Value);
                        hv = _displayValues["ARC-210.RT Label"] as HeliosValue;
                        if (hv?.Value.StringValue != valueMatches[0].Groups["value"].Value)
                        {
                            hv.SetValue(new BindingValue(valueMatches[0].Groups["value"].Value), false);
                        }
                        changedValueList.Remove("top_line_label");
                        hv = _displayValues["ARC-210.Upper FSK Label"] as HeliosValue;
                        if (hv?.Value.StringValue != valueMatches[1].Groups["value"].Value)
                        {
                            hv.SetValue(new BindingValue(valueMatches[1].Groups["value"].Value), false);
                        }
                        changedValueList.Remove("mid_line_label");
                        hv = _displayValues["ARC-210.Middle FSK Label"] as HeliosValue;
                        if (hv?.Value.StringValue != valueMatches[2].Groups["value"].Value)
                        {
                            hv.SetValue(new BindingValue(valueMatches[2].Groups["value"].Value.Replace('_', ' ')), false);
                        }
                    }
                }
                if (valueMatches.Count == 4 ||
                    (valueMatches.Count == 5 && (valueMatches[4].Groups["variable"].Value == "active_oper_date" || valueMatches[4].Groups["variable"].Value == "active_channel")) ||
                    (valueMatches.Count == 6 && (valueMatches[3].Groups["variable"].Value.Contains("_segment"))))
                {
                    //Possibly this is a set of button labels
                    if (valueMatches[0].Groups["variable"].Value == "txt_RT" && 
                        (
                        valueMatches[1].Groups["variable"].Value == "top_line_label" || 
                        (valueMatches[1].Groups["variable"].Value == valueMatches[1].Groups["value"].Value)
                        )
                        )
                    {
                        changedValueList.Remove(valueMatches[0].Groups["variable"].Value);
                        hv = _displayValues["ARC-210.RT Label"] as HeliosValue;
                        if (hv?.Value.StringValue != valueMatches[0].Groups["value"].Value)
                        {
                            hv.SetValue(new BindingValue(valueMatches[0].Groups["value"].Value), false);
                        }
                        changedValueList.Remove("top_line_label");
                        hv = _displayValues["ARC-210.Upper FSK Label"] as HeliosValue;
                        if (hv?.Value.StringValue != valueMatches[1].Groups["value"].Value)
                        {
                            hv.SetValue(new BindingValue(valueMatches[1].Groups["value"].Value), false);
                        }
                        changedValueList.Remove("mid_line_label");
                        hv = _displayValues["ARC-210.Middle FSK Label"] as HeliosValue;
                        if (hv?.Value.StringValue != valueMatches[2].Groups["value"].Value)
                        {
                            hv.SetValue(new BindingValue(valueMatches[2].Groups["value"].Value.Replace('_',' ')), false);
                        }
                        if (valueMatches.Count != 6)
                        {
                            if (valueMatches.Count == 4 && valueMatches[3].Groups["variable"].Value == "active_channel")
                            {
                                changedValueList.Remove("active_channel");
                                hv = _displayValues["ARC-210.Active Channel Number"] as HeliosValue;
                                if (hv?.Value.StringValue != valueMatches[3].Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue(valueMatches[3].Groups["value"].Value), false);
                                }
                            }
                            else
                            {
                                changedValueList.Remove("bot_line_label");
                                hv = _displayValues["ARC-210.Lower FSK Label"] as HeliosValue;
                                if (hv?.Value.StringValue != valueMatches[3].Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue(valueMatches[3].Groups["value"].Value), false);
                                }
                            }
                            if (valueMatches.Count == 5)
                            {
                                changedValueList.Remove("active_channel");
                                hv = _displayValues["ARC-210.Active Channel Number"] as HeliosValue;
                                if (hv?.Value.StringValue != valueMatches[4].Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue(valueMatches[4].Groups["value"].Value), false);
                                }
                            }
                            ClearUnusedValues(changedValueList);
                            return;
                        }
                    }
                }
                foreach (Match match in valueMatches)
                {
                    switch(match.Groups["variable"].Value)
                    {
                        case "HQII_segment_data_mhz":
                        case "WOD_segment_data_mhz":
                        case "freq_label_mhz":
                            mHz = match.Groups["value"].Value;
                            continue;
                        case "dot_mark":
                        case "dot_mark_121":
                        case "dot_mark_243":
                            dotMark = match.Groups["value"].Value;
                            continue;
                        case "HQII_segment_data_khz":
                        case "WOD_segment_data_khz":
                        case "freq_label_khz":
                            kHz = match.Groups["value"].Value;
                            continue;
                        case "prev_freq_label":
                            changedValueList.Remove("top_line_label");
                            hv = _displayValues["ARC-210.Upper FSK Label"] as HeliosValue;
                            if (hv?.Value.StringValue != match.Groups["value"].Value)
                            {
                                hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                continue;
                            }
                            continue;
                        case "HQII_segment":
                        case "WOD_segment":
                            changedValueList.Remove("WOD_segment");
                            hv = _displayValues["ARC-210.WOD Segment Display"] as HeliosValue;
                            if (hv?.Value.StringValue != match.Groups["value"].Value)
                            {
                                hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                continue;
                            }
                            continue;
                        case "active_eccm_channel_index":
                        case "active_oper_date":
                            changedValueList.Remove("active_channel");
                            hv = _displayValues["ARC-210.Active Channel Number"] as HeliosValue;
                            if (hv?.Value.StringValue != match.Groups["value"].Value)
                            {
                                hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                continue;
                            }
                            continue;
                        case "satcom_top_button_label":
                            changedValueList.Remove("top_line_label");
                            hv = _displayValues["ARC-210.Upper FSK Label"] as HeliosValue;
                            if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                            {
                                hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                continue;
                            }
                            continue;
                        case "comsec_satcom_mode":
                            changedValueList.Remove("comsec_mode");
                            hv = _displayValues["ARC-210.Communications Security Mode"] as HeliosValue;
                            if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                            {
                                hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                continue;
                            }
                            continue;
                        case "ky_submode_label":
                            changedValueList.Remove("comsec_satcom_delay");
                            hv = _displayValues["ARC-210.Comm security sat comm delay"] as HeliosValue;
                            if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                            {
                                hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                continue;
                            }
                            continue;
                    }
                    if (match.Groups["variable"].Value.Contains("{") && match.Groups["variable"].Value.Contains("}"))
                    {
                        if (match.Groups["value"].Value == "UNAVAILABLE")
                        {
                            changedValueList.Remove("ky_label");
                            hv = _displayValues["ARC-210.KY label"] as HeliosValue;
                        } else
                        {
                            changedValueList.Remove("mid_line_label");
                            hv = _displayValues["ARC-210.Middle FSK Label"] as HeliosValue;
                        }

                        if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                        {
                            hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                            continue;
                        }
                        continue;
                    }
                    if (match.Groups["variable"].Value == match.Groups["value"].Value)
                    {
                        switch (match.Groups["value"].Value)
                        {
                            case "PREV":
                                hv = _displayValues["ARC-210.Upper FSK Label"] as HeliosValue;
                                changedValueList.Remove("top_line_label");
                                if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue("PREV"), false);
                                    continue;
                                }
                                break;
                            case "CT":
                                changedValueList.Remove("comsec_submode");
                                hv = _displayValues["ARC-210.Communications Security Submode"] as HeliosValue;
                                if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                    continue;
                                }
                                break;
                            case "FM":
                            case "AM":
                                changedValueList.Remove("modulation_label");
                                hv = _displayValues["ARC-210.Modulation Mode"] as HeliosValue;
                                if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue("AM"), false);
                                    continue;
                                }
                                break;
                            case "ATTEMPTING\nTO LOGIN":
                            case "LOGGED IN-\nCONNECTING":
                                changedValueList.Remove("satcom_connection_status");
                                hv = _displayValues["ARC-210.Sat comm connection status"] as HeliosValue;
                                if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                    continue;
                                }
                                break;
                            case "UNAVAILABLE":
                            case "INITIALIZING":
                                changedValueList.Remove("ky_label");
                                hv = _displayValues["ARC-210.KY label"] as HeliosValue;
                                if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                                    continue;
                                }
                                break;
                            case "NO FILL":
                                changedValueList.Remove("mid_line_label");
                                hv = _displayValues["ARC-210.Middle FSK Label"] as HeliosValue;
                                if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue("NO FILL"), false);
                                    continue;
                                }
                                break;
                            case "COLD ST":
                                changedValueList.Remove("top_line_label");
                                hv = _displayValues["ARC-210.Upper FSK Label"] as HeliosValue;
                                if (hv != null && hv.Value.StringValue != match.Groups["value"].Value)
                                {
                                    hv.SetValue(new BindingValue("COLD ST"), false);
                                    continue;
                                }
                                break;
                            default:
                                Logger.Debug($"Encountered ARC-210 Label which was not processed: {match.Groups["value"].Value}");
                                break;
                        }

                        continue;
                    }
                    if (valueDictionary.ContainsKey(match.Groups["variable"].Value))
                    {
                        hv = _displayValues[$"ARC-210.{valueDictionary[match.Groups["variable"].Value][0]}"] as HeliosValue;
                        changedValueList.Remove(match.Groups["variable"].Value);
                        if (hv?.Value.StringValue != match.Groups["value"].Value)
                        {
                            hv.SetValue(new BindingValue(match.Groups["value"].Value), false);
                        }
                    } else
                    {
                        Logger.Debug($"Encountered ARC-210 Label which was not processed: {match.Groups["variable"].Value} =  {match.Groups["value"].Value}");
                    }
                }
            }
            if($"{mHz}{dotMark}{kHz}" != "")
            {
                hv = _displayValues["ARC-210.Frequency Display"] as HeliosValue;
                changedValueList.Remove("freq_label_mhz");
                if (hv != null &&  hv.Value.StringValue != $"{mHz}{(dotMark=="" ? " " : dotMark)}{kHz}")
                {
                    hv.SetValue(new BindingValue($"{mHz}{(dotMark == "" ? " " : dotMark)}{kHz}"), false);
                }
            }
            ClearUnusedValues(changedValueList);
        }

        private string decodeIndication(string networkData)
        {
            return networkData.Replace("%0A%0D", "-----------------------------------------\n").Replace("%0A", "\n").Replace("%3A", ":");
        }
        private void ClearUnusedValues(List<string> valueList)
        {
            foreach (string value in valueList)
            {
                if (_displayValues[$"ARC-210.{valueDictionary[value][0]}"] is HeliosValue hv)
                {
                    hv.SetValue(new BindingValue(""), false);
                }
            }
        }
        private void AddValue(HeliosValue hv)
        {
            Values.Add(hv);
            Triggers.Add(hv);
            _displayValues.Add(hv);
        }

        public override void Reset()
        {
            foreach( HeliosValue hv in _displayValues) {
                hv.SetValue(BindingValue.Empty, true);
            }
        }

        private class List<T1, T2>
        {
        }
    }
}
