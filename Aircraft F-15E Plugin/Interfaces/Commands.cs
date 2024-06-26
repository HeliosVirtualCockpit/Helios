﻿//  Copyright 2023 Helios Contributors
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
    internal class Commands
    {
        internal enum Keys
        {
            PlanePickleOn = 350,
            PlanePickleOff = 351,
            PlaneFireOn = 84,
            PlaneFireOff = 85,
            PlaneGear = 68,
            PlaneFonar = 71,
            PlaneSalvoOnOff = 81,
            PlaneChangeWeapon = 101,
            PlaneChangeWaypoint = 102,
            PlaneZoomIn = 103,
            PlaneZoomOut = 104,
            PlaneBrightnessILS = 156,
            PlaneLightsOnOff = 175,
            PlaneDropSnarOnce = 176,
            PlaneHeadLightOnOff = 328,
            PlaneModeNAV = 105,
            PlaneModeBVR = 106,
            PlaneModeVS = 107,
            PlaneModeBore = 108,
            PlaneModeHelmet = 109,
            PlaneModeFI0 = 110,
            PlaneModeGround = 111,
            SwitchMasterArm = 283,
            HOTAS_CoolieUp = 539,
            HOTAS_CoolieDown = 540,
            HOTAS_CoolieLeft = 541,
            HOTAS_CoolieRight = 542,
            HOTAS_CoolieOff = 543,
            HOTAS_TargetManagementSwitchUp = 544,
            HOTAS_TargetManagementSwitchDown = 545,
            HOTAS_TargetManagementSwitchLeft = 546,
            HOTAS_TargetManagementSwitchRight = 547,
            HOTAS_TargetManagementSwitchOff = 548,
            HOTAS_DataManagementSwitchUp = 549,
            HOTAS_DataManagementSwitchDown = 550,
            HOTAS_DataManagementSwitchLeft = 551,
            HOTAS_DataManagementSwitchRight = 552,
            HOTAS_DataManagementSwitchOff = 553,
            PlaneSAUAutomatic = 58,
            PlaneSAUHBarometric = 59,
            PlaneSAUHRadio = 60,
            PlaneSAUHorizon = 61,
            PlaneAutopilot = 62,
            PlaneAUTOnOff = 63,
            PlaneAUTIncrease = 64,
            PlaneAUTDecrease = 65,
            PlaneHook = 69,
            PlanePackWing = 70,
            PlaneFlaps = 72,
            PlaneAirBrake = 73,
            PlaneWheelBrakeOn = 74,
            PlaneWheelBrakeOff = 75,
            PlaneParachute = 76,
            PlaneDropSnar = 77,
            PlaneWingtipSmokeOnOff = 78,
            PlaneFuelOn = 79,
            PlaneFuelOff = 80,
            PlaneJettisonWeapons = 82,
            PlaneEject = 83,
            PlaneFire = 84,
            PlaneRadarOnOff = 86,
            PlaneEOSOnOff = 87,
            PlaneRadarLeft = 88,
            PlaneRadarRight = 89,
            PlaneRadarUp = 90,
            PlaneRadarDown = 91,
            PlaneRadarCenter = 92,
            PlaneTrimLeft = 93,
            PlaneTrimRight = 94,
            PlaneTrimUp = 95,
            PlaneTrimDown = 96,
            PlaneTrimCancel = 97,
            PlaneTrimLeftRudder = 98,
            PlaneTrimRightRudder = 99,
            PlaneChangeLock = 100,
            PlaneChangeTarget = 102,
            PlaneModeGrid = 112,
            PlaneModeCannon = 113,
            PlaneDoAndHome = 114,
            PlaneDoAndBack = 115,
            PlaneFormation = 116,
            PlaneJoinUp = 117,
            PlaneAttackMyTarget = 118,
            PlaneCoverMySix = 119,
            PlaneShipTakeOff = 120,
            ActiveJamming = 136,
            LandDetailsIncrease = 137,
            LandDetailsDecrease = 138,
            SelecterLeft = 139,
            SelecterRight = 140,
            SelecterUp = 141,
            SelecterDown = 142,
            RefusalTWS = 143,
            PlaneResetMasterWarning = 144,
            PlaneFlapsOn = 145,
            PlaneFlapsOff = 146,
            PlaneAirBrakeOn = 147,
            PlaneAirBrakeOff = 148,
            PlaneAirRefuel = 155,
            BrightnessILS = 156,
            PlaneAUTIncreaseLeft = 161,
            PlaneAUTDecreaseLeft = 162,
            PlaneAUTIncreaseRight = 163,
            PlaneAUTDecreaseRight = 164,
            PlaneJettisonFuelTanks = 178,
            PlaneWingmenCommand = 179,
            PlaneDown = 186,
            PlaneUp = 187,
            PlaneLeft = 188,
            PlaneRight = 189,
            PlaneUpStart = 193,
            PlaneUpStop = 194,
            PlaneDownStart = 195,
            PlaneDownStop = 196,
            PlaneLeftStart = 197,
            PlaneLeftStop = 198,
            PlaneRightStart = 199,
            PlaneRightStop = 200,
            PlaneLeftRudderStart = 201,
            PlaneLeftRudderStop = 202,
            PlaneRightRudderStart = 203,
            PlaneRightRudderStop = 204,
            PlaneTrimStop = 215,
            SelecterUpRight = 226,
            SelecterDownRight = 227,
            SelecterDownLeft = 228,
            SelecterUpLeft = 229,
            SelecterStop = 230,
            PlaneRadarUpRight = 231,
            PlaneRadarDownRight = 232,
            PlaneRadarDownLeft = 233,
            PlaneRadarUpLeft = 234,
            PlaneRadarStop = 235,
            PlaneHUDFilterOnOff = 247,
            PlaneMasterCaution = 1,
            PlaneScalesReject = 253,
            PlaneBettyRepeat = 254,
            ThreatMissilePadlock = 258,
            AllMissilePadlock = 259,
            DecreaseRadarScanArea = 262,
            IncreaseRadarScanArea = 263,
            AWACSHomeBearing = 267,
            AWACSTankerBearing = 268,
            AWACSBanditBearing = 269,
            AWACSDeclare = 270,
            EasyRadarOnOff = 271,
            AutoLockOnNearestAircraft = 272,
            AutoLockOnCenterAircraft = 273,
            AutoLockOnNextAircraft = 274,
            AutoLockOnPreviousAircraft = 275,
            AutoLockOnNearestSurfaceTarget = 276,
            AutoLockOnCenterSurfaceTarget = 277,
            AutoLockOnNextSurfaceTarget = 278,
            AutoLockOnPreviousSurfaceTarget = 279,
            ChangeGunRateOfFire = 280,
            ChangeRippleQuantity = 281,
            ChangeRippleInterval = 282,
            ChangeReleaseMode = 284,
            PlaneRadarChangeMode = 285,
            ChangeRWRMode = 286,
            FlightClockReset = 288,
            PlaneCockpitIllumination = 300,
            ChangeRippleIntervalDown = 308,
            EnginesStart = 309,
            EnginesStop = 310,
            LeftEngineStart = 311,
            RightEngineStart = 312,
            LeftEngineStop = 313,
            RightEngineStop = 314,
            PowerOnOff = 315,
            AltimeterPressureIncrease = 316,
            AltimeterPressureDecrease = 317,
            AltimeterPressureStop = 318,
            PlaneLockPadlock = 329,
            PlaneUnlockPadlock = 330,
            PlaneLaunchPermissionOverride = 349,
            PlaneDropFlareOnce = 357,
            PlaneDropChaffOnce = 358,
            //    CMD
            PlaneCMDDispence = 364,
            PlaneCMDDispenceOff = 365,
            PlaneCMDDispenceStop = 366,
            PlaneCMDDispenceStopOff = 367,
            PlaneCMDChangeRippleQuantity = 368,
            PlaneCMDChangeRippleQuantityOff = 369,
            PlaneCMDChangeRippleInterval = 370,
            PlaneCMDChangeRippleIntervalOff = 371,
            PlaneCMDChangeBurstAmount = 372,
            PlaneCMDChangeBurstAmountOff = 373,
            PlaneCMDCancelCurrentProgram = 374,
            PlaneCMDCancelCurrentProgramOff = 375,
            PlaneCMDChangeBoard = 376,
            PlaneCMDShowAmountOrProgram = 377,
            PlaneCancelWeaponsDelivery = 378,
            PlaneMasterCautionOff = 379,
            PlaneStabTangBank = 386,
            PlaneStabHbarBank = 387,
            PlaneStabHorizon = 388,
            PlaneStabHbar = 389,
            PlaneStabHrad = 390,
            ActiveIRJamming = 391,
            PlaneLaserRangerOnOff = 392,
            PlaneNightTVOnOff = 393,
            PlaneChangeRadarPRF = 394,
            PlaneStabCancel = 408,
            PlaneThreatWarnSoundVolumeDown = 409,
            PlaneThreatWarnSoundVolumeUp = 410,
            ViewLaserOnOff = 411,
            PlaneIncreaseBase_Distance = 412,
            PlaneDecreaseBase_Distance = 413,
            PlaneStopBase_Distance = 414,
            PlaneAutopilotOverrideOn = 427,
            PlaneAutopilotOverrideOff = 428,
            PlaneRouteAutopilot = 429,
            PlaneGearUp = 430,
            PlaneGearDown = 431,
            ViewNightVisionGogglesOn = 438,
            PlaneDesignate_CageOn = 439,
            PlaneDesignate_CageOff = 440,
            PlaneDesignate_CageOn_vertical = 441,
            PlaneDesignate_CageOn_horizontal = 442,
            PlaneDLK_Target1 = 443,
            PlaneDLK_Target2 = 444,
            PlaneDLK_Target3 = 445,
            PlaneDLK_RefPoint = 446,
            PlaneDLK_Wingman1 = 447,
            PlaneDLK_Wingman2 = 448,
            PlaneDLK_Wingman3 = 449,
            PlaneDLK_Wingman4 = 450,
            PlaneDLK_All = 451,
            PlaneDLK_Erase = 452,
            PlaneDLK_Ingress = 453,
            PlaneDLK_SendMemory = 454,
            PlaneNavChangePanelModeRight = 455,
            PlaneNavChangePanelModeLeft = 456,
            PlaneNavSetFixtakingMode = 457,
            PlaneNav_DLK_OnOff = 458,
            PlaneNav_PB1 = 459,
            PlaneNav_PB2 = 460,
            PlaneNav_PB3 = 461,
            PlaneNav_PB4 = 462,
            PlaneNav_PB5 = 463,
            PlaneNav_PB6 = 464,
            PlaneNav_PB7 = 465,
            PlaneNav_PB8 = 466,
            PlaneNav_PB9 = 467,
            PlaneNav_PB0 = 468,
            PlaneNav_Steerpoints = 469,
            PlaneNav_INU_realign = 470,
            PlaneNav_POS_corrMode = 471,
            PlaneNav_INU_precise_align = 472,
            PlaneNav_Airfields = 473,
            PlaneNav_INU_normal_align = 474,
            PlaneNav_Targets = 475,
            PlaneNav_Enter = 476,
            PlaneNav_Cancel = 477,
            PlaneNav_POS_init = 478,
            PlaneNav_SelfCoord = 479,
            PlaneNav_CourseTimeRange = 480,
            PlaneNav_Wind = 481,
            PlaneNav_THeadingTimeRangeF = 482,
            PlaneNav_BearingRangeT = 483,
            PlaneCockpitIlluminationPanels = 493,
            PlaneCockpitIlluminationGauges = 494,
            Plane_RouteMode = 506,
            Plane_DescentMode = 507,
            Plane_DescentModeOff = 508,
            Plane_SpotLight_left = 511,
            Plane_SpotLight_right = 512,
            Plane_SpotLight_up = 513,
            Plane_SpotLight_down = 514,
            Plane_SpotLight_stop = 515,
            PlaneRotorTipLights = 516,
            Plane_SpotSelect_switch = 517,
            PlaneAntiCollisionLights = 518,
            PlaneNavLights_CodeModeOn = 519,
            PlaneNavLights_CodeModeOff = 520,
            PlaneFormationLights = 521,
            Plane_EngageAirDefenses = 522,
            Plane_EngageGroundTargets = 523,
            Plane_AutomaticTracking_Gunsight_switch = 524,
            Plane_AutomaticTurn = 526,
            Plane_GroundMovingTarget = 527,
            Plane_AirborneTarget = 528,
            Plane_HeadOnAspect = 529,
            Plane_ChangeDeliveryMode_right = 530,
            Plane_ChangeDeliveryMode_left = 531,
            Plane_WeaponMode_Manual_Auto = 532,
            Plane_WeaponMode_switch = 533,
            Plane_AmmoTypeSelect = 534,
            Plane_FireRate = 535,
            PlaneDropSnarOnceOff = 536,
            HelicopterHover = 537,
            AutopilotEmergOFF = 538,
            Plane_HOTAS_CoolieUp = 539,
            Plane_HOTAS_CoolieDown = 540,
            Plane_HOTAS_CoolieLeft = 541,
            Plane_HOTAS_CoolieRight = 542,
            Plane_HOTAS_CoolieOff = 543,
            Plane_HOTAS_TargetManagementSwitchUp = 544,
            Plane_HOTAS_TargetManagementSwitchDown = 545,
            Plane_HOTAS_TargetManagementSwitchLeft = 546,
            Plane_HOTAS_TargetManagementSwitchRight = 547,
            Plane_HOTAS_TargetManagementSwitchOff = 548,
            Plane_HOTAS_DataManagementSwitchUp = 549,
            Plane_HOTAS_DataManagementSwitchDown = 550,
            Plane_HOTAS_DataManagementSwitchLeft = 551,
            Plane_HOTAS_DataManagementSwitchRight = 552,
            Plane_HOTAS_DataManagementSwitchOff = 553,
            Plane_HOTAS_TriggerSecondStage = 554,
            Plane_HOTAS_TriggerFirstStage = 555,
            Plane_HOTAS_CMS_Up = 556,
            Plane_HOTAS_CMS_Down = 557,
            Plane_HOTAS_CMS_Left = 558,
            Plane_HOTAS_CMS_Right = 559,
            Plane_HOTAS_CMS_Off = 560,
            Plane_HOTAS_MasterModeControlButton = 561,
            Plane_HOTAS_NoseWheelSteeringButton = 562,
            Plane_HOTAS_NoseWheelSteeringButtonOff = 606,
            Plane_HOTAS_BoatSwitchForward = 563,
            Plane_HOTAS_BoatSwitchAft = 564,
            Plane_HOTAS_BoatSwitchCenter = 565,
            Plane_HOTAS_ChinaHatForward = 566,
            Plane_HOTAS_ChinaHatAft = 567,
            Plane_HOTAS_ChinaHatOff = 589,
            Plane_HOTAS_PinkySwitchForward = 568,
            Plane_HOTAS_PinkySwitchAft = 569,
            Plane_HOTAS_PinkySwitchCenter = 570,
            Plane_HOTAS_LeftThrottleButton = 571,
            Plane_HOTAS_MIC_SwitchUp = 572,
            Plane_HOTAS_MIC_SwitchDown = 573,
            Plane_HOTAS_MIC_SwitchLeft = 574,
            Plane_HOTAS_MIC_SwitchRight = 575,
            Plane_HOTAS_MIC_SwitchOff = 576,
            Plane_HOTAS_SpeedBrakeSwitchForward = 577,
            Plane_HOTAS_SpeedBrakeSwitchAft = 578,
            Plane_HOTAS_SpeedBrakeSwitchCenter = 579,
            Plane_HOTAS_MasterModeControlButtonUP = 633,
            Plane_HOTAS_TDC_depress_on = 634,
            Plane_HOTAS_TDC_depress_off = 635,
            Plane_HOTAS_TriggerSecondStage_Off = 638,
            Plane_HOTAS_TriggerFirstStage_Off = 639,
            Plane_HOTAS_CMS_Zaxis = 1041,
            Plane_HOTAS_CMS_Zaxis_Off = 1042,
            Plane_HOTAS_BoatSwitchOff = 822,
            Plane_HOTAS_BoatSwitchForwardMomentary = 823,
            Plane_HOTAS_BoatSwitchAftMomentary = 824,
            Plane_HOTAS_LeftThrottleButton_Off = 1557,

            // analog commands
            PlaneRadarHorizontal = 2025,
            PlaneRadarVertical = 2026,
            PlaneRadarHorizontalAbs = 2027,
            PlaneRadarVerticalAbs = 2028,
            PlaneMFDZoom = 2029,
            PlaneMFDZoomAbs = 2030,
            PlaneSelecterHorizontal = 2031,
            PlaneSelecterVertical = 2032,
            PlaneSelecterHorizontalAbs = 2033,
            PlaneSelecterVerticalAbs = 2034,
            PlaneBase_Distance = 2040,
            PlaneBase_DistanceAbs = 2041,
            LampsControl = 762,
            LampsControl_up = 800,
            //    F-15E Commands
        }
        internal enum fltinst_commands
        {
            fuelqty_totalizer = 3381,
            fuelqty_totalizer_kb = 3382,
            bingo_sel_knob = 3385,
            pitch_ratio_sw = 3335,
            art_hor_uncage = 3350,
            art_hor_adj = 3351,
            alt_adj_knob = 3360,
            clk_adj_knob = 3366,
            tmr_stop_btn = 3367,
            //    
            rc_art_hor_uncage = 3401,
            rc_art_hor_adj = 3402,
            rc_alt_adj_knob = 3403,
            rc_clk_adj_knob = 3404,
            rc_tmr_stop_btn = 3405,
        }
        internal enum ldg_commands
        {
            Gear_lever = 3324,
            warn_tone_sil_btn = 3325,
            em_gear_lever = 3337,
            em_gear_lever_rotate = 3431,
            rc_em_gear_lever = 3342,
        }
        internal enum fltctrl_commands
        {
            Flaps_Control_SW = 3459,
            rudder_trim_sw = 3460,
            rudder_trim_sw_rear = 3403,
        }
        internal enum ufc_commands
        {
            UFC_PB_1 = 3001,
            UFC_PB_2 = 3002,
            UFC_PB_3 = 3003,
            UFC_PB_4 = 3004,
            UFC_PB_5 = 3005,
            UFC_PB_6 = 3006,
            UFC_PB_7 = 3007,
            UFC_PB_8 = 3008,
            UFC_PB_9 = 3009,
            UFC_PB_0 = 3010,
            UFC_PRESET_LEFT = 3011,
            UFC_PRESET_RIGHT = 3012,
            UFC_VOL_R1 = 3013,
            UFC_VOL_R2 = 3014,
            UFC_VOL_R3 = 3015,
            UFC_VOL_R4 = 3016,
            UFC_BRT_CTRL = 3017,
            UFC_EMIS_LMT = 3018,
            UFC_GREC_CM_LEFT = 3019,
            UFC_KEY_A1 = 3020,
            UFC_KEY_N2 = 3021,
            UFC_KEY_B3 = 3022,
            UFC_GREC_CM_RIGHT = 3023,
            UFC_MARK = 3024,
            UFC_KEY_W4 = 3025,
            UFC_KEY_M5 = 3026,
            UFC_KEY_E6 = 3027,
            UFC_I_P = 3028,
            UFC_DOT = 3029,
            UFC_KEY__7 = 3030,
            UFC_KEY_S8 = 3031,
            UFC_KEY_C9 = 3032,
            UFC_SHF = 3033,
            UFC_A_P = 3034,
            UFC_CLEAR = 3035,
            UFC_KEY__0 = 3036,
            UFC_DATA = 3037,
            UFC_MENU = 3038,
            UFC_BRT_CTRL_CCW = 3039,
            UFC_BRT_CTRL_CW = 3040,
            UFC_PRESET_LEFT_CCW = 3041,
            UFC_PRESET_LEFT_CW = 3042,
            UFC_PRESET_RIGHT_CCW = 3043,
            UFC_PRESET_RIGHT_CW = 3044,
            UFC_VOL_R1_CCW = 3045,
            UFC_VOL_R1_CW = 3046,
            UFC_VOL_R2_CCW = 3047,
            UFC_VOL_R2_CW = 3048,
            UFC_VOL_R3_CCW = 3049,
            UFC_VOL_R3_CW = 3050,
            UFC_VOL_R4_CCW = 3051,
            UFC_VOL_R4_CW = 3052,
            UFC_UHF_1_3_SWITCH = 3053,
            UFC_UHF_2_4_SWITCH = 3054,
            UFC_PRESET_SW_LEFT = 3055,
            UFC_PRESET_SW_RIGHT = 3056,
        }
        internal enum mfdg_commands
        {
            Button_01 = 3061,
            Button_02 = 3062,
            Button_03 = 3063,
            Button_04 = 3064,
            Button_05 = 3065,
            Button_06 = 3066,
            Button_07 = 3067,
            Button_08 = 3068,
            Button_09 = 3069,
            Button_10 = 3070,
            Button_11 = 3071,
            Button_12 = 3072,
            Button_13 = 3073,
            Button_14 = 3074,
            Button_15 = 3075,
            Button_16 = 3076,
            Button_17 = 3077,
            Button_18 = 3078,
            Button_19 = 3079,
            Button_20 = 3080,
            Switch_Power = 3081,
            Switch_Cont = 3082,
            Switch_BRT = 3083,
            Button_BIT = 3084,
        }
        internal enum hudctrl_commands
        {
            HUD_BRT_Knob = 3120,
            HUD_REJ_Switch = 3121,
            HUD_MODE_Switch = 3122,
            HUD_BIT_Button = 3123,
            HUD_VIDEO_BRT_Knob = 3124,
            HUD_VIDEO_CONT_Knob = 3125,
            MM_AA_Switch = 3126,
            MM_AG_Switch = 3127,
            MM_NAV_Switch = 3128,
            MM_INST_Switch = 3129,
            //    --
            HUD_BRT_Knob_KB = 3130,
            HUD_REJ_Switch_KB = 3131,
            HUD_MODE_Switch_KB = 3132,
            HUD_BIT_Button_KB = 3133,
            HUD_VIDEO_BRT_Knob_KB = 3134,
            HUD_VIDEO_CONT_Knob_KB = 3135,
            //    
            HUD_BRT_Knob_AXIS = 3136,
            HUD_VIDEO_BRT_Knob_AXIS = 3137,
            HUD_VIDEO_CONT_Knob_AXIS = 3138,
        }
        internal enum hotas_cmds
        {
            //    FRONT COCKPIT
            FC_STICK_CASTLE_FWD = 3101,
            FC_STICK_CASTLE_AFT = 3102,
            FC_STICK_CASTLE_LEFT = 3103,
            FC_STICK_CASTLE_RIGHT = 3104,
            FC_STICK_CASTLE_PRESS = 3105,
            //    
            FC_STICK_AACQ_FWD = 3106,
            FC_STICK_AACQ_AFT = 3107,
            FC_STICK_AACQ_PRESS = 3108,
            //    
            FC_COOLIE_UP = 3109,
            FC_COOLIE_DOWN = 3110,
            FC_COOLIE_LEFT = 3111,
            FC_COOLIE_RIGHT = 3112,
            FC_WSS = 3113,
            FC_WSS_CYCLE = 3114,
            FC_WSS_LOOP = 3115,
            //    
            FC_RDR_ANT_UP = 3116,
            FC_RDR_ANT_DOWN = 3117,
            //    
            FC_THROTTLE_MIC_FWD = 3118,
            FC_THROTTLE_MIC_AFT = 3119,
            FC_THROTTLE_MIC_CTR = 3120,
            FC_TDC_FWD = 3121,
            FC_TDC_AFT = 3122,
            FC_TDC_LEFT = 3123,
            FC_TDC_RIGHT = 3124,
            FC_TDC_PRESS = 3125,
            FC_TDC_AXIS_HORZ = 3126,
            FC_TDC_AXIS_VERT = 3127,
            FC_ANT_AXIS = 3128,
            FC_BOAT_FWD = 3129,
            FC_BOAT_AFT = 3130,
            FC_LEFT_MF = 3131,
            FC_CMD_UP = 3132,
            FC_CMD_DOWN = 3133,
            FC_NWS = 3134,
            //    REAR COCKPIT
            RC_HAND_CONTROLLER_SELECT = 3199,
            //    REAR COCKPIT - LEFT HAND CONTROLLER
            RC_LHC_CMD_FWD = 3201,
            RC_LHC_CMD_AFT = 3202,
            RC_LHC_CMD_CTR = 3203,
            RC_LHC_COOLIE_UP = 3204,
            RC_LHC_COOLIE_DOWN = 3205,
            RC_LHC_COOLIE_LEFT = 3206,
            RC_LHC_COOLIE_RIGHT = 3207,
            RC_LHC_COOLIE_OFF = 3208,
            RC_LHC_CASTLE_FWD = 3209,
            RC_LHC_CASTLE_AFT = 3210,
            RC_LHC_CASTLE_LEFT = 3211,
            RC_LHC_CASTLE_RIGHT = 3212,
            RC_LHC_CASTLE_PRESS = 3213,
            RC_LHC_TDC_FWD = 3214,
            RC_LHC_TDC_AFT = 3215,
            RC_LHC_TDC_LEFT = 3216,
            RC_LHC_TDC_RIGHT = 3217,
            RC_LHC_TDC_PRESS = 3218,
            RC_LHC_AACQ_FWD = 3219,
            RC_LHC_AACQ_AFT = 3220,
            RC_LHC_AACQ_PRESS = 3221,
            RC_LHC_TRIGGER_1 = 3222,
            RC_LHC_TRIGGER_2 = 3223,
            RC_LHC_LASER_FIRE = 3224,
            RC_LHC_TDC_AXIS_HORZ = 3225,
            RC_LHC_TDC_AXIS_VERT = 3226,
            //    REAR COCKPIT - RIGHT HAND CONTROLLER
            RC_RHC_AAI_FWD = 3301,
            RC_RHC_AAI_AFT = 3302,
            RC_RHC_AAI_CTR = 3303,
            RC_RHC_COOLIE_UP = 3304,
            RC_RHC_COOLIE_DOWN = 3305,
            RC_RHC_COOLIE_LEFT = 3306,
            RC_RHC_COOLIE_RIGHT = 3307,
            RC_RHC_COOLIE_OFF = 3308,
            RC_RHC_CASTLE_FWD = 3309,
            RC_RHC_CASTLE_AFT = 3310,
            RC_RHC_CASTLE_LEFT = 3311,
            RC_RHC_CASTLE_RIGHT = 3312,
            RC_RHC_CASTLE_PRESS = 3313,
            RC_RHC_TDC_FWD = 3314,
            RC_RHC_TDC_AFT = 3315,
            RC_RHC_TDC_LEFT = 3316,
            RC_RHC_TDC_RIGHT = 3317,
            RC_RHC_TDC_PRESS = 3318,
            RC_RHC_AACQ_FWD = 3319,
            RC_RHC_AACQ_AFT = 3320,
            RC_RHC_AACQ_PRESS = 3321,
            RC_RHC_TRIGGER_1 = 3322,
            RC_RHC_TRIGGER_2 = 3323,
            RC_RHC_LASER_FIRE = 3324,
            RC_RHC_TDC_AXIS_HORZ = 3325,
            RC_RHC_TDC_AXIS_VERT = 3326,
            RC_SET_SWAP_HANDS = 3400,
            RC_TOGGLE_SWAP_HANDS = 3401,
        }
        internal enum amadctrl_commands
        {
            fire_ext_sw = 3314,
            amad_sw_cover = 3315,
            amad_sw = 3316,
            left_eng_fire_cover = 3317,
            left_eng_fire_sw = 3318,
            right_eng_fire_cover = 3319,
            right_eng_fire_sw = 3320,
        }
        internal enum armtctrl_commands
        {
            laser_code_2 = 3071,
            laser_code_3 = 3072,
            laser_code_4 = 3073,
            JETT_Selector_Knob = 3321,
            JETT_Button = 3322,
            Master_Arm_SW = 3323,
            EMER_JETT_Button = 3340,
            arm_safety_override_sw = 9999,
        }
        internal enum engpnl_commands
        {
            generator_left_sw = 3587,
            generator_right_sw = 3588,
            generator_emerg_sw = 3589,
            ground_power_sw = 3594,
            starter_gen_sw = 3595,
        }
        internal enum fuelpnl_commands
        {
            fueltrnfr_wing_sw = 3527,
            fueltrnfr_ctr_sw = 3528,
            fueltrnfr_cft_sw = 3529,
            fuel_dump_sw = 3530,
            fuel_cft_emergtrf_sw = 3531,
            fuel_ext_trfr_sw = 3532,
            fuel_AIR_sw = 3533,
            fuel_Emerg_AIR_cover = 3539,
            fuel_Emerg_AIR_sw = 3540,
            //    Interior Lights
        }
        internal enum intlt_commands
        {
            console_lt_knob = 3566,
            inst_pnl_lt_knob = 3567,
            ufc_bcklt_br_knob = 3568,
            lights_test_sw = 3569,
            compass_lt_sw = 3570,
            daynite_mode_sw = 3571,
            chart_lt_knob = 3572,
            wac_bklt_knob = 3573,
            wac_bklt_knob_reset = 3473,
            flood_lt_knob = 3574,
            chart_lt_lamp = 3575,
            //    
            compass_lt_tgl = 3165,
            daynite_mode_tgl = 3166,
            console_lt_kb = 3167,
            inst_pnl_lt_kb = 3168,
            inst_bklt_kb = 3169,
            chart_lt_kb = 3170,
            wac_bklt_kb = 3171,
            flood_lt_kb = 3172,
            chart_lt_lamp_kb = 3173,
            //    
            rc_console_lt_knob = 3456,
            rc_inst_pnl_lt_knob = 3457,
            rc_ufc_bcklt_br_knob = 3458,
            rc_lights_test_sw = 3459,
            rc_compass_lt_sw = 3460,
            rc_daynite_mode_sw = 3461,
            rc_chart_lt_knob = 3462,
            rc_wac_bklt_knob = 3463,
            rc_wac_bklt_knob_reset = 3474,
            rc_flood_lt_knob = 3464,
            rc_chart_lt_lamp = 3188,
            //    TEWS
        }
        internal enum tews_commands
        {
            rwr_power_sw = 3901,
            ewss_power_sw = 3902,
            ewss_tone_sw = 3903,
            rwr_power_sw_KB = 3904,
            ewss_power_sw_KB = 3905,
            ewss_tone_sw_KB = 3906,
            cmd_disp_sel_sw = 3911,
            cmd_mode_knob = 3912,
            cmd_jett_cover = 3913,
            cmd_jett_sw = 3914,
            cmd_mode_knob_KB = 3915,
            cmd_disp_sel_sw_KB = 3916,
            cmd_jett_cover_KB = 3917,
            cmd_jett_sw_KB = 3918,
            ics_power_sw = 3921,
            ics_set1_sw = 3922,
            ics_set2_sw = 3923,
            ics_set3_sw = 3924,
            ics_power_sw_KB = 3925,
            ics_set1_sw_KB = 3926,
            ics_set2_sw_KB = 3927,
            ics_set3_sw_KB = 3928,
            rwr_ics_mode_sw = 3931,
            pods_mode_sw = 3932,
            ics_mode_sw = 3933,
            //    
            rwr_ics_mode_sw_KB = 3934,
            pods_mode_sw_KB = 3935,
            ics_mode_sw_KB = 3936,
            //    misc
        }
        internal enum misc_commands
        {
            arr_hook_lever = 3336,
            arr_hook_lever_rear = 3344,
            em_jett_btn = 3340,
            em_bk_steer_lever = 3341,
            em_bk_steer_lever_rear = 3345,
            rud_adj_lever = 3342,
            rud_adj_lever_rear = 3346,
            em_vent_lever = 3427,
            jfs_lever = 3386,
            jfs_handle_turn = 3430,
            park_brake_sw = 3387,
            park_brake_sw_toggle = 3487,
            master_caution_btn = 3401,
            master_caution_btn_rc = 3176,
            roll_ratio_sw = 3534,
            left_inlet_sw = 3535,
            right_inlet_sw = 3536,
            anti_skid_sw = 3537,
            em_ar_cover = 3539,
            em_ar_sw = 3540,
            ewws_cover = 3518,
            ewws_sw = 3519,
            seat_adj_sw = 3521,
            seat_adj_sw_rc = 3431,
            seat_arm_handle = 3800,
            seat_arm_handle_rc = 3802,
            flyup_cover = 3522,
            flyup_sw = 3523,
            nctr_sw = 3524,
            hide_controls = 3001,
            rc_ac_vent_vertical = 3347,
            rc_ac_Vent_horizontal = 3348,
            mirror_center = 3010,
            mirror_left = 3011,
            mirror_right = 3012,
            mirror_center_adjust = 3910,
            mirror_left_adjust = 3911,
            mirror_right_adjust = 3912,
            service_door_cycle = 3913,
            service_door_open = 3914,
            service_door_close = 3915,
            //    NUC
        }
        internal enum nuc_commands
        {
            nuc_cover = 3450,
            nuc_sw = 3451,
            nuc_cover_rc = 3452,
            nuc_sw_rc = 3453,
            //    CAS
        }
        internal enum cas_commands
        {
            yaw_sw = 3452,
            roll_sw = 3453,
            pitch_sw = 3454,
            bit_button = 3455,
            tf_couple_sw = 3456,
            to_button = 3457,
            //    Volume controls
        }
        internal enum volctrl_commands
        {
            caution_vol = 3501,
            launch_vol = 3502,
            ics_vol = 3503,
            wpn_vol = 3504,
            ils_vol = 3505,
            tacan_vol = 3506,
            //    
            caution_keyb_vol = 3507,
            launch_keyb_vol = 3508,
            ics_keyb_vol = 3509,
            wpn_keyb_vol = 3510,
            ils_keyb_vol = 3511,
            tacan_keyb_vol = 3512,
            //    
            rc_caution_vol = 3601,
            rc_launch_vol = 3602,
            rc_ics_vol = 3603,
            rc_wpn_vol = 3604,
            rc_ils_vol = 3605,
            rc_tacan_vol = 3606,
            //    
            rc_caution_k_vol = 3607,
            rc_launch_k_vol = 3608,
            rc_ics_k_vol = 3609,
            rc_wpn_k_vol = 3610,
            rc_ils_k_vol = 3611,
            rc_tacan_k_vol = 3612,
            //    Mics
        }
        internal enum micsctrl_commands
        {
            crypto_sw = 3508,
            mic_sw = 3509,
            vw_tone_sw = 3510,
            //    
            rc_crypto_sw = 3426,
            rc_mic_sw = 3427,
            rc_vw_tone_sw = 3428,
            //    Radio
        }
        internal enum radioctrl_commands
        {
            uhf_ant_sw = 3511,
            vhf_ant_sw = 3512,
            tone_sw = 3513,
            cypher_txt_sw = 3514,
            //    
            rc_tone_sw = 3429,
            rc_cypher_txt_sw = 3430,
            //    IFF
        }
        internal enum iffctrl_commands
        {
            mode_sw = 3515,
            rec_sw = 3516,
            master_sw = 3517,
            iff_ant_sel_sw = 3520,
            //    Ext Lights
        }
        internal enum extlt_commands
        {
            ldg_taxi_light_sw = 3538,
            //    --
            formation_lt_knob = 3465,
            anticoll_lt_sw = 3466,
            pos_lt_knob = 3467,
            vert_tail_lt_sw = 3468,
            //    --
            ldg_taxi_light_kb = 3101,
            formation_lt_kb = 3102,
            anticoll_lt_kb = 3103,
            pos_lt_kb = 3104,
            vert_tail_lt_kb = 3105,
            //    SENSOR
        }
        internal enum snsrctrl_commands
        {
            tf_rdr_sw = 3469,
            rdr_alt_sw = 3470,
            rdr_power_sw = 3471,
            ins_knob = 3472,
            nav_flir_gain_knob = 3473,
            nav_flir_gain_level = 3474,
            nav_flir_sw = 3475,
            jtids_knob = 3476,
            cc_reset_btn = 3477,
            //    --------
            tpod_pwr_sw = 3413,
            tpod_flir_gain_knob = 3414,
            tpod_flir_level_knob = 3415,
            tpod_laser_sw = 3416,
            //    ---
            tf_rdr_kb = 3101,
            rdr_alt_kb = 3102,
            rdr_kb = 3103,
            ins_kb = 3104,
            nav_flir_gain_kb = 3105,
            nav_flir_level_kb = 3106,
            nav_flir_KB = 3107,
            jtids_kb = 3108,
            //    ---
            tpod_pwr_kb = 3110,
            tpod_flir_gain_kb = 3111,
            tpod_flir_level_kb = 3112,
            tpod_laser_kb = 3113,
            //    Ground Power
        }
        internal enum gndpwrctrl_commands
        {
            gnd_pwr_2_sw = 3478,
            gnd_pwr_3_sw = 3479,
            gnd_pwr_4_sw = 3480,
            pacs_sw = 3481,
            gnd_pwr_1_sw = 3483,
            mpdp_A1U_sw = 3484,
            //    FUEL
        }
        internal enum fuelctrl_commands
        {
            fueltrnfr_wing_sw = 3527,
            fueltrnfr_ctr_sw = 3528,
            fueltrnfr_cft_sw = 3529,
            fuel_dump_sw = 3530,
            fuel_cft_emergtrf_sw = 3531,
            fuel_ext_trfr_sw = 3532,
            fuel_AIR_sw = 3533,
            fuel_Emerg_AIR_cover = 3539,
            fuel_Emerg_AIR_sw = 3540,
            fuel_AIR_sw_KB = 3601,
            fuel_AIR_sw_KB_toggle = 3602,
            //    OXYGEN
        }
        internal enum oxyctrl_commands
        {
            oxy_test_btn = 3556,
            oxy_emer_norm_test_sw = 3551,
            oxy_100_norm_sw = 3552,
            oxy_pbg_on_off_sw = 3553,
            //    --
            wso_oxy_emer_norm_test_sw = 3571,
            wso_oxy_100_norm_sw = 3572,
            wso_oxy_pbg_on_off_sw = 3573,
            wso_oxy_test_btn = 3576,
            //    ECS
        }
        internal enum ecs_commands
        {
            anti_fog_sw = 3558,
            windshield_anti_ice_sw = 3559,
            pitot_heat_sw = 3560,
            eng_heat_sw = 3561,
            //    ENGINES
        }
        internal enum engctrl_commands
        {
            vmax_cover = 3525,
            vmax_sw = 3526,
            left_eng_ctrl_sw = 3590,
            right_eng_ctrl_sw = 3591,
            left_eng_master_cover = 3592,
            left_eng_master_sw = 3593,
            right_eng_master_cover = 3597,
            right_eng_master_sw = 3598,
            left_eng_finger_lift = 3697,
            right_eng_finger_lift = 3698,
            //    AIRCO
        }
        internal enum aircoctrl_commands
        {
            airco_auto_man_off_sw = 3562,
            airco_max_norm_min_sw = 3563,
            airco_cold_hot_knob = 3564,
            airco_eng_knob = 3565,
            airco_cold_hot_knob_kb = 3664,
            //    Canopy
        }
        internal enum cnp_commands
        {
            cnpy_lever = 3599,
            em_cnpy_jett_lever = 3428,
            rear_cnpy_lever = 3600,
            rear_em_cnpy_jett_lever = 3385,
            eject_select_valve = 3386,
        }
    }
}
