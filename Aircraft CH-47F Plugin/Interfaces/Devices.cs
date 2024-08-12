//  Copyright 2024 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.CH47F
{
    internal enum devices
    {
        MCPU = 1,
        CDU_LEFT,
        CDU_RIGHT,
        MFD_COPILOT_OUTBOARD,
        MFD_COPILOT_INBOARD,
        MFD_CENTER,
        MFD_PILOT_INBOARD,
        MFD_PILOT_OUTBOARD,
        ADC,
        DTU,
        CAAS,
        FADEC1,
        FADEC2,
        FM_PROXY,
        CONTROLS,
        GUNNER_STATIONS,
        PDP1,
        PDP2,
        OVERHEAD_CONSOLE,
        CENTRAL_CONSOLE,
        CANTED_CONSOLE,
        SFD1,
        SFD2,
        MAINTENANCE_PANEL,
        CHRONOMETER,
        COMPASS,
        EXTERNAL_CARGO_EQUIPMENT,
        EXTERNAL_CARGO_SPEECH,
        GRIPS,
        HEATER,
        EMERGENCY_PANEL,
        LIGHTING_REFLECTS,
        TERTIARY_REFLECTS,
        FD,
        TURN,
        GPS1,
        GPS2,
        EGI1,
        EGI2,
        TACAN,
        ILS,
        MLS,
        MARKER_BEACON,
        ARN_147,
        ARN_149,
        APN_209,
        ANV_241A,
        ARC_164,
        ARC_186,
        ARC_201,
        ARC_220,
        COMM_PANEL_LEFT, // Communications Panel (COMM - LEFT)
        COMM_PANEL_RIGHT, // Communications Panel (COMM - RIGHT)
        COMM_PANEL_TROOP_COMMANDER,
        COMM_PANEL_LH_GUNNER,
        COMM_PANEL_RH_GUNNER,
        COMM_PANEL_AFT_ENGINEER,
        INTERCOM, // must be last in radio group
        CREWE,
        WORKSTATIONS,
        AFT_WORKSTATION,
        HELMET_DEVICE,
        MACROS,
        ARCADE,
        KNEEBOARD,
        AN_APR39, // RWR
        AN_AAR57, // CMWS
        AN_ALE47, // ICMDS
        VPM1, // Video Processing Module
        VPM2, // Video Processing Module
    }

}
