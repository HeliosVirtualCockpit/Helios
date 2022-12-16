// Copyright 2020 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.A10C
{
    using ComponentModel;
    using Common;

    /// <summary>
    /// Interface for original DCS A-10C, including devices which are unique to the original A-10C.
    /// </summary>
    [HeliosInterface("Helios.A10C", "DCS A-10C", typeof(DCSInterfaceEditor), typeof(UniqueHeliosInterfaceFactory), UniquenessKey = "Helios.DCSInterface")]
    public class A10C1Interface: A10CInterface
    {
        #region devices
        private const string TISL = "57";
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

        public A10C1Interface() : base(
            "DCS A-10C", 
            "A-10C",
            "pack://application:,,,/Helios;component/Interfaces/DCS/A10C/ExportFunctionsA10C1.lua")
        {
            // see if we can restore from JSON
#if (!DEBUG)
                        if (LoadFunctionsFromJson())
                        {
                            return;
                        }
#endif
            #region TISL Panel
            AddFunction(new Switch(this, TISL, "622", new SwitchPosition[] { new SwitchPosition("0.0", "Off", BUTTON_1), new SwitchPosition("0.1", "Cage", BUTTON_1), new SwitchPosition("0.2", "Dive", BUTTON_1), new SwitchPosition("0.3", "Level Narrow Nar", BUTTON_1), new SwitchPosition("0.4", "Level Wide", BUTTON_1) }, "TISL", "Mode Select", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, TISL, BUTTON_2, "623", "1", "Over 10", "0", "10-5", "-1", "Under 5", "TISL", "Slant Range", "%1d"));
            AddFunction(new Axis(this, TISL, BUTTON_3, "624", 0.1d, 0.0d, 1.0d, "TISL", "Altitude above target tens of thousands of feet"));
            AddFunction(new Axis(this, TISL, BUTTON_4, "626", 0.1d, 0.0d, 1.0d, "TISL", "Altitude above target thousands of feet"));
            AddFunction(new Axis(this, TISL, BUTTON_5, "636", 0.05d, 0.0d, 1.0d, "TISL", "TISL Code Wheel 1", true, "%0.2f"));
            AddFunction(new Axis(this, TISL, BUTTON_6, "638", 0.05d, 0.0d, 1.0d, "TISL", "TISL Code Wheel 2", true, "%0.2f"));
            AddFunction(new Axis(this, TISL, BUTTON_7, "640", 0.05d, 0.0d, 1.0d, "TISL", "TISL Code Wheel 3", true, "%0.2f"));
            AddFunction(new Axis(this, TISL, BUTTON_8, "642", 0.05d, 0.0d, 1.0d, "TISL", "TISL Code Wheel 4", true, "%0.2f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, TISL, BUTTON_9, "644", "1", "TISL", "0", "Both", "-1", "Aux", "TISL", "Code Select", "%1d"));
            AddFunction(new PushButton(this, TISL, BUTTON_10, "628", "TISL", "Enter"));
            AddFunction(new PushButton(this, TISL, BUTTON_11, "630", "TISL", "OverTemp"));
            AddFunction(new PushButton(this, TISL, BUTTON_12, "632", "TISL", "Bite"));
            AddFunction(new PushButton(this, TISL, BUTTON_13, "634", "TISL", "Track"));
            AddFunction(new NetworkValue(this, "629", "TISL", "TISL-AUX indicator", "Current value of the indicator", "0 to 1", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "631", "TISL", "OVER TEMP indicator", "Current value of the indicator", "0 to 1", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "633", "TISL", "DET-ACD indicator", "Current value of the indicator", "0 to 1", BindingValueUnits.Numeric));
            AddFunction(new NetworkValue(this, "635", "TISL", "TRACK indicator", "Current value of the indicator", "0 to 1", BindingValueUnits.Numeric));
            #endregion
        }
    }
}
