﻿//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios.Gauges.F15E.ADI
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    // Annotation Indicating the unique name for this control, display, toolbox group, and control renderer.
    [HeliosControl("Helios.F15E.AoA", "Angle of Attack", "F-15E Strike Eagle", typeof(GaugeRenderer))]
    public class AOA : BaseGauge
    {
        // Value object used to expose angle of attack rendered on the gauge
        private HeliosValue _aoa;
        // Needle object used to update position
        private GaugeNeedle _needle;
        // Calibration scale used to render needle
        private CalibrationPointCollectionDouble _needleCalibration;

        // Base construcor is passed default name and native size
        public AOA()
            : base("Angle of Attack", new Size(364, 376))
        {
            // Components contains all artifacts that are used to draw the gauge, they are drawn in the order they are added to the collection.

            // Add faceplate image to drawing components
            // Source image file (xaml will be vector rendered to appropriate size)
            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/AOA/aoa_faceplate.xaml",
                // Rectangle inside gauge where image will be drawn (scaled automatically to fit rectangle)
                                          new Rect(32d, 38d, 300d, 300d)));

            // Create needle calibration scale which will be used to represent 0 degrees rotation for 0 input and 270 degrees rotation when input is 30.
            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 30d, 270d);

            // Add needle to drawing components
            // Source image file (xaml will be vector rendered to appropriate size)
            _needle = new GaugeNeedle("{Helios}/Gauges/A-10/Common/needle_a.xaml",
                // Location on gauge which the needle will be rotated
                                      new Point(182d, 188d),
                // Size of needle image (will scale image to this size automatically)
                                      new Size(22d, 165d),
                // Center point of needle image to rotate around
                                      new Point(11d, 130d),
                // Initial rotation for this needle
                                      -90d);
            Components.Add(_needle);

            Components.Add(new GaugeImage("{Helios}/Gauges/A-10/Common/gauge_bezel.png", new Rect(0d, 0d, 364d, 376d)));

            // Create Angle of Attack value holder
            // Owning Object
            _aoa = new HeliosValue(this,
                // Default Value
                                   new BindingValue(0d),
                // Device Hint
                                   "",
                // Name
                                   "angle of attack",
                // Description
                                   "Current angle of attack of the aircraft.",
                // Value Description
                                   "(0 - 30)",
                // Value Unit of Measure
                                   BindingValueUnits.Degrees);
            // Hook event callback for when the Angle of Attack value is updated
            _aoa.Execute += new HeliosActionHandler(AOA_Execute);

            // Add angle of attack value into possible action list for bindings
            Actions.Add(_aoa);
        }

        // Event callback for angle updates
        void AOA_Execute(object action, HeliosActionEventArgs e)
        {
            // Interpolate needle rotation based upon angle of attack input
            _needle.Rotation = -_needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
