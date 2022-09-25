//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.A10C
{
    using Functions;
    using Common;

    /// <summary>
    /// common base for interfaces for DCS A-10C and DCS A-10C II, containing only functions that are the
    /// same in both aircraft
    /// </summary>
    public class A10CInterface : DCSInterface
    {
        #region Devices
        // ReSharper disable InconsistentNaming
        private const string ELEC_INTERFACE = "1";
        private const string MFCD_LEFT = "2";
        private const string MFCD_RIGHT = "3";
        private const string CMSP = "4";
        private const string CMSC = "5";
        private const string AHCP = "7";
        private const string UFC = "8";
        private const string CDU = "9";
        private const string IFFCC = "12";
		private const string DIGITAL_CLOCK = "15";
		private const string AAP = "22";
		private const string AN_ALR69V = "29";
		private const string SYS_CONTROLLER = "24";
        private const string FM_PROXY = "35";
        private const string FUEL_SYSTEM = "36";
        private const string ENGINE_SYSTEM = "37";
        private const string AUTOPILOT = "38";
        private const string CPT_MECH = "39";
        private const string OXYGEN_SYSTEM = "40";
        private const string ENVIRONMENT_SYSTEM = "41";
        private const string IFF = "43";
        private const string HARS = "44";
        private const string HSI = "45";
        private const string NMSP = "46";
        private const string ADI = "47";
        private const string SAI = "48";
        private const string LIGHT_SYSTEM = "49";
        private const string FIRE_SYSTEM = "50";
        // ReSharper disable once UnusedMember.Local there are no known controls in this device
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "there are no known controls in this device")]
        private const string TACAN = "51";
        private const string STALL = "52";
        private const string ILS = "53";
        private const string UHF_RADIO = "54";
        private const string VHF_AM_RADIO = "55";
        private const string VHF_FM_RADIO = "56";
        private const string INTERCOM = "58";
		private const string AAU34 = "62";
		private const string AN_APN_194 = "67";
        private const string KY_58 = "69";
		private const string DVADR = "73";
		private const string ACCELEROMETER = "72";
        private const string TACAN_CTRL_PANEL = "74";
        // ReSharper restore InconsistentNaming
        #endregion

        #region Buttons
        private const string BUTTON_1 = "3001";
        private const string BUTTON_2 = "3002";
        private const string BUTTON_3 = "3003";
        private const string BUTTON_4 = "3004";
        private const string BUTTON_5 = "3005";
        private const string BUTTON_6 = "3006";
        private const string BUTTON_7 = "3007";
        private const string BUTTON_8 = "3008";
        private const string BUTTON_9 = "3009";
        private const string BUTTON_10 = "3010";
        private const string BUTTON_11 = "3011";
        private const string BUTTON_12 = "3012";
        private const string BUTTON_13 = "3013";
        private const string BUTTON_14 = "3014";
        private const string BUTTON_15 = "3015";
        private const string BUTTON_16 = "3016";
        private const string BUTTON_17 = "3017";
        private const string BUTTON_18 = "3018";
        private const string BUTTON_19 = "3019";
        private const string BUTTON_20 = "3020";
        private const string BUTTON_21 = "3021";
        private const string BUTTON_22 = "3022";
        private const string BUTTON_23 = "3023";
        private const string BUTTON_24 = "3024";
        private const string BUTTON_25 = "3025";
        private const string BUTTON_26 = "3026";
        private const string BUTTON_27 = "3027";
        private const string BUTTON_28 = "3028";
        private const string BUTTON_29 = "3029";
        private const string BUTTON_30 = "3030";
        private const string BUTTON_31 = "3031";
        private const string BUTTON_32 = "3032";
        private const string BUTTON_33 = "3033";
        private const string BUTTON_34 = "3034";
        private const string BUTTON_35 = "3035";
        private const string BUTTON_36 = "3036";
        private const string BUTTON_37 = "3037";
        private const string BUTTON_38 = "3038";
        private const string BUTTON_39 = "3039";
        private const string BUTTON_40 = "3040";
        private const string BUTTON_41 = "3041";
        private const string BUTTON_42 = "3042";
        private const string BUTTON_43 = "3043";
        private const string BUTTON_44 = "3044";
        private const string BUTTON_45 = "3045";
        private const string BUTTON_46 = "3046";
        private const string BUTTON_47 = "3047";
        private const string BUTTON_48 = "3048";
        private const string BUTTON_49 = "3049";
        private const string BUTTON_50 = "3050";
        private const string BUTTON_51 = "3051";
        private const string BUTTON_52 = "3052";
        private const string BUTTON_53 = "3053";
        private const string BUTTON_54 = "3054";
        private const string BUTTON_55 = "3055";
        private const string BUTTON_56 = "3056";
        private const string BUTTON_57 = "3057";
        private const string BUTTON_58 = "3058";
        private const string BUTTON_59 = "3059";
        private const string BUTTON_60 = "3060";
        private const string BUTTON_61 = "3061";
        private const string BUTTON_62 = "3062";
        private const string BUTTON_63 = "3063";
        private const string BUTTON_64 = "3064";
        private const string BUTTON_65 = "3065";
        private const string BUTTON_66 = "3066";
        private const string BUTTON_67 = "3067";
        #endregion

        protected A10CInterface(string heliosName, string dcsVehicleName, string exportFunctionsUri)
            : base(heliosName, dcsVehicleName, exportFunctionsUri)
        {
            #region Indexers
            AddFunction(new FlagValue(this, "540", "AOA Indexer", "High Indicator", "High AOA indicator light."));
            AddFunction(new FlagValue(this, "541", "AOA Indexer", "Normal Indicator", "Norm AOA indidcator light."));
            AddFunction(new FlagValue(this, "542", "AOA Indexer", "Low Indicator", "Low AOA indicator light."));
            AddFunction(new FlagValue(this, "730", "Refuel Indexer", "Ready Indicator", "Refuel ready indicator light."));
            AddFunction(new FlagValue(this, "731", "Refuel Indexer", "Latched Indicator", "Refuel latched indicator light."));
            AddFunction(new FlagValue(this, "732", "Refuel Indexer", "Disconnect Indicator", "Refuel disconnect indicator light."));
			#endregion

			#region DVADR
			AddFunction(Switch.CreateThreeWaySwitch(this, DVADR, BUTTON_1, "789", "0.0", "Off", "0.1", "Stby", "0.2", "Rec", "DVADR", "Function control toggle switch", "%0.1f"));
			AddFunction(Switch.CreateThreeWaySwitch(this, DVADR, BUTTON_2, "790", "0.0", "Hud", "0.1", "Auto", "0.2", "Tvm", "DVADR", "Video selector toggle switch", "%0.1f"));
			AddFunction(new FlagValue(this, "791", "DVADR", "EOT Lamp", ""));
			AddFunction(new FlagValue(this, "792", "DVADR", "REC ON Lamp", ""));
			#endregion

			#region Leftpanelback
			AddFunction(Switch.CreateToggleSwitch(this, IFFCC, BUTTON_2, "709", "1", "open", "0", "Closed", "Misc", "Arm Ground Safety Override Cover", "%1d"));
			AddFunction(Switch.CreateToggleSwitch(this, IFFCC, BUTTON_3, "710", "1", "Oride", "0", "Safe", "Misc", "Arm Ground Safety Override Switch", "%1d"));
			AddFunction(Switch.CreateThreeWaySwitch(this, IFF, BUTTON_19, "706", "1.0", "Upper", "0.5", "Both", "0.0", "Lower", "Misc", "IFF antenna switch", "%0.1f"));
			AddFunction(Switch.CreateThreeWaySwitch(this, UHF_RADIO, BUTTON_16, "707", "1.0", "Upper", "0.5", "Both", "0.0", "Lower", "Misc", "UHF antenna switch", "%0.1f"));
			AddFunction(Switch.CreateToggleSwitch(this, UHF_RADIO, BUTTON_17, "708", "1", "Single", "0", "Dissable", "Misc", "EGI HQ TOD", "%1d"));
			#endregion

			#region Engine Gauges

            CalibrationPointCollectionDouble engineFanSpeedScale =
                new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 100d)
                {
                    new CalibrationPointDouble(0.125d, 25d),
                    new CalibrationPointDouble(0.250d, 50d),
                    new CalibrationPointDouble(0.375d, 75d),
                    new CalibrationPointDouble(0.500d, 80d),
                    new CalibrationPointDouble(0.625d, 85d),
                    new CalibrationPointDouble(0.750d, 90d),
                    new CalibrationPointDouble(0.875d, 95d)
                };
            AddFunction(new DualNetworkValue(this, "76", engineFanSpeedScale, "Left Engine", "Fan Speed", "Fan speed of engine.", "0-100", BindingValueUnits.RPMPercent));
            AddFunction(new DualNetworkValue(this, "77", engineFanSpeedScale, "Right Engine", "Fan Speed", "Fan speed of engine.", "0-100", BindingValueUnits.RPMPercent));
            AddFunction(new ScaledNetworkValue(this, "78", 100d, "Left Engine", "Core Speed", "Engine core speed as a percent of compressor RPM.", "0-100", BindingValueUnits.RPMPercent));
            AddFunction(new ScaledNetworkValue(this, "80", 100d, "Right Engine", "Core Speed", "Engine core speed as a percent of compressor RPM", "0-100", BindingValueUnits.RPMPercent));
            AddFunction(new ScaledNetworkValue(this, "84", 5000d, "Left Engine", "Fuel Flow", "Fuel flow to the engine", "", BindingValueUnits.PoundsPerHour));
            AddFunction(new ScaledNetworkValue(this, "85", 5000d, "Right Engine", "Fuel Flow", "Fuel flow to the engine", "", BindingValueUnits.PoundsPerHour));
            CalibrationPointCollectionDouble engineTempScale =
                new CalibrationPointCollectionDouble(0.0d, 0.0d, 0.995d, 1100d)
                {
                    new CalibrationPointDouble(0.005d, 100d),
                    new CalibrationPointDouble(0.095d, 200d),
                    new CalibrationPointDouble(0.183d, 300d),
                    new CalibrationPointDouble(0.275d, 400d),
                    new CalibrationPointDouble(0.365d, 500d),
                    new CalibrationPointDouble(0.463d, 600d),
                    new CalibrationPointDouble(0.560d, 700d),
                    new CalibrationPointDouble(0.657d, 800d),
                    new CalibrationPointDouble(0.759d, 900d),
                    new CalibrationPointDouble(0.855d, 1000d)
                };
            AddFunction(new DualNetworkValue(this, "70", engineTempScale, "Left Engine", "Interstage Turbine Temperature", "", "", BindingValueUnits.Celsius));
            AddFunction(new DualNetworkValue(this, "73", engineTempScale, "Right Engine", "Interstage Turbine Temperature", "", "", BindingValueUnits.Celsius));
			AddFunction(new NetworkValue(this, "71", "Left Engine", "Interstage Turbine Temp small needle", "Position of the needle.", "(0 to 1)", BindingValueUnits.Numeric));
			AddFunction(new FlagValue(this, "72", "Left Engine", "Left eng temp Off Flag", "Indicator is Off"));
			AddFunction(new NetworkValue(this, "74", "Right Engine", "Interstage Turbine Temp small needle", "Position of the needle.", "(0 to 1)", BindingValueUnits.Numeric));
			AddFunction(new FlagValue(this, "75", "Right Engine", "Right eng temp Off Flag", "Indicator is Off"));
			AddFunction(new ScaledNetworkValue(this, "82", 100d, "Left Engine", "Oil Pressure", "Oil pressure in engine", "", BindingValueUnits.PoundsPerSquareInch));
            AddFunction(new ScaledNetworkValue(this, "83", 100d, "Right Engine", "Oil Pressure", "Oil pressure in engine", "", BindingValueUnits.PoundsPerSquareInch));
            AddFunction(new ScaledNetworkValue(this, "13", 120d, "APU", "RPM", "Current percentage of maximum RPM for the APU.", "", BindingValueUnits.RPMPercent));
            AddFunction(new ScaledNetworkValue(this, "14", 1000d, "APU", "Exhaust Gas Temperature", "Current temperature of the APU exhaust gas.", "", BindingValueUnits.Celsius));
            #endregion

            #region Flight Gauges
            AddFunction(new Altimeter(this));
            AddFunction(new RotaryEncoder(this, AAU34, BUTTON_1, "62", 0.04d, "Altimeter", "Pressure"));
			AddFunction(Switch.CreateThreeWaySwitch(this, AAU34, BUTTON_2, "60", "1.0", "PNEU", "0.0", "NONE", "-1.0", "ELECT", "Altimeter", "ELECT/PNEU switch", "%0.1f"));

			AddFunction(new PushButton(this, ACCELEROMETER, BUTTON_1, "904", "Accelerometer", "Push to set"));
			CalibrationPointCollectionDouble accelerometerScale = new CalibrationPointCollectionDouble(0.0d, -5.0d, 1.0d, 10d);
			AddFunction(new DualNetworkValue(this, "15", accelerometerScale, "Accelerometer", "Accelerometer", "Current gs", "", BindingValueUnits.Numeric));
			AddFunction(new DualNetworkValue(this, "902", accelerometerScale, "Accelerometer", "Accelerometer Min", "Min Gs attained.", "", BindingValueUnits.Numeric));
			AddFunction(new DualNetworkValue(this, "903", accelerometerScale, "Accelerometer", "Accelerometer Max", "Max Gs attained.", "", BindingValueUnits.Numeric));

			AddFunction(new Axis(this, AN_ALR69V, BUTTON_1, "16", 0.05d, 0.15d, 0.85d, "Misc", "RWR Adjust Display Brightness"));

            CalibrationPointCollectionDouble airspeedScale =
                new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 550d)
                {
                    // non linear section at start
                    new CalibrationPointDouble(0.0488, 50d),
                    new CalibrationPointDouble(0.14d, 100d)
                };
            AddFunction(new DualNetworkValue(this, "48", airspeedScale, "IAS", "Airspeed", "Current indicated air speed of the aircraft.", "", BindingValueUnits.Knots));
			AddFunction(new DualNetworkValue(this, "50", airspeedScale, "IAS", "Max Airspeed", "Current altitude compensated, limiting structural airspeed of the aircraft.", "(0 to 550)", BindingValueUnits.Knots));

            CalibrationPointCollectionDouble vviScale = new CalibrationPointCollectionDouble(-1.0d, -6000d, 1.0d, 6000d)
            {
                new CalibrationPointDouble(-0.5d, -2000d),
                new CalibrationPointDouble(-0.29d, -1000d),
                new CalibrationPointDouble(0.29d, 1000d),
                new CalibrationPointDouble(0.5d, 2000d)
            };

            AddFunction(new DualNetworkValue(this, "12", vviScale, "VVI", "Vertical Velocity", "Current vertical velocity of the aircraft.", "", BindingValueUnits.FeetPerMinute));

			AddFunction(new ScaledNetworkValue(this, "4", 30d, "AOA", "Angle of Attack", "Current angle of attack of the aircraft.", "", BindingValueUnits.Degrees));
            AddFunction(new FlagValue(this, "55", "AOA", "Off Flag", ""));

            AddFunction(new ScaledNetworkValue(this, "17", -90d, "ADI", "Pitch", "Current pitch displayed on the ADI.", "", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "18", 180d, "ADI", "Bank", "Current bank displayed on the ADI.", "", BindingValueUnits.Degrees));
            AddFunction(new NetworkValue(this, "24", "ADI", "Slip Ball", "Current position of the slip ball relative to the center of the tube.", "(-1 to 1) -1 is full left and 1 is full right.", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "23", "ADI", "Turn Needle", "Position of the turn needle.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new FlagValue(this, "25", "ADI", "Attitude Warning Flag", "Indicates that the ADI has lost electrical power or otherwise been disabled."));
            AddFunction(new FlagValue(this, "19", "ADI", "Course Warning Flag", "Indicates thatn an operative ILS or TACAN signal is received."));
            AddFunction(new FlagValue(this, "26", "ADI", "Glide Slope Warning Flag", "Indicates that the ADI is not recieving a ILS glide slope signal."));
            AddFunction(new NetworkValue(this, "20", "ADI", "Bank Steering Bar Offset", "Location of bank steering bar relative to the middle of the ADI.", "(-1 to 1) -1 is full left and 1 is full right.", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "21", "ADI", "Pitch Steering Bar Offset", "Location of pitch steering bar relative to the middle of the ADI.", "(-1 to 1) 1 is full up and -1 is full down.", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "27", "ADI", "Glide Slope Indicator", "Location of the glide slope indicator relative to the middle of the slope deviation scale.", "(-1 to 1) 1 is full up and -1 is full down.", BindingValueUnits.Numeric));
            AddFunction(new Axis(this, ADI, BUTTON_1, "22", 0.05d, -0.5d, 0.5d, "ADI", "Pitch Trim Knob"));

            AddFunction(new ScaledNetworkValue(this, "63", 90d, "SAI", "Pitch", "Current pitch displayed on the SAI.", "", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "64", -180d, "SAI", "Bank", "Current bank displayed on the SAI.", "", BindingValueUnits.Degrees));
            AddFunction(new FlagValue(this, "65", "SAI", "Warning Flag", "Displayed when SAI is caged or non-functional."));
            AddFunction(new RotaryEncoder(this, SAI, BUTTON_3, "66", 0.1d, "SAI", "Pitch Trim / Cage"));
            AddFunction(new NetworkValue(this, "715", "SAI", "Pitch Adjust", "Current pitch adjustment setting", "-1 to 1", BindingValueUnits.Numeric));
			AddFunction(new NetworkValue(this, "717", "SAI", "Pitch Adjust neddle", "Current pitch adjustment needle position ", "-1 to 1", BindingValueUnits.Numeric));

			AddFunction(new FlagValue(this, "40", "HSI", "Power Off Flag", "This flag is on when the HSI gaue has no power."));
            AddFunction(new FlagValue(this, "32", "HSI", "Range Flag", "This flag indicates that the range to steer point or TACAN station is not available."));
            AddFunction(new FlagValue(this, "46", "HSI", "Bearing Flag", "This flag is displayed if the aircraft is significantly off course."));
            AddFunction(new FlagValue(this, "42", "HSI", "From Flag", "This flag is displayed when heading away from VOR."));
            AddFunction(new FlagValue(this, "43", "HSI", "To Flag", "This flag is displayed when heading towards VOR."));
            AddFunction(new HSIMiles(this));
            AddFunction(new ScaledNetworkValue(this, "34", -360d, "HSI", "Heading", "Current heading displayed on the HSI", "", BindingValueUnits.Degrees, 360d, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "36", 360d, "HSI", "Desired Heading", "Direction of desired heading needle relative to current heading.", "Rotation realtive to current heading towards the desired heading.", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "47", 360d, "HSI", "Desired Course", "Direction of desired course relative to the current heading.", "Rotation realtive to current heading towards the desired course.", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "41", -1d, "HSI", "Course Deviation", "Current offset of the deviation needle.", "(-1 to 1) -1 is full left and 1 is full right.", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "33", 360d, "HSI", "Bearing 1", "Direction of currently selected TACAN station or the selected UHF station relative to current heading.", "When both TACAN and ADF are selected ADF UHF station will take priority.  If neither mod is selected will align to current steer point.  Direction is realative to current heading.", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "35", 360d, "HSI", "Bearing 2", "Direction of currently selected steer point relative to current heading.", "Direction is realtive to current heading.", BindingValueUnits.Degrees));
            AddFunction(new RotaryEncoder(this, HSI, BUTTON_1, "45", 0.05d, "HSI", "Heading Set Knob"));
            AddFunction(new RotaryEncoder(this, HSI, BUTTON_2, "44", 0.05d, "HSI", "Course Set Knob"));
            #endregion

            #region Digital Clock
            AddFunction(new PushButton(this, DIGITAL_CLOCK, BUTTON_1, "68", "Digital Clock", "Toggle Clock and Elapsed Time Modes"));
            AddFunction(new PushButton(this, DIGITAL_CLOCK, BUTTON_2, "69", "Digital Clock", "Start, Stop and Reset Elapsed Timer"));
            AddFunction(new Text(this, "2409", "Digital Clock", "Time Hours:Mins", "Text Value of Hours and Minutes"));
            AddFunction(new Text(this, "2410", "Digital Clock", "Time Seconds", "Text Value of Seconds"));
            AddFunction(new Text(this, "2408", "Digital Clock", "Clock Function Type", "Text for Clock or Elapsed Time"));
            #endregion

            #region Front Panel Indicators
            AddFunction(new PushButton(this, IFFCC, BUTTON_1, "101", "IFFCC", "Ext Stores Jettison"));
            AddFunction(new FlagValue(this, "662", "Misc", "Gun Ready Indicator", "Indicator is lit when the GAU-8 cannon is armed and ready to fire."));
            AddFunction(new FlagValue(this, "663", "Misc", "Nose Wheel Steering Indicator", "Indicator is lit when nose wheel steering is engaged."));
            AddFunction(new FlagValue(this, "665", "Misc", "Canopy Unlocked Indicator", "Indicator is lit when canopy is open."));
            AddFunction(new FlagValue(this, "664", "Misc", "Marker Beacon Indicator", "Indicator is lit when in ILS mode and a beacon is overflown."));
            AddFunction(new FlagValue(this, "215", "Fire System", "Left Engine Fire Indicator", "Indicator lights when a fire is detected in the left engine."));
            AddFunction(new FlagValue(this, "216", "Fire System", "APU Fire Indicator", "Indicator lights when a fire is detedted in the APU."));
            AddFunction(new FlagValue(this, "217", "Fire System", "Right Engine Fire Indicator", "Indicator lights when a fire is detected in the right engine."));
            AddFunction(Switch.CreateToggleSwitch(this, FIRE_SYSTEM, BUTTON_1, "102", "0", "Normal", "1", "Pulled", "Fire System", "Left Engine Fire Pull", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FIRE_SYSTEM, BUTTON_2, "103", "0", "Normal", "1", "Pulled", "Fire System", "APU Fire Pull", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FIRE_SYSTEM, BUTTON_3, "104", "0", "Normal", "1", "Pulled", "Fire System", "Right Engine Fire Pull", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, FIRE_SYSTEM, BUTTON_4, "105", "-1", "Discharge Left Bottle", "0", "Off", "1", "Discharge Right Bottle", "Fire System", "Discharge Switch", "%1d"));
		
			#endregion

			#region Left MCFD
			AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_1, "300", "Left MFCD", "OSB1"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_2, "301", "Left MFCD", "OSB2"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_3, "302", "Left MFCD", "OSB3"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_4, "303", "Left MFCD", "OSB4"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_5, "304", "Left MFCD", "OSB5"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_6, "305", "Left MFCD", "OSB6"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_7, "306", "Left MFCD", "OSB7"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_8, "307", "Left MFCD", "OSB8"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_9, "308", "Left MFCD", "OSB9"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_10, "309", "Left MFCD", "OSB10"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_11, "310", "Left MFCD", "OSB11"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_12, "311", "Left MFCD", "OSB12"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_13, "312", "Left MFCD", "OSB13"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_14, "313", "Left MFCD", "OSB14"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_15, "314", "Left MFCD", "OSB15"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_16, "315", "Left MFCD", "OSB16"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_17, "316", "Left MFCD", "OSB17"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_18, "317", "Left MFCD", "OSB18"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_19, "318", "Left MFCD", "OSB19"));
            AddFunction(new PushButton(this, MFCD_LEFT, BUTTON_20, "319", "Left MFCD", "OSB20"));
            AddFunction(new Rocker(this, MFCD_LEFT, BUTTON_21, BUTTON_22, BUTTON_23, BUTTON_23, "320", "Left MFCD", "Moving Map Scale", true));
            AddFunction(new Rocker(this, MFCD_LEFT, BUTTON_24, BUTTON_25, BUTTON_26, BUTTON_26, "321", "Left MFCD", "Backlight", true));
            AddFunction(new Rocker(this, MFCD_LEFT, BUTTON_27, BUTTON_28, BUTTON_29, BUTTON_29, "322", "Left MFCD", "Brightness", true));
            AddFunction(new Rocker(this, MFCD_LEFT, BUTTON_30, BUTTON_31, BUTTON_32, BUTTON_32, "323", "Left MFCD", "Contrast", true));
            AddFunction(new Rocker(this, MFCD_LEFT, BUTTON_33, BUTTON_34, BUTTON_35, BUTTON_35, "324", "Left MFCD", "Entity Level", false));
            AddFunction(Switch.CreateThreeWaySwitch(this, MFCD_LEFT, BUTTON_36, "325", "0.2", "Day", "0.1", "Night", "0.0", "Off", "Left MFCD", "Day/Night/Off", "%0.1f"));
            #endregion

            #region Right MCFD
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_1, "326", "Right MFCD", "OSB1"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_2, "327", "Right MFCD", "OSB2"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_3, "328", "Right MFCD", "OSB3"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_4, "329", "Right MFCD", "OSB4"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_5, "330", "Right MFCD", "OSB5"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_6, "331", "Right MFCD", "OSB6"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_7, "332", "Right MFCD", "OSB7"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_8, "333", "Right MFCD", "OSB8"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_9, "334", "Right MFCD", "OSB9"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_10, "335", "Right MFCD", "OSB10"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_11, "336", "Right MFCD", "OSB11"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_12, "337", "Right MFCD", "OSB12"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_13, "338", "Right MFCD", "OSB13"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_14, "339", "Right MFCD", "OSB14"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_15, "340", "Right MFCD", "OSB15"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_16, "341", "Right MFCD", "OSB16"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_17, "342", "Right MFCD", "OSB17"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_18, "343", "Right MFCD", "OSB18"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_19, "344", "Right MFCD", "OSB19"));
            AddFunction(new PushButton(this, MFCD_RIGHT, BUTTON_20, "345", "Right MFCD", "OSB20"));
            AddFunction(new Rocker(this, MFCD_RIGHT, BUTTON_21, BUTTON_22, BUTTON_23, BUTTON_23, "346", "Right MFCD", "Moving Map Scale", true));
            AddFunction(new Rocker(this, MFCD_RIGHT, BUTTON_24, BUTTON_25, BUTTON_26, BUTTON_26, "347", "Right MFCD", "Backlight", true));
            AddFunction(new Rocker(this, MFCD_RIGHT, BUTTON_27, BUTTON_28, BUTTON_29, BUTTON_29, "348", "Right MFCD", "Brightness", true));
            AddFunction(new Rocker(this, MFCD_RIGHT, BUTTON_30, BUTTON_31, BUTTON_32, BUTTON_32, "349", "Right MFCD", "Contrast", true));
            AddFunction(new Rocker(this, MFCD_RIGHT, BUTTON_33, BUTTON_34, BUTTON_35, BUTTON_35, "350", "Right MFCD", "Entity Level", false));
            AddFunction(Switch.CreateThreeWaySwitch(this, MFCD_RIGHT, BUTTON_36, "351", "0.2", "Day", "0.1", "Night", "0.0", "Off", "Right MFCD", "Day/Night/Off", "%0.1f"));
            #endregion

            #region UFC
            AddFunction(new PushButton(this, UFC, BUTTON_1, "385", "UFC", "1"));
            AddFunction(new PushButton(this, UFC, BUTTON_2, "386", "UFC", "2"));
            AddFunction(new PushButton(this, UFC, BUTTON_3, "387", "UFC", "3"));
            AddFunction(new PushButton(this, UFC, BUTTON_4, "388", "UFC", "4"));
            AddFunction(new PushButton(this, UFC, BUTTON_5, "389", "UFC", "5"));
            AddFunction(new PushButton(this, UFC, BUTTON_6, "390", "UFC", "6"));
            AddFunction(new PushButton(this, UFC, BUTTON_7, "391", "UFC", "7"));
            AddFunction(new PushButton(this, UFC, BUTTON_8, "392", "UFC", "8"));
            AddFunction(new PushButton(this, UFC, BUTTON_9, "393", "UFC", "9"));
            AddFunction(new PushButton(this, UFC, BUTTON_10, "395", "UFC", "0"));
            AddFunction(new PushButton(this, UFC, BUTTON_11, "396", "UFC", "Space"));
            AddFunction(new PushButton(this, UFC, BUTTON_12, "394", "UFC", "Display Hack Time"));
            AddFunction(new PushButton(this, UFC, BUTTON_13, "397", "UFC", "Select function Mode"));
            AddFunction(new PushButton(this, UFC, BUTTON_14, "398", "UFC", "Select Letter Mode"));
            AddFunction(new PushButton(this, UFC, BUTTON_15, "399", "UFC", "Clear"));
            AddFunction(new PushButton(this, UFC, BUTTON_16, "400", "UFC", "Enter"));
            AddFunction(new PushButton(this, UFC, BUTTON_17, "401", "UFC", "Create Overhead Mark Point"));
            AddFunction(new PushButton(this, UFC, BUTTON_18, "402", "UFC", "Display and Adjust Altitude Alert Values"));
            AddFunction(new Rocker(this, UFC, BUTTON_20, BUTTON_21, BUTTON_20, BUTTON_21, "405", "UFC", "Steer", true));
            AddFunction(new Rocker(this, UFC, BUTTON_22, BUTTON_23, BUTTON_22, BUTTON_23, "406", "UFC", "Data", true));
            AddFunction(new Rocker(this, UFC, BUTTON_24, BUTTON_25, BUTTON_24, BUTTON_25, "407", "UFC", "Select", true));
            AddFunction(new Rocker(this, UFC, BUTTON_26, BUTTON_27, BUTTON_26, BUTTON_27, "408", "UFC", "Adjust Depressible Pipper", true));
            AddFunction(new Rocker(this, UFC, BUTTON_28, BUTTON_29, BUTTON_28, BUTTON_29, "409", "UFC", "HUD Brightness", false));
            AddFunction(new PushButton(this, UFC, BUTTON_30, "531", "UFC", "FWD"));
            AddFunction(new PushButton(this, UFC, BUTTON_31, "532", "UFC", "MID"));
            AddFunction(new PushButton(this, UFC, BUTTON_32, "533", "UFC", "AFT"));
            AddFunction(new PushButton(this, SYS_CONTROLLER, BUTTON_1, "403", "UFC", "Master Caution"));
            AddFunction(new FlagValue(this, "404", "UFC", "Master Caution Indicator", "Indicator lamp on master caution button.")); 
            #endregion

            #region CMSC
            AddFunction(new PushButton(this, CMSC, BUTTON_1, "365", "CMSC", "Cycle JMR Program Button"));
            AddFunction(new PushButton(this, CMSC, BUTTON_2, "366", "CMSC", "Cycle MWS Program Button"));
            AddFunction(new PushButton(this, CMSC, BUTTON_3, "369", "CMSC", "Priority Button"));
            AddFunction(new PushButton(this, CMSC, BUTTON_4, "370", "CMSC", "Separate Button"));
            AddFunction(new PushButton(this, CMSC, BUTTON_5, "371", "CMSC", "Unknown Button"));
            AddFunction(new Axis(this, CMSC, BUTTON_6, "367", 0.05d, 0.15d, 0.85d, "CMSC", "Brightness"));
            AddFunction(new Axis(this, CMSC, BUTTON_7, "368", 0.1d, 0d, 1d, "CMSC", "RWR Volume"));
            AddFunction(new FlagValue(this, "372", "CMSC", "Missle Launch Indicator", "Flashes when missile has been launched near your aircraft."));
            AddFunction(new FlagValue(this, "373", "CMSC", "Priority Status Indicator", "Lit when priority display mode is active."));
            AddFunction(new FlagValue(this, "374", "CMSC", "Unknown Status Indicator", "Lit when unknown threat display is active."));
            AddFunction(new Text(this, "2405", "CMSC", "MWS Display", "Text Value MWS Display"));
            AddFunction(new Text(this, "2406", "CMSC", "Chaff Flare Display", "Text Value for Chaff/Flare Display"));
            AddFunction(new Text(this, "2407", "CMSC", "JMR Display", "Text Value JMR Display"));

            #endregion

            #region Landing Gear Panel
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_1, "716", "0", "Up", "1", "Down", "Mechanical", "Landing Gear Lever", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, LIGHT_SYSTEM, BUTTON_14, "655", "0.2", "Land", "0.1", "Off", "0.0", "Taxi", "Light System", "Land/Taxi Lights", "%0.1f"));
            AddFunction(new PushButton(this, CPT_MECH, BUTTON_3, "651", "Mechanical", "Gear Downlock Override"));
            AddFunction(new Switch(this, AUTOPILOT, "654", new SwitchPosition[] { new SwitchPosition("1", "On", BUTTON_28, BUTTON_29, "0"), new SwitchPosition("0", "Off", BUTTON_28, BUTTON_29, "0") }, "Autopilot", "Anti-Skid", "%1d", true));
            AddFunction(new FlagValue(this, "659", "Mechanical", "Gear Nose Safe Indicator", "Lit when the nose gear is down and locked."));
            AddFunction(new FlagValue(this, "660", "Mechanical", "Gear Left Safe Indicator", "Lit when the left gear is down and locked."));
            AddFunction(new FlagValue(this, "661", "Mechanical", "Gear Right Safe Indicator", "Lit when the right gear is down and locked."));
            AddFunction(new FlagValue(this, "737", "Mechanical", "Gear Handle Indicator", "Lit when the landing gear are moving between down and stowed position."));
            AddFunction(new ScaledNetworkValue(this, "653", 30d, "Misc", "Flaps Position", "Current flaps position.", "", BindingValueUnits.Degrees));
            #endregion

            #region AHCP
            AddFunction(Switch.CreateThreeWaySwitch(this, AHCP, BUTTON_1, "375", "0.2", "Arm", "0.1", "Safe", "0.0", "Train", "AHCP", "Master Arm", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, AHCP, BUTTON_2, "376", "0.2", "Gun/Pac Arm", "0.1", "Safe", "0.0", "Gun Arm", "AHCP", "Gun Arm", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, AHCP, BUTTON_3, "377", "0.2", "Arm", "0.1", "Safe", "0.0", "Train", "AHCP", "Laser Arm", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, AHCP, BUTTON_4, "378", "1", "On", "0", "Off", "AHCP", "TGP Power", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, AHCP, BUTTON_5, "379", "0.2", "Barometric", "0.1", "Delta", "0.0", "Radar", "AHCP", "Altimeter Source", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, AHCP, BUTTON_6, "380", "1", "Day", "0", "Night", "AHCP", "HUD Day/Night", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, AHCP, BUTTON_7, "381", "1", "Norm", "0", "Standby", "AHCP", "HUD Norm/Standbyh", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, AHCP, BUTTON_8, "382", "1", "On", "0", "Off", "AHCP", "CICU Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, AHCP, BUTTON_9, "383", "1", "On", "0", "Off", "AHCP", "Datalink Power", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, AHCP, BUTTON_10, "384", "0.2", "On", "0.1", "Test", "0.0", "Off", "AHCP", "IFFCC Power", "%0.1f"));
            #endregion

            #region Fuel Panel
            AddFunction(new ScaledNetworkValue(this, "88", 6000d, "Fuel Gauge", "Left Quantity", "Quantity displayed on the left needle", "", BindingValueUnits.Pounds));
            AddFunction(new ScaledNetworkValue(this, "89", 6000d, "Fuel Gauge", "Right Quantity", "Quantity displayed on the right needle", "", BindingValueUnits.Pounds));
            AddFunction(new TotalFuel(this));
            AddFunction(new Switch(this, FUEL_SYSTEM, "645", new SwitchPosition[] { new SwitchPosition("0.0", "Int", BUTTON_17), new SwitchPosition("0.1", "Main", BUTTON_17), new SwitchPosition("0.2", "Wing", BUTTON_17), new SwitchPosition("0.3", "Ext Wing", BUTTON_17), new SwitchPosition("0.4", "Ext Ctr", BUTTON_17) }, "Fuel System", "Fuel Display Selector", "%0.1f"));
            AddFunction(new PushButton(this, FUEL_SYSTEM, BUTTON_18, "646", "Fuel System", "Test Indicator"));
            AddFunction(new ScaledNetworkValue(this, "647", 4000d, "Hydraulics", "left pressure", "Current pressure in the left hydraulics system", "(0-4000)", BindingValueUnits.PoundsPerSquareInch));
            AddFunction(new ScaledNetworkValue(this, "648", 4000d, "Hydraulics", "right pressure", "Current pressure in the right hydraulics system", "(0-4000)", BindingValueUnits.PoundsPerSquareInch));
            #endregion

            #region Navigation Mode Panel
            AddFunction(new PushButton(this, NMSP, BUTTON_1, "605", "Navigation Mode Select Panel", "HARS"));
            AddFunction(new FlagValue(this, "606", "Navigation Mode Select Panel", "HARS Indicator", "HARS button indicator lamp."));
            AddFunction(new PushButton(this, NMSP, BUTTON_2, "607", "Navigation Mode Select Panel", "EGI"));
            AddFunction(new FlagValue(this, "608", "Navigation Mode Select Panel", "EGI Indicator", "EGI button indicator lamp."));
            AddFunction(new PushButton(this, NMSP, BUTTON_3, "609", "Navigation Mode Select Panel", "TISL"));
            AddFunction(new FlagValue(this, "610", "Navigation Mode Select Panel", "TISL Indicator", "TISL button indicator lamp."));
            AddFunction(new PushButton(this, NMSP, BUTTON_4, "611", "Navigation Mode Select Panel", "STEERPT"));
            AddFunction(new FlagValue(this, "612", "Navigation Mode Select Panel", "STEERPT Indicator", "STEERPT button indicator lamp."));
            AddFunction(new PushButton(this, NMSP, BUTTON_5, "613", "Navigation Mode Select Panel", "ANCHR"));
            AddFunction(new FlagValue(this, "614", "Navigation Mode Select Panel", "ANCHR Indicator", "ANCHR button indicator lamp."));
            AddFunction(new PushButton(this, NMSP, BUTTON_6, "615", "Navigation Mode Select Panel", "TCN"));
            AddFunction(new FlagValue(this, "616", "Navigation Mode Select Panel", "TCN Indicator", "TCN button indicator lamp."));
            AddFunction(new PushButton(this, NMSP, BUTTON_7, "617", "Navigation Mode Select Panel", "ILS"));
            AddFunction(new FlagValue(this, "618", "Navigation Mode Select Panel", "ILS Indicator", "ILS button indicator lamp."));
            AddFunction(new FlagValue(this, "619", "Navigation Mode Select Panel", "UHF Homing Indicator", "Lit when the UHF control panel is ste to ADF."));
            AddFunction(new FlagValue(this, "620", "Navigation Mode Select Panel", "VHF/FM Homing Indicator", "Lit when the VHF/FM control panel is set to homing mode."));
            AddFunction(Switch.CreateToggleSwitch(this, NMSP, BUTTON_8, "621", "0", "Able", "1", "Stow", "Navigation Mode Select Panel", "Able - Stow", "%1d"));
            #endregion

            // HARS Fast Erect Panel
            AddFunction(new PushButton(this, HARS, BUTTON_1, "711", "HARS", "Fast Erect"));

            // UHF Repeater
            AddFunction(new NetworkValue(this, "2000", "UHF Radio", "Fequency", "Currently tuned frequency of UHF radio.", "", BindingValueUnits.Text, null));

			#region CDU
			AddFunction(new PushButton(this, CDU, BUTTON_1, "410", "CDU", "LSK 3L"));
            AddFunction(new PushButton(this, CDU, BUTTON_2, "411", "CDU", "LSK 5L"));
            AddFunction(new PushButton(this, CDU, BUTTON_3, "412", "CDU", "LSK 7L"));
            AddFunction(new PushButton(this, CDU, BUTTON_4, "413", "CDU", "LSK 9L"));
            AddFunction(new PushButton(this, CDU, BUTTON_5, "414", "CDU", "LSK 3R"));
            AddFunction(new PushButton(this, CDU, BUTTON_6, "415", "CDU", "LSK 5R"));
            AddFunction(new PushButton(this, CDU, BUTTON_7, "416", "CDU", "LSK 7R"));
            AddFunction(new PushButton(this, CDU, BUTTON_8, "417", "CDU", "LSK 9R"));
            AddFunction(new PushButton(this, CDU, BUTTON_9, "418", "CDU", "SYS"));
            AddFunction(new PushButton(this, CDU, BUTTON_10, "419", "CDU", "NAV"));
            AddFunction(new PushButton(this, CDU, BUTTON_11, "420", "CDU", "WP MENU"));
            AddFunction(new PushButton(this, CDU, BUTTON_12, "421", "CDU", "OFFSET"));
            AddFunction(new PushButton(this, CDU, BUTTON_13, "422", "CDU", "FPMENU"));
            AddFunction(new PushButton(this, CDU, BUTTON_14, "423", "CDU", "PREV"));

            AddFunction(new PushButton(this, CDU, BUTTON_15, "425", "CDU", "1"));
            AddFunction(new PushButton(this, CDU, BUTTON_16, "426", "CDU", "2"));
            AddFunction(new PushButton(this, CDU, BUTTON_17, "427", "CDU", "3"));
            AddFunction(new PushButton(this, CDU, BUTTON_18, "428", "CDU", "4"));
            AddFunction(new PushButton(this, CDU, BUTTON_19, "429", "CDU", "5"));
            AddFunction(new PushButton(this, CDU, BUTTON_20, "430", "CDU", "6"));
            AddFunction(new PushButton(this, CDU, BUTTON_21, "431", "CDU", "7"));
            AddFunction(new PushButton(this, CDU, BUTTON_22, "432", "CDU", "8"));
            AddFunction(new PushButton(this, CDU, BUTTON_23, "433", "CDU", "9"));
            AddFunction(new PushButton(this, CDU, BUTTON_24, "434", "CDU", "0"));
            AddFunction(new PushButton(this, CDU, BUTTON_25, "435", "CDU", "Point"));
            AddFunction(new PushButton(this, CDU, BUTTON_26, "436", "CDU", "Slash"));
            AddFunction(new PushButton(this, CDU, BUTTON_27, "437", "CDU", "A"));
            AddFunction(new PushButton(this, CDU, BUTTON_28, "438", "CDU", "B"));
            AddFunction(new PushButton(this, CDU, BUTTON_29, "439", "CDU", "C"));
            AddFunction(new PushButton(this, CDU, BUTTON_30, "440", "CDU", "D"));
            AddFunction(new PushButton(this, CDU, BUTTON_31, "441", "CDU", "E"));
            AddFunction(new PushButton(this, CDU, BUTTON_32, "442", "CDU", "F"));
            AddFunction(new PushButton(this, CDU, BUTTON_33, "443", "CDU", "G"));
            AddFunction(new PushButton(this, CDU, BUTTON_34, "444", "CDU", "H"));
            AddFunction(new PushButton(this, CDU, BUTTON_35, "445", "CDU", "I"));
            AddFunction(new PushButton(this, CDU, BUTTON_36, "446", "CDU", "J"));
            AddFunction(new PushButton(this, CDU, BUTTON_37, "447", "CDU", "K"));
            AddFunction(new PushButton(this, CDU, BUTTON_38, "448", "CDU", "L"));
            AddFunction(new PushButton(this, CDU, BUTTON_39, "449", "CDU", "M"));
            AddFunction(new PushButton(this, CDU, BUTTON_40, "450", "CDU", "N"));
            AddFunction(new PushButton(this, CDU, BUTTON_41, "451", "CDU", "O"));
            AddFunction(new PushButton(this, CDU, BUTTON_42, "452", "CDU", "P"));
            AddFunction(new PushButton(this, CDU, BUTTON_43, "453", "CDU", "Q"));
            AddFunction(new PushButton(this, CDU, BUTTON_44, "454", "CDU", "R"));
            AddFunction(new PushButton(this, CDU, BUTTON_45, "455", "CDU", "S"));
            AddFunction(new PushButton(this, CDU, BUTTON_46, "456", "CDU", "T"));
            AddFunction(new PushButton(this, CDU, BUTTON_47, "457", "CDU", "U"));
            AddFunction(new PushButton(this, CDU, BUTTON_48, "458", "CDU", "V"));
            AddFunction(new PushButton(this, CDU, BUTTON_49, "459", "CDU", "W"));
            AddFunction(new PushButton(this, CDU, BUTTON_50, "460", "CDU", "X"));
            AddFunction(new PushButton(this, CDU, BUTTON_51, "461", "CDU", "Y"));
            AddFunction(new PushButton(this, CDU, BUTTON_52, "462", "CDU", "Z"));
			AddFunction(new PushButton(this, CDU, BUTTON_53, "464", "CDU", "V1"));
			AddFunction(new PushButton(this, CDU, BUTTON_54, "465", "CDU", "V2"));
			AddFunction(new PushButton(this, CDU, BUTTON_55, "466", "CDU", "MK"));
            AddFunction(new PushButton(this, CDU, BUTTON_56, "467", "CDU", "BCK"));
            AddFunction(new PushButton(this, CDU, BUTTON_57, "468", "CDU", "SPC"));
            AddFunction(new PushButton(this, CDU, BUTTON_58, "470", "CDU", "CLR"));
            AddFunction(new PushButton(this, CDU, BUTTON_59, "471", "CDU", "FA"));

            AddFunction(new Rocker(this, CDU, BUTTON_60, BUTTON_61, BUTTON_60, BUTTON_61, "424", "CDU", "Brightness", false));
            AddFunction(new Rocker(this, CDU, BUTTON_62, BUTTON_63, BUTTON_62, BUTTON_63, "463", "CDU", "Page", true));
            AddFunction(new Rocker(this, CDU, BUTTON_64, BUTTON_65, BUTTON_64, BUTTON_65, "469", "CDU", "Blank", false));
            AddFunction(new Rocker(this, CDU, BUTTON_66, BUTTON_67, BUTTON_66, BUTTON_67, "472", "CDU", "+/-", true));
            #endregion

            #region Electrical Power Panel
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_1, "241", "1", "Power", "0", "Off/Reset", "Electrical", "APU Generator", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, BUTTON_2, "242", "1", "STBY", "0", "Off", "-1.0", "Test", "Electrical", "Inverter", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, LIGHT_SYSTEM, BUTTON_7, "243", "1", "On", "0", "Off", "Electrical", "Emergency Flood", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_4, "244", "1", "Power", "0", "Off/Reset", "Electrical", "AC Generator - Left", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_5, "245", "1", "Power", "0", "Off/Reset", "Electrical", "AC Generator - Right", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_6, "246", "1", "Power", "0", "Off/Reset", "Electrical", "Battery", "%1d"));
            #endregion

            #region Oxygen System Control Panel
            AddFunction(Switch.CreateThreeWaySwitch(this, OXYGEN_SYSTEM, BUTTON_3, "601", "1", "Emergency", "0", "Normal", "-1", "Test Mask", "Oxygen System", "Emergency Lever", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, OXYGEN_SYSTEM, BUTTON_2, "602", "1", "100%", "0", "Normal", "Oxygen System", "Dilution Lever", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, OXYGEN_SYSTEM, BUTTON_1, "603", "1", "On", "0", "Off", "Oxygen System", "Supply Lever", "%1d"));
            AddFunction(new ScaledNetworkValue(this, "274", 10d, "Oxygen System", "Volume", "Indicates the quantity of liquid oxygen in the regulator.", "", BindingValueUnits.Liters));

            CalibrationPointCollectionDouble oxyPressureScale =
                new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 500d) {new CalibrationPointDouble(0.5d, 100d)};
            AddFunction(new ScaledNetworkValue(this, "604", oxyPressureScale, "Oxygen System", "Pressure", "Current PSI of the regulator.", "", BindingValueUnits.PoundsPerSquareInch));
            AddFunction(new FlagValue(this, "600", "Oxygen System", "Breathflow", "Flashs with each breath."));
            #endregion

            #region Cockpit Mechanical Controls
            AddFunction(new Switch(this, CPT_MECH, "712", new SwitchPosition[] { new SwitchPosition("1.0", "Open", BUTTON_6), new SwitchPosition("0.5", "Hold", BUTTON_6), new SwitchPosition("0.0", "Close", BUTTON_7, BUTTON_7, "0.5") }, "Mechanical", "Canopy Open/Hold/Close", "%0.2f"));
			AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_11, "787", "1", "Open", "0", "Closed", "Mechanical", "Extend boarding ladder cover", "%1d"));
			AddFunction(new PushButton(this, CPT_MECH, BUTTON_12, "788", "Mechanical", "Extend boarding ladder button"));
			AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_15, "786", "0", "Down", "1", "Up", "Mechanical", "Canopy Jettison Lever Unlock Button", "%1d"));
			AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_14, "785", "0", "Down", "1", "Up", "Mechanical", "Canopy jettison lever", "%1d"));
			AddFunction(Switch.CreateThreeWaySwitch(this, CPT_MECH, BUTTON_2, "773", "0.0", "UP", "0.5", "MVR", "1.0", "DN", "Mechanical", "Flap Setting", "%0.1f"));
			AddFunction(new Rocker(this, CPT_MECH, BUTTON_5, BUTTON_4, BUTTON_5, BUTTON_4, "770", "Mechanical", "Seat Height Adjustment", true));
			AddFunction(new Axis(this, CPT_MECH, BUTTON_13, "777", 0.1d, 0.0d, 1.0d, "Mechanical", "Internal canopy actuator disengage lever"));
            #endregion

            #region Circuit Breaker Panel

            AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_7, "666", "Circuit Breaker Panel", "AILERON DISC L"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_8, "667", "Circuit Breaker Panel", "AILERON DISC R"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_9, "668", "Circuit Breaker Panel", "SPS & RUDDER AUTH LIMIT"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_10, "669", "Circuit Breaker Panel", "ELEVATION DISC L"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_11, "670", "Circuit Breaker Panel", "ELEVATION DISC R"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_12, "671", "Circuit Breaker Panel", "AILERON TAB L"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_13, "672", "Circuit Breaker Panel", "AILERON TAB R"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_14, "673", "Circuit Breaker Panel", "EMER FLAP"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_15, "674", "Circuit Breaker Panel", "EMER TRIM"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_16, "675", "Circuit Breaker Panel", "LAND GEAR"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_17, "676", "Circuit Breaker Panel", "ENGINE START L"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_18, "677", "Circuit Breaker Panel", "ENGINE START R"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_19, "678", "Circuit Breaker Panel", "APU CONT"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_20, "679", "Circuit Breaker Panel", "ENG IGNITOR L/R-1"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_21, "680", "Circuit Breaker Panel", "ENG IGNITOR L/R-2"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_22, "681", "Circuit Breaker Panel", "EMER FUEL SHUTOFF ENG L"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_23, "682", "Circuit Breaker Panel", "EMER FUEL SHUTOFF ENG R"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_24, "683", "Circuit Breaker Panel", "DC FUEL PUMP"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_25, "684", "Circuit Breaker Panel", "BLEED AIR CONT L"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_26, "685", "Circuit Breaker Panel", "BLEED AIR CONT R"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_27, "686", "Circuit Breaker Panel", "EXT STORES JETT 1"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_28, "687", "Circuit Breaker Panel", "EXT STORES JETT 2"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_29, "688", "Circuit Breaker Panel", "STBY ATT IND"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_30, "689", "Circuit Breaker Panel", "MASTER CAUT"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_31, "690", "Circuit Breaker Panel", "PITOT HEAT AC"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_32, "691", "Circuit Breaker Panel", "IFF"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_33, "692", "Circuit Breaker Panel", "UHF COMM"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_34, "693", "Circuit Breaker Panel", "INTER COMM"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_35, "694", "Circuit Breaker Panel", "GENERATOR CONT L"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_36, "695", "Circuit Breaker Panel", "GENERATOR CONT R"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_37, "696", "Circuit Breaker Panel", "CONVERTER L"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_38, "697", "Circuit Breaker Panel", "AUX ESS BUS 0A"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_39, "698", "Circuit Breaker Panel", "AUX ESS BUS 0B"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_40, "699", "Circuit Breaker Panel", "AUX ESS BUS 0C"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_41, "700", "Circuit Breaker Panel", "BATTERY BUS TRANS"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_42, "701", "Circuit Breaker Panel", "INVERTER PWR"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_43, "702", "Circuit Breaker Panel", "INVERTER CONT"));
			AddFunction(new PushButton(this, ELEC_INTERFACE, BUTTON_44, "703", "Circuit Breaker Panel", "AUX ESS BUS TIE"));
            #endregion

            #region EW Panel (CMSP)
            AddFunction(new PushButton(this, CMSP, BUTTON_1, "352", "CMSP", "OSB 1"));
            AddFunction(new PushButton(this, CMSP, BUTTON_2, "353", "CMSP", "OSB 2"));
            AddFunction(new PushButton(this, CMSP, BUTTON_3, "354", "CMSP", "OSB 3"));
            AddFunction(new PushButton(this, CMSP, BUTTON_4, "355", "CMSP", "OSB 4"));
            AddFunction(new Rocker(this, CMSP, BUTTON_5, BUTTON_6, BUTTON_5, BUTTON_6, "356", "CMSP", "Page Cycle", true));
            AddFunction(new PushButton(this, CMSP, BUTTON_7, "357", "CMSP", "Return to Previous Rotary Menu"));
            AddFunction(Switch.CreateToggleSwitch(this, CMSP, BUTTON_8, "358", "1", "Jettison", "0", "Off", "CMSP", "ECM Pod Jettison", "%1d"));
            AddFunction(new Axis(this, CMSP, BUTTON_9, "359", 0.1d, 0.15d, 0.85d, "CMSP", "Brightness"));
            AddFunction(new Switch(this, CMSP, "360", new SwitchPosition[] { new SwitchPosition("0.2", "Menu", BUTTON_11, BUTTON_11, "0.1"), new SwitchPosition("0.1", "On", null), new SwitchPosition("0.0", "Off", BUTTON_10, BUTTON_10, "0.1") }, "CMSP", "MWS", "%0.1f"));
            AddFunction(new Switch(this, CMSP, "361", new SwitchPosition[] { new SwitchPosition("0.2", "Menu", BUTTON_13, BUTTON_13, "0.1"), new SwitchPosition("0.1", "On", null), new SwitchPosition("0.0", "Off", BUTTON_12, BUTTON_12, "0.1") }, "CMSP", "JMR", "%0.1f"));
            AddFunction(new Switch(this, CMSP, "362", new SwitchPosition[] { new SwitchPosition("0.2", "Menu", BUTTON_15, BUTTON_15, "0.1"), new SwitchPosition("0.1", "On", null), new SwitchPosition("0.0", "Off", BUTTON_14, BUTTON_14, "0.1") }, "CMSP", "RWR", "%0.1f"));
            AddFunction(new Switch(this, CMSP, "363", new SwitchPosition[] { new SwitchPosition("0.2", "Menu", BUTTON_17, BUTTON_17, "0.1"), new SwitchPosition("0.1", "On", null), new SwitchPosition("0.0", "Off", BUTTON_16, BUTTON_16, "0.1") }, "CMSP", "DISP", "%0.1f"));
            AddFunction(new Switch(this, CMSP, "364", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_18), new SwitchPosition("0.1", "Standby", BUTTON_18), new SwitchPosition("0.2", "Manual", BUTTON_18), new SwitchPosition("0.3", "Semi-Automatic", BUTTON_18), new SwitchPosition("0.4", "Automatic", BUTTON_18) }, "CMSP", "Mode Select Dial", "%0.1f"));
            AddFunction(new Text(this, "2400", "CMSP", "Line 1 Display", "Line 1 Display"));
            AddFunction(new Text(this, "2401", "CMSP", "Line 2 MWS Display", "Text Line 2 MWS Display"));
            AddFunction(new Text(this, "2402", "CMSP", "Line 2 JMR Display", "Text Line 2 JMR Display"));
            AddFunction(new Text(this, "2403", "CMSP", "Line 2 RWR Display", "Text Line 2 RWR Display"));
            AddFunction(new Text(this, "2404", "CMSP", "Line 2 DISP Display", "Text Line 2 DISP Display"));

            #endregion

            #region Environment Control Panel
            AddFunction(new PushButton(this, ENVIRONMENT_SYSTEM, BUTTON_1, "275", "Environmental Control", "Oxygen Indicator Test"));
            AddFunction(Switch.CreateToggleSwitch(this, ENVIRONMENT_SYSTEM, BUTTON_2, "276", "1", "Defog/Deice", "0", "Off", "Environmental Control", "Windshield Defog/Deice", "%1d"));
            AddFunction(new Axis(this, ENVIRONMENT_SYSTEM, BUTTON_3, "277", 0.1d, 0.0d, 1.0d, "Environmental Control", "Canopy Defog"));
			
			AddFunction(Switch.CreateThreeWaySwitch(this, ENVIRONMENT_SYSTEM, BUTTON_4, "278", "1", "Remove", "0", "Off", "-1", "Wash", "Environmental Control", "Windshield Remove/Wash", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ENVIRONMENT_SYSTEM, BUTTON_5, "279", "1", "On", "0", "Off", "Environmental Control", "Pitot heat", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ENVIRONMENT_SYSTEM, BUTTON_6, "280", "1", "On", "0", "Off", "Environmental Control", "Bleed Air", "%1d"));
            AddFunction(new ScaledNetworkValue(this, "281", 50000d, "Environmental Control", "Cabin Pressure", "Current cabin pressure of the aircraft.", "(0 - 50,000)", BindingValueUnits.Numeric));
            AddFunction(Switch.CreateThreeWaySwitch(this, ENVIRONMENT_SYSTEM, BUTTON_7, "282", "1", "Normal", "0", "Dump", "-1", "RAM", "Environmental Control", "Temp/Press", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ENVIRONMENT_SYSTEM, BUTTON_8, "283", "1", "On", "0", "Off", "Environmental Control", "Main Air Supply", "%1d"));
            AddFunction(new Axis(this, ENVIRONMENT_SYSTEM, BUTTON_9, "284", 0.1d, 0.0d, 1.0d, "Environmental Control", "Flow Level"));
			AddFunction(new Axis(this, ENVIRONMENT_SYSTEM, BUTTON_13, "286", 0.1d, 0.0d, 1.0d, "Environmental Control", "Temp Level Control"));
			AddFunction(new PushButton(this, ENVIRONMENT_SYSTEM, BUTTON_14, "776", "Environmental Control", "Anti-G suit valve test button"));
			AddFunction(new Switch(this, ENVIRONMENT_SYSTEM, "285",new SwitchPosition[]  { new SwitchPosition("0.3", "Hot", "3012"), new SwitchPosition("0.2", "Cold", "3011"), new SwitchPosition("0.1", "Auto", "3010"), new SwitchPosition("0.0", "Manual", "3010") }, "Environmental Control", "Air Conditioner MAN/AUTO", "%0.1f"));
			#endregion

			#region Light System Control Panel
			AddFunction(Switch.CreateThreeWaySwitch(this, LIGHT_SYSTEM, BUTTON_8, "287", "1", "Flash", "0", "Off", "-1", "Steady", "Light System", "Position Flash", "%1d"));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_9, "288", 0.1d, 0.0d, 1.0d, "Light System", "Formation Lights"));
            AddFunction(new Switch(this, LIGHT_SYSTEM, "289", new SwitchPosition[] { new SwitchPosition("1", "On", BUTTON_10, BUTTON_11, "0"), new SwitchPosition("0", "Off", BUTTON_10, BUTTON_11, "0") }, "Light System", "Anti-Collision", "%1d", true));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_1, "290", 0.1d, 0.0d, 1.0d, "Light System", "Engine Instrument Lights"));
            AddFunction(Switch.CreateToggleSwitch(this, LIGHT_SYSTEM, BUTTON_12, "291", "1", "On", "0", "Off", "Light System", "Nose Illumination", "%1d"));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_2, "292", 0.1d, 0.0d, 1.0d, "Light System", "Flight Instruments Lights"));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_3, "293", 0.1d, 0.0d, 1.0d, "Light System", "Auxillary instrument Lights"));
            AddFunction(Switch.CreateToggleSwitch(this, LIGHT_SYSTEM, BUTTON_13, "294", "1", "Bright", "0", "Dim", "Light System", "Signal Lights", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, LIGHT_SYSTEM, BUTTON_4, "295", "1", "On", "0", "Off", "Light System", "Accelerometer & Compass Lights", "%1d"));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_5, "296", 0.1d, 0.0d, 1.0d, "Light System", "Flood Light"));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_6, "297", 0.1d, 0.0d, 1.0d, "Light System", "Console Lights"));
            #endregion

            #region Caution Panel
            AddFunction(new FlagValue(this, "480", "Caution Panel", "ENG START CYCLE", "Lit if either engine is in engine start process."));
            AddFunction(new FlagValue(this, "481", "Caution Panel", "L-HYD PRESS", "Lit if left hydraulic system pressure falls below 1,000 psi."));
            AddFunction(new FlagValue(this, "482", "Caution Panel", "R-HYD PRESS", "Lit if right hydraulic system pressure falls below 1,000 psi."));
            AddFunction(new FlagValue(this, "483", "Caution Panel", "GUN UNSAFE", "Lit if gun is capable of being fired."));
            AddFunction(new FlagValue(this, "484", "Caution Panel", "ANTI SKID", "Lit if landing gear is down but anti-skid is disengaged."));
            AddFunction(new FlagValue(this, "485", "Caution Panel", "L-HYD RES", "Lit if left hyudraulic fluid reservoir is low."));
            AddFunction(new FlagValue(this, "486", "Caution Panel", "R-HYD RES", "Lit if right hyudraulic fluid reservoir is low."));
            AddFunction(new FlagValue(this, "487", "Caution Panel", "OXY LOW", "Lit if oxygen gauge indices 0.5 liters or less."));
            AddFunction(new FlagValue(this, "488", "Caution Panel", "ELEV DISENG", "Lit if at least one elevator is disengaged from the Emergency Flight Control panel."));
            AddFunction(new FlagValue(this, "489", "Caution Panel", "VOID1", ""));
            AddFunction(new FlagValue(this, "490", "Caution Panel", "SEAT NOT ARMED", "Lit if ground safety lever is in the safe position."));
            AddFunction(new FlagValue(this, "491", "Caution Panel", "BLEED AIR LEAK", "Lit if bleed air is 400 degrees or higher."));
            AddFunction(new FlagValue(this, "492", "Caution Panel", "AIL DISENG", "Lit if at least one aileron is disngaged from the Emergency FLight Control panel."));
            AddFunction(new FlagValue(this, "493", "Caution Panel", "L-AIL TAB", "Lit if left aileron is not at normal positoin due to MRFCS."));
            AddFunction(new FlagValue(this, "494", "Caution Panel", "R-AIL TAB", "Lit if right aileron is not at normal positoin due to MRFCS."));
            AddFunction(new FlagValue(this, "495", "Caution Panel", "SERVICE AIR HOT", "Lit if air temperature exceeds allowable ECS range."));
            AddFunction(new FlagValue(this, "496", "Caution Panel", "PITCH SAS", "Lit if at least one pitch SAS channel has been disabled."));
            AddFunction(new FlagValue(this, "497", "Caution Panel", "L-ENG HOT", "Lit if left engine ITT exceeds 880 degrees C."));
            AddFunction(new FlagValue(this, "498", "Caution Panel", "R-ENG HOT", "Lit if right engine ITT exceeds 880 degrees C."));
            AddFunction(new FlagValue(this, "499", "Caution Panel", "WINDSHIELD HOT", "Lit if windshield temperature exceeds 150 degrees F."));
            AddFunction(new FlagValue(this, "500", "Caution Panel", "YAW SAS", "Lit if at least one yaw SAS channel has been disabled."));
            AddFunction(new FlagValue(this, "501", "Caution Panel", "L-ENG OIL PRESS", "Lit if left engine oil pressure is less than 27.5 psi."));
            AddFunction(new FlagValue(this, "502", "Caution Panel", "R-ENG OIL PRESS", "Lit if right engine oil pressure is less than 27.5 psi."));
            AddFunction(new FlagValue(this, "503", "Caution Panel", "CICU", "Lit if ?."));
            AddFunction(new FlagValue(this, "504", "Caution Panel", "GCAS", "Lit if LASTE failure is detected that affects GCAS."));
            AddFunction(new FlagValue(this, "505", "Caution Panel", "L-MAIN PUMP", "Lit if boost pump pressure is low."));
            AddFunction(new FlagValue(this, "506", "Caution Panel", "R-MAIN PUMP", "Lit if boost pump pressure is low."));
            AddFunction(new FlagValue(this, "507", "Caution Panel", "VOID2", ""));
            AddFunction(new FlagValue(this, "508", "Caution Panel", "LASTE", "Lit if fault is detected in LASTE computer."));
            AddFunction(new FlagValue(this, "509", "Caution Panel", "L-WING PUMP", "Lit if boost pump pressure is low."));
            AddFunction(new FlagValue(this, "510", "Caution Panel", "R-WING PUMP", "Lit if boost pump pressure is low."));
            AddFunction(new FlagValue(this, "511", "Caution Panel", "HARS", "Lit if HARS heading or attitude is invalid."));
            AddFunction(new FlagValue(this, "512", "Caution Panel", "IFF MODE-4", "Lit if inoperative mode 4 capability is detected."));
            AddFunction(new FlagValue(this, "513", "Caution Panel", "L-MAIN FUEL LOW", "Lit if left main fuel tank has 500 pounds or less."));
            AddFunction(new FlagValue(this, "514", "Caution Panel", "R-MAIN FUEL LOW", "Lit if right main fuel tank has 500 pounds or less."));
            AddFunction(new FlagValue(this, "515", "Caution Panel", "L-R TKS UNEQUAL", "Lit if thers is a 750 or more pund difference between the two main fuel tanks."));
            AddFunction(new FlagValue(this, "516", "Caution Panel", "EAC", "Lit if EAC is turned off."));
            AddFunction(new FlagValue(this, "517", "Caution Panel", "L-FUEL PRESS", "Lit if low fuel pressure is detected in fuel feed lines."));
            AddFunction(new FlagValue(this, "518", "Caution Panel", "R-FUEL PRESS", "Lit if low fuel pressure is detected in fuel feed lines."));
            AddFunction(new FlagValue(this, "519", "Caution Panel", "NAV", "Lit if there is a CDU failure while in alignment mode."));
            AddFunction(new FlagValue(this, "520", "Caution Panel", "STALL SYS", "Lit if there is a power failure to the AoA and Mach meters."));
            AddFunction(new FlagValue(this, "521", "Caution Panel", "L-CONV", "Lit if left electrical converter fails."));
            AddFunction(new FlagValue(this, "522", "Caution Panel", "R-CONV", "Lit if right electrical converter fails."));
            AddFunction(new FlagValue(this, "523", "Caution Panel", "CADC", "Lit if CADC has failed."));
            AddFunction(new FlagValue(this, "524", "Caution Panel", "APU GEN", "Lit if APU is on but APU generator is not set to PWR."));
            AddFunction(new FlagValue(this, "525", "Caution Panel", "L-GEN", "Lit if left generator has shut down or AC power is out of limits."));
            AddFunction(new FlagValue(this, "526", "Caution Panel", "R-GEN", "Lit if right generator has shut down or AC power is out of limits."));
            AddFunction(new FlagValue(this, "527", "Caution Panel", "INST INV", "Lit if AC powered systems are not receiving power from inverter."));
            #endregion

            #region TACAN Control Panel
            AddFunction(new RotaryEncoder(this, TACAN_CTRL_PANEL, BUTTON_1, "256", 0.02, "TACAN", "Channel Selector (Tens)"));
            AddFunction(new RotaryEncoder(this, TACAN_CTRL_PANEL, BUTTON_2, "257", 0.1, "TACAN", "Channel Selector (Ones )"));
			AddFunction(Switch.CreateToggleSwitch(this, TACAN_CTRL_PANEL, BUTTON_3, "258", "-1", "X", "1", "Y", "TACAN", "Channel Selector Mode", "%0.2f"));
            AddFunction(new PushButton(this, TACAN_CTRL_PANEL, BUTTON_4, "259", "TACAN", "Test"));
            AddFunction(new Axis(this, TACAN_CTRL_PANEL, BUTTON_5, "261", 0.1d, 0.0d, 1.0d, "TACAN", "Volumne"));
            AddFunction(new Switch(this, TACAN_CTRL_PANEL, "262", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_6), new SwitchPosition("0.1", "Receive", BUTTON_6), new SwitchPosition("0.2", "T/R", BUTTON_6), new SwitchPosition("0.3", "A/A Receive", BUTTON_6), new SwitchPosition("0.4", "A/A T/R", BUTTON_6) }, "TACAN", "Mode", "%0.1f"));
            AddFunction(new FlagValue(this, "260", "TACAN", "Test Light", ""));
            AddFunction(new TACANChannel(this));
            #endregion

            #region ILS Control Panel
            AddFunction(Switch.CreateToggleSwitch(this, ILS, BUTTON_1, "247", "1", "On", "0", "Off", "ILS", "Power", "%1d"));
            AddFunction(new AbsoluteEncoder(this, ILS, BUTTON_2, BUTTON_2, "248", 0.05d, 0.0d, 0.3d, "ILS", "ILS Frequencey Mhz", false, "%0.1f"));
            AddFunction(new AbsoluteEncoder(this, ILS, BUTTON_3, BUTTON_3, "249", 0.05d, 0.0d, 0.9d, "ILS", "ILS Frequencey Khz", false, "%0.1f"));
            AddFunction(new Axis(this, ILS, BUTTON_5, "250", 0.1d, 0d, 1d, "ILS", "Volume"));
            AddFunction(new ILSFrequency(this));
            #endregion

            #region HARS Control Panel
            AddFunction(Switch.CreateToggleSwitch(this, HARS, BUTTON_2, "270", "1", "Slave", "0", "DG", "HARS", "Mode", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, HARS, BUTTON_3, "273", "1", "North", "0", "South", "HARS", "Hemisphere Selector", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, HARS, BUTTON_4, "272", "1", "+15", "0", "0", "-1", "-15", "HARS", "Magnetic Variation", "%1d"));
            AddFunction(new Axis(this, HARS, BUTTON_5, "271", 0.1d, 0d, 1d, "HARS", "Latitude Correction"));
            AddFunction(new PushButton(this, HARS, BUTTON_6, "267", "HARS", "Sync Button Push"));
            AddFunction(new Axis(this, HARS, BUTTON_7, "268", 0.5d, 0d, 1d, "HARS", "Sync Button Rotate"));
            AddFunction(new NetworkValue(this, "269", "HARS", "SYN-IND Sync Needle", "Position of needle on SYN-IND anannunciation.", "-1 to 1", BindingValueUnits.Numeric));
            #endregion

            #region Aux Avionics Panel
            AddFunction(new Switch(this, AAP, "473", new SwitchPosition[] { new SwitchPosition("0.0", "Flight Plan", BUTTON_1), new SwitchPosition("0.1", "Mark", BUTTON_1), new SwitchPosition("0.2", "Mission", BUTTON_1) }, "AAP", "Steer Point Dial", "%0.1f"));
            AddFunction(new Rocker(this, AAP, BUTTON_2, BUTTON_3, BUTTON_2, BUTTON_3, "474", "AAP", "Steer Toggle Switch", true));
            AddFunction(new Switch(this, AAP, "475", new SwitchPosition[] { new SwitchPosition("0.0", "Other", BUTTON_4), new SwitchPosition("0.1", "Position", BUTTON_4), new SwitchPosition("0.2", "Steer", BUTTON_4), new SwitchPosition("0.3", "Waypoint", BUTTON_4) }, "AAP", "CDU Page Select", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, AAP, BUTTON_5, "476", "1", "On", "0", "Off", "AAP", "CDU Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, AAP, BUTTON_6, "477", "1", "On", "0", "Off", "AAP", "EGI Power", "%1d"));
            #endregion

            #region Fuel System Panel
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_1, "106", "1", "On", "0", "Off", "Fuel System", "External Wing Tank Boost Pump", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_2, "107", "1", "On", "0", "Off", "Fuel System", "External Fuselage Tank Boost Pump", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_3, "108", "1", "Open", "0", "Closed", "Fuel System", "Tank Gate", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_4, "109", "1", "On", "0", "Off", "Fuel System", "Cross Feed", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_5, "110", "1", "On", "0", "Off", "Fuel System", "Boost Pump Left Wing", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_6, "111", "1", "On", "0", "Off", "Fuel System", "Boost Pump Right Wing", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_7, "112", "1", "On", "0", "Off", "Fuel System", "Boost Pump Main Fuseloge Left", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_8, "113", "1", "On", "0", "Off", "Fuel System", "Boost Pump Main Fuseloge Right", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_9, "114", "1", "Normal", "0", "Override", "Fuel System", "Signal Amplifier", "%1d"));
            AddFunction(new PushButton(this, FUEL_SYSTEM, BUTTON_10, "115", "Fuel System", "Line Check"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_12, "117", "1", "On", "0", "Off", "Fuel System", "Fill Disable Wing Left", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_13, "118", "1", "On", "0", "Off", "Fuel System", "Fill Disable Wing Right", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_14, "119", "1", "On", "0", "Off", "Fuel System", "Fill Disable Main Left", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_15, "120", "1", "On", "0", "Off", "Fuel System", "Fill Disable Main Right", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUEL_SYSTEM, BUTTON_16, "121", "1", "Closed", "0", "Open", "Fuel System", "Refuel Control Lever", "%1d"));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_18, "116", 0.1d, 0.0d, 1.0d, "Light System", "Refueling Lighting Dial"));
            #endregion

            #region Engine Throtle Panel
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_SYSTEM, BUTTON_1, "122", "1", "Norm", "0", "Override", "Engine System", "Left Engine Fuel Flow Control", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_SYSTEM, BUTTON_2, "123", "1", "Norm", "0", "Override", "Engine System", "Right Engine Fuel Flow Control", "%1d"));
            AddFunction(new Switch(this, ENGINE_SYSTEM, "124", new SwitchPosition[] { new SwitchPosition("1", "Ignite", BUTTON_7, BUTTON_7, "0"), new SwitchPosition("0", "Normal", BUTTON_3), new SwitchPosition("-1", "Motor", BUTTON_3) }, "Engine System", "Engine Operate Left", "%1d"));
            AddFunction(new Switch(this, ENGINE_SYSTEM, "125", new SwitchPosition[] { new SwitchPosition("1", "Ignite", BUTTON_8, BUTTON_8, "0"), new SwitchPosition("0", "Normal", BUTTON_4), new SwitchPosition("-1", "Motor", BUTTON_4) }, "Engine System", "Engine Operate Right", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_SYSTEM, BUTTON_5, "126", "1", "On", "0", "Off", "Engine System", "APU", "%1d"));
            AddFunction(new PushButton(this, SYS_CONTROLLER, BUTTON_3, "127", "System Controller", "Landing Gear Horn Silence Button"));
			AddFunction(new Axis(this, ENGINE_SYSTEM, BUTTON_6, "128", 0.1d, 0.0d, 1.0d, "Engine System", "Throttle Friction Control"));
			AddFunction(new NetworkValue(this, "8", "Engine System", "Left Engine Throttle", "Position of the Left Engine Throttle.", "(0 to 1)", BindingValueUnits.Numeric));
			AddFunction(new NetworkValue(this, "9", "Engine System", "Right Engine Throttle", "Position of the Right Engine Throttle.", "(0 to 1)", BindingValueUnits.Numeric));
			AddFunction(new PushButton(this, ENGINE_SYSTEM, BUTTON_9, "652", "Misc", "TEMS DATA"));
			#endregion

			#region LASTE Panel
			AddFunction(Switch.CreateThreeWaySwitch(this, AUTOPILOT, BUTTON_1, "132", "1", "Path", "0", "Altitude / Heading", "-1", "Altitude", "Autopilot", "Mode Selection", "%1d"));
            AddFunction(new PushButton(this, AUTOPILOT, BUTTON_2, "131", "Autopilot", "Engage/Disengage"));
            AddFunction(new Switch(this, AUTOPILOT, "129", new SwitchPosition[] { new SwitchPosition("1", "On", BUTTON_26, BUTTON_27, "0"), new SwitchPosition("0", "Off", BUTTON_26, BUTTON_27, "0") }, "Autopilot", "EAC", "%1d", true));
            AddFunction(Switch.CreateToggleSwitch(this, AN_APN_194, BUTTON_1, "130", "1", "Normal", "0", "Disengage", "Radar Altimeter", "Normal/Disabled", "%1d"));
            #endregion

            #region VHF AM Radio
            AddFunction(new Functions.VHFPresetSelector(this, VHF_AM_RADIO, BUTTON_1, "137", 0.01d, 0.00d, 0.19d, "VHF AM Radio", "Preset Channel Selector"));
            AddFunction(new Switch(this, VHF_AM_RADIO, "138", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_3), new SwitchPosition("0.1", "TX", BUTTON_3), new SwitchPosition("0.2", "DN", BUTTON_3) }, "VHF AM Radio", "Frequency Mode Dial", "%0.1f"));
            AddFunction(new Switch(this, VHF_AM_RADIO, "135", new SwitchPosition[] { new SwitchPosition("0.0", "Emergency FM", BUTTON_4), new SwitchPosition("0.1", "Emergency AM", BUTTON_4), new SwitchPosition("0.2", "Manual", BUTTON_4), new SwitchPosition("0.3", "PRE", BUTTON_4) }, "VHF AM Radio", "Frequency Selection Dial", "%0.1f"));
            AddFunction(new Axis(this, VHF_AM_RADIO, BUTTON_5, "133", 0d, 0d, 1d, "VHF AM Radio", "Volume"));
            AddFunction(new PushButton(this, VHF_AM_RADIO, BUTTON_6, "136", "VHF AM Radio", "Load"));
            AddFunction(new Switch(this, VHF_AM_RADIO, "134", new SwitchPosition[] { new SwitchPosition("-1", "Squelch", BUTTON_7), new SwitchPosition("0", "Off", BUTTON_7), new SwitchPosition("1", "Tone", BUTTON_8, BUTTON_8, "0") }, "VHF AM Radio", "Squelch / Tone", "%1d"));

            // silently consume values sent by old export scripts generated before these were fixed
            AddFunction(new SilentValueConsumer(this, "139", "Previous incorrect or out of date assignment for value 143"));
            AddFunction(new SilentValueConsumer(this, "140", "Previous incorrect or out of date assignment for value 144"));
            AddFunction(new SilentValueConsumer(this, "141", "Previous incorrect or out of date assignment for value 145"));
            AddFunction(new SilentValueConsumer(this, "142", "Previous incorrect or out of date assignment for value 146"));

            AddFunction(new Functions.VHFRadioEncoder1(this, VHF_AM_RADIO, BUTTON_9, "143", 0.1d, 0d, 1d, "VHF AM Radio", "1st Frequency Selector"));
			AddFunction(new Functions.VHFRadioEncoder(this, VHF_AM_RADIO, BUTTON_11, "144", 0.1d, 0.0d, 0.9d, "VHF AM Radio", "2nd Frequency Selector"));
            AddFunction(new Functions.VHFRadioEncoder3(this, VHF_AM_RADIO, BUTTON_13, "145", 0.1d, 0.0d, 0.9d, "VHF AM Radio", "3rd Frequency Selector"));
            AddFunction(new Functions.VHFRadioEncoder4(this, VHF_AM_RADIO, BUTTON_15, "146", 0.25d, 0.0d, 0.9d, "VHF AM Radio", "4th Frequency Selector"));
			#endregion

			#region SAS Panel
			AddFunction(new Switch(this, AUTOPILOT, "185", new SwitchPosition[] { new SwitchPosition("1", "On", BUTTON_3, BUTTON_4, "0"), new SwitchPosition("0", "Off", BUTTON_3, BUTTON_4, "0") }, "Autopilot", "Yaw SAS Engage Left", "%1d", true));
            AddFunction(new Switch(this, AUTOPILOT, "186", new SwitchPosition[] { new SwitchPosition("1", "On", BUTTON_5, BUTTON_6, "0"), new SwitchPosition("0", "Off", BUTTON_5, BUTTON_6, "0") }, "Autopilot", "Yaw SAS Engage Right", "%1d", true));
            AddFunction(new Switch(this, AUTOPILOT, "187", new SwitchPosition[] { new SwitchPosition("1", "On", BUTTON_7, BUTTON_8, "0"), new SwitchPosition("0", "Off", BUTTON_7, BUTTON_8, "0") }, "Autopilot", "Pitch SAS Engage Left", "%1d", true));
            AddFunction(new Switch(this, AUTOPILOT, "188", new SwitchPosition[] { new SwitchPosition("1", "On", BUTTON_9, BUTTON_10, "0"), new SwitchPosition("0", "Off", BUTTON_9, BUTTON_10, "0") }, "Autopilot", "Pitch SAS Engage Right", "%1d", true));
            AddFunction(Switch.CreateThreeWaySwitch(this, AUTOPILOT, BUTTON_11, "189", "-1", "Test Left", "0", "Off", "1", "Test Right", "Autopilot", "Monitor Test Left/Right", "%1d"));
            AddFunction(new PushButton(this, AUTOPILOT, BUTTON_12, "190", "Autopilot", "Set Takeoff Trim"));
            AddFunction(new Axis(this, AUTOPILOT, BUTTON_13, "192", 0.1d, -1.0d, 1.0d, "Autopilot", "Yaw Trim"));
            AddFunction(new FlagValue(this, "191", "Autopilot", "Take Off Trim Indicator", "Lit when reseting autopilot for take off trim"));
            #endregion

            #region Aux Light Control Panel
            AddFunction(new PushButton(this, SYS_CONTROLLER, BUTTON_2, "197", "System Controller", "Test Cockpit Indication Lights"));
            AddFunction(Switch.CreateToggleSwitch(this, AUTOPILOT, BUTTON_31, "196", "1", "Override", "0", "Norm", "Autopilot", "HARS-SAS Override/Norm", "%1d"));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_15, "193", 0.1d, 0d, 1d, "Light System", "Refuel Status Indexer Brightness"));
            AddFunction(new Axis(this, LIGHT_SYSTEM, BUTTON_16, "195", 0.1d, 0d, 1d, "Light System", "Weapon Station Lights Brightness"));
            AddFunction(Switch.CreateThreeWaySwitch(this, LIGHT_SYSTEM, BUTTON_17, "194", "0.2", "Top", "0.1", "All", "0.0", "Off", "Light System", "Nightvision Lights", "%0.1f"));
            AddFunction(new PushButton(this, SYS_CONTROLLER, BUTTON_4, "198", "Fire System", "Fired Detect Bleed Air Test"));
            #endregion

            #region UHF Radio
           
            AddFunction(new AbsoluteEncoder(this, UHF_RADIO, BUTTON_1, BUTTON_1, "161", 0.05d, 0.0d, 0.95d, "UHF Radio", "Preset Channel Selector", true, "%0.2f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, UHF_RADIO, BUTTON_2, "162", "0.0", "2", "0.1", "3", "0.2", "A", "UHF Radio", "100Mhz Selector", "%0.1f"));
            AddFunction(new AbsoluteEncoder(this, UHF_RADIO, BUTTON_3, BUTTON_3, "163", 0.1d, 0.0d, 0.9d, "UHF Radio", "10Mhz Selector", true, "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, UHF_RADIO, BUTTON_4, BUTTON_4, "164", 0.1d, 0.0d, 0.9d, "UHF Radio", "1Mhz Selector", true, "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, UHF_RADIO, BUTTON_5, BUTTON_5, "165", 0.1d, 0.0d, 0.9d, "UHF Radio", "0.1Mhz Selector", true, "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, UHF_RADIO, BUTTON_6, BUTTON_6, "166", 0.1d, 0.0d, 0.4d, "UHF Radio", "0.025Mhz Selector", true, "%0.2f"));

            AddFunction(new Switch(this, UHF_RADIO, "167", new SwitchPosition[] { new SwitchPosition("0.0", "Manual", BUTTON_7), new SwitchPosition("0.1", "Preset", BUTTON_7), new SwitchPosition("0.2", "Guard", BUTTON_7) }, "UHF Radio", "Frequency Mode Dial", "%0.1f"));
            AddFunction(new Switch(this, UHF_RADIO, "168", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_8), new SwitchPosition("0.1", "Main", BUTTON_8), new SwitchPosition("0.2", "Both", BUTTON_8), new SwitchPosition("0.3", "ADF", BUTTON_8) }, "UHF Radio", "Frequency Dial", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, UHF_RADIO, BUTTON_9, "169", "1", "T", "0", "Off", "-1", "Tone", "UHF Radio", "T/Tone Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, UHF_RADIO, BUTTON_10, "170", "0", "On", "1", "Off", "UHF Radio", "Squelch", "%1d"));
            AddFunction(new Axis(this, UHF_RADIO, BUTTON_11, "171", 0.1d, 0.0d, 1.0d, "UHF Radio", "Volume"));
            AddFunction(new PushButton(this, UHF_RADIO, BUTTON_12, "172", "UHF Radio", "Test Display Button"));
            AddFunction(new PushButton(this, UHF_RADIO, BUTTON_13, "173", "UHF Radio", "Status Button"));
            AddFunction(new PushButton(this, UHF_RADIO, BUTTON_15, "735", "UHF Radio", "Load Button"));
            AddFunction(Switch.CreateToggleSwitch(this, UHF_RADIO, BUTTON_14, "734", "0", "Down", "1", "Up", "UHF Radio", "Cover", "%1d"));
            AddFunction(new Text(this, "2413", "UHF_RADIO", "Repeater Display", "UHF Repeater Display"));
            AddFunction(new Text(this, "2412", "UHF_RADIO", "Frequency Display", "UHF Frequency Display"));
            AddFunction(new Text(this, "2411", "UHF_RADIO", "Channel Display", "UHF Channel Display"));

            #endregion

            #region VHF FM Radio
            AddFunction(new Functions.VHFPresetSelector(this, VHF_FM_RADIO, BUTTON_1, "151", 0.01d, 0.00d, 0.19d, "VHF FM Radio", "Preset Channel Selector"));

            // silently consume values sent by old export scripts generated before these were fixed
            AddFunction(new SilentValueConsumer(this, "153", "Previous incorrect or out of date assignment for value 157"));
            AddFunction(new SilentValueConsumer(this, "154", "Previous incorrect or out of date assignment for value 158"));
            AddFunction(new SilentValueConsumer(this, "155", "Previous incorrect or out of date assignment for value 159"));
            AddFunction(new SilentValueConsumer(this, "156", "Previous incorrect or out of date assignment for value 160"));

            AddFunction(new Functions.VHFRadioEncoder1(this, VHF_FM_RADIO, BUTTON_9, "157", 0.05d, 0d, 1d, "VHF FM Radio", "1st Frequency Selector"));
            AddFunction(new Functions.VHFRadioEncoder(this, VHF_FM_RADIO, BUTTON_11, "158", 0.1d, 0.0d, 0.9d, "VHF FM Radio", "2nd Frequency Selector"));
            AddFunction(new Functions.VHFRadioEncoder3(this, VHF_FM_RADIO, BUTTON_13, "159", 0.1d, 0.0d, 0.9d, "VHF FM Radio", "3rd Frequency Selector"));
            AddFunction(new Functions.VHFRadioEncoder4(this, VHF_FM_RADIO, BUTTON_15, "160", 0.25d, 0.0d, 0.9d, "VHF FM Radio", "4th Frequency Selector"));
            AddFunction(new Switch(this, VHF_FM_RADIO, "152", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_3), new SwitchPosition("0.1", "TX", BUTTON_3), new SwitchPosition("0.2", "DN", BUTTON_3) }, "VHF FM Radio", "Frequency Mode Dial", "%0.1f"));
            AddFunction(new Switch(this, VHF_FM_RADIO, "149", new SwitchPosition[] { new SwitchPosition("0.0", "Emergency FM", BUTTON_4), new SwitchPosition("0.1", "Emergency AM", BUTTON_4), new SwitchPosition("0.2", "Manual", BUTTON_4), new SwitchPosition("0.3", "PRE", BUTTON_4) }, "VHF FM Radio", "Frequency Selection Dial", "%0.1f"));
            AddFunction(new Axis(this, VHF_FM_RADIO, BUTTON_5, "147", 0d, 0d, 1d, "VHF FM Radio", "Volume"));
            AddFunction(new PushButton(this, VHF_FM_RADIO, BUTTON_6, "150", "VHF FM Radio", "Load"));
            AddFunction(new Switch(this, VHF_FM_RADIO, "148", new SwitchPosition[] { new SwitchPosition("-1", "Squelch", BUTTON_7), new SwitchPosition("0", "Off", BUTTON_7), new SwitchPosition("1", "Tone", BUTTON_8, BUTTON_8, "0") }, "VHF FM Radio", "Squelch / Tone", "%1d"));
            #endregion

            #region Secure Voice Panel
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, KY_58, BUTTON_2, "779", BUTTON_1, "778", "1", "0", "1", "Enable", "0", "off", "KY-58 Secure Voice", "Zeroize", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, KY_58, BUTTON_3, "780", "1", "On", "0", "Off", "KY-58 Secure Voice", "Delay", "%1d"));
            AddFunction(Switch.CreateRotarySwitch(this, KY_58, BUTTON_4, "781", "KY-58 Secure Voice", "Radio Select Dial", "%0.1f", "0.0", "C/RAD 1", "0.1", "PLAIN", "0.2", "C/RAD 2"));
            AddFunction(Switch.CreateRotarySwitch(this, KY_58, BUTTON_5, "782", "KY-58 Secure Voice", "Encryption Code Preset", "%0.1f", "0.0", "1", "0.1", "2", "0.2", "3", "0.3", "4", "0.4", "5", "0.5", "6"));
            AddFunction(Switch.CreateRotarySwitch(this, KY_58, BUTTON_6, "783", "KY-58 Secure Voice", "Mode Dial", "%0.1f", "0.0", "Operation", "0.1", "Load", "0.2", "Receive Variable"));
            AddFunction(Switch.CreateToggleSwitch(this, KY_58, BUTTON_7, "784", "1", "On", "0", "Off", "KY-58 Secure Voice", "Power Switch", "%1d"));
            #endregion

            AddFunction(Switch.CreateToggleSwitch(this, AUTOPILOT, BUTTON_30, "772", "1", "On", "0", "Off", "Autopilot", "Emergency Brake", "%1d"));

			#region IFF Panel
			AddFunction(new DualRocker(this, IFF, BUTTON_7, BUTTON_7, BUTTON_7, BUTTON_7, "199", "IFF", "Code", true));
			AddFunction(Switch.CreateRotarySwitch(this, IFF, BUTTON_8, "200", "IFF", "Master Mode", "%0.1f", "0.0", "Off", "0.1", "Standby", "0.2", "Low", "0.3", "Normal", "0.4", "Emergency"));
			AddFunction(new PushButton(this, IFF, BUTTON_17, "795", "IFF", "Reply Button"));
			AddFunction(new PushButton(this, IFF, BUTTON_18, "796", "IFF", "Test Button"));
			AddFunction(Switch.CreateThreeWaySwitch(this, IFF, BUTTON_9, "201", "1", "Audio", "0", "Out", "-1", "Light", "IFF", "Audio Light Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, IFF, BUTTON_10, "202", "1", "Test", "0", "On", "-1", "Out", "IFF", "M-1 Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, IFF, BUTTON_11, "203", "1", "Test", "0", "On", "-1", "Out", "IFF", "M-2 Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, IFF, BUTTON_12, "204", "1", "Test", "0", "On", "-1", "Out", "IFF", "M-3/A Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, IFF, BUTTON_13, "205", "1", "Test", "0", "On", "-1", "Out", "IFF", "M-C Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, IFF, BUTTON_14, "206", "1", "Test", "0", "Out", "-1", "Monitor", "IFF", "RAD Test/Monitor Switch", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, IFF, BUTTON_15, "207", "1", "Ident", "0", "Out", "-1", "Mic", "IFF", "Ident/Mic Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, IFF, BUTTON_16, "208", "1", "On", "0", "Out", "IFF", "IFF On/Out", "%1d"));
            AddFunction(new AbsoluteEncoder(this, IFF, BUTTON_1, BUTTON_1, "209", 0.1d, 0.0d, 0.7d, "IFF", "Mode 1 Wheel 1", true, "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, IFF, BUTTON_2, BUTTON_2, "210", 0.1d, 0.0d, 0.3d, "IFF", "Mode 1 Wheel 2", true, "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, IFF, BUTTON_3, BUTTON_3, "211", 0.1d, 0.0d, 0.7d, "IFF", "Mode 3/A Wheel 1", true, "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, IFF, BUTTON_4, BUTTON_4, "212", 0.1d, 0.0d, 0.7d, "IFF", "Mode 3/A Wheel 2", true, "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, IFF, BUTTON_5, BUTTON_5, "213", 0.1d, 0.0d, 0.7d, "IFF", "Mode 3/A Wheel 3", true, "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, IFF, BUTTON_6, BUTTON_6, "214", 0.1d, 0.0d, 0.7d, "IFF", "Mode 3/A Wheel 4", true, "%0.2f"));
            AddFunction(new FlagValue(this, "798", "IFF", "Reply Lamp", ""));
            AddFunction(new FlagValue(this, "799", "IFF", "Test Lamp", ""));
            #endregion

            #region Emergency Flight Control Panel
            AddFunction(Switch.CreateToggleSwitch(this, AUTOPILOT, BUTTON_15, "174", "1", "On", "0", "Off", "Autopilot", "Speed Brake Emergency Retract", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, AUTOPILOT, BUTTON_16, "175", "1", "Norm", "0", "Override", "Autopilot", "Pitch/Roll Emergency Override", "%1d"));
            AddFunction(new HatSwitch(this, AUTOPILOT, "176", BUTTON_20, "0.4", BUTTON_17, "0.1", BUTTON_18, "0.2", BUTTON_19, "0.3", BUTTON_25, "0.0", "Autopilot", "Emegency Trim Hat", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, AUTOPILOT, BUTTON_21, "177", "-1", "Left", "0", "Off", "1", "Right", "Autopilot", "Alieron Emergency Disengage", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, AUTOPILOT, BUTTON_22, "180", "-1", "Left", "0", "Off", "1", "Right", "Autopilot", "Elevator Emergency Disengage", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, AUTOPILOT, BUTTON_23, "183", "1", "On", "0", "Off", "Autopilot", "Flaps Emergency Retract", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, AUTOPILOT, BUTTON_24, "184", "1", "Normal", "0", "Manual Reversion", "Autopilot", "Manual Reversion Flight Control System Switch", "%1d"));
            AddFunction(new FlagValue(this, "178", "Autopilot", "Left Aileron Disengage Indicator", "Lit when the left aileron is disengaged."));
            AddFunction(new FlagValue(this, "179", "Autopilot", "Right Aileron Disengage Indicator", "Lit when the right aileron is disengaged."));
            AddFunction(new FlagValue(this, "181", "Autopilot", "Left Elevator Disengage Indicator", "Lit when the left elevator is disengaged."));
            AddFunction(new FlagValue(this, "182", "Autopilot", "Right Elevator Disengage Indicator", "Lit when the right elevator is disengaged."));
            #endregion 

            #region Intercomm Panel
            AddFunction(new Axis(this, INTERCOM, BUTTON_2, "221", 0.1d, 0d, 1d, "Intercom", "INT Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_1, "222", "1", "On", "0", "Off", "Intercom", "INT Switch", "%1d"));
            AddFunction(new Axis(this, INTERCOM, BUTTON_4, "223", 0.1d, 0d, 1d, "Intercom", "FM Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_3, "224", "1", "On", "0", "Off", "Intercom", "FM Switch", "%1d"));
            AddFunction(new Axis(this, INTERCOM, BUTTON_6, "225", 0.1d, 0d, 1d, "Intercom", "VHF Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_5, "226", "1", "On", "0", "Off", "Intercom", "VHF Switch", "%1d"));
            AddFunction(new Axis(this, INTERCOM, BUTTON_8, "227", 0.1d, 0d, 1d, "Intercom", "UHF Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_7, "228", "1", "On", "0", "Off", "Intercom", "UHF Switch", "%1d"));
            AddFunction(new Axis(this, INTERCOM, BUTTON_10, "229", 0.1d, 0d, 1d, "Intercom", "AIM Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_9, "230", "1", "On", "0", "Off", "Intercom", "AIM Switch", "%1d"));
            AddFunction(new Axis(this, INTERCOM, BUTTON_12, "231", 0.1d, 0d, 1d, "Intercom", "IFF Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_11, "232", "1", "On", "0", "Off", "Intercom", "IFF Switch", "%1d"));
            AddFunction(new Axis(this, INTERCOM, BUTTON_14, "233", 0.1d, 0d, 1d, "Intercom", "ILS Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_13, "234", "1", "On", "0", "Off", "Intercom", "ILS Switch", "%1d"));
            AddFunction(new Axis(this, INTERCOM, BUTTON_16, "235", 0.1d, 0d, 1d, "Intercom", "TCN Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_15, "236", "1", "On", "0", "Off", "Intercom", "TCN Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, INTERCOM, BUTTON_17, "237", "1", "On", "0", "Off", "Intercom", "Hot Mic Switch", "%1d"));
            AddFunction(new Axis(this, INTERCOM, BUTTON_18, "238", 0.1d, 0d, 1d, "Intercom", "Master Volume"));
            AddFunction(Switch.CreateRotarySwitch(this, INTERCOM, BUTTON_19, "239", "Intercom", "Transmitter Select Dial", "%0.1f",  "0.0", "Intercom", "0.1", "FM", "0.2", "VHF", "0.3", "HF", "0.4", "None"));
            AddFunction(new PushButton(this, INTERCOM, BUTTON_20, "240", "Intercom", "Call Button"));
            #endregion

            #region Stall Warning Panel
            AddFunction(new Axis(this, STALL, BUTTON_1, "704", 0.1d, 0d, 1d, "Stall Warning", "Stall Volume"));
            AddFunction(new Axis(this, STALL, BUTTON_2, "705", 0.1d, 0d, 1d, "Stall Warning", "Peak Volume"));
            #endregion

            #region Aux Landing
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_8, "718", "1", "Up", "0", "Down", "Mechanical", "Auxiliary Landing Gear Handle", "%1d"));
            AddFunction(new PushButton(this, CPT_MECH, BUTTON_9, "722", "Mechanical", "Auxiliary Landing Gear Handle Lock Button"));
            #endregion

            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_10, "733", "1", "Disarmed", "0", "Armed", "Mechanical", "Seat Arm Handle", "%1d"));
        }
     }
}
