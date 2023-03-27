//  Copyright 2020 Ammo Goettsch
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

namespace GadrocsWorkshop.Helios.Interfaces.DCS.H60
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.Interfaces.DCS.H60.Functions;
    using GadrocsWorkshop.Helios.Gauges.H60;
    using System;

    public class H60Interface : DCSInterface
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private bool _jsonInterfaceLoaded = false;
        protected H60Interface(string heliosName, string dcsVehicleName, string exportFunctionsUri)
            : base(heliosName, dcsVehicleName, exportFunctionsUri)
        {
        }
        protected void AddFunctions()
        {
            // PILOT BARO ALTIMETER
            AddFunction(new Altimeter(this, FLYER.Pilot));

            // COPILOT ALTIMETER
            AddFunction(new Altimeter(this, FLYER.Copilot));

            #region Network Values

            AddFunction(new SegmentedMeter(this, "2065", 30, "Engine Management", "Fuel Quantity Left", "Bar display of the left fuel quantity"));
            AddFunction(new SegmentedMeter(this, "2066", 30, "Engine Management", "Fuel Quantity Right", "Bar display of the right fuel quantity"));
            AddFunction(new NetworkValue(this, "2060", "Engine Management", "Total Fuel Quantity", "Display of the total fuel amount.", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2067", 30, "Engine Management", "Transmission Temperature", "Bar display of the transmission temp in celsius"));
            AddFunction(new SegmentedMeter(this, "2068", 30, "Engine Management", "Transmission Pressure", "Pressure in transmission in PSI"));
            AddFunction(new SegmentedMeter(this, "2069", 29, "Engine Management", "Engine 1 Oil Temperature", "Bar display of the oil temperature in celsius"));
            AddFunction(new SegmentedMeter(this, "2070", 29, "Engine Management", "Engine 2 Oil Temperature", "Bar display of the oil temperature in celsius"));
            AddFunction(new SegmentedMeter(this, "2071", 30, "Engine Management", "Engine 1 Oil Pressure", "Bar display of the oil pressure in PSI"));
            AddFunction(new SegmentedMeter(this, "2072", 30, "Engine Management", "Engine 2 Oil Pressure", "Bar display of the oil pressure in PSI"));
            AddFunction(new SegmentedMeter(this, "2073", 30, "Engine Management", "Engine 1 TGT", "Bar display of the Turbine Gas Temperature in celsius"));
            AddFunction(new NetworkValue(this, "2061", "Engine Management", "Engine 1 TGT Text", "Display of the Turbine Gas Temperature", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2074", 30, "Engine Management", "Engine 2 TGT", "Bar display of the Turbine Gas Temperature in celsius"));
            AddFunction(new NetworkValue(this, "2062", "Engine Management", "Engine 2 TGT Text", "Display of the Turbine Gas Temperature", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2075", 30, "Engine Management", "Engine 1 Ng", "Bar display of the Gas Generator Speed in RPM"));
            AddFunction(new NetworkValue(this, "2063", "Engine Management", "Engine 1 Ng Text", "Display of the Gas Generator Speed", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2076", 30, "Engine Management", "Engine 2 Ng", "Bar display of the Gas Generator Speed in RPM"));
            AddFunction(new NetworkValue(this, "2064", "Engine Management", "Engine 2 Ng Text", "Display of the Gas Generator Speed", "Text", BindingValueUnits.Text, null));

            AddFunction(new SegmentedMeter(this, "2079", 41, "Engine Management (Pilot)", "Engine 1 RPM", "Bar display of Engine 1 RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2080", 41, "Engine Management (Pilot)", "Rotor RPM", "Bar display of Rotor RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2081", 41, "Engine Management (Pilot)", "Engine 2 RPM", "Bar display of Engine 2 RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2082", 30, "Engine Management (Pilot)", "Engine 1 Torque", "Bar display of Engine 1 Torque (percentage)"));
            AddFunction(new NetworkValue(this, "2077", "Engine Management (Pilot)", "Engine 1 Torque Text", "Display of the engine torque percentage", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2083", 30, "Engine Management (Pilot)", "Engine 2 Torque", "Bar display of Engine 2 Torque (percentage)"));
            AddFunction(new NetworkValue(this, "2078", "Engine Management (Pilot)", "Engine 2 Torque Text", "Display of the engine torque percentage", "Text", BindingValueUnits.Text, null));

            AddFunction(new SegmentedMeter(this, "2086", 41, "Engine Management (Copilot)", "Engine 1 RPM", "Bar display of Engine 1 RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2087", 41, "Engine Management (Copilot)", "Rotor RPM", "Bar display of Rotor RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2088", 41, "Engine Management (Copilot)", "Engine 2 RPM", "Bar display of Engine 2 RPM (percentage)"));
            AddFunction(new SegmentedMeter(this, "2089", 30, "Engine Management (Copilot)", "Engine 1 Torque", "Bar display of Engine 1 Torque (percentage)"));
            AddFunction(new NetworkValue(this, "2084", "Engine Management (Copilot)", "Engine 1 Torque Text", "Display of the engine torque percentage", "Text", BindingValueUnits.Text, null));
            AddFunction(new SegmentedMeter(this, "2090", 30, "Engine Management (Copilot)", "Engine 2 Torque", "Bar display of Engine 2 Torque (percentage)"));
            AddFunction(new NetworkValue(this, "2085", "Engine Management (Copilot)", "Engine 2 Torque Text", "Display of the engine torque percentage", "Text", BindingValueUnits.Text, null));

            //-- PILOT LC6 CHRONOMETER
            AddFunction(new Chronometer(this, "2096", "2097", FLYER.Pilot));

            //-- COPILOT LC6 CHRONOMETER
            AddFunction(new Chronometer(this, "2098", "2099", FLYER.Copilot));

            // Doppler Nav
            AddFunction(new NetworkValue(this, "2091", "Doppler Nav", "Display Line 1", "Nav Computer first display line", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2092", "Doppler Nav", "Display Line 2", "Nav Computer second display line", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2093", "Doppler Nav", "Display Line 3", "Nav Computer third display line", "Text", BindingValueUnits.Text, null));
            AddFunction(new NetworkValue(this, "2094", "Doppler Nav", "Display Line 4", "Nav Computer fourth display line", "Text", BindingValueUnits.Text, null));

            //-- APN-209 Radar Altimeter
            AddFunction(new RADARAltimeter(this, "2055", FLYER.Pilot, "Digital Altitude", "RADAR altitude above ground in feet for digital display."));

            // 174-176 are the altitude digits 
            AddFunction(new RADARAltimeter(this, "2056", FLYER.Copilot, "Digital Altitude", "RADAR altitude above ground in feet for digital display."));

            #endregion

        }
        virtual protected bool JsonInterfaceLoaded { get => _jsonInterfaceLoaded; }
    }
}
