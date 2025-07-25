namespace GadrocsWorkshop.Helios.Interfaces.DCS.DH98Mosquito
{
	using System;
	using System.Collections.Generic;
    using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
    using System.Windows.Documents;

    internal static class ProcessClickables
	{
		internal static Dictionary<string, FunctionData> GetFunctions()
		{
            Dictionary<string, FunctionData> functions = new Dictionary<string, FunctionData>();

            string pattern = @"^\s*elements\[""(?'var'[\w-_\s]*)""\]\s*\=\s*mcabin_(?'function'\w*)\(\s*(?'head'.*)(,\s*devices.(?'device'[A-Z0-9_]*),.*device_commands.Button_(?'command'[\d]*),\s*(?'arg'\d{1,3}),?\s*(?'tail'.*))\s*\)\s*$";
            string input = new StreamReader(@"C:\Program Files\Eagle Dynamics\DCS World\Mods\aircraft\MosquitoFBMkVI\Cockpit\Scripts\clickabledata.lua").ReadToEnd();

            RegexOptions options = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
			List<string> functionList = new List<string>();
            Dictionary<string, string> argDescriptions = new Dictionary<string, string>
            {
                { "245", "Button A - MG Trigger" },
                { "246", "Button B1 - Cannon Trigger" },
                { "244", "Button B2 - Secondary & Drop Ordnance Trigger" },
                { "248", "Wheel Brakes Lever" },
                { "251", "Left Side Window Open/Close Control" },
                { "253", "Right Side Window Open/Close Control" },
                { "107", "Reflector Sight Setter Rings" },
                { "13", "Reflector Sight Dimmer Type D" },
                { "124", "Master Gate & Magneto Switches" },
                { "129", "Port Starter Switch Assembly" },
                { "130", "Starboard Starter Switch Assembly" },
                { "133", "Port Booster Switch Assembly" },
                { "134", "Starboard Booster Switch Assembly" },
                { "354", "Port Feathering Switch" },
                { "355", "Starboard Feathering Switch" },
                { "14", "R.H. Flood Light Dimmer Type D" },
                { "15", "J.B.B. Flood Light Dimmer Type D" },
                { "112", "Port Radiator Flap Switch" },
                { "113", "Starboard Radiator Flap Switch" },
                { "114", "Tropical Air Filter Switch" },
                { "282", "LR Pump Light Cover" },
                { "49", "Repeater Compass" },
                { "72", "Altimeter" },
                { "74", "DI" },
                { "304", "Undercarriage Indicator Blind" },
                { "375", "Clock Reference Knobs" },
                { "84", "Port Oxygen Regulator" },
                { "292", "Boost Cut-Off T-Handle" },
                { "62", "Port Landing Light Switch" },
                { "63", "Starboard Landing Light Switch" },
                { "327", "Three-Way Selector Valve" },
                { "120", "Gun Firing Master Switch Assembly" },
                { "372", "De-Ice Glycol Pump Handle" },
                { "111", "Rudder Trim Knob" },
                { "280", "Aileron Trim Knob" },
                { "16", "Bomb Panel Flood Light Dimmer Type D" },
                { "284", "Bomb Doors Light Cover" },
                { "144", "Containers Jettison Switch Assembly" },
                { "311", "Protective Cover Trigger" },
                { "143", "Cine Camera Changeover Switch" },
                { "148", "Station 1 Switch" },
                { "149", "Station 2 Switch" },
                { "150", "Station 3 Switch" },
                { "90", "Footage Indicator" },
                { "151", "Station 4 Switch" },
                { "152", "Nose Fusing Switch" },
                { "153", "Tail Fusing Switch" },
                { "28", "Magnetic Compass" },
                { "10", "Emergency Light Rheostat" },
                { "11", "Compass Flood Light Dimmer Type D" },
                { "12", "L.H. Flood Light Dimmer Type D" },
                { "8", "Wing Tank Jettison Switch Assembly" },
                { "294", "UV Exciter Button" },
                { "295", "LH UV Lamp Cap" },
                { "296", "RH UV Lamp Cap" },
                { "7", "Beam Approach Volume Rheostat" },
                { "32", "Off Button" },
                { "33", "A Button" },
                { "34", "B Button" },
                { "35", "C Button" },
                { "36", "D Button" },
                { "42", "Dimmer Toggle" },
                { "43", "Transmit Lock Toggle" },
                { "364", "Volume Knob" },
                { "25", "Throttles Frictioner" },
                { "288", "Throttle Levers" },
                { "24", "Prop Levers Frictioner" },
                { "22", "Prop Levers" },
                { "27", "Mixture Control" },
                { "26", "Supercharger Gear Change Switch" },
                { "386", "Rockets Firing Switch" },
                { "385", "Master Switch" },
                { "387", "Salvo Selector Switch" },
                { "384", "Manual Advancement Button" },
                { "1", "R.I. Compass Switch" },
                { "2", "R.I. Compass De-ground Switch" },
                { "3", "Beam Approach Switch" },
                { "4", "SCR 522 PTT" },
                { "279", "Tail Trim Wheel" },
                { "254", "Door" },
                { "160", "Down Ident Lights Switch" },
                { "161", "Camera Gun Switch" },
                { "162", "Nav Lights Switch" },
                { "163", "UV Lighting Switch" },
                { "164", "Pitot Head Switch" },
                { "165", "Fuel Pump Switch" },
                { "166", "Reflector Sight Switch" },
                { "167", "Nav. Head Lamp Switch" },
                { "168", "IFF Switch" },
                { "169", "IFF Detonator Buttons Assembly" },
                { "172", "Port Extinguisher Switch Assembly" },
                { "174", "Starboard Extinguisher Switch Assembly" },
                { "176", "Windscreen Wiper Rheostat" },
                { "178", "Resin Lights Switch" },
                { "157", "Morse Key" },
                { "158", "Downward Lamp Mode Selector" },
                { "159", "Upward Lamp Mode Selector" },
                { "293", "Oxygen High Pressure Control Valve" },
                { "187", "Starboard Oxygen Regulator" },
                { "303", "Chart Table Flood Light Dimmer Type D" },
                { "202", "Aerial Winch" },
                { "305", "T.1154 / R.1155 L.T. Power Unit Switch" },
                { "306", "T.1154 / R.1155 H.T. Power Unit Switch" },
                { "307", "Transmitter Type F Switch" },
                { "192", "Port Engine Cut-Out Handle" },
                { "191", "Starboard Engine Cut-Out Handle" },
                { "193", "Port Fuel Cock" },
                { "194", "Starboard Fuel Cock" },
                { "195", "Transfer Fuel Cock" },
                { "197", "Port Oil Dilution Switch" },
                { "198", "Starboard Oil Dilution Switch" },
                { "196", "Tank Pressurizing Lever" },
                { "200", "Port Auxiliary Oil Supply Lever" },
                { "199", "Starboard Auxiliary Oil Supply Lever" },
                { "203", "Cabin Heater Control" },
                { "204", "Gun Heater Control" },
                { "256", "LH Arm Rest" },
                { "257", "RH Arm Rest" },
                { "323", "Harness Release Lever" },
                { "333", "Emergency Selector Knob" },
                { "365", "T.1154 Key" },
                { "302", "Roof Dome Light Dimmer Type D" },
                { "17", "Loop Antenna Flood Light Dimmer Type D" },
                { "358", "C2" },
                { "359", "C4" },
                { "360", "C15" },
                { "361", "C16" },
                { "362", "C17" },
                { "212", "C2 Presets Knob" },
                { "211", "C4 Presets Knob" },
                { "213", "C15 Presets Knob" },
                { "214", "C16 Presets Knob" },
                { "210", "C17 Presets Knob" },
                { "223", "C2 Vernier" },
                { "222", "C4 Vernier" },
                { "215", "S1 / S2" },
                { "216", "S3" },
                { "217", "S4" },
                { "218", "S5" },
                { "219", "S6" },
                { "220", "S7" },
                { "221", "L6" },
                { "224", "Aerial Selector Switch Type J" },
                { "238", "Master Selector Switch" },
                { "231", "Frequency Range Switch" },
                { "229", "Volume Knob" },
                { "233", "Tuning (Fine)" },
                { "234", "Tuning (Coarse)" },
                { "230", "Heterodyne Switch" },
                { "225", "Meter Balance Knob" },
                { "226", "Filter Switch" },
                { "227", "Meter Amplitude Knob" },
                { "235", "Meter Deflection Sensitivity Switch" },
                { "236", "Aural Sense Switch" },
                { "237", "Meter Frequency Switch" },
                { "241", "Lock" },
                { "240", "Loop Rotation" },
                { "188", "Channel Switch" },
                { "363", "Detonator Switch Assembly" },
                { "190", "Power Switch" },
                { "312", "Armor Headrest" }
            };
            string currentDescription = string.Empty;
            foreach (Match m in Regex.Matches(input, pattern, options))
			{
                string arg = m.Groups["arg"].Value;
                if (argDescriptions.ContainsKey(arg))
                {
                    currentDescription = argDescriptions[arg];
                }
                functions.Add(arg, new FunctionData()
                {
                    Fn = m.Groups["function"].Value,
                    Var = m.Groups["var"].Value,
                    Arg = arg,
                    Device = m.Groups["device"].Value,
                    Command = m.Groups["command"].Value,
                    Head = m.Groups["head"].Value,
                    Tail = m.Groups["tail"].Value,
                    Description = currentDescription
                });

                //var argArray = m.Groups["arg"].Captures.OfType<Capture>().Select(c => c.Value).ToArray();
            }

            foreach (FunctionData fd in functions.Values)
            {
                Console.WriteLine("\"{0}\" | \"{1}\"", fd.Arg, fd.Description);
            }
            //foreach (string s in functionList)
            //{
            //    Console.WriteLine("case \"{0}\":\n\nbreak;", s);
            //}
            return (functions);
        }
	}
    internal class FunctionData
    {
        internal string Fn;
        internal string Var;
        internal string Arg;
        internal string Device;
        internal string Command;
        internal string Head;
        internal string Tail;
        internal string Description;
    }
}


