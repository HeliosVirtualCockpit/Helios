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

namespace GadrocsWorkshop.Helios.Gauges.M2000C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("HELIOS.M2000C.ENGINESENSORS_PANEL", "Engine Sensors Panel", "M-2000C Gauges", typeof(BackgroundImageRenderer),HeliosControlFlags.NotShownInUI)]
    class M2000C_ENGINESENSORSPanel : M2000CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 151, 270);
        private string _interfaceDeviceName = "Engine Sensors Panel";
        private Rect _scaledScreenRect = SCREEN_RECT;

        public M2000C_ENGINESENSORSPanel()
            : base("Engine Sensors Panel", new Size(151, 270))
        {
            AddDrum("Engine RPM (%) (Tens)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "tens engine pourcentage", "(0 - 10)", "##",
                new Point(54, 112), new Size(10d, 15d), new Size(12d, 20d));
            AddDrum("Engine RPM (%) (Ones)", "{Helios}/Gauges/M2000C/Common/drum_tape.xaml", "ones engine pourcentage", "(0 - 9)", "#",
                new Point(78, 112), new Size(10d, 15d), new Size(12d, 20d));
            double[,] rpmCalibrationPoints = new double[,] {
                 { 0.5d, -15d },
                 { 0.8d, 60d },
                };
            AddNeedle("Engine RPM Needle", "{Helios}/Gauges/M2000C/Common/needleA.xaml", "engine pourcentage", "(0 - 100)", 
                new Point(78, 70), new Size(10d, 70d), new Point(5d, 60d), BindingValueUnits.RPMPercent, new double[] { 0d, -135d, 100d, 109d }, rpmCalibrationPoints);
            AddNeedle("Engine T7 Needle", "{Helios}/Gauges/M2000C/Common/needleB.xaml", "engine temperature", "(0 - 10)",
                new Point(80, 230), new Size(10d, 60d), new Point(5d, 50d), BindingValueUnits.Numeric, new double[] { 0d, -110d, 10d, 107d });
        }

        #region Properties

        public override string DefaultBackgroundImage
        {
            get { return "{M2000C}/Images/EngineSensorsPanel/engine-sensors-panel.png"; }
        }

        #endregion

        private void AddDrum(string name, string gaugeImage, string actionIdentifier, string valueDescription, string format, Point posn, Size size, Size renderSize)
        {
            AddDrumGauge(name: name,
                gaugeImage: gaugeImage,
                posn: posn,
                size: size,
                renderSize: renderSize,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                actionIdentifier: actionIdentifier,
                valueDescription: valueDescription,
                format: format,
                fromCenter: false);
        }

        private void AddNeedle(string name, string needleImage, string actionIdentifier, string valueDescription, 
            Point posn, Size size, Point centerPoint, BindingValueUnit typeValue, double[] initialCalibration, double[,] calibrationPoints = null)
        {
            AddNeedle(name: name,
                needleImage: needleImage,
                posn: posn,
                size: size,
                centerPoint: centerPoint,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: name,
                actionIdentifier: actionIdentifier,
                valueDescription: valueDescription,
                typeValue: typeValue,
                initialCalibration: initialCalibration,
                calibrationPoints: null,
                fromCenter: false);
        }

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Width") || args.PropertyName.Equals("Height"))
            {
                double scaleX = Width / NativeSize.Width;
                double scaleY = Height / NativeSize.Height;
                _scaledScreenRect.Scale(scaleX, scaleY);
            }
            base.OnPropertyChanged(args);
        }

        public override bool HitTest(Point location)
        {
            if (_scaledScreenRect.Contains(location))
            {
                return false;
            }

            return true;
        }

        public override void MouseDown(Point location)
        {
            // No-Op
        }

        public override void MouseDrag(Point location)
        {
            // No-Op
        }

        public override void MouseUp(Point location)
        {
            // No-Op
        }
    }
}
