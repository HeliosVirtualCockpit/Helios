//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon.BMS
{
    class BMSFalconDataExporter : FalconDataExporter
    {
        private const int SLOW_BLINK_LENGTH_MS = 500;
        private const int FAST_BLINK_LENGTH_MS = 200;
        private const int PHASE_LENGTH = 1;
        private const int MAX_SECONDS = 60 * 60 * 24;

        private SharedMemory _sharedMemory = null;
        private SharedMemory _sharedMemory2 = null;
        private SharedMemory _sharedMemoryStringArea = null;

        private RadarContact[] _contacts = new RadarContact[40];
        private string[] _rwrInfo;

        private FlightData _lastFlightData;
        private FlightData2 _lastFlightData2;
        private uint _stringAreaSize;
        private uint _stringAreaTime;

        private DateTime _outerMarkerLastTick;
        private bool _outerMarkerOnState;

        private DateTime _middleMarkerLastTick;
        private bool _middleMarkerOnState;

        private DateTime _probeheatLastTick;
        private bool _probeheatOnState;

        private DateTime _auxsrchLastTick;
        private bool _auxsrchOnState;

        private DateTime _launchLastTick;
        private bool _launchOnState;

        private DateTime _primodeLastTick;
        private bool _primodeOnState;

        private DateTime _unkLastTick;
        private bool _unkOnState;
        private List<string> _navPoints;
        private string _acName;
        private string _acNCTR;
        private bool _stringDataUpdated;
        private uint _lastStringAreaTime;
        private bool _panelTypeIFF;

        public BMSFalconDataExporter(FalconInterface falconInterface)
            : base(falconInterface)
        {
            // Altimeter Bits
            AddValue("Altimeter", "altitidue", "Current altitude of the aircraft.", "Altitude in feet.", BindingValueUnits.Feet);
            AddValue("Altimeter", "indicated altitude", "Inidicated barometric altitude. (Depends on calibration)", "Altitiude in feet.", BindingValueUnits.Feet);
            AddValue("Altimeter", "barimetric pressure", "Calibrated barimetric pressure.", "", BindingValueUnits.InchesOfMercury);
            AddValue("Altimeter", "altimeter calibration type", "", "True if hg otherwise hpa.", BindingValueUnits.Boolean);
            AddValue("Altimeter", "altimeter pneu flag", "", "True if visible", BindingValueUnits.Boolean);
            AddValue("Altimeter", "radar alt", "radar altitude", "Altitude in feet.", BindingValueUnits.Feet);

            //Alt Bits
            AddValue("Altitude", "Cabin Altitude", "Current cabin altitude", "", BindingValueUnits.Numeric);

            // IAS Bits
            AddValue("IAS", "mach", "Current mach speed of the aircraft.", "", BindingValueUnits.Numeric);
            AddValue("IAS", "indicated air speed", "Current indicated air speed in knots.", "", BindingValueUnits.Knots);
            AddValue("IAS", "true air speed", "Current true air speed in feet per second.", "", BindingValueUnits.FeetPerSecond);

            // General Bits
            AddValue("General", "Gs", "Current g-force load", "", BindingValueUnits.Numeric);
            AddValue("General", "speed brake position", "Speed brake position", "0(Fully Closed) to 1(Fully Open)", BindingValueUnits.Degrees);
            AddValue("General", "speed brake indicator", "Speed brake open indicator.", "True if the speed brake is open at all.", BindingValueUnits.Boolean);
            AddValue("General", "on ground", "Indicates weight on wheels.", "True if wheight is on wheels.", BindingValueUnits.Boolean);
            AddValue("General", "parking brake engaged", "Indicates if the parking brake is engaged.", "True if engaged", BindingValueUnits.Boolean);
            AddValue("General", "speed barke", "Indicates if the speed brake is deployed.", "True if speed breake is in any other position than stowed.", BindingValueUnits.Boolean);
            AddValue("General", "power off", "Flag indicating if the cockpit has any power.", "True if the cockpit does not have any power.", BindingValueUnits.Boolean);

            // Fuel Bits
            AddValue("Fuel", "internal fuel", "Amount of fuel in the internal tanks.", "", BindingValueUnits.Pounds);
            AddValue("Fuel", "external fuel", "Amount of fuel in the external tanks.", "", BindingValueUnits.Pounds);
            AddValue("Fuel", "fwd fuel", "Amount of fuel in the fwd tanks", "", BindingValueUnits.Pounds);
            AddValue("Fuel", "aft fuel", "Amount of fuel in the aft tanks", "", BindingValueUnits.Pounds);
            AddValue("Fuel", "total fuel", "Amount of total fuel", "", BindingValueUnits.Pounds);

            // EPU Fuel Bits
            AddValue("EPU", "fuel", "Remaining EPU fuel.", "Percent (0-100)", BindingValueUnits.Numeric);

            // Engine Bits
            AddValue("Engine", "fuel flow", "Current fuel flow to the engine.", "", BindingValueUnits.PoundsPerHour);
            AddValue("Engine", "rpm", "Current RPM of the engine.", "Percent (0-103)", BindingValueUnits.RPMPercent);
            AddValue("Engine", "ftit", "Forward turbine intake temperature", "", BindingValueUnits.Celsius);
            AddValue("Engine", "nozzle position", "Current afterburner nozzel position.", "Percent open (0-100)", BindingValueUnits.Numeric);
            AddValue("Engine", "oil pressure", "Current oil pressure in the engine.", "Percent (0-100)", BindingValueUnits.Numeric);
            AddValue("Engine", "nozzle 2 position", "Current engine nozzle2.", "Percent open (0-100)", BindingValueUnits.Numeric);
            AddValue("Engine", "rpm2", "Current engine rpm2.", "Percent (0-103)", BindingValueUnits.Numeric);
            AddValue("Engine", "ftit2", "Current forward turbine inlet temp2", "Degrees C", BindingValueUnits.Numeric);
            AddValue("Engine", "oil pressure 2", "Current oil pressure 2 in the engine.", "Percent (0-100)", BindingValueUnits.Numeric);
            AddValue("Engine", "fuel flow 2", "Current fuel flow to the engine 2.", "", BindingValueUnits.PoundsPerHour);

            // Landing Gear Bits
            AddValue("Landging Gear", "position", "Landing gear current position.", "True for down, false for up", BindingValueUnits.Boolean, "Landing Gear");
            AddValue("Landing Gear", "nose gear indicator", "Landing gear panel nose gear indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Landing Gear", "left gear indicator", "Landing gear panel left gear indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Landing Gear", "right gear indicator", "Landing gear panel right gear indicator.", "True if lit.", BindingValueUnits.Boolean);

            // Gear Handle Bits
            AddValue("Gear Handle", "gear handle solenoid", "Landing gear handle solenoid status", "True if failed", BindingValueUnits.Boolean);
            AddValue("Gear Handle", "handle indicator", "Landing gear handle indicator light.", "True if lit.", BindingValueUnits.Boolean, "Landing Gear");
            

            // CMDS Bits
            AddValue("CMDS", "chaff remaining", "Number chaff charges remaining.", "", BindingValueUnits.Numeric);
            AddValue("CMDS", "flares remaining", "Number of flares remaining.", "", BindingValueUnits.Numeric);
            AddValue("CMDS", "CMDS Mode", "Current CMDS mode", "(0 off, 1 stby, 2 Man, 3 Semi, 4 Auto, 5 BYP)", BindingValueUnits.Numeric);
            AddValue("CMDS", "Go", "CMDS is on and operating normally.", "True if CMDS is on and operating", BindingValueUnits.Boolean);
            AddValue("CMDS", "NoGo", "CMDS is on but a malfunction is present.", "True if CMDS is on but malfunctioning", BindingValueUnits.Boolean);
            AddValue("CMDS", "Degr", "Status message AUTO DEGR should be displayed.", "True if AUTO DEGR should be displayed", BindingValueUnits.Boolean);
            AddValue("CMDS", "Rdy", "Status message DISPENSE RDY should be displayed.", "True if DISPENSE RDY should be displayed", BindingValueUnits.Boolean);
            AddValue("CMDS", "ChaffLo", "Indicates bingo chaff quantity is reached.", "True if bingo quantity reached", BindingValueUnits.Boolean);
            AddValue("CMDS", "FlareLo", "Inidcates bingo flare quantity is reached.", "True if bingo quantity reached", BindingValueUnits.Boolean);
            
            // ECM Bits
            AddValue("ECM", "power indicator", "ECM power indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("ECM", "fail indicator", "ECM failure indicator.", "True if lit.", BindingValueUnits.Boolean);

            // Trim Bits
            AddValue("Trim", "roll trim", "Amount of roll trim currently set.", "(-0.5 to 0.5)", BindingValueUnits.Numeric);
            AddValue("Trim", "pitch trim", "Amount of pitch trim currently set.", "(-0.5 to 0.5)", BindingValueUnits.Numeric);
            AddValue("Trim", "yaw trim", "Amount of yaw trim currently set.", "(-0.5 to 0.5)", BindingValueUnits.Numeric);

            // Right Eyebrow Bits
            AddValue("Right Eyebrow", "oxy low indicator", "OXY LOW indicator on right eyebrow", "True if lit", BindingValueUnits.Boolean);
            AddValue("Right Eyebrow", "flcs indicator", "FLCS Indicator", "True if lit", BindingValueUnits.Boolean);
            AddValue("Right Eyebrow", "dbu on indicator", "DBU Warning light on the right eyebrow.", "True if lit", BindingValueUnits.Boolean);
            AddValue("Right Eyebrow", "engine fire indicator", "", "True if engine fire is detected.", BindingValueUnits.Boolean);
            AddValue("Right Eyebrow", "hydraulic/oil indicator", "", "True if hydraulic pressure is to high.", BindingValueUnits.Boolean);
            AddValue("Right Eyebrow", "canopy indicator", "", "True if canopy is not closed or is damaged", BindingValueUnits.Boolean);
            AddValue("Right Eyebrow", "takeoff landing config indicator", "", "True if configuration is inncorrect for takeoff or landing.", BindingValueUnits.Boolean);
            AddValue("Right Eyebrow", "engine indicator", "Right eyebrow engine indicator.", "True if lit.", BindingValueUnits.Boolean);

            // Left eyebrow Bits
            AddValue("Left Eyebrow", "master caution indicator", "", "True if master caution light is turned on.", BindingValueUnits.Boolean);
            AddValue("Left Eyebrow", "tf-fail indicator", "", "True if TF switch in MAN TF positoin.", BindingValueUnits.Boolean);

            // AOA Indexer Bits
            AddValue("AOA Indexer", "above indicator", "AOA Indexer above indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("AOA Indexer", "on indicator", "AOA Indexer on indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("AOA Indexer", "below indicator", "AOA Indexer below indicator.", "True if lit.", BindingValueUnits.Boolean);
            
            // Refuel INdexer Bits
            AddValue("Refuel Indexer", "ready indicator", "Refuel Indexer ready indciator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Refuel Indexer", "air/nws indicator", "Refuel Indexer Air/NWS indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Refuel Indexer", "disconnect indicator", "Refuel Indexer disconnect indicator.", "True if lit.", BindingValueUnits.Boolean);

            // Caution bits
            AddValue("Caution", "equip hot indicator", "Equip hot indicator on caution panel", "True if lit", BindingValueUnits.Boolean);
            AddValue("Caution", "cadc indicator", "CADC indicator lamp on the caution panel.", "True if lit", BindingValueUnits.Boolean);
            AddValue("Caution", "atf not engaged", "ATF NOT ENGAGED Caution Light", "True if lit", BindingValueUnits.Boolean);
            AddValue("Caution", "Inlet Icing indicator", "Inlet Icing indicator lamp on the caution panel.", "True if lit", BindingValueUnits.Boolean);
            AddValue("Caution", "stores config indicator", "Caution panel stores config indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "flight control system indicator", "Caution panel flight control indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "leading edge flaps indicator", "Caution panel leading edge flaps indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "engine fault indticator", "Caution panel engine fault indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "overheat indicator", "Caution panel overheat indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "low fuel indicator", "Caution panel low fuel indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "avionics indicator", "Caution panel avionics indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "radar altimeter indicator", "Caution panel radar altimeter indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "iff indicator", "Caution iff indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "ecm indicator", "Caution ecm indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "hook indicator", "Caution panel hook indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "nws fail indicator", "Caution panel nose wheel steering fail indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "cabin pressure indicator", "Caution panel cabin pressure indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "forward fuel low indicator", "Caution panel forward fuel low indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "aft fuel low indicator", "Caution panel aft fuel low indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "second engine compressor indicator", "Caution panel second engine compressor indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "oxygen low indicator", "Caution panel oxygen low indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "probe heat indicator", "Caution panel probe heat indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "seat arm indicator", "Caution panel seat not armed indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "backup fuel control indicator", "Caution panel backup fuel control indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "fuel oil hot indicator", "Caution panel oil hot indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "anti skid indicator", "Caution panel anti skid indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "electric bus fail indicator", "Caution panel electric bus fail indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Caution", "lef fault indicator", "Caution panel leading edge fault indicator.", "True if lit.", BindingValueUnits.Boolean);
            

            // Test Panel Bits
            AddValue("Test Panel", "FLCS channel lamp A", "FLCS channel lamp A on test panel (abcd)", "True if lit", BindingValueUnits.Boolean);
            AddValue("Test Panel", "FLCS channel lamp B", "FLCS channel lamp B on test panel (abcd)", "True if lit", BindingValueUnits.Boolean);
            AddValue("Test Panel", "FLCS channel lamp C", "FLCS channel lamp C on test panel (abcd)", "True if lit", BindingValueUnits.Boolean);
            AddValue("Test Panel", "FLCS channel lamp D", "FLCS channel lamp D on test panel (abcd)", "True if lit", BindingValueUnits.Boolean);
            AddValue("Test Panel", "FLCS channel lamps", "FLCS channel lamps on test panel (abcd)", "True if lit", BindingValueUnits.Boolean);
            
            // Auto Pilot Bits
            AddValue("Autopilot", "on indicator", "Indicates whether the autopilot is on.", "True if on", BindingValueUnits.Boolean);

            // Flight Control Panel Bits
            AddValue("Flight Control", "run light", "Run light on the flight control panel indicating bit is running.", "True if lit", BindingValueUnits.Boolean);
            AddValue("Flight Control", "fail light", "Fail light on the flight control panel indicating bit failure.", "True if lit", BindingValueUnits.Boolean);

            // HSI Bits
            AddValue("HSI", "to flag", "HSI to flag indicating we are heading to the beacon.", "True if displayed and aircraft is heading towards beacon.", BindingValueUnits.Boolean);
            AddValue("HSI", "from flag", "HSI from flag indicating we are heading away from the beacon.", "True if displayed and aircraft is moving away from the beacon.", BindingValueUnits.Boolean);
            AddValue("HSI", "ils warning flag", "HSI ils warning flag indicating if course steering data is available.", "True if displayed and data is not accurate or available.", BindingValueUnits.Boolean);
            AddValue("HSI", "dme flag", "HSI dem flag indicating distance to beacon is not available.", "True if displayed.", BindingValueUnits.Boolean);
            AddValue("HSI", "off flag", "HSI off flag indicating hsi is not recieving data.", "True if displayed and HSI data is not available.", BindingValueUnits.Boolean);
            AddValue("HSI", "init flag", "HSI init flag", "True if displayed.", BindingValueUnits.Boolean);
            AddValue("HSI", "desired course calculated", "Internally calculated value", "360 - current heading + desired course", BindingValueUnits.Degrees);
            AddValue("HSI", "desired heading calculated", "Internally calculated value", "360 - current heading + desired heading", BindingValueUnits.Degrees);
            AddValue("HSI", "bearing to beacon calculated", "Internally calculated value", "360 - current heading + bearing to beacon", BindingValueUnits.Degrees);
            AddValue("HSI", "Outer marker indicator", "Outer marker indicator on HSI", "True if lit", BindingValueUnits.Boolean);
            AddValue("HSI", "Middle marker indicator", "Middle marker indicator on HSI", "True if lit", BindingValueUnits.Boolean);
            AddValue("HSI", "nav mode", "Nav mode currently selected for the HSI/eHSI", "", BindingValueUnits.Numeric);
            AddValue("HSI", "bearing to beacon", "Compass heading in degrees to the currently selected beacon.", "", BindingValueUnits.Degrees);
            AddValue("HSI", "desired course", "Currently selected desired course in degrees.", "", BindingValueUnits.Degrees);
            AddValue("HSI", "current heading", "Current heading of the aircraft.", "", BindingValueUnits.Degrees);
            AddValue("HSI", "distance to beacon", "Distance to the currently selected beacon.", "", BindingValueUnits.NauticalMiles);
            AddValue("HSI", "desired heading", "Currently selected desired heading.", "", BindingValueUnits.Degrees);
            AddValue("HSI", "course deviation", "Current location of course deviation bar.", "(-1 full left to 1 full right)", BindingValueUnits.Numeric);

            // ADI Bits
            AddValue("ADI", "off flag", "ADI off flag indicating ADI is powered off or not recieving data.", "True if displayed and ADI is off or not recieving data.", BindingValueUnits.Boolean);
            AddValue("ADI", "aux flag", "ADI aux flag", "True if displayed.", BindingValueUnits.Boolean);
            AddValue("ADI", "gs flag", "ADI gs flag", "True if displayed.", BindingValueUnits.Boolean);
            AddValue("ADI", "loc flag", "ADI loc flag", "True if displayed.", BindingValueUnits.Boolean);
            AddValue("ADI", "pitch", "Pitch of the aircraft", "", BindingValueUnits.Radians);
            AddValue("ADI", "roll", "Roll of the aircraft", "", BindingValueUnits.Radians);
            AddValue("ADI", "ils horizontal", "Position of horizontal ils bar.", "(-1 full left, 1 full right)", BindingValueUnits.Numeric);
            AddValue("ADI", "ils vertical", "Position of vertical ils bar.", "(-1 highest, 1 lowest)", BindingValueUnits.Numeric);
            AddValue("ADI", "ils vertical to flight path", "Position of vertical ils bar with AOA correction.", "(-1 highest, 1 lowest)", BindingValueUnits.Numeric);
            AddValue("ADI", "sideslip angle", "Angle of sideslip of the aircraft.", "", BindingValueUnits.Degrees);

            // Backup ADI
            AddValue("Backup ADI", "off flag", "Backup ADI off flag", "True if displayed.", BindingValueUnits.Boolean);

            // VVI Bits
            AddValue("VVI", "off flag", "VVI Off flag indicating VVI is turned off or not receiving data.", "True if displayed.", BindingValueUnits.Boolean);
            AddValue("VVI", "vertical velocity", "Current vertical velocity of the aircraft.", "", BindingValueUnits.FeetPerSecond);
            
            // AOA Bits
            AddValue("AOA", "off flag", "AOA Off flag indicating AOA is turned off or not receiving data.", "True if displayed.", BindingValueUnits.Boolean);
            AddValue("AOA", "angle of attack", "Current angle of attack of the aircraft.", "", BindingValueUnits.Degrees);

            // Tacan Bits
            AddValue("Tacan", "ufc tacan band", "Tacan band set with the UFC.", "1 = X, 2 = Y", BindingValueUnits.Numeric);
            AddValue("Tacan", "aux tacan band", "Tacan band set with the AUX COM panel.", "1 = X, 2 = Y", BindingValueUnits.Numeric);
            AddValue("Tacan", "ufc tacan mode", "Tacan mode set with the UFC.", "1 = TR, 2 = AA", BindingValueUnits.Numeric);
            AddValue("Tacan", "aux tacan mode", "Tacan mode set with the AUX COM panel.", "1 = TR, 2 = AA", BindingValueUnits.Numeric);
            AddValue("Tacan", "ufc tacan chan", "Tacan channel set with the UFC.", "", BindingValueUnits.Numeric);
            AddValue("Tacan", "aux tacan chan", "Tacan channel set with the AUX COM panel.", "", BindingValueUnits.Numeric);

            // AVTR Bits
            AddValue("AVTR", "avtr indicator", "Indicates whether the acmi is recording", "True if lit", BindingValueUnits.Boolean);

            // Threat Warning Prime Bits
            AddValue("Threat Warning Prime", "systest indicator", "Threat warning prime systest indicator", "True if lit", BindingValueUnits.Boolean);
            AddValue("Threat Warning Prime", "handoff indicator", "Threat warning prime handoff dot indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Threat Warning Prime", "launch indicator", "Threat warning prime launch indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Threat Warning Prime", "prioirty mode indicator", "Threat warning prime priority mode indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Threat Warning Prime", "open mode indicator", "Threat warning prime open mode indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Threat Warning Prime", "naval indicator", "Threat warning prime naval indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Threat Warning Prime", "unknown mode indicator", "Threat warning prime unkown mode indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Threat Warning Prime", "target step indicator", "Threat warning prime target step indicator.", "True if lit.", BindingValueUnits.Boolean);
            
            // AUX Threat Warning
            AddValue("Aux Threat Warning", "search indicator", "Aux threat warning search indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Aux Threat Warning", "activity indicator", "Aux threat warning activity indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Aux Threat Warning", "low altitude indicator", "Aux threat warning low altitude indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Aux Threat Warning", "power indicator", "Aux threat warning system power indicator.", "True if lit.", BindingValueUnits.Boolean);

            // UHF Backup Radio Bits
            AddValue("UHF", "Backup channel", "Current Backup UHF channel", "", BindingValueUnits.Numeric);
            AddValue("UHF", "Backup frequency", "Current Backup UHF frequency", "", BindingValueUnits.Numeric);
            AddValue("UHF", "Backup frequency digit 1", "Current Backup UHF frequency digit 1", "", BindingValueUnits.Numeric);
            AddValue("UHF", "Backup frequency digit 2", "Current Backup UHF frequency digit 2", "", BindingValueUnits.Numeric);
            AddValue("UHF", "Backup frequency digit 3", "Current Backup UHF frequency digit 3", "", BindingValueUnits.Numeric);
            AddValue("UHF", "Backup frequency digit 4", "Current Backup UHF frequency digit 4", "", BindingValueUnits.Numeric);
            AddValue("UHF", "Backup frequency digit 5,6", "Current Backup UHF frequency digit 5,6", "", BindingValueUnits.Numeric);

            // Hydraulic Bits
            AddValue("Hydraulic", "Pressure A", "Current hydraulic pressure a", "", BindingValueUnits.PoundsPerSquareInch);
            AddValue("Hydraulic", "Pressure B", "Current hydraulic pressure b", "", BindingValueUnits.PoundsPerSquareInch);

            // Time Bits
            AddValue("Time", "Time", "Current tine in seconds", "(max 60 * 60 * 24)", BindingValueUnits.Seconds);

            //Power Bits
            AddValue("POWER", "bus power battery", "at least the battery bus is powered", "True if powered", BindingValueUnits.Boolean);
            AddValue("POWER", "bus power emergency", "at least the emergency bus is powered", "True if powered", BindingValueUnits.Boolean);
            AddValue("POWER", "bus power essential", "at least the essential bus is powered", "True if powered", BindingValueUnits.Boolean);
            AddValue("POWER", "bus power non essential", "at least the non-essential bus is powered", "True if powered", BindingValueUnits.Boolean);
            AddValue("POWER", "main generator", "main generator is online", "True if online", BindingValueUnits.Boolean);
            AddValue("POWER", "standby generator", "standby generator is online", "True if online", BindingValueUnits.Boolean);
            AddValue("POWER", "Jetfuel starter", "JFS is running, can be used for magswitch", "True if running", BindingValueUnits.Boolean);
            
            // AV8B Specific Bits
            AddValue("AV8B", "vtol exhaust angle position", "angle of vtol exhaust", "", BindingValueUnits.Degrees);
            
            // DED String Bits
            AddValue("DED", "DED Line 1", "Data entry display line 1", "", BindingValueUnits.Text);
            AddValue("DED", "DED Line 2", "Data entry display line 2", "", BindingValueUnits.Text);
            AddValue("DED", "DED Line 3", "Data entry display line 3", "", BindingValueUnits.Text);
            AddValue("DED", "DED Line 4", "Data entry display line 4", "", BindingValueUnits.Text);
            AddValue("DED", "DED Line 5", "Data entry display line 5", "", BindingValueUnits.Text);

            //PFL String Bits
            AddValue("PFL", "PFL Line 1", "Pilot Fault Lights line 1", "", BindingValueUnits.Text);
            AddValue("PFL", "PFL Line 2", "Pilot Fault Lights line 2", "", BindingValueUnits.Text);
            AddValue("PFL", "PFL Line 3", "Pilot Fault Lights line 3", "", BindingValueUnits.Text);
            AddValue("PFL", "PFL Line 4", "Pilot Fault Lights line 4", "", BindingValueUnits.Text);
            AddValue("PFL", "PFL Line 5", "Pilot Fault Lights line 5", "", BindingValueUnits.Text);

            // Ownship Bits
            AddValue("Ownship", "latitude", "Ownship latitude", "in degrees (as known by avionics)", BindingValueUnits.Degrees);
            AddValue("Ownship", "longitude", "Ownship longitude", "in degrees (as known by avionics)", BindingValueUnits.Degrees);
            AddValue("Ownship", "x", "Ownship North (Ft)","", BindingValueUnits.Feet);
            AddValue("Ownship", "y", "Ownship East (Ft)", "", BindingValueUnits.Feet);
            AddValue("Ownship", "ground speed", "Ownship ground speed", "in feet per second", BindingValueUnits.FeetPerSecond);
            AddValue("Ownship", "distance from bullseye", "Ownship distance from bullseye", "", BindingValueUnits.Feet);
            AddValue("Ownship", "heading from bullseye", "Ownship heading from bullseye", "", BindingValueUnits.Degrees);
            AddValue("Ownship", "heading to bullseye", "Ownship heading to bullseye", "", BindingValueUnits.Degrees);
            AddValue("Ownship", "orientation to bullseye", "Ownship orientation to bullseye", "", BindingValueUnits.Degrees);
            AddValue("Ownship", "deltaX from bulls", "Delta from bullseye North (Ft)", "", BindingValueUnits.Feet);
            AddValue("Ownship", "deltaY from bulls", "Delta from bullseye East (Ft)", "", BindingValueUnits.Feet);

            //IFF Digits Bits
            AddValue("IFF", "backup mode 1 digit 1", "AUX COMM: Mode 1 left digit", "", BindingValueUnits.Numeric);
            AddValue("IFF", "backup mode 1 digit 2", "AUX COMM: Mode 1 right digit", "", BindingValueUnits.Numeric);
            AddValue("IFF", "backup mode 3 digit 1", "AUX COMM: Mode 3 left digit", "", BindingValueUnits.Numeric);
            AddValue("IFF", "backup mode 3 digit 2", "AUX COMM: Mode 3 right digit", "", BindingValueUnits.Numeric);

            // Lighting
            AddValue("Lighting", "instrument backlight", "Instrument panel backlight brightness.", "0 = Off, 1 = Dim, 2 = Bright", BindingValueUnits.Numeric);

            // Misc Bits
            AddValue("Misc", "tfs stanby indicator", "Misc panel Terrain Following(TFS) standby indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Misc", "tfs engaged indicator", "Misc panel Terrain Following(TFS) engaged indicator.", "True if lit.", BindingValueUnits.Boolean);

            // EPU Bits
            AddValue("EPU", "on indicator", "EPU on indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("EPU", "hydrazine indicator", "EPU hydrazine indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("EPU", "air indicator", "EPU air indicator.", "True if lit.", BindingValueUnits.Boolean);

            // JFS Bits
            AddValue("JFS", "run indicator", "JFS run indicator.", "True if lit.", BindingValueUnits.Boolean);


            // Electronic Bits
            AddValue("Electronic", "flcs pmg indicator", "Electronic panel flcs pmg indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Electronic", "main gen indicator", "Electronic panel main generator indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Electronic", "standby generator indicator", "Electronic panel standby generator indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Electronic", "epu gen indicator", "Electronic panel epu gen indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Electronic", "epu pmg indicator", "Electronic panel epu pmg indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Electronic", "to flcs indicator", "Electronic panel to flcs indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Electronic", "flcs rly indicator", "Electronic panel flcs rly indicator.", "True if lit.", BindingValueUnits.Boolean);
            AddValue("Electronic", "bat fail indicator", "Electronic panel battery fail indicator.", "True if lit.", BindingValueUnits.Boolean);

            // Falcon RunTime Data
            AddValue("Runtime", "Current Theater", "Name of the Current Theater", "", BindingValueUnits.Text);
            AddValue("Runtime", "Flying", "Player flying state", "True if in 3D.", BindingValueUnits.Boolean);
            AddValue("Runtime", "Flight Start Mode", "Flight initial start mode.", "1 = Ramp Start, 2 = Hot Start, 3 = Air Start", BindingValueUnits.Numeric);
            AddValue("Runtime", "RTT Enabled", "RTT texture extraction state", "True if RTT enabled.", BindingValueUnits.Boolean);
            AddValue("Runtime", "Aircraft Name", "The name of the aircraft", "Example: F-16B-15 or F/A-18D", BindingValueUnits.Text);
            AddValue("Runtime", "Aircraft Nomenclature", "The nomenclature of the aircraft", "Example: F16 or F18", BindingValueUnits.Text);
            AddValue("Runtime", "Aircraft IFF Panel", "Type of panel IFF or AUX COMM", "True if IFF, False if AUX COMM. ", BindingValueUnits.Boolean);
            AddValue("Runtime", "Aircraft ECM Panel", "Type of panel ECM or IDIAS", "True if ECM, False if IDIAS. ", BindingValueUnits.Boolean);
        }

        internal override void InitData()
        {
            _sharedMemory = new SharedMemory("FalconSharedMemoryArea");
            _sharedMemory.Open();

            _sharedMemory2 = new SharedMemory("FalconSharedMemoryArea2");
            _sharedMemory2.Open();

            _sharedMemoryStringArea = new SharedMemory("FalconSharedMemoryAreaString");
            _sharedMemoryStringArea.Open();
        }

        internal override void PollUserInterfaceData()
        {
            SetValue("Runtime", "Current Theater", new BindingValue(FalconInterface.CurrentTheater));

            if (_sharedMemory != null && _sharedMemory.IsDataAvailable)
            {
                FlightData lastFlightData = (FlightData)_sharedMemory.MarshalTo(typeof(FlightData));

                SetValue("Runtime", "Flying", new BindingValue(lastFlightData.hsiBits.HasFlag(HsiBits.Flying)));
            }

            SetValue("Runtime", "RTT Enabled", new BindingValue(FalconInterface.Rtt.Enabled));
        }

        internal override void PollFlightStartData()
        {
            if (_sharedMemory != null && _sharedMemory.IsDataAvailable)
            {
                FlightData lastFlightData = (FlightData)_sharedMemory.MarshalTo(typeof(FlightData));

                if (lastFlightData.hsiBits.HasFlag(HsiBits.Flying))
                {
                    if (lastFlightData.lightBits3.HasFlag(BMSLightBits3.OnGround))
                    {
                        if (lastFlightData.rpm > 0d)
                        {
                            // Hot Start mode.
                            SetValue("Runtime", "Flight Start Mode", new BindingValue(2d));
                        }
                        else
                        {
                            // Ramp Start mode.
                            SetValue("Runtime", "Flight Start Mode", new BindingValue(1d));
                        }
                    }
                    else
                    {
                        // Air Start mode.
                        SetValue("Runtime", "Flight Start Mode", new BindingValue(3d));
                    }
                }
                else
                {
                    // In user interface so set to zero.
                    SetValue("Runtime", "Flight Start Mode", new BindingValue(0d));
                }
            }
        }

        internal override void PollFlightData()
        {
            if (_sharedMemory != null && _sharedMemory.IsDataAvailable)
            {
                _lastFlightData = (FlightData)_sharedMemory.MarshalTo(typeof(FlightData));

                float altitidue = _lastFlightData.z;
                if (_lastFlightData.z < 0)
                {
                    altitidue = 99999.99f - _lastFlightData.z;
                }

                SetValue("Altimeter", "altitidue", new BindingValue(altitidue));
                SetValue("ADI", "pitch", new BindingValue(_lastFlightData.pitch));
                SetValue("ADI", "roll", new BindingValue(_lastFlightData.roll));
                SetValue("ADI", "ils horizontal", new BindingValue((_lastFlightData.AdiIlsHorPos / 2.5f) - 1f));
                SetValue("ADI", "ils vertical", new BindingValue((_lastFlightData.AdiIlsVerPos * 2f) - 1f));
                SetValue("ADI", "sideslip angle", new BindingValue(_lastFlightData.beta));
                SetValue("HSI", "bearing to beacon", new BindingValue(_lastFlightData.bearingToBeacon));
                SetValue("HSI", "current heading", new BindingValue(_lastFlightData.currentHeading));
                SetValue("HSI", "desired course", new BindingValue(_lastFlightData.desiredCourse));
                SetValue("HSI", "desired heading", new BindingValue(_lastFlightData.desiredHeading));
                SetValue("HSI", "desired course calculated", new BindingValue(ClampDegrees(360 - _lastFlightData.currentHeading) + _lastFlightData.desiredCourse));
                SetValue("HSI", "desired heading calculated", new BindingValue(ClampDegrees(360 - _lastFlightData.currentHeading) + _lastFlightData.desiredHeading));
                SetValue("HSI", "bearing to beacon calculated", new BindingValue(ClampDegrees(360 - _lastFlightData.currentHeading) + _lastFlightData.bearingToBeacon));
                SetValue("HSI", "course deviation", new BindingValue(CalculateHSICourseDeviation(_lastFlightData.deviationLimit, _lastFlightData.courseDeviation)));
                SetValue("HSI", "distance to beacon", new BindingValue(_lastFlightData.distanceToBeacon));

                SetValue("VVI", "vertical velocity", new BindingValue(_lastFlightData.zDot));
                SetValue("AOA", "angle of attack", new BindingValue(_lastFlightData.alpha));
                
                SetValue("IAS", "mach", new BindingValue(_lastFlightData.mach));
                SetValue("IAS", "indicated air speed", new BindingValue(_lastFlightData.kias));
                SetValue("IAS", "true air speed", new BindingValue(_lastFlightData.vt));

                SetValue("General", "Gs", new BindingValue(_lastFlightData.gs));
                SetValue("General", "speed brake position", new BindingValue(_lastFlightData.speedBrake));
                SetValue("General", "speed brake indicator", new BindingValue(_lastFlightData.speedBrake > 0d));
                
                
                SetValue("Fuel", "internal fuel", new BindingValue(_lastFlightData.internalFuel));
                SetValue("Fuel", "external fuel", new BindingValue(_lastFlightData.externalFuel));
                
                SetValue("Engine", "fuel flow", new BindingValue(_lastFlightData.fuelFlow));
                SetValue("Engine", "rpm", new BindingValue(_lastFlightData.rpm));
                SetValue("Engine", "ftit", new BindingValue(_lastFlightData.ftit * 100));
                SetValue("Engine", "nozzle position", new BindingValue(_lastFlightData.nozzlePos * 100));
                SetValue("Engine", "oil pressure", new BindingValue(_lastFlightData.oilPressure));

                SetValue("Landging Gear", "position", new BindingValue(_lastFlightData.gearPos != 0d));                

                SetValue("EPU", "fuel", new BindingValue(_lastFlightData.epuFuel));

                SetValue("CMDS", "chaff remaining", new BindingValue(_lastFlightData.ChaffCount));
                SetValue("CMDS", "flares remaining", new BindingValue(_lastFlightData.FlareCount));

                SetValue("Trim", "roll trim", new BindingValue(_lastFlightData.TrimRoll));
                SetValue("Trim", "pitch trim", new BindingValue(_lastFlightData.TrimPitch));
                SetValue("Trim", "yaw trim", new BindingValue(_lastFlightData.TrimYaw));

                SetValue("Fuel", "fwd fuel", new BindingValue(_lastFlightData.fwd));
                SetValue("Fuel", "aft fuel", new BindingValue(_lastFlightData.aft));
                SetValue("Fuel", "total fuel", new BindingValue(_lastFlightData.total));

                SetValue("Tacan", "ufc tacan chan", new BindingValue(_lastFlightData.UFCTChan));
                SetValue("Tacan", "aux tacan chan", new BindingValue(_lastFlightData.AUXTChan));

                ProcessContacts(_lastFlightData);

                //DED
                SetValue("DED", "DED Line 1", new BindingValue(DecodeUserInterfaceText(_lastFlightData.DED, 0, 26)));
                SetValue("DED", "DED Line 2", new BindingValue(DecodeUserInterfaceText(_lastFlightData.DED, 26, 26)));
                SetValue("DED", "DED Line 3", new BindingValue(DecodeUserInterfaceText(_lastFlightData.DED, 26 * 2, 26)));
                SetValue("DED", "DED Line 4", new BindingValue(DecodeUserInterfaceText(_lastFlightData.DED, 26 * 3, 26)));
                SetValue("DED", "DED Line 5", new BindingValue(DecodeUserInterfaceText(_lastFlightData.DED, 26 * 4, 26)));

                //PFL
                SetValue("PFL", "PFL Line 1", new BindingValue(DecodeUserInterfaceText(_lastFlightData.PFL, 0, 26)));
                SetValue("PFL", "PFL Line 2", new BindingValue(DecodeUserInterfaceText(_lastFlightData.PFL, 26, 26)));
                SetValue("PFL", "PFL Line 3", new BindingValue(DecodeUserInterfaceText(_lastFlightData.PFL, 26 * 2, 26)));
                SetValue("PFL", "PFL Line 4", new BindingValue(DecodeUserInterfaceText(_lastFlightData.PFL, 26 * 3, 26)));
                SetValue("PFL", "PFL Line 5", new BindingValue(DecodeUserInterfaceText(_lastFlightData.PFL, 26 * 4, 26)));

                //Ownship
                SetValue("Ownship", "x", new BindingValue(_lastFlightData.x));
                SetValue("Ownship", "y", new BindingValue(_lastFlightData.y));
                SetValue("Ownship", "longitude", new BindingValue(_lastFlightData2.longitude));
                SetValue("Ownship", "latitude", new BindingValue(_lastFlightData2.latitude));
                SetValue("Ownship", "ground speed", new BindingValue(GroundSpeedInFeetPerSecond(_lastFlightData.xDot, _lastFlightData.yDot)));

                //ADI ILS with AOA consideration value
                SetValue("ADI", "ils vertical to flight path", new BindingValue(((_lastFlightData.AdiIlsVerPos * 2f) - 1f) - ClampAOA(_lastFlightData.alpha)));
            }
            if (_sharedMemory2 != null && _sharedMemory2.IsDataAvailable)
            {
                _lastFlightData2 = (FlightData2)_sharedMemory2.MarshalTo(typeof(FlightData2));
                _stringAreaTime = _lastFlightData2.StringAreaTime;
                _stringAreaSize = _lastFlightData2.StringAreaSize;

                SetValue("Altimeter", "indicated altitude", new BindingValue(-_lastFlightData2.aauz));
                SetValue("Altimeter", "barimetric pressure", new BindingValue(_lastFlightData2.AltCalReading));

                SetValue("HSI", "nav mode", new BindingValue((int)_lastFlightData2.navMode));
                SetValue("Tacan", "ufc tacan band", new BindingValue(_lastFlightData2.tacanInfo[(int)TacanSources.UFC].HasFlag(TacanBits.band) ? 1 : 2));
                SetValue("Tacan", "ufc tacan mode", new BindingValue(_lastFlightData2.tacanInfo[(int)TacanSources.UFC].HasFlag(TacanBits.mode) ? 2 : 1));
                SetValue("Tacan", "aux tacan band", new BindingValue(_lastFlightData2.tacanInfo[(int)TacanSources.AUX].HasFlag(TacanBits.band) ? 1 : 2));
                SetValue("Tacan", "aux tacan mode", new BindingValue(_lastFlightData2.tacanInfo[(int)TacanSources.AUX].HasFlag(TacanBits.mode) ? 2 : 1));

                SetValue("Engine", "nozzle 2 position", new BindingValue(_lastFlightData2.nozzlePos2 * 100));
                SetValue("Engine", "rpm2", new BindingValue(_lastFlightData2.rpm2));
                SetValue("Engine", "ftit2", new BindingValue(_lastFlightData2.ftit2 * 100));
                SetValue("Engine", "oil pressure 2", new BindingValue(_lastFlightData2.oilPressure2));
                SetValue("Engine", "fuel flow 2", new BindingValue(_lastFlightData2.fuelFlow2));
                
                ProcessAltBits(_lastFlightData2.altBits);
                ProcessPowerBits(_lastFlightData2.powerBits);

                SetValue("CMDS", "CMDS Mode", new BindingValue((int)_lastFlightData2.cmdsMode));
                SetValue("UHF", "Backup channel", new BindingValue(_lastFlightData2.BupUhfPreset));
                SetValue("UHF", "Backup frequency", new BindingValue(_lastFlightData2.BupUhfFreq));
                SetValue("UHF", "Backup frequency digit 1", new BindingValue(_lastFlightData2.BupUhfFreq / 100000 % 10));
                SetValue("UHF", "Backup frequency digit 2", new BindingValue(_lastFlightData2.BupUhfFreq / 10000 % 10));
                SetValue("UHF", "Backup frequency digit 3", new BindingValue(_lastFlightData2.BupUhfFreq / 1000 % 10));
                SetValue("UHF", "Backup frequency digit 4", new BindingValue(_lastFlightData2.BupUhfFreq / 100 % 10));
                SetValue("UHF", "Backup frequency digit 5,6", new BindingValue(_lastFlightData2.BupUhfFreq % 100));
                SetValue("Altitude", "Cabin Altitude", new BindingValue(_lastFlightData2.cabinAlt));
                SetValue("Hydraulic", "Pressure A", new BindingValue(_lastFlightData2.hydPressureA));
                SetValue("Hydraulic", "Pressure B", new BindingValue(_lastFlightData2.hydPressureB));
                SetValue("Time", "Time", new BindingValue(_lastFlightData2.currentTime));
                SetValue("Lighting", "instrument backlight", new BindingValue((int)_lastFlightData2.instrLight));

                //Bullseye                
                ProcessOwnshipFromBullseye(_lastFlightData.x, _lastFlightData.y, _lastFlightData2.bullseyeX, _lastFlightData2.bullseyeY, _lastFlightData.currentHeading);

                //AV8B Values
                SetValue("AV8B", "vtol exhaust angle position", new BindingValue(_lastFlightData2.vtolPos));

                //IFF Data
                SetValue("IFF", "backup mode 1 digit 1", new BindingValue(_lastFlightData2.iffBackupMode1Digit1));
                SetValue("IFF", "backup mode 1 digit 2", new BindingValue(_lastFlightData2.iffBackupMode1Digit2));
                SetValue("IFF", "backup mode 3 digit 1", new BindingValue(_lastFlightData2.iffBackupMode3ADigit1));
                SetValue("IFF", "backup mode 3 digit 2", new BindingValue(_lastFlightData2.iffBackupMode3ADigit2));

                ProcessRwrInfo(_lastFlightData2);
                ProcessMiscBits(_lastFlightData2.miscBits, _lastFlightData2.RALT);
                ProcessHsiBits(_lastFlightData.hsiBits, _lastFlightData2.blinkBits);
                ProcessLightBits(_lastFlightData.lightBits);
                ProcessLightBits2(_lastFlightData.lightBits2, _lastFlightData2.blinkBits, _lastFlightData2.currentTime);
                ProcessLightBits3(_lastFlightData.lightBits3);
            }
            if (_sharedMemoryStringArea != null && _sharedMemoryStringArea.IsDataAvailable)
            {
                if (_stringAreaTime != _lastStringAreaTime)
                {
                    _lastStringAreaTime = _stringAreaTime;
                    if(_stringAreaSize != 0)
                    {
                        _stringDataUpdated = true;
                        StringData stringData = new StringData();
                        var _rawStringData = new byte[_stringAreaSize];
                        Marshal.Copy(_sharedMemoryStringArea.GetPointer(), _rawStringData, 0, (int)_stringAreaSize);
                        _navPoints = stringData.GetNavPoints(_rawStringData);

                        _acName = stringData.GetValueForStrId(_rawStringData, StringIdentifier.AcName);
                        _acNCTR = stringData.GetValueForStrId(_rawStringData, StringIdentifier.AcNCTR);
                    }
                }
                else
                {
                    _stringDataUpdated = false;
                }
            }

            string AN6 = _acName.Substring(0, Math.Min(_acName.Length, 6));
            string AN8 = _acName.Substring(0, Math.Min(_acName.Length, 8));
            string AN9 = _acName.Substring(0, Math.Min(_acName.Length, 9));

            if (AN6 == "F-16AM" || AN8 == "F-16C-52" || AN8 == "F-16D-52" || AN8 == "F-16I-52" || AN9 == "F-16CM-40" ||
                AN9 == "F-16CM-42" || AN9 == "F-16CM-50" || AN9 == "F-16CM-52" || AN9 == "F-16DM-40" || AN9 == "F-16DM-52")
            {
                _panelTypeIFF = true;
            }
            else if (AN8 == "F-16A-15" || AN8 == "F-16B-15" || AN8 == "F-16C-25" || AN8 == "F-16C-30" || AN8 == "F-16C-32" ||
                     AN8 == "F-16C-40" || AN8 == "F-16C-50" || AN9 == "F-16DG-30" || AN9 == "F-16DG-40")
            {
                _panelTypeIFF = false;
            }
            else
            {
                _panelTypeIFF = true;
            }

            //Runtime bindings
            SetValue("Runtime", "Current Theater", new BindingValue(FalconInterface.CurrentTheater));
            SetValue("Runtime", "Aircraft Name", new BindingValue(_acName));
            SetValue("Runtime", "Aircraft Nomenclature", new BindingValue(_acNCTR));
            SetValue("Runtime", "Aircraft IFF Panel", new BindingValue(_panelTypeIFF));
            SetValue("Runtime", "Aircraft ECM Panel", new BindingValue(!_acName.Contains("HAF")));
        }

        internal float ClampAOA(float alpha)
        {
            float correctedValue;
            const float SCALE_FACTOR = 0.0025f;
          if (alpha > 20)
            {
                correctedValue = 20f;
            }
            else if (alpha < -20)
            {
                correctedValue = -20f;
            }
            else
            {
                correctedValue = alpha;
            }
            return correctedValue * SCALE_FACTOR;
        }

        internal float CalculateHSICourseDeviation(float deviationLimit, float courseDeviation)
        {
            float deviation = 0;
            float finalDeviation;

            if (Math.Floor(courseDeviation) <= 179)
            {
                deviation = (courseDeviation % 180) / deviationLimit;
            }
            else if (Math.Floor(courseDeviation) >= 180)
            {
                deviation = ((180 - courseDeviation) % 180) / deviationLimit;
            }

            //Apply limits to final deviation so we get variatons between 1.1 and -1.1
            if (deviation > -1.1 & deviation < 1.1)
            {
                finalDeviation = deviation;
            }
            else if (deviation > 1.1 & deviation < 17)
            {
                finalDeviation = 1;
            }
            else if (deviation > 17 & deviation < 18.1)
            {
                finalDeviation = -deviation + 18;
            }
            else if (deviation < -16.9)
            {
                finalDeviation = -deviation - 18;
            }
            else
            {
                finalDeviation = -1;
            }
            return finalDeviation;
        }

        private void ProcessMiscBits(MiscBits miscBits, float rALT)
        {
            var rAltString = "";
            if (miscBits.HasFlag(MiscBits.RALT_Valid))
            {
                rAltString = String.Format("{0:0}", RoundToNearestTen((int)Math.Abs(rALT)));
                
            }
            SetValue("Altimeter", "radar alt", new BindingValue(rAltString));

            //BMS 4.35 additions
            SetValue("Test Panel", "FLCS channel lamp A", new BindingValue(miscBits.HasFlag(MiscBits.Flcs_Flcc_A)));
            SetValue("Test Panel", "FLCS channel lamp B", new BindingValue(miscBits.HasFlag(MiscBits.Flcs_Flcc_A)));
            SetValue("Test Panel", "FLCS channel lamp C", new BindingValue(miscBits.HasFlag(MiscBits.Flcs_Flcc_A)));
            SetValue("Test Panel", "FLCS channel lamp D", new BindingValue(miscBits.HasFlag(MiscBits.Flcs_Flcc_A)));
            SetValue("Gear Handle", "gear handle solenoid", new BindingValue(miscBits.HasFlag(MiscBits.SolenoidStatus)));
        }

        private object RoundToNearestTen(int num)
        {
            int rem = num % 10;
            return rem >= 5 ? (num - rem + 10) : (num - rem);
        }

        internal float GroundSpeedInFeetPerSecond(float xDot, float yDot)
        {
            const float FPS_PER_KNOT = 1.68780986f;
            //return (int)((float)Math.Sqrt((xDot * xDot) + (yDot * yDot)) / FPS_PER_KNOT);
            return (float)Math.Sqrt((xDot * xDot) + (yDot * yDot)) / FPS_PER_KNOT;
        }

        internal string DecodeUserInterfaceText(byte[] buffer,int offset,int length)
        {
            var sanitized = new StringBuilder();
            ASCIIEncoding ascii = new ASCIIEncoding();

            // Decode the bytes
            String decoded = ascii.GetString(buffer, offset, length);

            foreach (var value in decoded)
            {
                var thisChar = value;
                if (value == 0x01) thisChar = '\u2195';
                if (value == 0x02) thisChar = '*';
                if (value == 0x5E) thisChar = '\u00B0'; //degree symbol
                sanitized.Append(thisChar);

            }

            return sanitized.ToString();
        }

        private void ProcessOwnshipFromBullseye(float ownshipX, float ownshipY, float bullseyeX, float bullseyeY, float currentHeading)
        {
            float deltaX = ownshipX - bullseyeX;
            float deltaY = ownshipY - bullseyeY;
            double nauticalMile = 6076.11549;

            double distance = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY)) / nauticalMile;
            double heading = ClampDegrees((Math.Atan2(deltaY, deltaX)) * (180 / Math.PI));
            double reciprocal = ClampDegrees(((Math.Atan2(deltaY, deltaX)) * (180 / Math.PI)) + 180);
            double orientation = heading - currentHeading - 180;

            SetValue("Ownship", "deltaX from bulls", new BindingValue(deltaX));
            SetValue("Ownship", "deltaY from bulls", new BindingValue(deltaY));
            SetValue("Ownship", "distance from bullseye", new BindingValue(string.Format("{0:0}", Math.Abs(distance))));
            SetValue("Ownship", "heading from bullseye", new BindingValue(string.Format("{0:0}", heading)));
            SetValue("Ownship", "heading to bullseye", new BindingValue(string.Format("{0:0}", reciprocal)));
            SetValue("Ownship", "orientation to bullseye", new BindingValue(string.Format("{0:0}", orientation)));
        }

        protected void ProcessLightBits(BMSLightBits bits)
        {
            SetValue("Left Eyebrow", "master caution indicator", new BindingValue(bits.HasFlag(BMSLightBits.MasterCaution)));
            SetValue("Left Eyebrow", "tf-fail indicator", new BindingValue(bits.HasFlag(BMSLightBits.TF)));
            SetValue("Right Eyebrow", "oxy low indicator", new BindingValue(bits.HasFlag(BMSLightBits.OXY_BROW)));
            SetValue("Right Eyebrow", "engine fire indicator", new BindingValue(bits.HasFlag(BMSLightBits.ENG_FIRE)));
            SetValue("Right Eyebrow", "hydraulic/oil indicator", new BindingValue(bits.HasFlag(BMSLightBits.HYD)));
            SetValue("Right Eyebrow", "canopy indicator", new BindingValue(bits.HasFlag(BMSLightBits.CAN)));
            SetValue("Right Eyebrow", "takeoff landing config indicator", new BindingValue(bits.HasFlag(BMSLightBits.T_L_CFG)));
            SetValue("Right Eyebrow", "flcs indicator", new BindingValue(bits.HasFlag(BMSLightBits.FLCS)));
            SetValue("Caution", "stores config indicator", new BindingValue(bits.HasFlag(BMSLightBits.CONFIG)));
            SetValue("AOA Indexer", "above indicator", new BindingValue(bits.HasFlag(BMSLightBits.AOAAbove)));
            SetValue("AOA Indexer", "on indicator", new BindingValue(bits.HasFlag(BMSLightBits.AOAOn)));
            SetValue("AOA Indexer", "below indicator", new BindingValue(bits.HasFlag(BMSLightBits.AOABelow)));
            SetValue("Refuel Indexer", "ready indicator", new BindingValue(bits.HasFlag(BMSLightBits.RefuelRDY)));
            SetValue("Refuel Indexer", "air/nws indicator", new BindingValue(bits.HasFlag(BMSLightBits.RefuelAR)));
            SetValue("Refuel Indexer", "disconnect indicator", new BindingValue(bits.HasFlag(BMSLightBits.RefuelDSC)));
            SetValue("Caution", "flight control system indicator", new BindingValue(bits.HasFlag(BMSLightBits.FltControlSys)));
            SetValue("Caution", "leading edge flaps indicator", new BindingValue(bits.HasFlag(BMSLightBits.LEFlaps)));
            SetValue("Caution", "engine fault indticator", new BindingValue(bits.HasFlag(BMSLightBits.EngineFault)));
            SetValue("Caution", "equip hot indicator", new BindingValue(bits.HasFlag(BMSLightBits.EQUIP_HOT)));
            SetValue("Caution", "overheat indicator", new BindingValue(bits.HasFlag(BMSLightBits.Overheat)));
            SetValue("Caution", "low fuel indicator", new BindingValue(bits.HasFlag(BMSLightBits.FuelLow)));
            SetValue("Caution", "avionics indicator", new BindingValue(bits.HasFlag(BMSLightBits.Avionics)));
            SetValue("Caution", "radar altimeter indicator", new BindingValue(bits.HasFlag(BMSLightBits.RadarAlt)));
            SetValue("Caution", "iff indicator", new BindingValue(bits.HasFlag(BMSLightBits.IFF)));
            SetValue("Caution", "ecm indicator", new BindingValue(bits.HasFlag(BMSLightBits.ECM)));
            SetValue("Caution", "hook indicator", new BindingValue(bits.HasFlag(BMSLightBits.Hook)));
            SetValue("Caution", "nws fail indicator", new BindingValue(bits.HasFlag(BMSLightBits.NWSFail)));
            SetValue("Caution", "cabin pressure indicator", new BindingValue(bits.HasFlag(BMSLightBits.CabinPress)));
            SetValue("Autopilot", "on indicator", new BindingValue(bits.HasFlag(BMSLightBits.AutoPilotOn)));
            SetValue("Misc", "tfs stanby indicator", new BindingValue(bits.HasFlag(BMSLightBits.TFR_STBY)));
            SetValue("Test Panel", "FLCS channel lamps", new BindingValue(bits.HasFlag(BMSLightBits.Flcs_ABCD)));
        }

        protected void ProcessLightBits2(BMSLightBits2 bits, BlinkBits blinkBits, int time)
        {
            bool rwrPower = bits.HasFlag(BMSLightBits2.AuxPwr);

            SetValue("Threat Warning Prime", "handoff indicator", new BindingValue(bits.HasFlag(BMSLightBits2.HandOff)));
            SetValue("Threat Warning Prime", "open mode indicator", new BindingValue(bits.HasFlag(BMSLightBits2.AuxPwr) && !bits.HasFlag(BMSLightBits2.PriMode)));
            SetValue("Threat Warning Prime", "naval indicator", new BindingValue(bits.HasFlag(BMSLightBits2.Naval)));
            SetValue("Threat Warning Prime", "target step indicator", new BindingValue(bits.HasFlag(BMSLightBits2.TgtSep)));
            SetValue("Aux Threat Warning", "activity indicator", new BindingValue(bits.HasFlag(BMSLightBits2.AuxAct)));
            SetValue("Aux Threat Warning", "low altitude indicator", new BindingValue(bits.HasFlag(BMSLightBits2.AuxLow)));
            SetValue("Aux Threat Warning", "power indicator", new BindingValue(bits.HasFlag(BMSLightBits2.AuxPwr)));

            SetValue("CMDS", "Go", new BindingValue(bits.HasFlag(BMSLightBits2.Go)));
            SetValue("CMDS", "NoGo", new BindingValue(bits.HasFlag(BMSLightBits2.NoGo)));
            SetValue("CMDS", "Degr", new BindingValue(bits.HasFlag(BMSLightBits2.Degr)));
            SetValue("CMDS", "Rdy", new BindingValue(bits.HasFlag(BMSLightBits2.Rdy)));
            SetValue("CMDS", "ChaffLo", new BindingValue(bits.HasFlag(BMSLightBits2.ChaffLo)));
            SetValue("CMDS", "FlareLo", new BindingValue(bits.HasFlag(BMSLightBits2.FlareLo)));

            SetValue("ECM", "power indicator", new BindingValue(bits.HasFlag(BMSLightBits2.EcmPwr)));
            SetValue("ECM", "fail indicator", new BindingValue(bits.HasFlag(BMSLightBits2.EcmFail)));
            SetValue("Caution", "forward fuel low indicator", new BindingValue(bits.HasFlag(BMSLightBits2.FwdFuelLow)));
            SetValue("Caution", "aft fuel low indicator", new BindingValue(bits.HasFlag(BMSLightBits2.AftFuelLow)));
            SetValue("EPU", "on indicator", new BindingValue(bits.HasFlag(BMSLightBits2.EPUOn)));
            SetValue("JFS", "run indicator", new BindingValue(bits.HasFlag(BMSLightBits2.JFSOn)));
            SetValue("Caution", "second engine compressor indicator", new BindingValue(bits.HasFlag(BMSLightBits2.SEC)));
            SetValue("Caution", "oxygen low indicator", new BindingValue(bits.HasFlag(BMSLightBits2.OXY_LOW)));
            SetValue("Caution", "seat arm indicator", new BindingValue(bits.HasFlag(BMSLightBits2.SEAT_ARM)));
            SetValue("Caution", "backup fuel control indicator", new BindingValue(bits.HasFlag(BMSLightBits2.BUC)));
            SetValue("Caution", "fuel oil hot indicator", new BindingValue(bits.HasFlag(BMSLightBits2.FUEL_OIL_HOT)));
            SetValue("Caution", "anti skid indicator", new BindingValue(bits.HasFlag(BMSLightBits2.ANTI_SKID)));
            SetValue("Misc", "tfs engaged indicator", new BindingValue(bits.HasFlag(BMSLightBits2.TFR_ENGAGED)));

            SetValue("Gear Handle", "handle indicator", new BindingValue(bits.HasFlag(BMSLightBits2.GEARHANDLE)));

            SetValue("Right Eyebrow", "engine indicator", new BindingValue(bits.HasFlag(BMSLightBits2.ENGINE)));

            UpdateBlinkingLightState(bits.HasFlag(BMSLightBits2.PROBEHEAT), blinkBits.HasFlag(BlinkBits.PROBEHEAT), ref _probeheatLastTick, ref _probeheatOnState);
            SetValue("Caution", "probe heat indicator", new BindingValue(_probeheatOnState));

            UpdateBlinkingLightState(bits.HasFlag(BMSLightBits2.AuxSrch), blinkBits.HasFlag(BlinkBits.AuxSrch), ref _auxsrchLastTick, ref _auxsrchOnState);
            SetValue("Aux Threat Warning", "search indicator", new BindingValue(_auxsrchOnState));

            UpdateBlinkingLightState(bits.HasFlag(BMSLightBits2.Launch), blinkBits.HasFlag(BlinkBits.Launch), ref _launchLastTick, ref _launchOnState);
            SetValue("Threat Warning Prime", "launch indicator", new BindingValue(_launchOnState));

            UpdateBlinkingLightState(bits.HasFlag(BMSLightBits2.PriMode), blinkBits.HasFlag(BlinkBits.PriMode), ref _primodeLastTick, ref _primodeOnState);
            SetValue("Threat Warning Prime", "prioirty mode indicator", new BindingValue(_primodeOnState));

            UpdateBlinkingLightState(bits.HasFlag(BMSLightBits2.Unk), blinkBits.HasFlag(BlinkBits.Unk), ref _unkLastTick, ref _unkOnState);
            SetValue("Threat Warning Prime", "unknown mode indicator", new BindingValue(_unkOnState));
        }

        protected void ProcessLightBits3(BMSLightBits3 bits)
        {
            SetValue("Electronic", "flcs pmg indicator", new BindingValue(bits.HasFlag(BMSLightBits3.FlcsPmg)));
            SetValue("Electronic", "main gen indicator", new BindingValue(bits.HasFlag(BMSLightBits3.MainGen)));
            SetValue("Electronic", "standby generator indicator", new BindingValue(bits.HasFlag(BMSLightBits3.StbyGen)));
            SetValue("Electronic", "epu gen indicator", new BindingValue(bits.HasFlag(BMSLightBits3.EpuGen)));
            SetValue("Electronic", "epu pmg indicator", new BindingValue(bits.HasFlag(BMSLightBits3.EpuPmg)));
            SetValue("Electronic", "to flcs indicator", new BindingValue(bits.HasFlag(BMSLightBits3.ToFlcs)));
            SetValue("Electronic", "flcs rly indicator", new BindingValue(bits.HasFlag(BMSLightBits3.FlcsRly)));
            SetValue("Electronic", "bat fail indicator", new BindingValue(bits.HasFlag(BMSLightBits3.BatFail)));
            SetValue("EPU", "hydrazine indicator", new BindingValue(bits.HasFlag(BMSLightBits3.Hydrazine)));
            SetValue("EPU", "air indicator", new BindingValue(bits.HasFlag(BMSLightBits3.Air)));
            SetValue("Caution", "electric bus fail indicator", new BindingValue(bits.HasFlag(BMSLightBits3.Elec_Fault)));
            SetValue("Caution", "lef fault indicator", new BindingValue(bits.HasFlag(BMSLightBits3.Lef_Fault)));
            
            SetValue("Caution", "atf not engaged", new BindingValue(bits.HasFlag(BMSLightBits3.ATF_Not_Engaged)));

            SetValue("General", "on ground", new BindingValue(bits.HasFlag(BMSLightBits3.OnGround)));
            SetValue("Flight Control", "run light", new BindingValue(bits.HasFlag(BMSLightBits3.FlcsBitRun)));
            SetValue("Flight Control", "fail light", new BindingValue(bits.HasFlag(BMSLightBits3.FlcsBitFail)));
            SetValue("Right Eyebrow", "dbu on indicator", new BindingValue(bits.HasFlag(BMSLightBits3.DbuWarn)));
            SetValue("General", "parking brake engaged", new BindingValue(bits.HasFlag(BMSLightBits3.ParkBrakeOn)));
            SetValue("Caution", "cadc indicator", new BindingValue(bits.HasFlag(BMSLightBits3.cadc)));
            SetValue("General", "speed barke", new BindingValue(bits.HasFlag(BMSLightBits3.SpeedBrake)));

            SetValue("Landing Gear", "nose gear indicator", new BindingValue(bits.HasFlag(BMSLightBits3.NoseGearDown)));
            SetValue("Landing Gear", "left gear indicator", new BindingValue(bits.HasFlag(BMSLightBits3.LeftGearDown)));
            SetValue("Landing Gear", "right gear indicator", new BindingValue(bits.HasFlag(BMSLightBits3.RightGearDown)));
            SetValue("General", "power off", new BindingValue(bits.HasFlag(BMSLightBits3.Power_Off)));

            SetValue("Threat Warning Prime", "systest indicator", new BindingValue(bits.HasFlag(BMSLightBits3.SysTest)));

            // BMS 4.35 additions
            SetValue("Caution", "Inlet Icing indicator", new BindingValue(bits.HasFlag(BMSLightBits3.Inlet_Icing)));
        }

        protected void ProcessHsiBits(HsiBits bits, BlinkBits blinkBits)
        {
            SetValue("HSI", "to flag", new BindingValue(bits.HasFlag(HsiBits.ToTrue)));
            SetValue("HSI", "from flag", new BindingValue(bits.HasFlag(HsiBits.FromTrue)));

            SetValue("HSI", "ils warning flag", new BindingValue(bits.HasFlag(HsiBits.IlsWarning)));
            SetValue("HSI", "course warning flag", new BindingValue(bits.HasFlag(HsiBits.CourseWarning)));
            SetValue("HSI", "off flag", new BindingValue(bits.HasFlag(HsiBits.HSI_OFF)));
            SetValue("HSI", "init flag", new BindingValue(bits.HasFlag(HsiBits.Init)));
            SetValue("ADI", "off flag", new BindingValue(bits.HasFlag(HsiBits.ADI_OFF)));
            SetValue("ADI", "aux flag", new BindingValue(bits.HasFlag(HsiBits.ADI_AUX)));
            SetValue("ADI", "gs flag", new BindingValue(bits.HasFlag(HsiBits.ADI_GS)));
            SetValue("ADI", "loc flag", new BindingValue(bits.HasFlag(HsiBits.ADI_LOC)));
            SetValue("Backup ADI", "off flag", new BindingValue(bits.HasFlag(HsiBits.BUP_ADI_OFF)));
            SetValue("VVI", "off flag", new BindingValue(bits.HasFlag(HsiBits.VVI)));
            SetValue("AOA", "off flag", new BindingValue(bits.HasFlag(HsiBits.AOA)));
            SetValue("AVTR", "avtr indicator", new BindingValue(bits.HasFlag(HsiBits.AVTR)));
            SetValue("Runtime", "Flying", new BindingValue(bits.HasFlag(HsiBits.Flying)));

            UpdateBlinkingLightState(bits.HasFlag(HsiBits.OuterMarker), blinkBits.HasFlag(BlinkBits.OuterMarker), ref _outerMarkerLastTick, ref _outerMarkerOnState);
            SetValue("HSI", "Outer marker indicator", new BindingValue(_outerMarkerOnState));

            UpdateBlinkingLightState(bits.HasFlag(HsiBits.MiddleMarker), blinkBits.HasFlag(BlinkBits.MiddleMarker), ref _middleMarkerLastTick, ref _middleMarkerOnState);
            SetValue("HSI", "Middle marker indicator", new BindingValue(_middleMarkerOnState));
        }

        protected void ProcessAltBits(AltBits bits)
        {
            SetValue("Altimeter", "altimeter calibration type", new BindingValue(bits.HasFlag(AltBits.CalType)));
            SetValue("Altimeter", "altimeter pneu flag", new BindingValue(bits.HasFlag(AltBits.PneuFlag)));
        }

        protected void ProcessPowerBits(PowerBits bits)
        {
            SetValue("POWER", "bus power battery", new BindingValue(bits.HasFlag(PowerBits.BusPowerBattery)));
            SetValue("POWER", "bus power emergency", new BindingValue(bits.HasFlag(PowerBits.BusPowerEmergency)));
            SetValue("POWER", "bus power essential", new BindingValue(bits.HasFlag(PowerBits.BusPowerEssential)));
            SetValue("POWER", "bus power non essential", new BindingValue(bits.HasFlag(PowerBits.BusPowerNonEssential)));
            SetValue("POWER", "main generator", new BindingValue(bits.HasFlag(PowerBits.MainGenerator)));
            SetValue("POWER", "standby generator", new BindingValue(bits.HasFlag(PowerBits.StandbyGenerator)));
            SetValue("POWER", "Jetfuel starter", new BindingValue(bits.HasFlag(PowerBits.JetFuelStarter)));
        }
        
        protected void UpdateBlinkingLightState(bool on, bool blinking, ref DateTime lastTick, ref bool onState)
        {
            if (blinking)
            {
                DateTime current = DateTime.Now;
                TimeSpan span = current - lastTick;

                if (span.Milliseconds > SLOW_BLINK_LENGTH_MS)
                {
                    onState = !onState;
                    lastTick = current;
                }
            }
            else
            {
                onState = on;
            }
        }

        ////https://stackoverflow.com/questions/1878907/the-smallest-difference-between-2-angles
        //private int negMod(int a, int n)
        //{
        //    return (a % n + n) % n;
        //}

        private float ClampValue(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private double ClampDegrees(double input)
        {
            while (input < 0d)
            {
                input += 360d;
            }
            while (input > 360d)
            {
                input -= 360d;
            }
            return input;
        }

        internal override void CloseData()
        {
            _sharedMemory.Close();
            _sharedMemory.Dispose();
            _sharedMemory = null;

            _sharedMemory2.Close();
            _sharedMemory2.Dispose();
            _sharedMemory2 = null;
        }

        override internal List<string> NavPoints
        {
            get
            {
                return _navPoints;
            }
        }
        
        override internal RadarContact[] RadarContacts
        {
            get
            {
                return _contacts;
            }
        }

        override internal string[] RwrInfo
        {
            get
            {
                return _rwrInfo;
            }
        }

        internal override bool StringDataUpdated
        {
            get
            {
                return _stringDataUpdated;
            }
        }

        private void ProcessRwrInfo(FlightData2 flightData2)
        {
            string _rwrInfoBuffer = string.Empty;
            for (int i = 0; i < flightData2.RwrInfo.Length; i++)
            {
                _rwrInfoBuffer += (char)flightData2.RwrInfo[i];
            }
            _rwrInfo = _rwrInfoBuffer.Split('<');
        }
        private void ProcessContacts(FlightData flightData)
        {
            for(int i = 0; i < flightData.RWRsymbol.Length; i++)
            {
                _contacts[i].Symbol = (RadarSymbols)flightData.RWRsymbol[i];
                _contacts[i].Selected = flightData.selected[i] > 0;
                _contacts[i].Bearing = flightData.bearing[i] * 57.3f;
                _contacts[i].RelativeBearing = (-flightData.currentHeading + _contacts[i].Bearing) % 360d;
                _contacts[i].Lethality = flightData.lethality[i];
                _contacts[i].MissileActivity = flightData.missileActivity[i] > 0;
                _contacts[i].MissileLaunch = flightData.missileLaunch[i] > 0;
                _contacts[i].NewDetection = flightData.newDetection[i] > 0;
                _contacts[i].Visible = i < flightData.RwrObjectCount;
                _contacts[i].ContactId = i;
            }
        }
    }
}
