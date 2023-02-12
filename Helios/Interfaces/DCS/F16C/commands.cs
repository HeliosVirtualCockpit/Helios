
namespace GadrocsWorkshop.Helios.Interfaces.DCS.F16C
{
    internal class F16CCommands
    {
        internal enum deviceCommands
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
            Button_70 = 3070
        }
        internal enum controlCommands
        {
            DigitalBackup = 3001,
            AltFlaps,
            BitSw,
            FlcsReset,
            LeFlaps,
            TrimApDisc,
            RollTrim,
            PitchTrim,
            YawTrim,
            ManualPitchOverride,
            StoresConfig,
            ApPitchAtt,
            ApPitchAlt,
            ApRoll,
            AdvMode,
            ManualTfFlyup,
            ThrottleFriction,
            AB_DETENT,
            // input commands
            DigitalBackup_ITER,
            AltFlaps_ITER,
            LeFlaps_ITER,
            TrimApDisc_ITER,
            RollTrim_ITER,
            RollTrim_AXIS,
            PitchTrim_ITER,
            PitchTrim_AXIS,
            YawTrim_ITER,
            YawTrim_AXIS,
            ManualPitchOverride_ITER,
            StoresConfig_ITER,
            ApPitchAtt_EXT,
            ApPitchAlt_EXT,
            ApRoll_ITER,
            AdvMode_ITER,
            ManualTfFlyup_ITER,
            ThrottleFriction_ITER
        }
        internal enum elecCommands
        {
            MainPwrSw = 3001,
            CautionResetBtn,
            FlcsPwrTestSwMAINT,
            FlcsPwrTestSwTEST,
            EPU_GEN_TestSw,
            ProbeHeatSw,
            ProbeHeatSwTEST,
            // input commands
            MainPwrSw_ITER,
            FlcsPwrTestSw_ITER,
            EPU_GEN_TestSw_ITER,
            ProbeHeatSw_EXT,
            ProbeHeatSw_ITER
        }
        internal enum fuelCommands
        {
            FuelMasterSw = 3001,
            FuelMasterSwCvr,
            ExtFuelTransferSw,
            EngineFeedSw,
            FuelQtySelSw,
            FuelQtySelSwTEST,
            TankInertingSw,
            AirRefuelSw,
            //input commands
            FuelMasterSw_ITER,
            FuelMasterSwCvr_ITER,
            ExtFuelTransferSw_ITER,
            EngineFeedSw_ITER,
            FuelQtySelSw_ITER,
            TankInertingSw_ITER,
            AirRefuelSw_ITER
        }
        internal enum engineCommands
        {
            EpuSwCvrOn = 3001,
            EpuSwCvrOff,
            EpuSw,
            EngAntiIceSw,
            JfsSwStart1,
            JfsSwStart2,
            EngContSwCvr,
            EngContSw,
            MaxPowerSw,
            ABResetSwReset,
            ABResetSwEngData,
            FireOheatTestBtn,
            // input commands
            EpuSwCvrOn_ITER,
            EpuSwCvrOff_ITER,
            EpuSw_ITER,
            EngAntiIceSw_ITER,
            EngContSwCvr_ITER,
            EngContSw_ITER,
            MaxPowerSw_ITER
        }
        internal enum gearCommands
        {
            LGHandle = 3001,
            DownLockRelBtn,
            ParkingSw,
            AntiSkidSw,
            BrakesChannelSw,
            HookSw,
            HornSilencerBtn,
            AltGearHandle,
            AltGearResetBtn,
            // input commands
            AntiSkidSw_EXT,
            BrakesChannelSw_ITER,
            HookSw_ITER
        }
        internal enum oxygenCommands
        {
            SupplyLever = 3001,
            DiluterLever,
            EmergencyLever,
            EmergencyLeverTestMask,
            ObogsBitSw,
            // input commands
            SupplyLever_ITER,
            DiluterLever_ITER,
            EmergencyLever_ITER,
        }
        internal enum cptCommands
        {
            CanopyHandcrank = 3001,
            CanopySwitchOpen,
            CanopySwitchClose,
            CanopyHandle,
            CanopyTHandle,
            EjectionHandle,
            ShoulderHarnessKnob,
            EmergencyOxygenGreenRing,
            EjectionSafetyLever,
            RadioBeaconSwitch,
            SurvivalKitDeploymentSwitch,
            EmergencyManualChuteHandle,
            SeatAdjSwitchUp,
            SeatAdjSwitchDown,
            StickHide,
            // input commands
            CanopyHandcrank_ITER,
            CanopySwitch_ITER,
            CanopyHandle_ITER,
            CanopyTHandle_ITER,
            ShoulderHarnessKnob_ITER,
            EjectionSafetyLever_ITER,
            RadioBeaconSwitch_ITER,
            SurvivalKitDeploymentSwitch_ITER,
            StickHide_EXT,

            SmokeDevice,
        }
        internal enum extlightsCommands
        {
            AntiCollKn = 3001,
            PosFlash,
            PosWingTail,
            PosFus,
            FormKn,
            Master,
            AerialRefuel,
            LandingTaxi,
            // input commands
            AntiCollKn_ITER,
            PosFlash_ITER,
            PosWingTail_ITER,
            PosFus_ITER,
            FormKn_ITER,
            FormKn_AXIS,
            Master_ITER,
            AerialRefuel_ITER,
            AerialRefuel_AXIS,
            LandingTaxi_ITER,
        }
        internal enum cptlightsCommands
        {
            MasterCaution = 3001,
            MalIndLtsTest,
            Consoles,
            IntsPnl,
            DataEntryDisplay,
            ConsolesFlood,
            InstPnlFlood,
            MalIndLtsBrt,
            MalIndLtsDim,
            IndBrtAoA,
            IndBrtAR,
            //input commands
            Consoles_EXT,
            Consoles_AXIS,
            IntsPnl_EXT,
            IntsPnl_AXIS,
            DataEntryDisplay_EXT,
            DataEntryDisplay_AXIS,
            ConsolesFlood_EXT,
            ConsolesFlood_AXIS,
            InstPnlFlood_EXT,
            InstPnlFlood_AXIS,
            UtilityBrt_ITER,
            UtilityBrt_AXIS,
            IndBrtAoA_ITER,
            IndBrtAoA_AXIS,
            IndBrtAR_ITER,
            IndBrtAR_AXIS,
        }
        internal enum hotasCommands
        {
            // Stick
            STICK_NWS_AR_DISC_MSL_STEP = 3001,
            STICK_TRIMMER_UP,       // Trimmer
            STICK_TRIMMER_DOWN,
            STICK_TRIMMER_LEFT,
            STICK_TRIMMER_RIGHT,
            STICK_DISP_MANAGE_UP,       // Display Management Switch
            STICK_DISP_MANAGE_DOWN,
            STICK_DISP_MANAGE_LEFT,
            STICK_DISP_MANAGE_RIGHT,
            STICK_TGT_MANAGE_UP,        // Target Management Switch
            STICK_TGT_MANAGE_DOWN,
            STICK_TGT_MANAGE_LEFT,
            STICK_TGT_MANAGE_RIGHT,
            STICK_CMS_MANAGE_FWD,       // Countermeasures Management Switch
            STICK_CMS_MANAGE_AFT,
            STICK_CMS_MANAGE_LEFT,
            STICK_CMS_MANAGE_RIGHT,
            STICK_EXPAND_FOV,
            STICK_PADDLE,
            STICK_TRIGGER_1ST_DETENT,
            STICK_TRIGGER_2ND_DETENT,
            STICK_WEAPON_RELEASE,
            // Throttle
            THROTTLE_CUTOFF_RELEASE,
            THROTTLE_TRANSMIT_FWD,      // Transmit Switch
            THROTTLE_TRANSMIT_AFT,
            THROTTLE_TRANSMIT_LEFT,
            THROTTLE_TRANSMIT_RIGHT,
            THROTTLE_MAN_RNG,
            THROTTLE_UNCAGE,
            THROTTLE_DOG_FIGHT,
            THROTTLE_SPEED_BRAKE,
            THROTTLE_ANT_ELEV_AXIS,
            THROTTLE_ANT_ELEV_UP,
            THROTTLE_ANT_ELEV_DOWN,
            THROTTLE_RDR_CURSOR_FWD,        // Radar Cursor Switch
            THROTTLE_RDR_CURSOR_AFT,
            THROTTLE_RDR_CURSOR_LEFT,
            THROTTLE_RDR_CURSOR_RIGHT,
            THROTTLE_ENABLE,
            // input commands
            //	THROTTLE_MAN_RNG_ITER			,
            //	THROTTLE_MAN_RNG_AXIS			,
            THROTTLE_MAN_RNG_INC,
            THROTTLE_MAN_RNG_DEC,
            THROTTLE_DOG_FIGHT_ITER,
            THROTTLE_DOG_FIGHT_CYCL,
            THROTTLE_DOG_FIGHT_EXT,
            THROTTLE_SPEEDSPEED_BRAKE_EXT,
            THROTTLE_RDR_CURSOR_Y_AXIS,
            THROTTLE_RDR_CURSOR_X_AXIS,

            THROTTLE_TRANSMIT_FWD_VOIP,
            THROTTLE_TRANSMIT_AFT_VOIP,
        }
        internal enum ecsCommands
        {
            AirSourceKnob = 3001,
            TempKnob,
            DefogLever,
            // input commands
            AirSourceKnob_ITER,
            TempKnob_ITER,
            TempKnob_AXIS,
            DefogLever_ITER,
            DefogLever_AXIS,
        }
        internal enum ufcCommands
        {
            UFC_Sw = 3001,

            DIG0_M_SEL,
            DIG1_T_ILS,
            DIG2_ALOW,
            DIG3,
            DIG4_STPT,
            DIG5_CRUS,
            DIG6_TIME,
            DIG7_MARK,
            DIG8_FIX,
            DIG9_A_CAL,

            COM1,
            COM2,
            IFF,
            LIST,

            ENTR,
            RCL,
            AA,
            AG,

            RET_DEPR_Knob,
            CONT_Knob,
            SYM_Knob,
            BRT_Knob,

            Wx,
            FLIR_INC,
            FLIR_DEC,
            FLIR_GAIN_Sw,

            DRIFT_CUTOUT,
            WARN_RESET,

            DED_INC,
            DED_DEC,
            DCS_RTN,
            DCS_SEQ,
            DCS_UP,
            DCS_DOWN,

            F_ACK,
            IFF_IDENT,
            RF_Sw,

            // input commands
            UFC_Sw_ITER,

            SYM_Knob_ITER,
            SYM_Knob_AXIS,
            RET_DEPR_Knob_ITER,
            RET_DEPR_Knob_AXIS,
            BRT_Knob_ITER,
            BRT_Knob_AXIS,
            CONT_Knob_ITER,
            CONT_Knob_AXIS,
            FLIR_GAIN_Sw_ITER,
            DriftCO_WarnReset_ITER,
            RF_Sw_ITER,
        }
        internal enum mmcCommands
        {
            MmcPwr = 3001,
            MasterArmSw,
            EmerStoresJett,
            GroundJett,
            AltRel,
            VvVah,
            AttFpm,
            DedData,
            DeprRet,
            Spd,
            Alt,
            Brt,
            Test,
            MFD,
            // input commands
            MmcPwr_ITER,
            MasterArmSw_ITER,
            MasterArmSw_EXT,
            GroundJett_ITER,
            VvVah_EXT,
            AttFpm_EXT,
            DedData_EXT,
            DeprRet_EXT,
            Spd_EXT,
            Alt_EXT,
            Brt_EXT,
            Test_EXT,
            MFD_ITER,
        }
        internal enum fcrCommands
        {
            PwrSw = 3001,
            // input commands
            PwrSw_ITER,
        }
        internal enum raltCommands
        {
            PwrSw = 3001,
            // input commands
            PwrSw_ITER,
        }
        internal enum smsCommands
        {
            StStaSw = 3001,
            LeftHDPT,
            RightHDPT,
            LaserSw,
            // input commands
            StSta_ITER,
            LeftHDPT_ITER,
            RightHDPT_ITER,
            LaserSw_ITER,
            //
            ChangeLaserCode100,
            ChangeLaserCode10,
            ChangeLaserCode1,
        }
        internal enum hmcsCommands
        {
            IntKnob = 3001,
            // input commands
            IntKnob_ITER,
            IntKnob_AXIS,
        }
        internal enum rwrCommands
        {
            IntKnob = 3001,        // Prime panel
            Handoff,
            Launch,
            Mode,
            UnknownShip,
            SysTest,
            TgtSep,
            BrtKnob,        // Auxiliary panel
            Search,
            ActPwr,
            Power,
            Altitude,
            // input commands
            IntKnob_ITER,
            IntKnob_AXIS,
            BrtKnob_ITER,
            BrtKnob_AXIS,
            Power_ITER,
        }
        internal enum cmdsCommands
        {
            RwrSrc = 3001,
            JmrSrc,
            MwsSrc,
            Jett,
            O1Exp,
            O2Exp,
            ChExp,
            FlExp,
            Prgm,
            Mode,
            DispBtn,
            // input commands
            RwrSrc_ITER,
            JmrSrc_ITER,
            MwsSrc_ITER,
            Jett_ITER,
            O1Exp_ITER,
            O2Exp_ITER,
            ChExp_ITER,
            FlExp_ITER,
            Prgm_ITER,
            Mode_ITER,
        }
        internal enum saiCommands
        {
            test = 3001,
            cage,
            reference,
            power,
            // input commands
            reference_EXT,
            power_EXT,
            cage_EXT,
            reference_AXIS,
        }
        internal enum intercomCommands
        {
            COM1_PowerKnob = 3001,
            COM1_ModeKnob,
            COM2_PowerKnob,
            COM2_ModeKnob,
            SecureVoiceKnob,
            MSL_ToneKnob,
            TF_ToneKnob,
            THREAT_ToneKnob,
            ILS_PowerKnob,
            TACAN_Knob,
            INTERCOM_Knob,
            HotMicCipherSw,
            IFF_AntSelSw,
            UHF_AntSelSw,
            VMS_InhibitSw,
            PlainCipherSw,
            ZeroizeSwCvr,
            ZeroizeSw,
            //
            COM1_PowerKnob_ITER,
            COM1_PowerKnob_AXIS,
            COM1_ModeKnob_ITER,
            COM2_PowerKnob_ITER,
            COM2_PowerKnob_AXIS,
            COM2_ModeKnob_ITER,
            SecureVoiceKnob_ITER,
            SecureVoiceKnob_AXIS,
            MSL_ToneKnob_ITER,
            MSL_ToneKnob_AXIS,
            TF_ToneKnob_ITER,
            TF_ToneKnob_AXIS,
            THREAT_ToneKnob_ITER,
            THREAT_ToneKnob_AXIS,
            ILS_PowerKnob_ITER,
            ILS_PowerKnob_AXIS,
            TACAN_Knob_ITER,
            TACAN_Knob_AXIS,
            INTERCOM_Knob_ITER,
            INTERCOM_Knob_AXIS,
            HotMicCipherSw_ITER,
            IFF_AntSelSw_ITER,
            UHF_AntSelSw_ITER,
            VMS_InhibitSw_ITER,
            PlainCipherSw_ITER,
            ZeroizeSwCvr_ITER,
            ZeroizeSw_ITER,
        }
        internal enum uhfCommands
        {
            ChannelKnob = 3001,
            FreqSelector100Mhz,
            FreqSelector10Mhz,
            FreqSelector1Mhz,
            FreqSelector01Mhz,
            FreqSelector0025Mhz,
            FreqModeKnob,
            FunctionKnob,
            TToneSw,
            SquelchSw,
            VolumeKnob,
            TestDisplayBtn,
            StatusBtn,
            AccessDoor,
            LoadBtn,
            ZeroSw,
            MnSq,
            GdSq,
            //
            FunctionKnob_ITER,
            FreqModeKnob_ITER,
            TToneSw_ITER,
            SquelchSw_ITER,
            VolumeKnob_ITER,
            VolumeKnob_AXIS,
            AccessDoor_ITER,
            ZeroSw_ITER,
            FreqSelector100Mhz_ITER,
        }
        internal enum iffCommands
        {
            CNI_Knob = 3001,
            MasterKnob,
            M4CodeSw,
            M4ReplySw,
            M4MonitorSw,
            EnableSw,
            M1M3Selector1_Inc,
            M1M3Selector1_Dec,
            M1M3Selector2_Inc,
            M1M3Selector2_Dec,
            M1M3Selector3_Inc,
            M1M3Selector3_Dec,
            M1M3Selector4_Inc,
            M1M3Selector4_Dec,

            // input commands
            CNI_Knob_ITER,
            MasterKnob_ITER,
            M4CodeSw_ITER,
            M4ReplySw_ITER,
            M4MonitorSw_ITER,
            EnableSw_ITER,
        }
        internal enum ky58Commands
        {
            KY58_ModeSw = 3001,
            KY58_FillSw,
            KY58_FillSw_Pull,
            KY58_PowerSw,
            KY58_Volume,

            // input commands
            KY58_ModeSw_ITER,
            KY58_FillSw_ITER,
            KY58_PowerSw_ITER,
            KY58_Volume_ITER,
            KY58_Volume_AXIS,
        }
        internal enum mfdCommands
        {
            OSB_1 = 3001,
            OSB_2 = 3002,
            OSB_3 = 3003,
            OSB_4 = 3004,
            OSB_5 = 3005,
            OSB_6 = 3006,
            OSB_7 = 3007,
            OSB_8 = 3008,
            OSB_9 = 3009,
            OSB_10 = 3010,
            OSB_11 = 3011,
            OSB_12 = 3012,
            OSB_13 = 3013,
            OSB_14 = 3014,
            OSB_15 = 3015,
            OSB_16 = 3016,
            OSB_17 = 3017,
            OSB_18 = 3018,
            OSB_19 = 3019,
            OSB_20 = 3020,
            GAIN_Rocker_UP,
            GAIN_Rocker_DOWN,
            SYM_Rocker_UP,
            SYM_Rocker_DOWN,
            CON_Rocker_UP,
            CON_Rocker_DOWN,
            BRT_Rocker_UP,
            BRT_Rocker_DOWN,
        }
        internal enum amiCommands
        {
            SettingKnob = 3001,
        }
        internal enum ehsiCommands
        {
            ModeBtn = 3001,
            LeftKnob,
            LeftKnobBtn,
            RightKnob,
            RightKnobBtn,
        }
        internal enum clockCommands
        {
            CLOCK_left_lev_up = 3001,
            CLOCK_left_lev_rotate,
            CLOCK_right_lev_down,
        }
        internal enum altCommands
        {
            PNEU = 3001,
            ELEC,
            ZERO,
        }
        internal enum midsCommands
        {
            PwrSw = 3001,
            //
            PwrSw_ITER,
        }
        internal enum insCommands
        {
            ModeKnob = 3001,
            //
            ModeKnob_ITER,
        }
        internal enum gpsCommands
        {
            PwrSw = 3001,
            //
            PwrSw_ITER,
        }
        internal enum idmCommands
        {
            PwrSw = 3001,
            //
            PwrSw_ITER,
        }
        internal enum mapCommands
        {
            PwrSw = 3001,
            //
            PwrSw_ITER,
        }
        internal enum ecmCommands
        {
            PwrSw = 3001,
            XmitSw,
            DimRotary,
            ResetBtn,
            BitBtn,
            OneBtn,
            TwoBtn,
            ThreeBtn,
            FourBtn,
            FiveBtn,
            SixBtn,
            FrmBtn,
            SplBtn,
            // input
            PwrSw_ITER,
            XmitSw_ITER,
            DimRotary_ITER,
            DimRotary_AXIS,
            OneBtn_ITER,
            TwoBtn_ITER,
            ThreeBtn_ITER,
            FourBtn_ITER,
            FiveBtn_ITER,
            SixBtn_ITER,
            FrmBtn_ITER,
            SplBtn_ITER,
        }
    }
}
