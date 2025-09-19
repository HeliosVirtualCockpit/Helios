//  Copyright 2014 Craig Courtney
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

#pragma warning disable IDE0051 // Remove unused private member: interfaces contain definitions we don't implement
namespace GadrocsWorkshop.Helios.Interfaces.DCS.AV8B
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.AV8B.Functions;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;

    [HeliosInterface(
        "Helios.AV8B",                         // Helios internal type ID used in Profile XML, must never change
        "DCS AV-8B",                    // human readable UI name for this interface
        typeof(DCSInterfaceEditor),             // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),   // can't be instantiated when specific other interfaces are present
        UniquenessKey = "Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class AV8BInterface : DCSInterface
    {
        #region Devices
        //  The device list seems to be a moving target at this moment and 2.5.1 saw some changes which altered numbers. 
        //  Need to keep an eye on devices.lua
        private const string ELECTRIC = "1";
        private const string COMM1 = "2";
        private const string COMM2 = "3";
        private const string INTERCOM = "4";
        private const string TACAN = "5";
        private const string AWLS = "6";
        private const string RSC = "7";
        private const string ACNIP = "8";
        private const string DECS = "9";
        private const string ADC = "10";
        private const string NAV_INS = "11";
        private const string MSC = "12";
        private const string VREST = "13";
        private const string NAVFLIR = "14";
        private const string TGTPOD = "15";
        private const string DMT = "16";
        private const string RWR = "17";
        private const string RWRCONTROL = "18";
        private const string FLIGHTINSTRUMENTS = "19";
        private const string EDP = "20";
        private const string FQIS = "21";
        private const string HUDCONTROL = "22";
        private const string UFCCONTROL = "23";
        private const string ODUCONTROL = "24";
        private const string EHSD_MAP = "25";
        private const string MPCD_LEFT = "26";
        private const string MPCD_RIGHT = "27";
        private const string FLIGHTCONTROLS = "28";
        private const string SMC = "29";
        private const string EWS = "30";
        private const string CBTSYST = "31";
        private const string TVWPNS = "32";
        private const string LTEXT = "33";
        private const string LTINT = "34";
        private const string LTWCA = "35";
        private const string ECS = "36";
        private const string HYDRAULIC = "37";
        private const string NETWORKINTERFACE = "38";
        private const string ANVIS9 = "39";
        private const string AUTOSTART = "40";
        #endregion

        public AV8BInterface()
            : base("DCS AV-8B", "AV8BNA", "pack://application:,,,/AV-8B;component/Interfaces/ExportFunctions.lua")
        {

            // see if we can restore from JSON
#if (!DEBUG)
            if (LoadFunctionsFromJson())
            {
                return;
            }
#endif
            #region Left MCFD
            AddFunction(new PushButton(this, MPCD_LEFT, "3200", "200", "Left MFCD", "OSB01", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3201", "201", "Left MFCD", "OSB02", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3202", "202", "Left MFCD", "OSB03", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3203", "203", "Left MFCD", "OSB04", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3204", "204", "Left MFCD", "OSB05", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3205", "205", "Left MFCD", "OSB06", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3206", "206", "Left MFCD", "OSB07", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3207", "207", "Left MFCD", "OSB08", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3208", "208", "Left MFCD", "OSB09", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3209", "209", "Left MFCD", "OSB10", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3210", "210", "Left MFCD", "OSB11", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3211", "211", "Left MFCD", "OSB12", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3212", "212", "Left MFCD", "OSB13", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3213", "213", "Left MFCD", "OSB14", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3214", "214", "Left MFCD", "OSB15", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3215", "215", "Left MFCD", "OSB16", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3216", "216", "Left MFCD", "OSB17", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3217", "217", "Left MFCD", "OSB18", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3218", "218", "Left MFCD", "OSB19", "1", "0", "%1d"));
            AddFunction(new PushButton(this, MPCD_LEFT, "3219", "219", "Left MFCD", "OSB20", "1", "0", "%1d"));
            AddFunction(new Axis(this, MPCD_LEFT, "3194", "194", 0.1d, 0d, 1d, "Left MFCD", "Off/Brightness Control"));
            AddFunction(new Switch(this, MPCD_LEFT, "220", new SwitchPosition[] { new SwitchPosition("-1.0", "Day", "3007", "3007", "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("1.0", "Night", "3008", "3008", "0.0", "0.0") }, "Left MFCD", "DAY/NIGHT Mode", "%0.1f"));
            AddFunction(new Switch(this, MPCD_LEFT, "221", new SwitchPosition[] { new SwitchPosition("1.0", "Up", "3001", "3001", "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Down", "3002", "3002", "0.0", "0.0") }, "Left MFCD", "Symbology", "%0.1f"));
            AddFunction(new Switch(this, MPCD_LEFT, "222", new SwitchPosition[] { new SwitchPosition("1.0", "Up", "3003", "3003", "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Down", "3004", "3004", "0.0", "0.0") }, "Left MFCD", "Gain", "%0.1f"));
            AddFunction(new Switch(this, MPCD_LEFT, "223", new SwitchPosition[] { new SwitchPosition("1.0", "Up", "3005", "3005", "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Down", "3006", "3006", "0.0", "0.0") }, "Left MFCD", "Contrast", "%0.1f"));

            #endregion

            #region Right MCFD
            AddFunction(new PushButton(this, MPCD_RIGHT, "3224", "224", "Right MFCD", "OSB01"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3225", "225", "Right MFCD", "OSB02"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3226", "226", "Right MFCD", "OSB03"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3227", "227", "Right MFCD", "OSB04"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3228", "228", "Right MFCD", "OSB05"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3229", "229", "Right MFCD", "OSB06"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3230", "230", "Right MFCD", "OSB07"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3231", "231", "Right MFCD", "OSB08"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3232", "232", "Right MFCD", "OSB09"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3233", "233", "Right MFCD", "OSB10"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3234", "234", "Right MFCD", "OSB11"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3235", "235", "Right MFCD", "OSB12"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3236", "236", "Right MFCD", "OSB13"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3237", "237", "Right MFCD", "OSB14"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3238", "238", "Right MFCD", "OSB15"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3239", "239", "Right MFCD", "OSB16"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3240", "240", "Right MFCD", "OSB17"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3241", "241", "Right MFCD", "OSB18"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3242", "242", "Right MFCD", "OSB19"));
            AddFunction(new PushButton(this, MPCD_RIGHT, "3243", "243", "Right MFCD", "OSB20"));
            AddFunction(new Axis(this, MPCD_RIGHT, "3195", "195", 0.1d, 0d, 1d, "Right MFCD", "Off/Brightness Control"));
            AddFunction(new Switch(this, MPCD_RIGHT, "244", new SwitchPosition[] { new SwitchPosition("-1.0", "Day", "3017", "3017", "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("1.0", "Night", "3018", "3018", "0.0", "0.0") }, "Right MFCD", "DAY/NIGHT Mode", "%0.1f"));
            AddFunction(new Switch(this, MPCD_RIGHT, "245", new SwitchPosition[] { new SwitchPosition("1.0", "Up", "3011", "3011", "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Down", "3012", "3012", "0.0", "0.0") }, "Right MFCD", "Symbology", "%0.1f"));
            AddFunction(new Switch(this, MPCD_RIGHT, "246", new SwitchPosition[] { new SwitchPosition("1.0", "Up", "3013", "3013", "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Down", "3014", "3014", "0.0", "0.0") }, "Right MFCD", "Gain", "%0.1f"));
            AddFunction(new Switch(this, MPCD_RIGHT, "247", new SwitchPosition[] { new SwitchPosition("1.0", "Up", "3015", "3015", "0.0", "0.0"), new SwitchPosition("0.0", "Off", null), new SwitchPosition("-1.0", "Down", "3016", "3016", "0.0", "0.0") }, "Right MFCD", "Contrast", "%0.1f"));
            #endregion

            #region UFC
            AddFunction(new PushButton(this, UFCCONTROL, "3302", "302", "UFC", "UFC Keypad Pushbutton 1"));
            AddFunction(new PushButton(this, UFCCONTROL, "3303", "303", "UFC", "UFC Keypad Pushbutton 2"));
            AddFunction(new PushButton(this, UFCCONTROL, "3304", "304", "UFC", "UFC Keypad Pushbutton 3"));
            AddFunction(new PushButton(this, UFCCONTROL, "3306", "306", "UFC", "UFC Keypad Pushbutton 4"));
            AddFunction(new PushButton(this, UFCCONTROL, "3307", "307", "UFC", "UFC Keypad Pushbutton 5"));
            AddFunction(new PushButton(this, UFCCONTROL, "3308", "308", "UFC", "UFC Keypad Pushbutton 6"));
            AddFunction(new PushButton(this, UFCCONTROL, "3310", "310", "UFC", "UFC Keypad Pushbutton 7"));
            AddFunction(new PushButton(this, UFCCONTROL, "3311", "311", "UFC", "UFC Keypad Pushbutton 8"));
            AddFunction(new PushButton(this, UFCCONTROL, "3312", "312", "UFC", "UFC Keypad Pushbutton 9"));
            AddFunction(new PushButton(this, UFCCONTROL, "3315", "315", "UFC", "UFC Keypad Pushbutton 0"));
            AddFunction(new PushButton(this, UFCCONTROL, "3316", "316", "UFC", "UFC Keypad Pushbutton ."));
            AddFunction(new PushButton(this, UFCCONTROL, "3313", "313", "UFC", "UFC Keypad Pushbutton -"));
            AddFunction(new PushButton(this, UFCCONTROL, "3314", "314", "UFC", "UFC Keypad Pushbutton ENT"));
            AddFunction(new PushButton(this, UFCCONTROL, "3305", "305", "UFC", "UFC Keypad Pushbutton CLR"));
            AddFunction(new PushButton(this, UFCCONTROL, "3294", "294", "UFC", "UFC Function Selector Pushbutton TMR"));
            AddFunction(new PushButton(this, UFCCONTROL, "3324", "324", "UFC", "UFC Function Selector Pushbutton ALT"));
            AddFunction(new PushButton(this, UFCCONTROL, "3318", "318", "UFC", "UFC Function Selector Pushbutton IFF"));
            AddFunction(new PushButton(this, UFCCONTROL, "3319", "319", "UFC", "UFC Function Selector Pushbutton TCN"));
            AddFunction(new PushButton(this, UFCCONTROL, "3320", "320", "UFC", "UFC Function Selector Pushbutton AWL"));
            AddFunction(new PushButton(this, UFCCONTROL, "3317", "317", "UFC", "UFC Function Selector Pushbutton ON/OFF"));
            AddFunction(new PushButton(this, UFCCONTROL, "3325", "325", "UFC", "UFC Emission Control Pushbutton"));
            AddFunction(new PushButton(this, UFCCONTROL, "3296", "296", "UFC", "UFC Function Selector Pushbutton TOO"));
            AddFunction(new PushButton(this, UFCCONTROL, "3322", "322", "UFC", "UFC Function Selector Pushbutton WOF"));
            AddFunction(new PushButton(this, UFCCONTROL, "3321", "321", "UFC", "UFC Function Selector Pushbutton WPN"));
            AddFunction(new PushButton(this, UFCCONTROL, "3323", "323", "UFC", "UFC Function Selector Pushbutton BCN"));
            AddFunction(new PushButton(this, UFCCONTROL, "3297", "297", "UFC", "UFC I/P Pushbutton"));
            AddFunction(new PushButton(this, UFCCONTROL, "3309", "309", "UFC", "UFC Keypad Pushbutton SVE"));
            AddFunction(new PushButton(this, LTWCA, "3198", "198", "Caution/Warning", "Master Caution Button"));
            AddFunction(new FlagValue(this, "196", "Caution/Warning", "Master Caution Indicator", "Master Caution indicator"));
            AddFunction(new PushButton(this, LTWCA, "3199", "199", "Caution/Warning", "Master Warning Button"));
            AddFunction(new FlagValue(this, "197", "Caution/Warning", "Master Warning Indicator", "Master Warning indicator"));
            AddFunction(new Axis(this, UFCCONTROL, "3295", "295", 0.1d, 0d, 1d, "UFC", "UFC Brightness Control Knob"));
            AddFunction(new Axis(this, UFCCONTROL, "3298", "298", 0.1d, 0d, 1d, "UFC", "UFC COMM 1 Volume Control Knob"));
            AddFunction(new Axis(this, UFCCONTROL, "3299", "299", 0.1d, 0d, 1d, "UFC", "UFC COMM 2 Volume Control Knob"));
            AddFunction(new RotaryEncoder(this, UFCCONTROL, "3300", "300", 0.01d, "UFC", "UFC COMM 1 Channel Selector Knob"));
            AddFunction(new RotaryEncoder(this, UFCCONTROL, "3301", "301", 0.01d, "UFC", "UFC COMM 2 Channel Selector Knob"));
            AddFunction(new PushButton(this, UFCCONTROL, "3178", "178", "UFC", "UFC COMM 1 Channel Selector Pull"));
            AddFunction(new PushButton(this, UFCCONTROL, "3179", "179", "UFC", "UFC COMM 2 Channel Selector Pull"));
            AddFunction(Switch.CreateThreeWaySwitch(this, HUDCONTROL, "3288", "288", "1.0", "Norm", "0.5", "Reject 1", "0.0", "Reject 2", "HUD Control", "Declutter switch", "%.1f"));
            AddFunction(new Axis(this, HUDCONTROL, "3289", "289", 0.1d, 0d, 1d, "HUD Control", "Off/Brightness control"));
            AddFunction(Switch.CreateThreeWaySwitch(this, HUDCONTROL, "3290", "290", "0.0", "Day", "0.5", "Auto", "1.0", "Night", "HUD Control", "Display Mode switch", "%.1f"));
            AddFunction(new Axis(this, HUDCONTROL, "3291", "291", 0.1d, 0d, 1d, "HUD Control", "Video Brightness"));
            AddFunction(new Axis(this, HUDCONTROL, "3292", "292", 0.1d, 0d, 1d, "HUD Control", "Video Contrast"));
            AddFunction(Switch.CreateToggleSwitch(this, HUDCONTROL, "3293", "293", "0", "RDR", "1", "Baro", "HUD Control", "Altimeter Mode Switch", "%1d"));

            AddFunction(new Text(this, "2092", "UFC", "Left Scratchpad Data", "Value of the scratchpad left display"));
            AddFunction(new Text(this, "2094", "UFC", "Scratchpad Number", "Value of the scratchpad number display"));
            AddFunction(new Text(this, "2095", "UFC", "Comm Channel 1", "Value of Communications Channel 1 display"));
            AddFunction(new Text(this, "2096", "UFC", "Comm Channel 2", "Value of Communications Channel 2 display"));


            #endregion

            #region Master Modes
            AddFunction(new PushButton(this, MSC, "3282", "282", "Master Modes", "NAV Button"));
            AddFunction(new FlagValue(this, "283", "Master Modes", "Nav Indicator", "Nav Master Mode Indicator"));
            AddFunction(new PushButton(this, MSC, "3284", "284", "Master Modes", "VSTOL Button"));
            AddFunction(new FlagValue(this, "285", "Master Modes", "VSTOL Indicator", "VSTOL Master Mode Indicator"));
            AddFunction(new PushButton(this, MSC, "3280", "280", "Master Modes", "A/G Button"));
            AddFunction(new FlagValue(this, "281", "Master Modes", "A/G Indicator", "A/G Master Mode Indicator"));
            #endregion

            #region ODU
            AddFunction(new PushButton(this, ODUCONTROL, "3250", "250", "ODU", "ODU Option Select Pushbutton 1"));
            AddFunction(new PushButton(this, ODUCONTROL, "3251", "251", "ODU", "ODU Option Select Pushbutton 2"));
            AddFunction(new PushButton(this, ODUCONTROL, "3252", "252", "ODU", "ODU Option Select Pushbutton 3"));
            AddFunction(new PushButton(this, ODUCONTROL, "3248", "248", "ODU", "ODU Option Select Pushbutton 4"));
            AddFunction(new PushButton(this, ODUCONTROL, "3249", "249", "ODU", "ODU Option Select Pushbutton 5"));

            AddFunction(new Text(this, "2082", "ODU", "Option Display 1", "Text Value of the Option Display #1"));
            AddFunction(new Text(this, "2083", "ODU", "Option Display 2", "Text Value of the Option Display #2"));
            AddFunction(new Text(this, "2084", "ODU", "Option Display 3", "Text Value of the Option Display #3"));
            AddFunction(new Text(this, "2085", "ODU", "Option Display 4", "Text Value of the Option Display #4"));
            AddFunction(new Text(this, "2086", "ODU", "Option Display 5", "Text Value of the Option Display #5"));
            AddFunction(new Text(this, "2087", "ODU", "Option Display 1 Selected", "Option Display #1 Selected"));
            AddFunction(new Text(this, "2088", "ODU", "Option Display 2 Selected", "Option Display #2 Selected"));
            AddFunction(new Text(this, "2089", "ODU", "Option Display 3 Selected", "Option Display #3 Selected"));
            AddFunction(new Text(this, "2090", "ODU", "Option Display 4 Selected", "Option Display #4 Selected"));
            AddFunction(new Text(this, "2091", "ODU", "Option Display 5 Selected", "Option Display #5 Selected"));


            #endregion

            #region Threat Indicators
            AddFunction(new FlagValue(this, "276", "Threat Indicators", "SAM", "SAM launch detected"));
            AddFunction(new FlagValue(this, "277", "Threat Indicators", "CW", "CW Ground Tracking (Continuous Wave) radar is locked on aircraft"));
            AddFunction(new FlagValue(this, "278", "Threat Indicators", "AI", "Air Intercept radar is locked on aircraft. Flashes if launch detected"));
            AddFunction(new FlagValue(this, "279", "Threat Indicators", "AAA", "AAA gun radar is locked on aircraft"));
            #endregion

            #region Caution Indicators
            AddFunction(new FlagValue(this, "326", "Caution Indicators", "Caution L Fuel Indicator", ""));
            AddFunction(new FlagValue(this, "327", "Caution Indicators", "Caution R fuel Indicator", ""));
            AddFunction(new FlagValue(this, "328", "Caution Indicators", "Caution 15 Sec Indicator", "JPT above normal lift rating"));
            AddFunction(new FlagValue(this, "329", "Caution Indicators", "Caution MFS", "Manual Fuel System on"));
            AddFunction(new FlagValue(this, "330", "Caution Indicators", "Caution Bingo", "Bingo Warning Light"));
            AddFunction(new FlagValue(this, "331", "Caution Indicators", "Caution H2O", "Less than 15 secs water left"));
            #endregion

            #region Warning Indicators
            AddFunction(new FlagValue(this, "334", "Warning Indicators", "Warning Fire", "Fire in engine"));
            AddFunction(new FlagValue(this, "335", "Warning Indicators", "Warning LAW", "Low alt warning"));
            AddFunction(new FlagValue(this, "336", "Warning Indicators", "Warning Flaps", "Flap System Failure"));
            AddFunction(new FlagValue(this, "337", "Warning Indicators", "Warning L Tank", "Left tank over pressure"));
            AddFunction(new FlagValue(this, "338", "Warning Indicators", "Warning R Tank", "Right Tank Over pressure"));
            AddFunction(new FlagValue(this, "339", "Warning Indicators", "Warning HYD", "Both hydraulics failed"));
            AddFunction(new FlagValue(this, "340", "Warning Indicators", "Warning Gear", "Gear Warning Light"));
            AddFunction(new FlagValue(this, "341", "Warning Indicators", "Warning OT", "Over temp JPT"));
            AddFunction(new FlagValue(this, "342", "Warning Indicators", "Warning JPTL", "JPTL Control Fail"));
            AddFunction(new FlagValue(this, "343", "Warning Indicators", "Warning EFC", "All Digital engine controls failed"));
            AddFunction(new FlagValue(this, "344", "Warning Indicators", "Warning GEN", "AC Generator offline"));
            #endregion

            #region Refueling Indicators
            AddFunction(new FlagValue(this, "750", "Refueling Indicators", "Canopy Left", "Refueling Left Tank"));
            AddFunction(new FlagValue(this, "751", "Refueling Indicators", "Canopy Ready", "Refueling Ready light"));
            AddFunction(new FlagValue(this, "752", "Refueling Indicators", "Canopy Right", "Refuelng Right Light"));
            #endregion

            #region Fuel Quantity Indicator System
            AddFunction(new Axis(this, FQIS, "3380", "380", 0.001d, 0d, 1d, "Fuel Quantity", "Bingo Fuel Set Knob", true, "%.3f"));  //looping axis
            AddFunction(new Switch(this, FQIS, "379", 
                new SwitchPosition[] {
                    new SwitchPosition("-0.99", "OUTBD", "3379"),
                    new SwitchPosition("-0.66", "INBD", "3379"),
                    new SwitchPosition("-0.33", "WING", "3379"),
                    new SwitchPosition("0.0", "INT", "3379"),
                    new SwitchPosition("0.33", "TOTAL", "3379"),
                    new SwitchPosition("0.66", "FEED", "3379"),
                    new SwitchPosition("0.99", "BIT", "3379")
                }, "Fuel Quantity", "Fuel Totaliser Selector", "%0.2f"));
            AddFunction(new Digits4Display(this, FQIS, "2011", "Fuel Quantity", "Left Tank display", "Fuel left tank quantity"));
            AddFunction(new Digits4Display(this, FQIS, "2012", "Fuel Quantity", "Right Tank display", "Fuel right tank quantity"));
            AddFunction(new Digits4Display(this, FQIS, "2013", "Fuel Quantity", "Bingo value display", "Fuel Bingo amount"));
            AddFunction(new FuelTotalDisplay(this));
            AddFunction(new FlagValue(this, "365", "Fuel Quantity", "Off Flag", "FQIS is off"));
            #endregion

            #region ECM
            AddFunction(new Axis(this, RWRCONTROL, "3273", "273", 0.1d, 0d, 1d, "RWR / ECM", "Off/Volume"));
            AddFunction(new Switch(this, EWS, "274", new SwitchPosition[] {
                new SwitchPosition("0.0", "OFF", "3274"),
                new SwitchPosition("0.25", "AUTO", "3274"),
                new SwitchPosition("0.50", "UP", "3274"),
                new SwitchPosition("0.75", "Down", "3274"),
                new SwitchPosition("1.00", "RWR", "3274") 
                }, "RWR / ECM", "Decoy Dispenser Control", "%0.2f"));
            AddFunction(new Switch(this, EWS, "275", new SwitchPosition[] { 
                new SwitchPosition("0.0", "OFF", "3275")
                ,new SwitchPosition("0.25", "STBY", "3275")
                , new SwitchPosition("0.50", "BIT", "3275")
                , new SwitchPosition("0.75", "RCV", "3275")
                , new SwitchPosition("1.00", "RPT", "3275")
                }, "RWR / ECM", "Jammer Control", "%0.2f"));
            #endregion

            #region Advisory indicators
            AddFunction(new FlagValue(this, "560", "Advisory Indicators", "Advisory OXY", "OBOGS malfunction"));
            AddFunction(new FlagValue(this, "561", "Advisory Indicators", "Advisory WSHLD", "Windshield hot"));
            AddFunction(new FlagValue(this, "562", "Advisory Indicators", "Advisory HYD 1", "HYD 1 pressure ≤ 1400 psi"));
            AddFunction(new FlagValue(this, "563", "Advisory Indicators", "Advisory HYD 2", "HYD 2 pressure ≤ 1400 psi"));
            AddFunction(new FlagValue(this, "564", "Advisory Indicators", "Advisory L PUMP", "Left fuel boost pump pressure low"));
            AddFunction(new FlagValue(this, "565", "Advisory Indicators", "Advisory R PUMP", "Right fuel boost pump pressure low"));
            AddFunction(new FlagValue(this, "566", "Advisory Indicators", "Advisory L TRANS", "Low air pressure to the left feeder tank"));
            AddFunction(new FlagValue(this, "567", "Advisory Indicators", "Advisory R TRANS", "Low air pressure to the right feeder tank"));
            AddFunction(new FlagValue(this, "568", "Advisory Indicators", "Advisory FLAPS 1", "Flaps 1 channel failed"));
            AddFunction(new FlagValue(this, "569", "Advisory Indicators", "Advisory FLAPS 2", "Flaps 2 channel failed"));
            AddFunction(new FlagValue(this, "570", "Advisory Indicators", "Advisory AUT FLP", "Auto flap mode or ADC failed"));
            AddFunction(new FlagValue(this, "571", "Advisory Indicators", "Advisory PROP", "Fuel proportioner off or failed"));
            AddFunction(new FlagValue(this, "572", "Advisory Indicators", "Advisory LIDS", "LIDS not in correct position"));
            AddFunction(new FlagValue(this, "573", "Advisory Indicators", "Advisory OIL", "Oil pressure low"));
            AddFunction(new FlagValue(this, "574", "Advisory Indicators", "Advisory APU GEN", "APU selected and emergency generator failed"));
            AddFunction(new FlagValue(this, "575", "Advisory Indicators", "Advisory Blank 1", "Unused"));
            AddFunction(new FlagValue(this, "576", "Advisory Indicators", "Advisory GPS", "GPS not valid"));
            AddFunction(new FlagValue(this, "577", "Advisory Indicators", "Advisory DEP RES", "Departure resistance reduced"));
            AddFunction(new FlagValue(this, "578", "Advisory Indicators", "Advisory DC", "Main transformer-rectifier failed"));
            AddFunction(new FlagValue(this, "579", "Advisory Indicators", "Advisory STBY TRU", "Standby TRU inoperative or off line"));
            AddFunction(new FlagValue(this, "580", "Advisory Indicators", "Advisory CS COOL", "Cockpit avionics cooling fan failed"));
            AddFunction(new FlagValue(this, "581", "Advisory Indicators", "Advisory LOAD", "Fuel asymmetry over VL limit"));
            AddFunction(new FlagValue(this, "582", "Advisory Indicators", "Advisory CANOPY", "Canopy not closed and locked"));
            AddFunction(new FlagValue(this, "583", "Advisory Indicators", "Advisory INS", "INS aligning or failed"));
            AddFunction(new FlagValue(this, "584", "Advisory Indicators", "Advisory SKID", "Anti-Skid System Malfunction"));
            AddFunction(new FlagValue(this, "585", "Advisory Indicators", "Advisory EFC", "DECU 1 or DECU 2 has failed"));
            AddFunction(new FlagValue(this, "586", "Advisory Indicators", "Advisory NWS", "Nosewheel Steering malfunction"));
            AddFunction(new FlagValue(this, "587", "Advisory Indicators", "Advisory AFC", "AFC malfunction or AFC deselected"));
            AddFunction(new FlagValue(this, "588", "Advisory Indicators", "Advisory C*AUT", "Computed delivery mode (AUTO and CCIP) not available"));
            AddFunction(new FlagValue(this, "589", "Advisory Indicators", "Advisory H2O SEL", "Over 250 knots and water switch not OFF"));
            AddFunction(new FlagValue(this, "590", "Advisory Indicators", "Advisory APU", "APU operating"));
            AddFunction(new FlagValue(this, "591", "Advisory Indicators", "Advisory PITCH", "Pitch stab aug off or failed"));
            AddFunction(new FlagValue(this, "592", "Advisory Indicators", "Advisory IFF", "Mode 4 off, not zeroized or not responding"));
            AddFunction(new FlagValue(this, "593", "Advisory Indicators", "Advisory SPD BRK", "Gear up and speed brake extended. Gear down and speed brake not 250"));
            AddFunction(new FlagValue(this, "594", "Advisory Indicators", "Advisory DROOP", "Ailerons dropped"));
            AddFunction(new FlagValue(this, "595", "Advisory Indicators", "Advisory ROLL", "Roll stab aug off or failed"));
            AddFunction(new FlagValue(this, "596", "Advisory Indicators", "Advisory AFT BAY", "Aft avionics bay ECS failed"));
            AddFunction(new FlagValue(this, "597", "Advisory Indicators", "Advisory AV BIT", "Refuelng Right Light"));
            AddFunction(new FlagValue(this, "598", "Advisory Indicators", "Advisory Blank 2", "Unused"));
            AddFunction(new FlagValue(this, "599", "Advisory Indicators", "Advisory YAW", "Yaw stab aug off or failed"));
            AddFunction(new FlagValue(this, "600", "Advisory Indicators", "Advisory CW NOGO", "Jammer Failure. Cannot jam CW radars"));
            AddFunction(new FlagValue(this, "601", "Advisory Indicators", "Advisory P JAM", "Jammer Pod Active: Jamming Pulse-Doppler radar signals"));
            AddFunction(new FlagValue(this, "602", "Advisory Indicators", "Advisory JMR HOT", "Jammer pod overtemp"));
            AddFunction(new FlagValue(this, "603", "Advisory Indicators", "Advisory ENG EXC", "Engine overspeed, overtemperature or over g was detected"));
            AddFunction(new FlagValue(this, "604", "Advisory Indicators", "Advisory P NOGO", "Jammer Failure. Cannot jam Pulse-Doppler radars"));
            AddFunction(new FlagValue(this, "605", "Advisory Indicators", "Advisory CW JAM", "Jammer Pod Active: Jamming CW radar signals"));
            AddFunction(new FlagValue(this, "606", "Advisory Indicators", "Advisory REPLY", "IFF responding to Mode 4 interrogation"));
            #endregion

            #region Landing Gear
            AddFunction(new FlagValue(this, "446", "Landing Gear", "Gear - Lever", "Red Gear Lever"));
            AddFunction(new FlagValue(this, "462", "Landing Gear", "Nose Wrn", "Nose Gear Warning"));
            AddFunction(new FlagValue(this, "463", "Landing Gear", "Nose", "Nose Gear Ready"));
            AddFunction(new FlagValue(this, "464", "Landing Gear", "Left Wrn", "Left Wheel Warning"));
            AddFunction(new FlagValue(this, "465", "Landing Gear", "Left", "Left Wheel Ready"));
            AddFunction(new FlagValue(this, "466", "Landing Gear", "Right Wrn", "Right Wheel Warning"));
            AddFunction(new FlagValue(this, "467", "Landing Gear", "Right", "Right Wheel Ready"));
            AddFunction(new FlagValue(this, "469", "Landing Gear", "Main Wrn", "Main Gear Warning"));   // I think the codes are switched for this and might get corrected in future by Razbam
            AddFunction(new FlagValue(this, "468", "Landing Gear", "Main", "Main Gear Ready"));         // I think the codes are switched for this and might get corrected in future by Razbam
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3461", "461", "1", "Gear up", "0", "Gear down", "Landing Gear", "lever", "%1d"));
            AddFunction(new PushButton(this, SMC, "3458", "458", "Landing Gear", "Emergency Jettison Button"));
            AddFunction(new PushButton(this, FLIGHTCONTROLS, "3448", "448", "Landing Gear", "Gear Down Lock Override Button"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3447", "447", "1.0", "Gear up", "0", "Gear down", "Landing Gear", "Emergency Landing Gear Lever", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3470", "470", "1.0", "Up", "0", "Down", "Landing Gear", "Landing Gear Emergency Battery Lever", "%1d"));
            #endregion

            #region Left Hand Advisory Indicators
            AddFunction(new FlagValue(this, "451", "LH Flaps & Water", "SEL indicator", "Combat thrust limiter selected"));
            AddFunction(new FlagValue(this, "452", "LH Flaps & Water", "CMBT indicator", "Combat thrust activated. Flashes after 2 ½ minutes"));
            AddFunction(new FlagValue(this, "453", "LH Flaps & Water", "STO indicator", "Flap switch in STOL"));
            AddFunction(Switch.CreateThreeWaySwitch(this, DECS, "3449", "449", "1.0", "Take Off", "0.5", "Off", "0.0", "Landing", "LH Flaps & Water", "H2O Mode Switch", "%.1f"));
            AddFunction(new PushButton(this, DECS, "3450", "450", "LH Flaps & Water", "Combat Thrust Button"));
            AddFunction(Switch.CreateThreeWaySwitch(this, VREST, "3454", "454", "1.0", "Cruise", "0.5", "Auto", "0.0", "STOL", "LH Flaps & Water", "Flaps Mode Switch", "%.1f"));
            AddFunction(new Digits2Display(this, SMC, "2014", "LH Flaps & Water", "Flaps position", "Position of the flaps in degrees"));
            AddFunction(new PushButton(this, VREST, "3460", "460", "LH Flaps & Water", "Flaps BIT"));
            AddFunction(Switch.CreateThreeWaySwitch(this, VREST, "3457", "457", "0.0", "Off", "0.5", "On", "1.0", "Reset", "LH Flaps & Water", "Flaps Power Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, FLIGHTCONTROLS, "3459", "459", "1.0", "Test", "0.5", "On", "0.0", "NWS", "LH Flaps & Water", "Anti-Skid Switch", "%.1f"));
            #endregion

            #region Centre Console

            //-- Misc Switch Panel
            AddFunction(Switch.CreateToggleSwitch(this, NAVFLIR, "3422", "422", "0", "Auto", "1", "Run", "Centre Console", "Video Recorder System Mode Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, NAVFLIR, "3423", "423", "0", "MPCD", "1", "HUD", "Centre Console", "Video Recorder System Display Selector Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DMT, "3424", "424", "1", "DMT", "0", "Off", "Centre Console", "DMT Toggle On/Off", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, MSC, "3425", "425", "1", "DP Prim", "0.5", "Alter", "0", "Alter", "Centre Console", "Dual Processor Mode Selector Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTINSTRUMENTS, "3426", "426", "1", "Probe Heat", "0", "Auto", "Centre Console", "Probe Heat Mode Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, MSC, "3427", "427", "1", "Mission Computer OVerride", "0.5", "Auto", "0", "Off", "Centre Console", "Mission Computer Mode Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, NAVFLIR, "3429", "429", "1", "FLIR", "0", "Off", "Centre Console", "FLIR Power Switch", "%1d"));
            AddFunction(new Switch(this, NAV_INS, "421", new SwitchPosition[] { new SwitchPosition("0.0", "Off", "3421"), new SwitchPosition("0.1", "Sea", "3421"), new SwitchPosition("0.2", "INS GND", "3421"), new SwitchPosition("0.3", "Nav", "3421"), new SwitchPosition("0.4", "IFA", "3421"), new SwitchPosition("0.5", "Gyro", "3421"), new SwitchPosition("0.6", "GB", "3421"), new SwitchPosition("0.7", "Test", "3421") }, "Centre Console", "INS Mode Switch", "%.1f"));

            #endregion

            #region Throttle Quadrant
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3481", "481", "1", "Off", "0", "On", "Throttle Quadrant", "JPTL Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3482", "482", "1", "On", "0", "Off", "Throttle Quadrant", "EMS Button", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3484", "484", "1", "On", "0", "Off", "Throttle Quadrant", "Manual Fuel Switch", "%1d"));
            AddFunction(new Axis(this, FLIGHTCONTROLS, "3485", "485", 0.1d, 0d, 1d, "Throttle Quadrant", "Throttle Lever Friction Knob"));
            AddFunction(new Axis(this, FLIGHTCONTROLS, "3486", "486", 0.1d, 0d, 1d, "Throttle Quadrant", "Nozzle Lever Friction Knob"));
            AddFunction(new Axis(this, DECS, "3490", "490", 0.1d, 0d, 1d, "Throttle Quadrant", "Throttle Cutoff Lever"));
            AddFunction(new Axis(this, FLIGHTCONTROLS, "3489", "489", 0.1d, 0d, 1d, "Throttle Quadrant", "Parking Brake Lever"));
            AddFunction(new Axis(this, VREST, "3487", "487", 0.1d, 0d, 1d, "Throttle Quadrant", "Nozzle Control Lever"));
            AddFunction(new Axis(this, VREST, "3488", "488", 0.1d, 0d, 1d, "Throttle Quadrant", "STO Stop Lever"));

            #endregion

            #region Left Hand Left Hand Switches Fuel External Lights SAAHS
            ////         --Trim Panel
            AddFunction(Switch.CreateThreeWaySwitch(this, FLIGHTCONTROLS, "3471", "471", "1.0", "Test", "0.5", "On", "0.0", "Off", "SAAHS", "RPS/YAW Trim Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, FLIGHTCONTROLS, "3483", "483", "1", "Left", "0", "Centre", "-1", "Right", "SAAHS", "Rudder trim switch", "%.1f"));

            CalibrationPointCollectionDouble trimScale = new CalibrationPointCollectionDouble(-1.0d, -1d, 1.0d, 1d);
            AddFunction(new ScaledNetworkValue(this, "473", trimScale, "SAAHS", "Aileron trim", "Position in degrees","", BindingValueUnits.Degrees));  // values at -1 to 1
            AddFunction(new ScaledNetworkValue(this, "474", trimScale, "SAAHS", "Rudder trim", "Position in degrees","", BindingValueUnits.Degrees));  // values at -1 to 1
            //         --SAAHS Panel
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3476", "476", "1", "Hold", "0", "Off", "SAAHS", "Altitude hold switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, FLIGHTCONTROLS, "3477", "477", "1.0", "On", "0.5", "Off", "0.0", "Reset", "SAAHS", "AFC Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3475", "475", "1", "On", "0", "Off", "SAAHS", "Q Feel switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3478", "478", "1", "On", "0", "Off", "SAAHS", "SAS Yaw Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3479", "479", "1", "On", "0", "Off", "SAAHS", "SAS Roll Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3480", "480", "1", "On", "0", "Off", "SAAHS", "SAS Pitch Switch", "%1d"));
            //         --Fuel Panel
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3504", "504", "1", "On", "0", "Off", "Fuel", "Fuel Proportioner", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3508", "508", "1", "Dump", "0", "Off", "Fuel", "Fuel Dump L Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3509", "509", "1", "Dump", "0", "Off", "Fuel", "Fuel Dump R Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, DECS, "3505", "505", "1.0", "Norm", "0.5", "Off", "0.0", "DC Oper", "Fuel", "Fuel Pump L Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, DECS, "3506", "506", "1.0", "Norm", "0.5", "Off", "0.0", "DC Oper", "Fuel", "Fuel Pump R Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, DECS, "3507", "507", "1.0", "Press", "0.5", "Out", "0.0", "In", "Fuel", "Air Refueling Probe Switch", "%.1f"));

            //          -- External Lights Panel
            AddFunction(Switch.CreateThreeWaySwitch(this, LTEXT, "3472", "472", "1.0", "Bright", "0.5", "Dim", "0.0", "Off", "External Lights", "Landing/Taxi Lights Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, LTEXT, "3503", "503", "1.0", "Bright", "0.5", "Dim", "0.0", "Off", "External Lights", "External Lights Mode Switch", "%.1f"));
            AddFunction(new Axis(this, LTEXT, "3510", "510", 0.1d, 0d, 1d, "External Lights", "Formation Lights brightness"));
            AddFunction(Switch.CreateThreeWaySwitch(this, LTEXT, "3511", "511", "1.0", "Bright", "0.5", "Dim", "0.0", "Off", "External Lights", "Position Lights Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, LTEXT, "3512", "512", "1", "On", "0", "Off", "External Lights", "Anti-Collision Lights Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, LTEXT, "3513", "513", "1", "On", "0", "Off", "External Lights", "External Auxiliary Lights Switch", "%1d"));

            //          -- Pilot Service Panel
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3514", "514", "1", "On", "0", "Off", "Pilot Service", "Oxygen Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3515", "515", "1", "Dump", "0", "Off", "Pilot Service", "H2O Dump Switch", "%1d"));
            // need proper labels
            AddFunction(Switch.CreateToggleSwitch(this, VREST, "3516", "516", "1", "On", "0", "Off", "Pilot Service", "LIDS Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3517", "517", "1", "On", "0", "Off", "Pilot Service", "ENG RPM Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3518", "518", "1", "On", "0", "Off", "Pilot Service", "EFC Switch", "%1d"));
            #endregion

            #region Stores Management Controller
            AddFunction(Switch.CreateToggleSwitch(this, SMC, "3287", "287", "1", "Armed", "0", "Safe", "Stores Management", "Master Arm Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, SMC, "3396", "396", "1", "Up", "0", "Off", "-1", "Dn", "Stores Management", "Armament Mode control", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, SMC, "3397", "397", "1", "Increase", "0", "Off", "-1", "Decrease", "Stores Management", "Armament Fuzing control", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, SMC, "3398", "398", "1", "Increase", "0", "Off", "-1", "Decrease", "Stores Management", "Armament Quantity Tens", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, SMC, "3399", "399", "1", "Increase", "0", "Off", "-1", "Decrease", "Stores Management", "Armament Quantity Units", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, SMC, "3400", "400", "1", "Increase", "0", "Off", "-1", "Decrease", "Stores Management", "Armament Multiple Release", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, SMC, "3401", "401", "1", "Increase", "0", "Off", "-1", "Decrease", "Stores Management", "Armament Release interval hundreds", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, SMC, "3402", "402", "1", "Increase", "0", "Off", "-1", "Decrease", "Stores Management", "Armament Release interval tens", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, SMC, "3403", "403", "1", "Increase", "0", "Off", "-1", "Decrease", "Stores Management", "Armament Release interval units", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, SMC, "3420", "420", "1", "Cooling", "0", "Off", "Stores Management", "IR Cool Switch", "%1d"));
            AddFunction(new PushButton(this, SMC, "3407", "407", "Stores Management", "Station 1 Button"));
            AddFunction(new FlagValue(this, "406", "Stores Management", "Station 1 Selected Indicator", "Station selected flag"));
            AddFunction(new PushButton(this, SMC, "3409", "409", "Stores Management", "Station 2 Button"));
            AddFunction(new FlagValue(this, "408", "Stores Management", "Station 2 Selected Indicator", "Station selected flag"));
            AddFunction(new PushButton(this, SMC, "3411", "411", "Stores Management", "Station 3 Button"));
            AddFunction(new FlagValue(this, "410", "Stores Management", "Station 3 Selected Indicator", "Station selected flag"));
            AddFunction(new PushButton(this, SMC, "3413", "413", "Stores Management", "Station 4 Button"));
            AddFunction(new FlagValue(this, "412", "Stores Management", "Station 4 Selected Indicator", "Station selected flag"));
            AddFunction(new PushButton(this, SMC, "3415", "415", "Stores Management", "Station 5 Button"));
            AddFunction(new FlagValue(this, "414", "Stores Management", "Station 5 Selected Indicator", "Station selected flag"));
            AddFunction(new PushButton(this, SMC, "3417", "417", "Stores Management", "Station 6 Button"));
            AddFunction(new FlagValue(this, "416", "Stores Management", "Station 6 Selected Indicator", "Station selected flag"));
            AddFunction(new PushButton(this, SMC, "3419", "419", "Stores Management", "Station 7 Button"));
            AddFunction(new FlagValue(this, "418", "Stores Management", "Station 7 Selected Indicator", "Station 7 Selected Indicator"));
            AddFunction(new PushButton(this, SMC, "3286", "286", "Stores Management", "Launch Flare Salvo"));
            AddFunction(new Switch(this, SMC, "404", new SwitchPosition[] { new SwitchPosition("-1.0", "STA", "3404"), new SwitchPosition("-0.5", "STOR", "3404"), new SwitchPosition("0.0", "SAFE", "3404"), new SwitchPosition("0.5", "CMBT", "3404"), new SwitchPosition("1.0", "FUEL", "3404") },  "Stores Management", "Jettison Mode Selector", "%0.1f"));
            AddFunction(new PushButton(this, SMC, "405", "405", "Stores Management", "Jettison Stores"));
            AddFunction(new Switch(this, SMC, "395", new SwitchPosition[] { new SwitchPosition("0.0", "Norm", "3395"), new SwitchPosition("0.33", "N/T", "3395"), new SwitchPosition("0.66", "N", "3395"), new SwitchPosition("1.0", "T", "3395")  }, "Stores Management", "Manual Fuzing Release Control", "%0.2f"));
            AddFunction(new Digits3Display(this, SMC, "2020", "Stores Management", "Stores interval display", "Interval value in metres"));
            AddFunction(new Digits2Display(this, SMC, "2022", "Stores Management", "Stores quantity display", "Quantity of stores"));
            AddFunction(new SMCMultipleDisplay(this));
            AddFunction(new NetworkValue(this, "385", "Stores Management", "SMC mode (value)", "Current SMC mode in value form", "", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "386", "Stores Management", "Fuze mode Left (value)", "Fuze mode (Left Drum)", "", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "387", "Stores Management", "Fuze mode Right (value)", "Fuze mode (Right Drum)", "", BindingValueUnits.Numeric));
            AddFunction(new SMCFuzeDisplay(this, SMC, "2019", "Stores Management", "Fuze Mode", "Fuze mode in combined form"));
            #endregion

            #region Left Bulkhead
            // switch positions still need to be labeled
            AddFunction(Switch.CreateToggleSwitch(this, MSC, "3502", "502", "1", "Pressed", "0", "Off", "Left Bulkhead", "Seat adjustment switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3519", "519", "0", "Off", "1", "Pressed", "Left Bulkhead", "Fuel Shutoff Lever", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DECS, "3520", "520", "1", "On", "0", "Off", "Left Bulkhead", "DECS switch", "%1d"));

            #endregion

            #region V/UHF Radio
            // switch positions still need to be labeled
            AddFunction(new Axis(this, RSC, "3614", "614", 0.03d, 0d, 1d, "V/UHF Radio", "Volume Knob"));
            AddFunction(new Axis(this, RSC, "3615", "615", 0.03d, 0d, 1d, "V/UHF Radio", "jet"));
            AddFunction(new Switch(this, RSC, "616", new SwitchPosition[] {new SwitchPosition("-0.2", "Zero", "3616"), new SwitchPosition("0.0", "Off", "3616"), new SwitchPosition("0.2", "Test", "3616"), new SwitchPosition("0.4", "TR+G", "3616"), new SwitchPosition("0.6", "TR", "3616"), new SwitchPosition("0.8", "ADF", "3616"), new SwitchPosition("1.0", "Chng PRST", "3616")}, "V/UHF Radio", "Operational Mode Switch", "%0.1f"));
            AddFunction(new PushButton(this, RSC, "3617", "617", "V/UHF Radio", "Ancillary Mode Pointer A mode"));
            AddFunction(new PushButton(this, RSC, "3618", "618", "V/UHF Radio", "Ancillary Mode Switch P mode"));
            AddFunction(new Switch(this, RSC, "619", new SwitchPosition[] {new SwitchPosition("0.0", "AJ/M", "3619"), new SwitchPosition("0.15", "AJ", "3619"), new SwitchPosition("0.30", "MAR", "3619"), new SwitchPosition("0.45", "PRST", "3619"), new SwitchPosition("0.60", "MAN", "3619"), new SwitchPosition("0.75", "243", "3619"), new SwitchPosition("0.90", "121", "3619")}, "V/UHF Radio", "Frequency Mode Switch", "%0.2f"));
            AddFunction(new PushButton(this, RSC, "3620", "620", "V/UHF Radio", "LOAD/OFST Switch"));
            AddFunction(new Text(this, "2100", "V/UHF Radio", "Channel Number", "Radio Channel Number text display"));
            AddFunction(new Text(this, "2101", "V/UHF Radio", "Frequency", "Radio Frequency text display"));

            #endregion

            #region ACNIP
            // switch positions still need to be labeled
            AddFunction(Switch.CreateToggleSwitch(this, ACNIP, "3621", "621", "1", "MAN", "0", "UFC", "ACNIP", "ACNIP Mode Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ACNIP, "3622", "622", "1", "DIPH", "0", "BB", "ACNIP", "KY-1 Cipher Type Selector Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ACNIP, "3623", "623", "1", "DIPH", "0", "BB", "ACNIP", "KY-2 Cipher Type Selector Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ACNIP, "3624", "624", "1", "Code", "0", "Mode", "ACNIP", "KY-1 Code/Mode Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ACNIP, "3625", "625", "1", "Code", "0", "Mode", "ACNIP", "KY-2 Code/Mode Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ACNIP, "3626", "626", "1", "On", "0", "Off", "ACNIP", "ACNIP Radio Selector Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ACNIP, "3627", "627", "1", "On", "0", "Off", "ACNIP", "KY-58 Codes Clear Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ACNIP, "3628", "628", "1", "On", "0", "Off", "-1", "Off", "ACNIP", "KY-58 Remote Codes Load Switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ACNIP, "3632", "632", "1", "On", "0", "Off", "ACNIP", "IFF Operational Mode Switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ACNIP, "3633", "633", "1.0", "On", "0.0", "Mid", "-1.0", "Off", "ACNIP", "IFF Crypto Mode Switch", "%.1f"));

            AddFunction(new Text(this, "2102", "ACNIP", "ACNIP 1 Mode Label", "Title for the ACNIP 1 Mode display"));
            AddFunction(new Text(this, "2103", "ACNIP", "ACNIP 1 Mode", "ACNIP 1 Mode display"));
            AddFunction(new Text(this, "2104", "ACNIP", "ACNIP 1 Code Label", "Title for the ACNIP 1 Code display"));
            AddFunction(new Text(this, "2105", "ACNIP", "ACNIP 1 Code", "ACNIP 1 Code display"));
            AddFunction(new Text(this, "2106", "ACNIP", "ACNIP 2 Mode Label", "Title for the ACNIP 2 Mode display"));
            AddFunction(new Text(this, "2107", "ACNIP", "ACNIP 2 Mode", "ACNIP 2 Mode display"));
            AddFunction(new Text(this, "2108", "ACNIP", "ACNIP 2 Code Label", "Title for the ACNIP 2 Code display"));
            AddFunction(new Text(this, "2109", "ACNIP", "ACNIP 2 Code", "ACNIP 2 Code display"));


            #endregion

            #region ICS IFF
            // switch positions still need to be labeled
            AddFunction(new Axis(this, INTERCOM, "3629", "629", 0.03d, 0d, 1d, "Intercomm", "Aux Volume Knob"));
            AddFunction(new Axis(this, INTERCOM, "3630", "630", 0.03d, 0d, 1d, "Intercomm", "Ground Volume Knob"));
            AddFunction(new Switch(this, INTERCOM, "631", new SwitchPosition[] { new SwitchPosition("1.0", "Norm", "3631"), new SwitchPosition("0.5", "Norm", "3631"), new SwitchPosition("0.0", "Norm", "3631") }, "Intercomm", "Mic Operational Mode Switch", "%0.1f"));

            #endregion

            #region Interior Lights
            AddFunction(Switch.CreateThreeWaySwitch(this, LTINT, "3634", "634", "0.0", "Compass", "0.5", "Off", "1.0", "Lights Test","Interior Lights", "Compass Light/Test Lights","%.1f")); //default_tumb_button(_("Compass Light/Test Lights") * * * Not sure if this is correct
            AddFunction(new Axis(this, LTINT, "3635", "635", 0.03d, 0d, 1d, "Interior Lights", "Instruments Lights"));
            AddFunction(new Axis(this, LTINT, "3636", "636", 0.03d, 0d, 1d, "Interior Lights", "Console Lights"));
            AddFunction(new Axis(this, LTINT, "3637", "637", 0.03d, 0d, 1d, "Interior Lights", "Flood Lights"));
            AddFunction(new Axis(this, LTINT, "3638", "638", 0.03d, 0d, 1d, "Interior Lights", "Annunciator Lights"));
            AddFunction(new Axis(this, LTINT, "3150", "150", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Right Canopy Frame Top"));
            AddFunction(new Axis(this, LTINT, "3151", "151", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Right Canopy Frame Bottom"));
            AddFunction(new Axis(this, LTINT, "3152", "152", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Right Bulkhead Forward"));
            AddFunction(new Axis(this, LTINT, "3153", "153", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Right Bulkhead Aft Front"));
            AddFunction(new Axis(this, LTINT, "3154", "154", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Right Bulkhead Aft Back"));
            AddFunction(new Axis(this, LTINT, "3155", "155", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Left Bulkhead Aft Back"));
            AddFunction(new Axis(this, LTINT, "3156", "156", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Left Bulkhead Aft Front"));
            AddFunction(new Axis(this, LTINT, "3157", "157", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Left Bulkhead Forward"));
            AddFunction(new Axis(this, LTINT, "3158", "158", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Left Canopy Frame Bottom"));
            AddFunction(new Axis(this, LTINT, "3159", "159", 0.03d, -1d, 1d, "Interior Lights", "Flood Lamp Left Canopy Frame Top"));

            #endregion

            #region Canopy Controls
            // switch positions still need to be labeled
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3801", "801", "1", "On", "0", "Off", "Canopy Controls", "Canopy Handle Left", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3802", "802", "1", "On", "0", "Off", "Canopy Controls", "Canopy Handle Right", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3803", "803", "1", "On", "0", "Off", "Canopy Controls", "Canopy Locking Lever", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, MSC, "3800", "800", "1", "On", "0", "Off", "Canopy Controls", "Seat Ground Safety Lever", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTCONTROLS, "3501", "501", "1", "On", "0", "Off", "Canopy Controls", "MFS Emergency Lever", "%1d"));
            #endregion

            #region Stopwatch
            AddFunction(new PushButton(this, MSC, "3051", "1121", "Stopwatch", "Start/Stop Button"));
            AddFunction(new PushButton(this, MSC, "3052", "1122", "Stopwatch", "Lap/Reset Button"));
            AddFunction(new ScaledNetworkValue(this, "1119", 60d, "Stopwatch", "Minutes", "Current minute of the stopwatch.", "(0-60)", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "1120", 60d, "Stopwatch", "Seconds", "Current seconds of the stopwatch.", "(0-60)", BindingValueUnits.Numeric));

            #endregion

            #region Clock
            AddFunction(new PushButton(this, MSC, "3758", "758", "Clock", "Button"));
            AddFunction(new RotaryEncoder(this, MSC, "3757", "757", 0.01d, "Clock", "Knob"));
            AddFunction(new ScaledNetworkValue(this, "759", 12d, "Clock", "Hours", "Current seconds of the clock.", "(0-12)", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "760", 60d, "Clock", "Minutes", "Current minute of the clock.", "(0-60)", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "761", 60d, "Clock", "Seconds", "Current seconds of the clock.", "(0-60)", BindingValueUnits.Numeric));

            #endregion

            #region ECS
            AddFunction(new Axis(this, ECS, "3639", "639", 0.03d, 0d, 1d, "Environment Control", "Temperature Control Knob"));
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3640", "640", "0", "Norm", "1", "Reset", "Environment Control", "Fwd Equipment Bay ECS Switch", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ECS, "3641", "641", "1.0", "Norm", "0.5", "Defog", "0.0", "Max", "Environment Control", "Cabin Defog Switch", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ECS, "3642", "642", "1.0", "Reset", "0.5", "On", "0.0", "Off", "Environment Control", "Aft Equipment Bay ECS Switch", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ECS, "3643", "643", "1.0", "Press Norm", "0.5", "Dump", "0.0", "Ram", "Environment Control", "Cabin Pressure Switch", "%0.1f"));
            #endregion

            #region Engine Display Panel
            AddFunction(new ScaledNetworkValue(this, "271", 0.94d, "EDP", "Nozzle Position", "Current Nozzle position.", "Nozzle position in degrees", BindingValueUnits.Degrees));
            AddFunction(new PushButton(this, EDP, "3655", "655", "EDP", "BIT"));
            AddFunction(new Axis(this, EDP, "3272", "272", 0.1d, 0d, 1d, "EDP", "Off/Brightness Control"));
            AddFunction(new Digits2Display(this, EDP, "2006", "EDP", "H2O display", "Amount of H2O available"));
            AddFunction(new Digits4Display(this, EDP, "2002", "EDP", "RPM display", "Engine RPM percentage"));
            AddFunction(new Digits3Display(this, EDP, "2001", "EDP", "Duct pressure display", "Engine Duct pressure"));
            AddFunction(new Digits3Display(this, EDP, "2003", "EDP", "FF display", "Engine FF percentage"));
            AddFunction(new Digits3Display(this, EDP, "2004", "EDP", "JPT display", "Engine Jet pipe temperature"));
            AddFunction(new Digits2Display(this, EDP, "2005", "EDP", "Stabiliser display", "Amount of Stabiliser"));
            AddFunction(new NetworkValue(this, "266", "EDP", "Stabilzer Arrow", "Up/Down Arrow for the stabilizer"," value -1 to 1", BindingValueUnits.Numeric));
            AddFunction(new FlagValue(this, "177", "EDP", "H2O flow indicator", "")); 
            #endregion

            #region Electrical Panel
            //Battery
            CalibrationPointCollectionDouble batteryScale = new CalibrationPointCollectionDouble(0.0d, 0d, 0.30d, 30d);
            AddFunction(new ScaledNetworkValue(this, "608", batteryScale, "Electrical", "Battery Voltage", "Voltage of battery 15v to 30v", "", BindingValueUnits.Volts));
            AddFunction(Switch.CreateThreeWaySwitch(this, ELECTRIC, "3613", "613", "1.0", "On", "0.5", "Off", "0.0", "Alert", "Electrical", "Battery switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ELECTRIC, "3612", "612", "1.0", "On", "0.5", "Off", "0.0", "Test", "Electrical", "Generator switch", "%.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ELECTRIC, "3611", "611", "1", "Start", "0", "Off", "Electrical", "Engine start switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ELECTRIC, "3610", "610", "1.0", "Reset", "0.5", "On", "0.0", "Off", "Electrical", "APU generator switch", "%.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ELECTRIC, "3609", "609", "0.0", "Main", "0.5", "Off", "1.0", "Standby", "Electrical", "DC Test switch", "%.1f"));
            #endregion

            #region Brake / Hydraulic Pressures
            AddFunction(new Digits3Display(this, FLIGHTCONTROLS, "2015", "Brake/ Hydraulic", "Brake pressure display", "Brake pressure in psi"));
            AddFunction(new Digits3Display(this, FLIGHTCONTROLS, "2016", "Brake/ Hydraulic", "Hyd 1 pressure display", "Hydraulic system 1 pressure in psi"));
            AddFunction(new Digits3Display(this, FLIGHTCONTROLS, "2017", "Brake/ Hydraulic", "Hyd 2 pressure display", "Hydraulic system 2 pressure in psi"));
            // Brake accumulator pressure           
            CalibrationPointCollectionDouble accummulatorScale = new CalibrationPointCollectionDouble(0d, 0d, 0.36d, 3600d);
            AddFunction(new ScaledNetworkValue(this, "559", accummulatorScale, "Brake/ Hydraulic", "Brake accummulator", "Brake accummulator pressure in psi 600 to 3600 psi", "", BindingValueUnits.PoundsPerSquareInch));
            //Cabin Altitude Pressure 
            CalibrationPointCollectionDouble cabinScale = new CalibrationPointCollectionDouble(-0.003d,-300d,0.5000d, 50000d);
            cabinScale.Add(new CalibrationPointDouble(0d, 0d));
            AddFunction(new ScaledNetworkValue(this, "607", cabinScale, "Brake/ Hydraulic", "Cabin Altitude", "Cabin altitude pressue in feet 0 to +50000.", "", BindingValueUnits.Numeric));

            #endregion

            #region Flight Instruments

            AddFunction(new Altimeter(this,"Flight Instruments","2051","Altitude", "Barometric altitude above sea level of the aircraft.", "Value is adjusted per altimeter pressure setting.", "2059","Air Pressure", "Manually set barometric altitude.",""));
            AddFunction(new RotaryEncoder(this, ADC, "3653", "653", 0.01d, "Flight Instruments", "Barometric pressure calibration adjust"));
 
            CalibrationPointCollectionDouble vviScale = new CalibrationPointCollectionDouble(-0.6d, -6000d, 0.6d, 6000d);
            vviScale.Add(new CalibrationPointDouble(0d, 0d));
            AddFunction(new ScaledNetworkValue(this, "362", vviScale, "Flight Instruments", "VVI", "Vertical velocity indicator -6000 to +6000.", "", BindingValueUnits.FeetPerMinute));

            CalibrationPointCollectionDouble AoAScale = new CalibrationPointCollectionDouble(-0.05d, -5d, 0.20d, 20d);
            AoAScale.Add(new CalibrationPointDouble(0d, 0d));
            AddFunction(new ScaledNetworkValue(this, "361", AoAScale, "Flight Instruments", "Angle of Attack", "Current angle of attack of the aircraft.", "", BindingValueUnits.Degrees));
            AddFunction(new FlagValue(this, "360", "Flight Instruments", "AOA Flag", "Off Flag"));

            CalibrationPointCollectionDouble airspeedScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 1000d);
            AddFunction(new ScaledNetworkValue(this, "346", airspeedScale, "Flight Instruments", "IAS Airspeed", "Current indicated air speed of the aircraft.", "", BindingValueUnits.Knots));

            AddFunction(Switch.CreateThreeWaySwitch(this, NAV_INS, "3364", "364", "1", "Left", "0", "Centre", "-1", "Right", "NAV course", "Course Setting", "%.1f"));

            AddFunction(new ScaledNetworkValue(this, "349", 90d, "Flight Instruments", "SAI Pitch", "Current pitch displayed on the SAI.", "", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "348", -180d, "Flight Instruments", "SAI Bank", "Current bank displayed on the SAI.", "", BindingValueUnits.Degrees));
            AddFunction(new DCSBallRotation(this, "2050", "Flight Instruments", "SAI Gauge Rotation", "Single value containing X, Y & Z movement of the ADI Gauge.", new CalibrationPointCollectionDouble(-1.0d, -90d, 1.0d, 90d), null, new CalibrationPointCollectionDouble(-1d, -180d, 1d, 180d)));
            AddFunction(new FlagValue(this, "347", "Flight Instruments", "SAI Warning Flag", "Displayed when SAI is caged or non-functional."));
            AddFunction(new Axis(this, FLIGHTINSTRUMENTS, "3351", "351", 0.1d, -1d, 1d, "Flight Instruments", "SAI Pitch Adjust Knob"));
            AddFunction(new PushButton(this, FLIGHTINSTRUMENTS, "3350", "350", "Flight Instruments", "SAI Cage Pull Switch"));

            AddFunction(new NetworkValue(this, "363", "Flight Instruments", "Slip Ball", "Current position of the slip ball relative to the center of the tube.", "(-1 to 1) -1 is full left and 1 is full right.", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "652", "Flight Instruments", "Turn Indicator", "Current position of the turn indicator.", "(-1 to 1) -1 is full left and 1 is full right.", BindingValueUnits.Numeric));
            AddFunction(new FlagValue(this, "654", "Flight Instruments", "Slip/Turn Flag", "OFF Flag for the Slip / Turn gauge."));

            #endregion

        }
    }
}
