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
namespace GadrocsWorkshop.Helios.Interfaces.DCS.M2000C
{

    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.M2000C.Functions;

    [HeliosInterface("Helios.M2000C", "DCS M-2000C", typeof(DCSInterfaceEditor), typeof(UniqueHeliosInterfaceFactory), UniquenessKey = "Helios.DCSInterface")]
    public class M2000CInterface : DCSInterface
    {
        #region Devices
        private const string FLIGHTINST = "1";
        private const string NAVINST = "2";
        private const string ENGINE = "3";
        private const string INSTPANEL = "4";
        private const string VTH_VTB = "5";
        private const string VTB = "5";
        private const string PCA_PPA = "6";
        private const string PCA = "6";
        private const string ENGPANEL = "7";
        private const string PWRPNL = "8";
        private const string PCN_NAV = "9";
        private const string PCN = "9";
        private const string RADAR_RDI = "10";
        private const string INS = "10";
        private const string RADAR = "11";
        private const string PCR = "11";
        private const string EW_RWR = "12";
        private const string SERVAL = "12";	        // 12	SERVAL & ECLAIR (Counter Measure dispensers)
        private const string RWR = "13";
        private const string PCCM = "13";		    // 13	Poste de Controle Contre Measures (Countermeasures panel)
        private const string SUBSYSTEMS = "14";
        private const string HYDRAULICS = "14"; 
        private const string MAGIC = "15";
        private const string SYSLIGHTS = "16";
        private const string AFCS = "17";
        private const string CDVE = "17";
        private const string ELECTRIC = "18";
        private const string UVHF = "19";
        private const string VHF = "19";
        private const string UHF = "20";
        private const string INTERCOM = "21";
        private const string MISCPANELS = "22";
        private const string TACAN = "23";
        private const string VORILS = "24";
        private const string ECS = "25";
        private const string FBW = "26";
        private const string DDM = "27";	            // 27	Detecteur de Depart Missile (MLWS)
        private const string DDM_IND = "28";
        private const string SPIRALE = "28";            // 28	SPIRALE (Countermeasure dispenser)
        private const string WEAPONS_CONTROL = "29";
        private const string CAS = "29";                // 29	Calculateur Air Sol (Air Ground Calculator)
        private const string SNA = "30";				// 30	Systeme de navigation et d'attaque
        private const string Helmet_NVG = "31";         // 31
        private const string PPA = "32";                // 32	Panneau de Préparation Armement (Weapon Preparation Panel)
        private const string RDI = "33";                // 33	Radar Doppler Impulsions (main radar)
        private const string DATABUS = "34";            // 34	
        private const string CCLT = "35";               // 35	Calcul Continu Ligne Tracantes (Gun snake)
        private const string VTH = "36";                // 36	Visee Tete Haute (Head Up Display)
        private const string CAA = "37";                // 37	Calculateur Air Air	(Air Air Calculator)
        private const string RS = "38";                 // 38	Radio Sonde	(Radar Altimeter)
        private const string TAF = "39";                // 39	Teleaffichage (Datalink)
        private const string CANOPY = "40";             // 40
        private const string SABRE = "41";              // 41	SABRE (Jammer)

        private const string KNEEBOARD = "100";

        #endregion
        #region Ids
        private const string CMD = "3";
        private const string UVHF_PRESET_DISPLAY = "436";
        private const string UVHF_PRESET_KNOB = "445";
        private const string VORILS_DISPLAY = "611";
        private const string VORILS_FREQUENCY_CHANGE_WHOLE = "616";
        private const string VORILS_FREQUENCY_CHANGE_DECIMAL = "618";
        private const string VORILS_POWER = "617";
        private const string TACAN_DISPLAY = "621";
        private const string TACAN_C10_SELECTOR = "623";
        private const string TACAN_XY_SELECTOR = "624";
        private const string TACAN_C1_SELECTOR = "625";
        private const string TACAN_MODE_SELECTOR = "626";
        #endregion
        #region Commands Codes from command_defs.lua (note:  codes do not necessarily match the enumeration name) 
        private enum initial_Commands
        {
            Button_1 = 3001,
            Button_2 = 3002,
            Button_3 = 3003,
            Button_4 = 3004,
            Button_5 = 3005,
            Button_6 = 3006,
            Button_7 = 3007,
            Button_8 = 3008,
            Button_9 = 3009,
            Button_10 = 3010,
            Button_11 = 3011,
            Button_12 = 3012,
            Button_13 = 3013,
            Button_14 = 3014,
            Button_15 = 3015,
            Button_16 = 3016,
            Button_17 = 3017,
            Button_18 = 3018,
            Button_19 = 3019,
            Button_20 = 3020,
            Button_21 = 3021,
            Button_22 = 3022,
            Button_23 = 3023,
            Button_24 = 3024,
            Button_25 = 3025,
            Button_26 = 3026,
            Button_27 = 3027,
            Button_28 = 3028,
            Button_29 = 3029,
            Button_30 = 3030,
            Button_31 = 3031,
            Button_32 = 3032,
            Button_33 = 3033,
            Button_34 = 3034,
            Button_35 = 3035,
            Button_36 = 3036,
            Button_37 = 3037,
            Button_38 = 3038,
            Button_39 = 3039,
            Button_40 = 3040,
            Button_41 = 3041,
            Button_42 = 3042,
            Button_43 = 3043,
            Button_44 = 3044,
            Button_45 = 3045,
            Button_46 = 3046,
            Button_47 = 3047,
            Button_48 = 3048,
            Button_49 = 3049,
            Button_50 = 3050,
            Button_51 = 3051,
            Button_52 = 3052,
            Button_53 = 3053,
            Button_54 = 3054,
            Button_55 = 3055,
            Button_56 = 3056,
            Button_57 = 3057,
            Button_58 = 3058,
            Button_59 = 3059,
            Button_60 = 3060,
            Button_61 = 3061,
            Button_62 = 3062,
            Button_63 = 3063,
            Button_64 = 3064,
            Button_65 = 3065,
            Button_66 = 3066,
            Button_67 = 3067,
            Button_68 = 3068,
            Button_69 = 3069,
            Button_70 = 3070,
            Button_81 = 3081,
            Button_82 = 3082,
            Button_83 = 3083,
            Button_84 = 3084,
            Button_85 = 3085,
            Button_86 = 3086,
            Button_87 = 3087,
            Button_88 = 3088,
            Button_89 = 3089,
            Button_90 = 3090,
            Button_91 = 3091,
            Button_92 = 3092,
            Button_93 = 3093,
            Button_94 = 3094,
            Button_95 = 3095,
            Button_96 = 3096,
            Button_97 = 3097,
            Button_98 = 3098,
            Button_99 = 3099,
            Button_100 = 3100,
            Button_101 = 3101,
            Button_102 = 3102,
            Button_103 = 3103,
            Button_104 = 3104,
            Button_105 = 3105,
            Button_106 = 3106,
            Button_107 = 3107,
            Button_108 = 3108,
            Button_109 = 3109,
            Button_110 = 3110,
            Button_111 = 3111,
            Button_112 = 3112,
            Button_113 = 3113,
            Button_114 = 3114,
            Button_115 = 3115,
            Button_116 = 3116,
            Button_117 = 3117,
            Button_118 = 3118,
            Button_119 = 3119,
            Button_120 = 3120,
            Button_121 = 3121,
            Button_122 = 3122,
            Button_123 = 3123,
            Button_124 = 3124,
            Button_125 = 3125,
            Button_126 = 3126,
            Button_127 = 3127,
            Button_128 = 3128,
            Button_129 = 3129,
            Button_130 = 3130,
            Button_131 = 3131,
            Button_132 = 3132,
            Button_133 = 3133,
            Button_134 = 3134,
            Button_135 = 3135,
            Button_136 = 3136,
            Button_137 = 3137,
            Button_138 = 3138,
            Button_139 = 3139,
            Button_140 = 3140,
            Button_141 = 3141,
            Button_142 = 3142,
            Button_143 = 3143,
            Button_144 = 3144,
            Button_145 = 3145,
            Button_146 = 3146,
            Button_147 = 3147,
            Button_148 = 3148,
            Button_149 = 3149,
            Button_150 = 3150,
            Button_151 = 3151,
            Button_152 = 3152,
            Button_153 = 3153,
            Button_154 = 3154,
            Button_155 = 3155,
            Button_156 = 3156,
            Button_157 = 3157,
            Button_158 = 3158,
            Button_159 = 3159,
            Button_160 = 3160,
            Button_161 = 3161,
            Button_162 = 3162,
            Button_163 = 3163,
            Button_164 = 3164,
            Button_165 = 3165,
            Button_166 = 3166,
            Button_167 = 3167,
            Button_168 = 3168,
            Button_169 = 3169,
            Button_170 = 3170,
            Button_171 = 3171,
            Button_172 = 3172,
            Button_173 = 3173,
            Button_174 = 3174,
            Button_175 = 3175,
            Button_176 = 3176,
            Button_177 = 3177,
            Button_178 = 3178,
            Button_179 = 3179,
            Button_180 = 3180,
            Button_181 = 3181,
            Button_182 = 3182,
            Button_183 = 3183,
            Button_184 = 3184,
            Button_185 = 3185,
            Button_186 = 3186,
            Button_187 = 3187,
            Button_188 = 3188,
            Button_189 = 3189,
            Button_190 = 3190,
            Button_191 = 3191
        };
        private enum Unassigned_Commands
        {
            Button_198 = 3198,
            Button_199 = 3199,
            Button_200 = 3200
        };
        private enum ECM_Box_Commands
        {
            Button_194 = 3194,
            Button_195 = 3195,
            Button_196 = 3196,
            Button_197 = 3197
        };
        private enum VTH_VTB_Commands
        {
            Button_192 = 3192,
            Button_201 = 3201,
            Button_202 = 3202,
            Button_203 = 3203,
            Button_204 = 3204,
            Button_205 = 3205,
            Button_206 = 3206,
            Button_207 = 3207,
            Button_208 = 3208,
            Button_209 = 3209,
            Button_210 = 3210,
            Button_244 = 3244,
            Button_213 = 3213,
            Button_214 = 3214,
            Button_215 = 3215,
            Button_216 = 3216,
            Button_217 = 3217,
            Button_218 = 3218,
            Button_219 = 3219,
            Button_220 = 3220,
            Button_221 = 3221,
            Button_222 = 3222,
            Button_223 = 3223,
            Button_224 = 3224,
            Button_225 = 3225,
            Button_226 = 3226,
            Button_227 = 3227,
            Button_470 = 3470
        };
        private enum RWR_Commands
        {
            Button_228 = 3228
        };
        private enum PCA_PPA_Commands
        {
            Button_234 = 3234,
            Button_235 = 3235,
            Button_237 = 3237,
            Button_239 = 3239,
            Button_241 = 3241,
            Button_243 = 3243,
            Button_245 = 3245,
            Button_247 = 3247,
            Button_248 = 3248,
            Button_249 = 3249,
            Button_250 = 3250,
            Button_253 = 3253,
            Button_256 = 3256,
            Button_259 = 3259,
            Button_262 = 3262,
            Button_265 = 3265,
            Button_266 = 3266,
            Button_269 = 3269,
            Button_272 = 3272,
            Button_275 = 3275,
            Button_276 = 3276,
            Button_277 = 3277,
            Button_278 = 3278,
            Button_279 = 3279,
            Button_409 = 3409
        };
        private enum CDVE_Commands
        {
            Button_282 = 3282,
            Button_285 = 3285,
            Button_288 = 3288,
            Button_291 = 3291,
            Button_294 = 3294,
            Button_299 = 3299,
            Button_300 = 3300,
            Button_301 = 3301,
            Button_302 = 3302,
            Button_514 = 3514,
            Button_515 = 3515,
            Button_516 = 3516,
            Button_517 = 3517
        };
        private enum Cpt_Clock_Commands
        {
            Button_400 = 3400
        };
        private enum FBW_Commands
        {
            Button_330 = 3330,
            Button_420 = 3420,
            Button_421 = 3421,
            Button_422 = 3422,
            Button_423 = 3423,
            Button_479 = 3479,
            Button_480 = 3480
        };
        private enum ALT_ADI_Commands
        {
            Button_309 = 3309,
            Button_314 = 3314,
            Button_315 = 3315,
            Button_325 = 3325,
            Button_328 = 3328
        };
        private enum HSI_Commands
        {
            Button_340 = 3340,
            Button_341 = 3341
        };
        private enum GMeter_Commands
        {
            Button_348 = 3348
        };
        private enum IFF_Mode_Panel_Commands
        {
            Button_377 = 3377,
            Button_378 = 3378,
            Button_379 = 3379,
            Button_380 = 3380,
            Button_381 = 3381,
            Button_382 = 3382,
            Button_383 = 3383,
            Button_384 = 3384,
            Button_385 = 3385,
            Button_386 = 3386,
            Button_387 = 3387
        };
        private enum COMM_PANEL_Commands
        {
            Button_429 = 3429,
            Button_430 = 3430,
            Button_431 = 3431,
            Button_432 = 3432,
            Button_433 = 3433,
            Button_434 = 3434,
            Button_435 = 3435,
            Button_437 = 3437,
            Button_438 = 3438,
            Button_439 = 3439,
            Button_440 = 3440,
            Button_441 = 3441,
            Button_442 = 3442,
            Button_443 = 3443,
            Button_444 = 3444,
            Button_445 = 3445,
            Button_446 = 3446,
            Button_447 = 3447,
            Button_448 = 3448
        };
        private enum VOR_ILS_TACAN_Commands
        {
            Button_616 = 3616,
            Button_617 = 3617,
            Button_618 = 3618,
            Button_619 = 3619,
            Button_623 = 3623,
            Button_624 = 3624,
            Button_625 = 3625,
            Button_626 = 3626
        };
        private enum EXT_LIGHTS_CONTROLS_Commands
        {
            Button_449 = 3449,
            Button_450 = 3450,
            Button_452 = 3452,
            Button_453 = 3453,
            Button_454 = 3454,
            Button_455 = 3455
        };
        private enum SKID_SWITCH_Commands
        {
            Button_458 = 3458,
            Button_459 = 3459
        };
        private enum FLIGHT_CONTROLS_Commands
        {
            Button_396 = 3396,
            Button_404 = 3404,  // Landing Gear lever
            Button_408 = 3408,  // Landing Gear emergency release lever
            Button_926 = 3926,  // Landing Gear Beep
            Button_460 = 3460,
            Button_461 = 3461,
            Button_462 = 3462
        };
        private enum THROTTLE_Commands
        {
            Button_467 = 3467,
            Button_468 = 3468
        };
        private enum RADAR_Commands
        {
            Button_109 = 3109,
            Button_481 = 3481,
            Button_482 = 3482,
            Button_483 = 3483,
            Button_484 = 3484,
            Button_485 = 3485,
            Button_486 = 3486,
            Button_488 = 3488,
            Button_489 = 3489,
            Button_491 = 3491,
            Button_493 = 3493,
            Button_495 = 3495,
            Button_497 = 3497,
            Button_499 = 3499,
            Button_500 = 3500,
            Button_501 = 3501,
            Button_502 = 3502,
            Button_503 = 3503,
            Button_504 = 3504,
            Button_506 = 3506,
            Button_709 = 3709,
            Button_710 = 3710,
            Button_508 = 3508,
            Button_509 = 3509
        };
        private enum ELECTRICAL_Commands
        {
            Button_520 = 3520,  // Main battery switch
            Button_521 = 3521,  // Power transfer switch
            Button_522 = 3522,  // Alternator 1 switch
            Button_523 = 3523,  // Alternator 2 switch
            Button_524 = 3524,  // Warning lights test switch
            Button_654 = 3654   // QRA switch
        };
        private enum PCN_NAV_Commands
        {
            Button_570 = 3570,  // PREP
            Button_572 = 3572,  // DEST
            Button_574 = 3574,  // Mode Knob
            Button_575 = 3575,  // Brightness Knob
            Button_576 = 3576,  // BAD Button
            Button_578 = 3578,  // REC Button
            Button_580 = 3580,  // VAL Button
            Button_582 = 3582,  // MRQ Button
            Button_667 = 3667,  // ENC Button
            Button_584 = 3584,  // Keyb 1
            Button_585 = 3585,
            Button_586 = 3586,
            Button_587 = 3587,
            Button_588 = 3588,
            Button_589 = 3589,
            Button_590 = 3590,
            Button_591 = 3591,
            Button_592 = 3592,
            Button_593 = 3593,  // Keyb 0
            Button_594 = 3594,  // Clear Button
            Button_596 = 3596,  // Insert Button
            Button_627 = 3627,  // INS Op Mode
            Button_629 = 3629,  // INS Sts Mode
            Button_665 = 3665,  // Aux gyro
            Button_628 = 3628,  // MIP slot door
            Button_673 = 3673,  // MIP label click
            Button_674 = 3674,  // MIP label wheel
            Button_675 = 3675   // MIP top click
        };
        private enum RADAR_IFF_Commands
        {
            Button_598 = 3598,
            Button_599 = 3599,
            Button_600 = 3600,
            Button_601 = 3601,
            Button_602 = 3602,
            Button_603 = 3603,
            Button_604 = 3604
        };
        private enum EW_Commands
        {
            Button_605 = 3605,
            Button_606 = 3606,
            Button_607 = 3607,
            Button_608 = 3608,
            Button_609 = 3609,
            Button_610 = 3610,
            Button_990 = 3990
        };
        private enum ECS_Commands
        {
            Button_630 = 3630,
            Button_631 = 3631,
            Button_633 = 3633,
            Button_635 = 3635,
            Button_636 = 3636,
            Button_637 = 3637,
            Button_638 = 3638
        };
        private enum PANEL_LIGHTS_Commands
        {
            Button_639 = 3639,
            Button_640 = 3640,
            Button_641 = 3641,
            Button_642 = 3642,
            Button_643 = 3643,
            Button_644 = 3644,
            Button_672 = 3672
        };
        private enum ENGINE_START_PANEL
        {
            Button_645 = 3645,
            Button_646 = 3646,
            Button_647 = 3647,
            Button_648 = 3648,
            Button_649 = 3649,
            Button_650 = 3650,
            Button_651 = 3651,
            Button_652 = 3652,
            Button_477 = 3477,
            Button_478 = 3478,
            Button_471 = 3471,
            Button_472 = 3472,
            Button_463 = 3463,
            Button_464 = 3464,
            Button_465 = 3465,
            Button_473 = 3473,
            Button_474 = 3474,
            Button_475 = 3475,
            Button_476 = 3476,
            Button_193 = 3193
        };
        private enum FUEL_PANEL_Commands
        {
            Button_355 = 3355,
            Button_357 = 3357,
            Button_360 = 3360,
            Button_361 = 3361
        };
        private enum MISC_Commands
        {
            Button_395 = 3395,  // Hydraulic System Selector for gauge reading
            Button_655 = 3655,  // Canopy Open/Close
            Button_656 = 3656,  // Canopy Open/Close
            Button_907 = 3907,  // Canopy Open/Close
            Button_908 = 3908,  // Canopy Open/Close
            Button_657 = 3657,  // Hydraulic Emergency Pump
            Button_658 = 3658,  // Audio warning reset
            Button_659 = 3659,
            Button_660 = 3660,
            Button_666 = 3666,  // Parking brake handle
            Button_900 = 3900,
            Button_807 = 3807,
            Button_457 = 3457,
            Button_456 = 3456
        };
        private enum Sound_panel_Commands
        {
            Button_700 = 3700,
            Button_701 = 3701,
            Button_702 = 3702,
            Button_703 = 3703,
            Button_704 = 3704,
            Button_705 = 3705,
            Button_706 = 3706,
            Button_707 = 3707,
            Button_905 = 3905,
            Button_906 = 3906,
            Button_909 = 3909,
            Button_915 = 3915
        };
        private enum LOX_Commands
        {
            Button_910 = 3910,
            Button_911 = 3911,
            Button_912 = 3912
        };
        private enum VHF_Commands
        {
            Button_950 = 3950,
            Button_951 = 3951,
            Button_952 = 3952,
            Button_953 = 3953,
            Button_954 = 3954,
            Button_955 = 3955,
            Button_956 = 3956,
            Button_957 = 3957,
            Button_958 = 3958,
            Button_959 = 3959,
            Button_960 = 3960,
            Button_961 = 3961,
            Button_962 = 3962,
            Button_963 = 3963,
            Button_964 = 3964,
            Button_965 = 3965,
            Button_966 = 3966,
            Button_967 = 3967,
            Button_1004 = 3968,
            Button_1005 = 3969,
            Button_1006 = 3970
        };
        private enum EVF_Commands
        {
            Button_1004 = 3968,
            Button_1005 = 3969,
            Button_1006 = 3970
        };
        #endregion

        public M2000CInterface()
            : base("DCS M2000C", "M-2000C", "pack://application:,,,/M2000C;component/Interfaces/ExportFunctions.lua")
        {

            // see if we can restore from JSON
#if (!DEBUG)
                        if (LoadFunctionsFromJson())
                        {
                            return;
                        }
#endif

            #region Caution Panel
            AddFunction(new FlagValue(this, "525", "Caution Panel", "BATT", "WP BATT"));
            AddFunction(new FlagValue(this, "526", "Caution Panel", "TR", "TR"));
            AddFunction(new FlagValue(this, "527", "Caution Panel", "ALT1", "ALT 1"));
            AddFunction(new FlagValue(this, "528", "Caution Panel", "ALT2", "ALT 2"));
            AddFunction(new FlagValue(this, "529", "Caution Panel", "HUILE", "HUILLE"));
            AddFunction(new FlagValue(this, "530", "Caution Panel", "T7", "T7"));
            AddFunction(new FlagValue(this, "531", "Caution Panel", "CALC", "CALC"));
            AddFunction(new FlagValue(this, "532", "Caution Panel", "SOURIS", "SOURIS"));
            AddFunction(new FlagValue(this, "533", "Caution Panel", "PELLE", "PELLE"));
            AddFunction(new FlagValue(this, "534", "Caution Panel", "BP", "B.P"));
            AddFunction(new FlagValue(this, "535", "Caution Panel", "BPG", "BP.G"));
            AddFunction(new FlagValue(this, "536", "Caution Panel", "BPD", "BP.D"));
            AddFunction(new FlagValue(this, "537", "Caution Panel", "TRANSF", "TRANSF"));
            AddFunction(new FlagValue(this, "538", "Caution Panel", "NIVEAU", "NIVEAU"));
            AddFunction(new FlagValue(this, "539", "Caution Panel", "HYD1", "HYD 1"));
            AddFunction(new FlagValue(this, "540", "Caution Panel", "HYD2", "HYD 2"));
            AddFunction(new FlagValue(this, "541", "Caution Panel", "HYDS", "HYD S"));
            AddFunction(new FlagValue(this, "542", "Caution Panel", "EP", "EP"));
            AddFunction(new FlagValue(this, "543", "Caution Panel", "BINGO", "BINGO"));
            AddFunction(new FlagValue(this, "544", "Caution Panel", "PCAB", "P.CAB"));
            AddFunction(new FlagValue(this, "545", "Caution Panel", "TEMP", "TEMP"));
            AddFunction(new FlagValue(this, "546", "Caution Panel", "REGO2", "REG O2"));
            AddFunction(new FlagValue(this, "547", "Caution Panel", "5mnO2", "5mn O2"));
            AddFunction(new FlagValue(this, "548", "Caution Panel", "O2HA", "O2 HA"));
            AddFunction(new FlagValue(this, "549", "Caution Panel", "ANEMO", "ANEMO"));
            AddFunction(new FlagValue(this, "550", "Caution Panel", "CC", "CC"));
            AddFunction(new FlagValue(this, "551", "Caution Panel", "DSV", "DSV"));
            AddFunction(new FlagValue(this, "552", "Caution Panel", "CONDIT", "CONDIT"));
            AddFunction(new FlagValue(this, "553", "Caution Panel", "CONF", "CONF"));
            AddFunction(new FlagValue(this, "554", "Caution Panel", "PA", "PA"));
            AddFunction(new FlagValue(this, "555", "Caution Panel", "MAN", "MAN"));
            AddFunction(new FlagValue(this, "556", "Caution Panel", "DOM", "DOM"));
            AddFunction(new FlagValue(this, "557", "Caution Panel", "BECS", "BECS"));
            AddFunction(new FlagValue(this, "558", "Caution Panel", "USEL", "USEL"));
            AddFunction(new FlagValue(this, "559", "Caution Panel", "ZEICHEN", "ZEICHEN"));
            AddFunction(new FlagValue(this, "560", "Caution Panel", "GAIN", "GAIN"));
            AddFunction(new FlagValue(this, "561", "Caution Panel", "RPM", "RPM"));
            AddFunction(new FlagValue(this, "562", "Caution Panel", "DECOL", "DECOL"));
            AddFunction(new FlagValue(this, "563", "Caution Panel", "PARK", "PARK"));
            AddFunction(new Switch(this, PWRPNL, "520", new SwitchPosition[] {
                new SwitchPosition("1.0", "On", "3520"),
                new SwitchPosition("0.0", "Off", "3520")},
                "Caution Panel", "Main Battery Switch", "%0.1f"));
            AddFunction(new Switch(this, PWRPNL, "521", new SwitchPosition[] {
                new SwitchPosition("1.0", "On", "3521"),
                new SwitchPosition("0.0", "Off", "3521")},
                "Caution Panel", "Electric Power Transfer Switch", "%0.1f"));
            AddFunction(new Switch(this, PWRPNL, "522", new SwitchPosition[] {
                new SwitchPosition("1.0", "On", "3522"),
                new SwitchPosition("0.0", "Off", "3522")},
                "Caution Panel", "Alternator 1 Switch", "%0.1f"));
            AddFunction(new Switch(this, PWRPNL, "523", new SwitchPosition[] {
                new SwitchPosition("1.0", "On", "3523"),
                new SwitchPosition("0.0", "Off", "3523")},
                "Caution Panel", "Alternator 2 Switch", "%0.1f"));
            AddFunction(new Switch(this, PWRPNL, "524", new SwitchPosition[] {
                new SwitchPosition("1.0", "1", "3524"),
                new SwitchPosition("0.0", "OFF", "3524"),
                new SwitchPosition("-1.0", "2", "3524")},
                "Caution Panel", "Lights Test Switch", "%0.1f"));
            #endregion
            #region  ECM BOX
            AddFunction(new Switch(this, RWR, "194", new SwitchPosition[] {
                new SwitchPosition("1.0", "AUTO", "3194"),
                new SwitchPosition("0.5", "MANU", "3194"),
                new SwitchPosition("0.0", "ARRET", "3194")},
                "ECM Box", "Master Switch", "%0.1f"));
            AddFunction(new Switch(this, RWR, "195", new SwitchPosition[] {
                new SwitchPosition("1.0", "PTF", "3195"),
                new SwitchPosition("0.0", "C/C", "3195")},
                "ECM Box", "Dispensing Mode Switch", "%0.1f"));
            AddFunction(new Switch(this, RWR, "196", new SwitchPosition[] {
                new SwitchPosition("1.0", "On", "3196"),
                new SwitchPosition("0.0", "Off", "3196")},
                "ECM Box", "Lights Power Switch", "%0.1f"));
            AddFunction(new Axis(this, RWR, "3197", "197", 0.075d, 0d, 1d, "ECM Box", "Brightness Selector"));    // elements["PTN_197"] = default_axis_limited(_("ECM Box LCD Display Brightness"), devices.RWR, device_commands.Button_197, 197, 5, 0.5, false, 0)
            #endregion  
            #region Engine Sensors Panel
            AddFunction(new ScaledNetworkValue(this, "371", 1d, "Engine Sensors Panel", "Engine RPM (%) (Tens)", "Engine RPM (Tens).", "0 - 10", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "372", 1d, "Engine Sensors Panel", "Engine RPM (%) (Ones)", "Engine RPM (Ones).", "0 - 9", BindingValueUnits.Numeric));
            CalibrationPointCollectionDouble engineRPMScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1d, 100d);
            AddFunction(new ScaledNetworkValue(this, "369", engineRPMScale, "Engine Sensors Panel", "Engine RPM Needle", "Engine RPM Needle.", "0 - 110", BindingValueUnits.RPMPercent));
            CalibrationPointCollectionDouble engineTempScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1d, 10d);
            AddFunction(new ScaledNetworkValue(this, "370", engineTempScale, "Engine Sensors Panel", "Engine T7 Needle", "Engine Temp Needle.", "0 - 10", BindingValueUnits.Numeric));
            #endregion
            #region  Engine Start Panel
            AddFunction(new Switch(this, ENGPANEL, "645", new SwitchPosition[] {
                new SwitchPosition("1.0", "Open", "3645"),
                new SwitchPosition("0.0", "Closed", "3645")},
                "Engine Start Panel", "Engine Start Switch Guard", "%0.1f"));
            AddFunction(new PushButton(this, ENGPANEL, "3649", "649", "Engine Start Panel", "Engine Start Button"));
            AddFunction(new Switch(this, ENGPANEL, "646", new SwitchPosition[] {
                new SwitchPosition("0.0", "OFF", "3646"),
                new SwitchPosition("1.0", "ON", "3646")},
                "Engine Start Panel", "Starter Fuel Pump Switch", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "647", new SwitchPosition[] {
                new SwitchPosition("0.0", "OFF", "3647"),
                new SwitchPosition("1.0", "ON", "3647")},
                "Engine Start Panel", "Left Fuel Pump Switch", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "648", new SwitchPosition[] {
                new SwitchPosition("0.0", "OFF", "3648"),
                new SwitchPosition("1.0", "ON", "3648")},
                "Engine Start Panel", "Right Fuel Pump Switch", "%0.1f"));
            AddFunction(new Switch(this, ENGPANEL, "650", new SwitchPosition[] {
                new SwitchPosition("-1.0", "VENT", "3650"),
                new SwitchPosition("0.0", "G", "3650"),
                new SwitchPosition("1.0", "D", "3650")},
                "Engine Start Panel", "Ignition Ventilation Selector Switch", "%0.1f"));
            AddFunction(new Switch(this, ENGPANEL, "651", new SwitchPosition[] {
                new SwitchPosition("1.0", "Open", "3651"),
                new SwitchPosition("0.0", "Closed", "3651")},
                "Engine Start Panel", "Fuel Shut-Off Switch Guard", "%0.1f"));
            AddFunction(new Switch(this, ENGPANEL, "652", new SwitchPosition[] {
                new SwitchPosition("1.0", "On", "3652"),
                new SwitchPosition("0.0", "Off", "3652")},
                "Engine Start Panel", "Fuel Shut-Off Switch", "%0.1f"));
            #endregion
            #region Fuel Panel
            AddFunction(new FlagValue(this, "198", "Fuel Panel", "Air Refueling", "Air Refueling"));
            AddFunction(new FlagValue(this, "362", "Fuel Panel", "left-rl", "Fuel Left RL"));
            AddFunction(new FlagValue(this, "363", "Fuel Panel", "center-rl", "Fuel Center RL"));
            AddFunction(new FlagValue(this, "364", "Fuel Panel", "right-rl", "Fuel Right RL"));
            AddFunction(new FlagValue(this, "365", "Fuel Panel", "left-av", "Fuel Left AV"));
            AddFunction(new FlagValue(this, "366", "Fuel Panel", "right-av", "Fuel Right AV"));
            AddFunction(new FlagValue(this, "367", "Fuel Panel", "left-v", "Fuel Left V"));
            AddFunction(new FlagValue(this, "368", "Fuel Panel", "right-v", "Fuel Right V"));
            AddFunction(new ScaledNetworkValue(this, "349", 1d, "Fuel Panel", "Internal Fuel Quantity (Thousands)", "Internal Fuel Quantity (Thousands).", "0-9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "350", 1d, "Fuel Panel", "Internal Fuel Quantity (Hundreds)", "Internal Fuel Quantity (Hundreds).", "0-9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "351", 1d, "Fuel Panel", "Internal Fuel Quantity (Tens)", "Internal Fuel Quantity (Tens).", "0-9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "352", 1d, "Fuel Panel", "Total Fuel Quantity (Thousands)", "Internal Fuel Quantity (Thousands).", "0-9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "353", 1d, "Fuel Panel", "Total Fuel Quantity (Hundreds)", "Internal Fuel Quantity (Hundreds).", "0-9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "354", 1d, "Fuel Panel", "Total Fuel Quantity (Tens)", "Internal Fuel Quantity (Tens).", "0-9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "358", 1.55d, "Fuel Panel", "Internal Fuel Quantity Needle", "Internal Fuel Quantity.", "0-7", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "359", 1.55d, "Fuel Panel", "Total Fuel Quantity Needle", "Total Fuel Quantity.", "0-7", BindingValueUnits.Numeric));
            AddFunction(Switch.CreateThreeWaySwitch(this, INSTPANEL, "3355", "355", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "Fuel Panel", "Fuel Detotalizer Switch", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "357", new SwitchPosition[] {
                new SwitchPosition("0.0", "Open", "3357"),
                new SwitchPosition("1.0", "Closed", "3357") },
                "Fuel Panel", "Fuel CrossFeed Switch", "%0.1f"));
            #endregion
            #region  HSI
            AddFunction(new Axis(this, NAVINST, "3340", "340", 0.015d, -1d, 1d, "HSI Panel", "VAD Selector"));    // elements["PTN_340"] = default_axis_cycle(_("HSI VAD Selector"),devices.NAVINST, device_commands.Button_340, 340)
            AddFunction(new Switch(this, NAVINST, "341", SwitchPositions.Create(7, 0.0, 0.1, "3341", new string[] {"Cv/NAV", "NAV", "TAC", "VAD", "rho", "theta", "TEL"}),"HSI Panel", "Mode Selector", "%0.1f"));
            AddFunction(new ScaledNetworkValue(this, "342", 1d, "HSI Panel", "Compass Rose", "Compass Rose.", "0 - 360", BindingValueUnits.Degrees, 0d, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "336", 1d, "HSI Panel", "Distance (Hundreds)", "Distance (Hundreds).", "0 - 9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "337", 1d, "HSI Panel", "Distance (Tens)", "Engine RPM (Tens).", "0 - 9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "338", 1d, "HSI Panel", "Distance (Ones)", "Distance (Ones).", "0 - 9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "339", 1d, "HSI Panel", "Distance (Decimals)", "Distance (Decimals).", "0 - 9", BindingValueUnits.Numeric));
            AddFunction(new FlagValue(this, "344", "HSI Panel", "Flag 1", "Flag 1"));
            AddFunction(new FlagValue(this, "345", "HSI Panel", "Flag 2", "Flag 2"));
            AddFunction(new FlagValue(this, "346", "HSI Panel", "Flag CAP", "Flag CAP"));
            AddFunction(new FlagValue(this, "343", "HSI Panel", "Flag distance", "Flag distance"));
            AddFunction(new ScaledNetworkValue(this, "333", 1d, "HSI Panel", "Direction Needle", "Direction Needle.", "0 - 360", BindingValueUnits.Degrees, 0d, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "334", 1d, "HSI Panel", "Big Needle", "Big Needle.", "0 - 360", BindingValueUnits.Degrees, 0d, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "335", 1d, "HSI Panel", "Small Needle", "Small Needle.", "0 - 360", BindingValueUnits.Degrees, 0d, "%.4f"));

            #endregion
            #region INS Panel
            AddFunction(new Switch(this, PCN_NAV, "627", SwitchPositions.Create(8, 0.0, 0.1, "3627", new string[] { "AR", "VEI", "CAL", "TST", "ALN", "ALCM", "NAV", "SEC" }),"INS Panel", "Mode Selector", "%0.1f"));
            AddFunction(new Switch(this, PCN_NAV, "629", SwitchPositions.Create(5, 0.0, 0.1, "3629", new string[] { "N", "STS", "DCI", "CRV", "MAIN" }), "INS Panel", "Operation Selector", "%0.1f"));
            #endregion
            #region Landing Gear Panel
            AddFunction(new FlagValue(this, "410", "Landing Gear Panel", "A", "A Warnlamp"));
            AddFunction(new FlagValue(this, "411", "Landing Gear Panel", "F", "F Warnlamp"));
            AddFunction(new FlagValue(this, "412", "Landing Gear Panel", "DIRAV", "DIRA Warnlamp, blau"));
            AddFunction(new FlagValue(this, "413", "Landing Gear Panel", "FREIN", "FREIN"));
            AddFunction(new FlagValue(this, "414", "Landing Gear Panel", "CROSS", "Cross"));
            AddFunction(new FlagValue(this, "415", "Landing Gear Panel", "SPAD", "SPAD"));
            AddFunction(new FlagValue(this, "416", "Landing Gear Panel", "BIP", "Red Warnlamp under BIP"));
            AddFunction(new FlagValue(this, "417", "Landing Gear Panel", "left-gear", "Left Gear"));
            AddFunction(new FlagValue(this, "418", "Landing Gear Panel", "nose-gear", "Nose Gear"));
            AddFunction(new FlagValue(this, "419", "Landing Gear Panel", "right-gear", "Right Gear"));
            AddFunction(new Switch(this, INSTPANEL, "404", new SwitchPosition[] {
                new SwitchPosition("0.0", "UP", "3404"),
                new SwitchPosition("1.0", "DOWN", "3404") },
                "Landing Gear Panel", "Landing Gear Lever", "%0.1f"));
            AddFunction(new FlagValue(this, "405", "Landing Gear Panel", "Landing Gear Handle Indicator", "Red indicator inside the handle"));

            AddFunction(new Switch(this, INSTPANEL, "408", new SwitchPosition[] {
                new SwitchPosition("1.0", "On", "3408"),
                new SwitchPosition("0.0", "Off", "3408") },
                "Landing Gear Panel", "Emergency Landing Gear Lever", "%0.1f"));
            AddFunction(new ScaledNetworkValue(this, "424", 1d, "Landing Gear Panel", "Outer Left Indicator", "Control Surface Indicator.", "-1.0 to 1.0", BindingValueUnits.Numeric, 0d, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "425", 1d, "Landing Gear Panel", "Inner Left Indicator", "Control Surface Indicator.", "-1.0 to 1.0", BindingValueUnits.Numeric, 0d, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "426", 1d, "Landing Gear Panel", "Rudder Indicator", "Control Surface Indicator.", "-1.0 to 1.0", BindingValueUnits.Numeric, 0d, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "427", 1d, "Landing Gear Panel", "Inner Right Indicator", "Control Surface Indicator.", "-1.0 to 1.0", BindingValueUnits.Numeric, 0d, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "428", 1d, "Landing Gear Panel", "Outer Right Indicator", "Control Surface Indicator.", "-1.0 to 1.0", BindingValueUnits.Numeric, 0d, "%.4f"));
            AddFunction(new PushButton(this, PCA_PPA, "3409", "409", "Landing Gear Panel", "Emergency Jettison Lever"));
            AddFunction(new PushButton(this, INSTPANEL, "3926", "926", "Landing Gear Panel", "Landing Gear Tone"));
            #endregion
            #region MCL Panel
            AddFunction(new FlagValue(this, "199", "Master Caution Lights Panel", "Panne Yellow", "Master Warning"));
            AddFunction(new FlagValue(this, "200", "Master Caution Lights Panel", "Panne Red", "Master Caution"));
            #endregion
            #region Miscellaneous Panels and indicators
            AddFunction(new FlagValue(this, "229", "RWR Panel", "RWR V", "RWR V"));
            AddFunction(new FlagValue(this, "230", "RWR Panel", "RWR BR", "RWR BR"));
            AddFunction(new FlagValue(this, "231", "RWR Panel", "RWR DA", "RWR DA"));
            AddFunction(new FlagValue(this, "232", "RWR Panel", "RWR D2M", "RWR D2M"));
            AddFunction(new FlagValue(this, "233", "RWR Panel", "RWR LL", "RWR LL"));
            AddFunction(new FlagValue(this, "373", "Post Combustion Indicator Panel", "pc", "Post Combustion"));
            AddFunction(new FlagValue(this, "374", "Fire Warning Panel", "fire-warning-engine-chamber", "Fire Warning Engine Chamber"));
            AddFunction(new FlagValue(this, "375", "Fire Warning Panel", "fire-warning-afterburner-chamber", "Fire Warning Afterburner Chamber"));
            AddFunction(new FlagValue(this, "376", "Demar Indicator Panel", "demar", "Start-up"));
            AddFunction(new ScaledNetworkValue(this, "331", 3.7d, "AOA Panel", "AOA Needle", "Angle Of Attack Needle.", "0-7", BindingValueUnits.Numeric, 0.08d, "%.4f"));
            #endregion
            #region  PCA/PPA
            AddFunction(new Switch(this, PCA, "463", new SwitchPosition[] {
                new SwitchPosition("1.0", "Safe", "3463"),
                new SwitchPosition("0.0", "Armed", "3463") },
                "Landing Gear Panel", "Gun Arm/Safe Switch", "%0.1f"));
            AddFunction(new Switch(this, PCA, "234", new SwitchPosition[] {
                new SwitchPosition("1.0", "On", "3234"),
                new SwitchPosition("0.0", "Off", "3234")},
                "PCA Panel", "Master Arm Switch", "%0.1f"));
            AddFunction(new Switch(this, PCA, "248", new SwitchPosition[] {
                new SwitchPosition("1.0", "Open", "3248"),
                new SwitchPosition("0.0", "Closed", "3248")},
                "PCA Panel", "Selective Jettison Switch Guard", "%0.1f"));
            AddFunction(new Switch(this, PCA, "249", new SwitchPosition[] {
                new SwitchPosition("0.0", "Off", "3249"),
                new SwitchPosition("1.0", "On", "3249")},
                "PCA Panel", "Selective Jettison Switch", "%0.1f"));
            AddFunction(new PushButton(this, PCA, "3235", "235", "PCA Panel", "Targeting Mode Selection"));
            AddFunction(new PushButton(this, PCA, "3237", "237", "PCA Panel", "Master Mode Selection"));
            AddFunction(new PushButton(this, PCA, "3239", "239", "PCA Panel", "Approach Mode Selection"));
            AddFunction(new PushButton(this, PCA, "3241", "241", "PCA Panel", "Fligt Plan Route Selection"));
            AddFunction(new PushButton(this, PCA, "3243", "243", "PCA Panel", "INS Calibration"));
            AddFunction(new PushButton(this, PCA, "3245", "245", "PCA Panel", "Gun Mode Selector"));
            AddFunction(new PushButton(this, PCA, "3250", "250", "PCA Panel", "Weapon Store Selector 1"));
            AddFunction(new PushButton(this, PCA, "3253", "253", "PCA Panel", "Weapon Store Selector 2"));
            AddFunction(new PushButton(this, PCA, "3256", "256", "PCA Panel", "Weapon Store Selector 3"));
            AddFunction(new PushButton(this, PCA, "3259", "259", "PCA Panel", "Weapon Store Selector 4"));
            AddFunction(new PushButton(this, PCA, "3262", "262", "PCA Panel", "Weapon Store Selector 5"));
            AddFunction(new FlagValue(this, "236", "PCA Panel", "TMS S", "TMS S"));
            AddFunction(new FlagValue(this, "238", "PCA Panel", "MMS", "MMS"));
            AddFunction(new FlagValue(this, "240", "PCA Panel", "AMS", "AMS"));
            AddFunction(new FlagValue(this, "242", "PCA Panel", "FPRS", "FPRS"));
            AddFunction(new FlagValue(this, "244", "PCA Panel", "INS C", "INS C"));
            AddFunction(new FlagValue(this, "251", "PCA Panel", "WSS1 S", "WSS1 S"));
            AddFunction(new FlagValue(this, "252", "PCA Panel", "WSS1 P", "WSS1 P"));
            AddFunction(new FlagValue(this, "254", "PCA Panel", "WSS2 S", "WSS2 S"));
            AddFunction(new FlagValue(this, "255", "PCA Panel", "WSS2 P", "WSS2 P"));
            AddFunction(new FlagValue(this, "257", "PCA Panel", "WSS3 S", "WSS3 S"));
            AddFunction(new FlagValue(this, "258", "PCA Panel", "WSS3 P", "WSS3 P"));
            AddFunction(new FlagValue(this, "260", "PCA Panel", "WSS4 S", "WSS4 S"));
            AddFunction(new FlagValue(this, "261", "PCA Panel", "WSS4 P", "WSS4 P"));
            AddFunction(new FlagValue(this, "263", "PCA Panel", "WSS5 S", "WSS5 S"));
            AddFunction(new FlagValue(this, "264", "PCA Panel", "WSS5 P", "WSS5 P"));
            AddFunction(new FlagValue(this, "246", "PCA Panel", "KL1", "KL1 S"));
            AddFunction(new FlagValue(this, "247", "PCA Panel", "KL2", "KL2 P"));
            AddFunction(new Text(this, "2060", "PCA Panel", "PCA Upper Display", "Display Upper Line"));
            AddFunction(new Text(this, "2061", "PCA Panel", "PCA Lower Display", "Display Lower Line"));
            #endregion
            #region PPA Panel
            AddFunction(new PushButton(this, PPA, "3266", "266", "PPA Panel", "S530 Missile Enabler Button"));
            AddFunction(new PushButton(this, PPA, "3269", "269", "PPA Panel", "Missile Fire Mode Selector"));
            AddFunction(new PushButton(this, PPA, "3272", "272", "PPA Panel", "Magic II Missile Enabler Button")); 
            AddFunction(new PushButton(this, PPA, "3279", "279", "PPA Panel", "Guns/Rockets/Missiles Firing Mode Selector"));
            AddFunction(new Switch(this, PPA, "265", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Droite","3265"),
                new SwitchPosition("0.0", "Auto","3265"),
                new SwitchPosition("1.0", "Gauche","3265")},
                "PPA Panel", "Missile Selector Switch", "%0.1f"));
            AddFunction(new Switch(this, PPA, "275", new SwitchPosition[] {
                new SwitchPosition("-1.0", "Test","3275"),
                new SwitchPosition("0.0", "Neutral","3275"),
                new SwitchPosition("1.0", "Pres","3275")},
                "PPA Panel", "Test Switch", "%0.1f"));
            AddFunction(new Switch(this, PPA, "276", new SwitchPosition[] {
                new SwitchPosition("0.0", "INST.","3276"),
                new SwitchPosition("0.5", "RET.","3276"),
                new SwitchPosition("1.0", "INERT.","3276")},
                "PPA Panel", "Bomb Fuse Selector", "%0.1f")); 
            AddFunction(new Switch(this, PPA, "277", new SwitchPosition[] {
                new SwitchPosition("1.0", "+","3277"),
                new SwitchPosition("0.0", "Neutral","3277"),
                new SwitchPosition("-1.0", "-","3277")},
                "PPA Panel", "Release Quantity Selector", "%0.1f"));
            AddFunction(new Switch(this, PPA, "278", new SwitchPosition[] {
                new SwitchPosition("1.0", "+","3278"),
                new SwitchPosition("0.0", "Neutral","3278"),
                new SwitchPosition("-1.0", "-","3278")},
                "PPA Panel", "Bomb Drop Interval", "%0.1f"));
            AddFunction(new Text(this, "2065", "PPA Panel", "PPA Display Quantity", "Display Line for the PPA Quantity"));
            AddFunction(new Text(this, "2066", "PPA Panel", "PPA Display Interval", "Display Line for the PPA Interval"));
            AddFunction(new FlagValue(this, "267", "PPA Panel", "S530D P", "S530D P"));
            AddFunction(new FlagValue(this, "268", "PPA Panel", "S530D MIS", "S530D MIS"));
            AddFunction(new FlagValue(this, "270", "PPA Panel", "Missile AUT Mode", "Missile AUT Mode"));
            AddFunction(new FlagValue(this, "271", "PPA Panel", "Missile MAN Mode", "Missile MAN Mode"));
            AddFunction(new FlagValue(this, "273", "PPA Panel", "MAGIC P", "MAGIC P"));
            AddFunction(new FlagValue(this, "274", "PPA Panel", "MAGIC MAG", "MAGIC MAG"));
            AddFunction(new FlagValue(this, "280", "PPA Panel", "TOT Firing Mode", "TOT Firing Mode"));
            AddFunction(new FlagValue(this, "281", "PPA Panel", "PAR Firing Mode", "PAR Firing Mode"));
            #endregion
            #region  PCN
            AddFunction(new FlagValue(this, "564", "PCN Panel", "PRET", "PRET"));
            AddFunction(new FlagValue(this, "565", "PCN Panel", "ALN", "ALN"));
            AddFunction(new FlagValue(this, "566", "PCN Panel", "MIP", "MIP"));
            AddFunction(new FlagValue(this, "595", "PCN Panel", "EFF", "EFF"));
            AddFunction(new FlagValue(this, "597", "PCN Panel", "INS", "INS"));
            AddFunction(new FlagValue(this, "567", "PCN Panel", "NDEG", "NDEG"));
            AddFunction(new FlagValue(this, "568", "PCN Panel", "SEC", "SEC"));
            AddFunction(new FlagValue(this, "569", "PCN Panel", "UNI", "UNI"));
            AddFunction(new FlagValue(this, "669", "PCN Panel", "M91", "M91"));
            AddFunction(new FlagValue(this, "670", "PCN Panel", "M92", "M92"));
            AddFunction(new FlagValue(this, "671", "PCN Panel", "M93", "M93"));
            AddFunction(new PushButton(this, PCN, "3570", "570", "PCN Panel", "PREP Button"));
            AddFunction(new FlagValue(this, "571", "PCN Panel", "PREP", "PREP"));
            AddFunction(new PushButton(this, PCN, "3572", "572", "PCN Panel", "DEST Button"));
            AddFunction(new FlagValue(this, "573", "PCN Panel", "DEST", "DEST"));
            //The ENC button has been removed from the aircraft
            //AddFunction(new PushButton(this, PCN, "3667", "667", "PCN Panel", "Offset Waypoint/Target Button"));
            //AddFunction(new FlagValue(this, "668", "PCN Panel", "Offset Waypoint/Target", "ENC"));
            AddFunction(new PushButton(this, PCN, "3578", "578", "PCN Panel", "INS Update Button"));
            AddFunction(new FlagValue(this, "438", "PCN Panel", "INS Update", "REC Indicator"));
            AddFunction(new PushButton(this, PCN, "3580", "580", "PCN Panel", "Validate Data Entry Button"));
            AddFunction(new FlagValue(this, "440", "PCN Panel", "Validate Data Entry", "VAL Indicator"));
            AddFunction(new PushButton(this, PCN, "3582", "582", "PCN Panel", "Marq Position Button"));
            AddFunction(new FlagValue(this, "439", "PCN Panel", "Marq Position", "MRQ Indicator"));
            AddFunction(new PushButton(this, PCN, "3576", "576", "PCN Panel", "AUTO Navigation Button"));
            AddFunction(new FlagValue(this, "437", "PCN Panel", "AUTO Navigation", "BAD Indicator"));
            AddFunction(new Switch(this, PCN, "574", SwitchPositions.Create(11,0.0,0.1 , "3574",new string[]{"TR/VS","D/RLT","CP/PD","ALT","L/G","RT/TD","dL/dG","dALT","P/t","DEC","DV/FV" }, "%0.2f"),
                "PCN Panel", "INS Parameter Selector", "%0.2f"));
            AddFunction(new Axis(this, PCN, "3575", "575", 0.1d, 0d, 1d, "PCN Panel", "Light Brightnes Control/Test"));
            AddFunction(new PushButton(this, PCN, "3584", "584", "PCN Panel", "INS Button 1"));
            AddFunction(new PushButton(this, PCN, "3585", "585", "PCN Panel", "INS Button 2"));
            AddFunction(new PushButton(this, PCN, "3586", "586", "PCN Panel", "INS Button 3"));
            AddFunction(new PushButton(this, PCN, "3587", "587", "PCN Panel", "INS Button 4"));
            AddFunction(new PushButton(this, PCN, "3588", "588", "PCN Panel", "INS Button 5"));
            AddFunction(new PushButton(this, PCN, "3589", "589", "PCN Panel", "INS Button 6"));
            AddFunction(new PushButton(this, PCN, "3590", "590", "PCN Panel", "INS Button 7"));
            AddFunction(new PushButton(this, PCN, "3591", "591", "PCN Panel", "INS Button 8"));
            AddFunction(new PushButton(this, PCN, "3592", "592", "PCN Panel", "INS Button 9"));
            AddFunction(new PushButton(this, PCN, "3593", "593", "PCN Panel", "INS Button 0"));
            AddFunction(new PushButton(this, PCN, "3594", "594", "PCN Panel", "EFF Button"));
            AddFunction(new PushButton(this, PCN, "3596", "596", "PCN Panel", "INS Button"));

            AddFunction(new Text(this, "2068", "PCN Panel", "PCN Latitude Display", "Display Line for PCN Latitude"));
            AddFunction(new Text(this, "2069", "PCN Panel", "PCN Left Points Position", "Position of Points for PCN Left"));
            AddFunction(new Text(this, "2070", "PCN Panel", "PCN Longitude Display", "Display Line for PCN Longitude"));
            AddFunction(new Text(this, "2071", "PCN Panel", "PCN Right Points Position", "Position of Points for PCN Right"));

            AddFunction(new NetworkValue(this, "2072", "PCN Panel", "PCN North", "North Indicator on the PCN display", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2073", "PCN Panel", "PCN South", "South Indicator on the PCN display", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2074", "PCN Panel", "PCN East", "East Indicator on the PCN display", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2075", "PCN Panel", "PCN West", "West Indicator on the PCN display", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2076", "PCN Panel", "PCN Left Plus", "Plus Indicator on the Left PCN display", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2077", "PCN Panel", "PCN Left Minus", "Minus Indicator on the Left PCN display", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2078", "PCN Panel", "PCN Right Plus", "Plus Indicator on the Right PCN display", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new NetworkValue(this, "2079", "PCN Panel", "PCN Right Minus", "Minus Indicator on the Right PCN display", "Boolean True/False", BindingValueUnits.Boolean, null));
            AddFunction(new Text(this, "2080", "PCN Panel", "PCN Lower Left Display", "Display Lower Line Left Side"));
            AddFunction(new Text(this, "2081", "PCN Panel", "PCN Lower Right Display", "Display Lower Line Right Side"));

            #endregion
            #region Test Panel
            AddFunction(new FlagValue(this, "510", "Test Panel", "ELEC", "ELEC indicator"));
            AddFunction(new FlagValue(this, "511", "Test Panel", "HYD", "HYD indicator"));
            AddFunction(new FlagValue(this, "512", "Test Panel", "Test Fail", "Test fail indicator"));
            AddFunction(new FlagValue(this, "513", "Test Panel", "Test Ok", "Test ok indicator"));
            AddFunction(new Switch(this, AFCS, "514", new SwitchPosition[] {new SwitchPosition("0.0", "Open", "3514"), new SwitchPosition("1.0", "Closed", "3514") },
                "Test Panel", "Autopilot Test Guard", "%0.1f"));
            AddFunction(new Switch(this, AFCS, "515", new SwitchPosition[] {new SwitchPosition("0.0", "ON", "3515"), new SwitchPosition("1.0", "OFF", "3515") },
                "Test Panel", "Autopilot Test Switch", "%0.1f"));
            AddFunction(new Switch(this, AFCS, "516", new SwitchPosition[] {new SwitchPosition("0.0", "Open", "3516"), new SwitchPosition("1.0", "Closed", "3516") },
                "Test Panel", "FBW Test Guard", "%0.1f"));
            AddFunction(new Switch(this, AFCS, "517", new SwitchPosition[] {
                new SwitchPosition("1.0", "C", "3517"),
                new SwitchPosition("0.5", "OFF", "3517"),
                new SwitchPosition("0.0", "TEST", "3517"), },
                "Test Panel", "FBW Test Switch", "%0.1f"));
            AddFunction(new Switch(this, AFCS, "479", new SwitchPosition[] {
                new SwitchPosition("0.0", "Open", "3479"),
                new SwitchPosition("1.0", "Closed", "3479")},
                "Test Panel", "FBW Channel 5 Guard", "%0.1f"));
            AddFunction(new Switch(this, AFCS, "480", new SwitchPosition[] {
                new SwitchPosition("0.0", "On", "3480"),
                new SwitchPosition("1.0", "Off", "3480"), },
                "Test Panel", "FBW Channel 5 Switch", "%0.1f"));
            #endregion
            #region Navigational Antennas
            #region VOR.ILS Panel
            AddFunction(new ScaledNetworkValue(this, "615", 1d, "VOR/ILS Panel", "Channel output for display (Decimal Ones)", "Current channel", "0 - 9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "614", 1d, "VOR/ILS Panel", "Channel output for display (Decimal Tens)", "Current channel", "0 - 9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "613", 1d, "VOR/ILS Panel", "Channel output for display (Ones)", "Current channel", "0 - 9", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "612", 1d, "VOR/ILS Panel", "Channel output for display (Tens)", "Current channel", "0 - 1", BindingValueUnits.Numeric));
            AddFunction(new Axis(this, VORILS, "3616", "616", 1.0d / 10d, 0.0d, 1.0d, "VOR/ILS Panel", "Frequency Change whole"));
            AddFunction(Switch.CreateToggleSwitch(this, VORILS, "3617", "617", "0.0", "A", "1.0", "M", "VOR/ILS Panel", "Power Selector", "%0.1f"));
            AddFunction(new Axis(this, VORILS, "3618", "618", 1.0d / 20d, 0.0d, 1.0d, "VOR/ILS Panel", "Frequency Change Decimal"));
            AddFunction(Switch.CreateThreeWaySwitch(this, VORILS, "3619", "619", "0.0", "HG", "0.5", "Test", "1.0", "BD", "VOR/ILS Panel", "Mode Selector", "%0.1f"));
            #endregion
            #region TACAN Panel
            AddFunction(new Axis(this, TACAN, "3625", "625", 1.0d/10d, 0.0d, 1.0d, "TACAN Panel", "Channel 1 Selector"));
            AddFunction(new Axis(this, TACAN, "3623", "623", 1.0d/12d, 0.0d, 1.0d, "TACAN Panel", "Channel 10 Selector"));
            AddFunction(new ScaledNetworkValue(this, "622", 1d, "TACAN Panel", "Channel output for display (Ones)", "Current channel (Ones)", "(0-9)", BindingValueUnits.Numeric));
            AddFunction(new ScaledNetworkValue(this, "621", 1d, "TACAN Panel", "Channel output for display (Tens)", "Current channel (Tens)", "(0-12)", BindingValueUnits.Numeric, 0.3d, "%.2f"));
            AddFunction(new ScaledNetworkValue(this, "620", 1d, "TACAN Panel", "X/Y Drum", "X/Y Mode", "(1 - 2)", BindingValueUnits.Numeric));
            AddFunction(Switch.CreateToggleSwitch(this, TACAN, "3624", "624", "0.0", "X", "1.0", "Y", "TACAN Panel", "X/Y Selector", "%0.1f"));
            AddFunction(new Switch(this, TACAN, "626", SwitchPositions.Create(4, 0.0, 1.0/3, "3626", new string[] { "OFF", "REC", "T/R", "A/A" }, "%0.3f"), "TACAN Panel", "Mode Selector", "%0.3f"));
            #endregion
            #endregion
            #region Indicators
            AddFunction(new FlagValue(this, "185", "Indicators", "Indicators 185", "LIM, MIP,"));
            AddFunction(new FlagValue(this, "186", "Indicators", "Indicators 186", "IFF, MIP, Acc"));
            AddFunction(new FlagValue(this, "187", "Indicators", "Indicators 187", "LED green, ADI"));
            AddFunction(new FlagValue(this, "188", "Indicators", "Indicators 188", "LED green, ADI"));
            AddFunction(new FlagValue(this, "321", "Indicators", "Indicators 321", "ADI ILS light"));
            AddFunction(new FlagValue(this, "283", "Indicators", "Indicators 283", "AUTOPILOT P"));
            AddFunction(new FlagValue(this, "284", "Indicators", "Indicators 284", "AUTOPILOT A"));
            AddFunction(new FlagValue(this, "286", "Indicators", "Indicators 286", "AUTOPILOT Alt 1"));
            AddFunction(new FlagValue(this, "287", "Indicators", "Indicators 287", "AUTOPILOT blank Alt"));
            AddFunction(new FlagValue(this, "289", "Indicators", "Indicators 289", "AUTOPILOT Alt 2"));
            AddFunction(new FlagValue(this, "290", "Indicators", "Indicators 290", "AUTOPILOT AFF"));
            AddFunction(new FlagValue(this, "292", "Indicators", "Indicators 292", "AUTOPILOT blank1"));
            AddFunction(new FlagValue(this, "293", "Indicators", "Indicators 293", "AUTOPILOT blank2"));
            AddFunction(new FlagValue(this, "295", "Indicators", "Indicators 295", "AUTOPILOT left"));
            AddFunction(new FlagValue(this, "296", "Indicators", "Indicators 296", "AUTOPILOT blank L"));
            AddFunction(new FlagValue(this, "297", "Indicators", "Indicators 297", "AUTOPILOT G"));
            AddFunction(new FlagValue(this, "298", "Indicators", "Indicators 298", "AUTOPILOT blank G"));
            AddFunction(new FlagValue(this, "677", "Indicators", "Indicators 677", "COM left green lamp"));
            AddFunction(new FlagValue(this, "519", "Indicators", "Indicators 519", "Oxy flow lamp"));
            AddFunction(new FlagValue(this, "632", "Indicators", "Indicators 632", "TACAN C"));
            AddFunction(new FlagValue(this, "634", "Indicators", "Indicators 634", "TACAN F"));
            AddFunction(new FlagValue(this, "675", "Indicators", "Indicators 675", "COM Panel, lamp red"));
            AddFunction(new FlagValue(this, "676", "Indicators", "Indicators 676", "COM Panel, lamp red, over COM"));

            #endregion
            #region  Inflight Engine Panel
            AddFunction(Switch.CreateToggleSwitch(this, ENGPANEL, "3468", "468", "1.0", "On", "0.0", "Off", "Engine Start Panel", "Engine In-Flight Start Switch", "%0.1f"));
            AddFunction(new PushButton(this, ENGPANEL, "3467", "467", "Engine Start Panel", "Engine Shutdown Button")); 
            AddFunction(Switch.CreateToggleSwitch(this, INSTPANEL, "3477", "477", "0.0", "Open", "1.0", "Closed", "Engine Start Panel", "Fuel Dump Switch Cover", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, INSTPANEL, "3478", "478", "0.0", "On", "1.0", "Off", "Engine Start Panel", "Fuel Dump Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, INSTPANEL, "3471", "471", "1.0", "Open", "0.0", "Closed", "Engine Start Panel", "A/B Emergency Cutoff Switch Cover", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, INSTPANEL, "3472", "472", "1.0", "On", "0.0", "Off", "Engine Start Panel", "A/B Emergency Cutoff Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGPANEL, "3464", "464", "1.0", "Open", "0.0", "Closed", "Engine Start Panel", "Emergency Throttle Cover", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ENGPANEL, "3465", "465", "1.0", "Low", "0.0", "Mid", "-1.0", "High", "Engine Start Panel", "Emergency Throttle Handle", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGPANEL, "3473", "473", "1.0", "Open", "0.0", "Closed", "Engine Start Panel", "Secondary Oil Control Cover", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGPANEL, "3474", "474", "1.0", "On", "0.0", "Off", "Engine Start Panel", "Secondary Oil Control Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGPANEL, "3475", "475", "1.0", "Open", "0.0", "Closed", "Engine Start Panel", "Engine Emergency Control Cover", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, ENGPANEL, "3476", "476", "0.0", "Rearm", "0.5", "Off", "1.0", "On", "Engine Start Panel", "Engine Emergency Control Switch", "%0.1f"));
            #endregion  
            #region  HUD/VTB
            AddFunction(new Switch(this, VTH, "201", new SwitchPosition[] { new SwitchPosition("1.0", "Test", "3201"), new SwitchPosition("0.5", "On", "3201"), new SwitchPosition("0.0", "Off", "3201") }, "HUD/VTB", "HUD Power Switch", "%0.1f"));
            AddFunction(new Axis(this, VTH, "3202", "202", 0.1d, 0d, 2d, "HUD/VTB", "HUD Brightness Knob"));
            AddFunction(Switch.CreateThreeWaySwitch(this, VTH, "3203", "203", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "HUD/VTB", "HUD Declutter Switch", "%0.1f"));
            AddFunction(new Switch(this, RS, "204", new SwitchPosition[] { new SwitchPosition("0.0", "SELH", "3204"), new SwitchPosition("0.5", "H", "3204"), new SwitchPosition("1.0", "ZB", "3204") }, "HUD/VTB", "HUD Altimeter Selector Switch", "%0.1f"));
            AddFunction(new Switch(this, RS, "205", new SwitchPosition[] { new SwitchPosition("1.0", "Test", "3205"), new SwitchPosition("0.5", "M", "3205"), new SwitchPosition("0.0", "A", "3205") }, "HUD/VTB", "RADAR Altimeter Power Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, VTH, "3206", "206","1.0","High","0.5","Low", "HUD/VTB", "Auxiliary Gunsight","%0.1f"));
            AddFunction(new Axis(this,VTH, "3207", "207", 0.15d, 0d, 1d, "HUD/VTB", "Auxiliary Gunsight Deflection"));
            AddFunction(Switch.CreateToggleSwitch(this,VTH, "3208", "208","0.0","CCLT","1.0","PRED", "HUD/VTB", "A/G Gun Reticle Switch","%0.1f"));
            AddFunction(new Axis(this,VTH, "3209", "209", 0.15d, 0d, 1d, "HUD/VTB", "Target Wingspan Knob"));
            AddFunction(new PushButton(this,VTH, "3210", "210", "HUD/VTB", "HUD Clear Button"));
            AddFunction(new RotaryEncoder(this, RS, "3192", "192", 0.13d, "HUD/VTB", "Minimum Altitude Selector"));
            AddFunction(Switch.CreateToggleSwitch(this,VTB, "3470", "470","1.0","On","0.0","Off", "HUD/VTB", "RADAR WOW Emitter Authorize Switch","%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this,VTB, "3213", "213", "1.0", "Deb", "0.0", "Neutral", "-1.0", "Fin", "HUD/VTB", "Target Data Manual Entry Begin/End", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this,VTB, "3214", "214", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "HUD/VTB", "Bullseye Waypoint Selector N", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this,VTB, "3215", "215", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "HUD/VTB", "Target Range from Bullseye Rho", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this,VTB, "3216", "216", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "HUD/VTB", "Target Bearing from Bullseye Theta", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this,VTB, "3217", "217", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "HUD/VTB", "Target Heading C", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this,VTB, "3218", "218", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "HUD/VTB", "Target Altitude Z", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this,VTB, "3219", "219", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "HUD/VTB", "Target Mach Number M", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this,VTB, "3220", "220", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "HUD/VTB", "Target Age T", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this,VTB, "3221", "221", "1.0", "ON", "0.0", "OFF", "HUD/VTB", "VTB Power Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this,VTB, "3222", "222", "1.0", "Declutter", "0.0", "Neutral", "HUD/VTB", "VTB Declutter",    "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this,VTB, "3223", "223", "0.0", "AV", "1.0", "AR", "HUD/VTB", "VTB Map Forward/Centered", "%0.1f"));
            AddFunction(new Switch(this, VTB, "224", SwitchPositions.Create(8, 0.0, 0.1, "3224"), "HUD/VTB", "Markers Brightness", "%0.1f"));
            AddFunction(new Switch(this, VTB, "225", SwitchPositions.Create(8, 0.0, 0.1, "3225"), "HUD/VTB", "Main Brightness", "%0.1f"));
            AddFunction(new Switch(this, VTB, "226", SwitchPositions.Create(8, 0.0, 0.1, "3226"), "HUD/VTB", "Video Brightness", "%0.1f"));
            AddFunction(new Switch(this, VTB, "227", SwitchPositions.Create(8, 0.0, 0.1, "3227"), "HUD/VTB", "Cavalier Brightness", "%0.1f"));
            #endregion
            #region  AFCS
            AddFunction(new PushButton(this, AFCS, "3282", "282", "AFCS", "Autopilot Master Button"));
            AddFunction(new PushButton(this, AFCS, "3285", "285", "AFCS", "Altitude Hold Button"));
            AddFunction(new PushButton(this, AFCS, "3288", "288", "AFCS", "Selected Altitude Hold Button"));
            AddFunction(new PushButton(this, AFCS, "3294", "294", "AFCS", "Approach Hold Button"));
            AddFunction(new PushButton(this, AFCS, "3302", "302", "AFCS", "Autopilot Lights Test Button"));
            AddFunction(new Switch(this, AFCS, "299", SwitchPositions.Create(6,0.0,0.1,"3299","Ten Thousands"), "AFCS", "Altitude 10 000 ft Selector", "%0.1f"));
            AddFunction(new Switch(this, AFCS, "300", SwitchPositions.Create(10, 0.0, 0.1, "3300", "Thousands"), "AFCS", "Altitude 1 000 ft Selector", "%0.1f"));
            AddFunction(new Switch(this, AFCS, "301", SwitchPositions.Create(10, 0.0, 0.1, "3301", "Hundreds"), "AFCS", "Altitude 100 ft Selector", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, AFCS, "3508", "508", "0.0", "On", "1.0", "Off", "AFCS", "Trim Control Mode Dial", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, AFCS, "3509", "509", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "AFCS", "Rudder Trim Paddle", "%0.1f"));
            #endregion
            #region  Fly-By-Wire
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE, "3330", "330", "0.0", "Norm", "1.0", "Vrill", "Fly By Wire", "FBW Spin Mode Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE, "3420", "420", "1.0", "Open", "0.0", "Closed", "Landing Gear Panel", "FBW Gain Mode Switch Cover", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE, "3421", "421", "0.0", "Norm", "1.0", "Gain CDVE", "Landing Gear Panel", "Fly by Wire Gain Mode Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE, "3422", "422", "0.0", "AA", "1.0", "Charges", "Landing Gear Panel", "Fly by Wire G Limiter Switch", "%0.1f"));
            AddFunction(new PushButton(this, CDVE, "3423", "423", "Landing Gear Panel", "FBW Reset Button"));
            #endregion
            #region  PELLES, SOURIES AND BECS
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE, "3460", "460", "0.0", "Auto", "1.0", "Go", "PELLES, SOURIES AND BECS", "Intake Slats Operation Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ENGINE, "3461", "461", "0.0", "Auto", "1.0", "Go", "PELLES, SOURIES AND BECS", "Intake Cones Operation Switch", "%0.1f"));
            AddFunction(new Switch(this, HYDRAULICS, "462", SwitchPositions.Create(3,1.0, -1.0,"3462",new string[] { "Out", "Auto","Go" }), "PELLES, SOURIES AND BECS", "Slats Operation Switch", "%0.1f"));
            AddFunction(new Axis(this, HYDRAULICS, "3396", "396", 0.15d, 0d, 1d, "PELLES, SOURIES AND BECS", "Pedal Adjustment Lever"));    // elements["PTN_396"] = default_axis_limited(_("Pedal Adjustment Lever"),devices.SUBSYSTEMS,device_commands.Button_396,396, 0.5, -0.1, true, 0)
            AddFunction(Switch.CreateToggleSwitch(this, HYDRAULICS, "3395", "395", "1.0", "On", "0.0", "Off", "PELLES, SOURIES AND BECS", "Hydraulic System Selector", "%0.1f"));

            #endregion
            #region  RADAR
            AddFunction(new Switch(this, PCR, "481", SwitchPositions.Create(12,-1.0,2.0 / 12,"3481","%0.2f"), "RADAR", "Change Channel A", "%0.2f"));
            AddFunction(new PushButton(this, PCR, "3489", "489", "RADAR", "Validate Channel Change"));
            AddFunction(new FlagValue(this, "490", "RADAR", "Channel Validate Indicator", "Left consule VAL"));
            AddFunction(new Switch(this, PCR, "485", SwitchPositions.Create(12, -1.0, 2.0 / 12, "3485", "%0.2f"), "RADAR", "Change Channel B", "%0.2f"));
            AddFunction(new PushButton(this, PCR, "3483", "483", "RADAR", "Rearm Button"));
            AddFunction(Switch.CreateThreeWaySwitch(this, PCR, "3484", "484", "1.0", "Ave", "0.0", "Aut", "-1.0", "Sans", "RADAR", "Doppler Reject Switch", "%0.1f"));
            AddFunction(new Axis(this, PCR, "3488", "488", 0.15d, 0d, 1d, "RADAR", "Gain Dial"));
            AddFunction(new Switch(this, PCR, "486", SwitchPositions.Create(4, 0.0, 1d/3d, "3486",new string[] {"A", "P Ch","Sil","Em" },"%0.2f"), "RADAR", "Power Selector", "%0.2f"));
            AddFunction(new PushButton(this, PCR, "3482", "482", "RADAR", "Test Button"));
            AddFunction(new PushButton(this, PCR, "3493", "493", "RADAR", "A/G DEC Mode Button"));
            AddFunction(new FlagValue(this, "494", "RADAR", "DEC Mode Indicator", "Left consule DEC"));
            AddFunction(new PushButton(this, PCR, "3495", "495", "RADAR", "A/G VISU Mode Button"));
            AddFunction(new FlagValue(this, "496", "RADAR", "VISU Mode Indicator", "Left consule VISU"));
            AddFunction(Switch.CreateToggleSwitch(this, PCR, "3499", "499", "1.0", "PPI", "0.0", "B", "RADAR", "Grid Selector Switch", "%0.1f"));
            AddFunction(new PushButton(this, PCR, "3491", "491", "RADAR", "Hardened Clutter Gate Mode"));
            AddFunction(new FlagValue(this, "492", "RADAR", "Hardened Clutter Gate Indicator A", "Left consule A"));
            AddFunction(Switch.CreateToggleSwitch(this, PCR, "3710", "710", "1.0", "S", "0.0", "Z", "RADAR", "TDC Mode Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, PCR, "3500", "500", "0.0", "N", "1.0", "R", "RADAR", "Target Memory Time Selector Switch", "%0.1f"));
            AddFunction(new Switch(this, PCR, "109", SwitchPositions.Create(3, 1.0, -1.0, "3109", new string[] { "HFR", "-", "BFR" }), "RADAR", "PRF Switch", "%0.1f"));
            AddFunction(new Switch(this, PCR, "502", SwitchPositions.Create(3, 0.0, 0.5, "3502",new string[] {"4","2","1" }), "RADAR", "Scan Lines Selector", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, PCR, "3503", "503", "1.0", "+", "0.0", "Neutral", "-1.0", "-", "RADAR", "Range Selector Switch", "%0.1f"));
            AddFunction(new PushButton(this, PCR, "3504", "504", "RADAR", "PSIC/STT Mode Button"));
            AddFunction(new FlagValue(this, "505", "RADAR", "PSIC Indicator", "Left consule PSIC"));
            AddFunction(new Switch(this, PCR, "506", SwitchPositions.Create(3, 1.0, -0.5, "3506",new string[] { "60","30","15"}), "RADAR", "Azimuth Selector", "%0.1f"));
            #endregion
            #region  RADAR IFF
            AddFunction(new Switch(this, RADAR, "598", SwitchPositions.Create(6, 0.0, 0.2, "3598", new string[] { "1", "4","3/2","3/3","3/4", "2" }), "RADAR IFF", "Radar IFF Mode Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, RADAR, "3599", "599","1.0","L","0.0","R", "RADAR IFF", "Radar IFF L/R Selector", "%0.1f"));
            AddFunction(new Switch(this, RADAR, "600", SwitchPositions.Create(3, 0.0, 0.5, "3600", new string[] { "Off", "Sect", "blank" }), "RADAR IFF", "Radar IFF Power Switch", "%0.1f"));
            AddFunction(new Switch(this, RADAR, "601", SwitchPositions.Create(10, 0.0, 1.0d / 10d, "3601"), "RADAR IFF", "Radar IFF Code-4 Selector", "%0.1f"));
            AddFunction(new Switch(this, RADAR, "602", SwitchPositions.Create(10, 0.0, 1.0d / 10d, "3602"), "RADAR IFF", "Radar IFF Code-3 Selector", "%0.1f"));
            AddFunction(new Switch(this, RADAR, "603", SwitchPositions.Create(10, 0.0, 1.0d / 10d, "3603"), "RADAR IFF", "Radar IFF Code-2 Selector", "%0.1f"));
            AddFunction(new Switch(this, RADAR, "604", SwitchPositions.Create(10, 0.0, 1.0d / 10d, "3604"), "RADAR IFF", "Radar IFF Code-1 Selector", "%0.1f"));

            #endregion
            #region  ELECTRICAL PANEL
            AddFunction(Switch.CreateToggleSwitch(this, PWRPNL, "3654", "654", "1.0", "On", "0.0", "Off", "ELECTRICAL PANEL", "Alert Network (QRA) Switch", "%0.1f"));

            #endregion
            #region  EW PANEL
            AddFunction(new Axis(this, SYSLIGHTS, "3228", "228", 0.15d, 0d, 1d, "EW PANEL", "RWR Light Brightnes Control"));
            AddFunction(new Switch(this, PCCM, "605", SwitchPositions.Create(3, -1.0, 1.0, "3605", new string[] { "Vel", "", "PCM" }), "EW PANEL", "EW Mode Selector Switch", "%0.1f"));
            AddFunction(new Switch(this, PCCM, "606", SwitchPositions.Create(3, 1.0, -0.5, "3606", new string[] { "A", "M", "T" }), "EW PANEL", "BR Power Switch", "%0.1f"));
            AddFunction(new Switch(this, PCCM, "607", SwitchPositions.Create(3, 1.0, -0.5, "3607", new string[] { "A", "M", "T" }), "EW PANEL", "RWR Power Switch", "%0.1f"));
            AddFunction(new Switch(this, PCCM, "608", SwitchPositions.Create(3, 1.0, -0.5, "3608", new string[] { "A", "M", "T" }), "EW PANEL", "D2M Power Switch", "%0.1f"));
            AddFunction(new Switch(this, PCCM, "609", SwitchPositions.Create(3, 1.0, -0.5, "3609", new string[] { "A", "S.A.", "AU"}), "EW PANEL", "Decoy Release Mode Switch", "%0.1f"));
            AddFunction(new Switch(this, RWR, "610", SwitchPositions.Create(11, 0.0, 0.1, "3610",new string[] { "A","1","2","3","4","5","6","7","8","9","10"}, "%0.2f"), "EW PANEL", "Decoy Release Program Knob", "%0.2f"));
            AddFunction(new PushButton(this,SPIRALE,"3990","990","EW PANEL", "ECM Box Clear Button"));
            #endregion
            #region  Panel Lights
            AddFunction(Switch.CreateToggleSwitch(this, SYSLIGHTS, "3449", "449", "1.0", "On", "0.0", "Off", "Panel Lights", "Police Lights Switch", "%0.1f"));
            AddFunction(new Switch(this, SYSLIGHTS, "450", SwitchPositions.Create(3, 1.0, -0.5, "3450", new string[] { "Att", "Roul", "A" }), "Panel Lights", "Landing Lights Switch", "%0.1f"));
            AddFunction(new Switch(this, SYSLIGHTS, "452", SwitchPositions.Create(3, 1.0, -0.5, "3452", new string[] {"TB","TH","A" }), "Panel Lights", "SERPAM Recorder Switch", "%0.1f"));
            AddFunction(new Switch(this, SYSLIGHTS, "453", SwitchPositions.Create(3, 1.0, -0.5, "3453", new string[] { "Fort", "Faib", "A" }), "Panel Lights", "Anti-Collision Lights Switch", "%0.1f"));
            AddFunction(new Switch(this, SYSLIGHTS, "454", SwitchPositions.Create(3, 1.0, -0.5, "3454", new string[] { "Fort", "Faib", "A" }), "Panel Lights", "Navigation Lights Switch", "%0.1f"));
            AddFunction(new Switch(this, SYSLIGHTS, "455", SwitchPositions.Create(3, 1.0, -0.5, "3455", new string[] { "Fort", "Faib", "A" }), "Panel Lights", "Formation Lights Switch", "%0.1f"));
            AddFunction(new Axis(this, SYSLIGHTS, "3639", "639", 0.15d, 0d, 1d, "Panel Lights", "Dashboard U.V. Lights Knob"));    // elements["PTN_639"] = default_axis_limited(_("Dashboard U.V. Lights Knob"), devices.SYSLIGHTS, device_commands.Button_639, 639, 10, 0.3, false, 0)
            AddFunction(new Axis(this, SYSLIGHTS, "3640", "640", 0.15d, 0d, 1d, "Panel Lights", "Dashboard Panel Lights Knob"));    // elements["PTN_640"] = default_axis_limited(_("Dashboard Panel Lights Knob"), devices.SYSLIGHTS, device_commands.Button_640, 640, 10, 0.3, false, 0)
            AddFunction(new Axis(this, SYSLIGHTS, "3641", "641", 0.15d, 0d, 1d, "Panel Lights", "Red Flood Lights Knob"));    // elements["PTN_641"] = default_axis_limited(_("Red Flood Lights Knob"), devices.SYSLIGHTS, device_commands.Button_641, 641, 10, 0.3, false, 0)
            AddFunction(new Axis(this, SYSLIGHTS, "3642", "642", 0.15d, 0d, 1d, "Panel Lights", "Console Panel Lights Knob"));    // elements["PTN_642"] = default_axis_limited(_("Console Panel Lights Knob"), devices.SYSLIGHTS, device_commands.Button_642, 642, 10, 0.3, false, 0)
            AddFunction(new Axis(this, SYSLIGHTS, "3643", "643", 0.15d, 0d, 1d, "Panel Lights", "Caution/Advisory Lights Rheostat"));
            AddFunction(new Axis(this, SYSLIGHTS, "3644", "644", 0.15d, 0d, 1d, "Panel Lights", "White Flood Lights Knob"));    // elements["PTN_644"] = default_axis_limited(_("White Flood Lights Knob"), devices.SYSLIGHTS, device_commands.Button_644, 644, 10, 0.3, false, 0)
            AddFunction(new Axis(this, SYSLIGHTS, "Button_920", "920", 0.15d, 0d, 1d, "Panel Lights", "Refuel Lights Brightness Knob"));    // elements["PTN_920"] = default_axis_limited(_("Refuel Lights Brightness Knob"),devices.SYSLIGHTS,device_commands.Button_920, 920, 10, 0.3, false, 0)
            #endregion  
            #region  Fuel Panel 2
            AddFunction(new Switch(this, ENGPANEL, "193", SwitchPositions.Create(3, 1.0, -0.5, "3193"), "Miscellaneous Left Panel", "Refuel Transfer Switch", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "360", SwitchPositions.Create(4, 0.0, 0.1, "3360"), "Fuel Panel", "Bingo Fuel 1 000 kg Selector", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "361", SwitchPositions.Create(10, 0.0, 0.1, "3361"), "Fuel Panel", "Bingo Fuel 100 kg Selector", "%0.1f"));
            AddFunction(new Text(this, "2067", "Fuel Panel", "Fuel Burn Rate Display", "Three digit display showing Kg/Min Fuel"));

            #endregion
            #region  UHF Radio Panel
            AddFunction(new Text(this, "2064", "Radio Panel", "UHF Upper Comm Information", "Upper Display Line for the UHF Radio"));
            AddFunction(new Text(this, "2063", "Radio Panel", "VHF Lower Comm Information", "Lower Display Line for the VHF Radio"));

            AddFunction(Switch.CreateToggleSwitch(this, UHF, "3429", "429", "0.0", "5W", "1.0", "25W", "UHF Radio Panel", "UHF Power 5W/25W Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, UHF, "3430", "430", "1.0", "SIL", "0.0", "Off", "UHF Radio Panel", "UHF SIL Switch", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, UHF, "3431", "431", "1.0", "E+A2", "0.0", "Neutral", "-1.0", "R", "UHF Radio Panel", "UHF E+A2 Switch", "%0.1f"));
            AddFunction(new PushButton(this, UHF, "3432", "432", "UHF Radio Panel", "UHF CDE Switch"));
            AddFunction(new Switch(this, UHF, "433", SwitchPositions.Create(4, 0.0, 0.75d/3d, "3433", new string[] { "AR", "M", "FI", "H" }, "%0.2f"), "UHF Radio Panel", "UHF Mode Switch", "%0.2f"));
            AddFunction(new PushButton(this, UHF, "3434", "434", "UHF Radio Panel", "UHF TEST Switch"));
            AddFunction(new Axis(this, UHF, "3435", "435", 1.0d / 20d, 0.0d, 1.0d, "UHF Radio Panel", "UHF Channel Select", true, "%0.2f"));

            #endregion
            #region VHF Radio Panel
            AddFunction(new Text(this, "2062", "VHF Radio Panel", "VHF Comm Information", "Display Line for the VHF Radio"));
            AddFunction(new Switch(this, VHF, "950", SwitchPositions.Create(7, 0.0, 0.1, "3950", new string[] {"O","FF","HQ","SV","DL","G","EN" }, "%0.2f"), "VHF Radio Panel", "VHF MODE", "%0.2f"));
            AddFunction(new AbsoluteEncoder(this, VHF, "3951", "3951", "951", 1.0d / 20d, 0.0d, 1.0d, "VHF Radio Panel", "VHF Channel Select",true,"%0.2f"));
            AddFunction(new PushButton(this, VHF, "3952", "952", "VHF Radio Panel", "Key CLR/MEM"));
            AddFunction(new PushButton(this, VHF, "3953", "953", "VHF Radio Panel", "Key VLD/XFR"));
            AddFunction(new PushButton(this, VHF, "3954", "954", "VHF Radio Panel", "Key 1/READ"));
            AddFunction(new PushButton(this, VHF, "3955", "955", "VHF Radio Panel", "Key 2/SQL"));
            AddFunction(new PushButton(this, VHF, "3956", "956", "VHF Radio Panel", "Key 3/GR"));
            AddFunction(new PushButton(this, VHF, "3957", "957", "VHF Radio Panel", "Key 4"));
            AddFunction(new PushButton(this, VHF, "3958", "958", "VHF Radio Panel", "Key 5/20/LOW"));
            AddFunction(new PushButton(this, VHF, "3959", "959", "VHF Radio Panel", "Key 6/TONE"));
            AddFunction(new PushButton(this, VHF, "3960", "960", "VHF Radio Panel", "Key 7"));
            AddFunction(new PushButton(this, VHF, "3961", "961", "VHF Radio Panel", "Key 8/TOD"));
            AddFunction(new PushButton(this, VHF, "3962", "962", "VHF Radio Panel", "Key 9/ZERO"));
            AddFunction(new PushButton(this, VHF, "3963", "963", "VHF Radio Panel", "Key 0"));
            AddFunction(new PushButton(this, VHF, "3964", "964", "VHF Radio Panel", "Key CONF"));

            AddFunction(new FlagValue(this, "965", "VHF Radio Panel", "Indicator CLR", "Legend on key"));
            AddFunction(new FlagValue(this, "966", "VHF Radio Panel", "Indicator MEM", "Legend on key"));
            AddFunction(new FlagValue(this, "967", "VHF Radio Panel", "Indicator VLD", "Legend on key"));
            AddFunction(new FlagValue(this, "968", "VHF Radio Panel", "Indicator XFR", "Legend on key"));
            AddFunction(new FlagValue(this, "969", "VHF Radio Panel", "Indicator 1", "Legend on key"));
            AddFunction(new FlagValue(this, "970", "VHF Radio Panel", "Indicator READ", "Legend on key"));
            AddFunction(new FlagValue(this, "971", "VHF Radio Panel", "Indicator 2", "Legend on key"));
            AddFunction(new FlagValue(this, "972", "VHF Radio Panel", "Indicator 2LIGHT", "Legend on key"));
            AddFunction(new FlagValue(this, "973", "VHF Radio Panel", "Indicator SQL", "Legend on key"));
            AddFunction(new FlagValue(this, "974", "VHF Radio Panel", "Indicator 3", "Legend on key"));
            AddFunction(new FlagValue(this, "975", "VHF Radio Panel", "Indicator 3LIGHT", "Legend on key"));
            AddFunction(new FlagValue(this, "976", "VHF Radio Panel", "Indicator GR", "Legend on key"));
            AddFunction(new FlagValue(this, "977", "VHF Radio Panel", "Indicator 4", "Legend on key"));
            AddFunction(new FlagValue(this, "978", "VHF Radio Panel", "Indicator 5", "Legend on key"));
            AddFunction(new FlagValue(this, "979", "VHF Radio Panel", "Indicator 20", "Legend on key"));
            AddFunction(new FlagValue(this, "980", "VHF Radio Panel", "Indicator LOW", "Legend on key"));
            AddFunction(new FlagValue(this, "981", "VHF Radio Panel", "Indicator 6", "Legend on key"));
            AddFunction(new FlagValue(this, "982", "VHF Radio Panel", "Indicator TONE", "Legend on key"));
            AddFunction(new FlagValue(this, "983", "VHF Radio Panel", "Indicator 7", "Legend on key"));
            AddFunction(new FlagValue(this, "984", "VHF Radio Panel", "Indicator 8", "Legend on key"));
            AddFunction(new FlagValue(this, "985", "VHF Radio Panel", "Indicator TOD", "Legend on key"));
            AddFunction(new FlagValue(this, "986", "VHF Radio Panel", "Indicator 9", "Legend on key"));
            AddFunction(new FlagValue(this, "987", "VHF Radio Panel", "Indicator ZERO", "Legend on key"));
            AddFunction(new FlagValue(this, "988", "VHF Radio Panel", "Indicator 0", "Legend on key"));
            AddFunction(new FlagValue(this, "989", "VHF Radio Panel", "Indicator CONF", "Legend on key"));

            #endregion
            #region  Miscellaneous Left Panel
            AddFunction(new PushButton(this, SYSLIGHTS, "3191", "191", "Miscellaneous Left Panel", "Audio Warning Reset"));
            AddFunction(Switch.CreateToggleSwitch(this, MISCPANELS, "3458", "458", "1.0", "Open", "0,0", "Closed", "Miscellaneous Left Panel", "Anti-Skid Switch Cover", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, MISCPANELS, "3459", "459", "1.0", "On", "0,0", "Off", "Miscellaneous Left Panel", "Anti-Skid Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, MISCPANELS, "3666", "666", "0.0", "On", "1,0", "Off", "Miscellaneous Left Panel", "Parking Brake Lever", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, MISCPANELS, "3457", "457", "1.0", "On", "0,0", "Off", "Miscellaneous Left Panel", "Drag Chute Lever", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, PCN, "3905", "905", "1.0", "On", "0,0", "Off", "Miscellaneous Left Panel", "Emergency Compass", "%0.1f"));
            AddFunction(new NetworkValue(this, "800", "Miscellaneous Left Panel", "Throttle Indicator", "Throttle Position", "0 - 1", BindingValueUnits.Numeric));

            #endregion
            #region  HOTAS / Stick
            AddFunction(Switch.CreateThreeWaySwitch(this, MISCPANELS, "3807", "807", "-1.0", "Pushed", "0.0", "Neutral", "1.0", "Pulled", "Stick", "Nose Wheel Steering / IFF Interrogation Button", "%0.1f"));
            #endregion
            #region Canopy
            AddFunction(Switch.CreateToggleSwitch(this, CANOPY, "3456", "456", "1.0", "On", "0,0", "Off", "Canopy", "Canopy Jettison", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CANOPY, "3655", "655", "1.0", "Unfold", "0,0", "Auto", "Canopy", "ajar stick L (un)fold / R auto", "%0.1f"));
            AddFunction(Switch.CreateThreeWaySwitch(this, CANOPY, "3656", "656", "1.0", "Lock", "0.5", "Neutral", "0.0", "Lower", "Canopy", "Canopy Lock/Neutral/Lower Lever", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CANOPY, "3907", "907", "1.0", "Manual", "0,0", "Auto", "Canopy", "Canopy Handle (L drag(manual) / R click(auto)", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CANOPY, "3908", "908", "1.0", "Manual", "0,0", "Auto", "Canopy", "Canopy Handle (Emergency)", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, CANOPY, "3909", "909", "1.0", "On", "0,0", "Off", "Canopy", "Mirror Rendering Toggle", "%0.1f"));
            #endregion  
            #region  Miscellaneous Right Panel
            AddFunction(new Switch(this, ENGINE, "657", SwitchPositions.Create(3,1.0,-0.5,"3657"), "Miscellaneous Right Panel", "Emergency Hydraulic Pump Switch", "%0.1f"));    // ???? 
            AddFunction(Switch.CreateToggleSwitch(this, SYSLIGHTS, "3658", "658", "1.0", "On", "0.0", "Off", "Miscellaneous Right Panel", "Audio Warning Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, MISCPANELS, "3659", "659", "1.0", "Open", "0.0", "Closed", "Miscellaneous Right Panel", "Pitot Heat Cover", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, MISCPANELS, "3660", "660", "0.0", "On", "1.0", "Off", "Miscellaneous Right Panel", "Pitot Heat Switch", "%0.1f"));
            #endregion  
            #region  Miscellaneous Seat
            AddFunction(Switch.CreateThreeWaySwitch(this, MISCPANELS, "3900", "900", "1.0", "Up", "0.0", "Neutral", "-1.0", "Down", "Miscellaneous Seat", "Seat Adjustment Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3910", "910", "1.0", "On", "0.0", "Off", "Miscellaneous Seat", "LOX Dilution Lever", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3911", "911", "1.0", "On", "0.0", "Off", "Miscellaneous Seat", "LOX Test Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3912", "912", "1.0", "On", "0.0", "Off", "Miscellaneous Seat", "LOX Emergency Supply", "%0.1f"));
            #endregion  
            #region  Sound Panel
            AddFunction(Switch.CreateToggleSwitch(this, SYSLIGHTS, "3700", "700", "1.0", "2", "0.0", "1", "Sound Panel", "AMPLIS Selector Knob", "%0.1f"));
            AddFunction(new Axis(this, SYSLIGHTS, "3701", "701", 0.15d, 0d, 1d, "Sound Panel", "VOR/ILS Volume Knob"));    //default_axis_limited(_("VOR/ILS Volume Knob") device_commands.Button_701, 701, 0.8, 0.5, true, false, {0.0, 1.0})
            AddFunction(new Axis(this, SYSLIGHTS, "3702", "702", 0.15d, 0d, 1d, "Sound Panel", "TACAN Volume Knob"));    //default_axis_limited(_("TACAN Volume Knob"), devices.SYSLIGHTS, device_commands.Button_702, 702, 0.8, 0.5, true, false, {0.0, 1.0})
            AddFunction(new Axis(this, SYSLIGHTS, "3703", "703", 0.15d, 0d, 1d, "Sound Panel", "MAGIC Tone Volume Knob"));    //default_axis_limited(_("MAGIC Tone Volume Knob"), devices.SYSLIGHTS, device_commands.Button_703, 703, 0.8, 0.5, true, false, {0.0, 1.0})
            AddFunction(new Axis(this, SYSLIGHTS, "3704", "704", 0.15d, 0d, 1d, "Sound Panel", "TB APP Volume Knob"));    //default_axis_limited(_("TB APP Volume Knob"), devices.SYSLIGHTS, device_commands.Button_704, 704, 0.8, 0.5, true, false, {0.0, 1.0})
            AddFunction(new Axis(this, SYSLIGHTS, "3705", "705", 0.15d, 0d, 1d, "Sound Panel", "Marker Signal Volume Knob"));    //default_axis_limited(_("Marker Signal Volume Knob"), devices.SYSLIGHTS, device_commands.Button_705, 705, 0.8, 0.5, true, false, {0.0, 1.0})
            AddFunction(new Axis(this, SYSLIGHTS, "3706", "706", 0.15d, 0d, 1d, "Sound Panel", "UHF Radio Volume Knob"));    //default_axis_limited(_("UHF Radio Volume Knob"), devices.SYSLIGHTS, device_commands.Button_706, 706, 0.8, 0.5, true, false, {0.0, 1.0})
            AddFunction(new Axis(this, SYSLIGHTS, "3707", "707", 0.15d, 0d, 1d, "Sound Panel", "V/UHF Radio Volume Knob"));    //default_axis_limited(_("V/UHF Radio Volume Knob"), devices.SYSLIGHTS, device_commands.Button_707, 707, 0.8, 0.5, true, false, {0.0, 1.0})
            #endregion
            #region Miscellaneous Gauges
            AddFunction(new ScaledNetworkValue(this, "397", new CalibrationPointCollectionDouble(0.0d, 0.0d, 0.35d, 1d, 2), "Gauges (Misc)", "Hydraulic Pressure Sdes: Servitudes (Ancillaries)", "Pressure Needle Left", "0 - 1", BindingValueUnits.Numeric, "%.3f"));
            AddFunction(new ScaledNetworkValue(this, "398", new CalibrationPointCollectionDouble(0.0d, 0.0d, 0.35d, 1d, 2), "Gauges (Misc)", "Hydraulic Pressure Fs: Frein Secours (Emergency Brake)", "Pressure Needle Right", "0 - 1", BindingValueUnits.Numeric, "%.3f"));
            AddFunction(new ScaledNetworkValue(this, "399", new CalibrationPointCollectionDouble(0.0d, 0.0d, 0.4d, 1d, 3) { new CalibrationPointDouble(0.1d, 0.0835d), new CalibrationPointDouble(0.2d, 0.411d) }, "Gauges (Misc)", "Altitude Cabin Pressure Indicator (x1000 ft)", "Equivalent altitude", "0 - 1", BindingValueUnits.Numeric, "%.3f"));
            AddFunction(new ScaledNetworkValue(this, "518", new CalibrationPointCollectionDouble(0.0d, 0.0d, 0.5d, 1d, 3), "Gauges (Misc)", "LOX: Liquid OXygen Gauge", "Amount in Litres", "0 - 1", BindingValueUnits.Numeric, "%.3f"));

            #endregion
            #region  Flight Instruments
            AddFunction(new PushButton(this, FLIGHTINST, "3308", "308", "Flight Instruments", "G-Meter Reset"));
            AddFunction(new Altimeter(this));
            AddFunction(new RotaryEncoder(this, FLIGHTINST, "3309", "309", 0.01d, "Flight Instruments", "Barometric Pressure Calibration Knob"));

            AddFunction(new ScaledNetworkValue(this, "316", new CalibrationPointCollectionDouble(-1d, -90d, 1d, 90d), "Flight Instruments", "ADI Pitch", "Pitch", "-90 to 90", BindingValueUnits.Numeric,"%.4f"));
            AddFunction(new ScaledNetworkValue(this, "317", new CalibrationPointCollectionDouble(-1d, -180d, 1d, 180d), "Flight Instruments", "ADI Bank", "Bank", "-180 to 180", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(new NetworkValue(this, "318", "Flight Instruments", "ADI Heading", "Heading", "-1 to 1", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(new NetworkValue(this, "320", "Flight Instruments", "ADI Slip Ball", "Slip Ball turn coordinator", "-1 to 1", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTINST, "3314", "314", "1.0", "On", "0.0", "Off", "Flight Instruments", "ADI Cage Lever", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, FLIGHTINST, "3315", "315", "1.0", "On", "0.0", "Off", "Flight Instruments", "ADI Backlight Switch", "%0.1f"));
            AddFunction(new FlagValue(this, "319", "Flight Instruments", "ADI OFF Flag", "Indicator flag on ADI gauge"));
            AddFunction(new NetworkValue(this, "322", "Flight Instruments", "ADI ILS LOC VERT GLIDE", "ILS and localizer vertical needle", "-1 to 1", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(new NetworkValue(this, "323", "Flight Instruments", "ADI ILS GS HORIZ SLOPE", "ILS and Glideslope horizontal needle", "-1 to 1", BindingValueUnits.Numeric, "%.4f"));

            AddFunction(new ScaledNetworkValue(this, "327", new CalibrationPointCollectionDouble(-1d, -90d, 1d, 90d), "Flight Instruments", "Backup ADI Pitch", "Pitch", "-90 to 90", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(new ScaledNetworkValue(this, "326", new CalibrationPointCollectionDouble(-1d, -180d, 1d, 180d), "Flight Instruments", "Backup ADI Bank", "Bank", "-180 to 180", BindingValueUnits.Numeric, "%.4f"));
            AddFunction(new Axis(this, FLIGHTINST, "3328", "328", 0.15d, -1d, 1d, "Flight Instruments", "Backup ADI Pitch Adjust Knob"));
            AddFunction(new PushButton(this, FLIGHTINST, "3325", "325", "Flight Instruments", "Backup ADI Cage / Uncage"));
            AddFunction(new Switch(this, FLIGHTINST, "665", SwitchPositions.Create(3, 1.0, -0.5, "3665"), "Flight Instruments", "Backup ADI Switch", "%0.1f"));
            AddFunction(new FlagValue(this, "329", "Flight Instruments", "ADI Backup OFF Flag", "Indicator flag on Backup ADI gauge"));

            AddFunction(new PushButton(this, NAVINST, "3923", "923", "Flight Instruments", "Stopwatch Start/Stop/Reset button"));
            AddFunction(Switch.CreateToggleSwitch(this, NAVINST, "3924", "924", "0.0", "Normal", "1.0", "Adjust",  "Flight Instruments", "Clock Adjustment Switch", "%0.1f"));
            AddFunction(new RotaryEncoder(this, NAVINST, "3922", "922", 0.005d, "Flight Instruments", "Clock Adjustment Knob"));
            CalibrationPointCollectionDouble clockHoursScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 12d);
            CalibrationPointCollectionDouble clockMinutesScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 60d);
            AddFunction(new ScaledNetworkValue(this, "401", clockHoursScale, "Flight Instruments", "Clock Hours", "Hour value of current time", "0 - 12", BindingValueUnits.Hours));
            AddFunction(new ScaledNetworkValue(this, "402", clockMinutesScale, "Flight Instruments", "Clock Minutes", "Minute value of current time", "0 - 60", BindingValueUnits.Minutes));
            CalibrationPointCollectionDouble stopwatchSecondsScale = new CalibrationPointCollectionDouble(0.0d, 0.0d, 1.0d, 900d);
            AddFunction(new ScaledNetworkValue(this, "403", stopwatchSecondsScale, "Flight Instruments", "Stopwatch Seconds", "Seconds from stopwatch start ", "0 - 900", BindingValueUnits.Seconds,"%.3f"));

            #endregion
            #region  ECS Panel
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3630", "630", "0.0", "Manual", "1.0", "Auto", "ECS Panel", "ECS Main Mode Switch", "%0.1f"));
            AddFunction(new PushButton(this, ECS, "3631", "631", "ECS Panel", "ECS C Button"));
            AddFunction(new PushButton(this, ECS, "3633", "633", "ECS Panel", "ECS F Button"));
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3635", "635", "1.0", "On", "0.0", "Off", "ECS Panel", "ECS Cond Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3636", "636", "1.0", "On", "0.0", "Off", "ECS Panel", "ECS Air Exchange Switch", "%0.1f"));
            AddFunction(new Axis(this, ECS, "3637", "637", 0.15d, 0d, 1d, "ECS Panel", "ECS Temperature Select Knob"));    // elements["PTN_637"] = default_axis_limited_cycle(_("ECS Temperature Select Knob"), devices.ECS, device_commands.Button_637, 637, 0.8, 0.5, true, false, {-1.0, 1.0})
            AddFunction(Switch.CreateToggleSwitch(this, ECS, "3638", "638", "1.0", "On", "0.0", "Off", "ECS Panel", "ECS Defog Switch", "%0.1f"));
                                                                                                       // 
            #endregion
            #region  IFF
            AddFunction(new Switch(this, INSTPANEL, "377", SwitchPositions.Create(10, 0.0, 0.1, "3377", "Tens"), "IFF", "Mode-1 Tens Selector", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "378", SwitchPositions.Create(10, 0.0, 0.1, "3378", "Ones"), "IFF", "Mode-1 Ones Selector", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "379", SwitchPositions.Create(10, 0.0, 0.1, "3379", "Thousands"), "IFF", "Mode-3A Thousands Selector", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "380", SwitchPositions.Create(10, 0.0, 0.1, "3380", "Hundreds"), "IFF", "Mode-3A Hundreds Selector", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "381", SwitchPositions.Create(10, 0.0, 0.1, "3381", "Tens"), "IFF", "Mode-3A Tens Selector", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "382", SwitchPositions.Create(10, 0.0, 0.1, "3382", "Ones"), "IFF", "Mode-3A Ones Selector", "%0.1f"));
            AddFunction(new Switch(this, INSTPANEL, "383", SwitchPositions.Create(3, 1.0, -0.5, "3383", new string[] {"Ident","Out","Mic" }), "IFF", "Ident Power Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, INSTPANEL, "3384", "384", "1.0", "On", "0.0", "Off", "IFF", "Mode-1 Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, INSTPANEL, "3385", "385", "1.0", "On", "0.0", "Off", "IFF", "Mode-2 Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, INSTPANEL, "3386", "386", "1.0", "On", "0.0", "Off", "IFF", "Mode-3A Switch", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, INSTPANEL, "3387", "387", "1.0", "On", "0.0", "Off", "IFF", "Mode-C Switch", "%0.1f"));
            #endregion
            #region EVF (TAF) Panel
            AddFunction(new PushButton(this, TAF, EVF_Commands.Button_1006.ToString("d"), "1006", "EVF (TAF) Panel", "Test"));
            AddFunction(new RotaryEncoder(this, TAF, EVF_Commands.Button_1004.ToString("d"), "1004", 0.05d, "EVF (TAF) Panel", "Channel Selector"));
            AddFunction(new Text(this, "2082", "EVF (TAF) Panel", "EVF Display", "Two digit display on the EVF Panel"));

            #endregion
            #region NVG
            AddFunction(Switch.CreateToggleSwitch(this, Helmet_NVG, "4001", "1001", "1.0", "Mount", "0.0", "Unmount", "Night Vision", "Mount/Unmount NVG on Helmet", "%0.1f"));
            AddFunction(Switch.CreateToggleSwitch(this, Helmet_NVG, "4002", "1002", "1.0", "Stow", "0.0", "Unstow", "Night Vision", "STOW/UNSTOW NVG", "%0.1f"));
            #endregion

        }
    }
}