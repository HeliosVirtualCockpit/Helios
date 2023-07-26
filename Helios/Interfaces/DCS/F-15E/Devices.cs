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
    internal enum devices
    {
        RDRCTRL_AA = 1,
        RDRCTRL_AG = 2,
        AN_APG70 = 3,
        HOTASCTRL = 4,
        HPSS = 5,
        EPSS = 6,
        UHF1 = 7,
        UHF2 = 8,
        UHF3 = 9,
        TACAN = 10,
        ILS = 11,
        ALG = 12,
        ICS = 13,
        KY_HQ = 14,
        DEEC = 15,
        AFSS = 16,
        FLINST = 17,
        FLCTRL = 18,
        AFCS = 19,
        GCWS = 20,
        LGS = 21,
        EXTLT = 22,
        INTLT = 23,
        CNPYSYST = 24,
        MSOGS = 25,
        ECS = 26,
        OWS = 27,
        FWES = 28,
        WCAS = 29,
        BITS = 30,
        ADC = 31,
        ACC = 32,
        DMUX = 33,
        MPD_FLEFT = 34,
        MPCD_FCENTER = 35,
        MPD_FRIGHT = 36,
        MPCD_RLEFT = 37,
        MPD_RLEFT = 38,
        MPD_RRIGHT = 39,
        MPCD_RRIGHT = 40,
        INS_EGI = 41,
        INS = 42,
        DMS = 43,
        PACS = 44,
        IBS = 45,
        AGWCTRL = 46,
        AAWCTRL = 47,
        GUNCTRL = 48,
        AGMCTRL = 49,
        NAVPOD = 50,
        // ** Note ** the comments in the Razbam file which give the device number are incorrect because they removed item 51 and didn't change the comments.
        LANTPOD = 51,
        LTNGPOD = 52,
        SNPRPOD = 53,
        TGPCTRL = 54,
        HUDCTRL = 55,
        UFCCTRL_FRONT = 56,
        UFCCTRL_REAR = 57,
        EWS_RWR = 58,
        TEWS = 59,
        EWS_CMD = 60,
        EWS_JMR = 61,
        AUTOSTART = 62,
        TAPEREC = 63,
        DATALINK = 64,
        TFR = 65,
        ANVIS9 = 66,
        CARA = 67,
        SYSTEMS_SYNC = 68,
        SWITCHS_SYNC = 69,
        DATA_HUB = 70,
    }

}
