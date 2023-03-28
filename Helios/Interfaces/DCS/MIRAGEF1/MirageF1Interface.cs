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

//#define CREATEINTERFACE
namespace GadrocsWorkshop.Helios.Interfaces.DCS.MIRAGEF1
{
    using System;
    using System.Collections.Generic;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.MIRAGEF1.Tools;
    using GadrocsWorkshop.Helios.UDPInterface;
    //using Functions;
    using static GadrocsWorkshop.Helios.NativeMethods;

    /// <summary>
    /// common base for interfaces for DCS MirageF1CE and MirageF1EE, containing only functions that are the
    /// same in both aircraft
    /// </summary>
    public class MirageF1Interface : DCSInterface
    {
        private string _dcsAircraft = $@"{Environment.GetEnvironmentVariable("ProgramFiles")}\Eagle Dynamics\DCS World.openbeta\Mods\Aircraft";
        protected MirageF1Interface(string heliosName, string dcsVehicleName, string exportFunctionsUri)
            : base(heliosName, dcsVehicleName, exportFunctionsUri)
        {

#if (CREATEINTERFACE && DEBUG)
            DCSAircraft = $@"{Environment.GetEnvironmentVariable("userprofile")}\Documents\DCSLua\Mirage-F1";
            InterfaceCreation ic = new InterfaceCreation();
            foreach (string path in new string[] { $@"{DCSAircraft}\Cockpit\Mirage-F1\Mirage-F1_Common\clickabledata_common_F1C.lua", $@"{DCSAircraft}\Cockpit\Common\clickabledata_common.lua" })
            {
                foreach (NetworkFunction nf in ic.CreateFunctionsFromClickable(this, path))
                {
                    AddFunction(nf);
                }
            }
            return;
#endif
        }
        protected void AddFunctions()
        {
            // * * * Creating Interface functions from file: Cockpit\Mirage-F1\Mirage-F1_Common\clickabledata_common_F1C.lua
            #region Armament control panel
            AddFunction(new Switch(this, "1", "590", SwitchPositions.Create(3, 0d, 0.5d, "3580", "Posn", "%0.1f"), "Armament control panel", "Sight selector", "%0.1f"));
            AddFunction(new Switch(this, "1", "592", SwitchPositions.Create(3, 0d, 0.5d, "3581", "Posn", "%0.1f"), "Armament control panel", "Bomb/Rocket selector", "%0.1f"));
            AddFunction(new Switch(this, "1", "593", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3582"), new SwitchPosition("0.0", "Posn 2", "3582") }, "Armament control panel", "MATRA 550 or Sidewinder missile switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "594", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3583"), new SwitchPosition("0.0", "Posn 2", "3583") }, "Armament control panel", "Fore/Aft selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "595", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3584"), new SwitchPosition("0.0", "Posn 2", "3584") }, "Armament control panel", "Auto/Manual firing selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "596", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3585"), new SwitchPosition("0.0", "Posn 2", "3585") }, "Armament control panel", "Single/Salvo selector", "%0.1f"));
            AddFunction(new Switch(this, "1", "597", SwitchPositions.Create(3, 0d, 0.5d, "3586", "Posn", "%0.1f"), "Armament control panel", "Instantaneous/Delay/Safe selector switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3587", "604", "Armament control panel", "Left MATRA R550 or Sidewinder missile pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3588", "606", "Armament control panel", "Left or fuselage MATRA R530 missile pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3589", "608", "Armament control panel", "Air-to-Air guns pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3590", "610", "Armament control panel", "Wing bombs pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3591", "612", "Armament control panel", "Right MATRA R550 or Sidewinder missile pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3592", "614", "Armament control panel", "Right MATRA R530 missile pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3593", "616", "Armament control panel", "Air-to-Ground guns or rockets pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3594", "618", "Armament control panel", "Fuselage bombs pushbutton", "%1d"));
            AddFunction(new Switch(this, "1", "601", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3595"), new SwitchPosition("0.0", "Posn 2", "3595") }, "Armament control panel", "R 530 Missile Normal/Altitude difference selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "603", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3596"), new SwitchPosition("0.0", "Posn 2", "3596") }, "Armament control panel", "Normal/Jammer pursuit switch (No function)", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3597", "602", "Armament control panel", "Armament panel lights test", "%1d"));
            AddFunction(new Switch(this, "1", "598", SwitchPositions.Create(3, 0d, 0.5d, "3613", "Posn", "%0.1f"), "Armament control panel", "Radar selector", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3614", "599", "Armament control panel", "Radar emergency transmission button", "%1d"));
            AddFunction(new Switch(this, "1", "600", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3615"), new SwitchPosition("0.0", "Posn 2", "3615") }, "Armament control panel", "Radar 4 lines/1 line scan switch", "%0.1f"));
            #endregion Armament control panel
            #region Radar indicator scope control box
            AddFunction(new Axis(this, "1", "3618", "632", 0.1d, 0.0d, 1.0d, "Radar indicator scope control box", "Indicator lights brightness", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3620", "633", 0.1d, 0.0d, 1.0d, "Radar indicator scope control box", "Strobe brightness", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3622", "634", 0.1d, 0.0d, 1.0d, "Radar indicator scope control box", "Distance markers brightness", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3624", "635", 0.1d, 0.0d, 1.0d, "Radar indicator scope control box", "Horizon and radial velocity marker brightness", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3626", "636", 0.1d, 0.0d, 1.0d, "Radar indicator scope control box", "Horizon symbol vertical position", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "864", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3628"), new SwitchPosition("0.0", "Posn 2", "3628") }, "Radar indicator scope control box", "Radar cover remove toggle", "%0.1f"));
            #endregion Radar indicator scope control box
            #region Radar detector indicator - Type BF
            AddFunction(new PushButton(this, "1", "3573", "1290", "Radar detector indicator - Type BF", "Button Indicator lights intensity adjusting switch and lights 'T' test button", "%1d"));
            AddFunction(new Axis(this, "1", "3574", "1237", 0.5d, 0.0d, 1.0d, "Radar detector indicator - Type BF", "Lamp Indicator lights intensity adjusting switch and lights 'T' test button", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "1238", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3577", "3577", "0.0"), new SwitchPosition("1.0", "Posn 2", "3576", "3576", "0.0") }, "Radar detector indicator - Type BF", "Radar detector indicator test switch", "%0.1f"));
            #endregion Radar detector indicator - Type BF
            #region Armament master switch
            AddFunction(new Switch(this, "1", "432", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3143"), new SwitchPosition("-1.0", "Posn 2", "3143") }, "Armament master switch", "Armament master switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "433", SwitchPositions.Create(3, 0d, 0.5d, "3144", "Posn", "%0.1f"), "Armament master switch", "Armament master switch", "%0.1f"));
            #endregion Armament master switch
            #region ANTENNA-GYRO switch
            AddFunction(new Switch(this, "1", "408", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3453"), new SwitchPosition("0.0", "Posn 2", "3453") }, "ANTENNA-GYRO switch", "ANTENNA-GYRO switch", "%0.1f"));
            #endregion ANTENNA-GYRO switch
            #region UHF radio (TRT - TRAP 137B)
            AddFunction(new Switch(this, "1", "340", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3455"), new SwitchPosition("0.0", "Posn 2", "3455") }, "UHF radio (TRT - TRAP 137B)", "5W/25W selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "341", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3456"), new SwitchPosition("0.0", "Posn 2", "3456") }, "UHF radio (TRT - TRAP 137B)", "Squelch switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "342", SwitchPositions.Create(3, -1d, 1d, "3457", "Posn", "%0.-1f"), "UHF radio (TRT - TRAP 137B)", "Test selector switch", "%0.-1f"));
            AddFunction(new PushButton(this, "1", "3458", "343", "UHF radio (TRT - TRAP 137B)", "CDE button (not used)", "%1d"));
            AddFunction(new Switch(this, "1", "344", SwitchPositions.Create(5, 0d, 0.25d, "3459", "Posn", "%0.2f"), "UHF radio (TRT - TRAP 137B)", "Function selector", "%0.2f"));
            #endregion UHF radio (TRT - TRAP 137B)
            #region Left wall armament switches
            AddFunction(new PushButton(this, "1", "3475", "361", "Left wall armament switches", "(C + M or SW) R deselection switch", "%1d"));
            AddFunction(new Switch(this, "1", "360", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3476"), new SwitchPosition("0.0", "Posn 2", "3476"), new SwitchPosition("1.0", "Posn 3", "3476") }, "Left wall armament switches", "Telemeter/zone scanning switch", "%0.1f"));
            #endregion Left wall armament switches
            #region Accelerometer reset button and rheostat
            AddFunction(new PushButton(this, "1", "3477", "1288", "Accelerometer reset button and rheostat", "Button Accelerometer reset button and rheostat", "%1d"));
            AddFunction(new Axis(this, "1", "3478", "1289", 0.5d, 0.0d, 1.0d, "Accelerometer reset button and rheostat", "Lamp Accelerometer reset button and rheostat", false, "%0.1f"));
            #endregion Accelerometer reset button and rheostat
            #region Sight system and recording camera
            AddFunction(new Switch(this, "1", "760", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3480"), new SwitchPosition("0.0", "Posn 2", "3480") }, "Sight system and recording camera", "AUTO/MAN intensity selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "761", SwitchPositions.Create(3, 0d, 0.5d, "3481", "Posn", "%0.1f"), "Sight system and recording camera", "Lighting selector switch", "%0.1f"));
            AddFunction(new Axis(this, "1", "3482", "769", 0.1d, 0.0d, 1d, "Sight system and recording camera", "Manual gravity drop selection thumbwheel", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3483", "770", 0.1d, 0.0d, 1.0d, "Sight system and recording camera", "Fixed Reticle intensity rheostat", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3485", "771", 0.1d, 0.0d, 1.0d, "Sight system and recording camera", "Moving and Target Reticles intensity rheostat", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3487", "772", 0.1d, 0.0d, 1.0d, "Sight system and recording camera", "Attitude Reticle intensity rheostat", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3489", "773", "Sight system and recording camera", "Sight system test button", "%1d"));
            AddFunction(new Axis(this, "1", "3490", "774", 0.1d, 0.0d, 1.0d, "Sight system and recording camera", "Exposure time repeater", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3492", "775", 0.1d, 0.0d, 1.0d, "Sight system and recording camera", "Overrun select thumbwheel", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "777", SwitchPositions.Create(2, 0d, 1d, "3494", "Posn", "%0.-1f"), "Sight system and recording camera", "5/16 PPS framing rate selector switch", "%0.-1f"));
            AddFunction(new PushButton(this, "1", "3495", "776", "Sight system and recording camera", "Sight camera test switch", "%1d"));
            #endregion Sight system and recording camera
            #region Radar
            AddFunction(new PushButton(this, "1", "3505", "836", "Radar", "Radar test buttton", "%1d"));
            AddFunction(new Axis(this, "1", "3506", "837", 0.1d, 0.0d, 1.0d, "Radar", "Scope intensity adjustment", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "838", SwitchPositions.Create(8, 0d, 0.143d, "3508", "Posn", "%0.3f"), "Radar", "Radar function selection", "%0.3f"));
            AddFunction(new Axis(this, "1", "3510", "839", 0.1d, 0.0d, 1.0d, "Radar", "Storage adjustment", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3512", "840", "Radar", "Scope erasing", "%1d"));
            AddFunction(new Axis(this, "1", "3513", "841", 0.1d, 0.0d, 1.0d, "Radar", "Polaroid screen adjustment", false, "%0.1f"));
            #endregion Radar
            #region IFF
            AddFunction(new Switch(this, "1", "732", SwitchPositions.Create(9, 0d, 0.125d, "3520", "Posn", "%0.3f"), "IFF", "Mode 1 coding tens selector", "%0.3f"));
            AddFunction(new Switch(this, "1", "733", SwitchPositions.Create(9, 0d, 0.125d, "3522", "Posn", "%0.3f"), "IFF", "Mode 1 coding units selector", "%0.3f"));
            AddFunction(new Switch(this, "1", "734", SwitchPositions.Create(9, 0d, 0.125d, "3524", "Posn", "%0.3f"), "IFF", "Mode 3A coding thousands selector", "%0.3f"));
            AddFunction(new Switch(this, "1", "735", SwitchPositions.Create(9, 0d, 0.125d, "3526", "Posn", "%0.3f"), "IFF", "Mode 3A coding hundreds selector", "%0.3f"));
            AddFunction(new Switch(this, "1", "736", SwitchPositions.Create(9, 0d, 0.125d, "3528", "Posn", "%0.3f"), "IFF", "Mode 3A coding tens selector", "%0.3f"));
            AddFunction(new Switch(this, "1", "737", SwitchPositions.Create(9, 0d, 0.125d, "3530", "Posn", "%0.3f"), "IFF", "Mode 3A coding units selector", "%0.3f"));
            AddFunction(new Switch(this, "1", "744", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3533", "3533", "0.0"), new SwitchPosition("-1.0", "Posn 2", null), new SwitchPosition("1.0", "Posn 3", "3533", "3533", "0.0") }, "IFF", "Position identification selector", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3534", "738", "IFF", "IFF test button", "%1d"));
            AddFunction(new Switch(this, "1", "743", SwitchPositions.Create(4, 0d, 0.3333d, "3535", "Posn", "%0.4f"), "IFF", "Mode 4 selector switch", "%0.4f"));
            AddFunction(new PushButton(this, "1", "3537", "740", "IFF", "Button IFF monitoring light", "%1d"));
            AddFunction(new Axis(this, "1", "3538", "741", 0.5d, 0.0d, 1.0d, "IFF", "Lamp IFF monitoring light", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "739", SwitchPositions.Create(4, 0d, 0.3333d, "3540", "Posn", "%0.4f"), "IFF", "Function selector switch", "%0.4f"));
            AddFunction(new Switch(this, "1", "745", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3542"), new SwitchPosition("0.0", "Posn 2", "3542") }, "IFF", "IFF mode 4 switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3543", "746", "IFF", "Button IFF fault light", "%1d"));
            AddFunction(new Axis(this, "1", "3544", "747", 0.5d, 0.0d, 1.0d, "IFF", "Lamp IFF fault light", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "749", SwitchPositions.Create(3, -1d, 1d, "3546", "Posn", "%0.-1f"), "IFF", "AUDIO-LIGHT switch", "%0.-1f"));
            AddFunction(new Switch(this, "1", "750", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3547"), new SwitchPosition("0.0", "Posn 2", "3547") }, "IFF", "M-1 mode switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "751", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3548"), new SwitchPosition("0.0", "Posn 2", "3548") }, "IFF", "M-2 mode switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "752", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3549"), new SwitchPosition("0.0", "Posn 2", "3549") }, "IFF", "M-3 mode switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "753", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3550"), new SwitchPosition("0.0", "Posn 2", "3550") }, "IFF", "M-4 mode switch", "%0.1f"));
            #endregion IFF

            // * * * Creating Interface functions from file: Cockpit\Common\clickabledata_common.lua
            #region Pilot's stick hide/unhide
            AddFunction(new Switch(this, "1", "34", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3001"), new SwitchPosition("0.0", "Posn 2", "3001") }, "Pilot's stick hide/unhide", "Hide Stick toggle", "%0.1f"));
            #endregion Pilot's stick hide/unhide
            #region Incidence test switch
            AddFunction(new Switch(this, "1", "98", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3011", "3011", "0.0"), new SwitchPosition("1.0", "Posn 2", "3010", "3010", "0.0") }, "Incidence test switch", "Incidence test switch", "%0.1f"));
            #endregion Incidence test switch
            #region Flying aid and autopilot interlocks test display unit.
            AddFunction(new PushButton(this, "1", "3012", "368", "Flying aid and autopilot interlocks test display unit.", "Flight control test restart button", "%1d"));
            AddFunction(new Switch(this, "1", "369", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3013"), new SwitchPosition("1.0", "Posn 2", "3013") }, "Flying aid and autopilot interlocks test display unit.", "Flight control test switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "370", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3014"), new SwitchPosition("0.0", "Posn 2", "3014") }, "Flying aid and autopilot interlocks test display unit.", "Flight control test switch", "%0.1f"));
            #endregion Flying aid and autopilot interlocks test display unit.
            #region Flight Control System controls
            AddFunction(new PushButton(this, "1", "3020", "89", "Flight Control System controls", "Servo reset button", "%1d"));
            AddFunction(new Switch(this, "1", "97", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3021"), new SwitchPosition("0.0", "Posn 2", "3021") }, "Flight Control System controls", "Stick uncouple switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "95", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3022"), new SwitchPosition("-1.0", "Posn 2", "3022") }, "Flight Control System controls", "Stick uncouple switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "94", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3023"), new SwitchPosition("-1.0", "Posn 2", "3023") }, "Flight Control System controls", "ARTHUR selector switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "96", SwitchPositions.Create(3, 0d, 0.5d, "3024", "Posn", "%0.1f"), "Flight Control System controls", "ARTHUR selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "90", SwitchPositions.Create(3, 0d, 0.5d, "3025", "Posn", "%0.1f"), "Flight Control System controls", "Yaw/Anti-slip switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "91", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3026"), new SwitchPosition("0.0", "Posn 2", "3026") }, "Flight Control System controls", "Pitch switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "406", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3027", "3027", "0.0"), new SwitchPosition("-1.0", "Posn 2", "3028", "3028", "0.0") }, "Flight Control System controls", "Rudder trim control switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3035", "974", "Flight Control System controls", "Autopilot PA button", "%1d"));
            AddFunction(new PushButton(this, "1", "3036", "977", "Flight Control System controls", "Autopilot ALT button", "%1d"));
            AddFunction(new PushButton(this, "1", "3037", "980", "Flight Control System controls", "Autopilot CAP button", "%1d"));
            AddFunction(new PushButton(this, "1", "3038", "982", "Flight Control System controls", "Autopilot R button", "%1d"));
            AddFunction(new PushButton(this, "1", "3039", "987", "Flight Control System controls", "Autopilot G button", "%1d"));
            AddFunction(new Axis(this, "1", "3041", "992", 0.1d, 0.0d, 1.0d, "Flight Control System controls", "Autopilot intensity control", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3042", "973", "Flight Control System controls", "Autopilot control and indicator unit test button", "%1d"));
            #endregion Flight Control System controls
            #region Engine and fuel controls
            AddFunction(new PushButton(this, "1", "3051", "239", "Engine and fuel controls", "Throttle cut/idle switch", "%1d"));
            AddFunction(new PushButton(this, "1", "3052", "240", "Engine and fuel controls", "In-flight relight control", "%1d"));
            AddFunction(new Switch(this, "1", "376", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3053"), new SwitchPosition("0.0", "Posn 2", "3053"), new SwitchPosition("-1.0", "Posn 3", "3053") }, "Engine and fuel controls", "Ignition/Ventilation selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "374", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3054"), new SwitchPosition("-1.0", "Posn 2", "3054") }, "Engine and fuel controls", "Start button cover", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3055", "375", "Engine and fuel controls", "Start button", "%1d"));
            AddFunction(new Switch(this, "1", "380", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3056"), new SwitchPosition("1.0", "Posn 2", "3056") }, "Engine and fuel controls", "LP main cock switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "381", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3057"), new SwitchPosition("0.0", "Posn 2", "3057") }, "Engine and fuel controls", "LP main cock switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "396", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3062"), new SwitchPosition("0.0", "Posn 2", "3062") }, "Engine and fuel controls", "JPT emergency regulation switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "397", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3063"), new SwitchPosition("-1.0", "Posn 2", "3063") }, "Engine and fuel controls", "A/B main cock switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "398", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3064"), new SwitchPosition("0.0", "Posn 2", "3064") }, "Engine and fuel controls", "A/B main cock switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "377", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3065"), new SwitchPosition("0.0", "Posn 2", "3065") }, "Engine and fuel controls", "Starting pump switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "378", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3066"), new SwitchPosition("0.0", "Posn 2", "3066") }, "Engine and fuel controls", "R/H LP pump switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "379", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3067"), new SwitchPosition("0.0", "Posn 2", "3067") }, "Engine and fuel controls", "L/H LP pump switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "754", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3069", "3069", "0.0"), new SwitchPosition("1.0", "Posn 2", "3068", "3068", "0.0") }, "Engine and fuel controls", "Shock-cone manual control switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3070", "755", "Engine and fuel controls", "Shock-cone pushbutton", "%1d"));
            AddFunction(new Switch(this, "1", "591", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3071"), new SwitchPosition("0.0", "Posn 2", "3071") }, "Engine and fuel controls", "Firing fuel dipper switch", "%0.1f"));
            #endregion Engine and fuel controls
            #region Fuel quantity indicator
            AddFunction(new Switch(this, "1", "1144", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3075"), new SwitchPosition("0.0", "Posn 2", "3075") }, "Fuel quantity indicator", "Feeder tank/Fuselage selector switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3076", "1145", "Fuel quantity indicator", "Fuel gauge test button", "%1d"));
            #endregion Fuel quantity indicator
            #region Jettisoning panel
            AddFunction(new Switch(this, "1", "966", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3077"), new SwitchPosition("1.0", "Posn 2", "3077") }, "Jettisoning panel", "Emergency jettison button guard", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3078", "967", "Jettisoning panel", "Emergency jettison button", "%1d"));
            AddFunction(new Switch(this, "1", "968", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3079"), new SwitchPosition("1.0", "Posn 2", "3079") }, "Jettisoning panel", "Selective jettison button guard", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3080", "969", "Jettisoning panel", "Selective jettison button", "%1d"));
            AddFunction(new Switch(this, "1", "970", SwitchPositions.Create(3, -1d, 1d, "3081", "Posn", "%0.-1f"), "Jettisoning panel", "Jettisoning selector switch", "%0.-1f"));
            #endregion Jettisoning panel
            #region Fuel transfer, refuelling and indication
            AddFunction(new Axis(this, "1", "3082", "1150", 0.1d, 0.0d, 1d, "Fuel transfer, refuelling and indication", "Fuel quantity reset thumbwheel", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "1151", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3083"), new SwitchPosition("0.0", "Posn 2", "3083") }, "Fuel transfer, refuelling and indication", "Crossfeed switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "1152", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3084"), new SwitchPosition("0.0", "Posn 2", "3084") }, "Fuel transfer, refuelling and indication", "Emergency transfer switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "1153", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3085"), new SwitchPosition("0.0", "Posn 2", "3085") }, "Fuel transfer, refuelling and indication", "Fuel transfer sequence selector switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3086", "1166", "Fuel transfer, refuelling and indication", "Fuel transfer indicator test", "%1d"));
            #endregion Fuel transfer, refuelling and indication
            #region Radar control stick
            AddFunction(new PushButton(this, "1", "3100", "259", "Radar control stick", "Radar control stick elevation/altitude difference button", "%1d"));
            AddFunction(new Switch(this, "1", "258", SwitchPositions.Create(4, 0d, 0.333d, "3101", "Posn", "%0.3f"), "Radar control stick", "Radar control stick scale selection", "%0.3f"));
            AddFunction(new Switch(this, "1", "257", SwitchPositions.Create(3, 0d, 0.5d, "3103", "Posn", "%0.1f"), "Radar control stick", "Radar control stick scan selection", "%0.1f"));
            #endregion Radar control stick
            #region Alternative PTT
            AddFunction(new PushButton(this, "1", "3120", "405", "Alternative PTT", "Alternative PTT", "%1d"));
            #endregion Alternative PTT
            #region High-lift devices
            AddFunction(new Switch(this, "1", "248", SwitchPositions.Create(3, 0d, 0.5d, "3122", "Posn", "%0.1f"), "High-lift devices", "Slat/Flap lever", "%0.1f"));
            AddFunction(new Switch(this, "1", "399", SwitchPositions.Create(3, -1d, 1d, "3123", "Posn", "%0.-1f"), "High-lift devices", "High-lift device selector switch", "%0.-1f"));
            #endregion High-lift devices
            #region Radio selector unit
            AddFunction(new Axis(this, "1", "3124", "306", 0.1d, 0.0d, 1.0d, "Radio selector unit", "MISS potentiometer", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3126", "307", 0.1d, 0.0d, 1.0d, "Radio selector unit", "TAC potentiometer", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3128", "308", 0.1d, 0.0d, 1.0d, "Radio selector unit", "VOR potentiometer", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "309", SwitchPositions.Create(2, 0d, 1d, "3130", "Posn", "%0.-1f"), "Radio selector unit", "AMPLI 2-1 selector switch", "%0.-1f"));
            AddFunction(new Switch(this, "1", "310", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3131"), new SwitchPosition("1.0", "Posn 2", "3131") }, "Radio selector unit", "U + V pushbutton", "%0.1f"));
            AddFunction(new Axis(this, "1", "3132", "311", 0.1d, 0.0d, 1.0d, "Radio selector unit", "Rotate U + V pushbutton", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "313", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3134"), new SwitchPosition("1.0", "Posn 2", "3134") }, "Radio selector unit", "U pushbutton", "%0.1f"));
            AddFunction(new Axis(this, "1", "3135", "314", 0.1d, 0.0d, 1.0d, "Radio selector unit", "Rotate U pushbutton", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "316", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3137"), new SwitchPosition("1.0", "Posn 2", "3137") }, "Radio selector unit", "RAP + CME pushbutton", "%0.1f"));
            AddFunction(new Axis(this, "1", "3138", "317", 0.1d, 0.0d, 1.0d, "Radio selector unit", "Rotate RAP + CME pushbutton", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "319", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3140"), new SwitchPosition("1.0", "Posn 2", "3140") }, "Radio selector unit", "MKR + TP pushbutton", "%0.1f"));
            AddFunction(new Axis(this, "1", "3141", "320", 0.1d, 0.0d, 1.0d, "Radio selector unit", "Rotate MKR + TP pushbutton", false, "%0.1f"));
            #endregion Radio selector unit
            #region V/UHF radio control unit (TRT - TRAP 136)
            AddFunction(new Switch(this, "1", "272", SwitchPositions.Create(3, -1d, 1d, "3145", "Posn", "%0.-1f"), "V/UHF radio control unit (TRT - TRAP 136)", "Test selector switch", "%0.-1f"));
            AddFunction(new Switch(this, "1", "273", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3146"), new SwitchPosition("0.0", "Posn 2", "3146") }, "V/UHF radio control unit (TRT - TRAP 136)", "SIL switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "274", SwitchPositions.Create(3, 0.1d, 0.1d, "3147", "Posn", "%0.1f"), "V/UHF radio control unit (TRT - TRAP 136)", "Frequency select thumbwheel - Hundreds", "%0.1f"));
            AddFunction(new Switch(this, "1", "275", SwitchPositions.Create(11, 0d, 0.1d, "3149", "Posn", "%0.1f"), "V/UHF radio control unit (TRT - TRAP 136)", "Frequency select thumbwheel - Tens", "%0.1f"));
            AddFunction(new Switch(this, "1", "276", SwitchPositions.Create(11, 0d, 0.1d, "3151", "Posn", "%0.1f"), "V/UHF radio control unit (TRT - TRAP 136)", "Frequency select thumbwheel - Units", "%0.1f"));
            AddFunction(new Switch(this, "1", "277", SwitchPositions.Create(11, 0d, 0.1d, "3153", "Posn", "%0.1f"), "V/UHF radio control unit (TRT - TRAP 136)", "Frequency select thumbwheel - Tenths", "%0.1f"));
            AddFunction(new Switch(this, "1", "278", SwitchPositions.Create(4, 0d, 0.333d, "3155", "Posn", "%0.3f"), "V/UHF radio control unit (TRT - TRAP 136)", "Frequency select thumbwheel - Thousandths", "%0.3f"));
            AddFunction(new Switch(this, "1", "280", SwitchPositions.Create(6, 0d, 0.2d, "3157", "Posn", "%0.1f"), "V/UHF radio control unit (TRT - TRAP 136)", "Function selector", "%0.1f"));
            AddFunction(new Switch(this, "1", "281", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3159"), new SwitchPosition("0.0", "Posn 2", "3159") }, "V/UHF radio control unit (TRT - TRAP 136)", "25W - 5W switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "282", SwitchPositions.Create(3, 0d, 0.5d, "3160", "Posn", "%0.1f"), "V/UHF radio control unit (TRT - TRAP 136)", "Frequency selector switch", "%0.1f"));
            #endregion V/UHF radio control unit (TRT - TRAP 136)
            #region Engine emergency regulation
            AddFunction(new PushButton(this, "1", "3165", "354", "Engine emergency regulation", "Button Emergency regulation light", "%1d"));
            AddFunction(new Axis(this, "1", "3166", "355", 0.5d, 0.0d, 1.0d, "Engine emergency regulation", "Lamp Emergency regulation light", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "357", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3167"), new SwitchPosition("1.0", "Posn 2", "3167") }, "Engine emergency regulation", "Emergency regulation switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "358", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3168"), new SwitchPosition("0.0", "Posn 2", "3168") }, "Engine emergency regulation", "Emergency regulation switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "359", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3170", "3170", "0.0"), new SwitchPosition("1.0", "Posn 2", "3169", "3169", "0.0") }, "Engine emergency regulation", "Emergency regulation control lever", "%0.1f"));
            AddFunction(new Switch(this, "1", "54", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3171"), new SwitchPosition("0.0", "Posn 2", "3171") }, "Engine emergency regulation", "Brake chute control", "%0.1f"));
            #endregion Engine emergency regulation
            #region Canopy controls
            AddFunction(new Switch(this, "1", "55", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3172"), new SwitchPosition("0.0", "Posn 2", "3172") }, "Canopy controls", "Canopy lock control", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3173", "56", "Canopy controls", "Canopy embrittle control", "%1d"));
            AddFunction(new Switch(this, "1", "3", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3174"), new SwitchPosition("0.0", "Posn 2", "3174") }, "Canopy controls", "Canopy hinged handle", "%0.1f"));
            AddFunction(new Switch(this, "1", "233", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3175"), new SwitchPosition("0.0", "Posn 2", "3175") }, "Canopy controls", "Canopy seal valve control lever", "%0.1f"));
            AddFunction(new Switch(this, "1", "2", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3176"), new SwitchPosition("0.0", "Posn 2", "3176") }, "Canopy controls", "Mirrors", "%0.1f"));
            #endregion Canopy controls
            #region Undercarriage, nose wheel steering, brake and anti-skid
            AddFunction(new Switch(this, "1", "85", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3190"), new SwitchPosition("-1.0", "Posn 2", "3190") }, "Undercarriage, nose wheel steering, brake and anti-skid", "U/C safety lever", "%0.1f"));
            AddFunction(new Switch(this, "1", "86", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3191"), new SwitchPosition("0.0", "Posn 2", "3191") }, "Undercarriage, nose wheel steering, brake and anti-skid", "U/C control lever", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3192", "87", "Undercarriage, nose wheel steering, brake and anti-skid", "Anti-retraction override button", "%1d"));
            AddFunction(new Switch(this, "1", "84", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3193"), new SwitchPosition("0.0", "Posn 2", "3193") }, "Undercarriage, nose wheel steering, brake and anti-skid", "Emergency/Parking brake handle", "%0.1f"));
            AddFunction(new Switch(this, "1", "402", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3194"), new SwitchPosition("-1.0", "Posn 2", "3194") }, "Undercarriage, nose wheel steering, brake and anti-skid", "Anti-skid (SPAD) switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "403", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3195"), new SwitchPosition("0.0", "Posn 2", "3195") }, "Undercarriage, nose wheel steering, brake and anti-skid", "Anti-skid (SPAD) switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3196", "756", "Undercarriage, nose wheel steering, brake and anti-skid", "Nose wheel steering high sensitivity button", "%1d"));
            AddFunction(new Switch(this, "1", "757", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3197"), new SwitchPosition("-1.0", "Posn 2", "3197") }, "Undercarriage, nose wheel steering, brake and anti-skid", "Nose wheel steering switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "758", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3198"), new SwitchPosition("0.0", "Posn 2", "3198") }, "Undercarriage, nose wheel steering, brake and anti-skid", "Nose wheel steering switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "107", SwitchPositions.Create(3, 0d, 0.5d, "3199", "Posn", "%0.1f"), "Undercarriage, nose wheel steering, brake and anti-skid", "Emergency U/C handle", "%0.1f"));
            #endregion Undercarriage, nose wheel steering, brake and anti-skid
            #region Hydraulic system controls
            AddFunction(new Switch(this, "1", "88", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3200"), new SwitchPosition("0.0", "Posn 2", "3200") }, "Hydraulic system controls", "Hydraulic pressure selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "475", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3201"), new SwitchPosition("0.0", "Posn 2", "3201") }, "Hydraulic system controls", "Electro-pump switch", "%0.1f"));
            #endregion Hydraulic system controls
            #region Air data system and miscellaneous instrument controls 
            #endregion Air data system and miscellaneous instrument controls 
            #region Clock
            AddFunction(new PushButton(this, "1", "3202", "232", "Clock", "Button Chronometer starting control and clock winding/setting knob", "%1d"));
            AddFunction(new Axis(this, "1", "3203", "67", 0.5d, 0.0d, 1.0d, "Clock", "Lamp Chronometer starting control and clock winding/setting knob", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "231", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3204"), new SwitchPosition("0.0", "Posn 2", "3204") }, "Clock", "Clock winding/setting lever", "%0.1f"));
            #endregion Clock
            #region Incidence indicator
            AddFunction(new Axis(this, "1", "3205", "201", 0.1d, 0.0d, 1.0d, "Incidence indicator", "Incidence indicator lighting rheostat", false, "%0.1f"));
            #endregion Incidence indicator
            #region Air data instrument controls
            AddFunction(new Axis(this, "1", "3207", "1270", 0.1d, 0.0d, 1.0d, "Air data instrument controls", "Mach/Airspeed indicator reference airspeed knob", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3209", "1053", 0.1d, 0.0d, 1d, "Air data instrument controls", "Slaved altimeter barometric pressure setting knob", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3211", "1072", 0.1d, 0.0d, 1d, "Air data instrument controls", "Standby altimeter barometric pressure setting knob", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "477", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3213"), new SwitchPosition("0.0", "Posn 2", "3213") }, "Air data instrument controls", "Probe heater switch", "%0.1f"));
            #endregion Air data instrument controls
            #region Heading and vertical reference system and standby horizon controls
            AddFunction(new Switch(this, "1", "474", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3230"), new SwitchPosition("0.0", "Posn 2", "3230") }, "Heading and vertical reference system and standby horizon controls", "Standby horizon switch", "%0.1f"));
            AddFunction(new Axis(this, "1", "3231", "1116", 0.1d, 0.0d, 1.0d, "Heading and vertical reference system and standby horizon controls", "Spherical indicator day/night selector switch", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3232", "202", "Heading and vertical reference system and standby horizon controls", "Button Spherical indicator pole setting and marker beacon light test", "%1d"));
            AddFunction(new Axis(this, "1", "3233", "1117", 0.5d, 0.0d, 1.0d, "Heading and vertical reference system and standby horizon controls", "Lamp Spherical indicator pole setting and marker beacon light test", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3234", "1132", "Heading and vertical reference system and standby horizon controls", "Button Standby horizon uncage and aircraft model control", "%1d"));
            AddFunction(new Axis(this, "1", "3235", "1133", 0.5d, 0.0d, 1.0d, "Heading and vertical reference system and standby horizon controls", "Lamp Standby horizon uncage and aircraft model control", false, "%0.1f"));
            #endregion Heading and vertical reference system and standby horizon controls
            #region Heading control unit
            AddFunction(new Switch(this, "1", "537", SwitchPositions.Create(4, 0d, 0.3333d, "3236", "Posn", "%0.4f"), "Heading control unit", "Heading and vertical reference system control switch", "%0.4f"));
            AddFunction(new Switch(this, "1", "538", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3238"), new SwitchPosition("0.0", "Posn 2", "3238") }, "Heading control unit", "Emergency gyromagnetic compass switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3239", "540", "Heading control unit", "Heading control unit erection button", "%1d"));
            #endregion Heading control unit
            #region Electrical system controls
            AddFunction(new Switch(this, "1", "113", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3250"), new SwitchPosition("0.0", "Posn 2", "3250") }, "Electrical system controls", "Battery switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "114", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3251"), new SwitchPosition("0.0", "Posn 2", "3251") }, "Electrical system controls", "Alternator 1 switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "115", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3252"), new SwitchPosition("0.0", "Posn 2", "3252") }, "Electrical system controls", "Alternator 2 switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3253", "116", "Electrical system controls", "TR reset button", "%1d"));
            AddFunction(new Switch(this, "1", "117", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3255", "3255", "0.0", "0.0"), new SwitchPosition("0.0", "Posn 2", null), new SwitchPosition("-1.0", "Posn 3", "3254", "3254", "0.0", "0.0") }, "Electrical system controls", "Inverter selector switch", "%0.1f"));
            #endregion Electrical system controls
            #region Warning lights
            AddFunction(new PushButton(this, "1", "3265", "920", "Warning lights", "Master failure warning light", "%1d"));
            AddFunction(new PushButton(this, "1", "3266", "956", "Warning lights", "Button Combat flaps light", "%1d"));
            AddFunction(new Axis(this, "1", "3267", "957", 0.5d, 0.0d, 1.0d, "Warning lights", "Lamp Combat flaps light", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3269", "928", "Warning lights", "U/C light", "%1d"));
            AddFunction(new PushButton(this, "1", "3270", "950", "Warning lights", "Button Nose wheel steering light", "%1d"));
            AddFunction(new Axis(this, "1", "3271", "951", 0.5d, 0.0d, 1.0d, "Warning lights", "Lamp Nose wheel steering light", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3273", "944", "Warning lights", "Button Airbrake light", "%1d"));
            AddFunction(new Axis(this, "1", "3274", "945", 0.5d, 0.0d, 1.0d, "Warning lights", "Lamp Airbrake light", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3276", "932", "Warning lights", "Limit warning light + Horn", "%1d"));
            AddFunction(new PushButton(this, "1", "3277", "940", "Warning lights", "Fire warning light (ENG/AB) + Horn", "%1d"));
            AddFunction(new PushButton(this, "1", "3278", "1029", "Warning lights", "A/B INJ light", "%1d"));
            AddFunction(new PushButton(this, "1", "3279", "1031", "Warning lights", "A/B ON light", "%1d"));
            AddFunction(new PushButton(this, "1", "3280", "1033", "Warning lights", "A/B SRL light", "%1d"));
            AddFunction(new PushButton(this, "1", "3281", "92", "Warning lights", "Button Standby receptacle light", "%1d"));
            AddFunction(new Axis(this, "1", "3282", "93", 0.5d, 0.0d, 1.0d, "Warning lights", "Lamp Standby receptacle light", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3284", "1082", "Warning lights", "Configuration indicator test button", "%1d"));
            AddFunction(new PushButton(this, "1", "3285", "118", "Warning lights", "Failure warning panel T test button", "%1d"));
            AddFunction(new PushButton(this, "1", "3286", "119", "Warning lights", "Failure warning panel O2 test button", "%1d"));
            AddFunction(new Switch(this, "1", "476", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3287"), new SwitchPosition("0.0", "Posn 2", "3287") }, "Warning lights", "Warning horn switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3294", "197", "Warning lights", "Button (C + M or SW) R light", "%1d"));
            AddFunction(new Axis(this, "1", "3295", "198", 0.5d, 0.0d, 1.0d, "Warning lights", "Lamp (C + M or SW) R light", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3297", "108", "Warning lights", "Button Cannons too hot light", "%1d"));
            AddFunction(new Axis(this, "1", "3298", "109", 0.5d, 0.0d, 1.0d, "Warning lights", "Lamp Cannons too hot light", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3300", "1081", "Warning lights", "BIP button", "%1d"));
            #endregion Warning lights
            #region Exterior lighting
            AddFunction(new Switch(this, "1", "353", SwitchPositions.Create(3, 0d, 0.5d, "3310", "Posn", "%0.1f"), "Exterior lighting", "Landing light control", "%0.1f"));
            AddFunction(new Switch(this, "1", "112", SwitchPositions.Create(3, 0d, 0.5d, "3311", "Posn", "%0.1f"), "Exterior lighting", "Formation light control", "%0.1f"));
            AddFunction(new Switch(this, "1", "111", SwitchPositions.Create(3, 0d, 0.5d, "3312", "Posn", "%0.1f"), "Exterior lighting", "Navigation light control", "%0.1f"));
            AddFunction(new Switch(this, "1", "479", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3313"), new SwitchPosition("0.0", "Posn 2", "3313") }, "Exterior lighting", "Search light control", "%0.1f"));
            #endregion Exterior lighting
            #region Cabin lighting
            AddFunction(new Switch(this, "1", "470", SwitchPositions.Create(2, 0d, 1d, "3314", "Posn", "%0.-1f"), "Cabin lighting", "Miscellaneous instrument lighting switch", "%0.-1f"));
            AddFunction(new Axis(this, "1", "3315", "66", 0.1d, 0.0d, 1.0d, "Cabin lighting", "Map light rheostat", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3317", "68", 0.1d, 0.0d, 1.0d, "Cabin lighting", "Miscellaneous instrument integral lighting rheostat", false, "%0.1f"));
            #endregion Cabin lighting
            #region Lighting control unit
            AddFunction(new Switch(this, "1", "382", SwitchPositions.Create(2, 0d, 1d, "3319", "Posn", "%0.-1f"), "Lighting control unit", "Day/Night selector switch", "%0.-1f"));
            AddFunction(new Axis(this, "1", "3320", "383", 0.1d, 0.0d, 1.0d, "Lighting control unit", "Light and panel lighting rheostat", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3322", "384", 0.1d, 0.0d, 1.0d, "Lighting control unit", "Ultraviolet lighting rheostat", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3324", "385", 0.1d, 0.0d, 1.0d, "Lighting control unit", "Dual instrument panel lighting rheostat (Floodlights)", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3326", "386", 0.1d, 0.0d, 1.0d, "Lighting control unit", "Dual instrument panel lighting rheostat (Integral)", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3328", "387", 0.1d, 0.0d, 1.0d, "Lighting control unit", "Dual console and pedestal lighting rheostat (Floodlights)", false, "%0.1f"));
            AddFunction(new Axis(this, "1", "3330", "388", 0.1d, 0.0d, 1.0d, "Lighting control unit", "Dual console and pedestal lighting rheostat (Integral)", false, "%0.1f"));
            #endregion Lighting control unit
            #region Radionavigation
            #endregion Radionavigation
            #region TACAN control box
            AddFunction(new PushButton(this, "1", "3349", "480", "TACAN control box", "TACAN test button", "%1d"));
            AddFunction(new Switch(this, "1", "485", SwitchPositions.Create(2, 0d, 1d, "3335", "Posn", "%0.-1f"), "TACAN control box", "TACAN X/Y mode selector", "%0.-1f"));
            AddFunction(new Switch(this, "1", "487", SwitchPositions.Create(4, 0d, 0.3333d, "3337", "Posn", "%0.4f"), "TACAN control box", "TACAN mode selector", "%0.4f"));
            #endregion TACAN control box
            #region VOR/ILS control box
            AddFunction(new Switch(this, "1", "503", SwitchPositions.Create(2, 0d, 1d, "3340", "Posn", "%0.-1f"), "VOR/ILS control box", "VOR-ILS control unit ON/OFF selector", "%0.-1f"));
            AddFunction(new Switch(this, "1", "505", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3343", "3343", "0.0"), new SwitchPosition("1.0", "Posn 2", "3342", "3342", "0.0") }, "VOR/ILS control box", "VOR-ILS control unit test selector", "%0.1f"));
            #endregion VOR/ILS control box
            #region Omnibearing and VOR/ILS-TAC selector box
            AddFunction(new Axis(this, "1", "3345", "545", -0.1d, 0.0d, 1d, "Omnibearing and VOR/ILS-TAC selector box", "Omnibearing selector", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "544", SwitchPositions.Create(3, 0d, 0.5d, "3346", "Posn", "%0.1f"), "Omnibearing and VOR/ILS-TAC selector box", "VOR-ILS/OFF/TACAN selector", "%0.1f"));
            AddFunction(new Axis(this, "1", "3348", "547", -0.1d, 0.0d, 1d, "Omnibearing and VOR/ILS-TAC selector box", "Heading selection knob", false, "%0.1f"));
            #endregion Omnibearing and VOR/ILS-TAC selector box
            #region Oxygen system
            AddFunction(new PushButton(this, "1", "3360", "517", "Oxygen system", "Oxygen test button (T button)", "%1d"));
            AddFunction(new PushButton(this, "1", "3361", "519", "Oxygen system", "Pilot oxygen test button", "%1d"));
            AddFunction(new Switch(this, "1", "709", SwitchPositions.Create(3, 0d, 0.5d, "3362", "Posn", "%0.1f"), "Oxygen system", "N-100%-EMG mode selector switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3363", "710", "Oxygen system", "Oxygen overpressure button", "%1d"));
            AddFunction(new PushButton(this, "1", "3364", "712", "Oxygen system", "Anti-g connection cover", "%1d"));
            AddFunction(new PushButton(this, "1", "3365", "713", "Oxygen system", "Anti-g test button", "%1d"));
            AddFunction(new Switch(this, "1", "714", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3366"), new SwitchPosition("0.0", "Posn 2", "3366") }, "Oxygen system", "Anti-g valve cock", "%0.1f"));
            #endregion Oxygen system
            #region Air conditioning system
            AddFunction(new Switch(this, "1", "524", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3375"), new SwitchPosition("0.0", "Posn 2", "3375") }, "Air conditioning system", "Emergency cold switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "525", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3376"), new SwitchPosition("0.0", "Posn 2", "3376") }, "Air conditioning system", "Master valve control switch", "%0.1f"));
            AddFunction(new Axis(this, "1", "3377", "527", 0.1d, 0.0d, 1.0d, "Air conditioning system", "Temperature control rheostat", false, "%0.1f"));
            AddFunction(new Switch(this, "1", "529", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3379"), new SwitchPosition("0.0", "Posn 2", "3379") }, "Air conditioning system", "Auto/Manual selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "530", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3381", "3381", "0.0"), new SwitchPosition("-1.0", "Posn 2", "3380", "3380", "0.0") }, "Air conditioning system", "Hot/Cold selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "410", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3382"), new SwitchPosition("-1.0", "Posn 2", "3382") }, "Air conditioning system", "Ram air switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "411", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3383"), new SwitchPosition("0.0", "Posn 2", "3383") }, "Air conditioning system", "Ram air switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "412", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3384"), new SwitchPosition("0.0", "Posn 2", "3384") }, "Air conditioning system", "Demist switch", "%0.1f"));
            #endregion Air conditioning system
            #region Circuit breaker box
            AddFunction(new Switch(this, "1", "548", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3395"), new SwitchPosition("0.0", "Posn 2", "3395") }, "Circuit breaker box", "Gyro control unit power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "550", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3396"), new SwitchPosition("0.0", "Posn 2", "3396") }, "Circuit breaker box", "High-lift device servo unit power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "552", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3397"), new SwitchPosition("0.0", "Posn 2", "3397") }, "Circuit breaker box", "Indicator and failure detector power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "554", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3398"), new SwitchPosition("0.0", "Posn 2", "3398") }, "Circuit breaker box", "Inverter transfer unit power supply and control", "%0.1f"));
            AddFunction(new Switch(this, "1", "556", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3399"), new SwitchPosition("0.0", "Posn 2", "3399") }, "Circuit breaker box", "Dual hydraulic pressure gauge power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "558", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3400"), new SwitchPosition("0.0", "Posn 2", "3400") }, "Circuit breaker box", "Gun firing trigger power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "560", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3401"), new SwitchPosition("0.0", "Posn 2", "3401") }, "Circuit breaker box", "LP cock power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "562", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3402"), new SwitchPosition("0.0", "Posn 2", "3402") }, "Circuit breaker box", "Electro-pump relay power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "564", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3403"), new SwitchPosition("0.0", "Posn 2", "3403") }, "Circuit breaker box", "Flight refuelling system power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "566", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3404"), new SwitchPosition("0.0", "Posn 2", "3404") }, "Circuit breaker box", "Cabin pressurization system power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "568", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3405"), new SwitchPosition("0.0", "Posn 2", "3405") }, "Circuit breaker box", "Inverter 28 V power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "570", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3406"), new SwitchPosition("0.0", "Posn 2", "3406") }, "Circuit breaker box", "U/C normal operation power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "572", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3407"), new SwitchPosition("0.0", "Posn 2", "3407") }, "Circuit breaker box", "V/UHF power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "574", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3408"), new SwitchPosition("0.0", "Posn 2", "3408") }, "Circuit breaker box", "Starter and sequencing system power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "576", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3409"), new SwitchPosition("0.0", "Posn 2", "3409") }, "Circuit breaker box", "Refuelling probe control power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "578", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3410"), new SwitchPosition("0.0", "Posn 2", "3410") }, "Circuit breaker box", "Manual trim control power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "580", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3411"), new SwitchPosition("0.0", "Posn 2", "3411") }, "Circuit breaker box", "Valve position repeater, control valve and ground mode power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "582", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3412"), new SwitchPosition("0.0", "Posn 2", "3412") }, "Circuit breaker box", "Failure warning panel and master failure warning light power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "584", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3413"), new SwitchPosition("0.0", "Posn 2", "3413") }, "Circuit breaker box", "Configuration indicator (U/C section) and U/C warning light power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "586", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3414"), new SwitchPosition("0.0", "Posn 2", "3414") }, "Circuit breaker box", "IFF power supply", "%0.1f"));
            AddFunction(new Switch(this, "1", "588", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3415"), new SwitchPosition("0.0", "Posn 2", "3415") }, "Circuit breaker box", "Emergency regulation system and control lever power supply", "%0.1f"));
            #endregion Circuit breaker box
            #region Ejection seat
            AddFunction(new Switch(this, "1", "11", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3431", "3431", "0.0"), new SwitchPosition("1.0", "Posn 2", "3430", "3430", "0.0") }, "Ejection seat", "Seat height adjustment control", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3432", "13", "Ejection seat", "Face blind firing handle", "%1d"));
            AddFunction(new PushButton(this, "1", "3434", "14", "Ejection seat", "Alternative firing handle", "%1d"));
            AddFunction(new Axis(this, "1", "3650", "1316", 0.1d, 0.0d, 1.0d, "Ejection seat", "Ejection handle safety pin", false, "%0.1f"));
            #endregion Ejection seat
            #region Chaff and flares dispenser ALE 40 control unit
            AddFunction(new Switch(this, "1", "208", SwitchPositions.Create(3, 0d, 0.5d, "3437", "Posn", "%0.1f"), "Chaff and flares dispenser ALE 40 control unit", "Chaff/flares selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "209", SwitchPositions.Create(4, 0d, 0.3333d, "3438", "Posn", "%0.4f"), "Chaff and flares dispenser ALE 40 control unit", "Program selector switch", "%0.4f"));
            AddFunction(new PushButton(this, "1", "3440", "212", "Chaff and flares dispenser ALE 40 control unit", "Chaff counter reset button", "%1d"));
            AddFunction(new PushButton(this, "1", "3441", "215", "Chaff and flares dispenser ALE 40 control unit", "Flares counter reset button", "%1d"));
            AddFunction(new Switch(this, "1", "216", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3442"), new SwitchPosition("0.0", "Posn 2", "3442") }, "Chaff and flares dispenser ALE 40 control unit", "Emergency jettisoning switch", "%0.1f"));
            #endregion Chaff and flares dispenser ALE 40 control unit
            #region Radar detector switch
            AddFunction(new Switch(this, "1", "478", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3443"), new SwitchPosition("0.0", "Posn 2", "3443") }, "Radar detector switch", "Radar detector switch", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3444", "708", "Radar detector switch", "Chaff/flares release button", "%1d"));
            #endregion Radar detector switch        
        }
        protected string DCSAircraft { get => _dcsAircraft; set => _dcsAircraft = value; }
    }
}
