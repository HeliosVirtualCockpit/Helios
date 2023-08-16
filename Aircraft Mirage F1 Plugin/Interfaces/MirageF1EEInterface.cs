// Copyright 2023 Helios Contributors
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
//#define CREATEINTERFACE
namespace GadrocsWorkshop.Helios.Interfaces.DCS.MIRAGEF1.EE
{
    using ComponentModel;
    using Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.MIRAGEF1.Tools;
    using GadrocsWorkshop.Helios.UDPInterface;
    using System;

    /// <summary>
    /// Interface for DCS Mirage F1EE, including devices which are unique to this variant.
    /// </summary>
    [HeliosInterface("Helios.MIRAGEF1EE", "DCS Mirage F1EE", typeof(DCSInterfaceEditor), typeof(UniqueHeliosInterfaceFactory), UniquenessKey = "Helios.DCSInterface")]
    public class MirageF1CEInterface : MirageF1Interface
    {
        public MirageF1CEInterface() : base(
            "DCS Mirage F1EE",
            "Mirage-F1EE",
            "pack://application:,,,/MirageF1;component/Interfaces/ExportFunctionsMirageF1EE.lua")
        {
#if (CREATEINTERFACE && DEBUG)
            InterfaceCreation ic = new InterfaceCreation();
            foreach (string path in new string[] { $@"{DCSAircraft}\Cockpit\Common\clickabledata_common_F1EE_M.lua", $@"{DCSAircraft}\Cockpit\Mirage-F1\Mirage-F1EE\clickabledata.lua" })
            {
                foreach (NetworkFunction nf in ic.CreateFunctionsFromClickable(this, path))
                {
                    AddFunction(nf);
                }
            }
            return;
#endif
            // see if we can restore from JSON
#if (!DEBUG)
                        if (LoadFunctionsFromJson())
                        {
                            return;
                        }
#endif
#pragma warning disable CS0162 // Unreachable code detected

            // * * * Creating Interface functions from file: Cockpit\Common\clickabledata_common_F1EE_M.lua
            #region Navigation indicator
            AddFunction(new Switch(this, "1", "1254", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3655"), new SwitchPosition("0.0", "Posn 2", "3655") }, "Navigation indicator", "Gyromagnetic/True IDN heading selector", "%0.1f"));
            AddFunction(new Switch(this, "1", "1255", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3656"), new SwitchPosition("0.0", "Posn 2", "3656") }, "Navigation indicator", "VOR/Radar narrow needle selector", "%0.1f"));
            AddFunction(new Switch(this, "1", "1256", SwitchPositions.Create(6, 0d, 0.2d, "3657", "Posn", "%0.1f"), "Navigation indicator", "Mode selector switch", "%0.1f"));
            AddFunction(new Switch(this, "1", "1257", new SwitchPosition[] { new SwitchPosition("-1.0", "Posn 1", "3659", "3659", "0.0"), new SwitchPosition("1.0", "Posn 2", "3660", "3660", "0.0") }, "Navigation indicator", "Additional vector bearing/distance adjustment switch", "%0.1f"));
            #endregion Navigation indicator
            #region In-flight refuelling system
            AddFunction(new Switch(this, "1", "400", new SwitchPosition[] { new SwitchPosition("0.0", "Posn 1", "3664"), new SwitchPosition("1.0", "Posn 2", "3664") }, "In-flight refuelling system", "Transfer/filling switch guard", "%0.1f"));
            AddFunction(new Switch(this, "1", "401", new SwitchPosition[] { new SwitchPosition("1.0", "Posn 1", "3663"), new SwitchPosition("0.0", "Posn 2", "3663") }, "In-flight refuelling system", "Transfer/filling switch", "%0.1f"));
            AddFunction(new Axis(this, "1", "3667", "407", 0.1d, 0.0d, 1.0d, "In-flight refuelling system", "Aerial refuelling light adjustment potentiometer", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3670", "1012", "In-flight refuelling system", "ALR-300 Threats parameters recording", "%1d"));
            AddFunction(new PushButton(this, "1", "3671", "1013", "In-flight refuelling system", "ALR-300 Threats audio off", "%1d"));
            AddFunction(new PushButton(this, "1", "3672", "1014", "In-flight refuelling system", "ALR-300 Search threats off", "%1d"));
            AddFunction(new PushButton(this, "1", "3673", "1015", "In-flight refuelling system", "ALR-300 Display five most dangerous threats", "%1d"));
            AddFunction(new PushButton(this, "1", "3674", "1016", "In-flight refuelling system", "ALR-300 Detailed threats info", "%1d"));
            AddFunction(new Axis(this, "1", "3675", "1017", 0.1d, 0.0d, 1.0d, "In-flight refuelling system", "ALR-300 display brightness", false, "%0.1f"));
            AddFunction(new PushButton(this, "1", "3677", "1018", "In-flight refuelling system", "ALR-300 test", "%1d"));
            AddFunction(new PushButton(this, "1", "3291", "194", "In-flight refuelling system", "Button Jammer detection / Feeder tanks overflow light", "%1d"));
            AddFunction(new Axis(this, "1", "3292", "195", 0.5d, 0.0d, 1.0d, "In-flight refuelling system", "Lamp Jammer detection / Feeder tanks overflow light", false, "%0.1f"));
            #endregion In-flight refuelling system
            // * * * Creating Interface functions from file: C:\Users\bluef\desktop\Cockpit\Mirage-F1\Mirage-F1EE\clickabledata.lua
            #region Inertial Navigation System (INS)
            AddFunction(new Switch(this, "1", "665", SwitchPositions.Create(6, 0d, 0.1d, "3680", "Posn", "%0.1f"), "Inertial Navigation System (INS)", "Parameters selector", "%0.1f"));
            AddFunction(new Switch(this, "1", "666", SwitchPositions.Create(9, 0d, 0.125d, "3681", "Posn", "%0.3f"), "Inertial Navigation System (INS)", "Modes selector", "%0.3f"));
            AddFunction(new Switch(this, "1", "667", SwitchPositions.Create(10, 0d, 0.111d, "3682", "Posn", "%0.3f"), "Inertial Navigation System (INS)", "Waypoints selecting wheel", "%0.3f"));
            AddFunction(new Switch(this, "1", "668", SwitchPositions.Create(3, 0d, 0.5d, "3683", "Posn", "%0.1f"), "Inertial Navigation System (INS)", "Lights test and brightness selector", "%0.1f"));
            AddFunction(new PushButton(this, "1", "3696", "650", "Inertial Navigation System (INS)", "Vertical designation button", "%1d"));
            AddFunction(new PushButton(this, "1", "3697", "651", "Inertial Navigation System (INS)", "Position validation button", "%1d"));
            AddFunction(new PushButton(this, "1", "3698", "652", "Inertial Navigation System (INS)", "Data insertion button", "%1d"));
            AddFunction(new PushButton(this, "1", "3685", "653", "Inertial Navigation System (INS)", "INS - 1 pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3686", "654", "Inertial Navigation System (INS)", "INS N 2 pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3687", "655", "Inertial Navigation System (INS)", "INS + 3 pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3688", "656", "Inertial Navigation System (INS)", "INS W 4 pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3689", "657", "Inertial Navigation System (INS)", "INS 5 pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3690", "658", "Inertial Navigation System (INS)", "INS 6 E pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3691", "659", "Inertial Navigation System (INS)", "INS 7 pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3692", "660", "Inertial Navigation System (INS)", "INS 8 S pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3693", "661", "Inertial Navigation System (INS)", "INS 9 pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3695", "662", "Inertial Navigation System (INS)", "INS * pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3684", "663", "Inertial Navigation System (INS)", "INS 0 pushbutton", "%1d"));
            AddFunction(new PushButton(this, "1", "3694", "664", "Inertial Navigation System (INS)", "INS CLR pushbutton", "%1d"));
            #endregion Inertial Navigation System (INS)
            base.AddFunctions();
#pragma warning restore CS0162 // Unreachable code detected

        }
    }
}
