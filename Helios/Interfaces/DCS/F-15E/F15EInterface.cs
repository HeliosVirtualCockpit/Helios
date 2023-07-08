//  Copyright 2014 Craig Courtney
//  Copyright 2023 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.F15E
{
    using System;
    using System.Collections.Generic;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Interfaces.DCS.AV8B.Functions;

    public enum Cockpit { Pilot, WSO }
    /// <summary>
    /// common base for interfaces for DCS F15E containing only functions
    /// </summary>
    [HeliosInterface(
        "Helios.F15ESE",                          // Helios internal type ID used in Profile XML, must never change
        "DCS F-15E Strike Eagle",                 // human readable UI name for this interface
        typeof(DCSInterfaceEditor),               // uses basic DCS interface dialog
        typeof(UniqueHeliosInterfaceFactory),     // can't be instantiated when specific other interfaces are present
        UniquenessKey = "Helios.DCSInterface")]   // all other DCS interfaces exclude this interface

    public class F15EInterface : DCSInterface
    {
        private NetworkFunctionCollection _functions = new NetworkFunctionCollection();

        public F15EInterface(string heliosName) : base(
                heliosName,
                "F-15ESE",
                "pack://application:,,,/Helios;component/Interfaces/DCS/F-15E/ExportFunctionsF15E.lua")
        {

            // see if we can restore from JSON
//#if (!DEBUG)
//                        if (LoadFunctionsFromJson())
//                        {
//                            return;
//                        }
//#endif
#pragma warning disable CS0162 // Unreachable code detected
            #region UFC Panel
            #region ODU Pilot
            AddFunction(new Text(this, "2100", "UFC Panel (Pilot)", "Option Line 1", "Text value of Option Line"));
            AddFunction(new Text(this, "2101", "UFC Panel (Pilot)", "Option Line 2", "Text value of Option Line"));
            AddFunction(new Text(this, "2102", "UFC Panel (Pilot)", "Option Line 3", "Text value of Option Line"));
            AddFunction(new Text(this, "2103", "UFC Panel (Pilot)", "Option Line 4", "Text value of Option Line"));
            AddFunction(new Text(this, "2104", "UFC Panel (Pilot)", "Option Line 5", "Text value of Option Line"));
            AddFunction(new Text(this, "2105", "UFC Panel (Pilot)", "Option Line 6", "Text value of Option Line"));
            AddFunction(new Text(this, "2106", "UFC Panel (Pilot)", "Option Line 1 Center Text", "Text value of Option Line"));
            AddFunction(new Text(this, "2107", "UFC Panel (Pilot)", "Option Line 6 Scratchpad", "Text value of Option Line"));
            //AddFunction(new Text(this, "2106", "UFC Panel (Pilot)", "Line 3 Left (Pilot)", "Text value of ODU Line 3 Left side"));
            //AddFunction(new Text(this, "2107", "UFC Panel (Pilot)", "Line 3 Middle (Pilot)", "Text value of ODU Line 3 Center"));
            //AddFunction(new Text(this, "2108", "UFC Panel (Pilot)", "Line 3 Right (Pilot)", "Text value of ODU Line 3 Right side"));
            //AddFunction(new Text(this, "2109", "UFC Panel (Pilot)", "Line 4 Left (Pilot)", "Text value of ODU Line 4 Left side"));
            //AddFunction(new Text(this, "2110", "UFC Panel (Pilot)", "Line 4 Middle (Pilot)", "Text value of ODU Line 4 Center"));
            //AddFunction(new Text(this, "2111", "UFC Panel (Pilot)", "Line 4 Right (Pilot)", "Text value of ODU Line 4 Right side"));
            //AddFunction(new Text(this, "2112", "UFC Panel (Pilot)", "Line 5 Left (Pilot)", "Text value of ODU Line 5 Left side"));
            //AddFunction(new Text(this, "2113", "UFC Panel (Pilot)", "Line 5 Middle (Pilot)", "Text value of ODU Line 5 Center"));
            //AddFunction(new Text(this, "2114", "UFC Panel (Pilot)", "Line 5 Right (Pilot)", "Text value of ODU Line 5 Right side"));
            //AddFunction(new Text(this, "2115", "UFC Panel (Pilot)", "Line 6 Left (Pilot)", "Text value of ODU Line 6 Left side"));
            //AddFunction(new Text(this, "2116", "UFC Panel (Pilot)", "Line 6 Middle (Pilot)", "Text value of ODU Line 6 Center"));
            //AddFunction(new Text(this, "2117", "UFC Panel (Pilot)", "Line 6 Right (Pilot)", "Text value of ODU Line 6 Right side"));
            //AddFunction(new Text(this, "2118", "UFC Panel (Pilot)", "Display (Pilot)", "Text value of ODU General Display"));
            //AddFunction(new Text(this, "21??", "Option Display Unit (Pilot)", "Line 5 Left Decimal (Pilot)", "Text value of ODU Line 5 Left Decimal"));
            //AddFunction(new Text(this, "21??", "Option Display Unit (Pilot)", "Line 5 Right Decimal", "Text value of ODU Line 5 Right Decimal"));
            #endregion ODU Pilot
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_1.ToString("d"), "270", "UFC Panel (Pilot)", "Option Push Button 1 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_2.ToString("d"), "271", "UFC Panel (Pilot)", "Option Push Button 2 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_3.ToString("d"), "272", "UFC Panel (Pilot)", "Option Push Button 3 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_4.ToString("d"), "273", "UFC Panel (Pilot)", "Option Push Button 4 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_5.ToString("d"), "274", "UFC Panel (Pilot)", "Option Push Button 5 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_0.ToString("d"), "275", "UFC Panel (Pilot)", "Option Push Button 1 Right", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_9.ToString("d"), "276", "UFC Panel (Pilot)", "Option Push Button 2 Right", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_8.ToString("d"), "277", "UFC Panel (Pilot)", "Option Push Button 3 Right", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_7.ToString("d"), "278", "UFC Panel (Pilot)", "Option Push Button 4 Right", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PB_6.ToString("d"), "279", "UFC Panel (Pilot)", "Option Push Button 5 Right", "%1d"));
            AddFunction(new RotaryEncoder(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PRESET_LEFT.ToString("d"), "280", 0.1d, "UFC Panel (Pilot)", "Left UHF Preset Channel Selector"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PRESET_SW_LEFT.ToString("d"), "680", "UFC Panel (Pilot)", "Left UHF Preset Channel Pull Switch", "%1d"));
            AddFunction(new RotaryEncoder(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PRESET_RIGHT.ToString("d"), "281", 0.1d, "UFC Panel (Pilot)", "Right UHF Preset Channel Selector"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_PRESET_SW_RIGHT.ToString("d"), "681", "UFC Panel (Pilot)", "Right UHF Preset Channel Pull Switch", "%1d"));

            AddFunction(new Axis(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_VOL_R1.ToString("d"), "282", 0.1d, 0.0d, 1.0d, "UFC Panel (Pilot)", "UHF Radio 1 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_VOL_R2.ToString("d"), "283", 0.1d, 0.0d, 1.0d, "UFC Panel (Pilot)", "UHF Radio 2 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_VOL_R3.ToString("d"), "284", 0.1d, 0.0d, 1.0d, "UFC Panel (Pilot)", "UHF Radio 3 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_VOL_R4.ToString("d"), "285", 0.1d, 0.0d, 1.0d, "UFC Panel (Pilot)", "UHF Radio 4 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_BRT_CTRL.ToString("d"), "286", 0.1d, 0.0d, 1.0d, "UFC Panel (Pilot)", "UFC LCD Brightness", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_EMIS_LMT.ToString("d"), "287", "UFC Panel (Pilot)", "Emission Limit Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_GREC_CM_LEFT.ToString("d"), "288", "UFC Panel (Pilot)", "Left Guard Receiver Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY_A1.ToString("d"), "289", "UFC Panel (Pilot)", "A1 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY_N2.ToString("d"), "290", "UFC Panel (Pilot)", "N2 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY_B3.ToString("d"), "291", "UFC Panel (Pilot)", "B3 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_GREC_CM_RIGHT.ToString("d"), "292", "UFC Panel (Pilot)", "Right Guard Receiver Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_MARK.ToString("d"), "293", "UFC Panel (Pilot)", "MARK key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY_W4.ToString("d"), "294", "UFC Panel (Pilot)", "W4 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY_M5.ToString("d"), "295", "UFC Panel (Pilot)", "M5 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY_E6.ToString("d"), "296", "UFC Panel (Pilot)", "E6 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_I_P.ToString("d"), "297", "UFC Panel (Pilot)", "IP Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_DOT.ToString("d"), "298", "UFC Panel (Pilot)", "Decimal Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY__7.ToString("d"), "299", "UFC Panel (Pilot)", "7 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY_S8.ToString("d"), "300", "UFC Panel (Pilot)", "S8 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY_C9.ToString("d"), "301", "UFC Panel (Pilot)", "C9 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_SHF.ToString("d"), "302", "UFC Panel (Pilot)", "Shift Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_A_P.ToString("d"), "303", "UFC Panel (Pilot)", "AP Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_CLEAR.ToString("d"), "304", "UFC Panel (Pilot)", "Clr Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_KEY__0.ToString("d"), "305", "UFC Panel (Pilot)", "0 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_DATA.ToString("d"), "306", "UFC Panel (Pilot)", "Data Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_FRONT.ToString("d"), Commands.ufc_commands.UFC_MENU.ToString("d"), "307", "UFC Panel (Pilot)", "Menu Key", "%1d"));

            #endregion UFC Panel (Pilot)

            #region HUD Control Panel
            AddFunction(new Axis(this, devices.HUDCTRL.ToString("d"), Commands.hudctrl_commands.HUD_BRT_Knob.ToString("d"), "120", 0.1d, 0.0d, 1.0d, "HUD Control Panel", "HUD Brightness Control", false, "%0.1f"));
            AddFunction(new Switch(this, devices.HUDCTRL.ToString("d"), "121", SwitchPositions.Create(3, 0d, 0.5d, Commands.hudctrl_commands.HUD_REJ_Switch.ToString("d"), "Posn", "%0.3f"), "HUD Control Panel", "HUD Symbology Reject Mode", "%0.3f"));
            AddFunction(new Switch(this, devices.HUDCTRL.ToString("d"), "122", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", Commands.hudctrl_commands.HUD_MODE_Switch.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.hudctrl_commands.HUD_MODE_Switch.ToString("d")), new SwitchPosition("1.0", "Posn 3", Commands.hudctrl_commands.HUD_MODE_Switch.ToString("d")) }, "HUD Control Panel", "HUD DAY/AUTO/NIGHT Mode Selector", "%0.1f"));
            AddFunction(new PushButton(this, devices.HUDCTRL.ToString("d"), Commands.hudctrl_commands.HUD_BIT_Button.ToString("d"), "123", "HUD Control Panel", "HUD BIT", "%1d"));
            AddFunction(new Axis(this, devices.HUDCTRL.ToString("d"), Commands.hudctrl_commands.HUD_VIDEO_BRT_Knob.ToString("d"), "124", 0.1d, 0.0d, 1.0d, "HUD Control Panel", "HUD Video Brightness Control", false, "%0.1f"));
            AddFunction(new Axis(this, devices.HUDCTRL.ToString("d"), Commands.hudctrl_commands.HUD_VIDEO_CONT_Knob.ToString("d"), "125", 0.1d, 0.0d, 1.0d, "HUD Control Panel", "HUD Contrast Control", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.ACC.ToString("d"), Commands.hudctrl_commands.MM_AA_Switch.ToString("d"), "126", "HUD Control Panel", "AA Master Mode Selector", "%1d"));
            AddFunction(new FlagValue(this, "326", "HUD Control Panel", "AA Master Mode Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new PushButton(this, devices.ACC.ToString("d"), Commands.hudctrl_commands.MM_AG_Switch.ToString("d"), "127", "HUD Control Panel", "AG Master Mode Selector", "%1d"));
            AddFunction(new FlagValue(this, "327", "HUD Control Panel", "AG Master Mode Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new PushButton(this, devices.ACC.ToString("d"), Commands.hudctrl_commands.MM_NAV_Switch.ToString("d"), "128", "HUD Control Panel", "Nav Master Mode Selector", "%1d"));
            AddFunction(new FlagValue(this, "328", "HUD Control Panel", "Nav Master Mode Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new PushButton(this, devices.ACC.ToString("d"), Commands.hudctrl_commands.MM_INST_Switch.ToString("d"), "129", "HUD Control Panel", "Inst Master Mode Selector", "%1d"));
            AddFunction(new FlagValue(this, "329", "HUD Control Panel", "Inst Master Mode Indicator", "True when indicator is lit", "%1d"));
            #endregion HUD Control Panel
            #region AMAD Panel
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "314", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.amadctrl_commands.fire_ext_sw.ToString("d"), Commands.amadctrl_commands.fire_ext_sw.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", Commands.amadctrl_commands.fire_ext_sw.ToString("d"), Commands.amadctrl_commands.fire_ext_sw.ToString("d"), "0.0", "0.0") }, "AMAD Panel", "Fire Extinguisher Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "315", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.amadctrl_commands.amad_sw_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.amadctrl_commands.amad_sw_cover.ToString("d")) }, "AMAD Panel", "AMAD Fire Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "316", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.amadctrl_commands.amad_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.amadctrl_commands.amad_sw.ToString("d")) }, "AMAD Panel", "AMAD Fire Switch", "%0.1f"));
            AddFunction(new FlagValue(this, "130", "AMAD Panel", "AMAD Fire Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "317", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.amadctrl_commands.left_eng_fire_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.amadctrl_commands.left_eng_fire_cover.ToString("d")) }, "AMAD Panel", "Left Engine Fire Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "318", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.amadctrl_commands.left_eng_fire_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.amadctrl_commands.left_eng_fire_sw.ToString("d")) }, "AMAD Panel", "Left Engine Fire Switch", "%0.1f"));
            AddFunction(new FlagValue(this, "131", "AMAD Panel", "Engine Fire Left Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "319", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.amadctrl_commands.right_eng_fire_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.amadctrl_commands.right_eng_fire_cover.ToString("d")) }, "AMAD Panel", "Right Engine Fire Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "320", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.amadctrl_commands.right_eng_fire_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.amadctrl_commands.right_eng_fire_sw.ToString("d")) }, "AMAD Panel", "Right Engine Fire Switch", "%0.1f"));
            AddFunction(new FlagValue(this, "132", "AMAD Panel", "Engine Fire Right Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "133", "AMAD Panel", "Left Burn Thru Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "134", "AMAD Panel", "Right Burn Thru Indicator", "True when indicator is lit", "%1d"));
            #endregion AMAD Panel
            #region Armament Panel
            AddFunction(new Switch(this, devices.PACS.ToString("d"), "323", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.armtctrl_commands.Master_Arm_SW.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.armtctrl_commands.Master_Arm_SW.ToString("d")) }, "Armament Panel", "Master Arm", "%0.1f"));
            AddFunction(new Switch(this, devices.PACS.ToString("d"), "321", SwitchPositions.Create(7, 0d, 0.167d, Commands.armtctrl_commands.JETT_Selector_Knob.ToString("d"), "Posn", "%0.3f"), "Armament Panel", "Armament Jettison Selector", "%0.3f"));
            AddFunction(new PushButton(this, devices.PACS.ToString("d"), Commands.armtctrl_commands.JETT_Button.ToString("d"), "322", "Armament Panel", "Armament Jettison Button", "%1d"));
            AddFunction(new PushButton(this, devices.PACS.ToString("d"), Commands.misc_commands.em_jett_btn.ToString("d"), "340", "Armament Panel", "Emergency Jettison Button", "%1d"));
            #endregion Armament Panel
            #region Fuel Monitor Panel
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "381", SwitchPositions.Create(7, 0d, 0.167d, Commands.fltinst_commands.fuelqty_totalizer.ToString("d"), "Posn", "%0.3f"), "Fuel Monitor Panel", "Fuel Totalizer Selector", "%0.3f"));
            AddFunction(new Axis(this, devices.FLINST.ToString("d"), Commands.fltinst_commands.bingo_sel_knob.ToString("d"), "385", 0.1d, 0.0d, 1.0d, "Fuel Monitor Panel", "Bingo Selection", false, "%0.1f"));
            // * * *  384 is a value which represents the rotation of the bingo pointer (from 0 to 1)

            AddFunction(new Digits4Display(this, "Fuel Monitor Panel", "2011", "Fuel Monitor Panel", "Left Tank display", "Fuel left tank quantity"));
            AddFunction(new Digits4Display(this, "Fuel Monitor Panel", "2012", "Fuel Monitor Panel", "Right Tank display", "Fuel right tank quantity"));
            AddFunction(new Digits4Display(this, "Fuel Monitor Panel", "2013", "Fuel Monitor Panel", "Bingo value display", "Fuel Bingo amount"));
            AddFunction(new FuelTotalDisplay(this));

            #endregion Fuel Monitor Panel
            #region Landing Gear Panel
            AddFunction(new PushButton(this, devices.WCAS.ToString("d"), Commands.ldg_commands.warn_tone_sil_btn.ToString("d"), "325", "Landing Gear Panel", "Landing Gear Warning Tone Silence Switch", "%1d"));
            // Line 100: elements["PTN_324"] = LDG_Gear_Control_Lever(_("Landing Gear Lever"),						devices.LGS,			ldg_commands.Gear_lever,				324, 2.5)
            AddFunction(new FlagValue(this, "330", "Landing Gear Panel", "Left Gear Down Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "331", "Landing Gear Panel", "Nose Gear Down Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "332", "Landing Gear Panel", "Right Gear Down Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "333", "Landing Gear Panel", "Gear Lever Warning Indicator", "True when indicator is lit", "%1d"));

            AddFunction(new FlagValue(this, "338", "Landing Gear Panel", "Half Flaps Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "339", "Landing Gear Panel", "Full Flaps Indicator", "True when indicator is lit", "%1d"));


            #endregion Landing Gear Panel
            #region Flight Instruments
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "335", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fltinst_commands.pitch_ratio_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.fltinst_commands.pitch_ratio_sw.ToString("d")) }, "Flight Instruments", "Pitch Ratio switch", "%0.1f"));
            AddFunction(new GadrocsWorkshop.Helios.Interfaces.DCS.AH64D.Functions.Altimeter(this));
            CalibrationPointCollectionDouble airspeedScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 1000d);
            AddFunction(new ScaledNetworkValue(this, "345", airspeedScale, "Flight Instruments", "IAS Airspeed", "Current indicated air speed of the aircraft.", "", BindingValueUnits.Knots));

            CalibrationPointCollectionDouble vviScale = new CalibrationPointCollectionDouble(-0.6d, -6000d, 0.6d, 6000d);
            vviScale.Add(new CalibrationPointDouble(0d, 0d));
            AddFunction(new ScaledNetworkValue(this, "362", vviScale, "Flight Instruments", "VVI", "Vertical velocity indicator -6000 to +6000.", "", BindingValueUnits.FeetPerMinute));

            #endregion Flight Instruments
            #region Threat Indicators (Pilot)
            AddFunction(new FlagValue(this, "403", "Threat Indicators (Pilot)", "EMS Limit Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "404", "Threat Indicators (Pilot)", "AI Threat Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "405", "Threat Indicators (Pilot)", "SAM Threat Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "406", "Threat Indicators (Pilot)", "Low Altitude Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "407", "Threat Indicators (Pilot)", "TF Fail Indicator", "True when indicator is lit", "%1d"));
            #endregion Threat Indicators (Pilot)
            #region Warning Panel (Pilot)
            //  +-----------------------+-----------------------+
            //  | PROGRAM (GR)     411? | MINIMUM         412?  |
            //  +-----------------------+-----------------------+
            //  | CHAFF            413? | FLARE           414?  |
            //  +-----------------------+-----------------------+
            //  | EMER BST ON      415  | BST SYS MAL     416   |
            //  +-----------------------+-----------------------+
            //  | NUCLEAR          420? | FUEL LOW        417?  |
            //  +-----------------------+-----------------------+
            //  | L GEN            419  | R GEN           418   |
            //  +-----------------------+-----------------------+
            //  | ENGINE           421  | FLT CONTR       422   |
            //  +-----------------------+-----------------------+
            //  | HYD              423? | AV BIT          424?  |
            //  +-----------------------+-----------------------+
            //  | DSPFLOLO         425? | OXYGEN          426   |
            //  +-----------------------+-----------------------+
            //  | SPARE               ? | SPARE              ?  |
            //  +-----------------------+-----------------------+
            //  | SPARE               ? | SPARE              ?  |
            //  +-----------------------+-----------------------+

            AddFunction(new FlagValue(this, "411", "Warning Panel (Pilot)", "Program Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "412", "Warning Panel (Pilot)", "Minimum Warning Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "413", "Warning Panel (Pilot)", "Chaff Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "414", "Warning Panel (Pilot)", "Flare Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "415", "Warning Panel (Pilot)", "Emergency BST On Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "416", "Warning Panel (Pilot)", "BST System Malfunction Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "417", "Warning Panel (Pilot)", "FUEL LOW Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "418", "Warning Panel (Pilot)", "Right Generator Warning", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "419", "Warning Panel (Pilot)", "Left Generator Warning", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "420", "Warning Panel (Pilot)", "NUCLEAR Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "421", "Warning Panel (Pilot)", "Engine Warning Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "422", "Warning Panel (Pilot)", "Flight Control Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "423", "Warning Panel (Pilot)", "Hydraulics Warning Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "424", "Warning Panel (Pilot)", "AV BIT Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "425", "Warning Panel (Pilot)", "DSPFLOLO Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "426", "Warning Panel (Pilot)", "Oxygen Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "429", "Warning Panel (Pilot)", "SPARE #1 Indicator", "True when indicator is lit", "%1d"));  // guess
            AddFunction(new FlagValue(this, "430", "Warning Panel (Pilot)", "SPARE #2 Indicator", "True when indicator is lit", "%1d"));  // guess
            AddFunction(new FlagValue(this, "431", "Warning Panel (Pilot)", "SPARE #3 Indicator", "True when indicator is lit", "%1d"));  // guess
            AddFunction(new FlagValue(this, "432", "Warning Panel (Pilot)", "SPARE #4 Indicator", "True when indicator is lit", "%1d"));  // guess
            #endregion Warning Panel (Pilot)
            #region Indicators (others) Pilot
            AddFunction(new FlagValue(this, "409", "Indicators (Pilot)", "Laser Armed Indicator", "True when indicator is lit", "%1d"));  // guess
            AddFunction(new FlagValue(this, "410", "Indicators (Pilot)", "A/P Indicator", "True when indicator is lit", "%1d"));  // guess
            AddFunction(new FlagValue(this, "486", "Unknown Indicators (Pilot)", "Indicator " + "486", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "487", "Unknown Indicators (Pilot)", "Indicator " + "487", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "488", "Unknown Indicators (Pilot)", "Indicator " + "488", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "489", "Unknown Indicators (Pilot)", "Indicator " + "489", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "490", "Unknown Indicators (Pilot)", "Indicator " + "490", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "491", "Unknown Indicators (Pilot)", "Indicator " + "491", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "492", "Unknown Indicators (Pilot)", "Indicator " + "492", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "493", "Indicators (Pilot)", "Hook Down Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "494", "Unknown Indicators (Pilot)", "Indicator " + "494", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "495", "Unknown Indicators (Pilot)", "Indicator " + "495", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "496", "Indicators (Pilot)", "Bingo Fuel Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "497", "Unknown Indicators (Pilot)", "Indicator " + "497", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "498", "Unknown Indicators (Pilot)", "Indicator " + "498", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "499", "Indicators (Pilot)", "Anti-Skid Disabled Indicator ", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "500", "Unknown Indicators (Pilot)", "Indicator " + "500", "True when indicator is lit", "%1d"));
            #endregion Indicators (others) Pilot
            #region Emergency Jettison and Misc Handles
            AddFunction(new Switch(this, devices.LGS.ToString("d"), "336", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.arr_hook_lever.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.arr_hook_lever.ToString("d")) }, "Emergency Jettison and Misc Handles", "Arresting Hook Handle", "%0.1f"));
            AddFunction(new Switch(this, devices.LGS.ToString("d"), "341", new SwitchPosition[] { new SwitchPosition("1.0", "Brake", Commands.misc_commands.em_bk_steer_lever.ToString("d")), new SwitchPosition("0.0", "Steering Handle", Commands.misc_commands.em_bk_steer_lever.ToString("d")) }, "Emergency Jettison and Misc Handles", "Emergency Brake/Steering Handle", "%0.1f"));
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "342", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.rud_adj_lever.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.rud_adj_lever.ToString("d")) }, "Emergency Jettison and Misc Handles", "Rudder Pedal Ajust Handle", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "427", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.em_vent_lever.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.em_vent_lever.ToString("d")) }, "Emergency Jettison and Misc Handles", "Emergency Vent Handle", "%0.1f"));
            AddFunction(new PushButton(this, devices.WCAS.ToString("d"), Commands.misc_commands.master_caution_btn.ToString("d"), "401", "Emergency Jettison and Misc Handles", "Master Caution Button", "%1d"));
            AddFunction(new FlagValue(this, "402", "Emergency Jettison and Misc Handles", "Master Warning Indicator", "True when indicator is lit", "%1d"));

            #endregion Emergency Jettison and Misc Handles
            #region Jet Fuel Starter/Brake Panel
            AddFunction(new Switch(this, devices.LGS.ToString("d"), "387", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.park_brake_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.park_brake_sw.ToString("d")) }, "Jet Fuel Starter/Brake Panel", "Parking Brake Switch", "%0.1f"));
            #endregion Jet Fuel Starter/Brake Panel
            #region LEFT MPD
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_01.ToString("d"), "204", "Left MPD (Pilot)", "Push Button 1", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_02.ToString("d"), "203", "Left MPD (Pilot)", "Push Button 2", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_03.ToString("d"), "202", "Left MPD (Pilot)", "Push Button 3", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_04.ToString("d"), "201", "Left MPD (Pilot)", "Push Button 4", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_05.ToString("d"), "200", "Left MPD (Pilot)", "Push Button 5", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_06.ToString("d"), "219", "Left MPD (Pilot)", "Push Button 6", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_07.ToString("d"), "218", "Left MPD (Pilot)", "Push Button 7", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_08.ToString("d"), "217", "Left MPD (Pilot)", "Push Button 8", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_09.ToString("d"), "216", "Left MPD (Pilot)", "Push Button 9", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_10.ToString("d"), "215", "Left MPD (Pilot)", "Push Button 10", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_11.ToString("d"), "214", "Left MPD (Pilot)", "Push Button 11", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_12.ToString("d"), "213", "Left MPD (Pilot)", "Push Button 12", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_13.ToString("d"), "212", "Left MPD (Pilot)", "Push Button 13", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_14.ToString("d"), "211", "Left MPD (Pilot)", "Push Button 14", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_15.ToString("d"), "210", "Left MPD (Pilot)", "Push Button 15", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_16.ToString("d"), "209", "Left MPD (Pilot)", "Push Button 16", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_17.ToString("d"), "208", "Left MPD (Pilot)", "Push Button 17", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_18.ToString("d"), "207", "Left MPD (Pilot)", "Push Button 18", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_19.ToString("d"), "206", "Left MPD (Pilot)", "Push Button 19", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FLEFT.ToString("d"), Commands.mfdg_commands.Button_20.ToString("d"), "205", "Left MPD (Pilot)", "Push Button 20", "%1d"));
            AddFunction(new Switch(this, devices.MPD_FLEFT.ToString("d"), "221", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0") }, "Left MPD (Pilot)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MPD_FLEFT.ToString("d"), "222", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0") }, "Left MPD (Pilot)", "Brightness Control", "%0.1f"));
            AddFunction(new Switch(this, devices.MPD_FLEFT.ToString("d"), "223", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0") }, "Left MPD (Pilot)", "Contrast Control", "%0.1f"));
            #endregion LEFT MPD
            #region CENTER MPCD
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_01.ToString("d"), "251", "Center MPCD (Pilot)", "Push Button 1", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_02.ToString("d"), "250", "Center MPCD (Pilot)", "Push Button 2", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_03.ToString("d"), "249", "Center MPCD (Pilot)", "Push Button 3", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_04.ToString("d"), "248", "Center MPCD (Pilot)", "Push Button 4", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_05.ToString("d"), "247", "Center MPCD (Pilot)", "Push Button 5", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_06.ToString("d"), "266", "Center MPCD (Pilot)", "Push Button 6", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_07.ToString("d"), "265", "Center MPCD (Pilot)", "Push Button 7", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_08.ToString("d"), "264", "Center MPCD (Pilot)", "Push Button 8", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_09.ToString("d"), "263", "Center MPCD (Pilot)", "Push Button 9", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_10.ToString("d"), "262", "Center MPCD (Pilot)", "Push Button 10", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_11.ToString("d"), "261", "Center MPCD (Pilot)", "Push Button 11", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_12.ToString("d"), "260", "Center MPCD (Pilot)", "Push Button 12", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_13.ToString("d"), "259", "Center MPCD (Pilot)", "Push Button 13", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_14.ToString("d"), "258", "Center MPCD (Pilot)", "Push Button 14", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_15.ToString("d"), "257", "Center MPCD (Pilot)", "Push Button 15", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_16.ToString("d"), "256", "Center MPCD (Pilot)", "Push Button 16", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_17.ToString("d"), "255", "Center MPCD (Pilot)", "Push Button 17", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_18.ToString("d"), "254", "Center MPCD (Pilot)", "Push Button 18", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_19.ToString("d"), "253", "Center MPCD (Pilot)", "Push Button 19", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_FCENTER.ToString("d"), Commands.mfdg_commands.Button_20.ToString("d"), "252", "Center MPCD (Pilot)", "Push Button 20", "%1d"));
            AddFunction(new Switch(this, devices.MPCD_FCENTER.ToString("d"), "267", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0") }, "Center MPCD (Pilot)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MPCD_FCENTER.ToString("d"), "268", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0") }, "Center MPCD (Pilot)", "Brightness Control", "%0.1f"));
            AddFunction(new Switch(this, devices.MPCD_FCENTER.ToString("d"), "269", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0") }, "Center MPCD (Pilot)", "Contrast Control", "%0.1f"));
            #endregion CENTER MPCD
            #region RIGHT MFD
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_01.ToString("d"), "228", "Right MFD (Pilot)", "Push Button 1", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_02.ToString("d"), "227", "Right MFD (Pilot)", "Push Button 2", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_03.ToString("d"), "226", "Right MFD (Pilot)", "Push Button 3", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_04.ToString("d"), "225", "Right MFD (Pilot)", "Push Button 4", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_05.ToString("d"), "224", "Right MFD (Pilot)", "Push Button 5", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_06.ToString("d"), "243", "Right MFD (Pilot)", "Push Button 6", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_07.ToString("d"), "242", "Right MFD (Pilot)", "Push Button 7", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_08.ToString("d"), "241", "Right MFD (Pilot)", "Push Button 8", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_09.ToString("d"), "240", "Right MFD (Pilot)", "Push Button 9", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_10.ToString("d"), "239", "Right MFD (Pilot)", "Push Button 10", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_11.ToString("d"), "238", "Right MFD (Pilot)", "Push Button 11", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_12.ToString("d"), "237", "Right MFD (Pilot)", "Push Button 12", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_13.ToString("d"), "236", "Right MFD (Pilot)", "Push Button 13", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_14.ToString("d"), "235", "Right MFD (Pilot)", "Push Button 14", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_15.ToString("d"), "234", "Right MFD (Pilot)", "Push Button 15", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_16.ToString("d"), "233", "Right MFD (Pilot)", "Push Button 16", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_17.ToString("d"), "232", "Right MFD (Pilot)", "Push Button 17", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_18.ToString("d"), "231", "Right MFD (Pilot)", "Push Button 18", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_19.ToString("d"), "230", "Right MFD (Pilot)", "Push Button 19", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_FRIGHT.ToString("d"), Commands.mfdg_commands.Button_20.ToString("d"), "229", "Right MFD (Pilot)", "Push Button 20", "%1d"));
            AddFunction(new Switch(this, devices.MPD_FRIGHT.ToString("d"), "244", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0") }, "Right MFD (Pilot)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MPD_FRIGHT.ToString("d"), "245", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0") }, "Right MFD (Pilot)", "Brightness Control", "%0.1f"));
            AddFunction(new Switch(this, devices.MPD_FRIGHT.ToString("d"), "246", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0") }, "Right MFD (Pilot)", "Contrast Control", "%0.1f"));
            #endregion RIGHT MFD
            #region NUC
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "450", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.nuc_commands.nuc_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.nuc_commands.nuc_cover.ToString("d")) }, "NUC", "Nuclear Consent Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "451", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.nuc_commands.nuc_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.nuc_commands.nuc_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.nuc_commands.nuc_sw.ToString("d")) }, "NUC", "Nuclear Consent Switch", "%0.1f"));
            #endregion NUC
            #region CAS
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "452", SwitchPositions.Create(3, 0d, 0.5d, Commands.cas_commands.yaw_sw.ToString("d"), "Posn", "%0.3f"), "CAS", "Yaw CAS Switch, when ON (LMB) RESET/(RMB) OFF", "%0.3f"));
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "453", SwitchPositions.Create(3, 0d, 0.5d, Commands.cas_commands.roll_sw.ToString("d"), "Posn", "%0.3f"), "CAS", "Roll CAS Switch, when ON (LMB) RESET/(RMB) OFF", "%0.3f"));
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "454", SwitchPositions.Create(3, 0d, 0.5d, Commands.cas_commands.pitch_sw.ToString("d"), "Posn", "%0.3f"), "CAS", "Pitch CAS Switch, when ON (LMB) RESET/(RMB) OFF", "%0.3f"));
            AddFunction(new PushButton(this, devices.FLCTRL.ToString("d"), Commands.cas_commands.bit_button.ToString("d"), "455", "CAS", "BIT Button", "%1d"));
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "456", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.cas_commands.tf_couple_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.cas_commands.tf_couple_sw.ToString("d")) }, "CAS", "TF Couple Switch", "%0.1f"));
            AddFunction(new PushButton(this, devices.FLCTRL.ToString("d"), Commands.cas_commands.to_button.ToString("d"), "457", "CAS", "T/O Trim Button", "%1d"));
            AddFunction(new FlagValue(this, "458", "CAS", "T/O Trim Indicator", "True when indicator is lit", "%1d"));

            #endregion CAS
            #region MISC
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "534", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.roll_ratio_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.roll_ratio_sw.ToString("d")) }, "MISC", "Roll Ratio Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "535", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.left_inlet_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.left_inlet_sw.ToString("d")) }, "MISC", "Left Inlet Ramp Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "536", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.right_inlet_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.right_inlet_sw.ToString("d")) }, "MISC", "Right Inlet Ramp Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.LGS.ToString("d"), "537", SwitchPositions.Create(3, 0d, 0.5d, Commands.misc_commands.anti_skid_sw.ToString("d"), "Posn", "%0.3f"), "MISC", "Anti Skid Switch", "%0.3f"));
            #endregion MISC
            #region FUEL
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "527", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fuelpnl_commands.fueltrnfr_wing_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.fuelpnl_commands.fueltrnfr_wing_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.fuelpnl_commands.fueltrnfr_wing_sw.ToString("d")) }, "FUEL", "Fuel Control: Wing Tanks", "%0.1f"));
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "528", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fuelpnl_commands.fueltrnfr_ctr_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.fuelpnl_commands.fueltrnfr_ctr_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.fuelpnl_commands.fueltrnfr_ctr_sw.ToString("d")) }, "FUEL", "Fuel Control: Centerline Tank", "%0.1f"));
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "529", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fuelpnl_commands.fueltrnfr_cft_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.fuelpnl_commands.fueltrnfr_cft_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.fuelpnl_commands.fueltrnfr_cft_sw.ToString("d")) }, "FUEL", "Fuel Control: Conformal Tanks", "%0.1f"));
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "530", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fuelpnl_commands.fuel_dump_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.fuelpnl_commands.fuel_dump_sw.ToString("d")) }, "FUEL", "Fuel Dump", "%0.1f"));
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "531", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fuelpnl_commands.fuel_cft_emergtrf_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.fuelpnl_commands.fuel_cft_emergtrf_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.fuelpnl_commands.fuel_cft_emergtrf_sw.ToString("d")) }, "FUEL", "Conformal Tanks Emergency Transfer", "%0.1f"));
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "532", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fuelpnl_commands.fuel_ext_trfr_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.fuelpnl_commands.fuel_ext_trfr_sw.ToString("d")) }, "FUEL", "External Fuel Transfer", "%0.1f"));
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "533", SwitchPositions.Create(3, 0d, 0.5d, Commands.fuelpnl_commands.fuel_AIR_sw.ToString("d"), "Posn", "%0.3f"), "FUEL", "A/R Slipway", "%0.3f"));
            #endregion FUEL
            #region Throttle Quadrant
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "459", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fltctrl_commands.Flaps_Control_SW.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.fltctrl_commands.Flaps_Control_SW.ToString("d")) }, "Throttle Quadrant", "Flaps Control Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "460", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fltctrl_commands.rudder_trim_sw.ToString("d"), Commands.fltctrl_commands.rudder_trim_sw.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", Commands.fltctrl_commands.rudder_trim_sw.ToString("d"), Commands.fltctrl_commands.rudder_trim_sw.ToString("d"), "0.0", "0.0") }, "Throttle Quadrant", "Rudder Trim Switch", "%0.1f"));
            AddFunction(new PushButton(this, devices.DEEC.ToString("d"), Commands.engctrl_commands.left_eng_finger_lift.ToString("d"), "697", "Throttle Quadrant", "Left Throttle Finger Lift", "%1d"));
            AddFunction(new PushButton(this, devices.DEEC.ToString("d"), Commands.engctrl_commands.right_eng_finger_lift.ToString("d"), "698", "Throttle Quadrant", "Right Throttle Finger Lift", "%1d"));
            #endregion Throttle Quadrant
            #region VOL
            AddFunction(new Axis(this, devices.TEWS.ToString("d"), Commands.volctrl_commands.caution_vol.ToString("d"), "502", 0.1d, 0.0d, 1.0d, "VOL", "Caution Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.TEWS.ToString("d"), Commands.volctrl_commands.launch_vol.ToString("d"), "503", 0.1d, 0.0d, 1.0d, "VOL", "Launch Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.ICS.ToString("d"), Commands.volctrl_commands.ics_vol.ToString("d"), "504", 0.1d, 0.0d, 1.0d, "VOL", "ICS Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.PACS.ToString("d"), Commands.volctrl_commands.wpn_vol.ToString("d"), "505", 0.1d, 0.0d, 1.0d, "VOL", "WPN Voume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.ILS.ToString("d"), Commands.volctrl_commands.ils_vol.ToString("d"), "506", 0.1d, 0.0d, 1.0d, "VOL", "ILS Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.TACAN.ToString("d"), Commands.volctrl_commands.tacan_vol.ToString("d"), "507", 0.1d, 0.0d, 1.0d, "VOL", "TACAN Volume", false, "%0.1f"));
            #endregion VOL
            #region MICS
            AddFunction(new Switch(this, devices.ICS.ToString("d"), "508", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.micsctrl_commands.crypto_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.micsctrl_commands.crypto_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.micsctrl_commands.crypto_sw.ToString("d")) }, "MICS", "Crypto Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.ICS.ToString("d"), "509", SwitchPositions.Create(3, 0d, 0.5d, Commands.micsctrl_commands.mic_sw.ToString("d"), "Posn", "%0.3f"), "MICS", "MIC Switch", "%0.3f"));
            AddFunction(new PushButton(this, devices.WCAS.ToString("d"), Commands.micsctrl_commands.vw_tone_sw.ToString("d"), "510", "MICS", "VW/Tone Silence switch", "%1d"));
            #endregion MICS
            #region RADIO
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "511", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.radioctrl_commands.uhf_ant_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.radioctrl_commands.uhf_ant_sw.ToString("d")) }, "RADIO", "UHF Antenna Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "512", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.radioctrl_commands.vhf_ant_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.radioctrl_commands.vhf_ant_sw.ToString("d")) }, "RADIO", "VHF Antenna Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "513", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.radioctrl_commands.tone_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.radioctrl_commands.tone_sw.ToString("d")) }, "RADIO", "Tone Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "514", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.radioctrl_commands.cypher_txt_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.radioctrl_commands.cypher_txt_sw.ToString("d")) }, "RADIO", "Cypher Text Switch", "%0.1f"));
            #endregion RADIO
            #region IFF
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "515", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.iffctrl_commands.mode_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.iffctrl_commands.mode_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.iffctrl_commands.mode_sw.ToString("d")) }, "IFF", "IFF Mode Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "516", SwitchPositions.Create(3, 0d, 0.5d, Commands.iffctrl_commands.rec_sw.ToString("d"), "Posn", "%0.3f"), "IFF", "IFF Reply Switch", "%0.3f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "517", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.iffctrl_commands.master_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.iffctrl_commands.master_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.iffctrl_commands.master_sw.ToString("d")) }, "IFF", "IFF Master Switch", "%0.1f"));
            #endregion IFF
            #region EXT LT
            AddFunction(new Switch(this, devices.EXTLT.ToString("d"), "538", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.extlt_commands.ldg_taxi_light_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.extlt_commands.ldg_taxi_light_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.extlt_commands.ldg_taxi_light_sw.ToString("d")) }, "EXT LT", "Landing/Taxi Light Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLT.ToString("d"), "465", SwitchPositions.Create(7, 0d, 0.167d, Commands.extlt_commands.formation_lt_knob.ToString("d"), "Posn", "%0.3f"), "EXT LT", "Formation Lights Knob", "%0.3f"));
            AddFunction(new Switch(this, devices.EXTLT.ToString("d"), "466", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.extlt_commands.anticoll_lt_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.extlt_commands.anticoll_lt_sw.ToString("d")) }, "EXT LT", "Anti Collison Light Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EXTLT.ToString("d"), "467", SwitchPositions.Create(7, 0d, 0.167d, Commands.extlt_commands.pos_lt_knob.ToString("d"), "Posn", "%0.3f"), "EXT LT", "Position Lights Knob", "%0.3f"));
            AddFunction(new Switch(this, devices.EXTLT.ToString("d"), "468", SwitchPositions.Create(3, 0d, 0.5d, Commands.extlt_commands.vert_tail_lt_sw.ToString("d"), "Posn", "%0.3f"), "EXT LT", "Tail Flood Lights", "%0.3f"));
            #endregion EXT LT
            #region SENSOR
            AddFunction(new Switch(this, devices.TFR.ToString("d"), "469", SwitchPositions.Create(3, 0d, 0.5d, Commands.snsrctrl_commands.tf_rdr_sw.ToString("d"), "Posn", "%0.3f"), "SENSOR", "Terrain Follow Radar Switch", "%0.3f"));
            AddFunction(new Switch(this, devices.ADC.ToString("d"), "470", SwitchPositions.Create(3, 0d, 0.5d, Commands.snsrctrl_commands.rdr_alt_sw.ToString("d"), "Posn", "%0.3f"), "SENSOR", "Radar Altitude Switch", "%0.3f"));
            AddFunction(new Switch(this, devices.AN_APG70.ToString("d"), "471", SwitchPositions.Create(4, 0d, 0.333d, Commands.snsrctrl_commands.rdr_power_sw.ToString("d"), "Posn", "%0.3f"), "SENSOR", "Radar Mode Selector", "%0.3f"));
            AddFunction(new Switch(this, devices.INS.ToString("d"), "472", SwitchPositions.Create(4, 0d, 0.333d, Commands.snsrctrl_commands.ins_knob.ToString("d"), "Posn", "%0.3f"), "SENSOR", "INS Knob", "%0.3f"));
            AddFunction(new Axis(this, devices.NAVPOD.ToString("d"), Commands.snsrctrl_commands.nav_flir_gain_knob.ToString("d"), "473", 0.1d, 0.0d, 1.0d, "SENSOR", "Nav FLIR Gain", false, "%0.1f"));
            AddFunction(new Axis(this, devices.NAVPOD.ToString("d"), Commands.snsrctrl_commands.nav_flir_gain_level.ToString("d"), "474", 0.1d, 0.0d, 1.0d, "SENSOR", "Nav FLIR Level", false, "%0.1f"));
            AddFunction(new Switch(this, devices.NAVPOD.ToString("d"), "475", SwitchPositions.Create(3, 0d, 0.5d, Commands.snsrctrl_commands.nav_flir_sw.ToString("d"), "Posn", "%0.3f"), "SENSOR", "Nav FLIR Switch", "%0.3f"));
            AddFunction(new Switch(this, devices.RDRCTRL_AA.ToString("d"), "476", SwitchPositions.Create(5, 0d, 0.25d, Commands.snsrctrl_commands.jtids_knob.ToString("d"), "Posn", "%0.3f"), "SENSOR", "JTIDS Knob", "%0.3f"));
            AddFunction(new PushButton(this, devices.RDRCTRL_AA.ToString("d"), Commands.snsrctrl_commands.cc_reset_btn.ToString("d"), "477", "SENSOR", "CC Reset", "%1d"));
            #endregion SENSOR
            #region Ground Power
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "478", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.gndpwrctrl_commands.gnd_pwr_2_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.gndpwrctrl_commands.gnd_pwr_2_sw.ToString("d")) }, "Ground Power", "Ground Power 2 Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "479", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.gndpwrctrl_commands.gnd_pwr_3_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.gndpwrctrl_commands.gnd_pwr_3_sw.ToString("d")) }, "Ground Power", "Ground Power 3 Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "480", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.gndpwrctrl_commands.gnd_pwr_4_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.gndpwrctrl_commands.gnd_pwr_4_sw.ToString("d")) }, "Ground Power", "Ground Power 4 Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "481", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.gndpwrctrl_commands.pacs_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.gndpwrctrl_commands.pacs_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.gndpwrctrl_commands.pacs_sw.ToString("d")) }, "Ground Power", "PACS Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "483", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.gndpwrctrl_commands.gnd_pwr_1_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.gndpwrctrl_commands.gnd_pwr_1_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.gndpwrctrl_commands.gnd_pwr_1_sw.ToString("d")) }, "Ground Power", "Ground Power 1 Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "484", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.gndpwrctrl_commands.mpdp_A1U_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.gndpwrctrl_commands.mpdp_A1U_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.gndpwrctrl_commands.mpdp_A1U_sw.ToString("d")) }, "Ground Power", "MPDP/A1U Switch", "%0.1f"));
            #endregion Ground Power
            #region LEFT BULKHEAD PANEL
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "485", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.armtctrl_commands.arm_safety_override_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.armtctrl_commands.arm_safety_override_sw.ToString("d")) }, "LEFT BULKHEAD PANEL", "Armament Safety Override Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "539", new SwitchPosition[] { new SwitchPosition("1.0", "A", Commands.misc_commands.em_ar_cover.ToString("d")), new SwitchPosition("0.0", "R Switch Cover", Commands.misc_commands.em_ar_cover.ToString("d")) }, "LEFT BULKHEAD PANEL", "Emergency A/R Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.AFSS.ToString("d"), "540", new SwitchPosition[] { new SwitchPosition("1.0", "A", Commands.misc_commands.em_ar_sw.ToString("d")), new SwitchPosition("0.0", "R Switch", Commands.misc_commands.em_ar_sw.ToString("d")) }, "LEFT BULKHEAD PANEL", "Emergency A/R Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "518", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.ewws_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.ewws_cover.ToString("d")) }, "LEFT BULKHEAD PANEL", "EWWS Enable Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "519", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.ewws_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.ewws_sw.ToString("d")) }, "LEFT BULKHEAD PANEL", "EWWS Enable Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "520", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.iffctrl_commands.iff_ant_sel_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.iffctrl_commands.iff_ant_sel_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.iffctrl_commands.iff_ant_sel_sw.ToString("d")) }, "LEFT BULKHEAD PANEL", "IFF Antenna Select Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "521", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.seat_adj_sw.ToString("d"), Commands.misc_commands.seat_adj_sw.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", Commands.misc_commands.seat_adj_sw.ToString("d"), Commands.misc_commands.seat_adj_sw.ToString("d"), "0.0", "0.0") }, "LEFT BULKHEAD PANEL", "Seat Adjust Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "522", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.flyup_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.flyup_cover.ToString("d")) }, "LEFT BULKHEAD PANEL", "Flyup Enable Switch cover", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "523", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.flyup_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.flyup_sw.ToString("d")) }, "LEFT BULKHEAD PANEL", "Flyup Enable Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "524", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.nctr_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.nctr_sw.ToString("d")) }, "LEFT BULKHEAD PANEL", "NCTR Enable Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "525", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engctrl_commands.vmax_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engctrl_commands.vmax_cover.ToString("d")) }, "LEFT BULKHEAD PANEL", "VMAX Switch cover", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "526", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engctrl_commands.vmax_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engctrl_commands.vmax_sw.ToString("d")) }, "LEFT BULKHEAD PANEL", "VMAX Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "800", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.seat_arm_handle.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.seat_arm_handle.ToString("d")) }, "LEFT BULKHEAD PANEL", "Seat Arm Handle", "%0.1f"));
            #endregion LEFT BULKHEAD PANEL
            #region OXYGEN
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "551", new SwitchPosition[] { new SwitchPosition("1.0", "Emergency", Commands.oxyctrl_commands.oxy_emer_norm_test_sw.ToString("d")), new SwitchPosition("0.0", "Normal", Commands.oxyctrl_commands.oxy_emer_norm_test_sw.ToString("d")), new SwitchPosition("-1.0", "Test", Commands.oxyctrl_commands.oxy_emer_norm_test_sw.ToString("d")) }, "OXYGEN", "Oxygen Emergency/Normal/Test Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "552", new SwitchPosition[] { new SwitchPosition("1.0", "100%", Commands.oxyctrl_commands.oxy_100_norm_sw.ToString("d")), new SwitchPosition("0.0", "Normal Switch", Commands.oxyctrl_commands.oxy_100_norm_sw.ToString("d")) }, "OXYGEN", "Oxygen 100%/Normal Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "553", SwitchPositions.Create(3, 0d, 0.5d, Commands.oxyctrl_commands.oxy_pbg_on_off_sw.ToString("d"), "Posn", "%0.3f"), "OXYGEN", "Oxygen Supply/Mode Control Switch", "%0.3f"));
            AddFunction(new FlagValue(this, "555", "OXYGEN", "Oxygen Flow Indicator", "True when indicator is lit", "%1d"));

            #endregion OXYGEN
            #region ECS
            AddFunction(new PushButton(this, devices.ECS.ToString("d"), Commands.oxyctrl_commands.oxy_test_btn.ToString("d"), "556", "ECS", "Oxygen Test", "%1d"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "558", SwitchPositions.Create(3, 0d, 0.5d, Commands.ecs_commands.anti_fog_sw.ToString("d"), "Posn", "%0.3f"), "ECS", "Anti-Fog", "%0.3f"));
            #endregion ECS
            #region ENGINE
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "590", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engctrl_commands.left_eng_ctrl_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engctrl_commands.left_eng_ctrl_sw.ToString("d")) }, "ENGINE", "Left Engine Control Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "591", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engctrl_commands.right_eng_ctrl_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engctrl_commands.right_eng_ctrl_sw.ToString("d")) }, "ENGINE", "Right Engine Control Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "592", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engctrl_commands.left_eng_master_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engctrl_commands.left_eng_master_cover.ToString("d")) }, "ENGINE", "Left Engine Master Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "593", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engctrl_commands.left_eng_master_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engctrl_commands.left_eng_master_sw.ToString("d")) }, "ENGINE", "Left Engine Master Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "597", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engctrl_commands.right_eng_master_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engctrl_commands.right_eng_master_cover.ToString("d")) }, "ENGINE", "Right Engine Master Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.DEEC.ToString("d"), "598", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engctrl_commands.right_eng_master_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engctrl_commands.right_eng_master_sw.ToString("d")) }, "ENGINE", "Right Engine Master Switch", "%0.1f"));
            #endregion ENGINE
            #region Generators
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "587", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engpnl_commands.generator_left_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engpnl_commands.generator_left_sw.ToString("d")) }, "Generators", "Left Generator", "%0.1f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "588", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.engpnl_commands.generator_right_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engpnl_commands.generator_right_sw.ToString("d")) }, "Generators", "Right Generator", "%0.1f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "589", SwitchPositions.Create(3, 0d, 0.5d, Commands.engpnl_commands.generator_emerg_sw.ToString("d"), "Posn", "%0.3f"), "Generators", "Emergency Generator", "%0.3f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "594", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", Commands.engpnl_commands.ground_power_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.engpnl_commands.ground_power_sw.ToString("d")), new SwitchPosition("1.0", "Posn 3", Commands.engpnl_commands.ground_power_sw.ToString("d")) }, "Generators", "External Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EPSS.ToString("d"), "595", new SwitchPosition[] { new SwitchPosition("1.0", "ON", Commands.engpnl_commands.starter_gen_sw.ToString("d")), new SwitchPosition("0.0", "OFF", Commands.engpnl_commands.starter_gen_sw.ToString("d")) }, "Generators", "Jet Starter", "%0.1f"));
            AddFunction(new FlagValue(this, "596", "Generators", "Jet Starter Ready Indicator", "True when indicator is lit", "%1d"));

            #endregion Generators
            #region ANTI ICE
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "559", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.ecs_commands.windshield_anti_ice_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.ecs_commands.windshield_anti_ice_sw.ToString("d")) }, "ANTI ICE", "Windshield Anti-ice Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "560", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.ecs_commands.pitot_heat_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.ecs_commands.pitot_heat_sw.ToString("d")) }, "ANTI ICE", "Pitot Heat Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "561", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.ecs_commands.eng_heat_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.ecs_commands.eng_heat_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.ecs_commands.eng_heat_sw.ToString("d")) }, "ANTI ICE", "Engine Heat Switch", "%0.1f"));
            #endregion ANTI ICE
            #region AIR COND
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "562", SwitchPositions.Create(3, 0d, 0.5d, Commands.aircoctrl_commands.airco_auto_man_off_sw.ToString("d"), new string[] { "Auto", "Manual", "Off" }, "%0.3f"), "AIR COND", "Air Conditioning Auto/Manual/Off", "%0.3f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "563", SwitchPositions.Create(3, 0d, 0.5d, Commands.aircoctrl_commands.airco_max_norm_min_sw.ToString("d"), new string[] { "Max", "Norm", "Min" }, "%0.3f"), "AIR COND", "Air Conditioning Max/Norm/Min", "%0.3f"));
            AddFunction(new Axis(this, devices.ECS.ToString("d"), Commands.aircoctrl_commands.airco_cold_hot_knob.ToString("d"), "564", 0.1d, 0.0d, 1.0d, "AIR COND", "Air Conditioning Cold/Hot", false, "%0.1f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "565", SwitchPositions.Create(4, 0d, 0.333d, Commands.aircoctrl_commands.airco_eng_knob.ToString("d"), "Posn", "%0.3f"), "AIR COND", "Air Conditioning Source", "%0.3f"));
            #endregion AIR COND
            #region INT LT
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.console_lt_knob.ToString("d"), "566", 0.1d, 0.0d, 1.0d, "INT LT", "Console Lights", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.inst_pnl_lt_knob.ToString("d"), "567", 0.1d, 0.0d, 1.0d, "INT LT", "Instruments Panel Lights", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.ufc_bcklt_br_knob.ToString("d"), "568", 0.1d, 0.0d, 1.0d, "INT LT", "Gauges/UFC Backlights", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.INTLT.ToString("d"), Commands.intlt_commands.lights_test_sw.ToString("d"), "569", "INT LT", "Warning/Caution Lights Test", "%1d"));
            AddFunction(new Switch(this, devices.INTLT.ToString("d"), "570", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.intlt_commands.compass_lt_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.intlt_commands.compass_lt_sw.ToString("d")) }, "INT LT", "Compass Lights", "%0.1f"));
            AddFunction(new Switch(this, devices.INTLT.ToString("d"), "571", new SwitchPosition[] { new SwitchPosition("1.0", "Day", Commands.intlt_commands.daynite_mode_sw.ToString("d")), new SwitchPosition("0.0", "Night Mode Selector", Commands.intlt_commands.daynite_mode_sw.ToString("d")) }, "INT LT", "Day/Night Mode Selector", "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.chart_lt_knob.ToString("d"), "572", 0.1d, 0.0d, 1.0d, "INT LT", "Charts Spot Light", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.wac_bklt_knob.ToString("d"), "573", 0.5d, 0.0d, 1.0d, "INT LT", "Knob Warning/Caution Lights (RMB to RESET when BRT)", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.flood_lt_knob.ToString("d"), "574", 0.1d, 0.0d, 1.0d, "INT LT", "Storm FLood Lights", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.chart_lt_lamp.ToString("d"), "186", 0.1d, 0.0d, 1.0d, "INT LT", "Chart Spot Lamp", false, "%0.1f"));
            #endregion INT LT
            #region CANOPY
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "599", SwitchPositions.Create(4, 0d, 0.333d, Commands.cnp_commands.cnpy_lever.ToString("d"), "Posn", "%0.3f"), "CANOPY", "Canopy Handle", "%0.3f"));
            AddFunction(new FlagValue(this, "408", "CANOPY", "Canopy Unlocked Indicator (Pilot)", "True when indicator is lit", "%1d"));
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "1479", SwitchPositions.Create(4, 0d, 0.333d, Commands.cnp_commands.rear_cnpy_lever.ToString("d"), "Posn", "%0.3f"), "CANOPY", "Canopy Handle Rear", "%0.3f"));
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "428", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.cnp_commands.em_cnpy_jett_lever.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.cnp_commands.em_cnpy_jett_lever.ToString("d")) }, "CANOPY", "Emergency Canopy Jettison", "%0.1f"));

            AddFunction(new FlagValue(this, "196", "CANOPY", "Shoot Cue Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "197", "CANOPY", "READY Refuelling Indicator", "True when indicator is lit", "%1d"));
            #endregion CANOPY
            #region Flight Instruments (WSO)
            AddFunction(new PushButton(this, devices.WCAS.ToString("d"), Commands.misc_commands.master_caution_btn_rc.ToString("d"), "1176", "Flight Instruments (WSO)", "Master Caution Button", "%1d"));
            AddFunction(new FlagValue(this, "1177", "Flight Instruments (WSO)", "Master Warning Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new Axis(this, devices.FLINST.ToString("d"), Commands.fltinst_commands.rc_art_hor_uncage.ToString("d"), "1354", 0.5d, 0.0d, 1.0d, "Flight Instruments (WSO)", "Knob Backup ADI Cage/Pitch Adjust Knob - Pull and turn to uncage", false, "%0.1f"));
            AddFunction(new Axis(this, devices.FLINST.ToString("d"), Commands.fltinst_commands.rc_alt_adj_knob.ToString("d"), "1364", 0.1d, 0.0d, 1.0d, "Flight Instruments (WSO)", "Altitude adjust", false, "%0.1f"));
            AddFunction(new Axis(this, devices.FLINST.ToString("d"), Commands.fltinst_commands.rc_clk_adj_knob.ToString("d"), "1382", 0.1d, 0.0d, 1.0d, "Flight Instruments (WSO)", "Clock Adjust", false, "%0.1f"));
            AddFunction(new Axis(this, devices.FLINST.ToString("d"), Commands.fltinst_commands.rc_tmr_stop_btn.ToString("d"), "1383", 0.1d, 0.0d, 1.0d, "Flight Instruments (WSO)", "Timer Stop", false, "%0.1f"));
            #endregion Flight Instruments (WSO)
            #region UFC Panel (WSO)
            #region ODU WSO
            AddFunction(new Text(this, "2140", "UFC Panel (WSO)", "Option Line 1", "Text value of Option Line"));
            AddFunction(new Text(this, "2141", "UFC Panel (WSO)", "Option Line 2", "Text value of Option Line"));
            AddFunction(new Text(this, "2142", "UFC Panel (WSO)", "Option Line 3", "Text value of Option Line"));
            AddFunction(new Text(this, "2143", "UFC Panel (WSO)", "Option Line 4", "Text value of Option Line"));
            AddFunction(new Text(this, "2144", "UFC Panel (WSO)", "Option Line 5", "Text value of Option Line"));
            AddFunction(new Text(this, "2145", "UFC Panel (WSO)", "Option Line 6", "Text value of Option Line"));
            AddFunction(new Text(this, "2146", "UFC Panel (WSO)", "Option Line 1 Center Text", "Text value of Option Line"));
            AddFunction(new Text(this, "2147", "UFC Panel (WSO)", "Option Line 6 Scratchpad", "Text value of Option Line"));
            //AddFunction(new Text(this, "2206", "Option Display Unit (WSO)", "Line 3 Left (WSO)", "Text value of ODU Line 3 Left side"));
            //AddFunction(new Text(this, "2207", "Option Display Unit (WSO)", "Line 3 Middle (WSO)", "Text value of ODU Line 3 Center"));
            //AddFunction(new Text(this, "2208", "Option Display Unit (WSO)", "Line 3 Right (WSO)", "Text value of ODU Line 3 Right side"));
            //AddFunction(new Text(this, "2209", "Option Display Unit (WSO)", "Line 4 Left (WSO)", "Text value of ODU Line 4 Left side"));
            //AddFunction(new Text(this, "2210", "Option Display Unit (WSO)", "Line 4 Middle (WSO)", "Text value of ODU Line 4 Center"));
            //AddFunction(new Text(this, "2211", "Option Display Unit (WSO)", "Line 4 Right (WSO)", "Text value of ODU Line 4 Right side"));
            //AddFunction(new Text(this, "2212", "Option Display Unit (WSO)", "Line 5 Left (WSO)", "Text value of ODU Line 5 Left side"));
            //AddFunction(new Text(this, "2213", "Option Display Unit (WSO)", "Line 5 Middle (WSO)", "Text value of ODU Line 5 Center"));
            //AddFunction(new Text(this, "2214", "Option Display Unit (WSO)", "Line 5 Right (WSO)", "Text value of ODU Line 5 Right side"));
            //AddFunction(new Text(this, "2215", "Option Display Unit (WSO)", "Line 6 Left (WSO)", "Text value of ODU Line 6 Left side"));
            //AddFunction(new Text(this, "2216", "Option Display Unit (WSO)", "Line 6 Middle (WSO)", "Text value of ODU Line 6 Center"));
            //AddFunction(new Text(this, "2217", "Option Display Unit (WSO)", "Line 6 Right (WSO)", "Text value of ODU Line 6 Right side"));
            //AddFunction(new Text(this, "2218", "Option Display Unit (WSO)", "Display (WSO)", "Text value of ODU General Display"));
            //AddFunction(new Text(this, "22??", "Option Display Unit (WSO)", "Line 5 Right Decimal (WSO)", "Text value of ODU Line 5 Right Decimal"));
            //AddFunction(new Text(this, "22??", "Option Display Unit (WSO)", "Line 5 Left Decimal (WSO)", "Text value of ODU Line 5 Left Decimal"));

            #endregion ODU WSO 
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_1.ToString("d"), "1293", "UFC Panel (WSO)", "Option Push Button 1 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_2.ToString("d"), "1294", "UFC Panel (WSO)", "Option Push Button 2 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_3.ToString("d"), "1295", "UFC Panel (WSO)", "Option Push Button 3 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_4.ToString("d"), "1296", "UFC Panel (WSO)", "Option Push Button 4 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_5.ToString("d"), "1297", "UFC Panel (WSO)", "Option Push Button 5 Left", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_0.ToString("d"), "1298", "UFC Panel (WSO)", "Option Push Button 1 Right", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_9.ToString("d"), "1299", "UFC Panel (WSO)", "Option Push Button 2 Right", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_8.ToString("d"), "1300", "UFC Panel (WSO)", "Option Push Button 3 Right", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_7.ToString("d"), "1301", "UFC Panel (WSO)", "Option Push Button 4 Right", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PB_6.ToString("d"), "1302", "UFC Panel (WSO)", "Option Push Button 5 Right", "%1d"));
            AddFunction(new RotaryEncoder(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PRESET_LEFT.ToString("d"), "1303", 0.1d, "UFC Panel (WSO)", "Left UHF Preset Channel Selector"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PRESET_SW_LEFT.ToString("d"), "1304", "UFC Panel (WSO)", "Left UHF Preset Channel Pull Switch", "%1d"));
            AddFunction(new RotaryEncoder(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PRESET_RIGHT.ToString("d"), "1305", 0.1d, "UFC Panel (WSO)", "Right UHF Preset Channel Selector"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_PRESET_SW_RIGHT.ToString("d"), "1306", "UFC Panel (WSO)", "Right UHF Preset Channel Pull Switch", "%1d"));

            AddFunction(new Axis(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_VOL_R1.ToString("d"), "1307", 0.1d, 0.0d, 1.0d, "UFC Panel (WSO)", "UHF Radio 1 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_VOL_R2.ToString("d"), "1308", 0.1d, 0.0d, 1.0d, "UFC Panel (WSO)", "UHF Radio 2 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_VOL_R3.ToString("d"), "1309", 0.1d, 0.0d, 1.0d, "UFC Panel (WSO)", "UHF Radio 3 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_VOL_R4.ToString("d"), "1310", 0.1d, 0.0d, 1.0d, "UFC Panel (WSO)", "UHF Radio 4 Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_BRT_CTRL.ToString("d"), "1311", 0.1d, 0.0d, 1.0d, "UFC Panel (WSO)", "UFC LCD Brightness", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_EMIS_LMT.ToString("d"), "1312", "UFC Panel (WSO)", "Emission Limit", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_GREC_CM_LEFT.ToString("d"), "1313", "UFC Panel (WSO)", "Left Guard Receiver Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY_A1.ToString("d"), "1314", "UFC Panel (WSO)", "A1 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY_N2.ToString("d"), "1315", "UFC Panel (WSO)", "N2 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY_B3.ToString("d"), "1316", "UFC Panel (WSO)", "B3 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_GREC_CM_RIGHT.ToString("d"), "1317", "UFC Panel (WSO)", "Right Guard Receiver Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_MARK.ToString("d"), "1318", "UFC Panel (WSO)", "Mark key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY_W4.ToString("d"), "1319", "UFC Panel (WSO)", "W4 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY_M5.ToString("d"), "1320", "UFC Panel (WSO)", "M5 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY_E6.ToString("d"), "1321", "UFC Panel (WSO)", "E6 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_I_P.ToString("d"), "1322", "UFC Panel (WSO)", "IP Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_DOT.ToString("d"), "1323", "UFC Panel (WSO)", "Decimal Point Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY__7.ToString("d"), "1324", "UFC Panel (WSO)", "7 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY_S8.ToString("d"), "1325", "UFC Panel (WSO)", "S8 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY_C9.ToString("d"), "1326", "UFC Panel (WSO)", "C9 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_SHF.ToString("d"), "1327", "UFC Panel (WSO)", "Shift Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_A_P.ToString("d"), "1328", "UFC Panel (WSO)", "AP Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_CLEAR.ToString("d"), "1329", "UFC Panel (WSO)", "Clr Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_KEY__0.ToString("d"), "1330", "UFC Panel (WSO)", "0 Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_DATA.ToString("d"), "1331", "UFC Panel (WSO)", "Data Key", "%1d"));
            AddFunction(new PushButton(this, devices.UFCCTRL_REAR.ToString("d"), Commands.ufc_commands.UFC_MENU.ToString("d"), "1332", "UFC Panel (WSO)", "Menu Key", "%1d"));
            #endregion UFC Panel (WSO)

            #region Warning Panel (WSO)
            AddFunction(new FlagValue(this, "1179", "Warning Panel (WSO)", "Engine Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1180", "Warning Panel (WSO)", "Hydraulics Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1181", "Warning Panel (WSO)", "Flight Control Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1182", "Warning Panel (WSO)", "AV-BIT Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1183", "Warning Panel (WSO)", "Master Arm Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1184", "Warning Panel (WSO)", "A/P Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1185", "Warning Panel (WSO)", "PROGRAM Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1186", "Warning Panel (WSO)", "MINIMUM Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1187", "Warning Panel (WSO)", "Display Flow Low Indicator", "True when indicator is lit", "%1d"));

            AddFunction(new FlagValue(this, "1193", "Warning Panel (WSO)", "Left Generator Indicator", "True when indicator is lit", "%1d"));    //SIC
            AddFunction(new FlagValue(this, "1192", "Warning Panel (WSO)", "Right Generator Indicator", "True when indicator is lit", "%1d"));   //SIC
            AddFunction(new FlagValue(this, "1194", "Warning Panel (WSO)", "EMIS Limit Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1195", "Warning Panel (WSO)", "Fuel Low Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1178", "Warning Panel (WSO)", "Nuclear Indicator", "True when indicator is lit", "%1d"));  // this is a guess
            AddFunction(new FlagValue(this, "1196", "Warning Panel (WSO)", "Unarmed Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1197", "Warning Panel (WSO)", "Chaff Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1198", "Warning Panel (WSO)", "Flare Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1199", "Warning Panel (WSO)", "Oxygen Indicator", "True when indicator is lit", "%1d"));
            #endregion Warning Panel (WSO)
            #region Warning Indicators WSO
            AddFunction(new FlagValue(this, "1171", "Warning Indicators (WSO)", "Engine Fire Left Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1172", "Warning Indicators (WSO)", "Engine Fire Right Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1173", "Warning Indicators (WSO)", "Canopy Unlocked Indicator (Rear)", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1174", "Warning Indicators (WSO)", "Low Altitude Indicator", "True when indicator is lit", "%1d"));

            AddFunction(new FlagValue(this, "1188", "Warning Indicators (WSO)", "AI Threat Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1189", "Warning Indicators (WSO)", "SAM Threat Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1190", "Warning Indicators (WSO)", "OBST Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1191", "Warning Indicators (WSO)", "TF FAIL Indicator", "True when indicator is lit", "%1d"));
            #endregion Warning Indicators WSO
            #region Master Mode Indicators WSO
            AddFunction(new FlagValue(this, "1333", "Master Mode Indicators (WSO)", "A/A Master Mode Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1334", "Master Mode Indicators (WSO)", "A/G Master Mode Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1335", "Master Mode Indicators (WSO)", "NAV Master Mode Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1336", "Master Mode Indicators (WSO)", "INST Master Mode Indicator", "True when indicator is lit", "%1d"));
            #endregion Master Mode Indicators WSO
            #region Landing Gear/Flaps Indicators (WSO)
            AddFunction(new FlagValue(this, "1337", "Landing Gear/Flaps Indicators (WSO)", "Landing Gear Nose Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1338", "Landing Gear/Flaps Indicators (WSO)", "Landing Gear Left Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1339", "Landing Gear/Flaps Indicators (WSO)", "Landing Gear Right Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1340", "Landing Gear/Flaps Indicators (WSO)", "Landing Gear Unsafe Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1343", "Landing Gear/Flaps Indicators (WSO)", "Half Flaps Indicator", "True when indicator is lit", "%1d"));
            AddFunction(new FlagValue(this, "1344", "Landing Gear/Flaps Indicators (WSO)", "Full Flaps Indicator", "True when indicator is lit", "%1d"));
            #endregion Landing Gear/Flaps Indicators (WSO)
            #region Emergency Jettison and Misc Handles (WSO)
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "1346", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.rud_adj_lever_rear.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.rud_adj_lever_rear.ToString("d")) }, "Emergency Jettison and Misc Handles (WSO)", "Rudder Pedal Ajust Handle", "%0.1f"));
            AddFunction(new Switch(this, devices.LGS.ToString("d"), "1341", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.arr_hook_lever_rear.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.arr_hook_lever_rear.ToString("d")) }, "Emergency Jettison and Misc Handles (WSO)", "Arresting Hook Handle", "%0.1f"));
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "1386", SwitchPositions.Create(3, 0d, 0.5d, Commands.cnp_commands.eject_select_valve.ToString("d"), "Posn", "%0.3f"), "Emergency Jettison and Misc Handles (WSO)", "Eject Selector Handle", "%0.3f"));
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "1385", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.cnp_commands.rear_em_cnpy_jett_lever.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.cnp_commands.rear_em_cnpy_jett_lever.ToString("d")) }, "Emergency Jettison and Misc Handles (WSO)", "Emergency Canopy Jettison", "%0.1f"));
            AddFunction(new Switch(this, devices.LGS.ToString("d"), "1342", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.ldg_commands.rc_em_gear_lever.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.ldg_commands.rc_em_gear_lever.ToString("d")) }, "Emergency Jettison and Misc Handles (WSO)", "Emergency Landing Gear Handle", "%0.1f"));
            AddFunction(new Switch(this, devices.LGS.ToString("d"), "1345", new SwitchPosition[] { new SwitchPosition("1.0", "Brake", Commands.misc_commands.em_bk_steer_lever_rear.ToString("d")), new SwitchPosition("0.0", "Steering Handle", Commands.misc_commands.em_bk_steer_lever_rear.ToString("d")) }, "Emergency Jettison and Misc Handles (WSO)", "Emergency Brake/Steering Handle", "%0.1f"));
            #endregion Emergency Jettison and Misc Handles (WSO)
            #region LEFT MPCD (WSO)
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_01.ToString("d"), "1204", "Left MPCD (WSO)", "Push Button 1", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_02.ToString("d"), "1203", "Left MPCD (WSO)", "Push Button 2", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_03.ToString("d"), "1202", "Left MPCD (WSO)", "Push Button 3", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_04.ToString("d"), "1201", "Left MPCD (WSO)", "Push Button 4", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_05.ToString("d"), "1200", "Left MPCD (WSO)", "Push Button 5", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_06.ToString("d"), "1219", "Left MPCD (WSO)", "Push Button 6", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_07.ToString("d"), "1218", "Left MPCD (WSO)", "Push Button 7", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_08.ToString("d"), "1217", "Left MPCD (WSO)", "Push Button 8", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_09.ToString("d"), "1216", "Left MPCD (WSO)", "Push Button 9", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_10.ToString("d"), "1215", "Left MPCD (WSO)", "Push Button 10", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_11.ToString("d"), "1214", "Left MPCD (WSO)", "Push Button 11", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_12.ToString("d"), "1213", "Left MPCD (WSO)", "Push Button 12", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_13.ToString("d"), "1212", "Left MPCD (WSO)", "Push Button 13", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_14.ToString("d"), "1211", "Left MPCD (WSO)", "Push Button 14", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_15.ToString("d"), "1210", "Left MPCD (WSO)", "Push Button 15", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_16.ToString("d"), "1209", "Left MPCD (WSO)", "Push Button 16", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_17.ToString("d"), "1208", "Left MPCD (WSO)", "Push Button 17", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_18.ToString("d"), "1207", "Left MPCD (WSO)", "Push Button 18", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_19.ToString("d"), "1206", "Left MPCD (WSO)", "Push Button 19", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_20.ToString("d"), "1205", "Left MPCD (WSO)", "Push Button 20", "%1d"));
            AddFunction(new Switch(this, devices.MPCD_RLEFT.ToString("d"), "1221", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0") }, "Left MPCD (WSO)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MPCD_RLEFT.ToString("d"), "1222", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0") }, "Left MPCD (WSO)", "Brightness Control", "%0.1f"));
            AddFunction(new Switch(this, devices.MPCD_RLEFT.ToString("d"), "1223", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0") }, "Left MPCD (WSO)", "Contrast Control", "%0.1f"));
            #endregion LEFT MPCD (WSO)
            #region LEFT MPD (WSO)
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_01.ToString("d"), "1228", "Left MPD (WSO)", "Push Button 1", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_02.ToString("d"), "1227", "Left MPD (WSO)", "Push Button 2", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_03.ToString("d"), "1226", "Left MPD (WSO)", "Push Button 3", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_04.ToString("d"), "1225", "Left MPD (WSO)", "Push Button 4", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_05.ToString("d"), "1224", "Left MPD (WSO)", "Push Button 5", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_06.ToString("d"), "1243", "Left MPD (WSO)", "Push Button 6", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_07.ToString("d"), "1242", "Left MPD (WSO)", "Push Button 7", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_08.ToString("d"), "1241", "Left MPD (WSO)", "Push Button 8", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_09.ToString("d"), "1240", "Left MPD (WSO)", "Push Button 9", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_10.ToString("d"), "1239", "Left MPD (WSO)", "Push Button 10", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_11.ToString("d"), "1238", "Left MPD (WSO)", "Push Button 11", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_12.ToString("d"), "1237", "Left MPD (WSO)", "Push Button 12", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_13.ToString("d"), "1236", "Left MPD (WSO)", "Push Button 13", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_14.ToString("d"), "1235", "Left MPD (WSO)", "Push Button 14", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_15.ToString("d"), "1234", "Left MPD (WSO)", "Push Button 15", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_16.ToString("d"), "1233", "Left MPD (WSO)", "Push Button 16", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_17.ToString("d"), "1232", "Left MPD (WSO)", "Push Button 17", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_18.ToString("d"), "1231", "Left MPD (WSO)", "Push Button 18", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_19.ToString("d"), "1230", "Left MPD (WSO)", "Push Button 19", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RLEFT.ToString("d"), Commands.mfdg_commands.Button_20.ToString("d"), "1229", "Left MPD (WSO)", "Push Button 20", "%1d"));
            AddFunction(new Switch(this, devices.MPD_RLEFT.ToString("d"), "1244", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0") }, "Left MPD (WSO)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MPD_RLEFT.ToString("d"), "1245", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0") }, "Left MPD (WSO)", "Brightness Control", "%0.1f"));
            AddFunction(new Switch(this, devices.MPD_RLEFT.ToString("d"), "1246", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0") }, "Left MPD (WSO)", "Contrast Control", "%0.1f"));
            #endregion LEFT MPD (WSO)
            #region RIGHT MPD (WSO)
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_01.ToString("d"), "1251", "Right MPD (WSO)", "Push Button 1", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_02.ToString("d"), "1250", "Right MPD (WSO)", "Push Button 2", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_03.ToString("d"), "1249", "Right MPD (WSO)", "Push Button 3", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_04.ToString("d"), "1248", "Right MPD (WSO)", "Push Button 4", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_05.ToString("d"), "1247", "Right MPD (WSO)", "Push Button 5", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_06.ToString("d"), "1266", "Right MPD (WSO)", "Push Button 6", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_07.ToString("d"), "1265", "Right MPD (WSO)", "Push Button 7", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_08.ToString("d"), "1264", "Right MPD (WSO)", "Push Button 8", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_09.ToString("d"), "1263", "Right MPD (WSO)", "Push Button 9", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_10.ToString("d"), "1262", "Right MPD (WSO)", "Push Button 10", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_11.ToString("d"), "1261", "Right MPD (WSO)", "Push Button 11", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_12.ToString("d"), "1260", "Right MPD (WSO)", "Push Button 12", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_13.ToString("d"), "1259", "Right MPD (WSO)", "Push Button 13", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_14.ToString("d"), "1258", "Right MPD (WSO)", "Push Button 14", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_15.ToString("d"), "1257", "Right MPD (WSO)", "Push Button 15", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_16.ToString("d"), "1256", "Right MPD (WSO)", "Push Button 16", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_17.ToString("d"), "1255", "Right MPD (WSO)", "Push Button 17", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_18.ToString("d"), "1254", "Right MPD (WSO)", "Push Button 18", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_19.ToString("d"), "1253", "Right MPD (WSO)", "Push Button 19", "%1d"));
            AddFunction(new PushButton(this, devices.MPD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_20.ToString("d"), "1252", "Right MPD (WSO)", "Push Button 20", "%1d"));
            AddFunction(new Switch(this, devices.MPD_RRIGHT.ToString("d"), "1267", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0") }, "Right MPD (WSO)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MPD_RRIGHT.ToString("d"), "1268", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0") }, "Right MPD (WSO)", "Brightness Control", "%0.1f"));
            AddFunction(new Switch(this, devices.MPD_RRIGHT.ToString("d"), "1269", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0") }, "Right MPD (WSO)", "Contrast Control", "%0.1f"));
            #endregion RIGHT MPD (WSO)
            #region RIGHT MPCD (WSO)
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_01.ToString("d"), "1274", "Right MPCD (WSO)", "Push Button 1", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_02.ToString("d"), "1273", "Right MPCD (WSO)", "Push Button 2", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_03.ToString("d"), "1272", "Right MPCD (WSO)", "Push Button 3", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_04.ToString("d"), "1271", "Right MPCD (WSO)", "Push Button 4", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_05.ToString("d"), "1270", "Right MPCD (WSO)", "Push Button 5", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_06.ToString("d"), "1289", "Right MPCD (WSO)", "Push Button 6", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_07.ToString("d"), "1288", "Right MPCD (WSO)", "Push Button 7", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_08.ToString("d"), "1287", "Right MPCD (WSO)", "Push Button 8", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_09.ToString("d"), "1286", "Right MPCD (WSO)", "Push Button 9", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_10.ToString("d"), "1285", "Right MPCD (WSO)", "Push Button 10", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_11.ToString("d"), "1284", "Right MPCD (WSO)", "Push Button 11", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_12.ToString("d"), "1283", "Right MPCD (WSO)", "Push Button 12", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_13.ToString("d"), "1282", "Right MPCD (WSO)", "Push Button 13", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_14.ToString("d"), "1281", "Right MPCD (WSO)", "Push Button 14", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_15.ToString("d"), "1280", "Right MPCD (WSO)", "Push Button 15", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_16.ToString("d"), "1279", "Right MPCD (WSO)", "Push Button 16", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_17.ToString("d"), "1278", "Right MPCD (WSO)", "Push Button 17", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_18.ToString("d"), "1277", "Right MPCD (WSO)", "Push Button 18", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_19.ToString("d"), "1276", "Right MPCD (WSO)", "Push Button 19", "%1d"));
            AddFunction(new PushButton(this, devices.MPCD_RRIGHT.ToString("d"), Commands.mfdg_commands.Button_20.ToString("d"), "1275", "Right MPCD (WSO)", "Push Button 20", "%1d"));
            AddFunction(new Switch(this, devices.MPCD_RRIGHT.ToString("d"), "1290", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Power.ToString("d"), Commands.mfdg_commands.Switch_Power.ToString("d"), "0.0", "0.0") }, "Right MPCD (WSO)", "Power Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.MPCD_RRIGHT.ToString("d"), "1291", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_BRT.ToString("d"), Commands.mfdg_commands.Switch_BRT.ToString("d"), "0.0", "0.0") }, "Right MPCD (WSO)", "Brightness Control", "%0.1f"));
            AddFunction(new Switch(this, devices.MPCD_RRIGHT.ToString("d"), "1292", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Middle", null), new SwitchPosition("-1.0", "Posn 3", Commands.mfdg_commands.Switch_Cont.ToString("d"), Commands.mfdg_commands.Switch_Cont.ToString("d"), "0.0", "0.0") }, "Right MPCD (WSO)", "Contrast Control", "%0.1f"));
            #endregion RIGHT MPCD (WSO)
            #region LEFT INSTRUMENTS PANEL (WSO)
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "1401", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.nuc_commands.nuc_cover_rc.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.nuc_commands.nuc_cover_rc.ToString("d")) }, "LEFT INSTRUMENTS PANEL (WSO)", "Nuclear Consent Switch Cover", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "1402", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.nuc_commands.nuc_sw_rc.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.nuc_commands.nuc_sw_rc.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.nuc_commands.nuc_sw_rc.ToString("d")) }, "LEFT INSTRUMENTS PANEL (WSO)", "Nuclear Consent Switch", "%0.1f"));
            #endregion LEFT INSTRUMENTS PANEL (WSO)
            #region TPOD Panel (WSO)
            AddFunction(new Switch(this, devices.TGPCTRL.ToString("d"), "1413", SwitchPositions.Create(3, 0d, 0.5d, Commands.snsrctrl_commands.tpod_pwr_sw.ToString("d"), new string[] { "OFF", "STBY", "ON" }, "%0.3f"), "TPOD Panel (WSO)", "TGP Power Switch OFF/STBY/ON", "%0.3f"));
            AddFunction(new Axis(this, devices.TGPCTRL.ToString("d"), Commands.snsrctrl_commands.tpod_flir_gain_knob.ToString("d"), "1414", 0.1d, 0.0d, 1.0d, "TPOD Panel (WSO)", "TGP FLIR Gain", false, "%0.1f"));
            AddFunction(new Axis(this, devices.TGPCTRL.ToString("d"), Commands.snsrctrl_commands.tpod_flir_level_knob.ToString("d"), "1415", 0.1d, 0.0d, 1.0d, "TPOD Panel (WSO)", "TGP FLIR Level", false, "%0.1f"));
            AddFunction(new Switch(this, devices.TGPCTRL.ToString("d"), "1416", new SwitchPosition[] { new SwitchPosition("1.0", "SAFE", Commands.snsrctrl_commands.tpod_laser_sw.ToString("d")), new SwitchPosition("0.0", "ARM", Commands.snsrctrl_commands.tpod_laser_sw.ToString("d")) }, "TPOD Panel (WSO)", "TGP Laser Switch SAFE/ARM", "%0.1f"));
            #endregion TPOD Panel (WSO)
            #region EW Panel (WSO)
            AddFunction(new Switch(this, devices.TEWS.ToString("d"), "1417", new SwitchPosition[] { new SwitchPosition("1.0", "COMBAT", Commands.tews_commands.rwr_ics_mode_sw.ToString("d")), new SwitchPosition("0.0", "TRNG", Commands.tews_commands.rwr_ics_mode_sw.ToString("d")) }, "EW Panel (WSO)", "RWR/ICS Mode Switch COMBAT/TRNG", "%0.1f"));
            AddFunction(new Switch(this, devices.EWS_JMR.ToString("d"), "1418", new SwitchPosition[] { new SwitchPosition("1.0", "XMIT", Commands.tews_commands.pods_mode_sw.ToString("d")), new SwitchPosition("0.0", "STBY", Commands.tews_commands.pods_mode_sw.ToString("d")) }, "EW Panel (WSO)", "ECM PODS Mode Switch XMIT/STBY", "%0.1f"));
            AddFunction(new Switch(this, devices.EWS_JMR.ToString("d"), "1419", SwitchPositions.Create(3, 0d, 0.5d, Commands.tews_commands.ics_mode_sw.ToString("d"), new string[] { "STBY", "AUTO", "MAN" }, "%0.3f"), "EW Panel (WSO)", "ICS Operational Mode Switch STBY/AUTO/MAN", "%0.3f"));
            #endregion EW Panel (WSO)
            #region VOL (WSO)
            AddFunction(new Axis(this, devices.TEWS.ToString("d"), Commands.volctrl_commands.rc_caution_vol.ToString("d"), "1420", 0.1d, 0.0d, 1.0d, "VOL (WSO)", "Caution Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.TEWS.ToString("d"), Commands.volctrl_commands.rc_launch_vol.ToString("d"), "1421", 0.1d, 0.0d, 1.0d, "VOL (WSO)", "Launch Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.EWS_JMR.ToString("d"), Commands.volctrl_commands.rc_ics_vol.ToString("d"), "1422", 0.1d, 0.0d, 1.0d, "VOL (WSO)", "ICS Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.PACS.ToString("d"), Commands.volctrl_commands.rc_wpn_vol.ToString("d"), "1423", 0.1d, 0.0d, 1.0d, "VOL (WSO)", "WPN Voume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.ILS.ToString("d"), Commands.volctrl_commands.rc_ils_vol.ToString("d"), "1424", 0.1d, 0.0d, 1.0d, "VOL (WSO)", "ILS Volume", false, "%0.1f"));
            AddFunction(new Axis(this, devices.TACAN.ToString("d"), Commands.volctrl_commands.rc_tacan_vol.ToString("d"), "1425", 0.1d, 0.0d, 1.0d, "VOL (WSO)", "TACAN Volume", false, "%0.1f"));
            #endregion VOL (WSO)
            #region MICS (WSO)
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "1426", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.micsctrl_commands.rc_crypto_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.micsctrl_commands.rc_crypto_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.micsctrl_commands.rc_crypto_sw.ToString("d")) }, "MICS (WSO)", "Crypto Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "1427", SwitchPositions.Create(3, 0d, 0.5d, Commands.micsctrl_commands.rc_mic_sw.ToString("d"), "Posn", "%0.3f"), "MICS (WSO)", "MIC Switch", "%0.3f"));
            AddFunction(new PushButton(this, devices.WCAS.ToString("d"), Commands.micsctrl_commands.rc_vw_tone_sw.ToString("d"), "1428", "MICS (WSO)", "VW/Tone Silence switch", "%1d"));
            #endregion MICS (WSO)
            #region RADIO (WSO)
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "1429", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.radioctrl_commands.rc_tone_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.radioctrl_commands.rc_tone_sw.ToString("d")) }, "RADIO (WSO)", "Tone Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.FLINST.ToString("d"), "1430", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.radioctrl_commands.rc_cypher_txt_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.radioctrl_commands.rc_cypher_txt_sw.ToString("d")) }, "RADIO (WSO)", "Cypher Text Switch", "%0.1f"));
            #endregion RADIO (WSO)
            #region Throttle Quadrant (WSO)
            AddFunction(new Switch(this, devices.FLCTRL.ToString("d"), "1403", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.fltctrl_commands.rudder_trim_sw_rear.ToString("d"), Commands.fltctrl_commands.rudder_trim_sw_rear.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", Commands.fltctrl_commands.rudder_trim_sw_rear.ToString("d"), Commands.fltctrl_commands.rudder_trim_sw_rear.ToString("d"), "0.0", "0.0") }, "Throttle Quadrant (WSO)", "Rudder Trim Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "1431", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.seat_adj_sw_rc.ToString("d"), Commands.misc_commands.seat_adj_sw_rc.ToString("d"), "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", Commands.misc_commands.seat_adj_sw_rc.ToString("d"), Commands.misc_commands.seat_adj_sw_rc.ToString("d"), "0.0", "0.0") }, "Throttle Quadrant (WSO)", "Seat Adjust Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "802", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.misc_commands.seat_arm_handle_rc.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.misc_commands.seat_arm_handle_rc.ToString("d")) }, "Throttle Quadrant (WSO)", "Seat Arm Handle", "%0.1f"));
            #endregion Throttle Quadrant (WSO)
            #region OXYGEN Control Panel (WSO)
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "1450", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.oxyctrl_commands.wso_oxy_emer_norm_test_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.oxyctrl_commands.wso_oxy_emer_norm_test_sw.ToString("d")), new SwitchPosition("-1.0", "Posn 3", Commands.oxyctrl_commands.wso_oxy_emer_norm_test_sw.ToString("d")) }, "OXYGEN Control Panel (WSO)", "Oxygen Emergency/Normal/Test Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "1451", new SwitchPosition[] { new SwitchPosition("1.0", "", Commands.oxyctrl_commands.wso_oxy_100_norm_sw.ToString("d")), new SwitchPosition("0.0", "Normal Switch", Commands.oxyctrl_commands.wso_oxy_100_norm_sw.ToString("d")) }, "OXYGEN Control Panel (WSO)", "Oxygen 100%/Normal Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.ECS.ToString("d"), "1452", SwitchPositions.Create(3, 0d, 0.5d, Commands.oxyctrl_commands.wso_oxy_pbg_on_off_sw.ToString("d"), "Posn", "%0.3f"), "OXYGEN Control Panel (WSO)", "Oxygen Supply/Mode Control Switch", "%0.3f"));
            AddFunction(new FlagValue(this, "1454", "OXYGEN Control Panel (WSO)", "Oxygen Flow Indicator (WSO)", "True when indicator is lit", "%1d"));
            #endregion OXYGEN Control Panel (WSO)
            #region INT LT Control Panel (WSO)
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.rc_console_lt_knob.ToString("d"), "1456", 0.1d, 0.0d, 1.0d, "INT LT Control Panel (WSO)", "Console Lights", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.rc_inst_pnl_lt_knob.ToString("d"), "1457", 0.1d, 0.0d, 1.0d, "INT LT Control Panel (WSO)", "Instruments Panel Lights", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.rc_ufc_bcklt_br_knob.ToString("d"), "1458", 0.1d, 0.0d, 1.0d, "INT LT Control Panel (WSO)", "UFC Panel Backlights", false, "%0.1f"));
            AddFunction(new PushButton(this, devices.INTLT.ToString("d"), Commands.intlt_commands.rc_lights_test_sw.ToString("d"), "1459", "INT LT Control Panel (WSO)", "Warning/Caution Lights Test", "%1d"));
            AddFunction(new Switch(this, devices.INTLT.ToString("d"), "1460", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.intlt_commands.rc_compass_lt_sw.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.intlt_commands.rc_compass_lt_sw.ToString("d")) }, "INT LT Control Panel (WSO)", "Compass Lights", "%0.1f"));
            AddFunction(new Switch(this, devices.INTLT.ToString("d"), "1461", new SwitchPosition[] { new SwitchPosition("1.0", "Day", Commands.intlt_commands.rc_daynite_mode_sw.ToString("d")), new SwitchPosition("0.0", "Night Mode Selector", Commands.intlt_commands.rc_daynite_mode_sw.ToString("d")) }, "INT LT Control Panel (WSO)", "Displays Day/Night Mode Selector", "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.rc_chart_lt_knob.ToString("d"), "1462", 0.1d, 0.0d, 1.0d, "INT LT Control Panel (WSO)", "Chart Panel Lights", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.rc_wac_bklt_knob.ToString("d"), "1463", 0.5d, 0.0d, 1.0d, "INT LT Control Panel (WSO)", "Knob Warning/Caution Lights (RMB to RESET when BRT)", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.rc_flood_lt_knob.ToString("d"), "1464", 0.1d, 0.0d, 1.0d, "INT LT Control Panel (WSO)", "Storm FLood Lights", false, "%0.1f"));
            AddFunction(new Axis(this, devices.INTLT.ToString("d"), Commands.intlt_commands.rc_chart_lt_lamp.ToString("d"), "188", 0.1d, 0.0d, 1.0d, "INT LT Control Panel (WSO)", "Chart Light", false, "%0.1f"));
            #endregion INT LT Control Panel (WSO)
            #region TEWS Control Panel (WSO)
            AddFunction(new Switch(this, devices.EWS_JMR.ToString("d"), "1465", new SwitchPosition[] { new SwitchPosition("1.0", "ON", Commands.tews_commands.ics_power_sw.ToString("d")), new SwitchPosition("0.0", "OFF Switch", Commands.tews_commands.ics_power_sw.ToString("d")) }, "TEWS Control Panel (WSO)", "ICS ON/OFF Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EWS_JMR.ToString("d"), "1466", new SwitchPosition[] { new SwitchPosition("1.0", "MAN", Commands.tews_commands.ics_set1_sw.ToString("d")), new SwitchPosition("0.0", "AUTO Switch", Commands.tews_commands.ics_set1_sw.ToString("d")) }, "TEWS Control Panel (WSO)", "ICS SET 1 MAN/AUTO Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EWS_JMR.ToString("d"), "1467", new SwitchPosition[] { new SwitchPosition("1.0", "MAN", Commands.tews_commands.ics_set2_sw.ToString("d")), new SwitchPosition("0.0", "AUTO Switch", Commands.tews_commands.ics_set2_sw.ToString("d")) }, "TEWS Control Panel (WSO)", "ICS SET 2 MAN/AUTO Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.EWS_JMR.ToString("d"), "1468", new SwitchPosition[] { new SwitchPosition("1.0", "MAN", Commands.tews_commands.ics_set3_sw.ToString("d")), new SwitchPosition("0.0", "AUTO Switch", Commands.tews_commands.ics_set3_sw.ToString("d")) }, "TEWS Control Panel (WSO)", "ICS SET 3 MAN/AUTO Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.TEWS.ToString("d"), "1469", new SwitchPosition[] { new SwitchPosition("1.0", "ON", Commands.tews_commands.rwr_power_sw.ToString("d")), new SwitchPosition("0.0", "OFF Switch", Commands.tews_commands.rwr_power_sw.ToString("d")) }, "TEWS Control Panel (WSO)", "RWR ON/OFF Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.TEWS.ToString("d"), "1470", new SwitchPosition[] { new SwitchPosition("1.0", "ON", Commands.tews_commands.ewss_power_sw.ToString("d")), new SwitchPosition("0.0", "OFF Switch", Commands.tews_commands.ewss_power_sw.ToString("d")) }, "TEWS Control Panel (WSO)", "EWWS ON/OFF Switch", "%0.1f"));
            AddFunction(new Switch(this, devices.TEWS.ToString("d"), "1471", new SwitchPosition[] { new SwitchPosition("1.0", "TONE", Commands.tews_commands.ewss_tone_sw.ToString("d")), new SwitchPosition("0.0", "DEFEAT Switch", Commands.tews_commands.ewss_tone_sw.ToString("d")) }, "TEWS Control Panel (WSO)", "EWWS TONE/DEFEAT Switch", "%0.1f"));
            #endregion TEWS Control Panel (WSO)
            #region CMD Control Panel (WSO)
            AddFunction(new Switch(this, devices.EWS_CMD.ToString("d"), "1475", SwitchPositions.Create(3, 0d, 0.5d, Commands.tews_commands.cmd_disp_sel_sw.ToString("d"), new string[] { "FLARE", "BOTH", "CHAFF" }, "%0.3f"), "CMD Control Panel (WSO)", "CMD Dispenser Selection Switch FLARE/BOTH/CHAFF", "%0.3f"));
            AddFunction(new Switch(this, devices.EWS_CMD.ToString("d"), "1476", SwitchPositions.Create(5, 0d, 0.25d, Commands.tews_commands.cmd_mode_knob.ToString("d"), new string[] { "OFF", "STBY", "MAN", "SEMI", "AUTO" }, "%0.3f"), "CMD Control Panel (WSO)", "CMD Operational Mode OFF/STBY/MAN/SEMI/AUTO", "%0.3f"));
            AddFunction(new Switch(this, devices.EWS_CMD.ToString("d"), "1477", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", Commands.tews_commands.cmd_jett_cover.ToString("d")), new SwitchPosition("0.0", "Posn 2", Commands.tews_commands.cmd_jett_cover.ToString("d")) }, "CMD Control Panel (WSO)", "Flare Jettison Switch Cover", "%0.1f"));
            AddFunction(new PushButton(this, devices.EWS_CMD.ToString("d"), Commands.tews_commands.cmd_jett_sw.ToString("d"), "1478", "CMD Control Panel (WSO)", "Flare Jettison Switch NORM/JETT", "%1d"));
            #endregion CMD Control Panel (WSO)
            #region MISC CONTROLS (WSO)
            AddFunction(new Switch(this, devices.CNPYSYST.ToString("d"), "1001", new SwitchPosition[] { new SwitchPosition("1.0", "Show", Commands.misc_commands.hide_controls.ToString("d")), new SwitchPosition("0.0", "Hide Controls", Commands.misc_commands.hide_controls.ToString("d")) }, "MISC CONTROLS (WSO)", "Show/Hide Controls", "%0.1f"));
            #endregion MISC CONTROLS (WSO)

            // Unimplemented (yet)

            // Line 102: elements["PTN_337"] = Pull_And_Rotate_Control_Handle(_("Emergency Landing Gear Handle, (LMB)PUSH OR PULL/(RMB)ROTATE"), devices.LGS, ldg_commands.em_gear_lever, 337, ldg_commands.em_gear_lever_rotate, 431, 4.5)
            // Line 121: elements["PTN_386"] = Pull_And_Rotate_Control_Handle(_("JFS Control Handle, (LMB)PULL/(RMB)ROTATE"), devices.EPSS, misc_commands.jfs_lever, 386, misc_commands.jfs_handle_turn, 430, 8.0)
            // Line 644: elements["PTN_1010"] = Mirror_Adjust_Control(_("(LMB) Mirror Rendering Toggle / (SCROLL) Adjust Angle"), devices.CNPYSYST, misc_commands.mirror_center, misc_commands.mirror_center_adjust, nil, 10)
            // Line 645: elements["PTN_1011"] = Mirror_Adjust_Control(_("(LMB) Mirror Rendering Toggle / (SCROLL) Adjust Angle"), devices.CNPYSYST, misc_commands.mirror_left, misc_commands.mirror_left_adjust, nil, 11)
            // Line 646: elements["PTN_1012"] = Mirror_Adjust_Control(_("(LMB) Mirror Rendering Toggle / (SCROLL) Adjust Angle"), devices.CNPYSYST, misc_commands.mirror_right, misc_commands.mirror_right_adjust, nil, 12)

#pragma warning restore CS0162 // Unreachable code detected

        }
    }
}
