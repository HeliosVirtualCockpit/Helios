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
namespace GadrocsWorkshop.Helios.Interfaces.DCS.BlackShark
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;

    public class BlackSharkInterface : DCSInterface
    {
        #region Devices
        private const string ELEC_INTERFACE = "2";
        private const string FUELSYS_INTERFACE = "3";
        private const string ENGINE_INTERFACE = "4";
        private const string HYDRO_SYS_INTERFACE = "5";
        private const string EJECT_SYS_INTERFACE = "6";
        private const string HUD = "7";
        private const string SHKVAL = "8";
        private const string ABRIS = "9";
        private const string LASERRANGER = "11";
        private const string WEAP_INTERFACE = "12";
        private const string VMS = "13";
        private const string SYST_CONTROLLER = "14";
        private const string C061K = "15";
        private const string PVI = "20";
        private const string UV_26 = "22";
        private const string HELMET = "23";
        private const string DATALINK = "25";
        private const string NAV_INTERFACE = "28";
        private const string CLOCK = "29";
        private const string HSI = "30";
        private const string ADI = "31";
        private const string AUTOPILOT = "33";
        private const string CPT_MECH = "34";
        private const string LASER_WARNING_SYSTEM = "36";
        private const string PPK = "32";
        private const string RADAR_ALTIMETER = "38";
        private const string FIRE_EXTING_INTERFACE = "40";
        private const string MISC_SYSTEMS_INTERFACE = "41";
        private const string IFF = "42";
        private const string SPOTLIGHT_SYSTEM = "44";
        private const string NAVLIGHT_SYSTEM = "45";
        private const string ARK_22 = "46";
        private const string R_800 = "48";
        private const string R_828 = "49";
        private const string SPU_9 = "50";
        private const string ILLUMINATION_INTERFACE = "51";
        private const string SIGNAL_FLARE_DISPENSER = "52";
        private const string STBY_ADI = "55";
        private const string PShk_7 = "57";
        private const string ZMS_3 = "58";
        private const string K041 = "59";
        private const string ACCELEROMETER = "61";
        private const string AVIONICS_PROXY = "62";
        private const string CVM_MEMORY = "63";

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
        private const string BUTTON_68 = "3068";
        private const string BUTTON_69 = "3069";
        #endregion

        protected BlackSharkInterface(string heliosName, string dcsVehicleName, string exportFunctionsUri)
            : base(heliosName, dcsVehicleName, exportFunctionsUri)
        {
            #region Left Front Panel
            // Master Caution
            AddFunction(new IndicatorPushButton(this, SYST_CONTROLLER, BUTTON_1, "44", "System Controller", "Master Caution"));
            
            // Caution Lights
            AddFunction(new IndicatorPushButton(this, SYST_CONTROLLER, BUTTON_3, "46", "Caution", "Rotor RPM Indicator"));
            AddFunction(new FlagValue(this, "47", "Caution", "Under Fire Indicator", ""));
            AddFunction(new FlagValue(this, "48", "Caution", "Lower Gear Indicator", ""));
            AddFunction(new FlagValue(this, "78", "Caution", "Left Engine Max RPM Indicator", ""));
            AddFunction(new FlagValue(this, "79", "Caution", "Right Engine Max RPM Indicator", ""));
            AddFunction(new FlagValue(this, "80", "Caution", "Ny Max", ""));
            AddFunction(new FlagValue(this, "81", "Caution", "Left Engine Vibration Indicator", ""));
            AddFunction(new FlagValue(this, "82", "Caution", "Right Engine Vibration Indicator", ""));
            AddFunction(new FlagValue(this, "83", "Caution", "IAS Max Indicator", ""));
            AddFunction(new FlagValue(this, "84", "Caution", "Main Transmission Warning Indicator", ""));
            AddFunction(new FlagValue(this, "85", "Caution", "Fire Indicator", ""));
            AddFunction(new FlagValue(this, "86", "Caution", "IFF Failure Indicator", ""));

            // VVI
            AddFunction(new ScaledNetworkValue(this, "24", 30d, "VVI", "vertical velocity", "Current vertical velocity displayed on the VVI.", "(-30 to 30)", BindingValueUnits.MetersPerSecond));

            // ADI
            AddFunction(new ScaledNetworkValue(this, "100", 180d, "ADI", "roll", "Current roll displayed on the ADI.", "(-180 to 180)", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "101", -90d, "ADI", "pitch", "Current pitch displayed on the ADI.", "(-90 to 90)", BindingValueUnits.Degrees));
            AddFunction(new FlagValue(this, "102", "ADI", "steering flag", "Indicates whether the steering flag is displayed on the ADI."));
            AddFunction(new FlagValue(this, "109", "ADI", "malfunction flag", "Indicates whether the attitude flag is displayed on the ADI."));
            AddFunction(new NetworkValue(this, "107", "ADI", "bank deviation", "Current amount of bank steering displayed on the ADI.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "106", "ADI", "pitch deviation", "Current amount of pitch steering displayed on the ADI.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "111", "ADI", "air speed deviation", "Current amount of deviation from assigned air speed displayed on the ADI.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "103", "ADI", "lateral deviation", "Current amount of deviation from assigned track displayed on the ADI.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "526", "ADI", "altitude deviation", "Current amount of deviation from assigned altitude displayed on the ADI.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "108", "ADI", "side slip", "Current amount of side slip displayed on the ADI.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new RotaryEncoder(this, ADI, BUTTON_1, "105", 0.001, "ADI", "Zero Pitch Trim"));
            AddFunction(new PushButton(this, ADI, BUTTON_3, "110", "ADI", "Test"));

            // Barometric Altimeter
            AddFunction(new ScaledNetworkValue(this, "87", 10000d, "Barometric Altimeter", "Altitude", "Barometric altitude.", "(0-10000)", BindingValueUnits.Meters));
            AddFunction(new ScaledNetworkValue(this, "88", 200d, "Barometric Altimeter", "QFE Pressure", "", "600-800", BindingValueUnits.MilimetersOfMercury, 600d, "%0.2f"));
            AddFunction(new ScaledNetworkValue(this, "89", 10000d, "Barometric Altimeter", "Commanded Altitude", "Commanded altitude.", "(0-10000)", BindingValueUnits.Meters));
            AddFunction(new Axis(this, MISC_SYSTEMS_INTERFACE, BUTTON_5, "90", 0.001, 0, 1, "Barometric Altimeter", "Baro pressure QFE knob"));


            // HSI
            AddFunction(new ScaledNetworkValue(this, "112", 360d, "HSI", "Heading", "Current heading displayed on the HSI", "(0-360)", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "118", 360d, "HSI", "Commaned Course", "Current commanded course on the HSI", "(0-360)", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "124", 360d, "HSI", "Commanded Heading", "Current commanded heading on the HSI", "(0-360)", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "115", 360d, "HSI", "Bearing", "Current bearing displayed on the HSI", "(0-360)", BindingValueUnits.Degrees));
            AddFunction(new FlagValue(this, "119", "HSI", "Heading Warning Flag", "Heading flag indicator."));
            AddFunction(new FlagValue(this, "114", "HSI", "Course Warning Flag", "Course warning flag indicator."));
            AddFunction(new FlagValue(this, "125", "HSI", "Glide Warning Flag", "Glide warning flag indicator."));
            AddFunction(new Functions.HSIRange(this));
            AddFunction(new NetworkValue(this, "127", "HSI", "Longitudinal deviation", "Longitudinal deviation", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "128", "HSI", "Lateral deviation", "Lateral deviation", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new FlagValue(this, "116", "HSI", "Range flag", "Range unavailable flag"));
            AddFunction(new FlagValue(this, "121", "HSI", "Course flag", "Course unavailable flag"));
            AddFunction(new RotaryEncoder(this, HSI, BUTTON_1, "126", 0.001d, "HSI", "Commanded course rotary"));
            AddFunction(new RotaryEncoder(this, HSI, BUTTON_2, "129", 0.001d, "HSI", "Commanded heading rotary"));
            AddFunction(new PushButton(this, HSI, BUTTON_3, "113", "HSI", "Test button"));
            AddFunction(Switch.CreateToggleSwitch(this, HSI, BUTTON_4, "54", "1", "Manual", "0", "Auto", "HSI", "HSI DTA Desired Heading", "%1d"));

            // Laser RangeFinder
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, LASERRANGER, BUTTON_3, "56", BUTTON_2, "57", "1", "0", "0", "Norm", "1", "Stand By", "Laser Ranger", "Mode Switch", "%1d"));
            AddFunction(new PushButton(this, LASERRANGER, BUTTON_4, "55", "Laser Ranger", "Designator Reset"));

            // Blade Angle
            AddFunction(new ScaledNetworkValue(this, "53", 15d, "Rotor", "Pitch", "Current pitch of the rotor blades", "(0-15)", BindingValueUnits.Degrees));

            // Rotor RPM
            AddFunction(new ScaledNetworkValue(this, "52", 110d, "Rotor", "RPM", "Rotor RPM", "(0-110)", BindingValueUnits.RPMPercent));

            // Radar Altimeter
            CalibrationPointCollectionDouble radarAltScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 350d);
            //radarAltScale.Add(new CalibrationPointDouble(0.1838d, 20d));
            //radarAltScale.Add(new CalibrationPointDouble(0.4631d, 50d));
            //radarAltScale.Add(new CalibrationPointDouble(0.7541d, 150d));
            //radarAltScale.Add(new CalibrationPointDouble(0.8330d, 200d));
            //radarAltScale.Add(new CalibrationPointDouble(0.9329d, 300d));
            radarAltScale.Add( new CalibrationPointDouble( 0.0d, 0d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.095d, 10d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.181d, 20d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.264d, 30d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.345d, 40d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.427d, 50d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.510d, 80d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.570d, 100d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.659d, 130d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.716d, 150d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.802d, 200d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.875d, 250d ) );
            radarAltScale.Add( new CalibrationPointDouble( 0.9329d, 300d ) );
            radarAltScale.Add( new CalibrationPointDouble( 1.0d, 350d ) );

            AddFunction(new ScaledNetworkValue(this, "94", radarAltScale, "Radar Altimeter", "Altitude", "", "0-350", BindingValueUnits.Meters));
            AddFunction(new ScaledNetworkValue(this, "93", radarAltScale, "Radar Altimeter", "Dangerouse Altitude Index", "", "0-350", BindingValueUnits.Meters));
            AddFunction(new FlagValue(this, "95", "Radar Altimeter", "Warning Flag", "Flag displayed when radar altimeter is not functioning."));
            AddFunction(new RotaryEncoder(this, RADAR_ALTIMETER, BUTTON_1, "91", 0.1d, "Radar Altimeter", "Dangerous RALT set rotary"));
            AddFunction(new FlagValue(this, "92", "Radar Altimeter", "Dangerous altitude indicator", ""));
            AddFunction(new PushButton(this, RADAR_ALTIMETER, BUTTON_2, "96", "Radar Altimeter", "Test Button"));

            // Indicator Air Speed
            AddFunction(new ScaledNetworkValue(this, "51", 350d, "IAS", "Indicated Airspeed", "Airspeed gauge", "", BindingValueUnits.KilometersPerHour));

            // Accelerometer
            AddFunction(new ScaledNetworkValue(this, "97", 6d, "Accelerometer", "Acceleration", "Current gs", "", BindingValueUnits.Numeric, -2d, "%0.2f"));
            AddFunction(new ScaledNetworkValue(this, "98", 6d, "Accelerometer", "Maxium acceleration", "Max Gs attained.", "", BindingValueUnits.Numeric, -2d, "%0.2f"));
            AddFunction(new ScaledNetworkValue(this, "99", 6d, "Accelerometer", "Minimum acceleration", "Min Gs attained.", "", BindingValueUnits.Numeric, -2d, "%0.2f"));
            AddFunction(new PushButton(this, ACCELEROMETER, BUTTON_1, "572", "Accelerometer", "Reset"));

            // Lamp Test Button
            AddFunction(new PushButton(this, SYST_CONTROLLER, BUTTON_2, "45", "System Controller", "Lamp Test"));

            // Clock
            AddFunction(new ScaledNetworkValue(this, "68", 12d, "Clock", "Current Time Hours", "Current hour of the simulation time of day.", "", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "69", 60d, "Clock", "Current Time Minutes", "Current minute of the simulation time of day.", "", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "70", 60d, "Clock", "Current Time Seconds", "Current seconds of the simulation time of day.", "", BindingValueUnits.Numeric));
            AddFunction(new FlagValue(this, "75", "Clock", "Flight Time Indicator", "Indicator light determining whether flight time is engaged."));
            AddFunction(new ScaledNetworkValue(this, "72", 12d, "Clock", "Flight Time Hours", "Current hour of the simulation flight time.", "", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "531", 60d, "Clock", "Flight Time Minutes", "Current minute of the simulation flight time.", "", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "73", 30d, "Clock", "Stop Watch Minutes", "Current minute of the stop watch.", "", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "532", 30d, "Clock", "Stop Watch Seconds", "Current seconds of the stop watch.", "", BindingValueUnits.Numeric));
            AddFunction(new Switch(this, CLOCK, "76", new SwitchPosition[] { new SwitchPosition("1.0", "pushed", "3001"), new SwitchPosition("0.0", "normal", "3001"), new SwitchPosition("-1.0", "pulled", "3002") }, "Clock", "Mech clock left Button", "%0.1f"));
            AddFunction(new RotaryEncoder(this, CLOCK, BUTTON_3, "511", 0.001d, "Clock", "Mech clock left lever"));
            AddFunction(new PushButton(this, CLOCK, BUTTON_4, "77", "Clock", "Mech clock right Button"));
            AddFunction(new RotaryEncoder(this, CLOCK, BUTTON_5, "525", 0.001d, "Clock", "Mech clock right lever"));




            #endregion

            #region Right Front Panel
            // EKRAN
            AddFunction(new Text(this, "2004", "EKRAN", "Message", "EKRAN Message Text"));
            AddFunction(new Text(this, "2006", "EKRAN", "Queue message", "Queue window Message Text"));
            AddFunction(new Text(this, "2007", "EKRAN", "Failure message", "Failure window Message Text"));
            AddFunction(new Text(this, "2008", "EKRAN", "Memory message", "Memory window Message Text"));

            // Backup ADI
            AddFunction(new ScaledNetworkValue(this, "142", 180d, "Backup ADI", "roll", "Current roll displayed on the AGR-81 backup ADI.", "(-180 to 180)", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "143", -90d, "Backup ADI", "pitch", "Current pitch displayed on the AGR-81 backup ADI.", "(-90 to 90)", BindingValueUnits.Degrees));
            AddFunction(new NetworkValue(this, "144", "Backup ADI", "side slip", "Current amount of side slip displayed on the AGR-81 backup ADI.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "599", "Backup ADI", "LongitudinalDeviationBar", "Current amount of Longitudinal Deviation Bar.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "613", "Backup ADI", "LateralDeviationBar", "Current amount of Lateral Deviation Bar.", "(-1 to 1)", BindingValueUnits.Numeric));
            AddFunction(new FlagValue(this, "145", "Backup ADI", "warning flag", "Indicates whether the warning flag is displayed on the AGR-81 backup ADI."));
            AddFunction(new RotaryEncoder(this, STBY_ADI, BUTTON_3, "597", 1d, "Backup ADI", "Cage"));
            AddFunction(Switch.CreateToggleSwitch(this, STBY_ADI, BUTTON_4, "230", "1", "On", "0", "Off", "Backup ADI", "Power", "%1d"));
            AddFunction(new PushButton(this, STBY_ADI, BUTTON_1, "141", "Backup ADI", "Test backup ADI Button"));

            // Exhaust Gas Test Buttons
            AddFunction(new PushButton(this, ENGINE_INTERFACE, BUTTON_22, "131", "Engine", "Running EGT Test Button"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, BUTTON_23, "132", "Engine", "Stopped EGT Test Button"));

            // Exhaust Gas Gauge
            AddFunction(new ScaledNetworkValue(this, "133", 1200d, "Engine", "Left Engine EGT", "Left engine exhaust gas temperature.", "(0-1200)", BindingValueUnits.Celsius));
            AddFunction(new ScaledNetworkValue(this, "134", 1200d, "Engine", "Right Engine EGT", "Right engine exhaust gas temperature.", "(0-1200)", BindingValueUnits.Celsius));

            // Engine RPM
            AddFunction(new ScaledNetworkValue(this, "135", 110d, "Engine", "Left Engine RPM", "Left engine RPM.", "(0-110%) 100% = 19,537RPM", BindingValueUnits.RPMPercent));
            AddFunction(new ScaledNetworkValue(this, "136", 110d, "Engine", "Right Engine RPM", "Right engine RPM.", "(0-110%) 100% = 19,537RPM", BindingValueUnits.RPMPercent));

            // Fuel Gauge
            AddFunction(new ScaledNetworkValue(this, "138", 800d, "Fuel System", "Forward Tank Fuel Quantity", "Forward tank quantity.", "Max 705kg", BindingValueUnits.Kilograms));
            AddFunction(new ScaledNetworkValue(this, "137", 800d, "Fuel System", "Rear Tank Fuel Quantity", "Rear tank quantity.", "Max 745kg", BindingValueUnits.Kilograms));
            AddFunction(new FlagValue(this, "139", "Fuel System", "Forward Tank Lamp", "Forward Tank Indicator Lamp"));
            AddFunction(new FlagValue(this, "140", "Fuel System", "Rear Tank Lamp", "Rear Tank Indicator Lamp"));
            AddFunction(new PushButton(this, FUELSYS_INTERFACE, BUTTON_14, "616", "Fuel System", "Self Test Button"));

            // ABRIS
            AddFunction(new PushButton(this, ABRIS, BUTTON_1, "512", "ABRIS", "Button 1"));
            AddFunction(new PushButton(this, ABRIS, BUTTON_2, "513", "ABRIS", "Button 2"));
            AddFunction(new PushButton(this, ABRIS, BUTTON_3, "514", "ABRIS", "Button 3"));
            AddFunction(new PushButton(this, ABRIS, BUTTON_4, "515", "ABRIS", "Button 4"));
            AddFunction(new PushButton(this, ABRIS, BUTTON_5, "516", "ABRIS", "Button 5"));
            AddFunction(new PushButton(this, ABRIS, BUTTON_7, "523", "ABRIS", "Cursor Select"));
            AddFunction(new RotaryEncoder(this, ABRIS, BUTTON_6, "518", 0.04, "ABRIS", "Cursor"));
            AddFunction(new Axis(this, ABRIS, BUTTON_8, "517", 0.05, 0, 1, "ABRIS", "Brightness"));
            AddFunction(new Switch(this, ABRIS, "130", new SwitchPosition[] { new SwitchPosition("1.0", "On", "3009"), new SwitchPosition("0.0", "Off", "3009") }, "ABRIS", "Power", "%0.1f"));
            #endregion

            #region Center Panel
            // HUD
            AddFunction(new Axis(this, HUD, BUTTON_1, "8", 0.05d, 0d, 1d, "HUD", "Brightness"));
            AddFunction(new Switch(this, HUD, "9", new SwitchPosition[] { new SwitchPosition("-1", "Night", BUTTON_2), new SwitchPosition("0", "Day", BUTTON_2), new SwitchPosition("1", "Standby", BUTTON_2) }, "HUD", "Modes", "%1d"));
            AddFunction(new PushButton(this, HUD, BUTTON_3, "7", "HUD", "Test"));
            AddFunction(new Switch(this, HUD, "510", new SwitchPosition[] { new SwitchPosition("1", "On", "3004"), new SwitchPosition("0", "Off", "3004") }, "HUD", "Filter", "%0.1f"));

            // PUI-800
            AddFunction(new Switch(this, WEAP_INTERFACE, "387", new SwitchPosition[] { new SwitchPosition("1", "Armed", BUTTON_1), new SwitchPosition("0", "Disarmed", BUTTON_1) }, "PUI-800", "Master Arm", "%1d"));
            AddFunction(new PushButton(this, WEAP_INTERFACE, BUTTON_3, "402", "PUI-800", "External Stores Jettison"));
            AddFunction(new Switch(this, WEAP_INTERFACE, "396", new SwitchPosition[] { new SwitchPosition("1", "Armed", "3022"), new SwitchPosition("0", "Disarmed", "3022") }, "PUI-800", "Jettison Arm Mode", "%1d"));
            AddFunction(new Switch(this, WEAP_INTERFACE, "403", new SwitchPosition[] { new SwitchPosition("1", "Manual", "3005"), new SwitchPosition("0", "Auto", "3005") }, "PUI-800", "Weapon Control Mode", "%1d"));
            AddFunction(new Switch(this, WEAP_INTERFACE, "399", new SwitchPosition[] { new SwitchPosition("1", "HE", "3006"), new SwitchPosition("0", "AP", "3006") }, "PUI-800", "Cannon Round Select", "%1d"));
            AddFunction(new Switch(this, WEAP_INTERFACE, "400", new SwitchPosition[] { new SwitchPosition("0.2", "Long", "3004"), new SwitchPosition("0.1", "Medium", "3004"), new SwitchPosition("0.0", "Short", "3004") }, "PUI-800", "Weapon Burst Length", "%0.1f"));
            AddFunction(new Switch(this, WEAP_INTERFACE, "398", new SwitchPosition[] { new SwitchPosition("1", "Low", "3020"), new SwitchPosition("0", "High", "3020") }, "PUI-800", "Cannon Rate of Fire", "%1d"));
            AddFunction(new PushButton(this, WEAP_INTERFACE, BUTTON_21, "397", "PUI-800", "Emergency ATGM launch"));
            AddFunction(new FlagValue(this, "392", "PUI-800", "Station 1 Present Lamp", ""));
            AddFunction(new FlagValue(this, "393", "PUI-800", "Station 2 Present Lamp", ""));
            AddFunction(new FlagValue(this, "394", "PUI-800", "Station 3 Present Lamp", ""));
            AddFunction(new FlagValue(this, "395", "PUI-800", "Station 4 Present Lamp", ""));
            AddFunction(new FlagValue(this, "388", "PUI-800", "Station 1 Ready Lamp", ""));
            AddFunction(new FlagValue(this, "389", "PUI-800", "Station 2 Ready Lamp", ""));
            AddFunction(new FlagValue(this, "390", "PUI-800", "Station 3 Ready Lamp", ""));
            AddFunction(new FlagValue(this, "391", "PUI-800", "Station 4 Ready Lamp", ""));
            AddFunction(new NetworkValue(this, "2001", "PUI-800", "Selected Station Type", "Indicates what type of weapon is on the currently selected stations", "", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2002", "PUI-800", "Selected Station Count", "Indicates the number of remaining stores on the currently selected stations", "", BindingValueUnits.Numeric, null));
            AddFunction(new NetworkValue(this, "2003", "PUI-800", "Cannon Rounds", "Indicates the number of remaining cannon rounds for the currently selected ammo type", "", BindingValueUnits.Numeric, null));

            // Targeting Display Control Panel
            AddFunction(new Switch(this, SHKVAL, "404", new SwitchPosition[] { new SwitchPosition("1", "Black", BUTTON_1), new SwitchPosition("0", "White", BUTTON_1) }, "Shkval", "Shkval Inidication Polarity", "%1d"));
            AddFunction(new Axis(this, SHKVAL, BUTTON_2, "406", 0.05d, 0d, 1d, "Shkval", "Brightness"));
            AddFunction(new Axis(this, SHKVAL, BUTTON_3, "407", 0.05d, 0d, 1d, "Shkval", "Contrast"));
            AddFunction(new Axis(this, HELMET, BUTTON_1, "405", 0.05d, 0d, 1d, "HMS", "Brightness"));
            AddFunction(new Switch(this, WEAP_INTERFACE, "408", new SwitchPosition[] { new SwitchPosition("0.0", "1", "3007"), new SwitchPosition("0.1", "2", "3007"), new SwitchPosition("0.2", "3", "3007") }, "Laser", "Laser Code", "%0.1f"));
            AddFunction(new Switch(this, MISC_SYSTEMS_INTERFACE, "409", new SwitchPosition[] { new SwitchPosition("1", "Declutter", BUTTON_1), new SwitchPosition("0", "Full Data", BUTTON_1) }, "Misc", "HUD/TV Declutter", "%1d"));

            // Landing Lights and Voice Warning Control Panel
            AddFunction(new Switch(this, SPOTLIGHT_SYSTEM, "382", new SwitchPosition[] { new SwitchPosition("1.0", "On", BUTTON_1), new SwitchPosition("0.5", "Off", BUTTON_1), new SwitchPosition("0.0", "Retract", BUTTON_1) }, "Landing Light", "On/Off/Retract", "%0.1f"));
            AddFunction(new Switch(this, SPOTLIGHT_SYSTEM, "383", new SwitchPosition[] { new SwitchPosition("0", "Backup", BUTTON_2), new SwitchPosition("1", "Primary", BUTTON_2) }, "Landing Light", "Primary/Backup Select", "%1d"));
            AddFunction(new Switch(this, ARK_22, "381", new SwitchPosition[] { new SwitchPosition("1.0", "Inner", BUTTON_3), new SwitchPosition("0.5", "Auto", BUTTON_3), new SwitchPosition("0.0", "Outer", BUTTON_3) }, "ARK 22", "NDB Select", "%0.2f"));
            AddFunction(new PushButton(this, VMS, BUTTON_1, "384", "VMS", "Cease Message"));
            AddFunction(new PushButton(this, VMS, BUTTON_3, "385", "VMS", "Repeat Message"));
            AddFunction(new Switch(this, VMS, "386", new SwitchPosition[] { new SwitchPosition("1", "Normal", BUTTON_2), new SwitchPosition("0", "Emergency", BUTTON_2) }, "VMS", "Emergency Mode Switch", "%0.1f"));
            AddFunction(new PushButton(this, VMS, BUTTON_4, "442", "VMS", "BIT"));
            #endregion

            #region Landing Gear Panel
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_1, "65", "0", "Up", "1", "Down", "Mechanical", "Gear Lever", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, CPT_MECH, BUTTON_4, "66", BUTTON_5, "67", "1", "0", "0", "Common", "1", "Main", "Mechanical", "Emergency Gear Hydraulic Selector", "%1d"));
            AddFunction(new FlagValue(this, "63", "Mechanical", "Nose Gear Up Indicator", "Lit when the nose gear is in the up position."));
            AddFunction(new FlagValue(this, "64", "Mechanical", "Nose Gear Down Indicator", "Lit whent henose gear is extended."));
            AddFunction(new FlagValue(this, "61", "Mechanical", "Right Gear Up Indicator", "Lit when the nose gear is in the up position."));
            AddFunction(new FlagValue(this, "62", "Mechanical", "Right Gear Down Indicator", "Lit whent henose gear is extended."));
            AddFunction(new FlagValue(this, "59", "Mechanical", "Left Gear Up Indicator", "Lit when the nose gear is in the up position."));
            AddFunction(new FlagValue(this, "60", "Mechanical", "Left Gear Down Indicator", "Lit whent henose gear is extended."));
            #endregion

            #region Overhead Panel Left
            AddFunction(new Switch(this, NAVLIGHT_SYSTEM, "146", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_4), new SwitchPosition("0.1", "10%", BUTTON_4), new SwitchPosition("0.2", "30%", BUTTON_4), new SwitchPosition("0.3", "100%", BUTTON_4), new SwitchPosition("0.4", "Signal", BUTTON_5, BUTTON_5, "0.4") }, "Navigation Light System", "Brightness", "%0.1f"));
            AddFunction(new Switch(this, CPT_MECH, "147", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_7), new SwitchPosition("0.1", "Fast", BUTTON_7), new SwitchPosition("0.2", "Speed 1", BUTTON_7), new SwitchPosition("0.3", "Speed 2", BUTTON_7), new SwitchPosition("0.4", "Return", BUTTON_8, BUTTON_8, "0.4") }, "Mechanical", "Windshield Wiper Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_8, "539", "1", "On", "0", "Off", "Mechanical", "Pitot Static Port and AoA Heat Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, CPT_MECH, BUTTON_9, "151", "1", "On", "0", "Off", "Mechanical", "Pitot Ram Air and Clock Heat Switch", "%1d"));
            AddFunction(new FlagValue(this, "170", "Left Warning Panel", "R-Alt Hold Indicator", "Radar altitude-hold autopilot mode on"));
            AddFunction(new FlagValue(this, "175", "Left Warning Panel", "Auto Hover", "Hover autopilot mode is on"));
            AddFunction(new FlagValue(this, "172", "Left Warning Panel", "Auto Descent", "Controlled descent autopilot mode is on"));
            AddFunction(new FlagValue(this, "165", "Left Warning Panel", "ENR Nav On", "Route navigation with direct flight to steerpoint is enabled"));
            AddFunction(new FlagValue(this, "171", "Left Warning Panel", "ENR Course", "Route navigation with course following is enabled"));
            AddFunction(new FlagValue(this, "176", "Left Warning Panel", "Next WP", "Notification of passing one waypoint and advancing on to the next"));
            AddFunction(new FlagValue(this, "166", "Left Warning Panel", "ENR End", "Last waypoint reached notification; end of fligh plan"));
            AddFunction(new FlagValue(this, "164", "Left Warning Panel", "AC-POS Cal. Data", "Aircraft position is roughly calculated using air data systems information"));
            AddFunction(new FlagValue(this, "178", "Left Warning Panel", "Weap. Arm", "Weapons armed"));
            AddFunction(new FlagValue(this, "173", "Left Warning Panel", "Cannon", "Cannon has been slewed away from boresight position."));
            AddFunction(new FlagValue(this, "177", "Left Warning Panel", "Cannon <>", "Cannon has been slewed downward away from boresight position"));
            AddFunction(new FlagValue(this, "211", "Left Warning Panel", "X-Feed VLV Open", "Fuel is shared between tanks (crossfeed on)"));
            AddFunction(new FlagValue(this, "187", "Left Warning Panel", "Turbo Gear", "Accessory gearbox disconnected from rotor drive"));
            AddFunction(new FlagValue(this, "204", "Left Warning Panel", "AGB Oil Press", "Gearbox oil pressure normal (before start)"));
            AddFunction(new FlagValue(this, "213", "Left Warning Panel", "SL Hook Open", "Sling load lock (hook) is open"));
            AddFunction(new ScaledNetworkValue(this, "587", 60d, "Misc", "Ambien Temperature", "Current ambient temperature", "-60 - 50", BindingValueUnits.Celsius));

            #endregion

            #region Magnetic Compass
            AddFunction(new ScaledNetworkValue(this, "11", 360d, "Magnetic Compass", "Course", "Current course displayed on the magnetic compass.", "-360 - 360", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "12", 90d, "Magnetic Compass", "Pitch", "Current pitch of the magnetic compass card.", "-90 - 90", BindingValueUnits.Degrees));
            AddFunction(new ScaledNetworkValue(this, "14", 180d, "Magnetic Compass", "Bank", "Current bank displayed on the magnetic compass card.", "-180 - 180", BindingValueUnits.Degrees));
            #endregion

            #region Overhad Panel Right
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, BUTTON_13, "153", "1", "On", "0", "Off", "Engine", "Rotor De-icing system", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ENGINE_INTERFACE, BUTTON_14, "154", "1.0", "De-ice", "0.5", "Off", "0.0", "Dust Protection", "Engine", "Enginges De-icing / dust-protection system", "%0.1f"));
            AddFunction(new PushButton(this, CPT_MECH, BUTTON_11, "156", "Mechanical", "Pitot heat system test."));
            AddFunction(new FlagValue(this, "167", "Right Warning Panel", "Master Arm On", "Master is is on"));
            AddFunction(new FlagValue(this, "180", "Right Warning Panel", "Weapon Training", "Training mode for guided weapons is on."));
            AddFunction(new FlagValue(this, "179", "Right Warning Panel", "HMS Fail", "Helment-Mounted Sight malfunction detected."));
            AddFunction(new FlagValue(this, "188", "Right Warning Panel", "HUD Not Ready", "HUD failure or not ready"));
            AddFunction(new FlagValue(this, "189", "Right Warning Panel", "Computer Diagnose", "Onboard computers running in diagnostic mode"));
            AddFunction(new FlagValue(this, "206", "Right Warning Panel", "Computer Fail", "Failure of one or more central computers"));
            AddFunction(new FlagValue(this, "212", "Right Warning Panel", "Inverter On", "Electrical DC/AC inverter is on"));
            AddFunction(new FlagValue(this, "205", "Right Warning Panel", "Shkval Fail", "Shkval targeting system failure detected"));
            AddFunction(new FlagValue(this, "181", "Right Warning Panel", "LH Eng Anti-Ice", "Left enginge de-icing active"));
            AddFunction(new FlagValue(this, "190", "Right Warning Panel", "LH Eng Dust-Prot", "Left enginge dust protection is active"));
            AddFunction(new FlagValue(this, "207", "Right Warning Panel", "LH Power Set Lim", "Left engine has over-speed and was limited by electronic engine governor"));
            AddFunction(new FlagValue(this, "183", "Right Warning Panel", "Rotor Anti-Ice", "Rotor de-icing is active"));
            AddFunction(new FlagValue(this, "182", "Right Warning Panel", "RH Eng Anti-Ice", "Right enginge de-icing active"));
            AddFunction(new FlagValue(this, "191", "Right Warning Panel", "RH Eng Dust-Prot", "Right enginge dust protection is active"));
            AddFunction(new FlagValue(this, "208", "Right Warning Panel", "RH Power Set Lim", "Right engine has over-speed and was limited by electronic engine governor"));
            AddFunction(new FlagValue(this, "184", "Right Warning Panel", "Windshield Heater On", "Windshield heater is on"));
            AddFunction(new FlagValue(this, "200", "Right Warning Panel", "Fwd Tank Pump On", "Forward fuel tank has pressure"));
            AddFunction(new FlagValue(this, "209", "Right Warning Panel", "LH VLV Closed", "Left engine fuel valve is closed"));
            AddFunction(new FlagValue(this, "185", "Right Warning Panel", "LH Outer Tank Pump", "Left outer fuel tank has pressure"));
            AddFunction(new FlagValue(this, "202", "Right Warning Panel", "LH Inner Tank Pump", "Left inner fueld tank has pressure"));
            AddFunction(new FlagValue(this, "201", "Right Warning Panel", "Aft Tank Pump On", "Aft fuel tank has pressure"));
            AddFunction(new FlagValue(this, "210", "Right Warning Panel", "RH VLV Closed", "Right engine fuel valve is closed"));
            AddFunction(new FlagValue(this, "186", "Right Warning Panel", "RH Outer Tank Pump", "Right outer fuel tank has pressure"));
            AddFunction(new FlagValue(this, "203", "Right Warning Panel", "RH Inner Tank Pump", "Right inner fueld tank has pressure"));
            #endregion

            #region Data Link
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_1, "159", "Datalink", "Send/Memory"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_2, "150", "Datalink", "Ingress to Target"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_3, "161", "Datalink", "Erase"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_4, "15", "Datalink", "Blank Button"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_5, "16", "Datalink", "To All"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_6, "17", "Datalink", "To Wingman 1"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_7, "18", "Datalink", "To Wingman 2"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_8, "19", "Datalink", "To Wingman 3"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_9, "20", "Datalink", "To Wingman 4"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_10, "21", "Datalink", "Target #1/Vehicle"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_11, "22", "Datalink", "Target #2/SAM"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_12, "23", "Datalink", "Target #3/Other"));
            AddFunction(new IndicatorPushButton(this, DATALINK, BUTTON_13, "50", "Datalink", "Ingress Point"));
            #endregion

            #region Laser Warning Receiver
            AddFunction(new FlagValue(this, "25", "Laser Warning Receiver", "Bearing Forward", "On when laser energy is detect from forwad of the aircaft"));
            AddFunction(new FlagValue(this, "28", "Laser Warning Receiver", "Bearing Right", "On when laser energy is detect from right of the aircaft"));
            AddFunction(new FlagValue(this, "26", "Laser Warning Receiver", "Bearing Aft", "On when laser energy is detect from aft of the aircaft"));
            AddFunction(new FlagValue(this, "27", "Laser Warning Receiver", "Bearing Left", "On when laser energy is detect from left of the aircaft"));
            AddFunction(new FlagValue(this, "31", "Laser Warning Receiver", "Hemisphere Above", "On when laser energy is detect from above of the aircaft"));
            AddFunction(new FlagValue(this, "32", "Laser Warning Receiver", "Hemisphere Below", "On when laser energy is detect from below of the aircaft"));
            AddFunction(new FlagValue(this, "33", "Laser Warning Receiver", "Range Finder", "On if laster is of sufficient strength for range"));
            AddFunction(new FlagValue(this, "34", "Laser Warning Receiver", "Guidance", "On when laser energy is detected to being guided to the aircraft"));
            AddFunction(Switch.CreateToggleSwitch(this, LASER_WARNING_SYSTEM, BUTTON_2, "583", "1", "On", "0", "Off", "Laser Warning Receiver", "Power", "%1d"));
            AddFunction(new PushButton(this, LASER_WARNING_SYSTEM, BUTTON_3, "584", "Laser Warning Receiver", "Self Test Button"));
            AddFunction(new FlagValue(this, "582", "Laser Warning Receiver", "Ready Lamp", "Lit when LWS is warmed up and operating."));
            #endregion

            #region Countermeasures
            AddFunction(Switch.CreateThreeWaySwitch(this, UV_26, BUTTON_1, "36", "0.0", "Left", "0.1", "Both", "0.2", "Right", "UV-26 CMD", "Release Select Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, UV_26, BUTTON_3, "37", "0.0", "Remaining", "0.1", "Program", "UV-26 CMD", "Display Switch", "%0.1f"));
            AddFunction(new PushButton(this, UV_26, BUTTON_4, "38", "UV-26 CMD", "Num of sequences"));
            AddFunction(new PushButton(this, UV_26, BUTTON_5, "39", "UV-26 CMD", "Num in sequence"));
            AddFunction(new PushButton(this, UV_26, BUTTON_6, "41", "UV-26 CMD", "Dispence interval"));
            AddFunction(new PushButton(this, UV_26, BUTTON_7, "43", "UV-26 CMD", "Start dispense"));
            AddFunction(new PushButton(this, UV_26, BUTTON_8, "42", "UV-26 CMD", "Reset to default program"));
            AddFunction(new PushButton(this, UV_26, BUTTON_9, "40", "UV-26 CMD", "Stop dispense"));
            AddFunction(new FlagValue(this, "541", "UV-26 CMD", "Left Dispense Lamp", "Lit when set to dispense from left dispenser"));
            AddFunction(new FlagValue(this, "542", "UV-26 CMD", "Right Dispense Lamp", "Lit when set to dispense from right dispenser"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, UV_26, BUTTON_10, "496", BUTTON_12, "497", "1", "0", "1", "On", "0", "Off", "UV-26 CMD", "Power", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, UV_26, BUTTON_11, "498", BUTTON_13, "499", "1", "0", "1", "On", "0", "Off", "UV-26 CMD", "Test", "%1d"));
            AddFunction(new Text(this, "2005", "UV-26 CMD", "Display digits", "UV-26 Message Text"));
            #endregion

            #region PVI-800
            AddFunction(new PushButton(this, PVI, BUTTON_1, "312", "PVI-800 Control Panel", "0", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_2, "303", "PVI-800 Control Panel", "1", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_3, "304", "PVI-800 Control Panel", "2", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_4, "305", "PVI-800 Control Panel", "3", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_5, "306", "PVI-800 Control Panel", "4", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_6, "307", "PVI-800 Control Panel", "5", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_7, "308", "PVI-800 Control Panel", "6", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_8, "309", "PVI-800 Control Panel", "7", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_9, "310", "PVI-800 Control Panel", "8", "0.2", "0.0", "%0.1f"));
            AddFunction(new PushButton(this, PVI, BUTTON_10, "311", "PVI-800 Control Panel", "9", "0.2", "0.0", "%0.1f"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_11, "315", "PVI-800 Control Panel", "Waypoints"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_12, "519", "PVI-800 Control Panel", "INU Realignment"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_13, "316", "PVI-800 Control Panel", "Fixpoints"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_14, "520", "PVI-800 Control Panel", "Precise INU Alignment"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_15, "317", "PVI-800 Control Panel", "Airfields"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_16, "521", "PVI-800 Control Panel", "Normal INU Alignment"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_17, "318", "PVI-800 Control Panel", "Targets"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_18, "313", "PVI-800 Control Panel", "Enter"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_19, "314", "PVI-800 Control Panel", "Cancel"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_20, "522", "PVI-800 Control Panel", "Initial Nav Points"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_21, "319", "PVI-800 Control Panel", "Self coordinates"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_22, "320", "PVI-800 Control Panel", "Course Deviation/Time/Range"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_23, "321", "PVI-800 Control Panel", "Wind Heading/Speed"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_24, "322", "PVI-800 Control Panel", "True Heading/Time/Range"));
            AddFunction(new IndicatorPushButton(this, PVI, BUTTON_25, "323", "PVI-800 Control Panel", "Bearing/Range"));
            AddFunction(new Switch(this, PVI, "324", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_26), new SwitchPosition("0.1", "Verification", BUTTON_26), new SwitchPosition("0.2", "Edit", BUTTON_26), new SwitchPosition("0.3", "Operation", BUTTON_26), new SwitchPosition("0.4", "Simulate", BUTTON_26), new SwitchPosition("0.5", "K-1", BUTTON_26), new SwitchPosition("0.6", "K-2", BUTTON_26) }, "PVI-800 Control Panel", "Master Mode", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, PVI, BUTTON_28, "325", "1", "Shkval", "0", "Fly Over", "PVI-800 Control Panel", "INU fixtaking method", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DATALINK, BUTTON_16, "326", "1", "On", "0", "Off", "PVI-800 Control Panel", "Datalink power", "%1d"));
            AddFunction(new Axis(this, PVI, BUTTON_29, "327", 0.001, 0d, 1d, "PVI-800 Control Panel", "Brightness"));
            AddFunction(new Text(this, "2009", "PVI-800 Control Panel", "Upper big display digits", "Upper big display digits Message Text"));
            AddFunction(new Text(this, "2010", "PVI-800 Control Panel", "Upper big display apostrophe1", "Upper big display apostrophe1 Text"));
            AddFunction(new Text(this, "2011", "PVI-800 Control Panel", "Upper big display apostrophe2", "Upper big display apostrophe2 Text"));
            AddFunction(new Text(this, "2012", "PVI-800 Control Panel", "Upper small display digit", "Upper small display digit Message Text"));
            AddFunction(new Text(this, "2013", "PVI-800 Control Panel", "Lower big display digits", "Upper big display digits Message Text"));
            AddFunction(new Text(this, "2014", "PVI-800 Control Panel", "Lower big display apostrophe1", "Upper big display apostrophe1 Text"));
            AddFunction(new Text(this, "2015", "PVI-800 Control Panel", "Lower big display apostrophe2", "Upper big display apostrophe2 Text"));
            AddFunction(new Text(this, "2016", "PVI-800 Control Panel", "Lower small display digit", "Upper big display digit Message Text"));
            #endregion

            #region PVTz-800 Data Link Mode Panel
            AddFunction(new Switch(this, DATALINK, "328", new SwitchPosition[] { new SwitchPosition("0.0", "1", BUTTON_14), new SwitchPosition("0.1", "2", BUTTON_14), new SwitchPosition("0.2", "3", BUTTON_14), new SwitchPosition("0.3", "4", BUTTON_14) }, "PVTz-800 Data Link", "Self Id", "%0.1f"));
            AddFunction(new Switch(this, DATALINK, "329", new SwitchPosition[] { new SwitchPosition("0.0", "Disable", BUTTON_15), new SwitchPosition("0.1", "Receive", BUTTON_15), new SwitchPosition("0.2", "Wingman", BUTTON_15), new SwitchPosition("0.3", "Commander", BUTTON_15) }, "PVTz-800 Data Link", "Data Mode", "%0.1f"));
            #endregion

            #region Autopilot Panel
            AddFunction(new IndicatorPushButton(this, AUTOPILOT, BUTTON_1, "330", "Autopilot", "Bank Hold"));
            AddFunction(new IndicatorPushButton(this, AUTOPILOT, BUTTON_2, "332", "Autopilot", "Heading Hold"));
            AddFunction(new IndicatorPushButton(this, AUTOPILOT, BUTTON_3, "331", "Autopilot", "Pitch Hold"));
            AddFunction(new IndicatorPushButton(this, AUTOPILOT, BUTTON_4, "333", "Autopilot", "Altitude hold"));
            AddFunction(new IndicatorPushButton(this, AUTOPILOT, BUTTON_5, "334", "Autopilot", "Flight Director"));
            AddFunction(Switch.CreateThreeWaySwitch(this, AUTOPILOT, BUTTON_6, "335", "0.0", "Barometric", "0.5", "N/A", "1.0", "Radar", "Autopilot", "Altitude Source", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, NAV_INTERFACE, BUTTON_3, "336", "0.0", "Desired Heading", "0.5", "N/A", "1.0", "Desired Track", "Autopilot", "Autopilot Heading/Course", "%0.1f"));
            #endregion

            #region ADF ARK-22
            AddFunction(new PushButton(this, ARK_22, BUTTON_7, "355", "ARK 22", "Test Button"));
            AddFunction(Switch.CreateToggleSwitch(this, ARK_22, BUTTON_5, "354", "0", "Telephone", "1", "Telegraph", "ARK 22", "NDB Mode Switch", "%1d"));
            AddFunction(new Axis(this, ARK_22, BUTTON_7, "353", 0.05, 0d, 1d, "ARK 22", "Volume"));
            AddFunction(Switch.CreateToggleSwitch(this, ARK_22, BUTTON_4, "356", "0", "Compass", "1", "Antenna", "ARK 22", "ADF Mode", "%1d"));
            AddFunction(new Switch(this, ARK_22, "357", new SwitchPosition[] { new SwitchPosition("0.0", "1", BUTTON_2), new SwitchPosition("0.1", "2", BUTTON_2), new SwitchPosition("0.2", "3", BUTTON_2), new SwitchPosition("0.3", "4", BUTTON_2), new SwitchPosition("0.4", "5", BUTTON_2), new SwitchPosition("0.5", "6", BUTTON_2), new SwitchPosition("0.6", "7", BUTTON_2), new SwitchPosition("0.7", "8", BUTTON_2), new SwitchPosition("0.8", "9", BUTTON_2), new SwitchPosition("0.9", "10", BUTTON_2) }, "ARK 22", "Channel", "%0.1f"));
            #endregion

            #region R-828
            AddFunction(new Switch(this, R_828, "371", new SwitchPosition[] { new SwitchPosition("0.0", "1", BUTTON_1), new SwitchPosition("0.1", "2", BUTTON_1), new SwitchPosition("0.2", "3", BUTTON_1), new SwitchPosition("0.3", "4", BUTTON_1), new SwitchPosition("0.4", "5", BUTTON_1), new SwitchPosition("0.5", "6", BUTTON_1), new SwitchPosition("0.6", "7", BUTTON_1), new SwitchPosition("0.7", "8", BUTTON_1), new SwitchPosition("0.8", "9", BUTTON_1), new SwitchPosition("0.9", "10", BUTTON_1) }, "R-828 VHF-1 Radio", "Channel", "%0.1f"));
            AddFunction(new Axis(this, R_828, BUTTON_2, "372", 0.05d, 0d, 1d, "R-828 VHF-1 Radio", "Volume"));
            AddFunction(new PushButton(this, R_828, BUTTON_3, "373", "R-828 VHF-1 Radio", "Tuner Button"));
            AddFunction(Switch.CreateToggleSwitch(this, R_828, BUTTON_4, "374", "1", "On", "0", "Off", "R-828 VHF-1 Radio", "Squelch", "%1d"));
            AddFunction(new FlagValue(this, "375", "R-828 VHF-1 Radio", "Tuner Lamp", "Lit if R828 is under power and the automatic tuner button is pressed and radio has not been tuned."));
            AddFunction(new Text(this, "2017", "R-828 VHF-1 Radio", "Display digits", "Actual frequency selected"));
            #endregion

            #region Signal Flares
            AddFunction(new PushButton(this, SIGNAL_FLARE_DISPENSER, BUTTON_2, "376", "Signal Flares", "Fire Red"));
            AddFunction(new PushButton(this, SIGNAL_FLARE_DISPENSER, BUTTON_1, "377", "Signal Flares", "Fire Green"));
            AddFunction(new PushButton(this, SIGNAL_FLARE_DISPENSER, BUTTON_3, "378", "Signal Flares", "Fire White"));
            AddFunction(new PushButton(this, SIGNAL_FLARE_DISPENSER, BUTTON_4, "379", "Signal Flares", "Fire Yellow"));
            AddFunction(Switch.CreateToggleSwitch(this, SIGNAL_FLARE_DISPENSER, BUTTON_5, "380", "1", "On", "0", "Off", "Signal Flares", "Power", "%1d"));
            #endregion

            #region R-800 Panel
            AddFunction(new FlagValue(this, "419", "R-800 VHF-2 Radio", "Test Lamp", "Lit while test button is held down."));
            AddFunction(new PushButton(this, R_800, BUTTON_1, "418", "R-800 VHF-2 Radio", "Test Button"));
            AddFunction(Switch.CreateToggleSwitch(this, R_800, BUTTON_2, "417", "1", "AM", "0", "FM", "R-800 VHF-2 Radio", "AM/FM Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, R_800, BUTTON_3, "421", "1", "Emergency", "0", "Normal", "R-800 VHF-2 Radio", "Emergency Mode", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, R_800, BUTTON_4, "422", "1", "ADF", "0", "Normal", "R-800 VHF-2 Radio", "ADF Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, R_800, BUTTON_5, "420", "1", "100", "0", "50", "R-800 VHF-2 Radio", "Data Rate Switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, R_800, BUTTON_6, "423", "1", "On", "0", "Off", "R-800 VHF-2 Radio", "Squelch", "%1d"));
            AddFunction(new RotaryEncoder(this, R_800, BUTTON_7, "-1", 1, "R-800 VHF-2 Radio", "1st Rotary"));
            AddFunction(new RotaryEncoder(this, R_800, BUTTON_8, "-1", 1, "R-800 VHF-2 Radio", "2nd Rotary"));
            AddFunction(new RotaryEncoder(this, R_800, BUTTON_9, "-1", 1, "R-800 VHF-2 Radio", "3rd Rotary"));
            AddFunction(new RotaryEncoder(this, R_800, BUTTON_10, "-1", 1, "R-800 VHF-2 Radio", "4th Rotary"));
            AddFunction(new Functions.VHF2Rotary1(this, "577", "R-800 VHF-2 Radio", "1st Rotary Window"));
            AddFunction(new Functions.VHF2Rotary23(this, "574", "R-800 VHF-2 Radio", "2nd Rotoary Window"));
            AddFunction(new Functions.VHF2Rotary23(this, "575", "R-800 VHF-2 Radio", "3rd Rotoary Window"));
            AddFunction(new Functions.VHF2Rotary4(this, "576", "R-800 VHF-2 Radio", "4th Rotary Window"));
            #endregion

            #region Targeting Mode Control Panel
            AddFunction(Switch.CreateToggleSwitch(this, WEAP_INTERFACE, BUTTON_9, "432", "1", "Training", "0", "Off", "Targeting Mode Control", "Training Mode", "%1d"));
            AddFunction(new IndicatorPushButton(this, WEAP_INTERFACE, BUTTON_10, "437", "Targeting Mode Control", "Automatic Turn on Target"));
            AddFunction(new IndicatorPushButton(this, WEAP_INTERFACE, BUTTON_11, "438", "Targeting Mode Control", "Airborne Target"));
            AddFunction(new IndicatorPushButton(this, WEAP_INTERFACE, BUTTON_12, "439", "Targeting Mode Control", "Head On Airborne Target"));
            AddFunction(new IndicatorPushButton(this, WEAP_INTERFACE, BUTTON_13, "440", "Targeting Mode Control", "Moving Ground Target"));
            AddFunction(new Switch(this, WEAP_INTERFACE, "431", new SwitchPosition[] { new SwitchPosition("0.0", "Moving Cannon", BUTTON_14), new SwitchPosition("0.1", "Fixed Cannon", BUTTON_14), new SwitchPosition("0.2", "Backup/Manual", BUTTON_14), new SwitchPosition("0.3", "Backup  Navigation", BUTTON_14), new SwitchPosition("0.4", "Backup Combat", BUTTON_14) }, "Targeting Control Mode", "Weapons System Mode", "%0.1f"));
            AddFunction(new IndicatorPushButton(this, WEAP_INTERFACE, BUTTON_16, "441", "Targeting Mode Control", "Mode Reset"));
            AddFunction(Switch.CreateToggleSwitch(this, WEAP_INTERFACE, BUTTON_17, "436", "1", "Automatic", "0", "Gun sight", "Targeting Mode Control", "Tracking mode", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, K041, BUTTON_2, "433", "1", "On", "0", "Off", "Targeting Mode Control", "K-041 Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, LASERRANGER, BUTTON_1, "435", "1", "On", "0", "Off", "Targeting Mode Control", "Laser standby", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, HELMET, BUTTON_2, "434", "1", "On", "0", "Off", "Targeting Mode Control", "HMS Power", "%1d"));
            #endregion

            #region Engine/APU Start-Up
            AddFunction(new PushButton(this, ENGINE_INTERFACE, BUTTON_5, "412", "Engine", "Start Selected Engine"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, BUTTON_6, "413", "Engine", "Interrupt start-up"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, BUTTON_7, "414", "Engine", "Stop APU"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ENGINE_INTERFACE, BUTTON_12, "415", "0.0", "Start", "0.1", "Crank", "0.2", "False Start", "Engine", "Start-up Mode", "%0.1f"));
            AddFunction(new Switch(this, ENGINE_INTERFACE, "416", new SwitchPosition[] { new SwitchPosition("0.0", "APU", BUTTON_8), new SwitchPosition("0.1", "Left Engine", BUTTON_8), new SwitchPosition("0.2", "Right Engine", BUTTON_8), new SwitchPosition("0.3", "Turbo Gear", BUTTON_8) }, "Engine", "Engine Selector", "%0.1f"));
            AddFunction(new FlagValue(this, "163", "Engine", "Start Valve Lamp", "Lit when the start valve of the engine air-start is open."));
            #endregion

            #region Intercom Panel
            AddFunction(Switch.CreateRotarySwitch(this, SPU_9, BUTTON_2, "428", "SPU-9 Intercom", "Source Select", "%0.2f", new string[] { "0.00", "VHF-2", "0.11", "VHF-1", "0.22", "SW", "0.33", "Ground" }));
            #endregion

            #region APU Panel
            AddFunction(new FlagValue(this, "162", "APU", "Valve Open Indicator", "Lit when APU fuel shoutoff valve is open"));
            AddFunction(new FlagValue(this, "168", "APU", "Oil Pressure Indicator", "Lit when APU oil pressure is within acceptable limits"));
            AddFunction(new FlagValue(this, "169", "APU", "Stopped By RPM Indicator", "Lit when APU has shutdown due to over-revvign state."));
            AddFunction(new FlagValue(this, "174", "APU", "Operate Inddicator", "Lit when the APU has been started successfully."));
            AddFunction(new ScaledNetworkValue(this, "6", 900d, "APU", "Temperature", "Exhaust gas temperature of the APU", "0-900", BindingValueUnits.Celsius));
            #endregion

            #region Misc Engine Stuff
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, BUTTON_9, "554", "1", "Open", "0", "Closed", "Engine", "Left engine cut-off valve", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, BUTTON_10, "555", "1", "Open", "0", "Closed", "Engine", "Right engine cut-off valve", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE_INTERFACE, BUTTON_11, "556", "1", "On", "0", "Off", "Engine", "Rotor Brake", "%1d"));
            #endregion

            #region Misc Stuff
            AddFunction(Switch.CreateRotarySwitch(this, SHKVAL, BUTTON_6, "301", "Shkval", "Scan Rate", "%0.1f", new string[] { "0.0", "0.25", "0.1", "0.5", "0.2", "0.75", "0.3", "1.0", "0.4", "1.5", "0.5", "2.0", "0.6", "2.5", "0.7", "3" }));
            AddFunction(new PushButton(this, SHKVAL, BUTTON_7, "224", "Shkval", "Wiper"));
            #endregion

            #region Ground Power Indicators
            AddFunction(new FlagValue(this, "586", "Electrical", "AC Ground Power Indicator", "Lit when AC Ground power is on."));
            AddFunction(new FlagValue(this, "261", "Electrical", "DC Ground Power Indicator", "Lit when DC Ground power is on."));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_1, "262", BUTTON_2, "263", "1", "0", "1", "On", "0", "Off", "Electrical", "Ground DC Power", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_3, "543", BUTTON_4, "544", "1", "0", "1", "On", "0", "Off", "Electrical", "Battery 2", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_5, "264", BUTTON_6, "265", "1", "0", "1", "On", "0", "Off", "Electrical", "Battery 1", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_7, "267", "1", "On", "0", "Off", "Electrical", "AC Ground Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_8, "268", "1", "On", "0", "Off", "Electrical", "AC Left Generator", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ELEC_INTERFACE, BUTTON_9, "269", "1", "On", "0", "Off", "Electrical", "AC Right Generator", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ELEC_INTERFACE, BUTTON_10, "270", "0.2", "Auto", "0.1", "Off", "0.0", "Manual", "Electrical", "DC/AC Inverter", "%0.1f"));
            #endregion

            #region Fuel Pumps
            AddFunction(Switch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_1, "271", "1", "On", "0", "Off", "Fuel System", "Forward Fuel Tank Pumps", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_2, "272", "1", "On", "0", "Off", "Fuel System", "Rear Fuel Tank Pumps", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_3, "273", "1", "On", "0", "Off", "Fuel System", "Inner External Fuel Tank Pumps", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_4, "274", "1", "On", "0", "Off", "Fuel System", "Outer External Fuel Tank Pumps", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_5, "275", "1", "On", "0", "Off", "Fuel System", "Fuel Meter power", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_6, "276", BUTTON_7, "277", "1", "0", "1", "On", "0", "Off", "Fuel System", "Left Engine Fuel Shutoff Valve", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_8, "278", BUTTON_9, "279", "1", "0", "1", "On", "0", "Off", "Fuel System", "Right Engine Fuel Shutoff Valve", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_10, "280", BUTTON_11, "281", "1", "0", "1", "On", "0", "Off", "Fuel System", "APU Fuel Shutoff Valve", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, FUELSYS_INTERFACE, BUTTON_12, "282", BUTTON_13, "283", "1", "0", "1", "On", "0", "Off", "Fuel System", "Fuel Crossfeed Valve", "%1d"));
            #endregion

            #region Comms Power
            AddFunction(Switch.CreateToggleSwitch(this, SPU_9, BUTTON_1, "284", "1", "On", "0", "Off", "SPU-9 Intercom", "Power", "%1d"));
            //elements["COMM-PWR-UKV-1-PTR"]		= {class = {class_type.TUMB,class_type.TUMB}, hint = LOCALIZE("VHF-1 (R828) power switch"), device = devices.R_828, action = {device_commands.Button_5,device_commands.Button_5}, arg = {285,285}, arg_value = {-direction*1.0,direction*1.0}, arg_lim =  {{0, 1},{0, 1}}, use_OBB = true, updatable = true}
            AddFunction(Switch.CreateToggleSwitch(this, R_828, BUTTON_5, "285", "1", "On", "0", "Off", "R-828 VHF-1 Radio", "Power", "%1d"));
            // elements["COMM-PWR-UKV-2-PTR"]		= {class = {class_type.TUMB,class_type.TUMB}, hint = LOCALIZE("VHF-2 (R-800) power switch"), device = devices.R_800, action = {device_commands.Button_11,device_commands.Button_11}, arg = {286,286}, arg_value = {-direction*1.0,direction*1.0}, arg_lim = {{0, 1},{0, 1}}, use_OBB = true, updatable = true}
            AddFunction(Switch.CreateToggleSwitch(this, R_800, BUTTON_11, "286", "1", "On", "0", "Off", "R-800 VHF-2 Radio", "Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DATALINK, BUTTON_17, "287", "1", "On", "0", "Off", "Datalink", "TLK Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, DATALINK, BUTTON_18, "288", "1", "On", "0", "Off", "Datalink", "UHF TLK Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, "0", BUTTON_7, "289", "1", "On", "0", "Off", "Datalink", "SA-TLF Power", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, WEAP_INTERFACE, BUTTON_18, "547", BUTTON_19, "548", "1", "0", "1", "On", "0", "Off", "Weapon System", "Power", "%1d"));
            #endregion

            #region Ejection System
            AddFunction(Switch.CreateToggleSwitch(this, EJECT_SYS_INTERFACE, BUTTON_1, "214", "0", "On", "1", "Off", "Ejection System", "Power 1", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, EJECT_SYS_INTERFACE, BUTTON_2, "215", "0", "On", "1", "Off", "Ejection System", "Power 2", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, EJECT_SYS_INTERFACE, BUTTON_3, "216", "0", "On", "1", "Off", "Ejection System", "Power 3", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, EJECT_SYS_INTERFACE, BUTTON_4, "217", "0", "Up", "1", "Down", "Ejection System", "Power Cover", "%1d"));
            AddFunction(Switch.CreateRotarySwitch(this, EJECT_SYS_INTERFACE, BUTTON_5, "462", "Ejection System", "Circuit Selector", "%0.1f", new string[] { "0.0", "1", "0.1", "2", "0.2", "3", "0.3", "4", "0.4", "5", "0.5", "6", "0.6", "7" }));
            AddFunction(new PushButton(this, EJECT_SYS_INTERFACE, BUTTON_6, "460", "Ejection System", "Circuit Test"));
            AddFunction(new FlagValue(this, "461", "Ejection System", "Check Lamp", "")); 
            #endregion

            #region Misc Power
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, HYDRO_SYS_INTERFACE, BUTTON_1, "220", BUTTON_2, "221", "1", "0", "1", "On", "0", "Off", "Hydraulics", "Power", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, IFF, BUTTON_1, "218", BUTTON_2, "219", "1", "0", "1", "On", "0", "Off", "IFF", "Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, K041, BUTTON_1, "222", "1", "On", "0", "Off", "Navigation System", "Power", "%1d"));
            AddFunction(Switch.CreateThreeWaySwitch(this, NAV_INTERFACE, BUTTON_1, "229", "0.0", "Gyro", "0.1", "Magnetic", "0.2", "Manual", "Navigation System", "Heading Source", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, NAVLIGHT_SYSTEM, BUTTON_3, "228", "1", "On", "0", "Off", "Navigation Light System", "Anti-Collision Beacon", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, NAVLIGHT_SYSTEM, BUTTON_1, "296", "1", "On", "0", "Off", "Navigation Light System", "Rotor Tip Lights", "%1d"));
            AddFunction(new Switch(this, NAVLIGHT_SYSTEM, "297", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_2), new SwitchPosition("0.1", "10%", BUTTON_2), new SwitchPosition("0.2", "30%", BUTTON_2), new SwitchPosition("0.3", "100%", BUTTON_2) }, "Navigation Light System", "Formation Lights", "%0.1f")); 
            #endregion

            #region Engines
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, ENGINE_INTERFACE, BUTTON_1, "290", BUTTON_2, "291", "1", "0", "1", "On", "0", "Off", "Engine", "Left EEG", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, ENGINE_INTERFACE, BUTTON_3, "292", BUTTON_4, "293", "1", "0", "1", "On", "0", "Off", "Engine", "Right EEG", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, ENGINE_INTERFACE, BUTTON_15, "294", BUTTON_16, "569", "1", "0", "1", "Test", "0", "Operate", "Engine", "EEG Gas Generator Test", "%1d"));
            AddFunction(GuardedSwitch.CreateThreeWaySwitch(this, ENGINE_INTERFACE, BUTTON_17, "295", BUTTON_18, "570", "1.0", "0.0", "0.2", "PT-2 Test", "0.1", "Operate", "0.0", "PT-1 Test", "Engine", "EEG Power Turbine Test", "%0.1f"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, BUTTON_19, "457", "Engine", "Left EGT Governor Button"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, BUTTON_20, "458", "Engine", "Right EGT Governor Button"));
            AddFunction(new PushButton(this, ENGINE_INTERFACE, BUTTON_21, "459", "Engine", "Vibrations Monitoring System Button"));
            AddFunction(Switch.CreateToggleSwitch(this, ILLUMINATION_INTERFACE, BUTTON_1, "300", "1", "On", "0", "Off", "Lighting", "Panel Lights", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ILLUMINATION_INTERFACE, BUTTON_7, "299", "1", "On", "0", "Off", "Lighting", "Night Vision Lighting", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ILLUMINATION_INTERFACE, BUTTON_3, "298", "1", "On", "0", "Off", "Lighting", "ADI & SAI Lights", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, ILLUMINATION_INTERFACE, BUTTON_5, "450", "1", "On", "0", "Off", "Lighting", "Lighting auxiliary panel switch", "%1d"));
            AddFunction(new Switch(this, ILLUMINATION_INTERFACE, "1001", new SwitchPosition[] { new SwitchPosition("1.0", "On", "3009"), new SwitchPosition("0.0", "Off", "3009") }, "Lighting", "Plafond Light", "%0.1f"));
            AddFunction(new Axis(this, ILLUMINATION_INTERFACE, BUTTON_6, "451", 0.01, 0d, 1d, "Lighting", "Lighting auxiliary panel brightness knob"));
            AddFunction(new Axis(this, ILLUMINATION_INTERFACE, BUTTON_8, "507", 0.01, 0d, 1d, "Lighting", "Lighting night vision cockpit brightness knob"));
            AddFunction(new Axis(this, ILLUMINATION_INTERFACE, BUTTON_2, "593", 0.01, 0d, 1d, "Lighting", "Lighting cockpit panel brightness knob"));
            AddFunction(new Axis(this, ILLUMINATION_INTERFACE, BUTTON_4, "508", 0.01, 0d, 1d, "Lighting", "Lighting HSI and ADI brightness knob"));
            #endregion

            #region Fire Extinguishers Controls
            AddFunction(new PushButton(this, FIRE_EXTING_INTERFACE, BUTTON_1, "236", "Fire Extinguishers", "Left Engine"));
            AddFunction(new PushButton(this, FIRE_EXTING_INTERFACE, BUTTON_2, "238", "Fire Extinguishers", "APU"));
            AddFunction(new PushButton(this, FIRE_EXTING_INTERFACE, BUTTON_3, "240", "Fire Extinguishers", "Right Engine"));
            AddFunction(new PushButton(this, FIRE_EXTING_INTERFACE, BUTTON_4, "242", "Fire Extinguishers", "Ventilator"));
            AddFunction(GuardedSwitch.CreateThreeWaySwitch(this, FIRE_EXTING_INTERFACE, BUTTON_5, "248", BUTTON_7, "249", "1.0", "0.0", "0.2", "Work", "0.1", "Off", "0.0", "Test", "Fire Extinguishers", "Work/Off/Test", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, FIRE_EXTING_INTERFACE, BUTTON_8, "250", "1", "Warn", "0", "Off", "Fire Extinguishers", "Signaling", "%1d"));
            AddFunction(GuardedSwitch.CreateToggleSwitch(this, FIRE_EXTING_INTERFACE, BUTTON_9, "246", BUTTON_10, "247", "1", "0", "0", "Manual", "1", "Auto", "Fire Extinguishers", "Mode Select", "%1d"));
            AddFunction(new FlagValue(this, "237", "Fire Extinguishers", "Left Engine Fire Indicator", "Lit when a fire has been detected in the left enigne bay."));
            AddFunction(new FlagValue(this, "239", "Fire Extinguishers", "APU Fire Indicator", "Lit when a fire has been detected in the APU bay."));
            AddFunction(new FlagValue(this, "568", "Fire Extinguishers", "Hydraulics Fire Indicator", "Lit when a high temperature has been detected in the hydraulics bay."));
            AddFunction(new FlagValue(this, "241", "Fire Extinguishers", "Right Engine Fire Indicator", "Lit when a fire has been detected in the right engine bay."));
            AddFunction(new FlagValue(this, "243", "Fire Extinguishers", "Ventilator Fire Indicator", "Lit if a high temperature has been detected in the oil-coolers compartment."));
            AddFunction(new FlagValue(this, "244", "Fire Extinguishers", "Bottle 1 Indicator", "Lit when bottle 1 is charged and ready for use."));
            AddFunction(new FlagValue(this, "245", "Fire Extinguishers", "Bottle 2 Indicator", "Lit when bottle 2 is charged and ready for use."));

            // this is three valued momentary switch, with commands 3011, 3012, 3013, and argument 251 with values 0.0 (center), 0.1, 0.2, 0.3
            AddFunction(new Switch(this, FIRE_EXTING_INTERFACE, "251", new []
                {
                    new SwitchPosition("0.1", "Group 1", "3011", "3011", "0.0"),
                    new SwitchPosition("0.2", "Group 2", "3012", "3012", "0.0"),
                    new SwitchPosition("0.3", "Group 3", "3013", "3013", "0.0"),
                    new SwitchPosition("0.0", "Release", "3011")
                },
                "Fire Extinguishers", "Test Selector", "%.1f", false));
            #endregion

            // TODO Misc
            AddFunction(new Switch(this, ENGINE_INTERFACE, "258", new SwitchPosition[] { new SwitchPosition("0.0", "Left Hand", BUTTON_24, BUTTON_25, "0.1"), new SwitchPosition("0.1", "Center", null), new SwitchPosition("0.2", "Right Hand", BUTTON_24, BUTTON_25, "0.1") }, "Engine", "GearBox Reducing Oil Pressure Indicator", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, MISC_SYSTEMS_INTERFACE, BUTTON_4, "259", "1", "Auto", "0", "Manual", "Misc", "Sling Load Auto/Manual", "%1d"));

            #region Read Pre-flight Panel
            AddFunction(Switch.CreateRotarySwitch(this, PPK, BUTTON_1, "483", "PPK", "ATGM Temperature Selector", "%0.1f", new string[] {"0.0", "0", "0.1", "1", "0.2", "2",
                "0.3", "3", "0.4", "4", "0.5", "5", "0.6", "6", "0.7", "7", "0.8", "8", "0.9", "9", "1.0", "10" }));
            AddFunction(Switch.CreateRotarySwitch(this, WEAP_INTERFACE, BUTTON_23, "484", "PUI-800", "Unguided Ballistics Selector", "%0.1f", new string[] {"0.0", "0", "0.1", "1", "0.2", "2",
                "0.3", "3", "0.4", "4", "0.5", "5", "0.6", "6", "0.7", "7", "0.8", "8", "0.9", "9", "1.0", "10" }));
            AddFunction(Switch.CreateRotarySwitch(this, PPK, BUTTON_3, "485", "PPK", "Systems BIT Selector", "%0.1f", new string[] {"0.0", "0", "0.1", "1", "0.2", "2",
                "0.3", "3", "0.4", "4", "0.5", "5", "0.6", "6", "0.7", "7", "0.8", "8" }));
            AddFunction(Switch.CreateToggleSwitch(this, PPK, BUTTON_4, "486", "1", "On", "0", "Off", "PPK", "Computer BIT Selector", "%1d"));
            AddFunction(new PushButton(this, PPK, BUTTON_7, "489", "PPK", "Self Test Button"));
            AddFunction(Switch.CreateToggleSwitch(this, PPK, BUTTON_8, "490", "1", "On", "0", "Off", "PPK", "Emergency INU Alignment", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, PPK, BUTTON_9, "491", "1", "On", "0", "Off", "PPK", "Stabilization and indication of hanger cable switch", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, PPK, BUTTON_10, "492", "1", "On", "0", "Off", "PPK", "Video Tape Recorder Switch", "%1d"));

            AddFunction(Switch.CreateToggleSwitch(this, C061K, BUTTON_1, "487", "1", "On", "0", "Off", "INU", "Power", "%1d"));
            AddFunction(Switch.CreateToggleSwitch(this, C061K, BUTTON_2, "488", "1", "On", "0", "Off", "INU", "Heater Power", "%1d"));

            AddFunction(GuardedSwitch.CreateToggleSwitch(this, CPT_MECH, BUTTON_2, "452", BUTTON_3, "453", "1", "0", "1", "On", "0", "Off", "Mechanical", "Hydraulics/EKRAN Self Test Switch", "%1d"));
            #endregion

            // Engines Power Indicator Mode
            AddFunction(new NetworkValue(this, "592", "Engine Power Indicator", "Power Indicator Mode", "Central Index position on engine power indicator", "0-1", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "234", 5d, "Engine Power Indicator", "Left Engine Marker", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter, 5d, "%0.2f"));
            AddFunction(new ScaledNetworkValue(this, "235", 5d, "Engine Power Indicator", "Right Engine Marker", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter, 5d, "%0.2f"));

            // Oil Pressure/Temp Gauges
            AddFunction(new ScaledNetworkValue(this, "252", 8d, "Engine", "Left Engine Oil Pressure", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter));
            AddFunction(new ScaledNetworkValue(this, "253", 8d, "Engine", "Right Engine Oil Pressure", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter));
            AddFunction(new ScaledNetworkValue(this, "254", 8d, "Engine", "Gearbox Oil Pressure", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter));
            CalibrationPointCollectionDouble engineOilTempScale = new CalibrationPointCollectionDouble(0, -60d, 1, 180d);
            AddFunction(new ScaledNetworkValue(this, "255", engineOilTempScale, "Engine", "Left Engine Oil Temperature", "", "", BindingValueUnits.Celsius));
            AddFunction(new ScaledNetworkValue(this, "256", engineOilTempScale, "Engine", "Right Engine Oil Temperature", "", "", BindingValueUnits.Celsius));
            CalibrationPointCollectionDouble gearboxOilTempScale = new CalibrationPointCollectionDouble(0, -50d, 1, 150d);
            AddFunction(new ScaledNetworkValue(this, "257", gearboxOilTempScale, "Engine", "Gearbox Oil Temperature", "", "", BindingValueUnits.Celsius));

            // Hydraulics Indicators
            AddFunction(new FlagValue(this, "469", "Hydraulics", "Valve 1 Indicator", ""));
            AddFunction(new FlagValue(this, "470", "Hydraulics", "Valve 2 Indicator", ""));
            AddFunction(new ScaledNetworkValue(this, "471", 100d, "Hydraulics", "Common Pressure", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter));
            AddFunction(new ScaledNetworkValue(this, "472", 100d, "Hydraulics", "Main Pressure", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter));
            AddFunction(new ScaledNetworkValue(this, "473", 100d, "Hydraulics", "Accumulators Pressure", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter));
            AddFunction(new ScaledNetworkValue(this, "474", 100d, "Hydraulics", "Wheel Brake Pressure", "", "", BindingValueUnits.KilgramsForcePerSquareCentimenter));
            AddFunction(new ScaledNetworkValue(this, "475", gearboxOilTempScale, "Hydraulics", "Common Temperature", "", "", BindingValueUnits.Celsius));
            AddFunction(new ScaledNetworkValue(this, "476", gearboxOilTempScale, "Hydraulics", "Main Temperature", "", "", BindingValueUnits.Celsius));

            // Mag Variation Panel
            AddFunction(new RotaryEncoder(this, PShk_7, BUTTON_1, "340", 0.001, "PShK-7 Latitude Entry", "Latitude Correction"));
            AddFunction(Switch.CreateToggleSwitch(this, PShk_7, BUTTON_2, "341", "1", "North", "0", "South", "PShK-7 Latitude Entry", "Latitidue Correction Switch", "%1d"));
            AddFunction(new FlagValue(this, "342", "PShK-7 Latitude Entry", "Auto Lamp", ""));
            AddFunction(new Functions.LatitudeEntry(this));
            AddFunction(new RotaryEncoder(this, ZMS_3, BUTTON_1, "338", 0.001, "ZMS-3 Magnetic Variation", "Magnetic variatoin selection"));
            AddFunction(new Functions.MagVariation(this));
        }
    }
}
